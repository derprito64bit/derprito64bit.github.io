<#
.SYNOPSIS
  One-command Windows runner for [project]ion: project setup, EditMode/PlayMode tests, Web build, local server,
  and warm Library copies for extra worktrees.

.DESCRIPTION
  Runs Unity in batch mode with the editor version pinned in ProjectSettings/ProjectVersion.txt (override with
  $env:UNITY_EXE). Logs go to Build/logs/<step>.log and test results to Build/results-<platform>.xml. Exits
  non-zero when Unity fails or a test fails. Only one Unity process may have the project open: the runner
  refuses to start while Temp/UnityLockfile is held (close the editor, or use the Unity MCP tools instead).

  Unity slots: at most N batch-mode Unity processes run at once across every checkout and worktree on this
  machine (N = $env:ION_UNITY_SLOTS, default 2). Each Unity run holds one of the lock files
  %LOCALAPPDATA%\ion\unity-slots\slot<i>.lock until Unity exits; build, build-dev and snapshot-library first
  take build.intent (so new test runs queue behind them) and then hold every slot. Waiting runs poll every
  2 s, print one line per minute naming the holders, and give up after -SlotTimeoutMinutes (default 90) with
  exit code 75. A run that dies releases its slots with its process, so slots never leak.

  After setup, builds and test runs the runner restores regenerated files that differ from git only in
  fileIDs or line endings (ProjectSetup and WebBuild rewrite Main.unity and the settings assets on every
  run); pass -KeepChurn to keep them.

  snapshot-library mirrors a project's Library (default: this checkout) to
  %LOCALAPPDATA%\ion\library-snapshot\<project folder name>\Library and records snapshot.json next to it.
  seed-library mirrors a snapshot into another checkout's Library so its first Unity run starts warm; by
  default it uses the snapshot named after the target repository's main checkout. Both refuse while a Unity
  process has the project open.

.EXAMPLE
  powershell -File scripts/ion.ps1 setup -Twice     # fresh checkout (BUILD.md: run setup twice the first time)
  powershell -File scripts/ion.ps1 test             # EditMode, then PlayMode (needs a GPU; not -nographics)
  powershell -File scripts/ion.ps1 test-edit -Filter MeshClipperTests
  powershell -File scripts/ion.ps1 build; powershell -File scripts/ion.ps1 serve -Port 8080
.EXAMPLE
  $env:ION_UNITY_SLOTS = 1; powershell -File scripts/ion.ps1 test -SlotTimeoutMinutes 30
  powershell -File scripts/ion.ps1 slot-test        # 3 simulated runs; fails if more than N overlap
.EXAMPLE
  powershell -File scripts/ion.ps1 snapshot-library                         # in the main checkout, editor closed
  powershell -File scripts/ion.ps1 seed-library -Target .claude\worktrees\my-task
  powershell -File scripts/ion.ps1 seed-library -Target D:\wt -Snapshot D:\snapshots\project.ion
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('setup', 'test-edit', 'test-play', 'test', 'build', 'build-dev', 'serve', 'unity-path',
        'snapshot-library', 'seed-library', 'slot-test')]
    [string]$Command = 'test',
    [switch]$Twice,
    [switch]$KeepChurn,
    [string]$Filter,
    [int]$Port = 8080,
    [int]$SlotTimeoutMinutes = 90,
    [string]$Source,
    [string]$Snapshot,
    [string]$Target,
    # slot-test's child processes: hold a slot for 6 s instead of running Unity.
    [Parameter(DontShow)]
    [switch]$SimulateUnity
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
$Logs = Join-Path $Root 'Build\logs'
$SnapshotRoot = Join-Path $env:LOCALAPPDATA 'ion\library-snapshot'
# slot-test uses its own slot set, so it never waits behind (or delays) real Unity runs.
$SlotDir = Join-Path $env:LOCALAPPDATA $(if ($SimulateUnity) { 'ion\unity-slots-test' } else { 'ion\unity-slots' })
$SlotCount = 2
if ($env:ION_UNITY_SLOTS) { $SlotCount = [int]$env:ION_UNITY_SLOTS }
if ($SlotCount -lt 1) { throw "ION_UNITY_SLOTS must be 1 or more (got '$env:ION_UNITY_SLOTS')." }

function Get-UnityExe {
    if ($env:UNITY_EXE) {
        if (-not (Test-Path $env:UNITY_EXE)) { throw "UNITY_EXE points to a missing file: $env:UNITY_EXE" }
        return $env:UNITY_EXE
    }
    $line = Select-String -Path (Join-Path $Root 'ProjectSettings\ProjectVersion.txt') -Pattern '^m_EditorVersion:\s*(\S+)' | Select-Object -First 1
    $version = $line.Matches[0].Groups[1].Value
    $hubRoots = @("$env:ProgramFiles\Unity\Hub\Editor")
    $secondary = Join-Path $env:APPDATA 'UnityHub\secondaryInstallPath.json'
    if (Test-Path $secondary) {
        $extra = (Get-Content $secondary -Raw).Trim().Trim('"')
        if ($extra) { $hubRoots += $extra }
    }
    foreach ($r in $hubRoots) {
        $exe = Join-Path $r "$version\Editor\Unity.exe"
        if (Test-Path $exe) { return $exe }
    }
    throw ("Unity $version is not installed. Install it with Web Build Support:`n" +
        "  & `"$env:ProgramFiles\Unity Hub\resources\unity.exe`" install $version --cm -m webgl")
}

# Returns the project's Temp/UnityLockfile opened exclusively (so Unity cannot open the project while it is
# held), or $null when there is no lockfile. Throws when a Unity process holds it.
function Open-ProjectLock([string]$Project) {
    $lock = Join-Path $Project 'Temp\UnityLockfile'
    if (-not (Test-Path $lock)) { return $null }
    try { return [IO.File]::Open($lock, 'Open', 'ReadWrite', 'None') }
    catch { throw "Another Unity process has $Project open (Temp/UnityLockfile is locked). Close it first." }
}

function Assert-ProjectFree {
    $fs = Open-ProjectLock $Root
    if ($fs) { $fs.Close() }
}

# --- Unity slots --------------------------------------------------------------------------------------------

# Returns the lock file opened exclusively, or $null while another process holds it. -Probe opens it shared
# instead: that fails only while someone holds it exclusively, and concurrent probes never block each other.
function Open-Lock([string]$Path, [switch]$Probe) {
    try {
        if ($Probe) { return [IO.File]::Open($Path, 'OpenOrCreate', 'Read', 'ReadWrite') }
        return [IO.File]::Open($Path, 'OpenOrCreate', 'ReadWrite', 'None')
    }
    catch [IO.IOException] { return $null }
}

function Get-Holders([string[]]$Paths) {
    $names = foreach ($p in $Paths) {
        $who = 'unknown'
        try { $who = [IO.File]::ReadAllText("$p.holder").Trim() } catch { }
        '{0} ({1})' -f [IO.Path]::GetFileName($p), $who
    }
    return ($names -join '; ')
}

# Waits for a Unity slot (or, with -All, for build.intent and then every slot) and returns the held lock
# streams. Closing them, or this process exiting, frees the slots.
function Enter-UnitySlots([string]$Purpose, [switch]$All) {
    New-Item -ItemType Directory -Force $SlotDir | Out-Null
    $intent = Join-Path $SlotDir 'build.intent'
    $slots = @(0..($SlotCount - 1) | ForEach-Object { Join-Path $SlotDir "slot$_.lock" })
    $want = if ($All) { "all $SlotCount Unity slots" } else { 'a Unity slot' }
    $holder = 'pid {0} {1} {2} since {3:HH:mm}' -f $PID, $Purpose, $Root, (Get-Date)
    $utf8 = New-Object Text.UTF8Encoding $false
    $held = [ordered]@{}
    $sw = [Diagnostics.Stopwatch]::StartNew()
    $reported = -1
    try {
        while ($true) {
            $got = $null
            if ($All) {
                $busy = @()
                foreach ($path in @($intent) + $slots) {
                    if ($held.Contains($path)) { continue }
                    $fs = Open-Lock $path
                    if (-not $fs) {
                        $busy += $path
                        if ($path -eq $intent) { break }   # another build is queued first
                        continue
                    }
                    $held[$path] = $fs
                    try { [IO.File]::WriteAllText("$path.holder", $holder, $utf8) } catch { }   # a waiter may be reading it
                }
                if ($busy.Count -eq 0) { $got = @($held.Values) }
            }
            else {
                # A queued build holds build.intent; new runs wait behind it instead of taking freed slots.
                $busy = @($intent)
                $gate = Open-Lock $intent -Probe
                if ($gate) {
                    $gate.Close()
                    $busy = $slots
                    foreach ($path in $slots) {
                        $fs = Open-Lock $path
                        if ($fs) {
                            try { [IO.File]::WriteAllText("$path.holder", $holder, $utf8) } catch { }   # a waiter may be reading it
                            $got = @($fs)
                            break
                        }
                    }
                }
            }
            if ($got) {
                if ($reported -ge 0) { Write-Host ('got {0} after {1:n1} min' -f $want, $sw.Elapsed.TotalMinutes) }
                return $got
            }
            $min = [int][math]::Floor($sw.Elapsed.TotalMinutes)
            if ($min -ge $SlotTimeoutMinutes) {
                Write-Host ("Gave up after $SlotTimeoutMinutes min waiting for $want (holders: $(Get-Holders $busy)). " +
                    'Try again later, or pass -SlotTimeoutMinutes.')
                exit 75   # held locks close with the process
            }
            if ($min -gt $reported) {
                $reported = $min
                Write-Host "waiting for ${want}: $min min, holders: $(Get-Holders $busy)"
            }
            Start-Sleep -Seconds 2
        }
    }
    catch {
        foreach ($fs in $held.Values) { $fs.Close() }
        throw
    }
}

function Exit-UnitySlots($Locks) {
    foreach ($fs in @($Locks)) { if ($fs) { $fs.Close() } }
}

# --- Unity runs ---------------------------------------------------------------------------------------------

function Invoke-Unity([string]$Name, [string[]]$UnityArgs, [switch]$AllSlots) {
    if (-not $SimulateUnity) { Assert-ProjectFree }   # fail fast instead of after waiting for a slot
    $locks = Enter-UnitySlots $Name -All:$AllSlots
    try {
        if ($SimulateUnity) {
            [Console]::WriteLine("SIM acquire $([Diagnostics.Stopwatch]::GetTimestamp())")
            Start-Sleep -Seconds 6
            [Console]::WriteLine("SIM release $([Diagnostics.Stopwatch]::GetTimestamp())")
            return 0
        }
        Assert-ProjectFree   # again: the project may have been opened while this run waited
        New-Item -ItemType Directory -Force $Logs | Out-Null
        $log = Join-Path $Logs "$Name.log"
        $argList = @('-batchmode', '-projectPath', "`"$Root`"", '-logFile', "`"$log`"") + $UnityArgs
        $sw = [Diagnostics.Stopwatch]::StartNew()
        # Wait for the editor only. Start-Process -Wait also waits for Unity's lingering build-server
        # children (dotnet ILPP/bee), which outlive the editor.
        $p = Start-Process -FilePath (Get-UnityExe) -ArgumentList $argList -PassThru -NoNewWindow
        $null = $p.Handle   # cache the handle so ExitCode is readable after exit
        $p.WaitForExit()
        $sw.Stop()
        Write-Host ("{0,-10} exit={1}  {2:n1} min  {3}" -f $Name, $p.ExitCode, $sw.Elapsed.TotalMinutes, $log)
        return $p.ExitCode
    }
    finally { Exit-UnitySlots $locks }
}

function Show-TestResults([string]$Xml) {
    if (-not (Test-Path $Xml)) { Write-Host "No results file: $Xml"; return 1 }
    [xml]$doc = Get-Content $Xml
    $run = $doc.'test-run'
    Write-Host ("{0}: total={1} passed={2} failed={3} skipped={4} ({5:n0}s)" -f (Split-Path $Xml -Leaf), $run.total, $run.passed, $run.failed, $run.skipped, [double]$run.duration)
    $doc.SelectNodes("//test-case[@result='Failed']") | ForEach-Object {
        Write-Host "  FAILED: $($_.fullname)"
        $msg = $_.failure.message.'#cdata-section'
        if ($msg) { Write-Host ('          ' + ($msg -split "`n" | Select-Object -First 2) -join ' ') }
    }
    return [int]$run.failed
}

function Invoke-Tests([string]$Platform) {
    $xml = Join-Path $Root "Build\results-$($Platform.ToLower()).xml"
    if (Test-Path $xml) { Remove-Item $xml }
    $a = @('-runTests', '-testPlatform', $Platform, '-testResults', "`"$xml`"")
    if ($Platform -eq 'EditMode') { $a = @('-nographics') + $a }
    if ($Filter) { $a += @('-testFilter', $Filter) }
    $code = Invoke-Unity $Platform.ToLower() $a
    $failed = Show-TestResults $xml
    if ($code -ne 0 -or $failed -gt 0) { return 1 }
    return 0
}

function Normalize-Yaml([string]$Text) {
    $t = $Text -replace "`r`n", "`n"
    $t = [regex]::Replace($t, 'fileID: -?\d+', 'fileID: N')
    return [regex]::Replace($t, '(--- !u!\d+ &)-?\d+', '${1}N')
}

function Restore-SetupChurn {
    Push-Location $Root
    try {
        # 'git status' also lists files Unity rewrote with LF while core.autocrlf expects CRLF; those have no
        # content diff at all and are restored straight away.
        $changed = git status --porcelain | Where-Object { $_ -match '^ M ' } | ForEach-Object { $_.Substring(3) } |
            Where-Object { $_ -match '\.(unity|asset|lighting|prefab)$' }
        foreach ($f in $changed) {
            git diff --quiet -- $f
            if ($LASTEXITCODE -eq 0) {
                git restore -- $f
                Write-Host "  restored $f (line-ending churn only)"
                continue
            }
            $head = (git show "HEAD:$f") -join "`n"
            $work = Get-Content -Raw $f
            if ((Normalize-Yaml $head).TrimEnd() -eq (Normalize-Yaml $work).TrimEnd()) {
                git restore -- $f
                Write-Host "  restored $f (fileID / line-ending churn only)"
            }
            else { Write-Host "  kept     $f (real setting changes)" }
        }
    }
    finally { Pop-Location }
}

function Exit-Clean([int]$Code) {
    # Setup, builds and test runs all re-apply project settings, which rewrites tracked assets.
    if (-not $KeepChurn) { Restore-SetupChurn }
    exit $Code
}

# --- Library snapshots --------------------------------------------------------------------------------------

function Get-FullPath([string]$Path) {
    return $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Path)
}

# Runs git in $Dir and returns its trimmed output, or $null when git fails (for example: not a repository).
function Get-GitValue([string]$Dir, [string[]]$GitArgs) {
    $ErrorActionPreference = 'Continue'   # Windows PowerShell turns native stderr into errors under 'Stop'
    $out = git -C $Dir @GitArgs 2>$null
    if ($LASTEXITCODE -ne 0) { return $null }
    return "$out".Trim()
}

function Invoke-Robocopy([string]$From, [string]$To) {
    robocopy $From $To /MIR /MT:8 /R:1 /W:1 /NFL /NDL /NP | Out-Host
    if ($LASTEXITCODE -ge 8) { throw "robocopy failed with exit code ${LASTEXITCODE}: $From -> $To" }
}

switch ($Command) {
    'unity-path' { Get-UnityExe; exit 0 }
    'setup' {
        $passes = if ($Twice) { 2 } else { 1 }
        for ($i = 1; $i -le $passes; $i++) {
            $code = Invoke-Unity "setup$i" @('-nographics', '-quit', '-executeMethod', 'Ion.EditorTools.ProjectSetup.Run')
            if ($code -ne 0) { Exit-Clean $code }
            Select-String -Path (Join-Path $Logs "setup$i.log") -Pattern '\[Ion\]' | ForEach-Object { Write-Host ('  ' + $_.Line.Trim()) }
        }
        Exit-Clean 0
    }
    'test-edit' { Exit-Clean (Invoke-Tests 'EditMode') }
    'test-play' { Exit-Clean (Invoke-Tests 'PlayMode') }
    'test' {
        $e = Invoke-Tests 'EditMode'
        $p = Invoke-Tests 'PlayMode'
        Exit-Clean ([int]($e -ne 0 -or $p -ne 0))
    }
    'build' { Exit-Clean (Invoke-Unity 'build' @('-nographics', '-quit', '-buildTarget', 'WebGL', '-executeMethod', 'Ion.EditorTools.WebBuild.Build') -AllSlots) }
    'build-dev' { Exit-Clean (Invoke-Unity 'build-dev' @('-nographics', '-quit', '-buildTarget', 'WebGL', '-executeMethod', 'Ion.EditorTools.WebBuild.BuildDev') -AllSlots) }
    'serve' {
        $dir = Join-Path $Root 'Build\Web'
        if (-not (Test-Path (Join-Path $dir 'index.html'))) { throw "No build at $dir. Run: powershell -File scripts/ion.ps1 build" }
        Write-Host "Serving $dir at http://localhost:$Port/  (Ctrl+C to stop)"
        python -m http.server $Port --bind 127.0.0.1 --directory $dir
    }
    'snapshot-library' {
        $src = Get-FullPath $(if ($Source) { $Source } else { $Root })
        $lib = Join-Path $src 'Library'
        if (-not (Test-Path $lib)) { throw "No Library folder at $lib." }
        $snap = if ($Snapshot) { Get-FullPath $Snapshot } else { Join-Path $SnapshotRoot (Split-Path $src -Leaf) }
        $meta = Join-Path $snap 'snapshot.json'
        # Hold every slot so no runner starts Unity mid-copy, and the project lock so no editor opens it.
        $locks = Enter-UnitySlots 'snapshot-library' -All
        try {
            $projectLock = Open-ProjectLock $src
            try {
                # Without snapshot.json the snapshot counts as missing, so a failed copy is never seeded.
                if (Test-Path $meta) { Remove-Item $meta }
                Invoke-Robocopy $lib (Join-Path $snap 'Library')
                $bytes = (Get-ChildItem -LiteralPath (Join-Path $snap 'Library') -Recurse -File -Force | Measure-Object Length -Sum).Sum
                $json = [ordered]@{
                    source     = $src
                    sourceHead = Get-GitValue $src @('rev-parse', 'HEAD')
                    createdUtc = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
                    sizeBytes  = [long]$bytes
                } | ConvertTo-Json
                [IO.File]::WriteAllText($meta, $json, (New-Object Text.UTF8Encoding $false))
            }
            finally { if ($projectLock) { $projectLock.Close() } }
        }
        finally { Exit-UnitySlots $locks }
        Write-Host "Snapshot written to $snap"
        Write-Host $json
        exit 0
    }
    'seed-library' {
        if (-not $Target) { throw 'seed-library needs -Target <worktree or project folder>.' }
        $dst = Get-FullPath $Target
        if (-not (Test-Path (Join-Path $dst 'ProjectSettings'))) { throw "$dst is not a Unity project (no ProjectSettings folder)." }
        if ($Snapshot) { $snap = Get-FullPath $Snapshot }
        else {
            # Worktrees share one main checkout; snapshot-library run there names the snapshot after it.
            $common = Get-GitValue $dst @('rev-parse', '--path-format=absolute', '--git-common-dir')
            $main = if ($common) { Split-Path -Parent $common } else { $dst }
            $snap = Join-Path $SnapshotRoot (Split-Path $main -Leaf)
        }
        $meta = Join-Path $snap 'snapshot.json'
        if (-not (Test-Path $meta)) {
            $have = @(Get-ChildItem -LiteralPath $SnapshotRoot -Directory -ErrorAction SilentlyContinue | ForEach-Object { $_.Name })
            throw ("No library snapshot at $snap (snapshot.json missing). Run snapshot-library first or pass -Snapshot. " +
                "Snapshots in ${SnapshotRoot}: $(if ($have) { $have -join ', ' } else { 'none' }).")
        }
        $used = [IO.File]::ReadAllText($meta)
        $projectLock = Open-ProjectLock $dst
        try { Invoke-Robocopy (Join-Path $snap 'Library') (Join-Path $dst 'Library') }
        finally { if ($projectLock) { $projectLock.Close() } }
        $now = if (Test-Path $meta) { [IO.File]::ReadAllText($meta) } else { '' }
        if ($now -ne $used) { throw "The snapshot at $snap changed while seeding. Run seed-library again." }
        Write-Host "Seeded $dst\Library from $snap"
        Write-Host $used
        exit 0
    }
    'slot-test' {
        if ($SimulateUnity) { exit (Invoke-Unity 'slot-sim' @()) }
        # Three children compete for $SlotCount slots; each logs when it got its slot and when it let go.
        $dir = Join-Path ([IO.Path]::GetTempPath()) "ion-slot-test-$PID"
        New-Item -ItemType Directory -Force $dir | Out-Null
        $exe = (Get-Process -Id $PID).Path
        $kids = foreach ($i in 1..3) {
            $out = Join-Path $dir "child$i.out"
            $p = Start-Process -FilePath $exe -PassThru -NoNewWindow -RedirectStandardOutput $out -RedirectStandardError "$out.err" `
                -ArgumentList @('-NoProfile', '-File', "`"$PSCommandPath`"", 'slot-test', '-SimulateUnity')
            $null = $p.Handle
            [pscustomobject]@{ Name = "child$i"; Proc = $p; Out = $out }
        }
        $events = @()
        $bad = @()
        foreach ($k in $kids) {
            $k.Proc.WaitForExit()
            $lines = @(Get-Content $k.Out) + @(Get-Content "$($k.Out).err")
            $lines | Where-Object { $_ } | ForEach-Object { Write-Host "  $($k.Name): $_" }
            $marks = @(foreach ($l in $lines) {
                    if ($l -match '^SIM (acquire|release) (\d+)$') {
                        # At equal timestamps count the release first: it was logged before its lock closed.
                        [pscustomobject]@{ Name = $k.Name; Kind = $Matches[1]; Time = [long]$Matches[2]; Order = $(if ($Matches[1] -eq 'release') { 0 } else { 1 }) }
                    }
                })
            if ($k.Proc.ExitCode -ne 0 -or $marks.Count -ne 2) { $bad += "$($k.Name) (exit $($k.Proc.ExitCode))" }
            $events += $marks
        }
        Remove-Item -Recurse -Force $dir
        $t0 = ($events | Measure-Object Time -Minimum).Minimum
        $now = 0
        $max = 0
        foreach ($e in ($events | Sort-Object Time, Order)) {
            if ($e.Kind -eq 'acquire') { $now++ } else { $now-- }
            if ($now -gt $max) { $max = $now }
            Write-Host ('  {0,6:n2} s  {1} {2}' -f (($e.Time - $t0) / [Diagnostics.Stopwatch]::Frequency), $e.Name, $e.Kind)
        }
        Write-Host "slot-test: max overlap $max of 3 runs, $SlotCount slots"
        if ($bad) { Write-Host "slot-test FAILED: child runs failed: $($bad -join ', ')"; exit 1 }
        if ($max -gt $SlotCount) { Write-Host "slot-test FAILED: $max runs overlapped, limit is $SlotCount"; exit 1 }
        Write-Host 'slot-test passed'
        exit 0
    }
}

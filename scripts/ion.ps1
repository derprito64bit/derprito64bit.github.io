<#
.SYNOPSIS
  One-command Windows runner for [project]ion: project setup, EditMode/PlayMode tests, Web build, local server.

.DESCRIPTION
  Runs Unity in batch mode with the editor version pinned in ProjectSettings/ProjectVersion.txt (override with
  $env:UNITY_EXE). Logs go to Build/logs/<step>.log and test results to Build/results-<platform>.xml. Exits
  non-zero when Unity fails or a test fails. Only one Unity process may have the project open: the runner
  refuses to start while Temp/UnityLockfile is held (close the editor, or use the Unity MCP tools instead).

  After setup, builds and test runs the runner restores regenerated files that differ from git only in
  fileIDs or line endings (ProjectSetup and WebBuild rewrite Main.unity and the settings assets on every
  run); pass -KeepChurn to keep them.

.EXAMPLE
  powershell -File scripts/ion.ps1 setup -Twice     # fresh checkout (BUILD.md: run setup twice the first time)
  powershell -File scripts/ion.ps1 test             # EditMode, then PlayMode (needs a GPU; not -nographics)
  powershell -File scripts/ion.ps1 test-edit -Filter MeshClipperTests
  powershell -File scripts/ion.ps1 build; powershell -File scripts/ion.ps1 serve -Port 8080
#>
[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('setup', 'test-edit', 'test-play', 'test', 'build', 'build-dev', 'serve', 'unity-path')]
    [string]$Command = 'test',
    [switch]$Twice,
    [switch]$KeepChurn,
    [string]$Filter,
    [int]$Port = 8080
)

$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
$Logs = Join-Path $Root 'Build\logs'

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

function Assert-ProjectFree {
    $lock = Join-Path $Root 'Temp\UnityLockfile'
    if (-not (Test-Path $lock)) { return }
    try { $fs = [IO.File]::Open($lock, 'Open', 'ReadWrite', 'None'); $fs.Close() }
    catch { throw 'Another Unity process has this project open (Temp/UnityLockfile is locked). Close it first.' }
}

function Invoke-Unity([string]$Name, [string[]]$UnityArgs) {
    Assert-ProjectFree
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
    'build' { Exit-Clean (Invoke-Unity 'build' @('-nographics', '-quit', '-buildTarget', 'WebGL', '-executeMethod', 'Ion.EditorTools.WebBuild.Build')) }
    'build-dev' { Exit-Clean (Invoke-Unity 'build-dev' @('-nographics', '-quit', '-buildTarget', 'WebGL', '-executeMethod', 'Ion.EditorTools.WebBuild.BuildDev')) }
    'serve' {
        $dir = Join-Path $Root 'Build\Web'
        if (-not (Test-Path (Join-Path $dir 'index.html'))) { throw "No build at $dir. Run: powershell -File scripts/ion.ps1 build" }
        Write-Host "Serving $dir at http://localhost:$Port/  (Ctrl+C to stop)"
        python -m http.server $Port --bind 127.0.0.1 --directory $dir
    }
}

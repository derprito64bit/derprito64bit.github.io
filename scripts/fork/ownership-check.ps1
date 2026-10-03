<#
.SYNOPSIS
  Fails when a branch changes a file outside the crew's owned globs (-Owned). -Fork adds the old upstream-file rule;
  -Seam fails fork-only files.

.DESCRIPTION
  Lists the files changed in Base...Head (from the merge base; both sides of a rename count) and classifies each:
    fork-only  Assets/Portfolio/**, site/**, scripts/fork/**, docs/portfolio/**, docs/FORK.md
    exception  an approved fork edit to an upstream file (ownership-exceptions.txt next to this script)
    seam       an exception whose reason starts with 'seam': a file changed by a merged up/* branch
    upstream   everything else; the art-bible section 10 Lead is shown when the path maps to one
  A .meta file is classified like the file or folder it describes.

  Violations:
    default    (standalone, D-022) only -Owned is enforced: the project is no longer a fork, so engine files are ours
    -Fork      the old fork rule: upstream files (not exception or seam) are violations
    -Seam      fork-only files (for up/* branches, which may change upstream files)
    -Owned     any file that matches none of the given globs (the crew's issue lists them)
  Globs: ** any depth, * within one folder, ? one character, {a,b} alternatives; case-insensitive.
  Exits 0 when there are no violations, 1 otherwise. -Json prints one machine-readable object instead.

.EXAMPLE
  powershell -File scripts/fork/ownership-check.ps1 -Base origin/main -Owned 'Assets/Portfolio/Runtime/Camera/**','docs/portfolio/camera.md'
  powershell -File scripts/fork/ownership-check.ps1 -Seam -Base upstream/main -Head up/lens -Json
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Base,
    [string]$Head = 'HEAD',
    [switch]$Seam,
    [switch]$Fork,
    [string[]]$Owned,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'

$forkOnly = @('Assets/Portfolio/**', 'site/**', 'scripts/fork/**', 'docs/portfolio/**', 'docs/FORK.md')

# Art-bible section 10, first match wins (Lead D's two carve-outs come before A's Quality and C's PlayMode folders).
$leads = @(
    @{ Lead = 'Lead D'; Globs = @('Assets/Scripts/Presentation/Quality/SettingsPanel.cs', 'Assets/Tests/PlayMode/{RewindTests,SwitchTests}.cs') },
    @{ Lead = 'Lead A'; Globs = @('Assets/Shaders/**', 'Assets/Scripts/Presentation/{Palette,Surface,Atmosphere,ZoneMood,Backdrop}.cs',
            'Assets/Scripts/Presentation/{Ambience,Quality}/**', 'Assets/Scripts/Levels/Arch/**', 'Assets/Scripts/Levels/{Geo,DecorCombiner}.cs',
            'Assets/Scripts/Projection/{MeshClipper,MeshElements,PieceMerger,ProjectionSystem}.cs', 'Assets/Tests/EditMode/**', 'Assets/Editor/**') },
    @{ Lead = 'Lead B'; Globs = @('Assets/Scripts/Levels/Props/**', 'Assets/Scripts/Levels/{LevelProps,Kit,Scatter}.cs') },
    @{ Lead = 'Lead C'; Globs = @('Assets/Scripts/Levels/Rooms/**', 'Assets/Scripts/Levels/{Room,GameBootstrap}.cs', 'Assets/Scripts/DebugTools/**', 'Assets/Tests/PlayMode/**') },
    @{ Lead = 'Lead D'; Globs = @('Assets/Scripts/Gameplay/**', 'Assets/Scripts/Presentation/Motion/**', 'Assets/WebGLTemplates/**', 'Assets/Plugins/WebGL/**',
            'Assets/Scripts/Presentation/{Hud,PhotoOverlayUI,Crosshair,ClickToPlayOverlay,Onboarding,ScreenFx,FreshPulse,EndCard,UIFactory,UIUtil,IonCanvasScaler}.cs') },
    @{ Lead = 'Lead E'; Globs = @('Assets/Scripts/Presentation/Audio/**', 'Assets/Resources/Audio/**', 'tools/audio/**') }
)

function ConvertTo-GlobRegex([string]$Glob) {
    $sb = New-Object System.Text.StringBuilder '^'
    $i = 0
    while ($i -lt $Glob.Length) {
        $c = $Glob[$i]
        $rest = $Glob.Substring($i)
        if ($rest.StartsWith('**/')) { [void]$sb.Append('(?:.*/)?'); $i += 3; continue }
        if ($rest -eq '/**') { [void]$sb.Append('(?:/.*)?'); $i += 3; continue }
        if ($rest.StartsWith('**')) { [void]$sb.Append('.*'); $i += 2; continue }
        switch ($c) {
            '*' { [void]$sb.Append('[^/]*') }
            '?' { [void]$sb.Append('[^/]') }
            '{' { [void]$sb.Append('(?:') }
            '}' { [void]$sb.Append(')') }
            ',' { [void]$sb.Append('|') }
            default { [void]$sb.Append([regex]::Escape([string]$c)) }
        }
        $i++
    }
    return $sb.Append('$').ToString()
}

# True when the path, or the file/folder a .meta describes, matches one of the regexes.
function Test-GlobMatch([string]$Path, [string[]]$Regexes) {
    $subjects = @($Path)
    if ($Path.EndsWith('.meta')) { $subjects += $Path.Substring(0, $Path.Length - 5) }
    foreach ($s in $subjects) { foreach ($re in $Regexes) { if ($s -match $re) { return $true } } }
    return $false
}

function Get-Lead([string]$Path) {
    foreach ($l in $leads) {
        if (Test-GlobMatch $Path @($l.Globs | ForEach-Object { ConvertTo-GlobRegex $_ })) { return $l.Lead }
    }
    return ''
}

# Exceptions: '<glob>  # <reason>' per line; a reason is required.
$exceptions = @()
$exceptionsFile = Join-Path $PSScriptRoot 'ownership-exceptions.txt'
$n = 0
foreach ($line in [IO.File]::ReadAllLines($exceptionsFile)) {
    $n++
    $t = $line.Trim()
    if (-not $t -or $t.StartsWith('#')) { continue }
    $hash = $t.IndexOf('#')
    if ($hash -lt 1 -or -not $t.Substring($hash + 1).Trim()) { throw "ownership-exceptions.txt:${n}: '$t' needs a '# reason'" }
    $reason = $t.Substring($hash + 1).Trim()
    $kind = 'exception'
    if ($reason -match '^seam\b') { $kind = 'seam' }
    $exceptions += @{ Regex = (ConvertTo-GlobRegex $t.Substring(0, $hash).Trim()); Kind = $kind; Reason = $reason }
}

# -Owned arrives as one comma-joined string under 'powershell -File'; split on commas outside {...}.
$ownedGlobs = @($Owned | ForEach-Object { $_ -split ',(?![^{]*\})' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
$ownedRegexes = @($ownedGlobs | ForEach-Object { ConvertTo-GlobRegex $_ })
$forkRegexes = @($forkOnly | ForEach-Object { ConvertTo-GlobRegex $_ })

$diff = & git.exe -c core.quotepath=off diff --name-status -M "$Base...$Head"
if ($LASTEXITCODE -ne 0) { throw "git diff $Base...$Head failed (exit $LASTEXITCODE)" }

$files = @()
foreach ($row in @($diff | Where-Object { $_ })) {
    $parts = $row -split "`t"
    $status = $parts[0].Substring(0, 1)
    foreach ($path in @($parts | Select-Object -Skip 1)) {
        $class = 'upstream'; $note = ''
        if (Test-GlobMatch $path $forkRegexes) { $class = 'fork-only' }
        else {
            foreach ($e in $exceptions) {
                if (Test-GlobMatch $path @($e.Regex)) { $class = $e.Kind; $note = $e.Reason; break }
            }
        }
        $lead = ''
        if ($class -ne 'fork-only') { $lead = Get-Lead $path }

        $problems = @()
        if ($Seam -and $class -eq 'fork-only') { $problems += 'fork-only path on a seam branch' }
        if ($Fork -and -not $Seam -and $class -eq 'upstream') {
            $who = 'its owner'
            if ($lead) { $who = $lead }
            $problems += "upstream-owned: request it from $who (type:request) or build a seam"
        }
        if ($ownedRegexes.Count -gt 0 -and -not (Test-GlobMatch $path $ownedRegexes)) { $problems += 'outside -Owned globs' }

        $files += New-Object PSObject -Property ([ordered]@{
                path = $path; status = $status; class = $class; lead = $lead; note = $note
                violation = ($problems.Count -gt 0); problems = $problems
            })
    }
}

$counts = [ordered]@{}
foreach ($c in 'fork-only', 'exception', 'seam', 'upstream') { $counts[$c] = @($files | Where-Object { $_.class -eq $c }).Count }
$violations = @($files | Where-Object { $_.violation })
$mode = 'standalone'
if ($Fork) { $mode = 'fork' }
if ($Seam) { $mode = 'seam' }

if ($Json) {
    $result = [ordered]@{
        base = $Base; head = $Head; mode = $mode; owned = $ownedGlobs
        pass = ($violations.Count -eq 0); counts = $counts; violations = $violations.Count; files = $files
    }
    Write-Output (ConvertTo-Json -InputObject $result -Depth 5)
}
else {
    $ownedText = ''
    if ($ownedGlobs.Count -gt 0) { $ownedText = "  owned: $($ownedGlobs -join ', ')" }
    Write-Output "ownership-check $Base...$Head  mode: $mode$ownedText"
    # Quiet rows (allowed fork-only files) are only counted; everything else gets a line.
    foreach ($f in $files) {
        if (-not $f.violation -and $f.class -eq 'fork-only') { continue }
        $flag = 'ok  '
        if ($f.violation) { $flag = 'FAIL' }
        $detail = @()
        if ($f.lead) { $detail += $f.lead }
        if ($f.note) { $detail += $f.note }
        $detail += $f.problems
        Write-Output ('  {0} {1,-9} {2,-56} {3}' -f $flag, $f.class, $f.path, ($detail -join ' | '))
    }
    $summary = ($counts.Keys | ForEach-Object { "$($_): $($counts[$_])" }) -join '  '
    $verdict = 'PASS'
    if ($violations.Count -gt 0) { $verdict = 'FAIL' }
    Write-Output "$($files.Count) paths  $summary  violations: $($violations.Count)  $verdict"
}

if ($violations.Count -gt 0) { exit 1 }
exit 0

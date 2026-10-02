<#
.SYNOPSIS
  Copies the portfolio content from the portfolio-site repo into the Manor's Resources.

.DESCRIPTION
  Source file          ->  Assets/Portfolio/Resources/Portfolio/
    projects.json      ->  projects.json   (required: an object with a "projects" array of {slug, title, ...})
    awards.json        ->  awards.json     (optional)
    tokens.json        ->  identity.json   (required)

  Every file must parse as JSON with an object at the top level (JsonUtility cannot read a bare array).
  -Check compares only (line endings ignored) and copies nothing.

  Exit codes: 0 copied or in sync, 1 -Check found differences, 2 source folder or required file missing,
  3 invalid JSON.

.EXAMPLE
  powershell -File scripts/fork/sync-content.ps1 -Check
  powershell -File scripts/fork/sync-content.ps1
  powershell -File scripts/fork/sync-content.ps1 -Source D:\portfolio-site\content
#>
[CmdletBinding()]
param(
    [string]$Source,
    [string]$Destination,
    [switch]$Check
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if (-not $Source) {
    $common = [string](& git.exe -C $root rev-parse --path-format=absolute --git-common-dir)
    if ($LASTEXITCODE -ne 0) { throw 'Not a git repository.' }
    $Source = Join-Path (Split-Path -Parent (Split-Path -Parent $common.Trim())) 'portfolio-site\content'
}
if (-not $Destination) { $Destination = Join-Path $root 'Assets\Portfolio\Resources\Portfolio' }

if (-not (Test-Path -LiteralPath $Source -PathType Container)) {
    Write-Host "No content folder at $Source. Clone portfolio-site next to this repo or pass -Source."
    exit 2
}

$map = @(
    @{ From = 'projects.json'; To = 'projects.json'; Required = $true },
    @{ From = 'awards.json'; To = 'awards.json'; Required = $false },
    @{ From = 'tokens.json'; To = 'identity.json'; Required = $true }
)

# ---- Validate every source file before touching anything
$files = @()
$problems = @()
foreach ($e in $map) {
    $src = Join-Path $Source $e.From
    if (-not (Test-Path -LiteralPath $src -PathType Leaf)) {
        if ($e.Required) { Write-Host "Missing required $($e.From) in $Source"; exit 2 }
        continue
    }
    $text = [IO.File]::ReadAllText($src)
    try { $json = $text | ConvertFrom-Json } catch { $problems += "$($e.From): not valid JSON ($($_.Exception.Message))"; continue }
    if ($text.TrimStart([char]0xFEFF, ' ', "`t", "`r", "`n").StartsWith('{') -eq $false) {
        $problems += "$($e.From): the top level must be an object, not an array or value"
        continue
    }
    if ($e.From -eq 'projects.json') {
        $list = $json.PSObject.Properties['projects']
        if (-not $list -or -not ($list.Value -is [array])) { $problems += 'projects.json: "projects" must be an array' }
        else {
            $i = 0
            foreach ($p in $list.Value) {
                if (-not ($p -is [pscustomobject])) { $problems += "projects.json: projects[$i] is not an object" }
                elseif (-not $p.slug -or -not $p.title) { $problems += "projects.json: projects[$i] needs a slug and a title" }
                $i++
            }
        }
    }
    $files += [pscustomobject]@{ From = $e.From; To = $e.To; Src = $src; Dst = (Join-Path $Destination $e.To); Text = $text }
}
if ($problems.Count -gt 0) { $problems | ForEach-Object { Write-Host "  ! $_" }; Write-Host 'Nothing copied.'; exit 3 }

# ---- Compare (line endings ignored: git normalizes them)
$changed = 0
foreach ($f in $files) {
    if (-not (Test-Path -LiteralPath $f.Dst)) { $state = 'new' }
    elseif (([IO.File]::ReadAllText($f.Dst) -replace "`r`n", "`n") -ceq ($f.Text -replace "`r`n", "`n")) { $state = 'same' }
    else { $state = 'differs' }
    if ($state -ne 'same') { $changed++ }
    Write-Host ('  {0,-8} {1} -> {2}' -f $state, $f.From, $f.To)
}

if ($Check) {
    if ($changed -gt 0) { Write-Host "$changed file(s) out of sync. Run without -Check to copy."; exit 1 }
    Write-Host 'In sync.'
    exit 0
}
New-Item -ItemType Directory -Force $Destination | Out-Null
foreach ($f in $files) { Copy-Item -LiteralPath $f.Src -Destination $f.Dst -Force }
Write-Host "Copied $($files.Count) file(s) to $Destination ($changed changed)."

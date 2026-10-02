<#
.SYNOPSIS
  Publishes the portfolio to the fork's GitHub Pages: site/ at the root, the Unity Web build (Build/Web) at /play/.

.DESCRIPTION
  Clones the gh-pages branch into a temp folder, replaces its contents, commits and pushes (a normal
  fast-forward push, never a force push). Run scripts/ion.ps1 build first. -DryRun stops before the push and
  prints what would change.

.EXAMPLE
  powershell -File scripts/fork/publish.ps1
  powershell -File scripts/fork/publish.ps1 -Message "Deploy the Manor" -DryRun
#>
[CmdletBinding()]
param(
    [string]$Message = 'Deploy site',
    [string]$Remote = 'https://github.com/derprito64bit/derprito64bit.github.io.git',
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$web = Join-Path $root 'Build\Web'
$site = Join-Path $root 'site'
if (-not (Test-Path (Join-Path $web 'index.html'))) { throw 'No Web build in Build/Web. Run: powershell -File scripts/ion.ps1 build' }
if (-not (Test-Path (Join-Path $site 'index.html'))) { throw 'No site/index.html.' }

$work = Join-Path ([IO.Path]::GetTempPath()) 'ion-pages-publish'
if (Test-Path $work) { Remove-Item -Recurse -Force $work }
& git.exe clone --quiet --depth 1 --branch gh-pages $Remote $work
if ($LASTEXITCODE -ne 0) { throw 'Could not clone the gh-pages branch.' }

Get-ChildItem $work -Force | Where-Object { $_.Name -ne '.git' } | Remove-Item -Recurse -Force
Copy-Item -Recurse -Force (Join-Path $site '*') $work
New-Item -ItemType Directory -Force (Join-Path $work 'play') | Out-Null
Copy-Item -Recurse -Force (Join-Path $web '*') (Join-Path $work 'play')
New-Item -ItemType File -Force (Join-Path $work '.nojekyll') | Out-Null

$sha = (& git.exe -C $root rev-parse --short HEAD).Trim()
& git.exe -C $work add -A
& git.exe -C $work -c core.autocrlf=false commit -q -m "$Message ($sha)" -m 'Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>'
if ($LASTEXITCODE -ne 0) { Write-Host 'Nothing changed; nothing to publish.'; exit 0 }
& git.exe -C $work show --stat --oneline HEAD | Select-Object -First 25

if ($DryRun) { Write-Host "Dry run: not pushed. Inspect $work"; exit 0 }
& git.exe -C $work push -q origin gh-pages
if ($LASTEXITCODE -ne 0) { throw 'Push to gh-pages failed.' }
Write-Host 'Published. GitHub Pages rebuilds in a minute or two: https://derprito64bit.github.io/'

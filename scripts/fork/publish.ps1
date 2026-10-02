<#
.SYNOPSIS
  Composes the whole derprito64bit.github.io domain and publishes it to the fork's gh-pages branch.

.DESCRIPTION
  Layout of the published tree:
    /          the 2D site: portfolio-site's dist/, or the fork's site/ landing page until that build exists
    /manor/    the Unity Web build (Build/Web), with a phone guard and robots noindex injected into index.html
    /play/     a redirect stub to /manor/ that keeps the query string and hash (old links)
    /arcade/   the shared HTML5 demos from site/arcade/ (the Manor's cabinet opens ../arcade/<slug>/)
    .nojekyll  (404.html only when the site provides one)

  Clones gh-pages (shallow) into a temp folder, wipes everything but .git, composes the tree, checks every
  relative link, commits and pushes (a normal fast-forward push, never a force push). Broken links stop the
  publish before the commit unless -AllowBroken. -DryRun commits in the temp clone only and prints the change.

.PARAMETER SiteDist
  The root site. Default: ..\portfolio-site\dist next to this repository when it has an index.html, else the
  fork's site/ (without its arcade folder). Top-level manor/, play/ and arcade/ in it are never published.

.PARAMETER NoManor
  Do not publish a new Unity build: keep the manor/ that gh-pages already has (no Build/Web needed).

.EXAMPLE
  powershell -File scripts/fork/publish.ps1 -DryRun
  powershell -File scripts/fork/publish.ps1 -Message "Deploy the Manor"
  powershell -File scripts/fork/publish.ps1 -NoManor -Message "Update the landing page"
#>
[CmdletBinding()]
param(
    [string]$Message = 'Deploy site',
    [string]$Remote = 'https://github.com/derprito64bit/derprito64bit.github.io.git',
    [string]$Build,
    [string]$SiteDist,
    [switch]$NoManor,
    [switch]$DryRun,
    [switch]$AllowBroken
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$forkSite = Join-Path $root 'site'
$reserved = @('manor', 'play', 'arcade', '.git')
$utf8 = New-Object System.Text.UTF8Encoding $false

function Write-Utf8([string]$Path, [string]$Text) { [IO.File]::WriteAllText($Path, $Text, $utf8) }

# Copies the entries of $From into $To, skipping top-level names in $Skip.
function Copy-Contents([string]$From, [string]$To, [string[]]$Skip) {
    New-Item -ItemType Directory -Force $To | Out-Null
    Get-ChildItem -LiteralPath $From -Force | Where-Object { $Skip -notcontains $_.Name } | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $To $_.Name) -Recurse -Force
    }
}

# True when every segment of $Path below $Base exists with exactly that case (GitHub Pages is case-sensitive,
# Windows is not).
function Test-ExactCase([string]$Base, [string]$Path) {
    $dir = $Base
    foreach ($seg in $Path.Substring($Base.Length).Split([char[]]'\', [StringSplitOptions]::RemoveEmptyEntries)) {
        $names = @([IO.Directory]::GetFileSystemEntries($dir) | ForEach-Object { [IO.Path]::GetFileName($_) })
        if (-not ($names -ccontains $seg)) { return $false }
        $dir = Join-Path $dir $seg
    }
    return $true
}

# ---- Inputs
if (-not $Build) { $Build = Join-Path $root 'Build\Web' }
if (-not $SiteDist) {
    $common = [string](& git.exe -C $root rev-parse --path-format=absolute --git-common-dir)
    if ($LASTEXITCODE -ne 0) { throw 'Not a git repository.' }
    $dist = Join-Path (Split-Path -Parent (Split-Path -Parent $common.Trim())) 'portfolio-site\dist'
    if (Test-Path (Join-Path $dist 'index.html')) { $SiteDist = $dist } else { $SiteDist = $forkSite }
}
$SiteDist = (Resolve-Path $SiteDist).Path.TrimEnd('\')
if (-not (Test-Path (Join-Path $SiteDist 'index.html'))) { throw "No index.html in the site: $SiteDist" }
if (-not $NoManor -and -not (Test-Path (Join-Path $Build 'index.html'))) {
    throw "No Web build in $Build. Run: powershell -File scripts/ion.ps1 build (or pass -NoManor to keep the published Manor)"
}
$skipped = Get-ChildItem -LiteralPath $SiteDist -Force | Where-Object { $reserved -contains $_.Name } | ForEach-Object { $_.Name }
if ($skipped -and $SiteDist -ne $forkSite) { Write-Warning "Not published from the site (reserved paths): $($skipped -join ', ')" }
Write-Host "Site:  $SiteDist"
if ($NoManor) { Write-Host 'Manor: kept from gh-pages' } else { Write-Host "Manor: $Build" }

# ---- Clone gh-pages and wipe it
$work = Join-Path ([IO.Path]::GetTempPath()) 'ion-pages-publish'
if (Test-Path $work) { Remove-Item -Recurse -Force $work }
& git.exe -c core.autocrlf=false clone --quiet --depth 1 --branch gh-pages $Remote $work
if ($LASTEXITCODE -ne 0) { throw 'Could not clone the gh-pages branch.' }
$keep = @('.git')
if ($NoManor) { $keep += 'manor' }
Get-ChildItem $work -Force | Where-Object { $keep -notcontains $_.Name } | Remove-Item -Recurse -Force

# ---- Compose
Copy-Contents $SiteDist $work $reserved
if (-not $NoManor) { Copy-Contents $Build (Join-Path $work 'manor') @() }
if (Test-Path (Join-Path $forkSite 'arcade')) { Copy-Contents (Join-Path $forkSite 'arcade') (Join-Path $work 'arcade') @() }
New-Item -ItemType Directory -Force (Join-Path $work 'play') | Out-Null
Write-Utf8 (Join-Path $work 'play\index.html') @'
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<title>The Manor has moved</title>
<meta name="robots" content="noindex">
<link rel="canonical" href="/manor/">
<script>location.replace('/manor/' + location.search + location.hash);</script>
<meta http-equiv="refresh" content="0; url=/manor/">
</head>
<body>
<p>The Manor has moved to <a href="/manor/">/manor/</a>.</p>
</body>
</html>
'@
Write-Utf8 (Join-Path $work '.nojekyll') ''

# ---- Inject into manor/index.html (idempotent: the marker attribute is checked first)
$manorIndex = Join-Path $work 'manor\index.html'
if (Test-Path $manorIndex) {
    $html = [IO.File]::ReadAllText($manorIndex)
    if ($html.Contains('data-ion-publish')) {
        Write-Host 'manor/index.html: already injected'
    }
    else {
        $head = [regex]::Match($html, '<head(\s[^>]*)?>', 'IgnoreCase')
        if (-not $head.Success) { throw 'manor/index.html has no <head>.' }
        $inject = "`n" + '<meta name="robots" content="noindex" data-ion-publish="injected">' + "`n" +
            '<script data-ion-publish="phone-guard">(function(){if(/[?&]force=1(&|$)/.test(location.search))return;' +
            'if(window.matchMedia&&matchMedia("(pointer: coarse)").matches&&Math.min(screen.width,screen.height)<820)' +
            'location.replace("/?from=manor");})();</script>'
        $at = $head.Index + $head.Length
        Write-Utf8 $manorIndex ($html.Substring(0, $at) + $inject + $html.Substring($at))
        Write-Host 'manor/index.html: injected phone guard and robots noindex'
    }
}

# ---- Link check: every relative or root-relative href/src in the staged HTML must resolve
$broken = @()
$workFull = (Resolve-Path $work).Path.TrimEnd('\')
Get-ChildItem $work -Recurse -File -Filter *.html | Where-Object { $_.FullName -notlike "$workFull\.git\*" } | ForEach-Object {
    $page = $_
    $rel = $page.FullName.Substring($workFull.Length + 1).Replace('\', '/')
    $text = [IO.File]::ReadAllText($page.FullName)
    foreach ($m in [regex]::Matches($text, '\s(?:href|src)\s*=\s*(?:"([^"]*)"|''([^'']*)'')', 'IgnoreCase')) {
        $url = $m.Groups[1].Value + $m.Groups[2].Value
        if ($url -eq '' -or $url.StartsWith('#') -or $url.StartsWith('//') -or $url.Contains('{{')) { continue }
        if ($url -match '^[a-zA-Z][a-zA-Z0-9+.-]*:') { continue }   # http:, https:, mailto:, data:, javascript:, ...
        $path = [Uri]::UnescapeDataString(($url -split '[?#]', 2)[0])
        if ($path -eq '') { continue }
        if ($path.StartsWith('/')) { $target = Join-Path $workFull $path.TrimStart('/') }
        else { $target = Join-Path $page.DirectoryName $path }
        $target = [IO.Path]::GetFullPath($target)
        $ok = $false
        if ($target.TrimEnd('\') -eq $workFull -or $target.StartsWith("$workFull\")) {
            if (Test-Path -LiteralPath $target -PathType Leaf) { $ok = -not $path.EndsWith('/') }
            elseif (Test-Path -LiteralPath $target -PathType Container) { $target = Join-Path $target 'index.html'; $ok = Test-Path -LiteralPath $target }
            if ($ok) { $ok = Test-ExactCase $workFull $target }
        }
        if (-not $ok) { $broken += [pscustomobject]@{ Page = $rel; Url = $url } }
    }
}

Write-Host ''
Write-Host "Staged: $work"
Get-ChildItem $work -Force | Where-Object { $_.Name -ne '.git' } | Sort-Object Name | ForEach-Object {
    if ($_.PSIsContainer) { Write-Host ('  {0,-12} {1} files' -f ($_.Name + '/'), @(Get-ChildItem $_.FullName -Recurse -File).Count) }
    else { Write-Host "  $($_.Name)" }
}
Write-Host ''
if ($broken.Count -gt 0) {
    Write-Host "Broken links: $($broken.Count)"
    $broken | Group-Object Page | ForEach-Object {
        Write-Host "  $($_.Name)"
        $_.Group | ForEach-Object { Write-Host "    $($_.Url)" }
    }
    if (-not $AllowBroken) { Write-Host 'Not published. Fix the links or pass -AllowBroken.'; exit 1 }
}
else { Write-Host 'Links: all relative links resolve.' }

# ---- Commit and push
$sha = ([string](& git.exe -C $root rev-parse --short HEAD)).Trim()
# autocrlf=input stores text files with LF, as gh-pages has them, so CRLF working copies add no diff noise.
& git.exe -C $work -c core.autocrlf=input -c core.safecrlf=false add -A
if ($LASTEXITCODE -ne 0) { throw 'git add failed in the staged tree.' }
& git.exe -C $work diff --cached --quiet
if ($LASTEXITCODE -eq 0) { Write-Host 'Nothing changed; nothing to publish.'; exit 0 }
& git.exe -C $work commit -q -m "$Message ($sha)"
if ($LASTEXITCODE -ne 0) { throw 'Commit failed in the staged tree.' }
& git.exe -C $work show --stat --oneline HEAD | Select-Object -First 30

if ($DryRun) { Write-Host "Dry run: not pushed. Inspect $work"; exit 0 }
& git.exe -C $work push -q origin gh-pages
if ($LASTEXITCODE -ne 0) { throw 'Push to gh-pages failed.' }
Write-Host 'Published. GitHub Pages rebuilds in a minute or two: https://derprito64bit.github.io/'

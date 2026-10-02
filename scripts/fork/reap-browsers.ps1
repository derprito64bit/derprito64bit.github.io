<#
.SYNOPSIS
  Kills automation browsers (playwright-cli / Playwright / Lighthouse temp profiles) that agents left running.

.DESCRIPTION
  Only touches Chrome/Edge processes whose --user-data-dir is a temporary automation profile
  (playwright_chromiumdev_profile-*, lighthouse.*). The owner's own browser profile is never touched.
  -OlderThanMinutes keeps young sessions alive (a running agent may still be using them).

.EXAMPLE
  powershell -File scripts/fork/reap-browsers.ps1                   # report only
  powershell -File scripts/fork/reap-browsers.ps1 -Kill -OlderThanMinutes 30
#>
[CmdletBinding()]
param(
    [switch]$Kill,
    [int]$OlderThanMinutes = 0
)
$cut = (Get-Date).AddMinutes(-$OlderThanMinutes)
$rows = Get-CimInstance Win32_Process -Filter "Name='chrome.exe' OR Name='msedge.exe'" | ForEach-Object {
    $cl = $_.CommandLine
    if ($cl -match '--user-data-dir="?([^" ]*(playwright_chromiumdev_profile-|lighthouse\.)[^" ]*)') {
        [pscustomobject]@{ Pid = $_.ProcessId; Profile = Split-Path $matches[1] -Leaf; Start = $_.CreationDate }
    }
}
$groups = $rows | Group-Object Profile | ForEach-Object {
    $first = ($_.Group | Sort-Object Start | Select-Object -First 1).Start
    [pscustomobject]@{ Profile = $_.Name; Procs = $_.Count; Started = $first; Old = ($first -lt $cut); Pids = $_.Group.Pid }
}
if (-not $groups) { 'no automation browsers running'; exit 0 }
$groups | Sort-Object Started | Format-Table Profile, Procs, @{n='Started';e={$_.Started.ToString('HH:mm:ss')}}, Old -AutoSize | Out-String
if ($Kill) {
    $n = 0
    foreach ($g in $groups | Where-Object Old) { foreach ($p in $g.Pids) { try { Stop-Process -Id $p -Force -ErrorAction Stop; $n++ } catch { } } }
    "killed $n processes in $(@($groups | Where-Object Old).Count) stale automation profile(s)"
}

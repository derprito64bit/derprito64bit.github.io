<#
.SYNOPSIS
  Lists or kills automation browsers (playwright-cli / Playwright / Lighthouse) and orphaned harness servers that
  agents left running.

.DESCRIPTION
  Only touches automation processes. The owner's own browsers, profiles and terminals are never touched:
  - chrome.exe / msedge.exe whose --user-data-dir is a temporary automation profile
    (playwright_chromiumdev_profile-*, lighthouse.*);
  - Playwright's own browsers, by executable path under %LOCALAPPDATA%\ms-playwright: chrome-headless-shell.exe,
    chrome.exe, Playwright.exe, WebKitWebProcess.exe, WebKitGPUProcess.exe, WebKitNetworkProcess.exe, firefox.exe;
  - node.exe servers listening on a harness port (-Ports; default 4322, scripts/serve-dist.mjs, and 4391,
    scripts/crew.mjs, both in portfolio-site). One whose parent process is gone is orphaned. A server whose parent is
    still alive belongs to a running suite: it is listed, never killed.
  Processes are grouped per browser instance (its profile, or its top-most matched ancestor) or per server. A group is
  Old when it started more than -OlderThanMinutes ago (a running agent may still be using a young one). -Kill stops
  every process of each Old group, except live harness servers.

  -List writes the groups as JSON to stdout and never kills. portfolio-site's scripts/fleet/hostload.ps1 reads it.

.EXAMPLE
  powershell -File scripts/fork/reap-browsers.ps1                   # report only
  powershell -File scripts/fork/reap-browsers.ps1 -List             # JSON, never kills
  powershell -File scripts/fork/reap-browsers.ps1 -Kill -OlderThanMinutes 30
#>
[CmdletBinding()]
param(
    [switch]$Kill,
    [switch]$List,
    [int]$OlderThanMinutes = 0,
    [int[]]$Ports = @(4322, 4391)
)
if ($Kill -and $List) { throw '-List never kills: pass -Kill or -List, not both.' }
$cut = (Get-Date).AddMinutes(-$OlderThanMinutes)
$pwRoot = (Join-Path $env:LOCALAPPDATA 'ms-playwright') + '\'
$pwNames = 'chrome-headless-shell.exe', 'chrome.exe', 'Playwright.exe', 'WebKitWebProcess.exe', 'WebKitGPUProcess.exe',
    'WebKitNetworkProcess.exe', 'firefox.exe'

$all = @(Get-CimInstance Win32_Process)
$byPid = @{}
foreach ($p in $all) { $byPid[[int]$p.ProcessId] = $p }
function Test-Playwright($p) {
    $exe = [string]$p.ExecutablePath
    return ($exe -and $exe.StartsWith($pwRoot, [StringComparison]::OrdinalIgnoreCase) -and ($pwNames -contains $p.Name))
}
# The parent, when it is still the process that started this one (a reused PID is younger than its "child").
function Get-LiveParent($p) {
    $par = $byPid[[int]$p.ParentProcessId]
    if ($par -and $par.CreationDate -le $p.CreationDate) { return $par }
    return $null
}

$rows = New-Object System.Collections.Generic.List[object]
foreach ($p in $all) {
    $cl = [string]$p.CommandLine
    if (($p.Name -eq 'chrome.exe' -or $p.Name -eq 'msedge.exe') -and
        $cl -match '--user-data-dir="?([^" ]*(playwright_chromiumdev_profile-|lighthouse\.)[^" ]*)') {
        $rows.Add([pscustomobject]@{ Kind = 'automation-profile'; Group = (Split-Path $matches[1] -Leaf); Pid = [int]$p.ProcessId
                Name = $p.Name; Start = $p.CreationDate; Orphaned = $null; Port = $null })
    } elseif (Test-Playwright $p) {
        $root = $p
        while ($true) {
            $par = Get-LiveParent $root
            if ($par -and (Test-Playwright $par)) { $root = $par } else { break }
        }
        $dir = ([string]$p.ExecutablePath).Substring($pwRoot.Length).Split('\')[0]
        $rows.Add([pscustomobject]@{ Kind = 'ms-playwright'; Group = "$dir pid $($root.ProcessId)"; Pid = [int]$p.ProcessId
                Name = $p.Name; Start = $p.CreationDate; Orphaned = $null; Port = $null })
    }
}
$listeners = @(Get-NetTCPConnection -State Listen -LocalPort $Ports -ErrorAction SilentlyContinue |
        Select-Object LocalPort, OwningProcess -Unique)
foreach ($l in $listeners) {
    $p = $byPid[[int]$l.OwningProcess]
    if (-not $p -or $p.Name -ne 'node.exe') { continue }
    $rows.Add([pscustomobject]@{ Kind = 'harness-server'; Group = "node port $($l.LocalPort) pid $($p.ProcessId)"
            Pid = [int]$p.ProcessId; Name = $p.Name; Start = $p.CreationDate; Orphaned = (-not (Get-LiveParent $p))
            Port = [int]$l.LocalPort })
}

$groups = @($rows | Group-Object Kind, Group | ForEach-Object {
        $g = $_.Group
        $first = ($g | Sort-Object Start | Select-Object -First 1).Start
        $orphaned = $g[0].Orphaned
        [pscustomobject]@{
            Kind     = $g[0].Kind
            Group    = $g[0].Group
            Procs    = $g.Count
            Started  = $first
            Old      = ($first -lt $cut)
            Orphaned = $orphaned
            # A live harness server belongs to a running suite and is never killed.
            Killable = (($first -lt $cut) -and ($g[0].Kind -ne 'harness-server' -or $orphaned))
            Pids     = @($g | ForEach-Object { $_.Pid })
            Names    = @($g | ForEach-Object { $_.Name } | Sort-Object -Unique)
        }
    } | Sort-Object Started)

if ($List) {
    $out = [pscustomobject]@{
        tool             = 'reap-browsers.ps1'
        at               = (Get-Date).ToString('o')
        olderThanMinutes = $OlderThanMinutes
        ports            = @($Ports)
        groups           = @($groups | ForEach-Object {
                [pscustomobject]@{ kind = $_.Kind; group = $_.Group; procs = $_.Procs; started = $_.Started.ToString('o')
                    old = $_.Old; orphaned = $_.Orphaned; killable = $_.Killable; pids = @($_.Pids); names = @($_.Names) }
            })
    }
    $out | ConvertTo-Json -Depth 5
    exit 0
}

if (-not $groups.Count) { 'no automation browsers or harness servers running'; exit 0 }
$groups | Format-Table Kind, Group, Procs, @{n = 'Started'; e = { $_.Started.ToString('HH:mm:ss') } }, Old, Orphaned -AutoSize |
    Out-String -Width 200
if ($Kill) {
    $n = 0
    $targets = @($groups | Where-Object Killable)
    foreach ($g in $targets) { foreach ($p in $g.Pids) { try { Stop-Process -Id $p -Force -ErrorAction Stop; $n++ } catch { } } }
    "killed $n processes in $($targets.Count) stale group(s)"
}

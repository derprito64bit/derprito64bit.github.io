<#
.SYNOPSIS
  Reports what changed in the collaboration repo (remote 'upstream') since the last reviewed commit.

.DESCRIPTION
  Fetches upstream and lists the new commits on upstream/main. It also shows changed files per area with the
  art-bible Lead that owns them, and flags files that changed both upstream and on the current branch, which
  are likely merge conflicts. Read-only, except for -MarkSeen, which records the reviewed head in the git dir.

.EXAMPLE
  powershell -File scripts/fork/upstream-check.ps1
  powershell -File scripts/fork/upstream-check.ps1 -MarkSeen
  powershell -File scripts/fork/upstream-check.ps1 -Since 823d94e
#>
[CmdletBinding()]
param(
    [string]$Since,
    [switch]$MarkSeen,
    [string]$Remote = 'upstream',
    [string]$Branch = 'main'
)

$ErrorActionPreference = 'Stop'

function Invoke-Git([Parameter(ValueFromRemainingArguments)][string[]]$GitArgs) {
    $out = & git.exe @GitArgs
    if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') failed: $out" }
    return $out
}

$commonDir = (Invoke-Git rev-parse --git-common-dir | Select-Object -First 1).Trim()
$stateFile = Join-Path $commonDir 'ion-upstream-seen'
$target = "$Remote/$Branch"

Invoke-Git fetch $Remote --quiet | Out-Null
$head = (Invoke-Git rev-parse $target | Select-Object -First 1).Trim()

if (-not $Since) {
    if (Test-Path $stateFile) { $Since = (Get-Content $stateFile -Raw).Trim() }
    else { $Since = (Invoke-Git merge-base HEAD $target | Select-Object -First 1).Trim() }
}
$sinceFull = (Invoke-Git rev-parse $Since | Select-Object -First 1).Trim()

Write-Output "Upstream: $target = $($head.Substring(0, 7))   last reviewed: $($sinceFull.Substring(0, 7))"

if ($sinceFull -eq $head) {
    Write-Output 'No new upstream commits.'
}
else {
    Write-Output ''
    Write-Output '== New commits'
    Invoke-Git log --format='%h %ad %an | %s' --date=format:'%Y-%m-%d %H:%M' "$sinceFull..$head" | ForEach-Object { Write-Output "  $_" }

    Write-Output ''
    Write-Output '== Size'
    Write-Output ('  ' + ((Invoke-Git diff --shortstat $sinceFull $head) -join ' ').Trim())

    # Art-bible section 10 ownership, first match wins.
    $owners = @(
        # Exceptions first: SettingsPanel and the Rewind/Switch suites belong to Lead D, audio code to Lead E.
        @{ Re = '^Assets/Scripts/Presentation/Quality/SettingsPanel\.cs|^Assets/Tests/PlayMode/(RewindTests|SwitchTests)\.cs'; Lead = 'Lead D (feel, state, UI, web shell)' },
        @{ Re = '^Assets/Scripts/Presentation/Audio/'; Lead = 'Lead E (audio)' },
        @{ Re = '^Assets/Shaders/|^Assets/Scripts/Presentation/(Palette|Surface|Atmosphere|ZoneMood|Backdrop)\.cs|^Assets/Scripts/Presentation/(Ambience|Quality)/|^Assets/Scripts/Levels/Arch/|^Assets/Scripts/Levels/(Geo|DecorCombiner)\.cs|^Assets/Scripts/Projection/|^Assets/Tests/EditMode/|^Assets/Editor/'; Lead = 'Lead A (materials, architecture, projection core, editor)' },
        @{ Re = '^Assets/Scripts/Levels/Props/|^Assets/Scripts/Levels/(LevelProps|Kit|Scatter)\.cs'; Lead = 'Lead B (PropKit)' },
        @{ Re = '^Assets/Scripts/Levels/|^Assets/Scripts/DebugTools/|^Assets/Tests/PlayMode/'; Lead = 'Lead C (levels, solvability)' },
        @{ Re = '^Assets/Scripts/Gameplay/|^Assets/Scripts/Presentation/|^Assets/WebGLTemplates/|^Assets/Plugins/WebGL/'; Lead = 'Lead D (feel, state, UI, web shell)' },
        @{ Re = '^Assets/Resources/Audio/|^tools/audio/'; Lead = 'Lead E (audio)' },
        @{ Re = '.*'; Lead = 'unowned (docs, packages, workflows, other)' }
    )

    $changed = Invoke-Git diff --name-only $sinceFull $head | Where-Object { $_ -and $_ -notmatch '\.meta$' }
    Write-Output ''
    Write-Output '== Changed files by owner (excluding .meta)'
    $changed | Group-Object { $p = $_; ($owners | Where-Object { $p -match $_.Re } | Select-Object -First 1).Lead } |
        Sort-Object Name | ForEach-Object {
            Write-Output "  $($_.Name): $($_.Count)"
            $_.Group | Select-Object -First 12 | ForEach-Object { Write-Output "      $_" }
            if ($_.Count -gt 12) { Write-Output "      ... and $($_.Count - 12) more" }
        }

    $base = (Invoke-Git merge-base HEAD $target | Select-Object -First 1).Trim()
    $ours = Invoke-Git diff --name-only $base HEAD | Where-Object { $_ }
    $overlap = $changed | Where-Object { $ours -contains $_ }
    Write-Output ''
    Write-Output "== Conflict risk: files changed both upstream and on $(Invoke-Git rev-parse --abbrev-ref HEAD)"
    if ($overlap) { $overlap | ForEach-Object { Write-Output "  ! $_" } } else { Write-Output '  none' }
}

if ($MarkSeen) {
    Set-Content -Path $stateFile -Value $head -Encoding ascii
    Write-Output ''
    Write-Output "Marked $($head.Substring(0, 7)) as reviewed ($stateFile)."
}

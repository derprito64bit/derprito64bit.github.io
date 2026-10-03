## Summary
<!-- What changed and why: 1-3 lines. Name the beat it serves. -->

## Issue
Closes #<n>

## Evidence
<!-- Screenshots from TourShots or a crew shoot at the head SHA (paths or embedded images), with before and after
     where something visible changed. -->
- Head SHA: `<sha>`
- Tests: `powershell -File scripts/ion.ps1 <test-play -Filter X | test>` -> `<passed>/<total> passed, <failed> failed`
- Screenshots:

## Budgets
| Zone | Batches (max 120) | Triangles (max 60k) | Ultra lights on (max 8) | New textures |
|---|---|---|---|---|
| <zone> | | | | none |

## Ownership check output
```
<paste the full output of: powershell -File scripts/fork/ownership-check.ps1 -Base origin/main -Owned '<glob>','<glob>'>
```

## Known issues
<!-- What is unfinished or worse than before, plus the issue requests you filed (#n). Write "none" if there are none. -->

## Checklist
- [ ] Only files in my issue's owned globs changed (the ownership check reports PASS).
- [ ] The gate passed at the head SHA, the test count did not drop and the `ArtPlayTests` constants are unchanged.
- [ ] The zones are within budget. I added no new textures except photo previews, and no UV0 by hand.
- [ ] No owner facts are invented. Placeholders are typed and visibly marked.
- [ ] I did not use the shared MCP tools and ran no WebGL build. Unity ran through `scripts/ion.ps1`.
- [ ] The PR is at most about 800 changed lines (not counting `.meta` files). The last checkpoint is on the issue.

🤖 Generated with [Claude Code](https://claude.com/claude-code)

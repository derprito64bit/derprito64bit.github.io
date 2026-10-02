# Fork notes: derprito64bit.github.io

This repository is derprito64bit's personal-portfolio fork of the collaboration repo
[`Vasiniks/project.ion`](https://github.com/Vasiniks/project.ion). It is published as the user site
https://derprito64bit.github.io/. This file covers the fork itself. Engine facts are in [`BUILD.md`](BUILD.md),
and the binding art and code contract is in [`art-bible.md`](art-bible.md).

## Remotes

| Remote | Repository | Use |
|---|---|---|
| `origin` | `derprito64bit/derprito64bit.github.io` | Push all work here. |
| `upstream` | `Vasiniks/project.ion` | Fetch only. Never push to `upstream/main`. |

## Branches

| Branch | Contains | Rules |
|---|---|---|
| `main` | Mirror of `upstream/main`. | Never commit to it; only fast-forward it from upstream. |
| `overhaul` | The portfolio overhaul: the generic seams merged in, plus the personal layer. | Everything lands here first. Keep it green (EditMode and PlayMode suites pass). |
| `up/<seam>` | One generic, upstream-ready change, cut from `upstream/main`. | Merged into `overhaul` immediately. Not sent upstream (no PR, issue or amendment) until the owner says so. |

Commit discipline: a commit is either **generic** or **personal**, never both.
- A generic commit lives on an `up/*` branch and follows the art bible.
- A personal commit touches only fork-only paths (`Assets/Portfolio/**`, `docs/portfolio/**`, `docs/FORK.md`,
  `scripts/fork/**`, fork-only workflows). It never edits a file the art bible assigns to a Lead (§10).

If a personal feature seems to need an edit to an upstream-owned file, the seam is missing. Build the seam on an `up/*` branch instead.

## Syncing with the collaboration repo

```powershell
powershell -File scripts/fork/upstream-check.ps1              # what changed upstream since the last check
git fetch upstream
git switch main; git merge --ff-only upstream/main; git push origin main
git switch overhaul; git merge main               # resolve conflicts on overhaul, never on main
powershell -File scripts/fork/upstream-check.ps1 -MarkSeen    # record the reviewed upstream head
```

To rebase a seam branch onto the latest upstream: `git switch up/<seam>; git rebase upstream/main`, then re-merge it into `overhaul`.

**Standing rule.** After every major task, and whenever the owner asks a question, run the upstream check and report three things:
- what is new upstream;
- what to incorporate into the fork and what to leave out;
- what the collaboration repo is missing that the fork could contribute.

## Storage policy

- **No Git LFS in this fork.** LFS objects pushed to a fork count against the parent repository owner's quota.
- Blender sources (`.blend`) stay outside the repository. Commit only optimised exports (GLB/FBX) and compressed images.
- Large playable web builds of other projects live in their own repositories and Pages sites. The portfolio links to them or embeds them.

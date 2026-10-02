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

## Site and links

- The site is https://derprito64bit.github.io/, served from the `gh-pages` branch (legacy Pages). Upstream's Pages workflow (`.github/workflows/web.yml`, BUILD.md §5) is not used here: it is manual-only, and dispatching it on the fork would deploy the bare Unity build through Actions Pages instead of the composed `gh-pages` tree, so don't.
- "This site / this repo / play here" links point at the fork. Credits to Vasiniks and the original repo stay.
- Fork-only link edits in upstream-owned files. Keep the fork side when merging upstream, until a site-config seam replaces them:
  - `README.md`: the portfolio intro above the original README, and its **Play** link.
  - `Assets/WebGLTemplates/Ion/index.html`: both "View projects" links.
  - `Assets/Scripts/Presentation/EndCard.cs`: `ProjectsUrl`.
- `companyName` stays `vasiniks` (ProjectSetup, ProjectSettings): it names the game's publisher, and changing it moves the editor's PlayerPrefs.

## Deploy

`scripts/fork/publish.ps1` composes the whole domain in a temp clone of `gh-pages` and pushes it (never a force push):

| Path | Source |
|---|---|
| `/` | `portfolio-site/dist` (next to this repo) once it has an `index.html`; until then the fork's `site/` landing page. `404.html` only if the site has one. |
| `/manor/` | The Unity Web build (`Build/Web`). Its `index.html` gets a phone guard (touch screens under 820 px go to `/?from=manor`, where the landing page says why and links `manor/?force=1`, which skips the guard) and `robots noindex`, injected at publish time. The WebGL template stays untouched. |
| `/play/` | A redirect stub to `/manor/` that keeps the query string and hash, so old links still work. |
| `/arcade/` | `site/arcade/`: HTML5 demos shared by the site and the Manor's cabinet (`../arcade/<slug>/` from `/manor/`). |
| `.nojekyll` | Always. |

The Manor takes `?zone=gallery`, `?zone=game` or any zone key. Before committing, every relative link in the staged HTML must resolve with the exact case (Pages is case-sensitive); broken links are listed by page and stop the publish.

```powershell
powershell -File scripts/ion.ps1 build                          # Build/Web
powershell -File scripts/fork/sync-content.ps1 -Check            # is the Manor's content current? (exit 1 = no)
powershell -File scripts/fork/sync-content.ps1                   # copy it, then rebuild
powershell -File scripts/fork/publish.ps1 -DryRun                # compose, check links, commit in the temp clone only
powershell -File scripts/fork/publish.ps1 -Message "Deploy the Manor"
powershell -File scripts/fork/publish.ps1 -NoManor               # site-only deploy; keeps the published /manor/
```

Other options: `-Build <dir>`, `-SiteDist <dir>`, `-AllowBroken`, `-Remote <url>`.

**Content.** `portfolio-site/content` is the source of truth. `sync-content.ps1` copies `projects.json`, `awards.json` (optional) and `tokens.json` (as `identity.json`) into `Assets/Portfolio/Resources/Portfolio/` after checking that each parses, has an object at the top level (JsonUtility cannot read a bare array) and, for projects, that every entry is an object with a `slug` and a `title`. An `awards.json` left behind after it leaves the source is reported as an orphan, never deleted. Exit codes: 0 copied or in sync, 1 out of sync (`-Check`), 2 source missing, 3 invalid JSON.

## Storage policy

- **No Git LFS in this fork.** LFS objects pushed to a fork count against the parent repository owner's quota.
- Blender sources (`.blend`) stay outside the repository. Commit only optimised exports (GLB/FBX) and compressed images.
- Large playable web builds of other projects live in their own repositories and Pages sites. The portfolio links to them or embeds them.

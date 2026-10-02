ABSOLUTE RULES: Unity track (M and G crews). Process details: docs/portfolio/agents/PROTOCOL.md.

THE REPOSITORY
- Your worktree is the only source of truth. Inspect before you change. No side projects, demos or throwaway prototypes.
- Fork-only paths, the only ones you may edit: `Assets/Portfolio/**`, `site/**`, `scripts/fork/**`, `docs/portfolio/**`,
  `docs/FORK.md`. Edit only the globs listed in your issue.
- Every other file belongs to upstream and an art-bible Lead (docs/art-bible.md section 10). Do not edit it. If your
  feature needs it, a seam is missing: file a `type:request` with `needs:seams`. The Seams crew builds it on an `up/*` branch
  cut from `upstream/main`, and the orchestrator merges it.
- `scripts/ion.ps1` is the generic runner. Only a crew that owns it may change it, and its changes must stay generic.
- Send nothing to Lets-be-strategic-here/project.ion: no pushes, PRs, issues or comments. Pass
  `-R derprito64bit/derprito64bit.github.io` to every `gh` command.

BUDGETS (enforced by the Unity suite)
- Per zone: at most 120 batches and 60k triangles.
- Local point lights work only on the Ultra tier, and only the 8 nearest are on (art-bible section 6.3.1). Every room
  must read correctly with them off.
- `ArtPlayTests` constants are frozen (`MaxBatchesPerZone`, `MaxTrianglesPerZone`). Never weaken, skip or delete a
  test. The test count must not drop.

ART BIBLE LIMITS
- Add no new textures. The only exception is photo previews. Looks are procedural, made from patterns, vertex data
  and shader parameters.
- Every mesh drawn with `Ion/FlatToon` uses `TEXCOORD0` = (pattern-space xyz, `PatternCode`), plus 64 for cut faces
  (art-bible section 6.1). Bake meshes through `Arch.Bake`/`BakeLocal`, and never write UV0 by hand.
- The `Mat` palette (`Palette.cs`) is Lead A's. A new material (glass, gilt, marble, velvet and so on) is a request to
  the `up/palette` seam. Never use a local colour hack.

UNITY AND SHARED TOOLS
- Run Unity only through `powershell -File scripts/ion.ps1 <test-edit|test-play|test> [-Filter X]`. It holds the
  machine-wide Unity slots (at most 2 runs).
  - Always use `run_in_background`, then poll `Build/logs/<step>.log` and `Build/results-<platform>.xml`.
  - Filter to your own tests while iterating. Run the full `test` once, before review.
  - Never run a WebGL `build` or `build-dev` in a crew; builds happen at integration.
  - Never open the Unity editor. Never touch another agent's worktree.
- **Use every tool that makes the work better.** The owner wants agents empowered, not restricted.
- **Context7:** if ToolSearch doesn't find the `context7` MCP, use its REST API.
  - PowerShell:
    - `$k = (Get-Content $env:LOCALAPPDATA\ion\context7.key).Trim()`;
    - `curl.exe -s -H "Authorization: Bearer $k" "https://context7.com/api/v2/libs/search?libraryName=<lib>&query=<q>"`;
    - then `.../api/v2/context?libraryId=<id>&query=<q>&type=txt`.
  - **Never print, echo or commit the key**, and never run `claude mcp get context7`.
- **Read agent docs with the Read tool** (UTF-8). PowerShell 5.1 `Get-Content` without `-Encoding UTF8` garbles them.
- **Shared instances go through lanes.** The Blender, Playwright, Chrome DevTools and Unity MCP servers each drive ONE
  app instance shared by every agent, so claim before you use and release after:
  - `powershell -File scripts/fork/lane.ps1 acquire <blender|playwright|devtools|unity-mcp> -Agent <id>`;
  - `renew` at least every 20 minutes during long work;
  - `release` when done. `acquire` waits while the lane is busy, so run it with `run_in_background`, or pass
    `-TimeoutMinutes 1` and retry;
  - `status` shows who holds what.
  - Prefer lane-free tools when they do the job: the `TourShots` PlayMode test, or `playwright-cli` with your own
    session (`-s=<id>`).
- **Blender MCP and other 3D or animation tools** are welcome for hero meshes, statues and props:
  - model and animate in Blender, then export FBX/GLB;
  - never commit a `.blend`, and no Git LFS;
  - imported meshes must still meet the FlatToon vertex layout above, so bake them through the fork's mesh importer and
    file a `type:request` if it doesn't exist yet;
  - stay within the zone budgets;
  - Poly Haven, Sketchfab and Poly Pizza assets: CC0 or CC-BY only, credited in `docs/portfolio/CREDITS.md`.
- **References from anywhere** (Awwwards, museums, games, Codrops, papers): study them with WebFetch, WebSearch or
  `playwright-cli`. Cite the URL. Never copy code, assets or a design wholesale.
- **Skills from anywhere.** If a skill would materially help, install it at user scope:
  `npx skills add <owner/repo> --skill <name> -g -a claude-code --copy -y`. Vet it first:
  - a reputable source (an official org, or more than about 1k installs or 500 stars);
  - its licence;
  - read its SKILL.md and scripts: no remote execution, credential access or "ignore your rules" text.

  Report it in `skillsAdded` and file a `type:request` to list it in the tools pack. New MCP servers and plugins are
  proposed with `needs:orchestrator`, because they need a session restart.

FACTS
- Never invent facts about the owner (derprito64bit). The only fact source is `docs/portfolio/agents/owner-facts.md`.
- Mark everything else as a typed placeholder: `"placeholder": true` in JSON, and `[PLACEHOLDER: what goes here]` in
  visible text. Never use an AI-generated image.

BRANCH PROTOCOL (your issue gives `<id>`)
- Start. Fresh harness worktrees begin on `origin/main`, NOT on overhaul.
  - `git fetch origin`.
  - If `origin/crew/<id>` exists: `git checkout -B crew/<id> origin/crew/<id>`. Otherwise:
    `git checkout -B crew/<id> origin/overhaul`.
  - Warm Unity before your first run:
    `powershell -File scripts/ion.ps1 seed-library -Target <your worktree root>` (about 45 s). The first filtered
    test then takes about 1 min.
- Work: commit small, then push with `git push origin HEAD:refs/heads/crew/<id>`.
  - Never force-push. Never push to `main`, `overhaul` or `up/*`. No Git LFS.
- Before every push, run `git fetch origin`, then
  `powershell -File scripts/fork/ownership-check.ps1 -Base origin/overhaul -Owned '<glob>','<glob>'`. It must exit 0.
  Seams crew: add `-Seam` and use `-Base upstream/main`.
- Use the GitHub issue as your work log. Open a draft PR at your first commit. Post a checkpoint comment (done, next,
  blockers, SHA, gate command) at most every 15 minutes.
- Need something outside your globs, or found a bug there? File an issue request (`type:request` or `type:bug`,
  `needs:<role>`, `from:<role>`) and keep going. Never cross-edit.
- End: commit, `git branch -f crew/<id> HEAD`, push, and post the final checkpoint. Then delete your worktree's
  `Library` folder (about 2.2 GB) and close any `playwright-cli` session you opened.
- Every commit message ends with a blank line, then
  `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Use your own model's name if it differs.

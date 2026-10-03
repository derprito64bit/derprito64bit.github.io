ABSOLUTE RULES: Unity track (M and G crews). Process details: docs/portfolio/agents/PROTOCOL.md.

THE REPOSITORY
- Your worktree is the only source of truth. Inspect before you change. No side projects, demos or throwaway prototypes.
- **This is a standalone project (D-022), no longer a fork.** Any file in the repo may change, engine files included.
  Edit only the globs listed in your issue; if you need a file outside them, file a `type:request` for its owner crew.
- There are no more seams or `up/*` branches. Engine changes go straight into crew branches, with tests. Keep them
  clean and tested, because the collab repo is still a source of ideas.
- `main` is the project branch (`overhaul` is retired at d1db3f8). Base every crew branch on `origin/main`, and target
  every PR at `main`.
- The art bible (`docs/art-bible.md`) is still the style guide. Its Lead ownership table no longer restricts edits.
- Send nothing to Lets-be-strategic-here/project.ion (it stays a read-only `upstream` remote for inspiration). Pass
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
- New `Mat` materials (glass, gilt, marble, silver, velvet and so on) are added in `Surface.cs` and `Palette.cs` by the
  crew whose issue owns those files (#23, being re-scoped from Seams to engine work), with `ArchKitTests` kept green.
  The M plan's `up/palette` and Seams-crew steps are superseded by D-022. Never use a local colour hack.

UNITY AND SHARED TOOLS

**MACHINE RULES (owner, 2026-10-02: "my computer is getting eaten")**
- **Blender renders on the GPU only (AMD RX 6700 XT, HIP), never the CPU.**
  - In every Blender MCP session and every background script, first run
    `exec(open(r"<your worktree root>\scripts\fork\blender_gpu.py").read())`, using your own worktree's absolute path.
    It sets HIP, enables only the GPU device and sets every scene's `cycles.device = 'GPU'`.
  - Never write `cycles.device = "CPU"`.
  - Use EEVEE for quick previews. Use Cycles at 64 samples or fewer, with denoising, for stills. Preview renders
    stay at 1600 px or less.
  - Cap `render.threads` at 4.
  - Never run two Blender renders at once (the `blender` lane).
  - The PC has no NVIDIA GPU (a stale RTX 3070 driver entry exists): never select CUDA or OptiX.
- **Close every browser you open, every time.**
  - `playwright-cli`: always open with
    `--config C:\Users\Aaron\AppData\Local\ion\pwcli\agent.config.json --idle-timeout 600000`. The config
    **mutes all audio** (the owner's rule: music volume is 0 for agents). It also renders on the GPU, and the
    timeout closes the session after 10 idle minutes. Close
    your session with `playwright-cli -s=<id> close` as soon as you are done and before you return your result.
    Run `playwright-cli list` and close anything of yours still open.
  - Lighthouse: always pass `--chrome-flags="--headless=new --mute-audio"`, and let it exit; never leave it running.
  - Claude in Chrome tools: close every tab you opened with `tabs_close_mcp`.
  - Leftover automation browsers are killed by `scripts/fork/reap-browsers.ps1 -Kill`.
- **Fujifilm X-T5 fidelity (owner):** the camera is modelled on the Fujifilm X-T5 essentially 1:1. Gather
  reference images and dimensions. The **website** model is ultra-detailed; the **game/Manor** model is a
  simplified version made from the same proportions.

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
  - imported meshes must still meet the FlatToon vertex layout above, so bake them through the mesh importer and
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
- Start. Fresh harness worktrees begin on `origin/main`, the project branch.
  - `git fetch origin`.
  - If `origin/crew/<id>` exists: `git checkout -B crew/<id> origin/crew/<id>`. Otherwise:
    `git checkout -B crew/<id> origin/main`.
  - Warm Unity before your first run:
    `powershell -File scripts/ion.ps1 seed-library -Target <your worktree root>` (about 45 s). The first filtered
    test then takes about 1 min.
- Work: commit small, then push with `git push origin HEAD:refs/heads/crew/<id>`.
  - Never force-push. Never push to `main` (the orchestrator merges), `overhaul` (retired) or `up/*`. No Git LFS.
- Before every push, run `git fetch origin`, then
  `powershell -File scripts/fork/ownership-check.ps1 -Base origin/main -Owned '<glob>','<glob>'`. It must exit 0.
- Use the GitHub issue as your work log. Open a draft PR at your first commit. Post a checkpoint comment (done, next,
  blockers, SHA, gate command) at most every 15 minutes.
- Need something outside your globs, or found a bug there? File an issue request (`type:request` or `type:bug`,
  `needs:<role>`, `from:<role>`) and keep going. Never cross-edit.
- End: commit, `git branch -f crew/<id> HEAD`, push, and post the final checkpoint. Then delete your worktree's
  `Library` folder (about 2.2 GB) and close any `playwright-cli` session you opened.
- Every commit message ends with a blank line, then
  `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Use your own model's name if it differs.

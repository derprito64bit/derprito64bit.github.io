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
- Never use the shared Playwright, Unity, Blender or DevTools MCP tools unless your issue names you. For screenshots,
  use the `TourShots` PlayMode test.

FACTS
- Never invent facts about the owner (derprito64bit). The only fact source is `docs/portfolio/agents/owner-facts.md`.
- Mark everything else as a typed placeholder: `"placeholder": true` in JSON, and `[PLACEHOLDER: what goes here]` in
  visible text. Never use an AI-generated image.

BRANCH PROTOCOL (your issue gives `<id>`)
- Start:
  - run `git fetch origin`;
  - if `origin/crew/<id>` exists, run `git reset --hard origin/crew/<id>`, or else `crew/<id>` if only that exists;
  - otherwise stay on the `overhaul` base.
- Work: commit small, then push with `git push origin HEAD:refs/heads/crew/<id>`.
  - Never force-push. Never push to `main`, `overhaul` or `up/*`. No Git LFS.
- Before every push: `powershell -File scripts/fork/ownership-check.ps1 -Base overhaul -Owned '<glob>','<glob>'`. It
  must exit 0. Seams crew: add `-Seam` and use `-Base upstream/main`.
- Use the GitHub issue as your work log. Open a draft PR at your first commit. Post a checkpoint comment (done, next,
  blockers, SHA, gate command) at most every 15 minutes.
- Need something outside your globs, or found a bug there? File an issue request (`type:request` or `type:bug`,
  `needs:<role>`, `from:<role>`) and keep going. Never cross-edit.
- End: commit, `git branch -f crew/<id> HEAD`, push, and post the final checkpoint.
- Every commit message ends with a blank line, then
  `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Use your own model's name if it differs.

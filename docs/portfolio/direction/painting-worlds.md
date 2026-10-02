# Painting Worlds: G-Manager plan (Wave 1a)

> Locked by the orchestrator as the G plan, subject to the owner decisions in `docs/portfolio/agents/decisions.md` (D-007, D-010).

# Painting Worlds: G-Manager lock (north star v1.1)

Read-only judgement of the four Wave 1a recons (G01 pilot, G03 worlds, G04 rights, G05 look). Every disputed claim was re-checked in source; seven changed the plan.

## Verdict

The Starry Night pilot is a three-minute climb, not a diorama. You arrive in the dark foreground. A gilt exit frame glows in the crescent moon, upper right, and you can see it from the first frame. Two photos get you there. A handed photo pastes a cypress whose flame tiers are steps. Then you turn round, snap the spiral stair on the back hill and paste it into the 6 m wall. Everything solves with the default 50 degree, 4:3 camera at roll 0. The pilot needs no streaming, lenses, film looks or new Mats.

## What the source changed

1. **Freezing already works.** Capture clips non-device Sliceables at their current matrix and parents photo pieces to a static root (ProjectionSystem.cs:194-218, 1287-1292). No photo-clone seam is needed. Rotating ribbons are possible, but stay off the critical path.
2. **Pitch is assisted.** Snap and Place both snap to level within 5 degrees (PhotoHolder.cs:29, 411, 482; InstantCamera.cs:168). G01's 1.1 m junction error mostly disappears. Yaw error remains and gets a Forgiveness test.
3. **The cypress was out of frame.** At (-10,0,4) it sits about 79 degrees left of the spawn view; the player sees about 51 degrees either side (70 degree vertical FOV, 16:9). Move it to about 40 degrees left, near (-6,0,9), and re-run the frustum check.
4. **Deep links skip OnEnter.** Only GoToZoneWorld calls it (GameBootstrap.cs:351), so `?zone=` and test starts would have no camera. A CameraPickup on the easel grants it; the OnEnter unlock stays as a fallback.
5. **Ultra twins must not be cloned.** `Palette.IsUltra` is a reference set (Palette.cs:308). A PaintToon clone of an Ultra material would draw off Ultra, so the swap skips them.
6. **Chunk size.** A placement re-cooks every chunk it cuts (ArchBake.cs:28-32). Keep the 8 m default; raise it only if measured batches demand it and Place stays within 25 ms.
7. **Neighbours show.** Zone culling still draws the next zone's collider batches 50 m away (GameBootstrap.cs:386). Edge hills hide them until streaming lands.

## The pilot, locked

| Time | Beat |
|---|---|
| 0:00 | Arrive: cypress left rising off the top, lit village and spire low right, 10 stars and Venus, the crescent with its frame. Toast: "The Starry Night. Saint-Remy, 1889." |
| 0:20 | Take the camera and photo at the easel; place the cypress ladder from the brass marker |
| 0:50 | A 6 m wall; the hint points behind you |
| 1:10 | Snap the spiral stair facing -Z; place it from the H1 marker |
| 2:00 | Climb the stair, deck, moon stair and crescent |
| 2:40 | The gilt frame returns you to the Grand Gallery (the Painting Wing once it exists) |

Layout, markers and solutions are G01's, with the cypress moved. The exit is also built as a DioramaTeleporter, so a placement that swallows it re-pastes a working copy.

**Sky.** A non-Sliceable dome draws the swirl, 10 stars, Venus and the crescent, and is never cut. Star positions are traced by script from the evidence image. "Eleven" was the plan's wording, not the owner's; the sourced count stands.

**Ribbons.** Three Soft rotating ribbons ("photograph the wind") are a one-day G-C2 spike. They are cut if Validate rejects Soft plus Dynamic, if a Place that cuts them exceeds 25 ms, or if a reviewed filmstrip of the two solution placements looks accidental. A walkable frozen ribbon is deferred.

**Look.** Fork-only PaintToon twins (same Mats, same UV0, one CBUFFER) plus the dome. Existing Mats only; the cypress is Graphite under the night mood. G05's darkening multiplier is cut as a local colour hack. Tiers trim cost, never stroke layout. Reduced motion freezes sky and ribbons.

**Targets (measured, not estimated):** at most 90 batches, 20k triangles and 4 of the 8 Ultra lights; Place within 25 ms.

**Codas.** Cafe Terrace is cut (two checkpoints instead). Bedroom in Arles is deferred to its own small world; its illusion is geometry, not a photo trick.

## Art-bible scope

Painting Worlds may break the upstream look rules 2, 6 and 7 (right angles, off-white structure, no black), as the Manor already does. They keep every absolute limit: no textures, UV0 through Arch.Bake, Lead A's Mats, the zone budgets. The orchestrator logs this.

## Next worlds, ranked

1. **The Imaginary Prisons** (Piranesi, Carceri). The owner asked for a Fraud-style world. Three lies, each broken once: a decoy device floor that a paste removes, a ceiling stair rolled 180 degrees into a floor, a paste that cuts your way back. Existing engine only.
2. **Water Lilies, "No Horizon"** (Monet). Roll 180 turns a reflection into a floor; the cheapest world. Correction: capture and place level, because at -30 degrees of pitch the flip leaves pads tilted 60 degrees. Ship after 2027-01-01 (life plus 100).
3. **Garden of Earthly Delights, "Graft"** (Bosch), cut to two panels. Paste Eden over Hell; no torture imagery.
4. **The Great Wave** (Hokusai), after the lens seam and with a new verb. A paste is rigid, so "Fuji at your feet" cannot work; telephoto must earn its place as a narrow, surgical slab.

Cut: Klimt, Mondrian (the Trust's January 2026 dispute), Op Art, Melting Time, Day over Night. Atkins waits for film looks, as a coda.

## Rights

US public domain: published 1930 or earlier until 2027-01-01, then 1931. Museum CC0 first, Commons PD-Art second. Scans are reference only and never become textures in the build. Public-domain artists are credited on wall labels with their museum; inspired-by artists are never named in public.

## Crews

- G-C1 (pilot) and G-C2 (look kit) run in parallel after M-F. G-C2 first lands a no-op `PaintLook.Apply`.
- G-C3 (portals) follows the streaming seam, the Wing shell and G-C1.

## Decisions

- **G-D001 What the Starry Night pilot is:** A three-minute climb up the painting, not a diorama. The goal is in sight from the first frame: a gilt exit frame in the crescent moon's hollow, upper right. There are exactly two photo puzzles: a handed photo (it pastes a cypress whose flame tiers are steps up the 3 m hill), then your own photo (snap the spiral stair on the back hill facing -Z, paste it into the 6 m wall from the H1 marker). Then a plain walk: stair, belvedere deck, moon stair, crescent, exit. G01's layout, markers and solution list are adopted, except where G-D004 changes them. _(why: A goal you can see makes the level finishable by non-gamers in 3 to 10 minutes. One new verb per puzzle (place a given photo, then take your own) repeats the proven StairsWing to CameraWing escalation. The camera is the only way up, so the level shows the camera off, and the painting gives it its identity.; beats 4 craft, 1 identity (inside the Manor); budget: G01 estimate 60-85 batches, 5-7k triangles; locked targets in G-D011)_
- **G-D002 Photo grammar every world must obey:** Design to the engine's real rule. A photo is a rigid 3D slice. Place deletes the viewer's frustum (cutting Sliceables and deactivating devices whose pivot is inside) and pastes at worldPose = viewerPoseRolled x capturePose^-1 x piece. There is no scale change and no forced-perspective closing through photos. Poses are level: Snap and Place both snap pitch to 0 within 5 degrees. Anything needed after a Place must lie outside that solution's cone, or also be built in the diorama so the paste re-creates it. _(why: Verified in source: ProjectionSystem.cs:194-218 (capture), :397-405 (device removal), :408-420 (rigid paste); PhotoHolder.cs:29 LevelSnapDegrees 5, applied at PhotoHolder.cs:411,482 and InstantCamera.cs:168. Two G03 proposals break this rule (see conflicts).; beats 4 craft; budget: none)_
- **G-D003 The swirl: critical path vs sky vs ribbons:** (a) The critical path is a static Solid spiral stair: 24 four-sided prisms with yaw, 0.25 m rise each, 6 m total. It is built through PropBuild with B.Exempt so the off-grid yawed treads pass Arch.Validate. (b) The swirl, the 10 stars, Venus and the crescent are drawn by a non-Sliceable SwirlSky dome, so a photo never cuts them. (c) Three Soft rotating ribbons ('photograph the wind', down from G05's five) go through a one-day G-C2 spike with kill criteria. A walkable frozen ribbon is deferred. _(why: Source shows capture already freezes non-device Sliceables at their current matrix and parents photo pieces to a static root (ProjectionSystem.cs:194-218, 1287-1292), and BakeLocal accepts non-Interactable Dynamic pieces (ArchBake.cs:44-50, 247-250). So no photo-clone seam is needed, which corrects G01. But a time-driven ribbon on the critical path would make RunAll and Forgiveness non-deterministic and add moving colliders, so it stays optional.; beats 4 craft, 1 identity; budget: Dome 1 batch about 224 tris; ribbons at most 2 batches about 650 tris; stair about 288 tris)_
- **G-D004 First-frame composition:** Move the cypress into the spawn view: about 40 degrees left of the spawn axis, near (-6,0,9), still 22 m tall so it rises off the top. G01's (-10,0,4) sits about 79 degrees off-axis, outside the player's roughly 51 degree half-width (70 degree vertical FOV at 16:9). Keep the moon and exit about 35 degrees right and 28 degrees up (already in frame), and the hill skyline at 0.63 of the height. Re-run G01's frustum script with the new position. _(why: Beat 1 inside the world and the owner's 'based heavily off of van gogh night' need the painting legible at once, and the cypress is its most recognisable shape. Player FOV from PlayerFactory.cs:55 (70 degrees).; beats 1 identity, 4 craft; budget: none)_
- **G-D005 Star count and sky table:** 10 stars plus Venus plus a crescent moon. Positions come from a script that traces starry_night_1000.jpg into a 12-entry table, stored with the evidence. G01's by-eye list is only a fallback. _(why: The manor-refs correction and G04 both support 10 plus Venus. 'Eleven' is the plan's wording; owner-voice only says 'van gogh night', so no owner call is needed.; beats 1 identity; budget: none (shader uniform array))_
- **G-D006 Painterly look kit:** Adopt G05: a fork-only Ion/PaintToon (a FlatToon copy with the same UV0, the same Properties and one CBUFFER plus appended floats) and the SwirlSky dome, both in Assets/Portfolio/Resources/Portfolio/. A per-zone twin swap runs at the end of Build, before Arch.Bake, on both the zone and its diorama. It keeps the Ion_Mat_<Role> names and SKIPS any material for which Palette.IsUltra is true. Use existing Mats only; the cypress is Graphite under the Starry mood. Cut G05's _PaintDark multiplier and, in v1, per-stroke hue variation. Tiers trim cost and never stroke layout. Feel.ReducedMotion freezes the sky drift, star pulse and ribbons. _(why: Needs no upstream edit, texture or Mat seam. The IsUltra skip matters because IsUltra is a reference set (Palette.cs:308): a cloned Ultra twin would escape ApplyZoneCulling (GameBootstrap.cs:387) and draw off Ultra. A darkening multiplier is a local colour hack, which rules.unity.md bans.; beats 4 craft; budget: 0 extra batches from the swap (one material per Mat); about 100 ALU on painted pixels; sky 1-1.5 ms High (estimate, must be measured))_
- **G-D007 Art-bible scope for Painting Worlds:** Painting Worlds may break the upstream art-bible statement rules 2 (right angles only), 6 (off-white structure) and 7 (no black; Graphite darkest) inside their own zones, as the Manor's candlelit look already does. They are fully bound by the rules.unity.md limits: no new textures, UV0 through Arch.Bake/BakeLocal, Lead A owns Mat, the zone budgets, and no upstream edits. _(why: North star: each medium does what only it can; the owner asked for Van Gogh and 'trippy' worlds. The absolute rules list does not include rules 2, 6 and 7, but logging the exemption is the orchestrator's job.; beats 4 craft; budget: none)_
- **G-D008 Entry, exit and camera grant:** The pilot registers through its own registrar (key 'pw.starry', order 1100 or more) and is reachable through ?zone=pw.starry and IonDebug before the Painting Wing exists. The exit gilt frame teleports to pf.gallery and is also built with ctx.DioramaTeleporter. G-C3 later points it at the Wing frame. The camera is granted by a CameraPickup on the easel lectern (a Pickup step), with RoomContext.UnlockInstantCamera in OnEnter as a fallback. _(why: OnEnter runs only from GoToZoneWorld (GameBootstrap.cs:351), so a deep link or test start would have no camera. The Grand Gallery must always be one step away. A DioramaTeleporter keeps the way out if any placement swallows the frame (Room.cs:398-405). Film is refunded on rewind, so the player cannot get stuck.; beats 3 everything (skippable), 4 craft; budget: +1 device draw group (camera stand))_
- **G-D009 Streaming dependency:** The pilot does not wait for streaming. It ships built at boot through ZoneCatalog as an interim, with boot-time and memory cost measured. G-C3 moves it behind up/zone-streaming. Edge hills hide neighbour geometry until then. _(why: Decouples the riskiest seam from the pilot. ApplyZoneCulling still draws the adjacent zone's collider batches (GameBootstrap.cs:386), so walls are needed now.; beats 4 craft; budget: Adds one zone plus one diorama to boot (measure))_
- **G-D010 Codas:** Cut the Cafe Terrace as a room (two PropKit checkpoints instead: arrival and deck). Defer Bedroom in Arles to its own small world after the pilot; its skewed-room illusion is pure geometry seen from a marker, not a photo trick. _(why: Neither serves a beat inside the first three minutes; each adds a zone.; beats story gate; budget: saves one or two zones)_
- **G-D011 Pilot budget targets and bake chunk:** Measured targets: at most 90 batches and 20k triangles at High; 4 local lights (moon platform, exit, two village windows) of the 8 allowed; Place within 25 ms. BakeChunk stays at the 8 m default. Raise it only if measured batches exceed the target and Place timing stays within 25 ms. _(why: ArchBake.cs:28-32: every cut chunk re-cooks its MeshCollider on placement, the dominant cost on slow CPUs. G01's 16 m chunk trades placement time for batches without measurement.; beats 4 craft (weak-hardware floor); budget: Headroom of 30 batches and 40k triangles under the zone limits)_
- **G-D012 Next worlds, ranked:** 1 The Imaginary Prisons (Piranesi Carceri, the Fraud world). 2 Water Lilies 'No Horizon' (Monet), with level poses. 3 Garden of Earthly Delights 'Graft' (Bosch), two panels. 4 The Great Wave (Hokusai), after up/lens and with a new verb. Cut: Klimt, Mondrian, Op Art, Melting Time, Day over Night as worlds. Atkins becomes a later coda once film looks exist. _(why: Owner-voice names the Fraud concept, so Carceri comes first; it uses only existing engine features (roll, device removal, rewind). Water Lilies is the cheapest clear verb. Bosch is rights-clear but a budget and tone risk, so it is reduced. Great Wave's proposed verb breaks the rigid-paste rule and needs lenses.; beats 4 craft; budget: G03 estimates: Carceri 40k tris/70 batches; Lilies 8k/40; Bosch two panels about 20k/50; Wave 12k/40 (all unmeasured))_
- **G-D013 Rights rules:** US public domain means published 1930 or earlier until 2027-01-01, then 1931. Take images from museum CC0 sources first and Commons PD-Art second. Scans are reference only and never become textures in the Unity build. Credit public-domain artists on Painting Wing wall labels with the holding museum. Never name inspired-by artists (Escher, Dali, Magritte, Hopper, Riley, Vasarely) in public. Do not reproduce Mondrian. The Monet world ships after 2027-01-01. _(why: G04's sourced findings (LOC, Commons, Met/AIC APIs, the January 2026 Mondrian Trust dispute). rules.unity.md allows no textures except photo previews, so G04's 'previews are scans' assumption does not hold.; beats 4 craft, 3 everything (credits); budget: none)_
- **G-D014 Tests the G crews add (fork only):** Assets/Portfolio/Tests/PlayMode/PaintingWorlds/: RunAll solvable; a Forgiveness clone (plus or minus 0.3 m and 3 degrees of yaw on both Places); spawn composition (projected bands); exit usable after each solution Place at roll 0 and plus or minus 2.5 degrees; at most 8 local lights; PaintParity; a PaintToon-vs-FlatToon property parity guard; ambience restore on leaving. Upstream ArtPlayTests, ProjectionPlayTests and ZoneCatalogTests cover the zone automatically and are never weakened. _(why: Upstream Forgiveness is parametrised to core zones only (ProjectionPlayTests.cs:405), and local lights and textures are not checked automatically.; beats 4 craft; budget: none)_
- **G-D015 G crew sequencing:** G-C1 and G-C2 run in parallel after M-F; G-C2's first merge is a no-op PaintLook.Apply API. G-C3 starts after up/zone-streaming, the Painting Wing shell and G-C1. Carceri is the next world crew after G-C3. _(why: The pilot needs neither lenses nor streaming, so waiting for the Camera merge (plan) only delays it. The stub API removes the G-C1/G-C2 file dependency.; beats 4 craft; budget: none)_

## Seams, in order

- 1. up/zone-streaming (owned by M Streaming & Performance; built by the Seams crew; needed by G-C3, not G-C1). Files: Assets/Scripts/Levels/GameBootstrap.cs (on-demand Build/Unbuild, per-zone diorama capture on stream-in, ApplyZoneCulling for unbuilt and banded zones, spacing band); Assets/Scripts/Levels/ZoneCatalog.cs (streamed flag and band order); Assets/Scripts/Levels/Room.cs (optional Footprint or Streamed property); Assets/Tests/PlayMode/ZoneCatalogTests.cs; Assets/Tests/PlayMode/IonPlayTestBase.cs (StartInZone or ensure-built helper); Assets/Tests/PlayMode/ArtPlayTests.cs and ProjectionPlayTests.cs (build streamed zones before iterating; constants frozen, nothing weakened).
- 2. up/palette (already planned for M). Files: Assets/Scripts/Presentation/Surface.cs (append-only Mat), Assets/Scripts/Presentation/Palette.cs (s_MatColors), Assets/Tests/EditMode/ArchKitTests.cs. G needs only gilt for portal and exit frames (Brass fallback). Painter Mats (Ultramarine, ChromeYellow) are optional requests, not pilot dependencies.
- 3. up/lens (already planned for M Camera). Files: Assets/Scripts/Gameplay/InstantCamera.cs (per-lens FovY), Assets/Scripts/Gameplay/PhotoHolder.cs, Assets/Scripts/Levels/Room.cs (RoomSolution FOV or PhotoFovY default), Assets/Tests/PlayMode/ProjectionPlayTests.cs (frustum per photo FOV). Needed by world 4 (Great Wave) and the Atkins coda only.
- Plan order (lens, then streaming, then palette) is compatible, provided up/zone-streaming merges before G-C3 starts. No photo-clone or freeze seam is requested; the Forgiveness coverage for fork zones is a fork test, not a seam.

## Cross-track escalations

- M Streaming & Performance / Seams crew (up/zone-streaming): (a) build and unbuild one Painting World on demand, including its diorama and DioramaShot capture; (b) keep ArtPlayTests and ProjectionPlayTests covering streamed worlds without weakening them (for example a test hook that builds each streamed world); (c) give Painting Worlds their own spacing band or footprint so placement cones (lateral reach about 0.62 x depth, 250 m far) and neighbour culling cannot reach Manor rooms or other worlds; (d) fully hide neighbours while inside a world (ApplyZoneCulling draws the adjacent zone's collider batches today); (e) keep ?zone=<key> deep links; (f) return memory to baseline on unload; (g) state a stream-in time budget for the reference laptop.
- M Camera crew / up/lens: keep 50 degrees and 4:3 as the default capture and solution FOV. If a lens changes FovY, Place and Solutions_FrustaTouchOnlyTheirZone must use the photo's FOV. The pilot requires no lens or film look; the Great Wave and Atkins depend on them.
- M Manor rooms crew: a Painting Wing shell with frame anchors (position, facing, size) that G-C3 fills, one step from the Grand Gallery, and a stable destination key for world exits. The gilt Mat comes via up/palette, with Brass as the fallback.
- M-F Foundation: create Assets/Portfolio/Runtime/PaintingWorlds/{Look,StarryNight,Portals}/ and Assets/Portfolio/Tests/PlayMode/PaintingWorlds/ with .meta files, using the one-registrar-per-feature pattern.
- Orchestrator / Lead A: log that Painting Worlds are exempt from art-bible rules 2, 6 and 7 but bound by every rules.unity.md limit. Record that Ion/PaintToon is a fork-owned FlatToon copy, with drift caught by a parity test.
- Orchestrator: fix the plan's rights rule (1930 or earlier until 2027-01-01, then 1931), its star count (10 stars plus Venus) and its Mondrian row (cut).
- Manor Bridge / Overlay: carry the page's prefers-reduced-motion into Feel.ReducedMotion at boot (compose-time injection; Lead D's template untouched) so painted skies and ribbons freeze for those visitors (non-negotiable 4).
- IA & Content Model: if the Front Door teases Painting Worlds (beat 4 teaser), add a worlds entry to content/ (title, artist, year, holding museum, licence) so wall labels and the 2D teaser share one source.
- Budgets: zone limits are unchanged (120 batches, 60k tris, 8 Ultra lights). The pilot's 90-batch and 20k-tri targets are G-internal.
- Springs and tokens: none needed. The world loading cover reuses the existing develop flash; the gilt frame uses M's gilt Mat.

## Crew briefs

### G-C1 Starry Night pilot

- **Goal:** Build zone 'pw.starry': a three-minute, two-photo climb to a gilt exit frame in the crescent moon. It must be solvable with the default camera, read as The Starry Night from the first frame, and stay inside budget.
- **After:** M-F Foundation (PaintingWorlds folders, per-feature registrar), G-C2 PaintLook.Apply stub merged, M-QA TourShots
- **Owned globs:** Assets/Portfolio/Runtime/PaintingWorlds/StarryNight/**, Assets/Portfolio/Tests/PlayMode/PaintingWorlds/StarryNight*, docs/portfolio/worlds/starry-night*
- **In scope:** Room subclass, live and diorama builder, own registrar (key pw.starry, order 1100 or more); G01 layout with the cypress moved to about 40 degrees left of spawn (G-D004); re-run the frustum script; Solutions and brass markers: pickup camera and photo, Place cypress-ladder (pre-made DioramaShot), Blocked walk at the 6 m wall, Snap spiral stair (yaw 180), Place stair (H1 marker), goals at deck, moon stair and crescent, Teleport exit; Spiral stair from yawed prisms through PropBuild with B.Exempt; Solid; Painter's-view DioramaShot (preview only) for the Wing card; CameraPickup on the easel lectern plus the OnEnter UnlockInstantCamera fallback; Exit gilt frame: ctx.Teleporter to pf.gallery plus ctx.DioramaTeleporter copy; Starry ZoneMood values from G05 (via G-C2's params); hints in existing wording; checkpoints at arrival and deck; Edge hills that hide neighbour zones; 4 LocalLights (moon platform, exit, two village windows); Call PaintLook.Apply(root, diorama, StarryParams) from G-C2's API; Fork PlayMode tests (G-D014 items for this zone)
- **Out of scope:** PaintToon and SwirlSky shaders, ribbons, paint params internals (G-C2); Painting Wing, portals, streaming (G-C3 and M); Lenses, film looks, new Mats or Pats, textures; Cafe Terrace and Bedroom codas; Any upstream file (file a type:request with needs:seams)
- **Acceptance:**
  - RunAll solves pw.starry with the 50 degree, 4:3 camera at roll 0, with no lens or film look
  - Upstream suites pass unchanged: ArtPlayTests (120 batches, 60k tris, pattern space, Arch.Validate on-grid), ProjectionPlayTests (markers equal Place/Snap, ground within 0.15 m, frusta at 4 rolls touch no other zone, diorama preview edge pixels), ZoneCatalogTests; test count does not drop
  - Measured at most 90 batches and 20k tris at High; at most 4 local lights (fork test); every Place within 25 ms in the ProjectionSystem timing log
  - Forgiveness (fork): both Places from plus or minus 0.3 m and plus or minus 3 degrees of yaw still reach H1 and the deck
  - Composition (fork): from spawn at yaw 0, FOV 70, 16:9, the cypress axis projects within 5-28% x, the crescent and exit within 76-95% x and 5-25% y, and the skyline at 0.63 plus or minus 0.05 of the height
  - Exit usable (live or diorama copy) after each solution Place at roll 0 and plus or minus 2.5 degrees
  - ?zone=pw.starry and IonDebug starts can obtain the camera and finish; the exit lands in pf.gallery
  - TourShots at Low (lights off) and Ultra: the route reads, and no neighbour geometry is visible from spawn or any marker
  - Ownership check exits 0; no upstream file is touched
- **Evidence:** Build/results-<platform>.xml for the filtered run and one full test run at the final SHA; TourShots PNGs: spawn, each marker before and after, Low and Ultra; per-zone batch and triangle JSON; Placement timing log lines for both Places; Re-run frustum script output for the final layout (portfolio-evidence, not committed); Spawn frame beside starry_night_1000.jpg (local evidence only)

### G-C2 Painterly look kit

- **Goal:** A reusable, texture-free, capture-safe painterly kit for every Painting World: the PaintToon twin swap, the SwirlSky dome, per-world paint params and moods. Starry Night is the first preset.
- **After:** M-F Foundation
- **Owned globs:** Assets/Portfolio/Runtime/PaintingWorlds/Look/**, Assets/Portfolio/Resources/Portfolio/PaintToon*, Assets/Portfolio/Resources/Portfolio/SwirlSky*, Assets/Portfolio/Tests/PlayMode/PaintingWorlds/PaintLook*
- **In scope:** First merge: PaintLook.Apply(root, diorama, PaintParams) as a no-op stub; Ion/PaintToon (a FlatToon copy: same Properties and one CBUFFER plus appended floats, ForwardLit paint block, other passes verbatim) plus a Resources material; Per-zone twin swap: zone and diorama, Ion_Mat_<Role> names kept, Palette.IsUltra materials skipped; Stroke modes DAB, SWIRL, FLAME, DASH; zone-local anchoring with a constants test; Ion/SwirlSky dome (about 224 tris, 1 batch, non-Sliceable, footprint-gated, behind opaques); Star, Venus and moon table traced by script from the evidence image; StarryNight PaintParams and ZoneMood (G05 values); ZoneChanged hook: clouds and sun glow off inside worlds, restored on leaving; Tier gating; Feel.ReducedMotion freezes drift, pulse and ribbons; One-day ribbons spike: three Soft and Dynamic box-chain ribbons under a fork rotator; Tests: PaintParity, PaintToon/FlatToon property parity guard, zone-local constants, ambience restore
- **Out of scope:** Zone layout and puzzles (G-C1); New Mats or Pats, any edit to FlatToon or Palette (Lead A); _PaintDark colour multiplier and per-stroke hue variation (cut in v1); Textures of any kind, curl-noise or flow maps; Lenses and film looks; Walkable or colliding ribbons
- **Acceptance:**
  - No new textures and no upstream edits; PaintToon keeps the FlatToon UV0 layout and SRP Batcher compatibility; the parity guard fails if FlatToon's Properties or CBUFFER prefix diverges
  - Swap batch delta is 0 (plus or minus 1) against the same zone on FlatToon; TryGetMat still resolves every swapped material; no Ultra material is cloned
  - PaintParity: preview vs paste at the same pose, mean absolute difference under 1.5/255 outside the cut; stroke layout identical across tiers, time and camera; a rolled paste is lit correctly
  - Dome: 1 batch, at most 300 tris, never Sliceable, hidden outside its zone or diorama footprint, overdraws the Backdrop; 10 stars plus Venus plus crescent from the measured table
  - Measured sky cost at 1080p on the reference laptop: within 1.5 ms at High and 0.5 ms at Low (G05 estimates, now targets)
  - Reduced motion: the sky, star halos and ribbons are static
  - Ribbons ship only if Soft plus Dynamic passes Arch.Validate, a Place that cuts them stays within 25 ms, and a reviewed filmstrip of the two solution placements reads as intentional. Otherwise they are cut.
  - Ambience clouds and sun glow are off inside Painting Worlds and restored on leaving
- **Evidence:** Build/results-<platform>.xml (filtered and full); Tier strip TourShots (Low, Medium, High, Ultra) of the Starry zone; Frame-time captures for sky and impasto at Low and High; Star-tracing script and output table (portfolio-evidence); Ribbons spike verdict with filmstrip and placement timings

### G-C3 Painting Wing portals (with M Streaming)

- **Goal:** Make the Painting Wing's gilt frames the way in and out of Painting Worlds, with one world loaded at a time and the Grand Gallery always one step away.
- **After:** up/zone-streaming merged, M Manor rooms: Painting Wing shell with frame anchors, G-C1 merged, up/palette gilt (or Brass fallback)
- **Owned globs:** Assets/Portfolio/Runtime/PaintingWorlds/Portals/**, Assets/Portfolio/Tests/PlayMode/PaintingWorlds/Portal*
- **In scope:** PaintingPortal: gilt frame, Teleporter and wall label (title, artist, year, holding museum, licence) on the Wing's frame anchors; World registry (key, title, credit, card source) shared with content/ if IA adopts it; Stream-in and stream-out through the up/zone-streaming API with the existing develop-flash cover; Point the pilot exit at its Wing frame (a request to G-C1 for its file, or a registry lookup); Wing card from the painter's-view photo preview or a procedural SwirlSky canvas; Deep link ?zone=pw.starry kept working
- **Out of scope:** Streaming internals (M Streaming and the Seams crew); Wing room shell and lighting (M Manor rooms); World content (G-C1, later world crews); Gilt Mat (up/palette; Brass fallback); Scanned painting images in the build
- **Acceptance:**
  - Entering a Wing frame loads that world at its spawn, and its exit returns to the same frame; a test asserts at most one Painting World is built
  - The Grand Gallery is one step from the Wing
  - Streamed worlds still pass ArtPlayTests and ProjectionPlayTests through the seam's hook
  - Stream-in time measured on the reference laptop is within the seam's stated budget; memory returns to baseline (plus or minus 5%) after unload
  - Labels credit public-domain artists with their museum; no inspired-by artist name appears in any string, file name or metadata
  - No new textures beyond photo previews
- **Evidence:** Build/results-<platform>.xml; TourShots of the Wing and the frame-to-world-to-frame round trip; Stream-in timing and memory before and after logs; grep output showing no banned artist names in fork strings

## Owner questions

- Garden of Earthly Delights: is a toned-down Hell (ice, fire glow, giant instruments, no torture imagery) acceptable, or should Bosch be dropped?
- Should the Painting Wing credit public-domain artists by name with their museum on wall labels? (Recommended: yes. Inspired-by artists are never named.)
- Later, should a photographed sky ribbon become a walkable frozen path (more engine work), or stay a visual 'photograph the wind' moment?
- Is 'The Imaginary Prisons' acceptable as the Fraud-style world's title (keeping the game's name out of public copy)?
- Mondrian is cut. Reply only if you want an unnamed, original colour-plane world instead.

## Cut list

- Cafe Terrace checkpoint room (two checkpoints instead)
- Bedroom in Arles in the pilot (deferred to its own world)
- Walkable frozen ribbon (deferred); five ribbons cut to three, all three if the spike fails
- Film-look hidden stroke path
- Telephoto single-star mini game
- Wide lens as anything beyond optional garnish (after up/lens)
- Walkable village
- A second Snap/Place pair for the moon
- _PaintDark colour multiplier; per-stroke hue variation in v1
- Ultra daub geometry; curl-noise and flow-map textures; a new Pat id
- BakeChunk 16 as the default
- Photo-clone or freeze seam request
- Painter Mats as a pilot dependency
- Worlds: Klimt, Mondrian (any reproduction), Op Art, Melting Time, Day over Night; Atkins until film looks exist
- Bosch full triptych and torture imagery
- Great Wave 'bring the mountain to your feet' verb
- Scanned painting images as textures in the Unity build

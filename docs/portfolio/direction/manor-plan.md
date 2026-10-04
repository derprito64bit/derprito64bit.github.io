# Manor plan (locked)

> Wave 1b M-Manager final (seven rooms per D-003, Workshop/Study design, 2 red-team seats), locked by the orchestrator in Wave 2. Owner decisions in `docs/portfolio/agents/decisions.md` take precedence.
>
> **D-022 (2026-10-03): the repo is standalone.** Every `up/*` seam step, the Seams crew and M-D021 below are
> superseded. Engine changes, new `Mat`s in `Palette.cs` and `Surface.cs` included, go straight into the owning crew's
> branch from `origin/main` (palette: #23). Crew issue numbers changed; see `docs/portfolio/agents/ISSUES.md`.
>
> **D-023 (owner, 2026-10-04): the Manor is the game's world, not the website's door.** The website (portfolio-site) is
> a standalone portfolio of all the owner's work, and the game is one project in it, with its own project page and a
> "Play" link to `/manor/`. The `canEnterManor` desktop guard stays: `publish.ps1` sends visitors who fail it to the
> game's project page on the site (`/work/<slug>/` of the content entry whose demo is `/manor/`, with `?from=manor`),
> and publishing fails if that page is missing. Anything below about the website's door, the website's "Enter the
> Manor" and "Straight to the Grand Gallery" links, the ink drip, a handoff print or the website's beat 4 (including
> a Painting Worlds teaser there) is superseded. The game's own loader buttons, "Enter the Manor" and "Go straight to
> the Grand Gallery" (M-D018, M-C5), stand, as do the Manor's rooms, budgets, loader and accessibility contract. The
> loader's "Read everything as a page" (M-D018) goes to the game's project page, the same page the guard uses.
> Decisions: `docs/portfolio/agents/decisions.md`, D-023 to D-026.

# Manor plan: final (M-Manager, Wave 1b)

Supersedes `manor-plan-draft.md`. Built on D-001 to D-012 (D-006 to D-009 at their defaults), the m-ws-designer Workshop/Study design and both red teams. Numbers marked (est.) stay estimates until M-QA's TourShots measure them.

## In one line
A candlelit royal home of seven rooms, where you enter each hero room through its own photograph.

## Rooms
| Room | Key · order · index · x | Inner size (m) | Arrival | Holds | Its one moment |
|---|---|---|---|---|---|
| Foyer | pf.foyer · 1000 · 6 · 300 | 18×24×6 | (0,0,-2) | name frieze, [ ] medallion, three doors | the far wall is a triptych: Study, Grand Gallery, Play the game |
| Grand Gallery | pf.gallery · 1010 · 7 · 350 | 10×48×7.5; three bays and an arcade bay | (0,0,-2) | 8 projects, 2 door prints, arcade | your two strongest works, at statue scale, in bay 1 |
| Hall of Honours | pf.honours · 1020 · 8 · 400 | 16×18×9, apse | medallion (0,0,-1.5) | DECA glass hero, up to 6 award plinths, medal gantry | you arrive 10 m from the hero, exactly where its print was taken |
| Camera Room | pf.lens · 1030 · 9 · 450 | 16×24×6 | (0,0,-2) | X-T5 monument, camera stand, 4 lenses, 3 films | raise the camera and the room turns into the film stock |
| Painting Wing | pf.wing · 1040 · 10 · 500 | 12×14×7 | (0,0,-2) | 1 portal at launch, 3 slots | step on the inlay and the frame takes you in (G) |
| Workshop | pf.workshop · 1050 · 11 · 550 | 14×16×7 | medallion (0,0,-1.5) | robot/CAD statue, up to 3 CAD bays, a bench | the exploded robot snaps together as you reach the mark |
| Study | pf.study · 1060 · 12 · 600 | 10×14×4.5 | (0,0,-2) | contact bureau, portrait slot, up to 5 chapters | read the letter and the desk candle lights the room |

- **Painting Worlds** are G zones `pw.*` from order 1100, so they take index 13 and up (x 650 and up).
- **No index shift:** the Workshop and Study come after the Wing in order, so the five draft rooms keep indices 6 to 10.
- **Aliases:** honours|hall, lens|lenses|camera-room, wing, workshop|robot|cad and study|about|contact. `?zone=camera` still opens the puzzle wing.
- **The curve:** welcome (Foyer), intimacy (Study, optional), anticipation (Gallery), focus (Workshop), reverence (Hall), curiosity (Camera Room), wonder (Wing), play (arcade bay).

## Circulation
- **Foyer:** Grand Gallery in the centre (wide), Study on the left, the game on the right (D-009).
- **Gallery:** [ FOYER ] on the south wall beside spawn. [ HALL OF HONOURS ] on the west wall and [ WORKSHOP ] on the east wall, both in bay 3. [ CAMERA ROOM ] on the far wall, east of the arcade.
- **The chain:** Camera Room → Wing → worlds.
- **The way back:** every other room has one [ GRAND GALLERY ] door within 3 m of where you arrive.
- **Depth:** the Study and the Wing are 2 doors from the Gallery, and a world is 3. The loader, the pause sheet and the M plan each reach the Gallery in one step.

## The signature: you enter rooms through their photograph
- **The prints:** an instant print (1.6×1.2 m) hangs beside each of the Gallery's Hall and Workshop doors.
- **How they are made:** the Gallery builds each hero's set (hero, backdrop and a patch of floor) in its own DioramaRoot. It captures the set from the target room's arrival pose.
- **The payoff:** step through and you land on the brass [ ] medallion where the photo was taken. In the Workshop, that spot is also the anamorphic viewpoint: the exploded robot looks assembled only from there.
- **The rules it keeps:** this puts photo magic on the 60-second path without pasting anything. D-008 still protects the Gallery, Hall, Workshop and Study. The Hall and Workshop stay Lazy, because the Gallery owns their shots.

## Doors
- **Architraves, not pods:**
  - a 2.4×4.2 m Niche opening, lined dark;
  - a walnut casing with a brass fillet, and lettering on the lintel;
  - a fork `Teleporter` component at the threshold, which keeps the fade.
- **Cost:** no pad mesh and no pad light. That saves about 6 batches and 1 light candidate per door (est.).
- **Drapes:** every [ GRAND GALLERY ] door outside the Wing carries red drapes, so red always means the way back.

## Look
- **Light:**
  - The key light indoors is cool and dim (start at #8E9CC8, 0.25 or less).
  - Warm light comes only from flames, shader pools and Ultra lights.
  - The fill is cool and is set after `ZoneMood.Make`.
- **Ceiling:** the darkest surface in each room. A walnut field with no coffer pattern, plus brass fillets on the cornice lip and the beam soffits (about 200 tris, 0 batches).
- **Walls:**
  - Gallery and Foyer: ivory Plaster above the walnut dado, unless the three-swatch test (Cyanotype, crimson, ivory) scores better for another.
  - Pat.Lattice is gone. Columns are stone.
- **Camera Room:** a cool, dark room with one amber safelight pool (#E39B2B, the Front Door's accent) over the print table.
- **Wing:** no candles.
- **Candles:**
  - one 12-candle chandelier per room, in the first view; any others have 7 candles;
  - a two-candle girandole above each frame and candle cups on each plinth;
  - sconces only at doors;
  - at most 8 registered LocalLight candidates per zone;
  - no electric picture lights.
- **Gates (M-QA):**
  - lights off: mud 0.35 or less, ceiling-band mud 0.25 or less, cool shadow 0.30 or more;
  - pools on: warm share 10 to 35%.

## Exhibits
- **Frames:**
  - Bay 1 holds 4.2×3.15 m works; the rest are 3.2×2.4 m. The hang height alternates by bay.
  - Until the owner supplies a work, its frame shows a Frost [ ] slide.
  - Plaques sit at 1.5 m (centre), lower right, clear of the brass rail.
  - Niche openings stop the 3 m string course at every frame and at the Hall apse.
- **Heroes:**
  - **DECA glass** (4.25 m).
  - **Camera monument:** X-T5 proportions 1:1, with the real FUJIFILM and X-T5 lettering (D-011).
  - **Robot** (3.5 m): generic, with no team number.
  - All three are modelled in Blender as convex parts and baked through `Arch.BakeLocal`: at most 8k tris and 4 batches each. The code-built HeroKit ships first and stays as the fallback.
- **Verbs:** E reads, from up to 4.5 m, and opens a DOM wall label. Each exhibit class also has one physical response, and each is still under reduced motion:
  - medals swing on the soft spring;
  - the DECA catches a sweep of light;
  - the camera can be picked up;
  - the arcade plays;
  - the Study candle lights.
- **Ropes:** only at the DECA and the robot, 1.5 to 2.0 m from the plinth.
- **Identity:**
  - The owner's name is lettered on the Foyer far-wall frieze (cap height 0.6 m) and on the Gallery's first cross-arch (0.45 m), at 4.5:1 or better with the lights off.
  - The Foyer [ ] medallion replaces the two spawn pedestals.
  - No plaque breaks mid-word.

## Budgets
Design caps. Batches here = audited renderers + world-space canvases.

| Zone | Batches / tris | Lights | Basis |
|---|---|---|---|
| Foyer | 85 / 24k | ≤8 | measured 73 / 18,664 |
| Gallery | 80 / 44k | ≤8 | measured 63 / 40,292; robot leaves |
| Hall | 90 / 45k | ≤8 | est. |
| Camera Room | 85 / 40k | ≤8 | est.; pickups use 2 Mats or fewer |
| Wing | 55 / 25k | ≤8 | est. |
| Workshop | 75 / 30k | ≤8 | est. about 50 / 15k |
| Study | 60 / 20k | ≤8 | est. about 38 / 10.5k |

- **Ultra twins:** they count on every tier, because the audit reads `enabled` while culling sets `forceRenderingOff`. Ultra-only detail is therefore Brass/Gilt or Trim only.
- **New Mats:** each new Mat in a zone costs one more batch.

## Seams (one Seams crew, in this order)
1. up/lens, plus a `PhotoHolder` place-guard hook for D-008.
2. up/palette: Gilt 24, Marble 25, Velvet 26 (only as a full TextileRed swap), Flame 27, Silver 28; pools and finishes.
3. up/zone-streaming.
4. up/manor-ux, plus an optional label font so plaques can use the bridging typeface.
5. up/web-diet.

## Crews (all on Opus 5.5 at xhigh, per the owner)
1. **M-F** first: the folder split, seven registered zones, contract stubs and content v2.
2. Then, in parallel:
   - **M-QA**;
   - **M-C1** Exhibits;
   - **M-C2** Rooms;
   - **M-C5** Overlay (owns gate G0, the loader);
   - **M-C7** Workshop & Study;
   - **M-C8** Authored heroes (holds the Blender lane).
3. As their seams land:
   - **M-C4** Look: moods now, pools after up/palette;
   - **M-C3** Camera: after up/lens;
   - **M-C6** Streaming: after up/zone-streaming.

**Gates** (the owner approves each):
- **G0:** the loader restyle.
- **G3a, the Hall slice:** the code-built DECA, its plaque and label, and its door print.
- **G3b, the Workshop slice:** the anamorphic robot and its print.

## Delete first under pressure
1. Capture DoF.
2. The Bedroom world (G).
3. The Workshop bench block, then CAD bay 3.
4. Medal swing.
5. Halve the Hall plinths.
6. Velvet.
7. The anamorphic offset (fall back to a vertical explode).
8. The door prints (plain doors).
9. Pools on Low.

The Camera Room is off the list: it is where the visitor gets the camera.

## What changed since the draft
- Seven rooms (D-003). The robot moves from the Gallery to the Workshop, and contact moves from the Foyer to the Study.
- The Gallery is narrower and taller, and its works are bigger. The Hall and the Wing shrink to fit their launch content.
- Pods become architraves, and the door prints put Viewfinder-style photo magic on the main path.
- The orange interior sun goes. The mood gate now measures mud instead of hue.
- The Blender heroes return (the owner's words), with the code-built heroes as the fallback.
- Caps cover all seven rooms and count world canvases and registered lights.

## Owner questions
Ten questions are open (see ownerQuestions): your two strongest works; the robot's project and any CAD models; your awards and the DECA glass; the anamorphic robot; contact, chapters and portrait; the bench slot; the Study door's side; the camera finish; LMB; the first link.

## Decisions (locked)

| ID | Topic | Decision | Deciding role | Serves | Budget impact |
|---|---|---|---|---|---|
| M-D001 | Room set and emotional curve (D-003) | Seven Manor zones: Foyer, Grand Gallery (the arcade is a bay at its far end), Hall of Honours, Camera Room, Workshop (robotics/CAD), Study (about and contact) and Painting Wing. Painting Worlds are G zones behind the Wing. The curve runs welcome (Foyer), intimacy (Study, optional), anticipation (Gallery), focus (Workshop), reverence (Hall), curiosity (Camera Room), wonder (Wing) and play (arcade bay). The robot/CAD statue moves from the Gallery centre line to the Workshop, and the contact bureau moves from the Foyer to the Study. There is no separate Arcade zone. | owner (D-003), applied by M-Manager | 1, 2, 3, 4 | Two more zones: Workshop about 50 batches / 15k tris and Study about 38 / 10.5k (est., WS-D08). Both are Lazy, so they cost boot time only until streaming lands. |
| M-D002 | Circulation and the always-available skip | Door graph: - Foyer: Grand Gallery (centre, wide), Study (left), game t1 (right, D-009). - Gallery: Foyer (south wall beside spawn), Hall of Honours (west wall, bay 3), Workshop (east wall, bay 3), Camera Room (far wall, east of the arcade). - Camera Room: Wing, then worlds. Hall, Workshop, Study and Wing each have exactly one [ GRAND GALLERY ] door within 3 m of arrival (Hall 2.82 m, Workshop 2.70 m, Study 2.46 m). Depth from the Gallery: 1 for Foyer, Hall, Workshop and Camera Room; 2 for Study and Wing; 3 for a world. Returns land beside the door the visitor used: - from the Hall: (-2.8, 0, 37.5), yaw 90; - from the Workshop: (2.8, 0, 37.5), yaw -90; - from the Camera Room: (3.25, 0, 41.0), yaw 180; - from the Study and the Wing: the Gallery spawn. The Gallery has exactly 4 doors and the Foyer exactly 3. Three DOM layers reach the Gallery in one step: the loader button, the first item in the pause sheet, and the first entry in the M plan. | M-Manager | 1, 2, 3 | Doors are padless (M-D024): 0 device batches and 1 lintel canvas each. The DOM layers cost 0. |
| M-D003 | Zone registry, keys and URLs | Zone registry (order, index, x): - pf.foyer: 1000, 6, 300 - pf.gallery: 1010, 7, 350 - pf.honours: 1020, 8, 400 - pf.lens: 1030, 9, 450 (title 'Camera Room') - pf.wing: 1040, 10, 500 - pf.workshop: 1050, 11, 550 (title 'Workshop') - pf.study: 1060, 12, 600 (title 'Study') Painting Worlds use G's keys, pw.* from order 1100, so they take index 13 and up and x 650 and up (the pilot is pw.starry, G-D008). Closed rooms stay within ±9 m of their root in X. StartKeyFromUrl gains these aliases: honours\|hall, lens\|lenses\|camera-room, wing, workshop\|robot\|cad, study\|about\|contact. '?zone=camera' still means the core puzzle wing. The ZoneCatalogTests check that Rooms.Count - ZoneCatalog.Count == 6 is untouched. | M-Manager | 3, 4 | None. |
| M-D004 | Room shells and sizes | ManorHall.Build gains a height parameter, Door works on any wall, and a faceted apse helper is added. Inner sizes (W x L x H): - Foyer: 18 x 24 x 6 (x ±9, z -4..20) - Gallery: 10 x 48 x 7.5 (x ±5, z -4..44) - Hall: 16 x 18 x 9 (x ±8, z -4..14, apse z 8..14) - Camera Room: 16 x 24 x 6 (z -4..20) - Wing: 12 x 14 x 7 (z -4..10) - Workshop: 14 x 16 x 7 (x ±7, z -4..12) - Study: 10 x 14 x 4.5 (x ±5, z -4..10) Every shell follows these rules: - Closed roofs use Arch.Roof with parapet 0. - Style.Base is a Mat already in the room. - The ceiling is a dark Walnut field with no coffer pattern, with Brass fillets on the cornice lip and the beam soffits. - No walkable wall uses Pat.Lattice. - Column shafts are stone (Limestone, then Marble after up/palette), never Style.Wall. | M-Manager | 1, 2, 3, 4 | About -384 tris per room for the parapets. The fillets cost about 200 tris and 0 batches. The narrower Gallery shell is about neutral in tris (est.). |
| M-D005 | Grand Gallery re-curation | The room: x ±5, z -4..44, 7.5 m high. Spawn (0,0,-2), yaw 0. The [ FOYER ] doorway is on the south wall at x -3.0. Bay 1 (z -4..12): - works 1-2 (the owner's strongest) at z 5 on both walls, 4.2 x 3.15 m, bottom edge 1.35 m (above the rail). Cross-arch A (z 12): - carries the owner's name frieze, facing spawn, cap height 0.45 m. Bay 2 (z 12..28): - works 3-6 at z 16.5 and z 23.5, 3.2 x 2.4 m, centre y 2.6. Cross-arch B: z 28. Bay 3 (z 28..40): - works 7-8 at z 30.75, centre y 3.0; - door prints at (∓4.95, 2.75, 34.25); - [ HALL OF HONOURS ] doorway on the west wall and [ WORKSHOP ] on the east wall, both centred at z 37.5, thresholds at x ∓4.2. Arcade bay (z 40..44): - the cabinet on the centre line at ArcadeBase (0,0,42.75), facing -Z; - the [ CAMERA ROOM ] doorway on the far wall at x +3.25; - a framed 'Plan of the house' at x -3.25 (E opens the M plan). The carpet is 1.5 m wide. White mats, frame-side benches, blurb hints and toasts are removed. There are 8 frames, as ManorTests asserts. | M-Manager | 1, 2, 3 | Cap 80 batches (canvases included) / 44k tris. Estimate: about 55 mesh batches + 15 canvases. 2 pods removed (about -12), 2 PrintCards added (+2). |
| M-D006 | Hall of Honours | 16 x 18 x 9 m with a faceted apse (z 8..14). Arrival: - Arrival is the first-view [ ] medallion at (0,0,-1.5), yaw 0. - The [ GRAND GALLERY ] door is at (-2.25, 0, -3.2), 2.82 m from arrival. The DECA hero: - Base at (0,0,8.5), 10 m from arrival, set against the plain light apse wall with at least 1 m of margin. - Keyed from 30-45 degrees above. Awards and medals: - Award plinths are built from honours.json, in up to 3 pairs at x ±5.75 and z 1.0 / 3.5 / 6.0. All of them sit outside the print's ±31.8 degree frame. One pair at launch. - A medal gantry (3 shown, room for 5) stands on the east wall at z -1.0, facing -X. Finish: - Floor in Limestone/Terrazzo (Marble after up/palette). - One rope line around the DECA, 1.5-2.0 m from its plinth. - No vitrines. | M-Manager | 3 | Cap 90 / 45k. Heroes: code DECA 4 batches / 850 tris; authored heroes up to 8k tris and 4 batches each. |
| M-D007 | Camera Room | pf.lens, 16 x 24 x 6, Eager (its sample prints are diorama shots). The room: - A cool, dark darkroom: graphite walls and oak boards. - One amber safelight pool (#E39B2B, the shared accent read from identity.json) over a Frost print table. No brown fog. The monument: - The X-T5 monument stands on the axis: 1:1 proportions with the real FUJIFILM and X-T5 lettering (D-011). - It is Silver after up/palette, and never Concrete standing in for silver. The camera and its kit: - A camera stand (CameraPickup) hands Manor visitors the camera. - 4 lens plinths (50, 85, 135 and 16 mm), each with a sample print taken at that lens's FovY. - 3 film plinths: Vivid, Mono and Cyanotype (D-006 names). - Pickups use at most 2 Mats (Graphite + Brass). Plinth bodies are Soft zone geometry. Pasting photos is allowed here (D-008). The room comes off the delete list. | M-Manager | 4 | Cap 85 / 40k. Pickups cost about 2 batches each instead of 3-7 (est.). |
| M-D008 | Painting Wing | 12 x 14 x 7 m in pale plaster and limestone, the only room without candles. Portal anchors come from the PaintingWorldRegistry list: - with 1 world it hangs on the far wall, on the axis; - worlds 2-3 take staggered side slots (left z 2.0, right z 5.5). Each canvas is 3.2 x 2.56 m, with a brass threshold inlay 1.2 m in front. The canvas is a photo preview of the world or G's procedural relief, never a scan. G-C3 owns the portals. | M-Manager | 4 | Cap 55 / 25k. |
| M-D009 | Exhibit grammar and verbs | Display types: - frame: project; - plinth statue: award (Hall); - robot/CAD statue: Workshop; - gantry: medals; - lens or film plinth; - easel: print; - cabinet: playable demo; - CAD bay: CAD model; - [ ] medallion: a viewpoint. E reads (range 4.5 m, prompt 'read') and opens the DOM wall label. Each class also has one physical response, all switched off under reduced motion: - medals swing on their ribbons (soft spring, 2.08 Hz / 0.58); - the DECA catches a 1.2 s light sweep (after up/palette); - the camera is picked up; - the arcade plays; - the Study candle lights; - the robot's response is the visitor's own walk (anamorphic). No turntables and no continuous animation. | M-Manager | 3, 4 | Medals become devices with at most 2 Mats each (up to +6 batches in the Hall, est.). The sweep costs 0 batches. |
| M-D010 | Hero build method | Two paths. (a) Code-built HeroKit (PropBuild/Arch plus Frustum and ClippedHull) ships first and stays the permanent fallback. Budgets (batches / tris): - DECA: 4 / 850 - gantry: 4 / 2.5k - camera: 2 / 2.1k - robot: 2 / 1.4k (b) Authored heroes (M-C8): Blender models (GPU only, D-012) exported as convex parts named <Mat>.<part>. Each part is rebuilt as a ClippedHull and baked through Arch.BakeLocal (FlatToon UV0, no textures). Three heroes: the DECA glass, the X-T5 monument with its lettering, and the robot. Each stays at or under 8k tris and 4 batches, with no Ultra twins. HeroKit calls AuthoredHero.TryBuild(id) first and falls back to (a). RobotStatue takes an ExplodeSpec (anamorphic) on both paths. | owner-voice, applied by M-Manager | 3, 4 | Up to 8k tris and 4 batches per authored hero. |
| M-D011 | Placeholder and texture hygiene | Content v2 marks every entry placeholder:true, and every visible string reads [PLACEHOLDER: ...]. The content files are: - projects.json (+links, media, tags); - honours.json; - workshop.json (robot HeroSpec, cad[0..3], bench[0..1]); - study.json (contact, chapters[0..5], portrait). Slots are built from lists, so nothing empty exists. A frame with no cover shows a Frost [ ] slide. No runtime Texture2D is allowed in pf.* zones except photo previews. Whether owner covers count as photo previews is escalated. | M-Manager | 3 | Removes 3 runtime textures. 0 batches. |
| M-D012 | Mood family (anti-muddy-orange) | Every interior mood: - a cool, dim key (start #8E9CC8 at intensity 0.25 or less, tuned with TourShots); - a cool blue-violet fill, assigned after ZoneMood.Make; - a cool plum-blue fog horizon, #1C1A2E. Warm light comes only from flame cones, shader pools and Ultra LocalLights. The ceiling is the darkest value in every room. Per room: - Foyer: the brightest room. - Gallery: fog starts at 14 m. - Hall: dark nave, bright apse. - Camera Room: cool and dark, with one amber safelight pool. - Wing: pale, no candles. - Workshop: the clearest light (a neutral-cool key, start #DCE3F0 at 0.45 or less). - Study: reads unlit. StudyLit raises the key by 0.18 and lowers fog density from 0.028 to 0.020 over 2 s. Gates, measured with mud_metric on lights-off TourShots at every walk spot: - mud 0.35 or less; - ceiling-band mud 0.25 or less; - cool shadow 0.30 or more. With pools on, the warm share is 10-35%. | M-Manager | 1, 2, 4 | Shader globals only: 0 batches, 0 tris. |
| M-D013 | Candle hierarchy and Ultra lights | Real LocalLights go only on: - chandeliers; - the two door candelabras at the Gallery entries; - the Workshop lanterns; - the Study hearth and desk candle. Each zone has at most 8 registered candidates. Doors are padless, so pads add none. Fixtures: - one 12-candle chandelier per room, in its first view; any others are 7-candle single rings; - above each frame, a two-candle girandole about 0.35 m up (a shader pool, no LocalLight); - on each plinth, two candle cups (pools); - sconces only at doors, with no light; - no electric picture lights; - never an Ultra twin on a chandelier ring. Light candidates per room: - Foyer: 4 - Gallery: 5 - Hall: 4 - Camera Room: 3 - Wing: 2 (cool) - Workshop: 3 - Study: 2 | M-Manager | 1, 4 | Gallery about -1.5k tris. Light candidates drop from 25 to 5. |
| M-D014 | Shader light pools on every tier | Kept from the draft. up/palette adds 8 global pools to FlatToon (toon-banded, applied before fog), fed by Look/ManorGlow: - nearest N per tier: Low 4, Medium and High 8, Ultra 8 minus the active real lights; - flicker ±8% at 6-8 Hz; under reduced motion, breathing only (±3% below 0.5 Hz). Added pool kinds: - safelight: amber, no flicker; - sweep: the DECA response. Fallback: emissive flames plus moods. | M-Manager | 1, 4 | 0 batches and 0 tris. GPU about 8 x 20 ALU per pixel (est.); Low tier pools on vs off must stay within 1.0 ms. |
| M-D015 | Materials request and wall field | up/palette appends: - Gilt 24 (metal ramp); - Marble 25 (+ Pat.Marble 18); - Velvet 26 (sheen); - Flame 27; - Silver 28 (camera monument and silver medals). Each adds +1 batch in every zone that uses it. Usage rules: - Velvet ships only if it replaces every TextileRed use in the pf.* zones. - Gilt goes only on relief edges (frames, cornice lip, chandeliers), at most 10% of the visible wall. - Brass stays for plaques, markers, fillets and the accent. Gallery and Foyer walls are ivory Plaster above the walnut dado (0 new Mats), unless the three-swatch test (Cyanotype, crimson, ivory under pools, scored with mud_metric) picks another. Glass and Crystal are deferred. | M-Manager (Lead A signs off on finishes) | 3, 4 | +1 to +4 batches per zone (est.). |
| M-D016 | Flames | Kept from the draft: every candle top becomes an outer 6-sided Flame cone (r 0.022 m, h 0.09 m) around the Warm core. No billboard cards and no moving flame meshes. | M-Manager | 4 | +1 batch per zone (Flame), about 10 tris per cone. |
| M-D017 | Camera feature set | Kept from the draft: - five primes: 16, 28 (default), 50, 85 and 135 mm; ViewFovFor keeps 70 degrees at the default; - in camera mode: wheel = lens, Q/E = aperture, F = film; - five film looks named per D-006 (Standard, Candle, Vivid, Mono, Cyanotype), baked into the print; - the print develops over 2.4 s; - capture-time DoF comes last. Added: - D-008 protection through the up/lens PhotoHolder place-guard hook. Place is refused in pf.foyer, pf.gallery, pf.honours, pf.workshop and pf.study, with a prompt line, and allowed in pf.lens and pw.*. - The X-T5 viewmodel uses M-C8's LOD of at most 2.5k tris, with the real lettering (D-011). It falls back to Geo primitives until that LOD exists. | M-Manager | 4 | Viewmodel at most 2.5k tris, outside zone budgets. Print: 2 draws while visible. |
| M-D018 | Onboarding and identity at the thresholds (gate G0) | The loader restyle ships first (G0): - candlelit, with the [ ] monogram and the owner's name; - three control rows (walk, look, use) plus 'Esc steps back out'; - two buttons: 'Enter the Manor' and 'Go straight to the Grand Gallery'; - 'Read everything as a page' is the first focusable item; - removed: the graph paper, the '[project]ion' title, the 12-row controls wall and 'Built with Unity'. The skip uses the fork's travel request, so no seam is needed. In the world: - The owner's name is lettered on the Foyer far-wall frieze (cap height at least 0.6 m, about 21 px at spawn) and on the frieze of the Gallery's cross-arch A (at least 0.45 m, about 23 px at 14 m). Both reach at least 4.5:1 with the lights off. - A 116-tri [ ] medallion sits on the Foyer runner at z 7 (top at y 0.035 or higher, never coplanar). It replaces the two spawn pedestals. - No plaque wraps mid-word (a test checks this). | M-Manager | 1 | 0 Unity cost for the loader. Friezes cost 1 canvas each. The medallion is 116 tris, 0 batches. |
| M-D019 | Overlay architecture and accessibility | Kept from the draft: one window.ionOverlay module with an exclusive slot behind the jslib API, holding the wall label, the 'Plan of the house' and the arcade bezel. Defaults sit under :where(:root), so the site tokens win. Changes: - The plan lists all 7 rooms, Grand Gallery first. - The Study bureau opens the contact label. - Robot and CAD project labels add 'See it in the Workshop' (map travel). - The OS reduced-motion preference seeds Feel.ReducedMotion. | M-Manager | 2, 3, 4 | 0 batches. |
| M-D020 | Streaming load classes | Eager: the core six, Foyer, Gallery and Camera Room. Lazy: Hall, Wing, Workshop and Study. None of them registers a diorama shot, because the Gallery owns their door prints. Unloadable: pw.*, under the pin rule. The core six stay Eager, since they own diorama shots that phase-1 streaming forbids in streamed rooms. | M-Manager | 1, 3, 4 | Boot builds 4 fewer zones once streaming lands (est.). |
| M-D021 | Seam order | One Seams crew, one seam at a time: 1. up/lens, plus the PhotoHolder place-guard hook; 2. up/palette, plus Silver 28; 3. up/zone-streaming; 4. up/manor-ux, plus an optional label font; 5. up/web-diet. | M-Manager (orchestrator confirms) | 4 | None directly. |
| M-D022 | Budgets and evidence | Seven-zone design caps under the frozen 120 / 60k (batches = audited renderers + world-space Canvas count): - Foyer: 85 / 24k - Gallery: 80 / 44k - Hall: 90 / 45k - Camera Room: 85 / 40k - Wing: 55 / 25k - Workshop: 75 / 30k - Study: 60 / 20k At most 8 registered LocalLight candidates per zone. Ultra twins count on every tier, so Ultra-only detail is Brass/Gilt (+1 batch once per zone) or Trim (+0). Every PR carries TourShots PNG + JSON (Low, Ultra, lights-off; pools-off after up/palette) at its head SHA. | M-Manager | 3, 4 | Defines the budget. |
| M-D023 | Folder split, ownership and contracts | Assets/Portfolio/Runtime/ splits into: - Registry/ and Content/; - Rooms/ with Core/, Foyer/, Gallery/, Honours/, CameraRoom/, Wing/, Workshop/ and Study/; - Exhibits/, Authored/, Camera/, Look/, Overlay/ and Streaming/; - PaintingWorlds/ with Look/, StarryNight/ and Portals/. Assets/Portfolio/Models/ is added, plus a Tests/EditMode asmref. M-F seeds every folder with .meta files and compiling stubs. After that the globs are disjoint. Stubbed contracts: - HeroKit (+ExplodeSpec) - AuthoredHero.TryBuild - ExhibitUsable.Attach(..., onRead) - ManorGlow.Register - RoomLighting.Light - CameraRoomKit.Furnish - ManorDoor.Build - DoorPrint.Hang - HallSet.ApseBay - WorkshopSet.StatueBay - PaintingWorldRegistry.Register - ZoneLoads | M-Manager | 3, 4 | None. |
| M-D024 | Door kit: architraves, not pods | ManorDoor (Rooms/Core) is built from: - a Niche opening 2.4 x 4.2 m (3.2 x 4.8 m for the Foyer's Gallery door), 0.5 m deep, lined in Graphite; - a Walnut casing with a Brass fillet; - a lettered lintel (1 canvas); - the public Ion.Gameplay.Teleporter component on a fork GameObject at the threshold. Its trigger is 1.6 x 2.4 x 1.6 m, centred 0.8 m in front of the wall face. It keeps the Frost fade and the photo-copy registry. There is no PropKit teleporter pad and no pad LocalLight. Red drapes hang on every [ GRAND GALLERY ] door except the Wing's. Only each room's final Teleport is a Solution. | M-Manager | 1, 2 | About -6 batches and -1 light candidate per door (est.). Foyer about -12, Gallery about -12. |
| M-D025 | Door prints: the Manor signature | Two at launch, both in the Gallery's bay 3: - a 1.6 x 1.2 m Polaroid PictureFrame beside the Hall door and another beside the Workshop door, centre y 2.75; - each is bound to a DioramaShot registered by the Gallery. The Gallery builds each set in its own DioramaRoot with the target crew's shared builder (HallSet.ApseBay or WorkshopSet.StatueBay). Each set is walled, so its shot sees nothing else. Each shot is captured from the target room's arrival pose (eye 1.62 m, 50 degrees, 4:3), and arrival equals that pose: a test checks within 0.05 m and 0.5 degrees. The Camera Room door stays plain. The prints are cut position 8. | M-Manager | 2, 4 | +2 PrintCard batches in the Gallery. The replica sets total at most 4k tris in the diorama; that they are not audited needs verifying. +2 captures at boot. |
| M-D026 | Workshop | Built per WS-D03, D05, D07, D09 and D10. Inner size 14 x 16 x 7 m. Arrival and viewpoint: - Arrival is the brass [ ] viewpoint medallion at (0,0,-1.5), yaw 0. It is a ManorKit medallion, not PropKit.StandingMarker, so no art-bible §7.5.2 exception is needed. - The anamorphic robot is scaled about the eye E = (0, 1.62, -1.5) by p' = E + k(p - E), with k from 0.68 to 1.16; the tower and base keep k = 1. The robot: - Base centre at (0, 0.8, 8.0), on a 0.5 m plinth and a 0.3 m Solid dais (x ±1.75, z 4..10). - Brass trail rods. - A rope line 1.5-2.0 m out. Room contents: - a Cyanotype blueprint backdrop; - CAD bays on the west wall, built from a list (2 at launch, up to 3); - an oak bench with a shadow board and calipers; - an 'on the bench' slot; - 3 six-candle lanterns (the key light at (0, 5.0, 3.5), about 36 degrees above the face). Exit: the [ GRAND GALLERY ] door at (-2.25, 0, -3.0). Mood: the clearest neutral-cool key (M-D012). | M-Manager | 2, 3, 4 | Cap 75 / 30k, estimate about 50 / 15k. At most 4 light candidates. |
| M-D027 | Study | Built per WS-D06, D07, D09 and D10. Inner size 10 x 14 x 4.5 m. Arrival (0,0,-2). One axis runs through the room: - a red runner; - a Walnut bureau at (0, 0, 5.6) with the letter and desk candle; - a Limestone hearth on the far wall, with Flame cones and a mantel at 1.25 m; - a portrait slot at (0, 2.6, 9.5): a Frost [ ] slide until the owner supplies one. Around it: - Oak bookcases on both walls; up to 5 face-out chapter books on the west case (3 at launch). - A lectern with 'Read everything as a page' at (3.4, 0, -1.0). - The [ GRAND GALLERY ] door at (2.25, 0, -3.0). The letter: - E at the letter opens the contact label, lights the desk candle and blends to StudyLit over 2 s (instant under reduced motion). - Restart unlights the room. No photo trick here. | M-Manager | 1, 3 | Cap 60 / 20k, estimate about 38 / 10.5k. 2 light candidates. |
| M-D028 | Room detail rules | - Pass one Niche Opening per frame and one across the Hall apse backdrop to Arch.Wall, so the 3.0 m string course stops at every frame. - Plaque centres at 1.5 m, at the frame's lower right, at least 0.03 m above the rail top. - Rope lines only at the DECA and the robot: Soft, with one collider box. - Drapes only on [ GRAND GALLERY ] doors. - A fireplace only in the Study. - No dentils. - Medallions only at viewpoints: Foyer spawn, Hall arrival, Workshop arrival. - 4 Foyer columns, at the carpet's end. - Plinth bodies Soft, with a collider box. | M-Manager | 1, 2, 3 | Niches about 480 tris in the Gallery. Ropes about 524 tris each. Columns about -288 tris. Each item is 0 batches. |
| M-D029 | Crew model policy | Every M crew, red-team seat and gate agent spawned from this plan runs on Opus 5.5 at xhigh effort, replacing Sonnet 5.5 for subagents. | owner (relayed), recorded by M-Manager for the orchestrator | 3, 4 | None in-engine. |

## Conflicts resolved

- **m-ws-designer vs m-red-hostile vs m-red-detail:** Where the Study and Workshop doors go, and their registry orders. WS: Study off the Foyer, Workshop off Gallery bay 3, orders 1050/1060. Hostile: both off the Gallery, orders 1015/1035. Red-detail: at most 4 Gallery pads, so the doors go off the Hall or the Foyer. **Resolution:** Adopt WS: the Study door on the Foyer triptych, the Workshop door on the Gallery's bay-3 east wall, orders appended (1050/1060). The Gallery ends up with exactly 4 doors. _(rule: 5 (serves beat 3 more directly: contact is visible from spawn) then 6 (no index shift, fewer changes))_
- **m-red-hostile F4 vs m-red-detail vs draft M-D010:** How the heroes are built: Blender heroes at 6-10k tris, versus a 2.5k cap on the camera monument, versus 'Blender waits'. **Resolution:** Authored convex-part heroes at up to 8k tris and 4 batches each (M-C8). The code-built HeroKit ships first and stays as the fallback. _(rule: 1 owner-voice ('model a some absolutely huge things with extreme detail that is performance friendly ... blender mcp'))_
- **m-red-hostile F10 vs draft M-D009:** Physical responses on exhibits, versus 'one verb, no motion'. **Resolution:** E reads, plus one short physical response per exhibit class. All of them switch off under reduced motion. _(rule: North-star check words (tactile, playful) apply to every decision; 'less motion' is only the rule-6 tie-break)_
- **m-red-hostile F2 vs m-ws-designer WS-D07 vs draft M-C4 mood table:** Warm interior suns (#FFE6C2 at 0.70, #FFA65A at 0.42, #FFB46A at 0.72) versus a cool dim key. **Resolution:** A cool key in every room (#8E9CC8 at 0.25 or less; the Workshop gets a neutral-cool #DCE3F0 at 0.45 or less). Warm light only from flames, pools and lights. Mud gates replace the hue gate. _(rule: 1 owner-voice ('dim candles') plus measured evidence (mud 0.69-0.76))_
- **m-red-hostile F8 vs draft M-D004 vs m01:** Wall field: ivory stucco versus a plain Cyanotype field versus a sequence of colours. **Resolution:** Ivory Plaster in the Gallery and the Foyer by default. A three-swatch TourShot test scored with mud_metric decides before M-C4 locks the styles. The other rooms follow WS-D07 and M-D012. _(rule: 5 (whichever serves a beat more directly), decided by measurement)_
- **m-red-hostile F3 vs draft M-D005:** Gallery size and frame scale. **Resolution:** 10 x 48 x 7.5 m. Bay-1 works at 4.2 x 3.15 m; all others at 3.2 x 2.4 m with alternating hang heights. _(rule: 1 owner-voice ('overly large as if they were statues'))_
- **m-red-hostile F5 vs m-ws-designer WS-D04 vs m-red-detail device costs:** Door form: a photograph inside every opening (Eager targets only), versus a Polaroid beside the door, versus a pad costing about 6 batches. **Resolution:** Padless architrave doors everywhere (fork Teleporter component, no pad). Polaroid door prints beside the Gallery's Hall and Workshop doors, built from replica sets in the Gallery's DioramaRoot. _(rule: 5 then 6 (less code, fewer batches))_
- **m-red-hostile F6 vs D-008 (proposed) vs draft delete list:** A camera stand at Gallery bay 1, and the Camera Room on the delete list. **Resolution:** The Camera Room comes off the delete list. No camera in the Gallery: D-008 protects it, and a camera you cannot paste with frustrates. The door prints carry the photo magic on the main path. _(rule: 3 (D-008 default stands) then 5)_
- **m-red-hostile F9 vs draft vs m-red-detail cap table:** Hall and Wing sizes and caps. **Resolution:** Hall 16 x 18 x 9 with arrival on the inlay, cap 90 / 45k. Wing 12 x 14 x 7, cap 55 / 25k. _(rule: 5 (rooms sized to launch content read as finished))_
- **m-red-hostile F7 vs m-red-detail medallions:** A monogram inlay at every spawn, versus only two medallions. **Resolution:** Medallions only at viewpoints (Foyer spawn, Hall arrival, Workshop arrival). The owner's name in the architecture (Foyer and Gallery friezes) carries identity elsewhere. _(rule: 6 (delete before adding); the hostile's intent is still met)_
- **m-ws-designer WS-D03 vs art bible 7.5.2:** Reusing the brass standing marker as a viewpoint would need an art-bible exception. **Resolution:** Use the ManorKit [ ] medallion as the viewpoint mark. The standing marker keeps its Place/Snap meaning, so no exception is needed. _(rule: 3 (art-bible limits) then 6)_
- **D-011 (locked) vs draft M-C1 'the real FUJIFILM and X-T5 lettering (D-011), silver and black (D-014)':** Lettering on the camera monument. **Resolution:** The real FUJIFILM and X-T5 lettering is modelled as geometry on the monument and the viewmodel (M-C8). The [ ] monogram appears only on the strap or the print. _(rule: 3 locked decision)_
- **draft M-D007 vs m-red-hostile F2:** Camera Room: a warm darkroom with brown fog, versus cool and dark with a red safelight. **Resolution:** Cool and dark, with one amber safelight pool #E39B2B (the shared accent), which ties the room to the Front Door's Darkroom. _(rule: 3 (D-001 and D-002 identity))_
- **Front Door spring table (k/c) vs Manor Feel (f/zeta):** D-002 calls for one spring table, but the two tables differ: Front Door settle 110/11 is 1.67 Hz / 0.52, Manor settle is 2.6 / 0.82. **Resolution:** Manor fork motion uses the shared table (lens click = snap, medal swing = soft, print eject = settle). Upstream Feel Raise/Sway stay as game feel. Escalated to the orchestrator. _(rule: 3 (D-002), with 7 the orchestrator logging the decision)_
- **draft M-D003 (pf.world.*) vs G plan G-D008 (pw.starry):** Painting World key prefix. **Resolution:** Use pw.* from order 1100, so worlds take index 13 and up. _(rule: 4 (G owns worlds))_
- **m-ws-designer WS-D10 vs non-overlapping globs:** The new door tests were proposed inside ManorTests.cs, which M-C2 owns. **Resolution:** They live in M-C7's WorkshopStudyTests.cs instead. _(rule: Ownership protocol)_

## Red-team resolutions

- m-red-detail, seven-zone caps (blocker): ACCEPTED. Seven-row table in M-D022. The robot moves to the Workshop. Gallery tris 44k. Workshop 75 / 30k as proposed. Study 60 / 20k, between the 70 / 25k proposed and WS's 55 / 20k.
- m-red-detail, canvases invisible to the audit (blocker): ACCEPTED. Verified at ArchBake.cs:130-132. The M-QA guard counts audited batches plus world Canvases. One plaque per exhibit and one per door. The spawn pedestal plaques are deleted.
- m-red-detail, candle fixtures and light candidates: ACCEPTED as M-D013. Girandoles and cups as pools. At most 8 registered candidates. One 12-candle chandelier per room. Padless doors remove the pad lights too.
- m-red-detail, Ultra-only detail is not budget relief: ACCEPTED. Verified: GameBootstrap.cs:386-389 uses forceRenderingOff while the audit reads enabled. Ultra-only detail only in Brass/Gilt or Trim. No twinned chandelier rings.
- m-red-detail, ceiling and crown moulding: ACCEPTED, combined with m-red-hostile F2. A dark Walnut ceiling field with Brass fillets on the cornice lip and beam soffits. No dentils.
- m-red-detail, string course through the paintings: ACCEPTED. Niche openings per frame and across the apse (Arch.Wall takes params Opening[], Arch.cs:236-238). M-C2 acceptance checks for no course pixels inside frames.
- m-red-detail, plaques on the brass rail: ACCEPTED. Centre at 1.5 m, lower right, at least 0.03 m above the rail top.
- m-red-detail, Mat swaps are additions: ACCEPTED. Velvet ships only as a full TextileRed replacement. Gilt only on relief edges, 10% or less.
- m-red-detail, devices bake one renderer per Mat: ACCEPTED. Pickups use 2 Mats or fewer, with Soft plinth bodies. The 4-pad Gallery limit is met, and pads disappear entirely (M-D024).
- m-red-detail, floor medallions: ACCEPTED, MODIFIED. Medallions only at viewpoints: the Foyer spawn, the Hall arrival and the Workshop arrival. The Hall inlay and arrival are now the same point.
- m-red-detail, rope stanchions: ACCEPTED. Two rope lines only (DECA, robot), Soft with one collider box, 1.5-2.0 m out. None in the Gallery.
- m-red-detail, picture lights / drapes / fireplace: ACCEPTED. No electric picture lights. Drapes only on [ GRAND GALLERY ] doors, not in the Wing. A fireplace only in the Study.
- m-red-detail, hidden geometry, Solid plinths, Foyer columns: ACCEPTED. Parapet 0 (the parameter exists, Arch.cs:680). Style.Base set to an in-room Mat. Soft plinths with a collider. 4 stone columns.
- m-red-detail, the 2.5k cap on the camera monument: REJECTED in favour of owner voice and D-011. The authored monument may use up to 8k tris and must carry the real lettering.
- m-red-hostile F1, five-room plan (blocker): ACCEPTED, using WS's appended orders (1050/1060) rather than 1015/1035, so no index shifts. M-D001, the answered owner question and the Workshop/Study cut lines are removed.
- m-red-hostile F2, muddy orange and a blind gate (blocker): ACCEPTED. A cool key in every room. The Camera Room is cool and dark, with an amber (not red) safelight to bridge D-001. Gates: mud 0.35 or less, ceiling-band mud 0.25 or less, cool shadow 0.30 or more.
- m-red-hostile F3, Gallery scale (blocker): ACCEPTED, MODIFIED. 10 x 48 x 7.5 m (kept at 48 m long for 4 doors and the arcade bay). Works at 4.2 x 3.15 m and 3.2 x 2.4 m. The cover ruling is escalated to Lead A with a recommendation of yes.
- m-red-hostile F4, toy heroes and deferred Blender (blocker): ACCEPTED. A new crew, M-C8, builds convex-part authored heroes at up to 8k tris each. The code HeroKit stays as the fallback and as the G3a slice. Silver 28 is requested.
- m-red-hostile F5, sci-fi pods: ACCEPTED, MODIFIED. Architrave doors everywhere, with no pads. The photographs hang beside the two hero doors, not inside every opening, because Lazy targets cannot be rendered at boot.
- m-red-hostile F6, photo magic off the main path: ACCEPTED, MODIFIED. The door prints put it on the 60-second path. The Camera Room comes off the delete list. No camera stand in the Gallery: D-008 protects it.
- m-red-hostile F7, identity at both thresholds: ACCEPTED. The loader restyle is gate G0. The name frieze appears in the Foyer and the Gallery. The Foyer medallion stays, and a test checks that no plaque wraps mid-word.
- m-red-hostile F8, hotel wallpaper and teal halos: ACCEPTED. Ivory Plaster in the Gallery and the Foyer, confirmed by the three-swatch test. Stone columns.
- m-red-hostile F9, rooms sized for imagined content: ACCEPTED, MODIFIED. Hall 16 x 18 x 9 (arrival on the inlay) and Wing 12 x 14 x 7.
- m-red-hostile F10, tactile verbs deleted: ACCEPTED, MODIFIED. One response per exhibit class. Medal swing is cut position 4.
- m-red-hostile F11, arcade mall prop: ACCEPTED. Walnut and Brass with the [ ] monogram. The two runtime textures are removed.
- m-red-hostile F12, download weight: DEFERRED. The core six own diorama shots, which phase-1 streaming forbids. To be revisited once CaptureZone is proven on a Lazy room.
- m-ws-designer, all WS decisions: ADOPTED as M-D026 and M-D027 with three changes. (1) The viewpoint inlay is a medallion, not StandingMarker, so no art-bible exception is needed. (2) Moods follow the cool-key rule, not the warm suns. (3) The new door tests live in M-C7's own test file, not in ManorTests.cs.

## Seams, in order

- 1. up/lens (Lead D Gameplay + Lead A Projection/Shaders), cut from upstream/main. Files:
- InstantCamera.cs: Lens, FovY, SetLens, Aperture, Film, BeforeCapture, LensChanged; CaptureFovY stays 50.
- ViewfinderFrame.cs: settable FovY.
- FirstPersonController.cs: ViewFov and its spring, SetLensView/ClearLensView, look scaling.
- PhotoHolder.cs: skip digit, wheel and Q/E handling in camera mode. NEW: a static place-guard hook, Func<int,bool> PlaceAllowedInZone, where null means allowed; Place is refused while it returns false.
- PhotoOverlayUI.cs: reads ViewFov.
- Feel.cs: becomes partial.
- IonAtmosphere.hlsl: film branch.
- ProjectionSystem.cs: RenderInto made internal, with an optional projection.
- EditMode tests.
- 2. up/palette (Lead A), cut from upstream/main after orchestrator sign-off. Files:
- Surface.cs and Palette.cs: Gilt 24, Marble 25, Velvet 26, Flame 27, NEW Silver 28; Pat.Marble 18; finish modes.
- FlatToon.shader: _Finish 1 (metal ramp) and 2 (sheen); 8 global pools before fog.
- IonPattern.hlsl: Marble.
- IonAudio.Footsteps.cs: footstep sounds for Marble and Velvet.
- ArchKitTests.cs and PatternUVTests.cs: additions only.
- Optional: UltraFx fades lights instead of popping them.
- 3. up/zone-streaming (Lead C), stacked on up/zone-catalog once the orchestrator confirms. Files:
- Room.cs: Load = Eager / Lazy / Unloadable.
- ZoneCatalog.cs: Streaming switch defaulting to false; Create(key); SetLoad.
- GameBootstrap.cs: BuildZone and CaptureZone extracted with no behaviour change; EnsureBuilt inside GoToRoom, GoToZone and OnCheckpointRestored; UnloadIdle; timing logs.
- IonPlayTestBase.cs: AfterBoot hook and StartInZone.
- StreamingPlayTests.cs (new).
- 4. up/manor-ux (Lead D, with Lead C for room exits). Files:
- IonPointerLock.jslib and WebGLTemplates/Ion/index.html: a programmatic release does not start the cooldown.
- ClickToPlayOverlay.cs: controls-sheet provider.
- PlayerInteractor.cs: LMB-to-use behind a flag that defaults off.
- HubLightTable.cs and GalleryEnding.cs: a return to the Manor when pf.foyer is registered.
- NEW: an optional font on PropKit labels (Assets/Scripts/Levels/LevelProps.cs); by default the labels render as they do today.
- 5. up/web-diet (Lead A), cut from upstream/main. File: UniversalRenderPipelineGlobalSettings.asset. Null the ten FilmGrain textures and the SMAA Area/Search textures; Bloom is untouched. Measured at integration.

## Budgets

- Hard limits per zone (frozen, ArtPlayTests): at most 120 batches and 60,000 tris. Batches here count audited MeshRenderers plus world-space Canvases; the M-QA guard adds the canvases.
- pf.foyer: cap 85 / 24k, at most 8 registered lights (plan: 4). Measured 73 / 18,664 today with 2 pods and 6 canvases. Estimate after this plan: about 62 mesh batches + 6 canvases.
- pf.gallery: cap 80 / 44k, at most 8 lights (plan: 5). Measured 63 / 40,292 today with 11 canvases. Estimate: about 55 mesh batches + 15 canvases; about 36k tris (m-red-detail projection).
- pf.honours: cap 90 / 45k, at most 8 lights (plan: 4). Estimate only. Medal devices take at most 2 Mats each.
- pf.lens (Camera Room): cap 85 / 40k, at most 8 lights (plan: 3). Pickups take at most 2 Mats each. Estimate only.
- pf.wing: cap 55 / 25k, at most 8 lights (plan: 2, cool). Estimate only.
- pf.workshop: cap 75 / 30k, at most 4 light candidates. Estimate about 50 / 15k (WS-D08).
- pf.study: cap 60 / 20k, at most 3 light candidates. Estimate about 38 / 10.5k (WS-D08).
- pw.starry (G): at most 90 batches / 20k tris and 4 lights (G-D011).
- Per hero: authored at most 8,000 tris and 4 batches with no Ultra twins. Code fallback: DECA 4 / 850, gantry 4 / 2.5k, camera 2 / 2.1k, robot 2 / 1.4k. X-T5 viewmodel at most 2,500 tris, outside zone budgets.
- Per door: padless, 0 device batches, 1 lintel canvas (the Foyer's side doors carry 2). A door print adds 1 PrintCard batch to its source room. The replica sets total at most 4k tris in the Gallery diorama (est.; verify they are not audited).
- Ultra-only detail: Brass/Gilt costs +1 batch once per zone and Trim +0. Ultra twins count on every tier, so they never count as budget relief.
- Each new Mat in a zone costs +1 batch: Gilt, Marble, Velvet (net 0 only as a full TextileRed swap), Flame, Silver.
- GPU: pools on vs pools off on Low must stay within 1.0 ms at 1080p, otherwise Low falls back to emissive flames only. Every Place stays within 25 ms (art bible).
- Routes at 4.5 m/s (M-QA): fast path (Foyer, Gallery bay 1, the Workshop print, the Workshop inlay) within 60 s, modelled at 34 s. Everything path (Study bureau, 8 frames, Workshop, Hall at maximum, Camera Room) within 180 s, modelled at 145 s.
- Download: the first Manor visit measures 18.0 MB (gzip); up/web-diet saves about 2.8 MB raw. Each authored hero FBX is 2 MB or less in the repo, about 0.6 MB or less in Web.data (est.).
- Boot: +2 diorama captures for the door prints. Streaming target: a Lazy room's first build within 300 ms, hard cap 500 ms.

## Cross-track items (resolved by the orchestrator in decisions.md)

- Model policy (the owner's request, relayed by the harness this run): spawn every M crew, red-team seat and gate agent from this plan on Opus 5.5 at xhigh effort, replacing Sonnet 5.5 for subagents. The orchestrator applies this; I cannot change config.
- Identity, accent (D-002):
- In the world: Mat.Brass #C59A45.
- In the DOM: --ion-accent is the Front Door's amber #E39B2B, which measures 8.1:1 on darkroom #14100E and 7.72:1 for ink on amber, but only 2.14:1 on print paper, so it is never used as text on paper.
- The Camera Room's safelight pool reads the accent from identity.json.
- Bridging typeface: the D-001 direction names Bricolage Grotesque (OFL). The Manor's DOM overlays use it now. In-world plaques need the optional label-font slot in up/manor-ux, which defaults to Nunito so existing labels are unchanged. The orchestrator confirms this in tokens.json.
- Spring table: the Front Door publishes (k, c) at mass 1:
- snap 520/34 = 3.63 Hz / 0.745;
- soft 170/15 = 2.08 Hz / 0.575;
- settle 110/11 = 1.67 Hz / 0.524.
Manor fork motion consumes these: lens click = snap, medal swing = soft, print eject = settle. Upstream Feel Raise (2.6/0.82) and Sway (1.8/0.75) stay as game feel. The orchestrator logs this.
- Develop time: the Manor print develops in 2.4 s, and the Front Door must-fix asks for a readable hero print within about 2.5-3 s. Proposal: one shared --ion-develop token.
- Content schema v2 adds workshop.json (robot HeroSpec, cad[0..3], bench[0..1]) and study.json (contact, chapters[0..5], portrait). Everything stays JsonUtility-compatible. docs/portfolio/agents/owner-facts.md must be mirrored into the fork before any real content lands.
- Art bible / Lead A:
- Rule on whether owner project covers count as photo previews. Recommendation: yes, up to 8 at 512x384 or less through PrintCard; otherwise frames stay Frost slides.
- Sign off the Gilt metal-ramp and Velvet sheen finishes, the pools, and Silver 28.
- No exception is needed for viewpoint inlays: they are medallions.
- G interface:
- Worlds register as pw.* from order 1100, so index 13 and up, x 650 and up.
- Wing portal anchors: with 1 world the portal sits on the far wall axis; worlds 2-3 use side slots (left z 2.0, right z 5.5).
- Exits land at their Wing frame (G-C3) or, for the pilot, in the Gallery.
- M owns the PhotoHolder place-guard for D-008; worlds allow placement.
- Budgets doc: add the 7-row caps, the rule that world canvases count as batches, and the rule of at most 8 registered light candidates to budgets.md. The first Manor visit measures 18.0 MB.
- Release:
- publish.ps1 needs the compose-time injection hook for site/manor-overlay/**.
- The link check covers the new aliases (honours, hall, lens, camera-room, wing, workshop, robot, cad, study, about, contact).
- MCP for Unity is stripped from release builds.
- W bridge: the Darkroom handoff print should use a real Grand Gallery capture (M-QA's Gallery spawn TourShot at the integration SHA), never an AI image. Prefetch the wasm and data on 'Enter the Manor' intent.
- North-star signature (proposal for the orchestrator): 'You enter every hero room through its own photograph.' The print beside the door is taken from the brass mark you land on.
- Lock docs: register the new globs in ownership.json: Rooms/{Core,Foyer,Gallery,Honours,CameraRoom,Wing,Workshop,Study}, Authored, Models, scripts/fork/blender, Tests/PlayMode/Tour and Guards, scripts/fork/mud_metric.py.
- Sound: the Manor still falls through to the game's rooms.ogg (2.2 MB). It needs its own candlelit ambience or silence. This is owned by W-sound or the orchestrator.

## Crew briefs

Each brief is also a GitHub issue (milestone "Wave 3 - build").

### M-F

- **Goal:** Split the fork runtime into owned folders and register all seven Manor zones with working stub rooms. Stub every cross-crew contract and ship content v2 as placeholders. Every M crew can then build in parallel with no cross-edits, and the suite stays green.
- **After:** nothing
- **Owned globs:** `Assets/Portfolio/Runtime/Registry/**`, `Assets/Portfolio/Runtime/Content/**`, `Assets/Portfolio/Resources/Portfolio/*.json`, `Assets/Portfolio/Ion.Portfolio.asmref`, `Assets/Portfolio/Tests/EditMode/*.asmref`, `Assets/Portfolio/Tests/PlayMode/Registry*.cs`, `Assets/Portfolio/Tests/PlayMode/Content*.cs`, `SEED EXCEPTION (this PR only, before any other M crew starts): first versions of every stub file in the other crews' folders; ownership-check runs with -Owned 'Assets/Portfolio/**'`
- **In scope:**
  - Create Runtime/{Registry, Content, Rooms/{Core,Foyer,Gallery,Honours,CameraRoom,Wing,Workshop,Study}, Exhibits, Authored, Camera, Look, Overlay, Streaming, PaintingWorlds/{Look,StarryNight,Portals}}, Assets/Portfolio/Models/ and Tests/EditMode (asmref). Pre-commit every .meta file. The namespace stays Ion.Portfolio.
  - Move today's files with no behaviour change: ManorHall to Rooms/Core; ManorFoyer and GrandGallery to their room folders; candle and decor kit to Look/; painting frame and arcade geometry to Exhibits/; Arcade.cs and IonArcade.jslib to Overlay/; PortfolioCatalog to Content/; PortfolioRegistrar to Registry/.
  - Registry: register pf.foyer 1000, pf.gallery 1010, pf.honours 1020, pf.lens 1030, pf.wing 1040, pf.workshop 1050 and pf.study 1060. Add these StartKeyFromUrl aliases: honours|hall, lens|lenses|camera-room, wing, workshop|robot|cad, study|about|contact.
  - Stub rooms for Hall, Camera Room, Wing, Workshop and Study, each at its M-D004 size, with a spawn, one Walk solution and a padless [ GRAND GALLERY ] door within 3 m of arrival as the final Teleport. Build must work with ctx.Game == null.
  - ManorHall.Build(height = 6). Door works on any wall. Add a faceted apse helper. ManorDoor.Build: a Niche opening plus the public Ion.Gameplay.Teleporter component on a fork GameObject, with no PropKit pad. It is used by the stub rooms only; the Foyer and Gallery keep their pods until M-C2 swaps them.
  - Compiling no-op stubs:
- HeroKit.{GlassAward, MedalGantry, CameraMonument, RobotStatue(..., ExplodeSpec explode = null), Plinth, Audit};
- AuthoredHero.TryBuild(string id, Transform parent, Vector3 base, Dir facing) returning false;
- ExhibitUsable.Attach(Transform, Vector3 focus, ExhibitRef, float range = 4.5f, Action onRead = null);
- ManorGlow.Register(Transform, PoolKind);
- RoomLighting.Light(Transform, RoomContext, string key);
- CameraRoomKit.Furnish;
- DoorPrint.Hang(Transform, Vector3 centre, Dir facing, DioramaShot);
- HallSet.ApseBay(Transform);
- WorkshopSet.StatueBay(Transform);
- PaintingWorldRegistry.Register(WorldSpec);
- Streaming/ZoneLoads.
  - Content v2: ProjectEntry gains links, media, tags and placeholder. Add HonourEntry, WorkshopFile (robot HeroSpec, cad[], bench[]) and StudyFile (contact, chapters[], portrait). Ship projects.json, honours.json, workshop.json and study.json with placeholder:true and visible [PLACEHOLDER: ...] text, replacing 'Project One', 'Your role' and '2026'.
  - Add one smoke-test file per crew folder.
- **Out of scope:**
  - Any visual change to the Foyer or Gallery beyond moving files
  - Lighting, new Mats, exhibit meshes, door prints
  - Any upstream-owned file
  - Real owner facts
- **Acceptance:**
  - powershell -File scripts/ion.ps1 test is fully green. The test count is at least the baseline plus the new tests. ArtPlayTests constants are unchanged.
  - RegistryTests: the 7 keys sit at orders 1000-1060, indices 6-12 and x 300-600 (stepping 50). If pw.starry is registered, its index is 13 or more. ZoneCatalogTests passes unchanged (Rooms.Count - ZoneCatalog.Count == 6).
  - Every existing ManorTests assertion passes unchanged: 8 frames, foyer to gallery, foyer game door to t1, arcade usable with E, gallery leads home.
  - The StartKey tests cover all 13 new aliases, and '?zone=camera' still returns 'camera'.
  - Each stub room passes ArtPlayTests (pattern space, 120 / 60k, Arch.Validate with ctx.Game == null) and ProjectionPlayTests (non-empty solutions, ground within 0.15 m). Each sits within ±9 m of its root in X. Its [ GRAND GALLERY ] trigger centre is within 3.0 m of arrival.
  - A PlayMode test walks into a padless door and lands in the target zone within 3 s. The stub rooms contain 0 PropKit teleporter pads.
  - The placeholder guard passes: every visible string in pf.* zones and in the content JSON is sourced or starts with '[PLACEHOLDER'.
  - The ownership-check prints PASS, and no upstream file changes.
- **Evidence:**
  - Full-suite pass/fail summary lines and the Build/results-<platform>.xml path
  - ownership-check PASS output
  - git diff --stat at the PR head SHA
  - Before/after Foyer and Gallery PlayMode screenshots showing no visual change

### M-QA

- **Goal:** Give every crew and gate one evidence standard: TourShots, per-zone budget JSON that counts canvases and light candidates, a mud metric, route timings, and guards that catch invented facts, stray textures and too many lights.
- **After:** M-F
- **Owned globs:** `Assets/Portfolio/Tests/PlayMode/Tour/**`, `Assets/Portfolio/Tests/PlayMode/Guards/**`, `scripts/fork/tour.ps1`, `scripts/fork/mud_metric.py`, `docs/portfolio/qa/**`
- **In scope:**
  - TourShots PlayMode test: for every pf.* zone, and pw.* when present, teleport to the named viewpoints exported by each room (spawn, arrival, first view, each exhibit and walk spot, the door-print pairs). Capture 1920x1080 PNGs at Low, at Ultra, with lights off, and with pools off once up/palette lands.
  - Per-shot JSON: sha, zone, tier, viewpoint, audited batches, world Canvas count, tris, registered and enabled LocalLights, active pools, mud, ceiling-band mud, cool shadow, warm share, indicative frame ms.
  - Port m-red-hostile's mud_metric.py to scripts/fork/mud_metric.py with documented thresholds.
  - Fork guard tests (ratcheted):
- per zone, audited batches + canvases stay within the 7-row cap;
- at most 8 registered LocalLight candidates per zone, and at most 8 enabled on Ultra;
- no Light under any hero root;
- no runtime Texture2D in pf.* zones except photo previews (the allowlist holds the 3 known ManorKit generators until M-C1 empties it);
- no plaque string wraps mid-word;
- the placeholder guard.
Zones over their cap today (the Gallery has 25 lights) may not get worse than the recorded baseline. Baseline entries are deleted as the room crews bring zones under cap.
  - Route test at 4.5 m/s with dwell times. Fast path: Foyer, Gallery bay 1, Workshop print, Workshop inlay. Everything path: Study bureau, 8 frames, Workshop, Hall at maximum, Camera Room.
  - scripts/fork/tour.ps1 wraps ion.ps1 test-play -Filter TourShots in the background and copies the output to C:\Users\Aaron\Documents\GitHub\portfolio-evidence\wave3\<crew>\<sha>\. docs/portfolio/qa/tourshots.md documents the schema.
- **Out of scope:**
  - Changing room content, lights or moods
  - Editing upstream tests
  - WebGL builds
- **Acceptance:**
  - tour.ps1 runs only through ion.ps1 with run_in_background. It produces PNG + JSON for all 7 pf.* zones in every mode.
  - The JSON batch and tri numbers match ArtPlayTests for the same zone within 1%. canvasCount equals the number of world-space Canvas components under the zone root.
  - On m-red-hostile's live-02, live-03 and live-04 PNGs, mud_metric.py reproduces mud 0.76 / 0.69 / 0.70 within ±0.02.
  - The guards and the route test are green on the current content under the ratchet. The fast path takes 60 s or less and the everything path 180 s or less.
  - The full suite is green, the test count rises, and the ownership-check passes.
- **Evidence:**
  - One complete TourShots set (PNG + JSON) at the head SHA
  - mud_metric reproduction output
  - Route timing output
  - Full-suite summary lines and ownership-check PASS

### Seams

- **Goal:** Build the five upstream-owned seams on up/* branches cut from upstream/main, one at a time and behaviour-neutral by default, so no fork crew ever edits an upstream file.
- **After:** nothing
- **Owned globs:** `Assets/Scripts/Gameplay/InstantCamera.cs`, `Assets/Scripts/Gameplay/ViewfinderFrame.cs`, `Assets/Scripts/Gameplay/FirstPersonController.cs`, `Assets/Scripts/Gameplay/PhotoHolder.cs`, `Assets/Scripts/Gameplay/PlayerInteractor.cs`, `Assets/Scripts/Presentation/PhotoOverlayUI.cs`, `Assets/Scripts/Presentation/ClickToPlayOverlay.cs`, `Assets/Scripts/Presentation/Motion/Feel.cs`, `Assets/Scripts/Presentation/Surface.cs`, `Assets/Scripts/Presentation/Palette.cs`, `Assets/Scripts/Presentation/Quality/UltraFx.cs`, `Assets/Scripts/Presentation/Audio/IonAudio.Footsteps.cs`, `Assets/Shaders/FlatToon.shader`, `Assets/Shaders/IonAtmosphere.hlsl`, `Assets/Shaders/IonPattern.hlsl`, `Assets/Scripts/Projection/ProjectionSystem.cs`, `Assets/Scripts/Levels/Room.cs`, `Assets/Scripts/Levels/ZoneCatalog.cs`, `Assets/Scripts/Levels/GameBootstrap.cs`, `Assets/Scripts/Levels/LevelProps.cs`, `Assets/Scripts/Levels/Rooms/HubLightTable.cs`, `Assets/Scripts/Levels/Rooms/GalleryEnding.cs`, `Assets/Plugins/WebGL/IonPointerLock.jslib`, `Assets/WebGLTemplates/Ion/index.html`, `Assets/UniversalRenderPipelineGlobalSettings.asset`, `Assets/Editor/ProjectSetup.cs`, `Assets/Tests/EditMode/**`, `Assets/Tests/PlayMode/IonPlayTestBase.cs`, `Assets/Tests/PlayMode/StreamingPlayTests.cs`
- **In scope:**
  - Order: up/lens (with the PhotoHolder place-guard hook), then up/palette (with Silver 28), then up/zone-streaming, then up/manor-ux (with the optional label font), then up/web-diet. The file lists are in seams[].
  - Each seam ships its own tests and timing logs. Each PR names the fork crew it unblocks.
  - Ask for orchestrator sign-off on the FlatToon finishes, the pools and Silver before starting up/palette. Confirm the base for up/zone-streaming.
- **Out of scope:**
  - Any Assets/Portfolio/** file
  - Anything sent to Lets-be-strategic-here/project.ion
  - Merging (the orchestrator merges)
  - WebGL builds
- **Acceptance:**
  - Every seam: existing tests stay untouched and green; the test count rises; ArtPlayTests constants are unchanged; ownership-check.ps1 -Seam -Base upstream/main prints PASS. Shader seams compile for WebGL2.
  - up/lens:
- with no lens set, ViewFov = 70;
- CriticFixTests.cs:46-51 and RoomSolutionTests.cs:126 pass untouched;
- with film mix 0 the render is within 1/255 of before;
- with a null PlaceAllowedInZone, placement is unchanged; when it returns false, Place is refused and the photo stays held (new test).
  - up/palette:
- ArchKitTests round-trips all 29 Mats;
- Pat.Marble decodes;
- with pool count 0 the render is within 1/255 of before;
- the pool count is clamped to 8;
- the SRP Batcher stays compatible.
  - up/zone-streaming:
- with Streaming false, boot builds the same zones in the same order and checkpoint ids are unchanged;
- the mesh count is back at baseline after 5 load and unload cycles;
- a photo survives its zone unloading.
  - up/manor-ux:
- the default sheet provider and the default label font reproduce today's output (1/255 or less);
- LMB-to-use is off by default;
- the Manor return appears only when pf.foyer is registered.
- **Evidence:**
  - Per seam: diff --stat, full-suite summary lines, ownership-check -Seam PASS, and the new test names
  - Shader compile logs for up/lens and up/palette
  - Before/after TourShots pixel-diff numbers for the neutral defaults
  - StreamingPlayTests timing lines

### M-C1

- **Goal:** Statue-scale exhibits that read as monuments from the doorway on every tier. They are built in code first and use authored heroes once M-C8 delivers them, with no invented facts.
- **After:** M-F
- **Owned globs:** `Assets/Portfolio/Runtime/Exhibits/**`, `Assets/Portfolio/Tests/PlayMode/Exhibits*.cs`, `Assets/Portfolio/Tests/EditMode/Exhibits*.cs`
- **In scope:**
  - Exhibits/HeroKit, code path:
- GlassAward: the DECA glass, 4.25 m;
- MedalGantry: 1-5 medals that swing as devices with at most 2 Mats each;
- CameraMonument: 2.7 m. No lettering in code; the authored version carries it (D-011). Never Concrete standing in for silver.
- RobotStatue: 3.5 m, with ExplodeSpec as the anamorphic rule p' = E + k(p - E), Brass trail rods, and a vertical-explode fallback;
- Plinth and Audit. Add HeroMeshes.Frustum and ClippedHull.
  - HeroKit calls AuthoredHero.TryBuild(id) first and falls back to the code hero.
  - Exhibits/ExhibitKit:
- frames at 4.2 x 3.15 m (bay 1) and 3.2 x 2.4 m, with no white mat; a missing cover shows a Frost [ ] slide;
- plaques with their centre at 1.5 m, lower right, clear of the rail;
- the DoorPrint frame: Polaroid, 1.6 x 1.2 m, bound to a DioramaShot;
- the 'Plan of the house' frame;
- the arcade cabinet rebuilt in walnut and Brass with the [ ] monogram, its title as a label and the screen as its only glow, with the two runtime textures removed.
  - Attach ExhibitUsable to every hero, frame, cabinet and CAD bay.
  - Phase B (after up/palette): Gilt on frame edges, clips and rails (10% or less of the visible wall); Marble plinths; Silver medals.
- **Out of scope:**
  - Placing exhibits (M-C2, M-C7)
  - Lights, moods, pools and the DECA sweep (M-C4)
  - Panel UI (M-C5)
  - Blender modelling (M-C8)
  - Any Light component on a hero
- **Acceptance:**
  - HeroKitTests: every hero builds with ctx.Game == null, passes Arch.Validate, and has no Light component.
  - Code heroes (batches / tris, Ultra included):
- DECA: 4 / 850;
- gantry with 5 medals: 4 + 2 per swinging medal / 2.5k;
- camera: 2 / 2.1k;
- robot: 2 / 1.4k, rods included.
Heights within 5% of 4.25, 3.4, 2.7 and 3.5 m.
  - ExplodeSpec EditMode test:
- from E = (0, 1.62, -1.5), every part's projected centroid is within 0.5 px of its assembled position at 1920x1080 and 50 degrees;
- 0.30 m off to the side, the worst part shifts 0.75 degrees or more;
- k stays within 0.68-1.16, with the tower and base at k = 1.
  - AuthoredHero path: with a stub returning true, HeroKit uses the authored hero; with false, the code hero builds. Both paths pass ArtPlayTests.
  - Frame sizes are exact. Plaque centres sit at 1.5 m ±0.05 and at least 0.03 m above the rail top. Placeholder plaques start with '[PLACEHOLDER'. The glass shows only the emblem.
  - Medal swing: soft spring 2.08 Hz / 0.58, settled within 2.5 s, 0 motion under reduced motion. The Hall stays within 90 / 45k.
  - The arcade test passes unchanged, and the M-QA texture allowlist is empty after this crew.
  - Lights-off TourShots at 12 m show each hero's silhouette on Low. Gate G3a: the code DECA stands in pf.honours with its plaque, and E opens its label.
- **Evidence:**
  - TourShots of each hero at 12 m, 3 m and 1 m (Low, Ultra, lights off) at the head SHA
  - Per-hero audit JSON
  - ExplodeSpec test output
  - Full-suite summary lines and ownership-check PASS

### M-C2

- **Goal:** The Foyer, Grand Gallery, Hall of Honours, Camera Room and Painting Wing at their final sizes, with the architrave door kit and the two door prints. Every exhibit is placed for the 60-second and 3-minute paths, and the Grand Gallery is always one step away.
- **After:** M-F
- **Owned globs:** `Assets/Portfolio/Runtime/Rooms/Core/**`, `Assets/Portfolio/Runtime/Rooms/Foyer/**`, `Assets/Portfolio/Runtime/Rooms/Gallery/**`, `Assets/Portfolio/Runtime/Rooms/Honours/**`, `Assets/Portfolio/Runtime/Rooms/CameraRoom/**`, `Assets/Portfolio/Runtime/Rooms/Wing/**`, `Assets/Portfolio/Tests/PlayMode/ManorTests.cs`, `Assets/Portfolio/Tests/PlayMode/Rooms*.cs`
- **In scope:**
  - Shells per M-D004:
- roof parapet 0;
- Style.Base set to a Mat already in the room;
- a dark Walnut ceiling field with Brass fillets on the cornice lip and beam soffits;
- a Niche opening at each frame and across the apse, so the string course stops;
- stone columns;
- no Pat.Lattice.
  - Rooms/Core: the ManorDoor kit (M-D024) and DoorPrint.Hang. Swap the Foyer and Gallery pods for padless doors.
  - Foyer: the triptych, with the Study door at (-5.5, 0, 18.5), the Gallery door at (0, 0, 18.5) and the game door at (5.5, 0, 18.5). Add the name frieze (cap 0.6 m) and the [ ] medallion at z 7. Keep 4 columns. Delete the spawn pedestals and the welcome toast.
  - Gallery per M-D005: bays, frames, cross-arch A frieze, 4 doors, the two door prints (built with HallSet.ApseBay and WorkshopSet.StatueBay in its DioramaRoot), the arcade bay and the plan frame. Return landing spots per M-D002.
  - Hall per M-D006: Rooms/Honours/HallSet.ApseBay builds the shared set; the rope line goes around the DECA.
  - Camera Room shell, 16 x 24 x 6: the monument on the axis, calls CameraRoomKit.Furnish, and a door to the Wing.
  - Wing, 12 x 14 x 7: list-built portal anchors (1 world on the far wall axis; worlds 2-3 in the side slots).
  - Export walk spots, exhibit positions and door-print pairs as public constants for M-QA, M-C4 and M-C5.
- **Out of scope:**
  - Moods, fixtures and pools (M-C4)
  - Exhibit meshes (M-C1, M-C8)
  - Camera devices (M-C3)
  - Workshop and Study (M-C7)
  - Portals and worlds (G-C3)
  - Overlay UI (M-C5)
- **Acceptance:**
  - Every pf.* room passes ArtPlayTests and ProjectionPlayTests. Every existing ManorTests assertion passes; constants may move, assertions may not.
  - Inner sizes match M-D004 on the 0.25 grid, and closed rooms stay within ±9 m in X.
  - Door-graph test across the 7 rooms plus one pw.* world:
- the maximum depth from the Gallery is 3;
- every non-Gallery [ GRAND GALLERY ] trigger is within 3.0 m of arrival;
- the Gallery has exactly 4 doors and the Foyer 3.
  - There are 0 PropKit teleporter pads in pf.* zones. Each door adds 0 device batches and 1 canvas or fewer (2 for the Foyer's side doors).
  - Door prints:
- Hall arrival equals its print's shot pose within 0.05 m and 0.5 degrees;
- each preview has 0.5% or more edge pixels;
- a reviewed lights-off pair (the print beside the arrival view) shows nothing outside the set.
  - Name friezes: Foyer cap height 0.6 m or more, Gallery 0.45 m or more, each at least 4.5:1 measured on the lights-off PNG.
  - No string-course pixels inside any frame rectangle in the TourShots. No walkable wall uses Pat.Lattice, and no closed hall has a roof parapet.
  - Caps from the TourShots JSON (canvases included):
- Foyer: 85 / 24k
- Gallery: 80 / 44k
- Hall: 90 / 45k, with the code heroes
- Camera Room: 85 / 40k
- Wing: 55 / 25k
Route tests: fast path 60 s or less, everything path 180 s or less.
- **Evidence:**
  - TourShots (Low, Ultra, lights off) of every walk spot, plus JSON, at the head SHA
  - Door-print pairs (print beside the arrival view)
  - Door-graph test output and route timings
  - Full-suite summary lines and ownership-check PASS

### M-C3

- **Goal:** The owner's camera: an X-T5 in hand with collectable lenses, film looks and an instant print that develops. Everything stays lightweight and goes through the up/lens seam, and photos can never cut the protected rooms.
- **After:** M-F, Seams:up/lens
- **Owned globs:** `Assets/Portfolio/Runtime/Camera/**`, `Assets/Portfolio/Shaders/**`, `Assets/Portfolio/Resources/Portfolio/Camera/**`, `Assets/Portfolio/Tests/EditMode/Camera*.cs`, `Assets/Portfolio/Tests/PlayMode/Camera*.cs`
- **In scope:**
  - Camera/LensKit: 5 primes, ViewFovFor and LookScale. LensCollection: a PlayerPrefs bitmask, re-mounting the 28 mm on Restart.
  - LensPickup and FilmPickup are UsableBehaviours with serialized-only state, built from at most 2 Mats (Graphite + Brass).
  - FilmLook: Standard, Candle, Vivid, Mono and Cyanotype (D-006), baked into the print at capture.
  - CameraViewmodel on layer 8, using M-C8's X-T5 LOD (2.5k tris or less, with the real lettering per D-011). It falls back to Geo primitives without lettering until that LOD lands.
  - PrintEject and Ion/PrintDevelop: the print develops over 2.4 s and arrives developed under reduced motion.
  - D-008: a registrar sets PhotoHolder.PlaceAllowedInZone to refuse pf.foyer, pf.gallery, pf.honours, pf.workshop and pf.study, with a prompt line.
  - CameraRoomKit.Furnish:
- the camera stand and a refill crate;
- 4 lens plinths, each with a sample print made with RegisterDioramaShot at that lens's FovY;
- 3 film plinths;
- the Frost print table under the safelight anchor.
  - Capture DoF last.
- **Out of scope:**
  - Upstream files (up/lens)
  - The Camera Room shell (M-C2)
  - Moods and the safelight pool (M-C4)
  - Exposure triangle, zoom, manual focus, live DoF, rear LCD
  - Lens puzzles in worlds (G)
- **Acceptance:**
  - LensKitTests:
- each FovY (78.1, 50, 29.1, 17.4, 11.0) is within 0.3 degrees;
- ViewFovFor(50) = 70 ±0.01;
- the mapping is monotone and clamped to 20-88 degrees;
- LookScale(70) = 1.
  - FilmLookTests: Standard is the identity, and a Standard render is within 1/255 of film off.
  - CameraLensTests:
- with the 85 mm mounted, SnapShot gives FovY 17.4;
- the wheel changes the lens, not the photo;
- Restart re-mounts the 28 mm;
- photo clones cannot grant a lens twice.
  - ProtectionTests: Place is refused in all 5 protected zones and allowed in pf.lens and pw.starry.
  - The viewmodel never appears in a capture. Its proportions are within 2% of 129.5 : 91 : 63.8 mm.
  - Each pickup audits at 2 batches or fewer. The Camera Room stays within 85 / 40k. CriticFixTests and RoomSolutionTests are untouched and green.
  - DoF passes the focus-plane test, or ships disabled.
- **Evidence:**
  - Camera Room TourShots at the head SHA
  - 12-frame filmstrips of raise, lens swap, shutter and develop (normal and reduced motion)
  - One sample print per lens
  - Full-suite summary lines and ownership-check PASS

### M-C4

- **Goal:** Dim candles that read as royal on every tier: warm pools in cool shade, a dark ceiling with brass edges, and no muddy orange anywhere.
- **After:** M-F, M-QA
- **Owned globs:** `Assets/Portfolio/Runtime/Look/**`, `Assets/Portfolio/Tests/PlayMode/Look*.cs`, `docs/portfolio/direction/manor-look.md`
- **In scope:**
  - Phase A (fork-only, available now):
- Look/ManorLook moods for all 7 rooms plus StudyLit, per M-D012: a cool key, fill assigned after Make, horizon #1C1A2E;
- per-room ArchStyle, including the Workshop and Study Mat/Pat pairs from WS-D07;
- the three-swatch wall test.
  - Look/RoomLighting fixtures per M-D013, placed at positions exported by M-C2 and M-C7:
- chandeliers (one 12-candle per room, others 7);
- girandoles and plinth candle cups;
- door sconces without lights;
- the Workshop's three 6-candle lanterns;
- the Study hearth and desk-candle lights;
- at most 8 registered candidates per zone.
  - Brass fillets on the cornice lip and beam soffits (0 batches). Ultra-only detail only in Brass/Gilt or Trim.
  - Phase B (after up/palette):
- the ManorGlow pool feeder;
- the amber safelight pool in the Camera Room;
- the DECA light sweep on read (1.2 s; none under reduced motion);
- Flame cones;
- Gilt per M-D015; Velvet only as a full TextileRed swap;
- the Foyer candle wave (1.5 s or less).
- **Out of scope:**
  - Shader code (up/palette)
  - Exhibit and frame materials (M-C1)
  - Room geometry (M-C2, M-C7)
  - Any light on a hero
- **Acceptance:**
  - Lights-off TourShots at every walk spot in all 7 rooms: mud 0.35 or less, ceiling-band mud 0.25 or less, cool shadow 0.30 or more (mud_metric.py).
  - With pools on, the warm share is 10-35% at every walk spot.
  - Swatch test: Cyanotype, crimson and ivory under pools, scored and published. The chosen wall has the lowest mud, and halo pixels in the 170-200 degree hue range stay under 1% around pools.
  - At most 8 registered LocalLight candidates per zone, with the per-room counts matching M-D013. No LocalLight on sconces or heroes.
  - ManorGlow tests: flicker 8% or less; 0 under reduced motion; no single-frame jump above 10%. On Low at 1080p, pools on vs off costs 1.0 ms or less, otherwise Low drops to emissive only.
  - No zone exceeds its cap after Flame, Gilt and Silver. Gilt covers 10% or less of the visible wall pixels in the Gallery TourShots. If Velvet ships, no TextileRed material remains in pf.* zones.
  - The mood table is published in manor-look.md.
- **Evidence:**
  - Per room: TourShots at Low, at Ultra, with lights off and with pools off, at the head SHA
  - mud_metric JSON and swatch-test scores
  - Flicker log and frame-ms JSON
  - Full-suite summary lines and ownership-check PASS

### M-C5

- **Goal:** A non-gamer can enter the Manor, skip to the Grand Gallery, read any exhibit like a museum wall label and get back, by mouse or keyboard alone, in the shared identity's tokens. The loader goes first.
- **After:** nothing
- **Owned globs:** `Assets/Portfolio/Runtime/Overlay/**`, `Assets/Portfolio/Plugins/WebGL/**`, `site/manor-overlay/**`, `Assets/Portfolio/Tests/PlayMode/Overlay*.cs`
- **In scope:**
  - Phase 1, day one, gate G0, site/manor-overlay only: the loader restyle by compose-time script:
- candlelit, with the [ ] monogram and the owner's name;
- three control rows plus 'Esc steps back out';
- 'Enter the Manor' and 'Go straight to the Grand Gallery';
- 'Read everything as a page' as the first focusable item;
- removed: the graph paper, the '[project]ion' title, the 12-row wall and 'Built with Unity'.
  - Phase 2, after M-F merges:
- Overlay/ExhibitUsable with range 4.5 m, prompt 'read', and CanUse false for empty slots. Use passes the content JSON to IonPanelOpen and calls onRead.
- The jslib API: IonPanelOpen/Close, IonMapOpen/TakeRequest, IonArcadeOpen/Close, IonOverlayIsOpen/Serial, IonAnnounce, IonPrefersReducedMotion.
- The window.ionOverlay module: exclusive slot, focus trap and return, Esc, and history.pushState so Back closes it.
- The wall label, including the Study contact label and the 'See it in the Workshop' link.
- The 'Plan of the house' on M: an SVG plan plus a real list of all 7 rooms, Grand Gallery first.
- The arcade bezel, with tokens under :where(:root).
  - The loader skip and map travel both go through IonMapTakeRequest, so no seam is needed.
  - Seed Feel.ReducedMotion from the OS. Add a polite live region. Fix the arcade's accessibility defects (draft list).
- **Out of scope:**
  - Unity pause-card internals (up/manor-ux)
  - Room geometry and toasts (M-C2, M-C7)
  - publish.ps1 (request the injection hook)
  - Content facts
- **Acceptance:**
  - G0: the loader script, run on a copy of the template, produces 3 rows and both buttons, with 'Read everything as a page' first in focus order. The name and monogram are visible in the first screen. Visible strings contain no '[project]ion' or 'Built with Unity'.
  - Every pf.* exhibit has an ExhibitUsable, and Use passes valid JSON to an Editor bridge stub. Map travel lands within 0.5 m of the chosen spot, in the right zone, for all 7 rooms.
  - Harness page site/manor-overlay/test.html, checked in the crew's own playwright-cli session (muted config, closed afterwards):
- axe reports 0 serious issues;
- keyboard-only open, Tab cycle and close by Esc, browser Back and the Back button all work, returning focus to the canvas;
- under reduced motion, transitions are opacity only;
- contrast: ink on paper 16.5:1, accent on darkroom 8.1:1, and the accent is never text on paper;
- hit targets are 24 px or larger.
  - No overlay leaves pointer lock stuck (a test matrix of button, Esc and Back).
- **Evidence:**
  - playwright-cli screenshots at 1440 and 1280 of the loader, panel, plan and arcade, in normal and reduced motion
  - axe JSON
  - Keyboard-flow filmstrip
  - PlayMode summary lines and ownership-check PASS

### M-C6

- **Goal:** The Manor pays at boot only for what a visitor sees first, and the download shrinks, without ever breaking rewind, checkpoints, carried photos or the door prints.
- **After:** M-F, M-C2, M-C7, Seams:up/zone-streaming
- **Owned globs:** `Assets/Portfolio/Runtime/Streaming/**`, `Assets/Portfolio/Tests/PlayMode/Streaming*.cs`, `docs/portfolio/direction/manor-perf.md`
- **In scope:**
  - The Streaming/ registrar turns ZoneCatalog.Streaming on only when UNITY_WEBGL && !UNITY_EDITOR. Load classes:
- Eager: Foyer, Gallery, Camera Room;
- Lazy: Hall, Wing, Workshop, Study;
- Unloadable: the pw.* prefix.
  - Fork StreamingTests with streaming on, through the seam's AfterBoot hook:
- doors build their Lazy target under the fade;
- worlds unload when they are not pinned;
- a world photo can still be placed in the Camera Room after its world unloads.
  - Boot and build timing report. A written report on the music on-demand fetch spike. docs/portfolio/direction/manor-perf.md.
- **Out of scope:**
  - Upstream files
  - Streaming the core six
  - publish.ps1
  - Room content
- **Acceptance:**
  - The Editor and default test runs keep streaming off, and the full suite stays green.
  - With streaming on:
- boot builds only Eager zones;
- the first entry into the Hall and the Workshop builds each room within 300 ms (above 500 ms, file a type:bug against the room's crew);
- the Gallery's door prints show correctly while their target rooms are unbuilt;
- Lazy rooms register 0 diorama shots.
  - The mesh count returns to baseline after 5 world load and unload cycles. A zone holding the checkpoint or any WorldHistory change is never unloaded. A carried photo survives its world unloading.
- **Evidence:**
  - StreamingTests summary
  - Build and unload ms log lines
  - Mesh-count JSON
  - manor-perf.md at the head SHA and ownership-check PASS

### M-C7

- **Goal:** Build the Workshop and the Study as designed by m-ws-designer, with every exhibit a placeholder and both rooms within budget.
- Workshop: the robot/CAD statue as an anamorphic exploded view you assemble by walking to the mark, plus CAD bays and a bench.
- Study: contact, about and story around a lit desk candle.
- **After:** M-F
- **Owned globs:** `Assets/Portfolio/Runtime/Rooms/Workshop/**`, `Assets/Portfolio/Runtime/Rooms/Study/**`, `Assets/Portfolio/Tests/PlayMode/WorkshopStudy*.cs`
- **In scope:**
  - Workshop shell, 14 x 16 x 7 (x ±7, z -4..12):
- arrival is a [ ] viewpoint medallion at (0,0,-1.5), yaw 0 (a ManorKit-style medallion, never ctx.Marker or StandingMarker);
- the statue placed via HeroKit.RobotStatue with ExplodeSpec(E = (0, 1.62, -1.5)), base (0, 0.8, 8.0), on a 0.5 m plinth and a 0.3 m Solid dais (x ±1.75, z 4..10);
- a Soft rope line 1.5-2.0 m out, with one collider box;
- a Cyanotype backdrop, plain within 1 m of the silhouette;
- CAD bays built from workshop.json (2 at launch, up to 3);
- an oak bench, vice, shadow board with 6 outlines, 5 tools and the calipers;
- the 'on the bench' slot;
- the [ GRAND GALLERY ] door at (-2.25, 0, -3.0), which lands in the Gallery at (2.8, 0, 37.5), yaw -90.
  - Rooms/Workshop/WorkshopSet.StatueBay(Transform): the shared set (statue, dais, backdrop, floor patch, side walls) that the Gallery builds in its DioramaRoot for the door print.
  - Study shell, 10 x 14 x 4.5 (x ±5, z -4..10):
- a runner and a Walnut bureau at (0,0,5.6) with the letter and the desk candle as a Device;
- a Limestone hearth with a mantel at 1.25 m and a portrait slot at (0, 2.6, 9.5);
- Oak bookcases with chapter books from study.json (3 at launch, up to 5);
- a lectern 'Read everything as a page' at (3.4, 0, -1.0);
- the [ GRAND GALLERY ] door at (2.25, 0, -3.0), which lands at the Gallery spawn.
  - Bureau onRead: light the candle and Atmosphere.BlendTo(ManorLook.StudyLit, 2 s), instant under reduced motion. Restart unlights the room.
  - Solutions in order, per WS-D10, with the Teleport last. Export the lantern, hearth and desk-candle positions for M-C4.
  - Rooms/*/WorkshopStudyTests.cs:
- Foyer_StudyDoorLeadsToTheStudy;
- Gallery_WorkshopDoorLandsOnTheInlay;
- Study_LetterLightsTheRoomAndRestartUnlights;
- RunAll in both rooms ends in pf.gallery.
- **Out of scope:**
  - Statue mesh and ExplodeSpec math (M-C1)
  - The authored robot (M-C8)
  - Moods and fixtures (M-C4)
  - The Foyer and Gallery doors and prints (M-C2)
  - Label UI (M-C5)
  - Content facts and any visible text on props
- **Acceptance:**
  - Both rooms pass ArtPlayTests and ProjectionPlayTests. Every walk spot is on the floor, off the dais.
  - Caps (canvases included): Workshop 75 / 30k with 4 light candidates or fewer; Study 60 / 20k with 3 or fewer.
  - Gallery_WorkshopDoorLandsOnTheInlay: arrival within 0.05 m of (0,0,-1.5), yaw 0 ±0.5 degrees, and equal to the print's shot pose.
  - The return doors sit 2.70 m (Workshop) and 2.46 m (Study) from arrival, both 3.0 m or less.
  - The Study letter lights the candle, and the key reaches StudyLit ±0.02 within 2.1 s (instant under reduced motion). Restart restores the unlit state.
  - All visible strings are sourced or start with '[PLACEHOLDER'. Slots are built from lists, so an empty list builds 0 slots. Props carry no text.
  - Gate G3b: TourShots from the inlay, 'exploded' (-3.4,0,5.2) and 'behind' (3.4,0,10.8), at Low, Ultra and lights off, plus the inlay-vs-print pair, approved by the owner.
- **Evidence:**
  - TourShots set and the inlay-vs-print pair at the head SHA
  - Route timings (WS model: fast path 34 s, everything path 145 s)
  - Full-suite summary lines and ownership-check PASS

### M-C8

- **Goal:** Deliver the owner's 'absolutely huge things with extreme detail that is performance friendly': the DECA glass, the X-T5 monument and the robot statue, authored in Blender and baked into the FlatToon world as convex parts, plus the X-T5 viewmodel LOD.
- **After:** M-F
- **Owned globs:** `Assets/Portfolio/Runtime/Authored/**`, `Assets/Portfolio/Models/**`, `scripts/fork/blender/**`, `Assets/Portfolio/Tests/PlayMode/Authored*.cs`
- **In scope:**
  - Blender, GPU only (D-012): run scripts/fork/blender_gpu.py first. EEVEE previews at 1600 px or less; Cycles at 64 samples or fewer with denoise; render.threads at 4 or fewer. Acquire the blender lane, renew it every 20 minutes or less, and release it.
  - Gather X-T5 references and dimensions (129.5 x 91 x 63.8 mm, dials, controls). Model 1:1 proportions with the real FUJIFILM and X-T5 lettering as geometry (D-011). The [ ] monogram goes only on the strap or the print.
  - DECA glass, 4.25 m including the plinth: chamfered facets in Frost and Paper, an inset geometric emblem, no text.
  - Robot, 3.5 m: a generic FRC-style silhouette with no team number, season or competition. Part ids match ExplodeSpec.
  - Export convex parts named <Mat>.<part> to FBX in Assets/Portfolio/Models/Heroes/. Also export an X-T5 viewmodel LOD of 2.5k tris or less.
  - Authored/AuthoredHero.TryBuild: rebuild each part as a ClippedHull with a Surf taken from the name prefix, apply B.Exempt, and bake through Arch.BakeLocal. Two modes: Soft zone geometry, or a Device that photos capture whole.
  - Export scripts go in scripts/fork/blender/.
- **Out of scope:**
  - Placement (M-C2, M-C7)
  - Lights and moods
  - New Mats (request them through up/palette)
  - Textures of any kind
  - .blend files in git, Git LFS
  - Stock or CC models in place of the owner's heroes
- **Acceptance:**
  - Each hero is 8,000 tris or less in total and 4 audited batches or less. The viewmodel LOD is 2,500 tris or less.
  - Every part is convex: hull volume over mesh volume is 0.98 or more. Arch.Validate and ArtPlayTests pattern space pass for both modes.
  - Proportions: X-T5 bounding-box ratios within 2% of 129.5 : 91 : 63.8; DECA 4.25 m ±5%; robot 3.5 m ±5%. Lettering is present as geometry, matching the references.
  - Each FBX is 2 MB or less. No Texture2D, .blend or LFS is introduced.
  - Lights-off TourShots at 12 m show each silhouette on Low. Every Blender render log shows HIP GPU. The lane is released.
- **Evidence:**
  - EEVEE previews (front, side, three-quarter; 1600 px or less) per hero and a reference sheet with sources, in portfolio-evidence (not committed)
  - Per-hero audit JSON and convexity test output
  - TourShots at the head SHA
  - Full-suite summary lines and ownership-check PASS

## Cut list

- Robot/CAD statue in the Gallery (it moves to the Workshop, D-003)
- Contact bureau in the Foyer (it moves to the Study)
- Arcade Parlour as its own zone (it stays a bay at the Gallery's far end)
- Teleporter pods and pad lights on every Manor door (replaced by architraves)
- Pat.Lattice on walkable walls and column shafts in Style.Wall
- Warm orange interior sun keys; brown fog in the Camera Room; the hue-only warm-pixel gate
- Uniform sconce and candelabra rows; a floor candelabra per exhibit; sconce lights; more than one 12-candle chandelier per room; Ultra-twinned chandelier rings
- Electric picture lights, dentils, window and Wing drapes, Gallery rope lines, a Foyer fireplace
- Medallions anywhere except the Foyer spawn, Hall arrival and Workshop arrival
- Spawn pedestal plaques and the 'monogram stele slot'
- Hidden roof parapets on closed halls
- 4 of the 8 Foyer columns
- White mats on frames; frame-side benches; blurb hints; Foyer, painting and arcade toasts
- The 12-row controls wall, the graph-paper '[project]ion' loader card and 'Built with Unity' on the Manor loader
- Runtime-generated placeholder textures (ManorKit.cs:217, 246, 303) and the pixel 'ARCADE' marquee
- Gallery [ PLAY THE GAME ] door (D-009)
- 34 m Wing and 40 m Hall lengths; Wing slots 4-5 (extend the room when a fourth world exists)
- Turntables, statue quarter-turns, coin drops, any continuous hero animation
- Workshop: stools, pit cart, parts bins, team banners or bumper numbers, FRC field elements, sparks, VFX, tool sounds, a second hero, a sawtooth roof
- Study: armchairs, globe, clock, rug, window, plants, a guestbook of visitor photos, in-world letter text, any Viewfinder trick
- Diorama shots inside Lazy rooms (the Hall, Workshop, Study and Wing stay zero-shot)
- A camera stand in the Gallery
- Medal vitrines, Glass and Crystal Mats, Pat.Glint, translucent panes, billboard flames, decal or vertex-colour pools
- Exposure triangle, continuous zoom, manual focus, live DoF, rear LCD, live grain, 14 mm lens
- In-world 3D map; streaming the core six; prebuild on approach; brotli
- Empty frames, plinths, bays or cabinets: everything is built from lists
- Delete first under pressure, in order: (1) capture DoF; (2) the Bedroom world (G); (3) the Workshop bench block, then CAD bay 3; (4) medal swing; (5) halve the Hall plinths; (6) Velvet; (7) the anamorphic offset (fall back to a vertical explode); (8) the door prints (plain doors); (9) pools on Low

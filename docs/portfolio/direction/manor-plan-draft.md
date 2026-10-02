# Manor plan: M-Manager draft (Wave 1a)

> DRAFT. The owner overrode M-D001 with D-003 (seven rooms: the Workshop and Study stay). Wave 1b revises this draft. Owner decisions are in `docs/portfolio/agents/decisions.md`.

# Manor plan: Track M (M-Manager, north star v1.1)

One plan built from seven recon reports. Numbers marked (est.) have not been measured. Crew briefs carry the detail.

## The Manor in one line
A candlelit royal home of five rooms on one spine, with the Grand Gallery always one step away.

## Spine and rooms
- **The spine:** Foyer → **Grand Gallery** (the only hub) → Hall of Honours. A second branch runs Gallery → Camera Room → Painting Wing → world.
- **Distance:** no room is more than three doors from the Gallery, and every other room has a [ GRAND GALLERY ] pad within 3 m of where you arrive.
- **Curve:** welcome, anticipation, reverence, curiosity, wonder, play.
- **Cut:** the Workshop, the Study and a separate Arcade zone.

| Room | Key · order · x | Size (m) | Holds | Its one moment |
|---|---|---|---|---|
| Foyer | pf.foyer · 1000 · 300 | 18×24×6 | name, monogram stele, contact bureau, game door | candles wake in a wave; light the desk candle and the contact letter appears |
| Grand Gallery | pf.gallery · 1010 · 350 | 15×48×6, three bays | 8 projects, robot/CAD statue, arcade bay | the two strongest works meet you in bay 1; the statue waits on the axis |
| Hall of Honours | pf.honours · 1020 · 400 | 16×40×9, apse | DECA glass hero, ≤6 award plinths, medal gantry | the hero, top-lit in the apse, first seen from a brass inlay 10 m away |
| Camera Room | pf.lens · 1030 · 450 | 16×28×6 | X-T5 monument, camera stand, 4 lenses, 3 films, sample prints | raise the camera and the room turns into the film stock; the print develops |
| Painting Wing | pf.wing · 1040 · 500 | 12×34×7 | framed portals (1 at launch) | step on the inlay and the frame takes you in (G) |

Worlds are G zones from order 1100. URL aliases are honours, lens/lenses and wing; `?zone=camera` still means the puzzle wing.

## Exhibit grammar
- **Display types:**
  - frame = project;
  - plinth statue = robot/CAD (Gallery) or award (Hall);
  - gantry = medals;
  - lens plinth = lens;
  - easel = print;
  - cabinet = playable demo;
  - stele = monogram.
- **One verb:** **E reads** (range 4.5 m) and opens the DOM wall label. No turning statues, turntables or coin drops.
- **Content:** slots are built from lists, so nothing empty exists, and every unknown is `[PLACEHOLDER: …]`. Today's `projects.json` ("Project One", year "2026") is unmarked and is fixed first.
- **Heroes:** code-built with HeroKit on PropBuild/Arch, plus the fork's Frustum and ClippedHull. The DECA glass is self-lit Frost with brass bracket clips: it glows without transparency. Build order is DECA, gantry, camera monument, robot. Blender waits.

## Look: dim candles, not orange mud
- **One mood per room** in ManorLook:
  - Ambient fill is set *after* `ZoneMood.Make`. Make lifts ambient 55% toward white, which is why today's fill is grey.
  - Fog fades to cool plum-blue. Warm light lives only in pools and flame cores.
  - The Foyer is the brightest room and the Wing the only cool one. The Camera Room is a warm darkroom with one cool Frost table.
- **Candles have jobs:**
  - one chandelier per bay;
  - one candelabra per exhibit;
  - sconces only at doors.
- **Real lights:** Ultra lights only on chandeliers, candelabras and pads, at half today's intensity. Sconces lose theirs.
- **Shader pools (up/palette):** the nearest 8 candle pools light every tier (4 on Low). Flicker is ±8% smooth; under reduced motion there is none. Every room must still read with pools and lights off.
- **New Mats:** Gilt 24 (metal-ramp finish), Marble 25 (+Pat.Marble 18), Velvet 26 (sheen), Flame 27. Glass, Silver and Crystal are deferred.
- **Who wears what:** Brass stays for plaques, markers and the accent. Gilt goes on frames and candelabras, and Velvet on the carpets. The Gallery floor stays oak.

## Camera (up/lens, then M-C3)
- **Lenses:** five primes: 16, **28 (default, the same as today)**, 50, 85 and 135 mm. `ViewFovFor` keeps the view at 70° for the default lens, so the existing pins hold.
- **Controls:** wheel = lens, Q/E = aperture, F = film.
- **Film looks:** five, under original names, live in IonGrade and bake into the print.
- **Viewmodel and print:** an X-T5-style viewmodel on layer 8, and an instax-proportioned print that develops over 2.4 s.
- **Collection and DoF:** collected lenses persist in PlayerPrefs. Capture-time DoF comes last.
- **Cut:** the exposure triangle, zoom, manual focus and live DoF.

## Overlay and onboarding (M-C5)
- **Loader:** restyled by compose-time injection with `--ion-*` tokens. It shows three control rows, "Enter the Manor" and "Go straight to the Grand Gallery". Its first focusable item is "Read everything as a page".
- **Map:** **M** opens a DOM "Plan of the house" (an SVG plan plus a real list), with the Gallery first.
- **One overlay slot:** label, plan and arcade share one `window.ionOverlay` slot.
- **Accessibility:** focus trap and return, browser Back, and a live region. The OS reduced-motion setting seeds `Feel.ReducedMotion` through its existing setter, so no seam is needed.
- **Deleted:** the toasts and the 12-row controls wall.

## Streaming and download (up/zone-streaming, then M-C6)
- **The seam:** additive and off in the Editor and tests.
- **Load classes:**
  - Eager: Foyer, Gallery, Camera Room (its prints are diorama shots) and the core six;
  - Lazy: Hall and Wing;
  - Unloadable: worlds, under a pin rule taken from WorldHistory's single checkpoint.
- **Download:** the first visit measured 18.0 MB. up/web-diet removes the unused URP FilmGrain and SMAA textures (about 2.8 MB raw). A music-fetch spike follows.

## Seams, in order (one Seams crew, one at a time)
1. **up/lens:** InstantCamera, ViewfinderFrame, FirstPersonController, PhotoHolder, PhotoOverlayUI, Feel (partial), IonAtmosphere.hlsl, ProjectionSystem.RenderInto.
2. **up/palette:** Surface, Palette, FlatToon (finish + pools), IonPattern, IonAudio.Footsteps, EditMode tests.
3. **up/zone-streaming:** Room, ZoneCatalog (+SetLoad), GameBootstrap, IonPlayTestBase hook, StreamingPlayTests.
4. **up/manor-ux:** IonPointerLock cooldown flag, template skip hook, ClickToPlayOverlay sheet provider, Manor return from HubLightTable/GalleryEnding, LMB behind a flag.
5. **up/web-diet:** URP global settings asset.

**Why not lens → streaming → palette:**
- By the recon file lists these seams share no file, so the order follows unblock value.
- up/lens is on G's critical path.
- up/palette unblocks the look crew and the final exhibit materials.
- No development crew waits on streaming: doors call `GoToZone`, which gets `EnsureBuilt` inside the seam, and tests run with streaming off.
- The orchestrator confirms the order.

## Crews and order
1. **M-F** (split, stubs, contracts, content v2).
2. In parallel: **M-QA** (TourShots, budget JSON, route and fact guards), **M-C1 Exhibits**, **M-C2 Rooms** and **M-C5 Overlay**.
3. **Gate G3a slice:** the Hall with the DECA hero, plaque and label, approved by the owner.
4. Then, as their seams land:
   - **M-C4 Look** (moods now, pools and Mats after up/palette);
   - **M-C3 Camera** (after up/lens);
   - **M-C6 Streaming** (after up/zone-streaming).

The Seams crew starts on day one.

## Budgets (design caps under the frozen 120 / 60k)
| Zone | Batches | Tris | Basis |
|---|---|---|---|
| Foyer | 85 | 24k | measured 73 / 18,664 |
| Gallery | 80 | 47k | measured 63 / 40,292 |
| Hall | 95 | 50k | shell ≤83 + heroes 12 (est.) |
| Camera Room | 85 | 40k | est. |
| Wing | 75 | 35k | est. |

At most 8 Ultra lights are enabled anywhere, and uGUI plaques count as draws. Evidence is TourShots PNG + JSON (Low, Ultra, lights off) at the PR SHA.

## Delete first under pressure
1. Capture DoF.
2. The Bedroom world (G).
3. Fold the Camera Room into a Wing vestibule.
4. Halve the Hall plinths.
5. Drop pools on Low.
6. Drop the Velvet sheen.

## Escalated
- Accent, typeface and monogram.
- The spring table.
- The content schema v2, and whether owner covers count as photo previews.
- Art-bible sign-off for the Gilt and Velvet finishes and the pools.
- The G frame contract.
- The seam base.

## Decisions

- **M-D001 Room set and emotional curve:** Five Manor zones: Foyer, Grand Gallery (the Arcade is a bay at its far end), Hall of Honours, Camera Room and Painting Wing. Painting Worlds are G zones behind the Wing. The Workshop, the Study and a separate Arcade Parlour are cut. Contact moves to a bureau in the Foyer, which inherits the Study's moment: light the desk candle and the contact letter appears. The curve runs welcome, anticipation, reverence, curiosity, wonder, play. _(why: North star v1.1 lists exactly the Gallery, Hall, Camera Room, Painting Wing and Arcade. Every visitor passes the Foyer by second 5, but almost nobody would find a Study. All zones are built at boot (GameBootstrap.cs:117-125), so each extra zone adds load time to a page m12 measured at 18.0 MB. m01 and m02 both cut the Workshop; its robot/CAD piece becomes the Gallery statue.; beats 3, 4; budget: Two fewer zones than m01's set (est. about 150 batches and 40k tris of boot-time build avoided). The Arcade adds no zone.)_
- **M-D002 Circulation and the always-available skip:** Enfilade spine: Foyer -> Grand Gallery (the only hub) -> Hall of Honours, and Gallery -> Camera Room -> Painting Wing -> world. No room is more than 3 doors from the Gallery. Every non-Gallery room has a [ GRAND GALLERY ] pad within 3 m of its arrival point. Three DOM layers also reach the Gallery in one step: the loader button 'Go straight to the Grand Gallery', 'Grand Gallery' as the first item in the pause sheet, and the first entry in the M plan, which works in every zone including game zones and worlds. The game keeps one door, in the Foyer. _(why: The owner asked for a grand gallery 'at the very start' for visitors who skip the game, and the north star makes the skip a promise at every point. GoToZone(index, feet, yaw) can land a visitor beside the return door (GameBootstrap.cs:330-336). Keeping the game door only in the Foyer keeps ManorTests.Foyer_GameDoorLeadsToTheTutorial as it is and frees the Gallery's second far door for the Camera Room.; beats 1, 3; budget: DOM layers cost 0 batches. Doors reuse the existing pad, frame and plaque kit.)_
- **M-D003 Zone registry, keys and URLs:** pf.foyer 1000 (index 6, x 300), pf.gallery 1010 (7, 350), pf.honours 1020 (8, 400), pf.lens 1030 (9, 450, title 'Camera Room'), pf.wing 1040 (10, 500). G reserves pf.world.* from order 1100. Closed rooms stay within ±9 m of their root in X. StartKeyFromUrl gains aliases honours|hall -> pf.honours, lens|lenses|camera-room -> pf.lens and wing -> pf.wing; ?zone=camera keeps meaning the core puzzle wing. _(why: Index = 6 + rank by (order, key) and x = index*50 (ZoneCatalog.cs:132-139). ZoneAt picks the nearest root, so ±25 m is the hard limit. StartKeyFromUrl passes unknown values straight through (PortfolioRegistrar.cs:43), and IonDebug.FindRoom matches exact keys first, so the bare word 'camera' already means the core zone. A pf.lens key keeps URLs and debug lookups unambiguous.; beats 3, 4; budget: None)_
- **M-D004 Room shells and heights:** ManorHall.Build takes a height (default 6 m), Door works on any wall, and a faceted apse helper is added. Heights: Foyer 6 m, Gallery 6 m, Hall 9 m with an apse, Camera Room 6 m, Wing 7 m. Each room gets its own plan and wall surface, and Pat.Lattice comes off every walkable wall (the Gallery becomes a plain Cyanotype field). _(why: m01 found every room is the same 6 m box, but an emotional curve needs contrast in scale, so the Hall should be the one tall room. m05's heroes top out at 4.25 m, and 9 m gives 2.1x (manor-refs' 2x ceiling rule, an estimate after the David correction). m02's 8 m Gallery and 12 m Hall were sized for a 6.3 m hero we are not building. Lattice is a screen pattern in the art bible and reads as hotel wallpaper.; beats 2, 3, 4; budget: Hall walls cost about 0.5k more tris than at 6 m (est.). 0 batches.)_
- **M-D005 Grand Gallery re-curation:** Keep 15 x 48 m and 8 frames (ManorTests asserts 8). Two cross-arches at z=8 and z=24 make three bays. Bay 1 hangs catalogue slots 1-2 (the owner's two strongest) at 1.25x (3.0 x 2.25 m), one per wall; bays 2-3 hang slots 3-8 at 2.4 x 1.8 m. The robot/CAD statue (at most 3.5 m total) stands on the centre line where the carpet ends. The arcade bay sits behind it, with cabinets pulled 1.5 m off the wall and built from the demo list. Doors: [ FOYER ] on the south wall beside spawn, [ HALL OF HONOURS ] and [ CAMERA ROOM ] at the far end. One chandelier per bay (3 instead of 5). The blurb hints and toasts go, because the wall label replaces them. _(why: Today the Gallery is one 48 m tube of eight identical frames (ManorRooms.cs:149-190). Royal galleries work through a bay rhythm plus a focal object on the centre line (manor-refs §1). The 60 s path has to meet the best work first. Dropping chandeliers pays for the statue.; beats 2, 3; budget: Measured baseline 63 batches / 40,292 tris. Est. -2 chandeliers (about -2.4k tris), +robot (about +1.1k tris, +2 batches), arches about 0 batches. Cap 80 / 47k.)_
- **M-D006 Hall of Honours:** 16 x 40 m, 9 m high, with a faceted apse. The hero is the DECA glass award (4.25 m including its plinth), on the axis in the apse against a plain light backdrop with a 1 m margin, first seen from a brass inlay about 10 m away. Up to 6 award plinths stand in pairs, and one medal gantry (3 medals shown, room for 5) stands in a side bay. Both are built only from honours.json entries. Launch content: the hero, 2 award plinths and a gantry of 3, with every visible text a [PLACEHOLDER: ...]. No vitrines. Floor Limestone/Terrazzo; walnut side walls kept dim, apse bright. _(why: The owner wants awards 'overly large as if they were statues' and mentioned DECA glass and medals. The Hall holds awards and medals only, so its grammar stays pure. Vitrines would need a Glass Mat, and the gantry shows medals without one. Building from a list means no plinth exists only to exist.; beats 3; budget: Heroes about 4.8k tris base, 6.6k with Ultra twins, 12 audited batches (m05, derived). The shell must measure ≤83 batches to stay under the 95 cap.)_
- **M-D007 Camera Room:** pf.lens is 16 x 28 x 6 m: graphite walls, oak boards, a warm darkroom whose one cool light is a Frost light table. The X-T5-style camera monument (2.7 m) stands on the axis. A camera stand (the existing CameraPickup) hands Manor visitors the camera. Four lens plinths hold the 50, 85, 135 and 16 mm lenses (the 28 mm comes with the camera), each with a sample print taken through that lens. Three film-cartridge plinths hold Vivid, Mono and Cyanotype. The room is declared Eager because its sample prints are diorama shots. _(why: A Manor visitor never played the game, so the room itself must hand out the camera. Sample prints show what each lens does instead of describing it. m12's phase 1 forbids diorama shots in streamed rooms, so the room is Eager by declaration rather than forced at runtime.; beats 4; budget: Cap 85 / 40k. About 8 device draw groups plus 2 batches for the monument (est.).)_
- **M-D008 Painting Wing:** M builds the Wing shell: 12 x 34 x 7 m, pale plaster and limestone, the only cool room. Frames hang from a PaintingWorldRegistry list on a staggered 4.3 m bay grid (left wall z 5.3, 13.9, 22.5; right wall z 9.6, 18.2). Each canvas is 3.2 x 2.56 m, with a brass threshold inlay 1.2 m in front. G-C3 owns the portal transition and the world. One frame at launch, and no empty frames. The canvas is never a scanned image: it is either a photo preview of the world or G's procedural relief. _(why: Walking into a frame is the north star's beat-4 promise. The art bible allows no new textures except photo previews, so a public-domain scan is still not allowed. Building frames from a list keeps the Wing honest.; beats 4; budget: Cap 75 / 35k.)_
- **M-D009 Exhibit grammar and the one verb:** One display per exhibit type:
- frame = project;
- plinth statue = robot/CAD (Gallery) and awards (Hall);
- gantry = medals;
- lens plinth = lens;
- easel = print;
- cabinet = playable demo;
- stele = monogram;
- a Blender-model podium, built only when a model exists.
Every exhibit carries an ExhibitUsable: E (range 4.5 m, prompt 'read') opens the DOM wall label. No statue quarter-turn, no turntables, no coin-drop animation. _(why: Visitors learn the grammar once. Opening the label with E serves beat 3 directly, m05 already cut hero animation, and the conflict order prefers less motion.; beats 3; budget: 0 batches; one usable component per exhibit.)_
- **M-D010 Hero build method:** HeroKit is code-built from PropBuild/Arch primitives, plus fork helpers HeroMeshes.Frustum and ClippedHull that wrap Arch.Piece. Bodies are Soft, plinths are Solid in one Mat, and B.Exempt is applied to every hero group. Build order: DECA glass, medal gantry, camera monument, robot/CAD. The authored-mesh baker (Blender to FBX/GLB) is specified but deferred. _(why: Closed convex pieces bake, batch, cut and paste with no new infrastructure (ArchBake.cs:239-266, 508-548). Blender MCP was unreachable during recon, and the owner's real robot CAD is unknown.; beats 3, 4; budget: Per hero: DECA 4, gantry 4, camera 2, robot 2 audited batches (derived).)_
- **M-D011 Placeholder and texture hygiene:** Content v2 marks every entry placeholder:true and every visible string [PLACEHOLDER: ...]. Frames with no cover show a Frost slide with a plaque label, replacing the blurry runtime-generated textures (ManorKit.cs:217,246,303). No runtime Texture2D in pf.* zones except photo previews, plus owner covers if the orchestrator rules that they count as photo previews. _(why: Today's projects.json shows 'Project One', 'Your role' and year '2026' unmarked, and the plaques print 'PROJECT ONE', which reads as fact. That breaks non-negotiable 1. The generated washes read as unfinished fog (m01) and stretch the no-new-textures rule.; beats 3; budget: Removes 3 runtime textures. 0 batches.)_
- **M-D012 Mood family (anti-muddy-orange):** One ZoneMood per room in ManorLook. Ambient fill (blue-violet) is assigned after ZoneMood.Make, and the fog horizon is cool plum-blue. Warm light appears only in pools and flame cores. The Foyer is the brightest room. The Gallery's fog starts at 13-14 m so the far doors read. The Hall has a dark backdrop and a top light. The Camera Room is a warm darkroom with one cool Frost table. The Wing is the only cool room. Starting values are in the M-C4 brief and tuned with TourShots. _(why: Make lerps ambient 55% toward white (ZoneMood.cs:93), so today's fill is grey and the room reads muddy brown. The Candlelight horizon #2A1A18 warms every distant surface. manor-refs (estimate) says to keep the fill cool and leave orange to the falloff.; beats 2, 4; budget: Shader globals only: 0 batches, 0 tris.)_
- **M-D013 Candle hierarchy and Ultra lights:** Candles go only where they have a job: one chandelier per bay, one candelabra per exhibit pool, sconces only at doors. Real LocalLights stay only on chandeliers, candelabras and teleporter pads, at about half today's intensity (chandelier 1.8 to 0.9, candelabra 1.2 to 0.6). Sconces lose theirs (ManorKit.cs:91). The Foyer has exactly 8 candidates. _(why: Candles repeat every 8 m today, so they read as wallpaper. More than 21 registered lights in the Gallery make UltraFx toggle lights at range edges, and sconces steal slots from the chandeliers. Every room must read with the lights off.; beats 4; budget: Gallery light candidates fall from about 21 to about 10; fewer candle tris.)_
- **M-D014 Shader light pools on every tier:** up/palette adds 8 global light pools to FlatToon ForwardLit (_IonPoolPos[8], _IonPoolCol[8], _IonPoolInfo), toon-banded with half-lambert wrap and applied before fog. A fork-only feeder, Look/ManorGlow, picks the nearest N (Low 4, Medium and High 8, Ultra 8 minus active real lights) with smoothed fades. Flicker is ±8% from smooth noise at 6-8 Hz; under reduced motion there is no flicker, only breathing under 0.5 Hz of ±3%. Fallback, if the WebGL2/SRP Batcher compile or the Low-tier frame-time check fails: emissive flames plus room moods only. _(why: Below Ultra nothing lights anything today (only the 0.56 sun and dim ambient, ManorLook.cs:23-28). The owner's words are 'dim candles as lighting', and most visitors never reach Ultra. Decals and vertex-colour pools were rejected (z-fighting, batch cost, saturated vertex colour).; beats 1, 4; budget: 0 batches, 0 tris. GPU about 8 x 20 ALU per FlatToon pixel (4 on Low), an estimate to be measured.)_
- **M-D015 Materials request:** up/palette appends:
- Gilt 24 (#CFA24A, metal-ramp finish);
- Marble 25 (#E6E1D8, with Pat.Marble 18);
- Velvet 26 (#8B1A2B, sheen finish);
- Flame 27 (#FF9A3C, emission 0.9, self-lit 1).
Glass, Silver, Crystal and Pat.Glint are deferred (slots 28-29 reserved). The DECA glass is Frost. Brass stays for plaques, markers and the identity accent. Gilt replaces Brass and Mustard on frames, candelabras and rails, and Velvet replaces TextileRed on carpets. The Gallery floor stays oak; Marble goes on the Hall floor and the plinths. _(why: Brass and reference gilt are nearly the same hex, so gilt needs a view-dependent finish, not a new colour. Mat is append-only and each new Mat costs a batch, so delete before adding: vitrines are cut, so Glass is not needed yet.; beats 3, 4; budget: Net about +1 to +3 batches per Manor zone (Marble and Flame added; Gilt and Velvet replace Mustard and TextileRed), an estimate.)_
- **M-D016 Flames:** Each candle top becomes an outer 6-sided Flame cone (r 0.022 m, h 0.09 m) around the existing Warm core. No billboard cards. Flame meshes do not move. _(why: Today every flame is a single yellow prism (ManorKit.cs:25). A billboard card needs a runtime component per flame in a baked world.; beats 4; budget: +1 batch per zone (Flame). About 10 tris per cone (est.).)_
- **M-D017 Camera feature set:** Five primes: 16 mm, 28 mm (the default, identical to today's 50 degree capture), 50, 85 and 135 mm. LensKit.ViewFovFor keeps the view at 70 degrees for the default lens. Camera-mode controls: wheel = lens, Q/E = aperture, F = film, with photo selection and rotation suppressed in camera mode. Five original-named film looks (Standard, Candle, Vivid, Mono, Cyanotype) live in IonGrade and bake into the print. An X-T5-style viewmodel sits on layer 8. The instax-proportioned print develops over 2.4 s through a fork Ion/PrintDevelop shader. The lens collection persists in PlayerPrefs. Capture-time accumulation DoF comes last (Low 0, Medium 4, High 8, Ultra 12 samples). Cut: the exposure triangle, continuous zoom, manual focus, live DoF and the rear LCD. _(why: The owner asked for variable aperture, zooms, colour spaces, an X-T5-style body, collectable lenses and a lightweight print-film animation. The 50 degree pins (CriticFixTests.cs:46-51, RoomSolutionTests.cs:126) survive. The URP depth texture is off, so accumulation DoF needs no depth sampling.; beats 4; budget: Viewmodel about 1.5k tris drawn, parented under the player camera (outside zone budgets). Print: 2 draws while visible.)_
- **M-D018 Onboarding:** The loader is restyled by compose-time injection with --ion-* tokens:
- three control rows (walk, look, use) and the line 'Esc steps back out';
- buttons 'Enter the Manor' and 'Go straight to the Grand Gallery';
- 'Read everything as a page' (the Front Door) as the first focusable item.
The 12-row controls wall, the foyer welcome toast, the painting blurb toasts, the arcade toast and the 'Built with Unity' footer are deleted. A one-time 'WASD walk, mouse look' line appears only after 4 s of idling. _(why: Measured on the live build, 8 of the 12 control rows are puzzle-only. Non-gamers read the wall and leave. Hints should appear at the moment of need.; beats 1, 3; budget: 0 Unity cost.)_
- **M-D019 Overlay architecture and accessibility:** One window.ionOverlay module with an exclusive slot behind a jslib API:
- IonPanelOpen and IonPanelClose;
- IonMapOpen and IonMapTakeRequest;
- IonArcadeOpen and IonArcadeClose;
- IonOverlayIsOpen and IonOverlaySerial;
- IonAnnounce;
- IonPrefersReducedMotion.
The surfaces are a museum wall-label panel, the 'Plan of the house' on M (an SVG plan plus a real list of buttons) and the arcade bezel. Defaults sit under :where(:root) so the site tokens win. The OS reduced-motion preference seeds Feel.ReducedMotion through its existing public setter, only when no preference is stored. _(why: Three overlays would otherwise fight over Esc and focus. DOM gives keyboard and assistive-technology support for free. Feel.ReducedMotion already has a public setter (Feel.cs:170-180), so no seam is needed.; beats 2, 3, 4; budget: 0 batches.)_
- **M-D020 Streaming and download:** The up/zone-streaming seam is additive and off in the Editor and in tests. Load classes:
- Eager: the core six, Foyer, Gallery and Camera Room;
- Lazy: Hall and Wing;
- Unloadable: pf.world.*.
The fork classifies zones with ZoneCatalog.SetLoad from Streaming/. A zone may unload only when it is not current, not the start zone, not the checkpoint zone and has no WorldHistory change. Download diet: up/web-diet drops the unused URP FilmGrain and SMAA textures (about 2.8 MB raw), then a music-fetch spike runs. Brotli and streaming the core six are deferred. _(why: The first visit measured 18.0 MB on the wire (m12). WorldHistory keeps one checkpoint, so the pin rule makes the dangerous state unreachable. With the switch off, every existing test sees today's boot.; beats 1, 3, 4; budget: Boot builds 5 fewer zones once worlds exist (est.). Download about -2.8 MB raw.)_
- **M-D021 Seam order:** One Seams crew, one seam at a time: up/lens -> up/palette -> up/zone-streaming -> up/manor-ux -> up/web-diet. _(why: By the recon file lists the five seams share no file, so the order follows unblock value. up/lens is on the G critical path, because G crews start after Camera merges. up/palette unblocks the look crew and the exhibits' final materials, the most visible quality lever on every tier. No development crew waits on streaming: fork doors call GoToZone, which gains EnsureBuilt inside the seam, and tests run with streaming off. m12's argument that lens edits the boot loop does not match m08's file list.; beats 4; budget: None directly.)_
- **M-D022 Budgets and evidence:** Per-zone design caps under the frozen 120 batches / 60k tris:
- Foyer 85 / 24k;
- Gallery 80 / 47k;
- Hall 95 / 50k;
- Camera Room 85 / 40k;
- Wing 75 / 35k.
At most 8 enabled Ultra lights per zone; uGUI plaques count as draws. Every PR carries TourShots PNGs (Low, Ultra, lights off) and per-zone JSON at its head SHA. _(why: Measured baselines: Foyer 73 / 18,664 and Gallery 63 / 40,292. Ultra twins and new Mats each add a batch, so the caps keep at least 17% headroom.; beats 3, 4; budget: Defines the budget.)_
- **M-D023 Folder split, ownership and contracts:** Assets/Portfolio/Runtime splits into Rooms/, Exhibits/, Camera/, Overlay/ and Streaming/, plus Look/ (candles, decor, moods, pools) and Content/ (catalogue types). A Tests/EditMode asmref is added. M-F pre-commits .meta files and stubs every cross-crew contract (HeroKit, ExhibitUsable.Attach, ManorGlow.Register, RoomLighting.Light, CameraRoomKit.Furnish, PaintingWorldRegistry.Register), so crews never edit each other's files. _(why: Without Look/, lighting and room crews would both edit ManorKit.cs. Content/ keeps the schema in one owned place.; beats 3, 4; budget: None.)_

## Seams, in order

- 1. up/lens (Lead D Gameplay + Lead A Projection/Shaders), from upstream/main. Files:
- Assets/Scripts/Gameplay/InstantCamera.cs (Lens, FovY, SetLens, Aperture, Film, BeforeCapture, LensChanged; CaptureFovY const stays 50);
- Assets/Scripts/Gameplay/ViewfinderFrame.cs (settable FovY, ViewFov);
- Assets/Scripts/Gameplay/FirstPersonController.cs (ViewFov with its own spring, SetLensView, ClearLensView, look scaling at :477);
- Assets/Scripts/Gameplay/PhotoHolder.cs (skip digit, wheel and Q/E handling while IsCameraMode);
- Assets/Scripts/Presentation/PhotoOverlayUI.cs (:343 reads ViewFov);
- Assets/Scripts/Presentation/Motion/Feel.cs (make it partial);
- Assets/Shaders/IonAtmosphere.hlsl (_IonFilmR/G/B/P film branch in IonGrade);
- Assets/Scripts/Projection/ProjectionSystem.cs (RenderInto internal, optional Matrix4x4 projection);
- fallback only: Assets/Editor/ProjectSetup.cs RequiredShaders for Ion/PrintDevelop;
- new EditMode tests.
- 2. up/palette (Lead A), from upstream/main, after orchestrator sign-off on the finish modes. Files:
- Assets/Scripts/Presentation/Surface.cs (Mats Gilt 24, Marble 25, Velvet 26, Flame 27; Pat.Marble 18; Decode upper bound);
- Assets/Scripts/Presentation/Palette.cs (colour rows, EmissionOf, SelfLitLevel, GlowBoost, _Finish properties);
- Assets/Shaders/FlatToon.shader (_Finish modes 1 metal ramp and 2 sheen; global _IonPoolPos[8], _IonPoolCol[8] and _IonPoolInfo before IonApplyFog);
- Assets/Shaders/IonPattern.hlsl (ION_PAT_MARBLE);
- Assets/Scripts/Presentation/Audio/IonAudio.Footsteps.cs (Marble and Velvet surfaces);
- Assets/Tests/EditMode/ArchKitTests.cs and PatternUVTests.cs (additions only);
- optional: Assets/Scripts/Presentation/Quality/UltraFx.cs fade instead of popping.
- 3. up/zone-streaming (Lead C), stacked on up/zone-catalog pending orchestrator confirmation. Files:
- Assets/Scripts/Levels/Room.cs (Load: Eager, Lazy or Unloadable);
- Assets/Scripts/Levels/ZoneCatalog.cs (Streaming switch, default false; Create(key); SetLoad(key, load));
- Assets/Scripts/Levels/GameBootstrap.cs (BuildZone and CaptureZone extracted with no behaviour change; EnsureBuilt, IsBuilt, BuildAll, IsPinned, UnloadIdle; ZoneBuilt and ZoneUnloaded events; EnsureBuilt called inside GoToRoom, GoToZone and OnCheckpointRestored; per-zone build and unload ms logged);
- Assets/Tests/PlayMode/IonPlayTestBase.cs (AfterBoot hook and StartInZone helper);
- new Assets/Tests/PlayMode/StreamingPlayTests.cs.
- 4. up/manor-ux (Lead D, with Lead C for room exits). Files:
- Assets/Plugins/WebGL/IonPointerLock.jslib (a programmatic release does not start the 1000 ms re-lock cooldown);
- Assets/WebGLTemplates/Ion/index.html (the same cooldown flag, plus a skip-button hook that sends a travel request before the lock);
- Assets/Scripts/Presentation/ClickToPlayOverlay.cs (controls-sheet provider; the default is today's sheet);
- Assets/Scripts/Gameplay/PlayerInteractor.cs (LMB -> UseFocused when no photo is held, behind a flag that defaults off until the owner answers);
- Assets/Scripts/Levels/Rooms/HubLightTable.cs and Assets/Scripts/Levels/Rooms/GalleryEnding.cs (a return to the Manor, offered only when pf.foyer is registered).
- 5. up/web-diet (Lead A), from upstream/main. File: Assets/UniversalRenderPipelineGlobalSettings.asset (null the ten FilmGrain textures and the SMAA Area and Search textures; Bloom is untouched). Measured at the next integration build.

## Cross-track escalations

- IDENTITY accent: the Manor proposes Brass #C59A45 (Mat.Brass, Palette.cs:165) as the one shared accent. Measured contrast: 7.35:1 on #140E10, 4.20:1 on Graphite, 2.19:1 on paper #EFEBE3, so never use it for small text on a light ground. Gilt #CFA24A is a Manor material finish, not a second accent. Lock after the W direction tournament.
- IDENTITY type and monogram: Nunito ExtraBold and SemiBold (OFL, already shipped in Assets/Resources/Fonts, UIUtil.cs:19-27) for labels, plaques, buttons and numerals on both doors; the Front Door may add a display face. Monogram candidate: the [ ] bracket pair already used on plaques and the loader. Owner to confirm.
- TOKENS: the Manor overlays consume --ion-accent, --ion-font-display, --ion-font-ui, --ion-ink, --ion-paper, --ion-brass, --ion-wood-1, --ion-wood-2, --ion-scrim, --ion-focus, --ion-ease-out, --ion-in (220ms), --ion-out (160ms), --ion-rise (8px) and --ion-radius, with defaults under :where(:root). tokens.json should emit these names, and sync-content should copy them to Resources/Portfolio/identity.json.
- SPRINGS: the canonical table is Feel.cs, as (freqHz, zeta): settle 2.6/0.82 (Raise), lag 1.8/0.75 (Sway), FOV kick 2.0/0.80, plus a proposed click 3.2/0.60 for the lens mount. Panels: in 220 ms, out 160 ms, rise 8 px. JS equivalents at mass 1, using k = (2 pi f)^2 and c = 2 zeta (2 pi f): settle k 266.9 c 26.79; lag k 127.9 c 16.96. m01's weight 1.2/0.90 is dropped because nothing uses it.
- CONTENT SCHEMA v2:
- ProjectEntry gains optional links[{label,href}], media[{type,src,alt}], tags[] and a placeholder flag.
- New honours.json {awards[], medals[]}, each with title, event, placement, year, kind and placeholder.
- New contact {links[], about, placeholder}.
- Everything must stay JsonUtility-compatible: a wrapper object, public fields, no dictionaries.
- The rules point at docs/portfolio/agents/owner-facts.md, which does not exist in the fork. It needs mirroring before any real content lands.
- CONTENT images: do owner project covers and screenshots count as the 'photo preview' texture exception in-world? Proposal: yes, at most 8 covers at up to 512x384, compressed, shown through PhotoDisplay. Otherwise in-world frames stay Frost slides and covers appear only in the DOM label.
- BUDGETS: add the Manor per-zone design caps (Foyer 85/24k, Gallery 80/47k, Hall 95/50k, Camera Room 85/40k, Wing 75/35k) and the 8-light cap to budgets.md. The Manor's first visit is 18.0 MB (measured, gzip). The W Manor Bridge should prefetch the wasm and data on 'Enter the Manor' intent and offer a ?zone=gallery deep link as the skip.
- ART BIBLE: Gilt metal-ramp and Velvet sheen (_Finish modes) plus the shader light pools change FlatToon's 'no specular' look. They need the orchestrator's (Lead A role) sign-off before up/palette starts. The runtime-generated ManorKit textures are being removed.
- G INTERFACE:
- PaintingWorldRegistry.Register(WorldSpec{key pf.world.*, order ≥1100, title, credit, canvasAspect, spawn, returnSpawn}), called from G code.
- Worlds are laid out along Z within ±25 m in X, and Unloadable worlds register no diorama shots.
- The Wing canvas is a photo preview or a procedural relief, never a scan.
- Starry Night has 10 stars plus Venus (not 11, as the plan says).
- Film-look names are original (no Acros or Velvia).
- The lens FovY table (78.1, 50, 29.1, 17.4, 11.0) drives lens puzzles.
- The M key is reserved for the plan in every zone.
- SEAMS: confirm the revised order (lens -> palette -> zone-streaming -> manor-ux -> web-diet). Confirm whether up/zone-streaming may be stacked on up/zone-catalog, given PROTOCOL says to cut from upstream/main. Nothing is sent upstream until the owner says so.
- RELEASE: publish.ps1 needs a compose-time injection hook for site/manor-overlay/** (loader restyle, overlay CSS and JS). MCP for Unity should be stripped from release builds. The link check should cover the /manor/?zone= aliases.
- SOUND: the Foyer plays the game's rooms.ogg (2.2 MB) because AreaOfRoomName falls through to Puzzle. The Manor needs its own candlelit ambience, or silence, plus the music on-demand fetch spike.
- LOCK DOCS: decisions.md, budgets.md, tokens.json and ownership.json do not exist in the fork yet. The new M globs (Rooms, Exhibits, Camera, Look, Overlay, Streaming, Content, Tests/EditMode, site/manor-overlay) need registering in ownership.json.

## Crew briefs

### M-F

- **Goal:** Split the fork runtime into feature folders, with stub contracts and all five Manor zones registered, so every M crew can build in parallel without cross-editing and the suite stays green.
- **After:** nothing
- **Owned globs:** Assets/Portfolio/**
- **In scope:** Move Assets/Portfolio/Runtime/*.cs into Rooms/, Exhibits/, Camera/, Look/, Overlay/, Streaming/ and Content/, and pre-commit their .meta files. The namespace stays Ion.Portfolio.; Moves: ManorHall, ManorFoyer and GrandGallery go to Rooms/. The painting frame and arcade cabinet geometry go to Exhibits/ExhibitKit. Candles, chandeliers, sconces, carpet and wainscot go to Look/ManorDecor. Today's candle placement loops go to Look/RoomLighting.Light(root, ctx, key). ManorLook goes to Look/. Arcade.cs and IonArcade.jslib go to the Overlay folders. PortfolioCatalog goes to Content/.; One registrar per feature, using [RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]. PortfolioRegistrar keeps the StartKey logic and gains the URL aliases honours|hall, lens|lenses|camera-room and wing.; Stub rooms pf.honours (1020), pf.lens (1030) and pf.wing (1040): a ManorHall shell, a spawn point, one Walk solution and a [ GRAND GALLERY ] pad within 3 m of spawn. Build must work with ctx.Game == null.; ManorHall.Build gains a height parameter (default 6 m); Door works on any wall; add a faceted apse helper.; Compiling no-op contract stubs:
- HeroKit.GlassAward, MedalGantry, CameraMonument, RobotStatue, Plinth and Audit, with m05's signatures and the HeroSpec and MedalSpec types;
- ExhibitUsable.Attach(Transform parent, Vector3 focus, ExhibitRef r, float range = 4.5f);
- ManorGlow.Register(Transform anchor, PoolKind kind);
- CameraRoomKit.Furnish(Transform root, RoomContext ctx);
- PaintingWorldRegistry.Register(WorldSpec);
- Streaming/ZoneLoads (an empty table).; Content v2 types: ProjectEntry gains optional links, media, tags and placeholder; add HonourEntry and ContactEntry; ship projects.json, honours.json and contact.json with placeholder:true and visible [PLACEHOLDER: ...] text, replacing 'Project One', 'Your role' and '2026'.; Add Assets/Portfolio/Tests/EditMode/Ion.Portfolio.Tests.EditMode.asmref and one test file per crew (Rooms, Exhibits, Camera, Look, Overlay, Streaming), each with one smoke test.
- **Out of scope:** Any visual change beyond file moves and stub rooms; Lighting, new Mats or exhibit meshes; Any upstream-owned file; Real awards, contact or project facts
- **Acceptance:**
  - powershell -File scripts/ion.ps1 test is fully green. The test count is at least the pre-split baseline plus the new tests, and ArtPlayTests constants are unchanged.
  - Every ManorTests assertion passes unchanged: 8 frames, foyer to gallery, foyer game door to t1, arcade usable with E, gallery leads home.
  - pf.foyer through pf.wing sit at indices 6-10 with x 300-500, and ZoneCatalogTests passes.
  - StartKey tests cover each new alias, and '?zone=camera' still returns 'camera'.
  - Each stub room passes ArtPlayTests (pattern space, budgets, Arch.Validate with ctx.Game == null) and ProjectionPlayTests (non-empty solutions, ground within 0.15 m).
  - A new test asserts that every visible string in pf.* zones and in content JSON is either sourced or starts with '[PLACEHOLDER'.
  - ownership-check.ps1 -Base origin/overhaul -Owned 'Assets/Portfolio/**' prints PASS, and no upstream file is changed.
- **Evidence:** Pasted full-suite passed and failed summary lines with the results-*.xml path; ownership-check PASS output; git diff --stat at the PR head SHA; Before and after foyer and gallery screenshots from an Editor PlayMode run (TourShots arrives later) showing no visual change

### M-QA

- **Goal:** Give every crew and gate one evidence standard: TourShots images, per-zone budget JSON, route timings and guards that catch invented facts, stray textures and too many lights.
- **After:** M-F
- **Owned globs:** Assets/Portfolio/Tests/PlayMode/Tour/**, scripts/fork/tour.ps1, docs/portfolio/qa/**
- **In scope:** TourShots PlayMode test (fork). For each zone in a filterable list (default: every pf.* key), teleport to named viewpoints (spawn, first view, each exhibit and walk spot, exported as room constants) and capture 1920x1080 PNGs at Low, Ultra and 'lights off' (LocalLights disabled, pools zeroed).; Per-shot JSON: sha, zone, tier, viewpoint, audited batches, uGUI canvas count, tris, enabled local lights, active pools, warm-pixel fraction (hue 15-50 degrees above fill value) and indicative frame ms.; Route test: walk at 4.5 m/s, focusing each exhibit once. Fast path: see bay-1 works and the Gallery statue in 60 s or less. Everything path: every exhibit, plus the contact bureau, in 180 s or less.; Guard tests looping over every zone:
- at most 8 enabled Lights on Ultra;
- no Light component under any HeroKit root;
- no new Texture2D created while pf.* zones build, except photo previews (an allowlist file lists today's ManorKit generators until M-C1 removes them);
- every visible text in pf.* zones is sourced or [PLACEHOLDER.; scripts/fork/tour.ps1 wraps ion.ps1 test-play -Filter TourShots in the background and copies the output to C:\Users\Aaron\Documents\GitHub\portfolio-evidence\wave3\<crew>\<sha>\.; docs/portfolio/qa/tourshots.md documents the JSON schema and how a gate re-shoots at a SHA.
- **Out of scope:** Changing room content or lights; Editing upstream tests (light and texture guards live in fork test files); WebGL builds
- **Acceptance:**
  - tour.ps1 runs only through ion.ps1 with run_in_background, and produces PNGs plus JSON for all five pf.* zones at Low, Ultra and lights-off.
  - The JSON validates against the documented schema, and its batch and tri numbers match ArtPlayTests for the same zone to within 1%.
  - Route and guard tests are green on the current content; the texture allowlist holds exactly today's known offenders.
  - The full suite is green and the test count rises.
  - ownership-check passes.
- **Evidence:** One complete TourShots set (PNG plus JSON) at the head SHA; Route timing output; Full-suite summary lines; ownership-check PASS

### Seams

- **Goal:** Build the five upstream-owned seams on up/* branches cut from upstream/main, one at a time and behaviour-neutral by default, so no fork crew ever edits an upstream file.
- **After:** nothing
- **Owned globs:** Assets/Scripts/Gameplay/InstantCamera.cs, Assets/Scripts/Gameplay/ViewfinderFrame.cs, Assets/Scripts/Gameplay/FirstPersonController.cs, Assets/Scripts/Gameplay/PhotoHolder.cs, Assets/Scripts/Gameplay/PlayerInteractor.cs, Assets/Scripts/Presentation/PhotoOverlayUI.cs, Assets/Scripts/Presentation/ClickToPlayOverlay.cs, Assets/Scripts/Presentation/Motion/Feel.cs, Assets/Scripts/Presentation/Surface.cs, Assets/Scripts/Presentation/Palette.cs, Assets/Scripts/Presentation/Quality/UltraFx.cs, Assets/Scripts/Presentation/Audio/IonAudio.Footsteps.cs, Assets/Shaders/FlatToon.shader, Assets/Shaders/IonAtmosphere.hlsl, Assets/Shaders/IonPattern.hlsl, Assets/Scripts/Projection/ProjectionSystem.cs, Assets/Scripts/Levels/Room.cs, Assets/Scripts/Levels/ZoneCatalog.cs, Assets/Scripts/Levels/GameBootstrap.cs, Assets/Scripts/Levels/Rooms/HubLightTable.cs, Assets/Scripts/Levels/Rooms/GalleryEnding.cs, Assets/Plugins/WebGL/IonPointerLock.jslib, Assets/WebGLTemplates/Ion/index.html, Assets/UniversalRenderPipelineGlobalSettings.asset, Assets/Editor/ProjectSetup.cs, Assets/Tests/EditMode/**, Assets/Tests/PlayMode/IonPlayTestBase.cs, Assets/Tests/PlayMode/StreamingPlayTests.cs
- **In scope:** Sequence: up/lens -> up/palette -> up/zone-streaming -> up/manor-ux -> up/web-diet (see seams[] for each seam's files and API).; Each seam ships its own tests, logs its timings where specified, and carries a PR description that names the fork crew it unblocks.; Ask for orchestrator sign-off on the FlatToon finish modes and pools before starting up/palette, and confirm the up/zone-streaming base (stacked on up/zone-catalog or not).
- **Out of scope:** Any Assets/Portfolio/** file; Sending anything to Lets-be-strategic-here/project.ion (no pushes, PRs, issues or comments); Merging (the orchestrator merges); WebGL builds
- **Acceptance:**
  - Every seam: existing tests are untouched and green; the test count rises; ArtPlayTests constants are unchanged; ownership-check.ps1 -Seam -Base upstream/main prints PASS. Seams touching shaders compile FlatToon and IonAtmosphere for WebGL2 (GLES3).
  - up/lens:
- with no lens set, ViewFov equals BaseFieldOfView (70);
- the pins in CriticFixTests.cs:46-51 and RoomSolutionTests.cs:126 still pass untouched;
- in camera mode the wheel and Q/E never change the photo selection or a held photo's rotation, and outside camera mode Q/E rotation is unchanged;
- with mix 0 the film branch renders identically to before (pixel difference of 1/255 or less);
- RenderInto with a null projection matches today's preview.
  - up/palette:
- ArchKitTests round-trips all 28 Mats;
- PatternUV decodes Marble;
- with a pool count of 0 the render is identical to before (1/255 or less);
- the pool count uniform is clamped to 8 or fewer;
- footstep mapping is covered by AudioTests;
- the SRP Batcher stays compatible (a test or shader-compile log).
  - up/zone-streaming:
- with Streaming false, boot builds the same zones in the same order and checkpoint ids are unchanged;
- StreamingPlayTests cover a lazy build on GoToZone, the pin rule (current, start or checkpoint zone, or any WorldHistory change), and a mesh count back at baseline after 5 load and unload cycles;
- a photo taken in a zone survives that zone unloading;
- per-zone build and unload ms are logged.
  - up/manor-ux:
- a programmatic pointer-lock release does not start the cooldown;
- the default controls-sheet provider reproduces today's sheet;
- LMB to UseFocused is off by default;
- the Manor return appears only when pf.foyer is registered.
  - up/web-diet: the build report (at integration) no longer lists the FilmGrain and SMAA textures, and Bloom renders unchanged.
- **Evidence:** Per seam: diff --stat, full-suite summary lines, ownership-check -Seam PASS, and new test names; Shader compile logs for up/lens and up/palette; Before and after TourShots pixel-diff numbers for the neutral defaults (up/lens and up/palette); StreamingPlayTests timing log lines

### M-C1

- **Goal:** Statue-scale exhibits that read as monuments from the doorway on every tier, built in code within budget, with no invented facts.
- **After:** M-F
- **Owned globs:** Assets/Portfolio/Runtime/Exhibits/**, Assets/Portfolio/Tests/PlayMode/Exhibits*.cs
- **In scope:** Exhibits/HeroKit: GlassAward (DECA: a 1.1 x 2.7 x 0.16 m Frost blade with clipped top corners, brass bracket clips and edge rails, on a 1.25 m two-tier plinth), MedalGantry (1-5 medals of 1.0 m, ribbons, crossbar), CameraMonument (16x X-T5 proportions, no brand text, Concrete standing in for silver), RobotStatue (exploded assembly, at most 3.5 m total), Plinth and Audit. Add HeroMeshes.Frustum and ClippedHull.; Ultra twin detail (dentils, knurls, etch lines) inside Arch.Ultra(); B.Exempt on every hero group; one brass plaque per hero in the bracket idiom.; Exhibits/ExhibitKit: gallery frames. Slots 1-2 hang at 1.25x (3.0 x 2.25 m). Frames with no cover show a Frost slide plus a plaque label, replacing the runtime-generated textures. The arcade cabinet is reworked so it is pulled off the wall and its screen faces the room.; Attach ExhibitUsable (the M-F contract) to every hero, frame and cabinet, passing an ExhibitRef from content.; Phase B, after up/palette: Gilt on frames, clips and rails; Marble on plinths.
- **Out of scope:** Placing exhibits in rooms (M-C2 calls HeroKit); Lights, moods or pool anchors (M-C4); Panel UI (M-C5); Blender-authored meshes; Any Light component on a hero
- **Acceptance:**
  - HeroKitTests: every hero builds with ctx.Game == null, Arch.Validate passes, and no Light component exists under any hero.
  - Audited batches per hero: DECA 4 or fewer, gantry 4, camera 2, robot 2. Tris including Ultra twins: DECA 850 or fewer, gantry (5 medals) 2.5k, camera 2.1k, robot 1.3k.
  - Heights within 5% of their constants: DECA 4.25 m, gantry 3.4 m, camera 2.7 m, robot 3.5 m or less.
  - When placeholder is true, plaque text starts with '[PLACEHOLDER'. The glass shows only the geometric emblem, never text.
  - The M-QA texture allowlist is empty after this crew, so no runtime Texture2D remains except photo previews.
  - TourShots lights-off shots at 12 m show each hero's silhouette and its amber bevel band on Low.
  - Hall slice (gate G3a): the DECA hero stands in pf.honours with its plaque, and E opens the label (with M-C2 and M-C5).
- **Evidence:** TourShots PNGs of each hero at 12 m, 3 m and 1 m (Low, Ultra, lights off) at the head SHA; Per-hero audit JSON; Full-suite summary lines and ownership-check PASS

### M-C2

- **Goal:** Five rooms on one spine, each with its own silhouette, every exhibit placed for the 60 s and 3 min paths, and the Grand Gallery always one step away.
- **After:** M-F
- **Owned globs:** Assets/Portfolio/Runtime/Rooms/**, Assets/Portfolio/Tests/PlayMode/Rooms*.cs, Assets/Portfolio/Tests/PlayMode/ManorTests.cs
- **In scope:** Foyer: a wide central [ GRAND GALLERY ] door; a smaller [ PLAY THE GAME ] door (GamePad and its test unchanged); a contact bureau with a desk candle (ExhibitUsable opens the contact label); a framed house plan (opens the M plan); a monogram stele slot. Remove the welcome toast.; Grand Gallery: cross-arches at z=8 and z=24; frames re-hung per M-D005; the robot statue via HeroKit on the centre line where the carpet ends; an arcade bay behind it with cabinets built from the demo list (GrandGallery.Arcade and ArcadeBase kept for ManorTests); [ FOYER ] on the south wall beside spawn; [ HALL OF HONOURS ] and [ CAMERA ROOM ] at the far end; the [ PLAY THE GAME ] door removed; the Cyanotype wall with no pattern; blurb hints and the arcade toast removed.; Hall of Honours: 16 x 40 x 9 m with a faceted apse; the DECA hero on the axis with a 1 m plain backdrop margin; a brass first-view inlay about 10 m in front; up to 6 award plinths in pairs and one medal gantry in a side bay, all from honours.json; a Limestone/Terrazzo floor.; Camera Room shell: 16 x 28 x 6 m, graphite walls; the CameraMonument on the axis; calls CameraRoomKit.Furnish; a return pad at arrival and a far pad to the Wing.; Painting Wing shell: 12 x 34 x 7 m, pale plaster; frames from PaintingWorldRegistry on the staggered 4.3 m grid with brass inlays. With no registered world, only a [PLACEHOLDER: painting world] plaque appears, never an empty frame.; Per-room ArchStyle, Walk and Teleport solutions with Destination keys, and exported walk-spot constants for TourShots and the M plan.; Call RoomLighting.Light(root, ctx, key) in every room and export exhibit positions as public constants for M-C4.
- **Out of scope:** Moods, fixture positions and pools (M-C4); Exhibit meshes (M-C1); Camera devices (M-C3); Portal transitions and worlds (G-C3); Overlay UI (M-C5)
- **Acceptance:**
  - Every pf.* room passes ArtPlayTests and ProjectionPlayTests.
  - Every existing ManorTests assertion still passes; constants may move, assertions may not.
  - A door-graph test checks that every non-Gallery room's [ GRAND GALLERY ] pad is within 3 m of spawn and that no room is more than 3 doors from the Gallery.
  - Closed rooms stay within ±9 m of their root in X, and no walkable wall uses Pat.Lattice.
  - TourShots JSON is within the caps: Foyer 85/24k, Gallery 80/47k, Hall 95/50k with heroes, Camera Room 85/40k, Wing 75/35k.
  - M-QA's route test passes: fast path 60 s or less, everything path 180 s or less.
  - The Gallery first-view PNG shows the bay-1 works and the statue's silhouette, and the Hall view from the inlay shows the full hero.
- **Evidence:** TourShots PNGs (Low, Ultra, lights off) for every walk spot, plus JSON, at the head SHA; Route timings and door-graph test output; Full-suite summary lines and ownership-check PASS

### M-C3

- **Goal:** The owner's camera: an X-T5-style camera in hand with collectable lenses, film looks and an instant print that develops, all lightweight and all through the up/lens seam.
- **After:** M-F, Seams:up/lens
- **Owned globs:** Assets/Portfolio/Runtime/Camera/**, Assets/Portfolio/Shaders/**, Assets/Portfolio/Resources/Portfolio/Camera/**, Assets/Portfolio/Tests/EditMode/Camera*.cs, Assets/Portfolio/Tests/PlayMode/Camera*.cs
- **In scope:** Camera/LensKit: 5 primes, ViewFovFor, LookScale. LensCollection: a PlayerPrefs bitmask pf.lenses that is idempotent and reset in SubsystemRegistration; Restart remounts the 28 mm.; LensPickup and FilmPickup as UsableBehaviours with serialized-only state, so photo clones work.; FilmLook: Standard, Candle, Vivid, Mono and Cyanotype as Shader globals. The mix springs in on raise and is baked at capture through BeforeCapture.; CameraViewmodel: Geo primitives on layer 8, no shadows, no collider, hidden while the viewfinder is up. The lens swap takes about 0.55 s on a click spring of 3.2 Hz / 0.60 and is instant under reduced motion.; PrintEject plus Ion/PrintDevelop (Assets/Portfolio/Shaders), included through a Resources material. The print develops over 2.4 s and appears already developed under reduced motion.; CameraRoomKit.Furnish:
- the camera stand (CameraPickup with 3 film) and a refill crate;
- 4 lens plinths, each with a sample print made with RegisterDioramaShot at that lens's FovY;
- 3 film-cartridge plinths;
- a Frost light table.; CaptureDof last: Medium 4, High 8, Ultra 12 samples, Low 0; off-axis shear; one sample per frame within the 6 ms stage budget.
- **Out of scope:** Upstream files (up/lens); Camera Room shell (M-C2); Moods (M-C4); The exposure triangle, continuous zoom, manual focus, live DoF and the rear LCD; Lens puzzles in worlds (G)
- **Acceptance:**
  - LensKitTests: each FovY matches its focal length within 0.3 degrees; ViewFovFor(50) = 70 ± 0.01; the mapping is monotone and clamps at 20 and 88; LookScale(70) = 1; the default FovY equals InstantCamera.CaptureFovY.
  - FilmLookTests: Standard is the identity, the Candle row sums are as documented, and every output is finite. A Standard film render matches a film-off render within 1/255.
  - CameraLensTests: after mounting the 85 mm, SnapShot gives FovY 17.4; in camera mode the wheel changes the lens and not the photo; Restart remounts the 28 mm; LensPickup unlocks once and photo clones cannot grant twice.
  - The viewmodel never appears in a capture (layer test), and the F key binding is verified free in IonInput.
  - The Camera Room stays at 85 batches / 40k tris or less. CriticFixTests and RoomSolutionTests are untouched and green.
  - DoF: the focus-plane shear test passes, or DoF ships disabled (it is the first cut).
- **Evidence:** TourShots of the Camera Room at the head SHA; 12-frame filmstrips of raise, lens swap, shutter and print develop, both normal and reduced motion; One sample print per lens; Full-suite summary lines and ownership-check PASS

### M-C4

- **Goal:** Dim candles that read as royal on every tier: warm pools in cool shade, gilt that catches the flame, and no muddy orange.
- **After:** M-F, M-QA
- **Owned globs:** Assets/Portfolio/Runtime/Look/**, Assets/Portfolio/Tests/PlayMode/Look*.cs, docs/portfolio/direction/manor-look.md
- **In scope:** Phase A, fork-only and available now:
- Look/ManorLook per-room moods, with fill assigned after Make and a cool fog horizon (#1C1A2E);
- per-room ArchStyle surfaces;
- Pat.Lattice removed;
- Look/RoomLighting fixtures per the hierarchy (one chandelier per bay, one candelabra per exhibit pool, sconces at doors only);
- LocalLights removed from sconces, chandelier and candelabra intensity halved.; Starting moods (tune with TourShots): Foyer Make('#1B2140', '#1C1A2E', '#FFB46A', '#33284A', 0.72, 0.020, 12); Gallery ('#141222', '#1C1A2E', '#FFC47A', '#2A2440', 0.62, 0.020, 13); Hall ('#0E1020', '#171B33', '#FFE3B8', '#25376A', 0.80, 0.017, 15); Camera Room ('#10121F', '#3A1E14', '#DCEBFF', '#241E36', 0.35, 0.030, 8) with a warm fill #3A2218; Wing ('#0C1230', '#1B2552', '#C4CB8E', '#25376A', 0.42, 0.020, 11).; Phase B, after up/palette:
- Look/ManorGlow pool feeder: nearest N by tier with smoothed fades; ±8% smooth flicker at 6-8 Hz; under reduced motion no flicker, only breathing of ±3% under 0.5 Hz; skips any source whose real light is on;
- the Foyer first-entry candle wave (1.5 s or less; instant under reduced motion);
- a Flame cone on every candle; Gilt on candelabras and rails; Velvet carpets.
- **Out of scope:** Shader code (up/palette); Exhibit and frame materials (M-C1); Room geometry (M-C2); Any light on a hero
- **Acceptance:**
  - Every room reads in TourShots lights-off and pools-off shots: exhibits, doors and the carpet are legible (the gate agent checks against the art bible).
  - At most 8 enabled lights on Ultra in every zone; no LocalLights on sconces or heroes; pool count never exceeds the tier cap.
  - ManorGlow unit tests: flicker amplitude 8% or less, 0 under reduced motion, fades with no single-frame jump above 10%.
  - Warm-pixel fraction 0.40 or less at every walk spot (M-QA metric).
  - No zone exceeds its cap after Flame and Gilt.
  - Indicative pools-on versus pools-off frame-ms delta on Low is 1.0 ms or less at 1080p; otherwise Low drops to emissive only.
  - The mood table is published in manor-look.md.
- **Evidence:** Per room, TourShots at Low, Ultra, lights off and pools off at the head SHA; Warm-fraction and frame-ms JSON; Flicker log; Full-suite summary lines and ownership-check PASS

### M-C5

- **Goal:** A non-gamer can enter, skip to the Grand Gallery, read any exhibit like a museum wall label and get back, by mouse or keyboard alone, in the shared identity's tokens.
- **After:** M-F
- **Owned globs:** Assets/Portfolio/Runtime/Overlay/**, Assets/Portfolio/Plugins/WebGL/**, site/manor-overlay/**, Assets/Portfolio/Tests/PlayMode/Overlay*.cs
- **In scope:** Overlay/ExhibitUsable implementation: range 4.5 m, prompt 'read', CanUse false for empty slots, Use serialises the content entry to IonPanelOpen.; Assets/Portfolio/Plugins/WebGL jslib: IonPanelOpen and IonPanelClose, IonMapOpen and IonMapTakeRequest, IonArcadeOpen and IonArcadeClose, IonOverlayIsOpen and IonOverlaySerial, IonAnnounce, IonPrefersReducedMotion. Every entry is wrapped in try/catch.; site/manor-overlay: the window.ionOverlay module (exclusive slot, focus trap and return, Esc, history.pushState so Back closes), the wall label, the 'Plan of the house' on M (SVG plan plus a real list, Grand Gallery first), the arcade bezel, and the token stylesheet under :where(:root).; Loader restyle by compose-time script: three control rows, 'Enter the Manor', 'Go straight to the Grand Gallery', and 'Read everything as a page' as the first focusable item. The 12-row controls wall and the 'Built with Unity' footer are deleted.; Map travel: C# polls IonMapTakeRequest, then calls GoToRoom or Player.Teleport in front of the chosen exhibit.; Seed Feel.ReducedMotion from the OS only when no preference is stored; add a polite live region mirroring prompts and room titles.; Arcade fixes: aria-labelledby, a per-project iframe title, inert canvas, a visible focus ring, re-lock on button close, the 'ion-arcade:close' postMessage contract, and no allow-same-origin for third-party demos.; After up/manor-ux: the cooldown flag, the skip hook, and LMB only if the owner says yes.
- **Out of scope:** Unity-drawn pause card internals (up/manor-ux provider); Room geometry and toasts in room files (M-C2); publish.ps1 (request the injection hook from the Release crew); Content facts
- **Acceptance:**
  - PlayMode: every pf.* exhibit has an ExhibitUsable, and Use passes valid JSON to a bridge stub in the Editor. A map travel request lands the player within 0.5 m of the spot and in the right zone.
  - Harness page site/manor-overlay/test.html, with a fake Unity bridge, checked through the crew's own playwright-cli session:
- axe reports 0 serious issues;
- keyboard-only open, read, Tab cycle and close via Esc, Back and the Back button all work, with focus returned to the canvas;
- under reduced motion, transitions are opacity-only;
- token defaults meet WCAG AA contrast on paper and on wood;
- hit targets are 24 px or larger.
  - The loader script, run against a copy of the template HTML, produces three rows and both buttons, with the skip button first in the visual order after the primary button.
  - No overlay leaves pointer lock stuck: a test matrix covers open and close by button, Esc and Back.
- **Evidence:** playwright-cli screenshots at 1440 and 1280 of the panel, plan, loader and arcade, normal and reduced motion; axe JSON; Keyboard-flow filmstrip; PlayMode summary lines and ownership-check PASS

### M-C6

- **Goal:** The Manor pays at boot only for what a visitor sees first and the download shrinks, without ever breaking rewind, checkpoints or carried photos.
- **After:** M-F, M-C2, Seams:up/zone-streaming
- **Owned globs:** Assets/Portfolio/Runtime/Streaming/**, Assets/Portfolio/Tests/PlayMode/Streaming*.cs, docs/portfolio/direction/manor-perf.md
- **In scope:** Streaming/ registrar: turn ZoneCatalog.Streaming on only under UNITY_WEBGL && !UNITY_EDITOR, and classify zones with ZoneCatalog.SetLoad: Foyer, Gallery and pf.lens Eager; pf.honours and pf.wing Lazy; the pf.world.* prefix Unloadable.; Fork StreamingTests with streaming on through the seam's AfterBoot hook:
- doors into the Hall build it under the fade;
- worlds unload when unpinned;
- a photo taken in a world can be placed in the Gallery after the world unloads.; Boot and build timing report from the seam's log lines (Editor indicative; the WebGL number is taken at integration).; Music on-demand fetch spike as a written report. The crew does not build; the Release crew verifies it in a browser.; docs/portfolio/direction/manor-perf.md: the load classes, pin rule, timings, the prefetch contract for the W Manor Bridge, and the diet status.
- **Out of scope:** Upstream files (up/zone-streaming, up/web-diet); Streaming the core six; publish.ps1; Room content
- **Acceptance:**
  - The Editor and the default test runs keep streaming off and the full suite stays green.
  - With streaming on, boot builds only Eager zones; the first entry into the Hall builds it, and its build time is logged (300 ms target; above 500 ms, file a type:bug against the room's crew).
  - Mesh count returns to baseline after 5 world load and unload cycles.
  - A zone holding the checkpoint or any WorldHistory change is never unloaded.
  - A carried photo survives its world unloading.
- **Evidence:** StreamingTests summary; Build and unload ms log lines; Mesh-count JSON; manor-perf.md at the head SHA; ownership-check PASS

## Owner questions

- Is Brass #C59A45 the single shared accent (small active colour on the Front Door), and is the [ ] bracket pair your monogram?
- May we cut the Workshop and the Study? The robot/CAD piece becomes a statue in the Grand Gallery and your contact sits on a bureau in the Foyer.
- Which two projects are your strongest (they hang first, larger), and which project should the robot/CAD statue stand for? Placeholders until you say.
- How many awards and medals do you have, and what does your DECA glass look like (shape, event, placement, year)? Until then we ship 1 hero, 2 award plinths and 3 medals, all marked placeholder.
- Is this 'overly large' enough: Gallery statue about 3.5 m, DECA glass about 4.25 m in a 9 m Hall of Honours?
- Keep the game's door only in the Foyer, or also add one in the Grand Gallery?
- Carpet in classic crimson (#8B1A2B) or today's brick red? The Gallery floor stays warm oak parquet; is that OK?
- Film looks: our own names (Standard, Candle, Vivid, Mono, Cyanotype) or Fujifilm's (trademarks)? And should the X-T5-style camera be silver and black, or all black?
- May Manor visitors paste photos inside the Manor rooms (cutting the Gallery's walls), or only in the Camera Room and the painting worlds? Should collected lenses persist between visits?
- Should clicking (LMB) a painting also open its label, besides E? It needs a small change to an upstream file, kept in your fork only.
- May the Manor's first focusable link be 'Read everything as a page', pointing to the Front Door?

## Cut list

- Workshop room
- Study room (contact moves to a Foyer bureau)
- Arcade Parlour as a zone (it becomes the Gallery's arcade bay)
- Pat.Lattice walls in walkable rooms
- Uniform sconce and candelabra rows every 8 m; sconce real lights; Gallery chandeliers beyond one per bay
- Gallery [ PLAY THE GAME ] door
- Foyer welcome toast, painting blurb toasts, arcade toast, the 12-row controls wall and the 'Built with Unity' footer on the Manor loader
- Runtime-generated placeholder painting textures (ManorKit.cs:217,246,303)
- Statue quarter-turn, turntables, coin-drop animation and any hero animation
- Gallery 'develop on a brass marker' pulse
- Per-mood colour-grade seam
- Medal vitrines, the Glass Mat and Pat.Glint (deferred), Silver and Crystal Mats (deferred)
- Translucent glass panes, billboard flame cards, decal or vertex-colour light pools
- Blender-authored meshes in Wave 3 (baker specified only)
- Exposure triangle, continuous zoom, manual focus, live DoF, rear LCD, live grain and a 14 mm lens
- In-world 3D map
- Streaming the core six, prebuild on approach and brotli
- Empty frames, plinths, podiums or cabinets: everything is built from lists
- Delete first under pressure, in order:
1. capture DoF;
2. the Bedroom world (G);
3. fold the Camera Room into a Wing vestibule;
4. halve the Hall's award plinths;
5. drop shader pools on Low;
6. drop the Velvet sheen.

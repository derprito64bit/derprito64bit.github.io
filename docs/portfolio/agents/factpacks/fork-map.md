> **Fact pack `fork-map`**, Wave 0.5 (2026-10-02). It was researched by a Sonnet specialist and spot-checked by an independent skeptic,
> whose verdict was `fix`. **The corrections at the end override the text above them.** Claims are labelled
> measured, documented or estimate.

# Fork map: project.ion overhaul worktree (HEAD 9269053, measured)

Paths are relative to the worktree root. GB = Assets/Scripts/Levels/GameBootstrap.cs.

## 0. Orientation
- Fork code in `Assets/Portfolio/**` compiles INTO `Ion.Runtime` via an asmref (Assets/Portfolio/Ion.Portfolio.asmref:2 = GUID of Assets/Scripts/Ion.Runtime.asmdef.meta). It can therefore call `internal` upstream types: `PropBuild` (Levels/Props/PropBuild.cs:25), `PrintBuilder` (Props/PrintCard.cs:159). ManorKit does (ManorKit.cs:6,206). Fork tests compile into `Ion.Tests.PlayMode` the same way (Portfolio/Tests/PlayMode/Ion.Portfolio.Tests.PlayMode.asmref:2).
- Name trap: inside `namespace Ion.Levels`, `Arch` is a namespace. Write `using Arch = Ion.Levels.Arch.Arch;` inside the namespace block (GB:13, ManorRooms.cs:11, art-bible.md:259-261).
- Every static resets in `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]` (ZoneCatalog.cs:38, GB:91, Teleporter.cs:25). Copy that for new statics.

## 1. Boot and zone lifecycle
1. Scene `Main` = one `GameBootstrap` (BUILD.md:28). `Awake` (GB:99-156): Atmosphere.Apply (:110), ProjectionSystem (:113), six core rooms T1,T2,hub,stairs,camera,gallery = indices 0-5 (:117-122), `ZoneCatalog.CreateExtensions` appended (:124), `BuildRooms` (:125), `ResolveStartZone` (:126), Backdrop (:134), UIFactory+UltraFx (:138-141), player at StartZone spawn (:144-148), `StartCoroutine(CaptureDioramas)` (:154).
2. Fork registration runs earlier: `PortfolioRegistrar.Register` at BeforeSceneLoad (PortfolioRegistrar.cs:16-26) calls `ZoneCatalog.Register(key, order>=1000, factory)` (ZoneCatalog.cs:22,65-80). Foyer order 1000, gallery 1010 = indices 6, 7. Web only: `StartKey` comes from `?zone=` (PortfolioRegistrar.cs:21-25,32-45).
3. `BuildRooms` (GB:173-208): zone root at x = i*50 (RoomSpacing :31,182); diorama origin (i*200, -1000, 0) (:32-33,184); `ArchKit.Style = room.Style` (:188); `room.Build` (:189); `Arch.Bake(root, BakeChunk, BakeSoftChunk)` on zone and diorama (:198-199). Exceptions are caught per zone (:191,201).
4. `CaptureDioramas` (GB:210-269): per zone `Atmosphere.ApplyMood(room.Mood)` BEFORE its shots (:223), `ps.Capture(shot.WorldPose(EyeHeight)...)` (:232), wait deferred previews (:244-248), `ps.WarmUp` (:259), hide dioramas, `PhotosReady`, `ApplyZoneCulling` (:266-268).
5. Runtime: `GoToRoom`/`GoToZone` (GB:322-339) -> `GoToZoneWorld` (:341-354): set current zone, teleport, checkpoint `"zone."+Key` (:350), `Room.OnEnter` (:351), toast. `SetCurrentZone` blends mood 2 s (:357-368). `ApplyZoneCulling` (:376-391): current zone draws all, neighbours only direct-child collider batches, others nothing; Ultra twins off unless Ultra (:387).
6. `Restart` (GB:403-456): `UnwindAll`, clear switches/inventory/camera/pickups, `ZoneHint.ResetZone`, `Room.OnRestart` (:434-438), `GoToRoom(StartZone)` (:442). "Play again" returns to the fork's start zone.

## 2. Add a zone
Anatomy (Levels/Room.cs:20-97): subclass `Room`; override `Key` (unique; :29), `Title`, `Intro`, `Mood` (:32), `Style` (:35), `LowestFloorY` (void = value-6, :38,47), `BakeChunk`/`BakeSoftChunk` (:41,44; defaults 8 m / 0, ArchBake.cs:32,38), `Build(root, ctx)` (abstract), optional `OnEnter`/`OnRestart` (:52,55).
Build conventions: floor top y=0, player walks +Z (Room.cs:14-16); `ctx.SetSpawn` (:323); `AddSolution(name, Kind, feet, yaw, pitch, photoIndex, roll)` (:65) with kinds Place/Snap/Goal/Walk/Press/Pickup/Rewind/RewindToCheckpoint/Teleport (:106-126) plus Hazard/Via/Channel/Destination (:156-166); `ctx.Hint` (:408); `ctx.Marker` (brass `[ ]`, one per Place/Snap, :372); `ctx.RegisterDioramaShot` (:349) with the same builder run on `ctx.DioramaRoot` (StairsWing.cs:46-50; art-bible.md:806-815).
Fork examples: `ManorFoyer`/`GrandGallery` (Portfolio/Runtime/ManorRooms.cs:70-233): shell `ManorHall.Build` (:22-43); door = `Arch.BracketFrame` + `ctx.Teleporter` + `PropKit.Plaque` (:46-51); `GoTo(ctx,key)` null-checks `ctx.Game` (:54-63); solutions are only Walk and Teleport (:131-132,229-231).
Registration: add `ZoneCatalog.Register(key, ZoneCatalog.ExtensionOrderMin + n, () => new MyRoom())` at PortfolioRegistrar.cs:19-20. Invalid, duplicate or key-mismatched entries are logged and skipped, never thrown (ZoneCatalog.cs:16,100-130).
Placement is automatic: index = 6 + rank by (order, key) (ZoneCatalog.cs:132-139); x = index*50. `ZoneAt` = nearest root on X (GB:307-312), so stay within about +-25 m in X; photos aim along +-Z so Z length is free (gallery z -4..44, ManorRooms.cs:149). Grid: architecture on 0.25, zone origins whole metres (art-bible.md:28-31).
Tests that cover every zone automatically (loops over `Game.Rooms`):
- ArtPlayTests: pattern space on every Sliceable (ArtPlayTests.cs:24-35); <=120 batches and <=60,000 tris per zone (:20-21,38-56), counted over every enabled MeshRenderer including devices and Ultra twins (ArchBake.cs:125-139); `Arch.Validate` on a fresh `Activator.CreateInstance(room.GetType())` built with `RoomContext(null,...)` (:94-104). So: public parameterless ctor, and `Build` must work with `ctx.Game == null`.
- ProjectionPlayTests: `Solutions` non-empty (ProjectionPlayTests.cs:34); each non-Teleport/Rewind/Hazard/AfterPlace step is solid ground within 0.15 m (:21-24,36-45); markers == Place/Snap steps (:53-77); Place/Snap frusta at 4 rolls never touch other zones (:87-120); each diorama preview has >=0.5% edge pixels (:209-232, skipped without GPU).
- ZoneCatalogTests asserts `Rooms.Count - ZoneCatalog.Count == 6` (:52).
- NOT automatic: usables, teleport destinations, local-light count, texture use.

## 3. Add a prop or exhibit
Primitives (`PropBuild`): `Box` (PropBuild.cs:145,151), `Chamfer` (:157,166; 0.0625 default), `Prism(g, base, r, h, sides, surf, flags, yaw)` (:173), `Wedge` (:178), `Beam` (:188), `Rivet` (:200), `BracketClip` (:211), `BracketSurround` (:228), `Frame` (:60,69), `Label` (:369), `Exempt` (:137), `Collider` (:301), `DeviceRoot`/`BakeDevice` (:98,254). PropKit entries: Devices.cs:24-660, Furniture.cs:45-413, Frames.cs:21-149, Plants.cs:25, LightTable.cs:21. Convention: `(Transform p, Vector3 base, Dir facing, ...)`, +Z is the front (PropBuild.cs:14-18).
Triangles per primitive (derived, estimate): box 12 (Geo.cs:126), wedge 8 (:149), n-prism 4n-4 (Geo.cs:320-339: 6-gon 20, 8-gon 28, 12-gon 44), cone 2n-2 (:303-317), ChamferBox about 44 (26 faces, ArchMeshes.cs:13-14). Worked: ManorKit candle 32, candelabra about 320, 12-candle chandelier about 1,210 (ManorKit.cs:22-77).
Flags (ArchTypes.cs:8; art-bible.md:292-301): `Solid` = Collider|Sliceable; `Soft` = Sliceable only (walk-through, merged); `Device` = Dynamic (PropBuild.cs:31): skipped by the zone bake, merged by `BakeLocal` under an `Interactable`, primitive colliders only (PropBuild.cs:20-23). Devices are captured or removed whole, never cut (art-bible.md:66-68).
Materials: `Surf = Mat + Pat` (Surface.cs:9-16,30-36); 24 `Mat` roles (hex Palette.cs:152-177; art-bible.md:140-171); `Palette.Get(Mat)` (Palette.cs:235); patterns None..Courses (art-bible.md:173-199). Ion colour is for interactables only (art-bible.md:61-63). Fork look recombines existing Mats (ManorLook.cs:39-52).
Merging: zone `Arch.Bake` groups non-Dynamic pieces by (material, collider, sliceable, shadows, layer, chunk): Solid per 8 m chunk, Soft one object per material per zone (ArchBake.cs:20-38,268-300). More Soft props in existing Mats cost about 0 extra batches; a new Mat or an Ultra twin adds one. Every device and every `PrintCard` adds its own draw group (ArchBake.cs:44-51; PrintCard.cs:30-32).
`B.Exempt(g)` sets `ArchPiece.Snap=0` on every piece under g so details off the 0.0625 grid pass `Arch.Validate` (PropBuild.cs:137-141; used ManorKit.cs:42,60,90,119,145,176,197). Skip it and `Art_ArchitectureOnGrid` fails.
Ultra twins: `using (Arch.Ultra()) {...}` makes pieces use the Ultra material and drops their collider (Arch.cs:52-62,778-779); drawn only on Ultra (GB:387). Today only planter plants use it (PropKit.Furniture.cs:239).
LocalLights: `LocalLights.Add(parent, pos, color, range, intensity)` (UltraFx.cs:226-259) makes a disabled point light; UltraFx enables the 8 nearest within 26 m, Ultra only (UltraFx.cs:29,32,213). ManorKit adds one per candelabra, chandelier and sconce (ManorKit.cs:43,61,91); teleporters carry one (PropKit.Devices.cs:66).
Plants need `PropKit.BakeRoot` or a zone root two levels under a container (PropBuild.cs:313-327; PropKit.Furniture.cs:31).
Exhibit: `PropKit.Exhibit(p, base, facing, new ExhibitSpec{Number,Title,Key,State})` (Devices.cs:264; PropTypes.cs:22-35). Core hub hard-codes 4 coming-soon slots (HubLightTable.cs:79-86).
Texture rule: none except photo previews (art-bible.md:988). Tension: ManorKit generates 96x28, 64x48 and 192x144 textures at run time (ManorKit.cs:217,246,303).

## 4. Usable, door, teleporter, photos
- Usable: subclass `UsableBehaviour` (Gameplay/Interaction/IUsable.cs:51-73): `UsePrompt`, `UseRange` (2.25, :54), `FocusOffset`, `CanUse`, `Use()`. `PlayerInteractor.FindFocusedUsable` picks the nearest in range and roughly in view (dot >= 0.55 unless < 1.1 m) (PlayerInteractor.cs:195-222); `UseFocused` (:112). Example `ArcadeMachine` (Arcade.cs:14-31), serialized fields only so photo copies work (:12), placed at the cabinet screen (ManorRooms.cs:217-224). Tests: UsableTests.cs:18-45, ManorTests.cs:56-71.
- Teleporter/door: `ctx.Teleporter(localPos, facing, onEnter, tint)`; null onEnter = next zone (Room.cs:393). `ctx.DioramaTeleporter` keeps the way forward after a placement (Room.cs:402; art-bible.md:812-813). Trigger 1.6x2.4x1.6, 1 s cooldown (Teleporter.cs:52-53); `OnEnter` sits in a registry keyed by a serialized id so photo clones work (:12-38). Zone moves: `GoToRoom(index)`, `IndexOfKey` (GB:292,322). Switch doors: `PropKit.Gate(..., channel)` (Devices.cs:566), `Button` (:349), `Lever` (:464).
- Capture: `ProjectionSystem.Capture(pose, fovY, aspect, label)` (ProjectionSystem.cs:175); frustum near 0.6 m, far 250 m (:23). Cuts every Sliceable whose bounds touch the planes unless it sits under an `Interactable` (:186-192,1203-1207); clones outermost `Interactable`s whose pivot is inside the frustum (:231-249). Excluded: layers 8 Player and 9 PhotoUI (:24-26,1169; preview mask :1522). Anything without `Sliceable` (Backdrop, sky) is never captured or cut.
- Placement budget: <=25 ms per placement on the reference laptop; staged <=6 ms per frame (`StageBudgetMs`, ProjectionSystem.cs:279; art-bible.md:985-986). Colliders re-cook per touched 8 m chunk (ArchBake.cs:28-32).
- Instant camera: `RoomContext.UnlockInstantCamera(film)` (Room.cs:446); 50 deg, 4:3, preview 1024 (InstantCamera.cs:18-25); `ctx.CapturePhoto` (Room.cs:332).

## 5. Look levers
- `ZoneMood.Make(name, skyTop, skyHorizon, sun, shadowTint, toSun, sunIntensity, fogDensity, fogStart)` (ZoneMood.cs:80-100), then tune `ShadeValue` and ambients like `ManorLook.Candlelight` (ManorLook.cs:23-28). Core moods ZoneMood.cs:50-72.
- `Atmosphere.ApplyMood` (immediate) / `BlendTo(mood, 2 s)` (Atmosphere.cs:201,208) push `_IonAmbient*`, `_IonFogParams`, `_IonSky*`, `_IonSunWarm/Halo`, `_IonGrade`, `_IonShadowTint/Hue/Mix` (Atmosphere.cs:304-325). `IonGrade` constants (contrast 1.4, lift -0.035, pivot 0.85) are global, not per mood (Atmosphere.cs:68; IonAtmosphere.hlsl:110-127).
- Shaders (Assets/Shaders): `Ion/FlatToon` (ForwardLit :130, ShadowCaster :384, DepthOnly :452, DepthNormals :498; `_IonUltraFx` :92), `Ion/Backdrop`, `Ion/GradientSky`, `Ion/PhotoDisplay`; IonAtmosphere.hlsl (:7-17) and IonPattern.hlsl (`_IonPatternFade/_IonPatternOn/_IonCutHatch` :17-19). Pattern fade 25-45 m, Low 15-30 m, cut hatch off on Low (Atmosphere.cs:229-235).
- Tiers (QualityTier.cs:16-22; AdaptiveQuality.cs:49-58): Low scale 0.75, MSAA 1, no shadows; Med 0.85, MSAA 2, 28 m shadows; High 1.0, MSAA 4, 4096 map, 30 m; Ultra 2 cascades, 48 m, soft-high, HDR. Ultra also enables local lights, bloom, detail twins, 1280-px previews (art-bible.md:651-669). Auto reaches Ultra after 20 s under 8 ms (art-bible.md:653-654). Clouds 14/20/28/34 (AmbienceClouds.cs:18).

## 6. Testing
- `IonPlayTestBase` (Tests/PlayMode/IonPlayTestBase.cs:25): `[UnitySetUp]` reloads `Main`, simulates pointer lock, waits for PhotosReady (:43-61). Helpers: `Game`, `Player`, `Room(key)` (:90), `Spot`, `W`, `Local`, `GoTo("key:spot")` (:111), `Seconds`/`Frames` (:130-135), `WalkTo`, `RunSteps`/`RunAll(ctx)` (:342-352). Manual placement: `Player.Teleport(feet, yaw)` (ManorTests.cs:36).
- Fork recipe: `[OneTimeSetUp] ZoneCatalog.StartKey = key; [OneTimeTearDown] = null` (ManorTests.cs:16-20; ZoneCatalogTests.cs:33-45). StartKey only resets at subsystem registration, so always reset. The fork keeps T1 as start in Editor and tests (PortfolioRegistrar.cs:21-25).
- Run: `powershell -File scripts/ion.ps1 test-play -Filter ManorTests` (`-Filter` = `-testFilter`, ion.ps1:98; commands :24; `test` = EditMode then PlayMode, :156-160). Results `Build/results-<platform>.xml`, logs `Build/logs/` (:34,94). PlayMode needs a GPU; only EditMode gets `-nographics` (:97). No `-quit` with `-runTests` (BUILD.md:47).

## 7. Ownership and seams
- Fork-only: `Assets/Portfolio/**`, `docs/portfolio/**`, `docs/FORK.md`, `scripts/fork/**`, fork workflows (FORK.md:21-22); site/ (FORK.md:37); MCP for Unity (commit d23bfc5). Personal commits touch only these.
- Upstream (art-bible.md:947-956): A = Shaders, Palette/Surface/Atmosphere/ZoneMood/Backdrop, Quality/** (not SettingsPanel), Arch/**, Geo/DecorCombiner, clipper files, EditMode tests, Editor; B = Props/**, LevelProps/Kit/Scatter; C = Rooms/**, Room.cs, GameBootstrap.cs, DebugTools/**, Tests/PlayMode/** (not RewindTests/SwitchTests); D = Gameplay/**, HUD/UI files, WebGLTemplates, Plugins/WebGL; E = Audio.
- Fork edits inside upstream files (keep fork side on merge): README.md, WebGLTemplates/Ion/index.html, EndCard.cs `ProjectsUrl` (:24) (FORK.md:41-45).
- `up/*` seams, merged into overhaul, none sent upstream (FORK.md:16-17): `up/windows-runner` b5423cf+dcde2e3 (scripts/ion.ps1, BUILD.md, churn restore); `up/zone-catalog` f7bbd58+1fccae6 (ZoneCatalog.cs, GameBootstrap +38/-10, ZoneCatalogTests); `up/automation-bridge` 9b3ecbd (IonDebug `Lock`, `Hud`, IonDebug.cs:47-48,559-579); `up/usables` c9c461a (IUsable.cs, PlayerInteractor +51, UsableTests).
- Branches: main mirrors upstream; overhaul stays green; `up/<seam>` cut from upstream/main (FORK.md:14-19).

## 8. Gotchas
- Line endings: `core.autocrlf=true`, no .gitattributes (measured). Unity setup/build/test rewrite Main.unity and settings; ion.ps1 restores only `.unity/.asset/.lighting/.prefab` churn (ion.ps1:111-135; `-KeepChurn` keeps).
- PowerShell 5.1: no `&&`; here-strings for commit text. In this worktree the Bash tool refused compound commands naming git; run plain, separate git calls (measured this session).
- One Unity process per project: ion.ps1 aborts when Temp/UnityLockfile is held (:57-62); first setup runs twice (BUILD.md:35). Playwright/Unity/Blender MCPs are single shared instances reserved for the orchestrator.
- Web player has exceptions disabled (BUILD.md:64; ZoneCatalog.cs:16): do not rely on try/catch.
- `JsonUtility` reads only `[Serializable]` classes with public fields (no dictionaries, properties, top-level arrays); hence the `CatalogFile` wrapper (PortfolioCatalog.cs:42-47,80). `MaxProjects = 8` = the 8 gallery frames (PortfolioCatalog.cs:56; ManorRooms.cs:150,175-190; ManorTests.cs:48).
- IonDebug resolves a zone by key, index, title or type-name fragment (IonDebug.cs:162-180): keep keys unambiguous. Web debug needs `?debug=1` (IonDebug.cs:66-83).
- Core exits hard-code zone 0 or the hub (HubLightTable.cs:92); the ending opens EndCard (GalleryEnding.cs:68). Nothing returns the player from the game to the Manor.
- `Mat` is append-only (Surface.cs:6-8).

## 9. Missing seams (candidates)
1. Custom materials: `Mat`/`s_MatColors` fixed at 24 (Surface.cs:9-16; Palette.cs:152-177).
2. Hub exhibit registry replacing the 4 hard-coded slots (HubLightTable.cs:79-86).
3. Return path from the game to the Manor: tutorial pod (HubLightTable.cs:92) and end card (GalleryEnding.cs:68).
4. Zone footprint/spacing metadata (GB:31,307-312).
5. Local-light budget test beside ArtPlayTests.cs:38-56 (UltraFx.cs:29).
6. `StartInZone(key)` helper in IonPlayTestBase.cs:43-50.
7. Site-config seam for `ProjectsUrl` and template links (FORK.md:43-45; EndCard.cs:24).
8. Cover-art path that respects the no-new-textures rule (PortfolioCatalog.cs:94-98).

## Verification corrections (these override the text above)

- **Claim:** Ultra twins: `using (Arch.Ultra())` ... "Today only planter plants use it (PropKit.Furniture.cs:239)."
  - **Correction:** False. `Arch.Ultra()` is also used by the architecture kit itself at Arch.cs:340, 357, 387, 427 and 486, and at Arch.House.cs:86. Art-bible §6.3.1 lists string-course drips, pilaster capitals, column astragals, parapet drips, skirtings, screen mullions and railing collars as Ultra detail. Core zones therefore already carry Ultra twins, which affects the zone batch budget (each Ultra twin material adds a batch). PropKit plants are only one prop-level user. (source: Assets/Scripts/Levels/Arch/Arch.cs:340,357,387,427,486; Assets/Scripts/Levels/Arch/Arch.House.cs:86; docs/art-bible.md:~660 (Detail row))
- **Claim:** FORK.md line refs: branch roles FORK.md:14-19, up/* not sent upstream FORK.md:16-17, fork-only paths FORK.md:21-22, site/ FORK.md:37, fork edits in upstream files FORK.md:41-45, site-config seam / ProjectsUrl FORK.md:43-45
  - **Correction:** The content exists but every line number is stale. Branch table is lines 15-21; the 'not sent upstream' rule is line 21; fork-only paths are lines 25-26; the site/ description is line 49; the fork-only link edits are lines 51-54, with EndCard `ProjectsUrl` at line 54. Lines 41-45 are the Standing rule and the upstream-sync text. Agents opening these lines will land on the wrong text. (source: docs/FORK.md:15-26,42-54)

## Unsupported claims (treat as estimates until re-sourced)

- Triangle counts for prism, cone and ChamferBox (and the worked candle 32 / candelabra ~320 / chandelier ~1,210 figures) are derived estimates, not measured. The pack admits this. Only cube 12 and wedge 8 were confirmed in Geo.cs:126,149.
- The claim that the doc is near the 2500-word cap was never machine-checked (the pack admits it), so the cap may be exceeded.
- Test-enforcement claims (ArtPlayTests, ProjectionPlayTests, etc.) come from reading source only. No test was run, so 'keep it green' status and real per-zone batch/triangle counts are unknown. I confirmed only the constants MaxBatchesPerZone=120 and MaxTrianglesPerZone=60000 (ArtPlayTests.cs:20-21).
- `core.autocrlf=true` is labelled 'measured'. I confirmed it with git config, and there is no .gitattributes, so it holds today, but it is machine-local config, not a repo guarantee.
- The pack's sources list names 'git log (b5423cf, ...)' without per-claim commit-to-file evidence. The SHAs and branch seams do match git log, but the per-seam file lists (e.g. 'GameBootstrap +38/-10', 'PlayerInteractor +51') were not checked.
- Tension note 'ManorKit generates 96x28, 64x48 and 192x144 textures' is correct (ManorKit.cs:217,246,303), but the stated 'no new textures' rule is only an art-bible budget line (art-bible.md:988) and is not enforced by any test the pack cites.
- Claims about the unmerged crew/w0-* branches are absent, so the pack under-maps fork state. Those branches exist locally and on origin (crew/w0-ownership, crew/w0-publish, crew/w0-slots).

Verifier notes: I checked about 35 claims against source at HEAD 9269053 and found two defects.

Confirmed correct:
- **Asmref and internals:** the asmref GUID 0b7c75fb... matches Ion.Runtime.asmdef.meta. PropBuild is at PropBuild.cs:25 and PrintBuilder at PrintCard.cs:159, both `internal`.
- **GameBootstrap:** RoomSpacing/DioramaY/DioramaSpacing are at lines 31-33. Awake steps sit at lines 110, 113, 117-122, 124, 125 and 126. The zone checkpoint is at 349-350 and the Ultra culling check at 387.
- **ZoneCatalog:** ExtensionOrderMin is 1000. Register rejects order below 1000 and null key or factory with a log and no throw. CreateExtensions skips factory-null, key-mismatch and duplicate-key entries. The sort is by (order, key). StartKey is reset in the SubsystemRegistration method. The test asserts `Rooms.Count - ZoneCatalog.Count == 6` at ZoneCatalogTests.cs:52.
- **PortfolioRegistrar:** the foyer registers at 1000 and the gallery at 1010. StartKey comes from the URL only under `UNITY_WEBGL && !UNITY_EDITOR`.
- **Quality and presentation:**
  - UltraFx: MaxLights=8, LightCullDistance=26, UltraPreviewWidth=1280.
  - ProjectionSystem: HoldNear 0.6, MaxFar 250, excluded layers 8/9 (line 26), mask at 1522, and StageBudgetMs=6. The 25 ms placement budget and 'no new textures' rule are in art-bible.md:~985-988.
  - Tier table matches AdaptiveQuality.cs:49-58, and the Auto-to-Ultra rule (8 ms for 20 s) matches art-bible §6.3.1.
  - Cloud counts are 14/20/28/34. The Mat enum has exactly 24 entries and is documented append-only.
  - Grade constants are 1.4/-0.035/0.85. The pattern fade is 25-45 m, 15-30 m on Low, and cut hatch is off on Low.
  - ZoneMood.Make has the 9-parameter signature the pack gives. Room defaults match the claims (LowestFloorY 0, VoidY = LowestFloorY-6, chunk defaults 8 and 0).
- **Gameplay, tests and tooling:**
  - Gameplay: UseRange 2.25, the dot>=0.55 focus rule (unless within 1.1 m), and the Teleporter trigger 1.6x2.4x1.6 with 1 s cooldown.
  - Tests: ArtPlayTests builds a fresh Activator instance with `RoomContext(null,...)`, and ProjectionPlayTests asserts non-empty Solutions.
  - Tooling: ion.ps1 `-Filter` maps to `-testFilter` (line 98) and the UnityLockfile check is at lines 58-61. `-nographics` applies to EditMode only (line 97). BUILD.md says Exceptions None and 'do not pass -quit with -runTests'.
- **Fork content:** EndCard.cs:24 is ProjectsUrl. MaxProjects is 8 at PortfolioCatalog.cs:56 and ManorTests asserts 8 frames. HubLightTable has 4 coming-soon slots, a tutorial pod to zone 0 (line 92), and GalleryEnding opens EndCard (line 68). Commit SHAs b5423cf, dcde2e3, f7bbd58, 1fccae6, 9b3ecbd, c9c461a and d23bfc5 all exist and match their seam descriptions. There is no .gitattributes, and autocrlf is true.

The line-number drift in docs/FORK.md is systematic, so I would re-derive any other docs/FORK.md citations before agents rely on them.

The Ultra-twin error matters most. Agents may wrongly assume core zones have no Ultra twin materials and miscount the Ultra batch budget. Correct it to 'Arch kit details (Arch.cs, Arch.House.cs) plus planter plants'.

## Known gaps

- Triangle counts for prism, cone and ChamferBox are derived from the mesh-builder fan triangulation (Geo.cs:320-339, ArchMeshes.cs:13-14), not measured in Unity.
- Word count of docMarkdown was not machine-checked (the word-count command was blocked); it was kept compact and is estimated to be near the 2500-word cap.
- Did not read PropKit.Plants.cs, Frames.cs, DecorCombiner.cs, WorldHistory internals, the MeshClipper family or the T1/T2/Camera/Gallery rooms beyond skimming StairsWing and HubLightTable.
- Did not run any tests, builds or Unity; the claims about what tests enforce come from reading the test source, not from a run.
- crew/w0-* branches (slots, ownership, publish v2) exist but are not merged into overhaul; their contents were not mapped.
- Per-prop ChamferBox is used by PropBuild.Chamfer; its real triangle count after clipping was not verified.

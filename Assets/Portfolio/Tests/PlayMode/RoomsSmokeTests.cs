using System.Collections;
using System.Collections.Generic;
using Ion.Gameplay;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ArchKit = Ion.Levels.Arch.Arch;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// M-F seed for M-C2's room folders. The kit: ManorHall takes a height, the padless ManorDoor works on every wall,
    /// the apse helper closes a hall, and the DoorPrint and HallSet contracts build nothing yet. The rooms: the Hall
    /// of Honours, the Camera Room and the Painting Wing keep their plan shells, arrivals and padless way back to the
    /// Grand Gallery. These checks hold only what the plan locks, so the rooms can fill up with exhibits without
    /// breaking them: doors are found by their label, walls by short rays at their plan planes, and distances to
    /// arrival are measured on the floor, as M-D002 measures them.
    /// </summary>
    public sealed class RoomsSmokeTests : IonPlayTestBase
    {
        const float HalfX = 6f, Z0 = -4f, Z1 = 12f, Height = 7f;

        /// <summary>What the plan locks for a room: its M-D004 shell, its arrival and its way back (M-D002, M-D006).</summary>
        sealed class Plan
        {
            public string Key;
            public float HalfX, Z0, Z1, Height;
            /// <summary>The Hall's apse starts here (M-D004); 0 for a room without one.</summary>
            public float ApseZ;
            /// <summary>The arrival as the room builds it (checked against the plan where the plan fixes it).</summary>
            public Vector3 Arrival;
            /// <summary>M-D002: exactly one [ GRAND GALLERY ] door (otherwise at least one).</summary>
            public bool OneGalleryDoor;
            /// <summary>The room's last solution is the Teleport back to the Gallery (a dead-end room).</summary>
            public bool EndsInGallery;
        }

        static readonly Plan[] Rooms =
        {
            new Plan { Key = PortfolioRegistrar.HonoursKey, HalfX = 8f, Z0 = -4f, Z1 = 14f, Height = 9f, ApseZ = 8f, Arrival = HallOfHonours.Arrival, OneGalleryDoor = true, EndsInGallery = true },
            new Plan { Key = PortfolioRegistrar.LensKey, HalfX = 8f, Z0 = -4f, Z1 = 20f, Height = 6f, Arrival = CameraRoom.Arrival },
            new Plan { Key = PortfolioRegistrar.WingKey, HalfX = 6f, Z0 = -4f, Z1 = 10f, Height = 7f, Arrival = PaintingWing.Arrival, OneGalleryDoor = true },
        };

        static Transform Scratch(string name, float x)
        {
            var root = new GameObject(name).transform;
            root.position = new Vector3(x, 500f, 0f);
            return root;
        }

        // ------------------------------------------------------------------ the kit

        [Test]
        public void ManorDoor_WorksOnEveryWall()
        {
            Transform root = Scratch("smoke doors", 4000f);
            ArchStyle before = ArchKit.Style;
            try
            {
                ArchKit.Style = ManorLook.Style;
                WallOpening[] doors =
                {
                    ManorDoor.Opening(Dir.PosZ, -2f), ManorDoor.Opening(Dir.NegZ, 2f),
                    ManorDoor.Opening(Dir.PosX, 4f), ManorDoor.Opening(Dir.NegX, 6f),
                };
                ManorHall.Build(root, HalfX, Z0, Z1, Height, doors);
                var ctx = new RoomContext(null, 99, new PaintingWing(), root, root.position + Vector3.down * 200f);
                var thresholds = new List<Vector3>();
                foreach (WallOpening d in doors)
                {
                    Vector3 face = ManorHall.Face(HalfX, Z0, Z1, d);
                    Vector3 threshold = ManorDoor.Build(root, ctx, face, d.Facing, ManorDoor.GalleryLabel, null);
                    Assert.AreEqual(ManorDoor.TriggerInset, Vector3.Distance(face, threshold), 1e-4f, d.Facing + " threshold");
                    thresholds.Add(threshold);
                }
                var errors = new List<string>();
                ArchKit.Validate(root, errors);
                Assert.IsEmpty(errors, string.Join("\n", errors));

                Physics.SyncTransforms();
                for (int i = 0; i < doors.Length; i++)
                {
                    // From the threshold, the wall behind the door is the niche's back: the inset plus the recess.
                    Vector3 from = root.TransformPoint(thresholds[i] + Vector3.up * 1.5f);
                    Assert.IsTrue(Physics.Raycast(from, -PropBuild.Vec(doors[i].Facing), out RaycastHit hit, 4f, ~0, QueryTriggerInteraction.Ignore), doors[i].Facing + ": no wall");
                    Assert.AreEqual(ManorDoor.TriggerInset + ManorDoor.Depth, hit.distance, 0.02f, doors[i].Facing + ": niche depth");
                }
                Teleporter[] teleporters = root.GetComponentsInChildren<Teleporter>();
                Assert.AreEqual(4, teleporters.Length);
                Assert.AreEqual(4, ManorDoor.Find(root, ManorDoor.GalleryLabel).Count, "doors are found by their label");
                Assert.AreEqual(0, ManorDoor.Find(root, "[ FOYER ]").Count);
                foreach (Teleporter t in teleporters)
                    Assert.AreEqual(0, t.GetComponentsInChildren<Renderer>(true).Length, "padless: no pad mesh");
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(root.gameObject);
            }
        }

        [Test]
        public void ManorHall_ApseClosesTheFarEnd()
        {
            Transform root = Scratch("smoke apse", 4100f);
            ArchStyle before = ArchKit.Style;
            try
            {
                ArchKit.Style = ManorLook.Style;
                ManorHall.Build(root, 8f, -4f, 14f, 9f);
                GameObject apse = ManorHall.Apse(root, 8f, 8f, 14f, 9f);
                Assert.AreEqual(3, apse.transform.childCount, "three facets");
                var errors = new List<string>();
                ArchKit.Validate(root, errors);
                Assert.IsEmpty(errors, string.Join("\n", errors));
                Physics.SyncTransforms();
                Vector3 eye = root.TransformPoint(new Vector3(0f, 1.5f, 0f));
                Assert.IsTrue(Physics.Raycast(eye, Vector3.forward, out RaycastHit far, 30f, ~0, QueryTriggerInteraction.Ignore));
                Assert.That(far.distance, Is.InRange(8f, 14f), "the apse stands between z 8 and 14");
                Assert.IsTrue(Physics.Raycast(eye, Vector3.up, out RaycastHit up, 30f, ~0, QueryTriggerInteraction.Ignore));
                Assert.AreEqual(7.5f, up.distance, 0.05f, "a 9 m ceiling");
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(root.gameObject);
            }
        }

        [Test]
        public void RoomContracts_BuildNothingYet()
        {
            Transform root = Scratch("smoke contracts", 4200f);
            try
            {
                Assert.IsNull(DoorPrint.Hang(root, Vector3.zero, Dir.NegZ, null));
                HallSet.ApseBay(root);
                Assert.AreEqual(0, root.childCount);
            }
            finally
            {
                Object.Destroy(root.gameObject);
            }
        }

        // ------------------------------------------------------------------ the rooms

        /// <summary>The rooms' own constants match the plan: M-D004 sizes, the Hall's arrival and door (M-D006).</summary>
        [Test]
        public void ManorRooms_ConstantsMatchThePlan()
        {
            Assert.AreEqual(new[] { 8f, -4f, 14f, 9f, 8f },
                            new[] { HallOfHonours.HalfX, HallOfHonours.Z0, HallOfHonours.Z1, HallOfHonours.Height, HallOfHonours.ApseZ }, "Hall 16 x 18 x 9, apse from z 8");
            Assert.AreEqual(new[] { 8f, -4f, 20f, 6f }, new[] { CameraRoom.HalfX, CameraRoom.Z0, CameraRoom.Z1, CameraRoom.Height }, "Camera Room 16 x 24 x 6");
            Assert.AreEqual(new[] { 6f, -4f, 10f, 7f }, new[] { PaintingWing.HalfX, PaintingWing.Z0, PaintingWing.Z1, PaintingWing.Height }, "Wing 12 x 14 x 7");
            Assert.AreEqual(new Vector3(0f, 0f, -1.5f), HallOfHonours.Arrival, "Hall arrival (M-D006)");
            Assert.AreEqual(Dir.PosZ, HallOfHonours.GalleryDoor.Facing, "the Hall's way back is in the near wall");
            Assert.AreEqual(-2.25f, HallOfHonours.GalleryDoor.Along, "Hall door at x -2.25 (M-D006)");
        }

        /// <summary>Each room spawns at its arrival inside a shell at its plan size (the Hall with its apse).</summary>
        [UnityTest]
        public IEnumerator ManorRooms_ShellsAtTheirPlanSize()
        {
            var problems = new List<string>();
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                RoomContext ctx = Room(p.Key);
                string tag = p.Key + ": ";
                Vector3 spawn = ctx.WorldRoot.InverseTransformPoint(ctx.Spawn.position);
                if (Vector3.Distance(spawn, p.Arrival) > 0.01f) problems.Add(tag + "spawn " + spawn + ", arrival " + p.Arrival);
                ProbeShell(problems, ctx, tag, p.HalfX, p.Z0, p.Z1, p.Height);
                if (p.ApseZ > 0f) ProbeApse(problems, ctx, tag, p.HalfX, p.ApseZ, p.Z1);
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        /// <summary>The padless [ GRAND GALLERY ] door: found by its label, within 3.0 m of arrival on the floor, no pads.</summary>
        [UnityTest]
        public IEnumerator ManorRooms_PadlessWayBackNearArrival()
        {
            var problems = new List<string>();
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                CheckWayBack(problems, Room(p.Key), p.Arrival, p.OneGalleryDoor, p.EndsInGallery);
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        [UnityTest]
        public IEnumerator ManorRooms_WalkIntoTheGalleryDoorWithin3s()
        {
            int gallery = Game.IndexOfKey(PortfolioRegistrar.GalleryKey);
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                Teleporter door = NearestDoor(Room(p.Key));
                Assert.IsNotNull(door, p.Key + " has no " + ManorDoor.GalleryLabel + " door");
                Player.ScriptedWalkTo(door.transform.position, 3f, 0.2f);
                float t = 0f;
                while (Game.CurrentRoom != gallery && t < 3f)
                {
                    yield return null;
                    t += Step;
                }
                Player.StopScriptedWalk();
                Debug.Log("[IonTest] " + p.Key + " door: in the Grand Gallery after " + t.ToString("0.00") + " s");
                Assert.AreEqual(gallery, Game.CurrentRoom, p.Key + ": walking into the padless door did not land in the Grand Gallery within 3 s");
            }
        }

        /// <summary>Every room's solution script runs; each Teleport lands where it says, and the Hall's ends in the Gallery.</summary>
        [UnityTest]
        public IEnumerator ManorRooms_SolutionsRun()
        {
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                yield return RunAll(Room(p.Key));
                if (p.EndsInGallery)
                    Assert.AreEqual(Game.IndexOfKey(PortfolioRegistrar.GalleryKey), Game.CurrentRoom, p.Key + " leads back to the Grand Gallery");
            }
        }

        [Test]
        public void ManorRooms_WayBackIsANoOpWithoutAGame()
        {
            var container = new GameObject("M-C2 scratch").transform;
            container.position = new Vector3(3000f, 500f, 0f);
            ArchStyle before = ArchKit.Style;
            try
            {
                for (int i = 0; i < Rooms.Length; i++)
                {
                    Room fresh = PortfolioRegistrar.Create(Rooms[i].Key);
                    var root = new GameObject(fresh.Key).transform;
                    root.SetParent(container, false);
                    root.localPosition = new Vector3(i * 50f, 0f, 0f);
                    var ctx = new RoomContext(null, 90 + i, fresh, root, container.position + new Vector3(i * 50f, -200f, 0f));
                    ArchKit.Style = fresh.Style;
                    fresh.Build(root, ctx);
                    List<Teleporter> doors = ManorDoor.Find(root, ManorDoor.GalleryLabel);
                    Assert.Greater(doors.Count, 0, fresh.Key + " has its way back");
                    foreach (Teleporter door in doors)
                        Assert.DoesNotThrow(() => door.OnEnter(), fresh.Key + ": with no game the door is a no-op");
                }
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(container.gameObject);
            }
        }

        /// <summary>The probes that let the rooms fill up still catch a wrong shell: a hall 0.5 m too narrow, or no apse.</summary>
        [Test]
        public void ShellProbes_CatchAWrongShell()
        {
            Transform right = Scratch("probe hall", 4300f), narrow = Scratch("probe narrow", 4400f), open = Scratch("probe no apse", 4500f);
            ArchStyle before = ArchKit.Style;
            try
            {
                ArchKit.Style = ManorLook.Style;
                ManorHall.Build(right, 8f, -4f, 14f, 9f);
                ManorHall.Apse(right, 8f, 8f, 14f, 9f);
                ManorHall.Build(narrow, 7.5f, -4f, 14f, 9f);
                ManorHall.Build(open, 8f, -4f, 14f, 9f);
                Physics.SyncTransforms();
                RoomContext Ctx(Transform t) => new RoomContext(null, 98, new HallOfHonours(), t, t.position + Vector3.down * 200f);

                var problems = new List<string>();
                ProbeShell(problems, Ctx(right), "right: ", 8f, -4f, 14f, 9f);
                ProbeApse(problems, Ctx(right), "right: ", 8f, 8f, 14f);
                Assert.IsEmpty(problems, "the plan shell passes:\n" + string.Join("\n", problems));

                ProbeShell(problems, Ctx(narrow), "narrow: ", 8f, -4f, 14f, 9f);
                Assert.AreEqual(2, problems.Count, "both side walls are missed:\n" + string.Join("\n", problems));
                problems.Clear();
                ProbeApse(problems, Ctx(open), "open: ", 8f, 8f, 14f);
                Assert.AreEqual(2, problems.Count, "both apse sides are missed:\n" + string.Join("\n", problems));
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(right.gameObject);
                Object.Destroy(narrow.gameObject);
                Object.Destroy(open.gameObject);
            }
        }

        // ------------------------------------------------------------------ probes

        static Teleporter NearestDoor(RoomContext ctx)
        {
            Teleporter best = null;
            float bestD = float.MaxValue;
            foreach (Teleporter t in ManorDoor.Find(ctx.WorldRoot, ManorDoor.GalleryLabel))
            {
                float d = Vector3.Distance(t.transform.position, ctx.Spawn.position);
                if (d < bestD) { bestD = d; best = t; }
            }
            return best;
        }

        static void CheckWayBack(List<string> problems, RoomContext ctx, Vector3 arrival, bool exactlyOne, bool endsInGallery)
        {
            string tag = ctx.Room.Key + ": ";
            Transform root = ctx.WorldRoot;
            List<Teleporter> doors = ManorDoor.Find(root, ManorDoor.GalleryLabel);
            if (exactlyOne ? doors.Count != 1 : doors.Count == 0)
                problems.Add(tag + doors.Count + " " + ManorDoor.GalleryLabel + " doors" + (exactlyOne ? " (M-D002: exactly one)" : ""));
            if (root.GetComponentsInChildren<TeleporterFx>(true).Length != 0) problems.Add(tag + "has PropKit teleporter pads (M-D024)");
            foreach (Teleporter door in doors)
            {
                if (door.GetComponentsInChildren<Renderer>(true).Length != 0) problems.Add(tag + "the door has a pad mesh");
                Vector3 d = door.GetComponent<Collider>().bounds.center - ctx.World(arrival);
                float floor = new Vector2(d.x, d.z).magnitude;
                Debug.Log("[IonTest] " + tag + "door trigger centre " + floor.ToString("0.00") + " m from arrival on the floor (" + d.magnitude.ToString("0.00") + " m from the arrival feet)");
                if (floor > 3f) problems.Add(tag + "door trigger centre " + floor.ToString("0.00") + " m from arrival (M-D002: 3.0 m or less)");
            }

            List<RoomSolution> sol = ctx.Room.Solutions;
            if (!sol.Exists(s => s.Action == RoomSolution.Kind.Walk)) problems.Add(tag + "no Walk solution");
            foreach (RoomSolution s in sol)
                if (s.Action == RoomSolution.Kind.Teleport && s.Destination == PortfolioRegistrar.GalleryKey &&
                    !doors.Exists(door => Vector3.Distance(ctx.SolutionFeet(s), door.transform.position) < 0.01f))
                    problems.Add(tag + "the Teleport '" + s.Name + "' to the Gallery is not at a " + ManorDoor.GalleryLabel + " door");
            RoomSolution last = sol.Count > 0 ? sol[sol.Count - 1] : null;
            if (endsInGallery && (last == null || last.Action != RoomSolution.Kind.Teleport || last.Destination != PortfolioRegistrar.GalleryKey))
                problems.Add(tag + "the last solution is not the Teleport to " + PortfolioRegistrar.GalleryKey);
        }

        /// <summary>
        /// Checks the four walls and the ceiling at their plan planes. Each probe is a short ray that starts 5 cm in
        /// front of the plane and looks into it. Exhibits, niches and openings can cover some spots but never all of
        /// them, so one hit at 5 cm per surface is enough. A shell built at the wrong size gets none.
        /// </summary>
        static void ProbeShell(List<string> problems, RoomContext ctx, string tag, float halfX, float z0, float z1, float height)
        {
            float[] ys = { 2f, Mathf.Min(3.25f, height - 0.75f) };
            Surface(problems, ctx, tag + "left wall x -" + halfX, ys, u => new Vector3(-halfX, 0f, Mathf.Lerp(z0, z1, u)), Vector3.left);
            Surface(problems, ctx, tag + "right wall x +" + halfX, ys, u => new Vector3(halfX, 0f, Mathf.Lerp(z0, z1, u)), Vector3.right);
            Surface(problems, ctx, tag + "near wall z " + z0, ys, u => new Vector3(Mathf.Lerp(-halfX, halfX, u), 0f, z0), Vector3.back);
            Surface(problems, ctx, tag + "far wall z " + z1, ys, u => new Vector3(Mathf.Lerp(-halfX, halfX, u), 0f, z1), Vector3.forward);
            int hits = 0;
            for (int i = 1; i <= 3; i++)
                for (int j = 1; j <= 5; j++)
                    if (FaceAt(ctx, new Vector3(Mathf.Lerp(-halfX, halfX, i / 4f), height, Mathf.Lerp(z0, z1, j / 6f)), Vector3.up)) hits++;
            if (hits == 0) problems.Add(tag + "no ceiling at y " + height);
        }

        static void Surface(List<string> problems, RoomContext ctx, string what, float[] ys, System.Func<float, Vector3> along, Vector3 outward)
        {
            int hits = 0, probes = 0;
            for (int i = 1; i <= 9; i++)
                foreach (float y in ys)
                {
                    Vector3 p = along(i / 10f);
                    p.y = y;
                    probes++;
                    if (FaceAt(ctx, p, outward)) hits++;
                }
            if (hits == 0) problems.Add(what + ": no face at the plan plane in " + probes + " probes");
        }

        static bool FaceAt(RoomContext ctx, Vector3 localOnPlane, Vector3 outward)
        {
            Vector3 dir = ctx.WorldRoot.TransformDirection(outward);
            return Physics.Raycast(ctx.World(localOnPlane - outward * 0.05f), dir, out RaycastHit hit, 0.12f, WorldMask, QueryTriggerInteraction.Ignore)
                   && hit.distance > 0.02f && hit.distance < 0.08f;
        }

        /// <summary>
        /// The faceted apse closes the far end (M-D004: z <paramref name="zStart"/>..<paramref name="zEnd"/>). Rays start
        /// 1.5 m inside the half-ellipse, well off the axis where the hero stands, and look out along its radius. A
        /// facet (a chord inside the ellipse) stops each ray within 1.55 m; without the apse they run 2 m or more to
        /// the side walls.
        /// </summary>
        static void ProbeApse(List<string> problems, RoomContext ctx, string tag, float halfX, float zStart, float zEnd)
        {
            foreach (int side in new[] { -1, 1 })
            {
                int hits = 0;
                foreach (float deg in new[] { 20f, 30f, 40f })
                    foreach (float y in new[] { 2f, 3.25f })
                    {
                        float a = deg * Mathf.Deg2Rad;
                        var onEllipse = new Vector3(side * Mathf.Cos(a) * halfX, y, zStart + Mathf.Sin(a) * (zEnd - zStart));
                        Vector3 radial = new Vector3(onEllipse.x, 0f, onEllipse.z - zStart).normalized;
                        Vector3 from = onEllipse - radial * 1.5f;
                        if (Physics.Raycast(ctx.World(from), ctx.WorldRoot.TransformDirection(radial), out RaycastHit hit, 3f, WorldMask, QueryTriggerInteraction.Ignore)
                            && hit.distance > 0.05f && hit.distance < 1.55f) hits++;
                    }
                if (hits == 0) problems.Add(tag + "no apse facet on the " + (side < 0 ? "left" : "right") + " between z " + zStart + " and " + zEnd);
            }
        }
    }
}

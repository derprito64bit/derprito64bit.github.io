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
    /// M-F seed for M-C7. The WorkshopSet contract builds nothing yet. The Workshop and the Study keep their plan
    /// shells (M-D004), arrivals and one padless [ GRAND GALLERY ] door within 3.0 m of arrival on the floor
    /// (M-D002, M-D026, M-D027), and their solutions end in the Grand Gallery. These checks hold only what the plan
    /// locks, so the rooms can fill up without breaking them: the door is found by its label, walls by short rays at
    /// their plan planes.
    /// </summary>
    public sealed class WorkshopStudySmokeTests : IonPlayTestBase
    {
        sealed class Plan
        {
            public string Key;
            public float HalfX, Z0, Z1, Height;
            public Vector3 Arrival;
        }

        static readonly Plan[] Rooms =
        {
            new Plan { Key = PortfolioRegistrar.WorkshopKey, HalfX = 7f, Z0 = -4f, Z1 = 12f, Height = 7f, Arrival = ManorWorkshop.Arrival },
            new Plan { Key = PortfolioRegistrar.StudyKey, HalfX = 5f, Z0 = -4f, Z1 = 10f, Height = 4.5f, Arrival = ManorStudy.Arrival },
        };

        [Test]
        public void WorkshopSet_BuildsNothingYet()
        {
            var root = new GameObject("smoke workshop set").transform;
            try
            {
                WorkshopSet.StatueBay(root);
                Assert.AreEqual(0, root.childCount);
            }
            finally
            {
                Object.Destroy(root.gameObject);
            }
        }

        /// <summary>The rooms' constants match the plan: M-D004 sizes, the arrivals and the door x (M-D026, M-D027).</summary>
        [Test]
        public void WorkshopAndStudy_ConstantsMatchThePlan()
        {
            Assert.AreEqual(new[] { 7f, -4f, 12f, 7f }, new[] { ManorWorkshop.HalfX, ManorWorkshop.Z0, ManorWorkshop.Z1, ManorWorkshop.Height }, "Workshop 14 x 16 x 7");
            Assert.AreEqual(new[] { 5f, -4f, 10f, 4.5f }, new[] { ManorStudy.HalfX, ManorStudy.Z0, ManorStudy.Z1, ManorStudy.Height }, "Study 10 x 14 x 4.5");
            Assert.AreEqual(new Vector3(0f, 0f, -1.5f), ManorWorkshop.Arrival, "Workshop arrival (M-D026)");
            Assert.AreEqual(new Vector3(0f, 0f, -2f), ManorStudy.Arrival, "Study arrival (M-D027)");
            Assert.AreEqual(Dir.PosZ, ManorWorkshop.GalleryDoor.Facing);
            Assert.AreEqual(-2.25f, ManorWorkshop.GalleryDoor.Along, "Workshop door at x -2.25 (M-D026)");
            Assert.AreEqual(Dir.PosZ, ManorStudy.GalleryDoor.Facing);
            Assert.AreEqual(2.25f, ManorStudy.GalleryDoor.Along, "Study door at x +2.25 (M-D027)");
        }

        [Test]
        public void WorkshopAndStudy_GalleryDoorWithin3mOfArrival()
        {
            Check("workshop", ManorWorkshop.Arrival, ManorWorkshop.GalleryDoor, ManorWorkshop.HalfX, ManorWorkshop.Z0, ManorWorkshop.Z1);
            Check("study", ManorStudy.Arrival, ManorStudy.GalleryDoor, ManorStudy.HalfX, ManorStudy.Z0, ManorStudy.Z1);
        }

        // M-D002 measures the way back on the floor: from the arrival spot to the trigger centre's footprint.
        static void Check(string room, Vector3 arrival, WallOpening door, float halfX, float z0, float z1)
        {
            Vector3 threshold = ManorHall.Face(halfX, z0, z1, door) + PropBuild.Vec(door.Facing) * ManorDoor.TriggerInset;
            Vector3 d = threshold - arrival;
            Assert.LessOrEqual(new Vector2(d.x, d.z).magnitude, 3f, room + ": the " + ManorDoor.GalleryLabel + " trigger is too far from arrival");
        }

        [UnityTest]
        public IEnumerator WorkshopAndStudy_ShellsAtTheirPlanSize()
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
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        [UnityTest]
        public IEnumerator WorkshopAndStudy_PadlessWayBackNearArrival()
        {
            var problems = new List<string>();
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                CheckWayBack(problems, Room(p.Key), p.Arrival);
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        [UnityTest]
        public IEnumerator WorkshopAndStudy_WalkIntoTheGalleryDoorWithin3s()
        {
            int gallery = Game.IndexOfKey(PortfolioRegistrar.GalleryKey);
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                List<Teleporter> doors = ManorDoor.Find(Room(p.Key).WorldRoot, ManorDoor.GalleryLabel);
                Assert.AreEqual(1, doors.Count, p.Key + " has one " + ManorDoor.GalleryLabel + " door");
                Player.ScriptedWalkTo(doors[0].transform.position, 3f, 0.2f);
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

        /// <summary>RunAll in both rooms ends in pf.gallery (M-C7's brief: the Teleport is last).</summary>
        [UnityTest]
        public IEnumerator WorkshopAndStudy_SolutionsEndInTheGallery()
        {
            foreach (Plan p in Rooms)
            {
                yield return GoTo(p.Key);
                yield return RunAll(Room(p.Key));
                Assert.AreEqual(Game.IndexOfKey(PortfolioRegistrar.GalleryKey), Game.CurrentRoom, p.Key + " leads back to the Grand Gallery");
            }
        }

        [Test]
        public void WorkshopAndStudy_WayBackIsANoOpWithoutAGame()
        {
            var container = new GameObject("M-C7 scratch").transform;
            container.position = new Vector3(3500f, 500f, 0f);
            ArchStyle before = ArchKit.Style;
            try
            {
                for (int i = 0; i < Rooms.Length; i++)
                {
                    Room fresh = PortfolioRegistrar.Create(Rooms[i].Key);
                    var root = new GameObject(fresh.Key).transform;
                    root.SetParent(container, false);
                    root.localPosition = new Vector3(i * 50f, 0f, 0f);
                    var ctx = new RoomContext(null, 95 + i, fresh, root, container.position + new Vector3(i * 50f, -200f, 0f));
                    ArchKit.Style = fresh.Style;
                    fresh.Build(root, ctx);
                    List<Teleporter> doors = ManorDoor.Find(root, ManorDoor.GalleryLabel);
                    Assert.AreEqual(1, doors.Count, fresh.Key + " has its way back");
                    Assert.DoesNotThrow(() => doors[0].OnEnter(), fresh.Key + ": with no game the door is a no-op");
                }
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(container.gameObject);
            }
        }

        /// <summary>The probes that let the rooms fill up still catch a shell built at the wrong size.</summary>
        [Test]
        public void ShellProbes_CatchAWrongShell()
        {
            var right = new GameObject("probe study").transform;
            var small = new GameObject("probe small").transform;
            right.position = new Vector3(4600f, 500f, 0f);
            small.position = new Vector3(4700f, 500f, 0f);
            ArchStyle before = ArchKit.Style;
            try
            {
                ArchKit.Style = ManorLook.Style;
                ManorHall.Build(right, 5f, -4f, 10f, 4.5f);
                ManorHall.Build(small, 5f, -4f, 9.5f, 4f);
                Physics.SyncTransforms();
                RoomContext Ctx(Transform t) => new RoomContext(null, 97, new ManorStudy(), t, t.position + Vector3.down * 200f);

                var problems = new List<string>();
                ProbeShell(problems, Ctx(right), "right: ", 5f, -4f, 10f, 4.5f);
                Assert.IsEmpty(problems, "the plan shell passes:\n" + string.Join("\n", problems));
                ProbeShell(problems, Ctx(small), "small: ", 5f, -4f, 10f, 4.5f);
                Assert.AreEqual(2, problems.Count, "the far wall and the ceiling are missed:\n" + string.Join("\n", problems));
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(right.gameObject);
                Object.Destroy(small.gameObject);
            }
        }

        // ------------------------------------------------------------------ probes

        static void CheckWayBack(List<string> problems, RoomContext ctx, Vector3 arrival)
        {
            string tag = ctx.Room.Key + ": ";
            Transform root = ctx.WorldRoot;
            List<Teleporter> doors = ManorDoor.Find(root, ManorDoor.GalleryLabel);
            if (doors.Count != 1) problems.Add(tag + doors.Count + " " + ManorDoor.GalleryLabel + " doors (M-D002: exactly one)");
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
            RoomSolution last = sol.Count > 0 ? sol[sol.Count - 1] : null;
            if (last == null || last.Action != RoomSolution.Kind.Teleport || last.Destination != PortfolioRegistrar.GalleryKey)
                problems.Add(tag + "the last solution is not the Teleport to " + PortfolioRegistrar.GalleryKey);
            else if (!doors.Exists(door => Vector3.Distance(ctx.SolutionFeet(last), door.transform.position) < 0.01f))
                problems.Add(tag + "the Teleport to the Gallery is not at the " + ManorDoor.GalleryLabel + " door");
        }

        /// <summary>
        /// Checks the four walls and the ceiling at their plan planes with short rays that start 5 cm in front of each
        /// plane. Exhibits, niches and openings can cover some spots but never all of them, so one hit at 5 cm per
        /// surface is enough. A shell built at the wrong size gets none.
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
    }
}

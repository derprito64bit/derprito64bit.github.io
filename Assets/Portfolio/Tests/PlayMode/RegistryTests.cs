using System.Collections;
using System.Collections.Generic;
using Ion.Gameplay;
using Ion.Levels;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ArchKit = Ion.Levels.Arch.Arch;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// M-F: the seven Manor zones sit at their registry slots (M-D003), every URL alias opens its room, and each stub
    /// room is a working shell at its M-D004 size with an arrival, a walk and a padless [ GRAND GALLERY ] door.
    /// </summary>
    public sealed class RegistryTests : IonPlayTestBase
    {
        const string Foyer = PortfolioRegistrar.FoyerKey, Gallery = PortfolioRegistrar.GalleryKey;

        /// <summary>A stub room as the plan sizes it (M-D004 inner sizes, arrival spots from M-D006..M-D008, M-D026, M-D027).</summary>
        sealed class Shell
        {
            public string Key;
            public float HalfX, Z0, Z1, Height;
            public Vector3 Arrival;
            public bool Apse;
        }

        static readonly Shell[] Stubs =
        {
            new Shell { Key = PortfolioRegistrar.HonoursKey, HalfX = 8f, Z0 = -4f, Z1 = 14f, Height = 9f, Arrival = new Vector3(0f, 0f, -1.5f), Apse = true },
            new Shell { Key = PortfolioRegistrar.LensKey, HalfX = 8f, Z0 = -4f, Z1 = 20f, Height = 6f, Arrival = new Vector3(0f, 0f, -2f) },
            new Shell { Key = PortfolioRegistrar.WingKey, HalfX = 6f, Z0 = -4f, Z1 = 10f, Height = 7f, Arrival = new Vector3(0f, 0f, -2f) },
            new Shell { Key = PortfolioRegistrar.WorkshopKey, HalfX = 7f, Z0 = -4f, Z1 = 12f, Height = 7f, Arrival = new Vector3(0f, 0f, -1.5f) },
            new Shell { Key = PortfolioRegistrar.StudyKey, HalfX = 5f, Z0 = -4f, Z1 = 10f, Height = 4.5f, Arrival = new Vector3(0f, 0f, -2f) },
        };

        [UnityTest]
        public IEnumerator Registry_SevenZonesAtTheirSlots()
        {
            yield return null;
            string[] keys =
            {
                Foyer, Gallery, PortfolioRegistrar.HonoursKey, PortfolioRegistrar.LensKey, PortfolioRegistrar.WingKey,
                PortfolioRegistrar.WorkshopKey, PortfolioRegistrar.StudyKey,
            };
            Assert.AreEqual(keys.Length, PortfolioRegistrar.Zones.Length);
            var registered = new List<string>();
            foreach (string k in ZoneCatalog.Keys)
                if (k.StartsWith("pf.")) registered.Add(k);
            CollectionAssert.AreEqual(keys, registered, "the catalogue builds the Manor zones in plan order");
            for (int i = 0; i < keys.Length; i++)
            {
                Assert.AreEqual(keys[i], PortfolioRegistrar.Zones[i].Key);
                Assert.AreEqual(1000 + 10 * i, PortfolioRegistrar.Zones[i].Order, keys[i] + " order");
                int index = Game.IndexOfKey(keys[i]);
                Assert.AreEqual(6 + i, index, keys[i] + " index");
                Assert.AreEqual(300f + 50f * i, Game.Rooms[index].WorldRoot.position.x, 1e-3f, keys[i] + " x");
            }
            int starry = Game.IndexOfKey(StarryNightKeys.Key);
            if (starry >= 0) Assert.GreaterOrEqual(starry, 13, "Painting Worlds follow the Manor");
            Assert.GreaterOrEqual(StarryNightKeys.Order, 1100);
            Assert.AreEqual(6, Game.Rooms.Count - ZoneCatalog.Count, "six core zones, then the extensions");
        }

        [Test]
        public void StartKey_AliasesOpenEveryRoom()
        {
            var aliases = new (string Alias, string Key)[]
            {
                ("honours", PortfolioRegistrar.HonoursKey), ("honors", PortfolioRegistrar.HonoursKey), ("hall", PortfolioRegistrar.HonoursKey),
                ("lens", PortfolioRegistrar.LensKey), ("lenses", PortfolioRegistrar.LensKey), ("camera-room", PortfolioRegistrar.LensKey),
                ("wing", PortfolioRegistrar.WingKey),
                ("workshop", PortfolioRegistrar.WorkshopKey), ("robot", PortfolioRegistrar.WorkshopKey), ("cad", PortfolioRegistrar.WorkshopKey),
                ("study", PortfolioRegistrar.StudyKey), ("about", PortfolioRegistrar.StudyKey), ("contact", PortfolioRegistrar.StudyKey),
            };
            Assert.AreEqual(13, aliases.Length, "13 new aliases");
            foreach ((string alias, string key) in aliases)
            {
                Assert.AreEqual(key, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=" + alias), alias);
                Assert.AreEqual(key, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?debug=1&zone=" + alias.ToUpperInvariant() + "#top"),
                                alias + " in capitals, after another parameter");
            }
            Assert.AreEqual("camera", PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=camera"), "?zone=camera stays the core camera wing");
            Assert.AreEqual(Gallery, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=gallery"));
            Assert.AreEqual(Foyer, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/"));
            Assert.AreEqual(PortfolioRegistrar.StudyKey, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=pf.study"));
        }

        [UnityTest]
        public IEnumerator StubRooms_AreShellsWithAPadlessWayBack()
        {
            yield return null;
            var problems = new List<string>();
            foreach (Shell s in Stubs)
            {
                RoomContext ctx = Room(s.Key);
                Transform root = ctx.WorldRoot;
                string tag = s.Key + ": ";

                Vector3 spawn = root.InverseTransformPoint(ctx.Spawn.position);
                if (Vector3.Distance(spawn, s.Arrival) > 0.01f) problems.Add(tag + "arrival " + spawn + ", plan " + s.Arrival);

                // The built shell matches the plan's inner size (rays from the arrival eye; soft trim has no collider).
                Vector3 eye = s.Arrival + Vector3.up * 1.5f;
                Probe(problems, tag + "left wall", ctx, eye, Vector3.left, s.HalfX);
                Probe(problems, tag + "right wall", ctx, eye, Vector3.right, s.HalfX);
                Probe(problems, tag + "near wall", ctx, eye, Vector3.back, s.Arrival.z - s.Z0);
                Probe(problems, tag + "ceiling", ctx, eye, Vector3.up, s.Height - eye.y);
                if (s.Apse) ProbeRange(problems, tag + "apse", ctx, eye, Vector3.forward, 8f - eye.z, s.Z1 - eye.z);
                else Probe(problems, tag + "far wall", ctx, eye, Vector3.forward, s.Z1 - eye.z);

                foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
                {
                    float lo = r.bounds.min.x - root.position.x, hi = r.bounds.max.x - root.position.x;
                    if (lo < -9f || hi > 9f) problems.Add(tag + "'" + r.name + "' spans x " + lo + ".." + hi + " (limit ±9 m)");
                }

                List<RoomSolution> sol = ctx.Room.Solutions;
                int walks = sol.FindAll(x => x.Action == RoomSolution.Kind.Walk).Count;
                RoomSolution last = sol.Count > 0 ? sol[sol.Count - 1] : null;
                if (walks != 1) problems.Add(tag + walks + " walk solutions");
                if (last == null || last.Action != RoomSolution.Kind.Teleport || last.Destination != Gallery)
                    problems.Add(tag + "the last solution is not the Teleport to " + Gallery);

                Teleporter[] doors = root.GetComponentsInChildren<Teleporter>(true);
                if (doors.Length != 1) problems.Add(tag + doors.Length + " teleporters (want the one Grand Gallery door)");
                if (root.GetComponentsInChildren<TeleporterFx>(true).Length != 0) problems.Add(tag + "has PropKit teleporter pads");
                foreach (Teleporter door in doors)
                {
                    if (door.GetComponentsInChildren<Renderer>(true).Length != 0) problems.Add(tag + "the door has a pad mesh");
                    Vector3 trigger = door.GetComponent<Collider>().bounds.center;
                    float d = Vector3.Distance(trigger, ctx.World(s.Arrival));
                    if (d > 3f) problems.Add(tag + "door trigger centre " + d.ToString("0.00") + " m from arrival");
                    if (last != null && Vector3.Distance(ctx.SolutionFeet(last), door.transform.position) > 0.01f)
                        problems.Add(tag + "the Teleport step is not at the door");
                    Debug.Log("[IonTest] " + tag + "door trigger centre " + d.ToString("0.00") + " m from arrival");
                }
            }
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        static void Probe(List<string> problems, string what, RoomContext ctx, Vector3 localFrom, Vector3 dir, float want) =>
            ProbeRange(problems, what, ctx, localFrom, dir, want - 0.05f, want + 0.05f);

        static void ProbeRange(List<string> problems, string what, RoomContext ctx, Vector3 localFrom, Vector3 dir, float min, float max)
        {
            if (!Physics.Raycast(ctx.World(localFrom), dir, out RaycastHit hit, 40f, WorldMask, QueryTriggerInteraction.Ignore))
                problems.Add(what + ": nothing hit");
            else if (hit.distance < min || hit.distance > max)
                problems.Add(what + " at " + hit.distance.ToString("0.00") + " m ('" + hit.collider.name + "'), plan " + min.ToString("0.00") + ".." + max.ToString("0.00"));
        }

        [UnityTest]
        public IEnumerator PadlessDoor_WalkInLandsInTheGalleryWithin3s()
        {
            int gallery = Game.IndexOfKey(Gallery);
            foreach (Shell s in Stubs)
            {
                RoomContext ctx = Room(s.Key);
                yield return GoTo(s.Key);
                List<RoomSolution> sol = ctx.Room.Solutions;
                Player.ScriptedWalkTo(ctx.SolutionFeet(sol[sol.Count - 1]), 3f, 0.2f);
                float t = 0f;
                while (Game.CurrentRoom != gallery && t < 3f)
                {
                    yield return null;
                    t += Step;
                }
                Player.StopScriptedWalk();
                Debug.Log("[IonTest] " + s.Key + " door: in the Grand Gallery after " + t.ToString("0.00") + " s");
                Assert.AreEqual(gallery, Game.CurrentRoom, s.Key + ": walking into the padless door did not land in the Grand Gallery within 3 s");
            }
        }

        [UnityTest]
        public IEnumerator StubRooms_SolutionsRunToTheGallery()
        {
            foreach (Shell s in Stubs)
            {
                yield return GoTo(s.Key);
                yield return RunAll(Room(s.Key));
                Assert.AreEqual(Game.IndexOfKey(Gallery), Game.CurrentRoom, s.Key + " leads to the Grand Gallery");
            }
        }

        [UnityTest]
        public IEnumerator StubRooms_BuildWithoutAGame()
        {
            yield return null;
            var container = new GameObject("M-F scratch").transform;
            container.position = new Vector3(3000f, 500f, 0f);
            Ion.Levels.Arch.ArchStyle before = ArchKit.Style;
            try
            {
                for (int i = 0; i < Stubs.Length; i++)
                {
                    Room fresh = PortfolioRegistrar.Create(Stubs[i].Key);
                    var root = new GameObject(fresh.Key).transform;
                    root.SetParent(container, false);
                    root.localPosition = new Vector3(i * 50f, 0f, 0f);
                    var ctx = new RoomContext(null, 90 + i, fresh, root, container.position + new Vector3(i * 50f, -200f, 0f));
                    ArchKit.Style = fresh.Style;
                    fresh.Build(root, ctx);
                    var errors = new List<string>();
                    ArchKit.Validate(root, errors);
                    Assert.IsEmpty(errors, fresh.Key + ":\n" + string.Join("\n", errors));
                    Assert.IsFalse(ctx.HasDiorama, fresh.Key + " registers no diorama");
                    Teleporter door = root.GetComponentInChildren<Teleporter>();
                    Assert.IsNotNull(door, fresh.Key + " has its door");
                    Assert.DoesNotThrow(() => door.OnEnter(), fresh.Key + ": with no game the door is a no-op");
                }
            }
            finally
            {
                ArchKit.Style = before;
                Object.Destroy(container.gameObject);
            }
        }
    }
}

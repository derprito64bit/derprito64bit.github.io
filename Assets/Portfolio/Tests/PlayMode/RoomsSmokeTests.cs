using System.Collections.Generic;
using Ion.Gameplay;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using ArchKit = Ion.Levels.Arch.Arch;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// M-F seed for M-C2's room folders: ManorHall takes a height, the padless ManorDoor works on every wall, the apse
    /// helper closes a hall, and the DoorPrint and HallSet contracts build nothing yet.
    /// </summary>
    public sealed class RoomsSmokeTests
    {
        const float HalfX = 6f, Z0 = -4f, Z1 = 12f, Height = 7f;

        static Transform Scratch(string name, float x)
        {
            var root = new GameObject(name).transform;
            root.position = new Vector3(x, 500f, 0f);
            return root;
        }

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
                    Vector3 threshold = ManorDoor.Build(root, ctx, face, d.Facing, "[ GRAND GALLERY ]", null);
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
    }
}

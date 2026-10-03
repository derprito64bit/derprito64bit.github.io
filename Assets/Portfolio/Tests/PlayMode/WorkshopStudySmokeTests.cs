using Ion.Levels.Props;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for M-C7: the WorkshopSet contract builds nothing yet, and both stub rooms keep the way back close.</summary>
    public sealed class WorkshopStudySmokeTests
    {
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

        [Test]
        public void WorkshopAndStudy_GalleryDoorWithin3mOfArrival()
        {
            Check("workshop", ManorWorkshop.Arrival, ManorWorkshop.GalleryDoor, ManorWorkshop.HalfX, ManorWorkshop.Z0, ManorWorkshop.Z1);
            Check("study", ManorStudy.Arrival, ManorStudy.GalleryDoor, ManorStudy.HalfX, ManorStudy.Z0, ManorStudy.Z1);
        }

        static void Check(string room, Vector3 arrival, WallOpening door, float halfX, float z0, float z1)
        {
            Vector3 threshold = ManorHall.Face(halfX, z0, z1, door) + PropBuild.Vec(door.Facing) * ManorDoor.TriggerInset;
            Vector3 centre = threshold + Vector3.up * (ManorDoor.TriggerSize.y * 0.5f);
            Assert.LessOrEqual(Vector3.Distance(centre, arrival), 3f, room + ": the [ GRAND GALLERY ] trigger is too far from arrival");
        }
    }
}

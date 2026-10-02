using System.Collections;
using Ion.Levels;
using Ion.Levels.Arch;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ion.Tests.PlayMode
{
    using Arch = Ion.Levels.Arch.Arch;

    /// <summary>
    /// The zone registration seam: an extension zone follows the six core zones, the start key moves the spawn and
    /// "Play again", and invalid registrations are refused.
    /// </summary>
    public sealed class ZoneCatalogTests : IonPlayTestBase
    {
        const string Key = "test.extension";

        sealed class ExtensionRoom : Room
        {
            public override string Key => ZoneCatalogTests.Key;
            public override string Title => "Extension";

            public override void Build(Transform root, RoomContext ctx)
            {
                Arch.Terrace(root, new RectXZ(-4f, -4f, 4f, 8f), 0f, 2f);
                ctx.SetSpawn(Vector3.zero, 0f);
                AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 4f));
            }
        }

        [OneTimeSetUp]
        public void RegisterExtension()
        {
            Assert.IsTrue(ZoneCatalog.Register(Key, ZoneCatalog.ExtensionOrderMin, () => new ExtensionRoom()));
            ZoneCatalog.StartKey = Key;
        }

        [OneTimeTearDown]
        public void UnregisterExtension()
        {
            ZoneCatalog.Unregister(Key);
            ZoneCatalog.StartKey = null;
        }

        [UnityTest]
        public IEnumerator Extension_FollowsCoreZonesAndIsTheStart()
        {
            int index = Game.IndexOfKey(Key);
            Assert.Greater(index, Game.IndexOfKey("gallery"), "extension zones come after the six core zones");
            Assert.AreEqual(Game.Rooms.Count - ZoneCatalog.Count, 6, "six core zones, then the extensions");
            Assert.AreEqual(index, Game.StartZone);
            Assert.AreEqual(index, Game.CurrentRoom, "the player starts in the start zone");
            Assert.AreEqual(index, Game.ZoneAt(Player.transform.position), "the spawn lies in the start zone's slot");
            yield return RunAll(Game.Rooms[index]);
        }

        [UnityTest]
        public IEnumerator Restart_ReturnsToTheStartZone()
        {
            Game.GoToRoom(Game.IndexOfKey("hub"));
            yield return null;
            Game.Restart();
            yield return null;
            Assert.AreEqual(Game.StartZone, Game.CurrentRoom, "Play again returns to the start zone");
        }

        [Test]
        public void Register_RefusesInvalidEntries()
        {
            LogAssert.ignoreFailingMessages = true;
            try
            {
                Assert.IsFalse(ZoneCatalog.Register("", ZoneCatalog.ExtensionOrderMin, () => new ExtensionRoom()));
                Assert.IsFalse(ZoneCatalog.Register("x", ZoneCatalog.ExtensionOrderMin, null));
                Assert.IsFalse(ZoneCatalog.Register("x", ZoneCatalog.ExtensionOrderMin - 1, () => new ExtensionRoom()));
            }
            finally
            {
                LogAssert.ignoreFailingMessages = false;
            }
        }
    }
}

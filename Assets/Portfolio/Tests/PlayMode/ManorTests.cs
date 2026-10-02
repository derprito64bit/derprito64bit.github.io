using System.Collections;
using Ion.Levels;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// The fork's Manor: the visit starts in the foyer, its doors lead to the Grand Gallery and to the game, the
    /// gallery hangs every catalogued project and its far door leads home.
    /// </summary>
    public sealed class ManorTests : IonPlayTestBase
    {
        [OneTimeSetUp]
        public void StartInTheManor() => ZoneCatalog.StartKey = PortfolioRegistrar.FoyerKey;

        [OneTimeTearDown]
        public void Reset() => ZoneCatalog.StartKey = null;

        [UnityTest]
        public IEnumerator Foyer_IsTheStartAndLeadsToTheGallery()
        {
            RoomContext foyer = Room(PortfolioRegistrar.FoyerKey);
            Assert.AreEqual(foyer.Index, Game.StartZone);
            Assert.AreEqual(foyer.Index, Game.CurrentRoom, "the visit starts in the foyer");
            yield return RunAll(foyer);
            Assert.AreEqual(Game.IndexOfKey(PortfolioRegistrar.GalleryKey), Game.CurrentRoom, "the left door opens the gallery");
        }

        [UnityTest]
        public IEnumerator Foyer_GameDoorLeadsToTheTutorial()
        {
            RoomContext foyer = Room(PortfolioRegistrar.FoyerKey);
            Player.Teleport(foyer.World(ManorFoyer.GamePad + new Vector3(0f, 0f, -2f)), foyer.WorldYaw(0f));
            yield return Seconds(0.3f);
            Player.Teleport(foyer.World(ManorFoyer.GamePad), foyer.WorldYaw(0f));
            for (int i = 0; i < 180 && Game.CurrentRoom == foyer.Index; i++) yield return null;
            Assert.AreEqual(Game.IndexOfKey("t1"), Game.CurrentRoom, "the right door starts the game");
        }

        [UnityTest]
        public IEnumerator Gallery_HangsEveryProjectAndLeadsHome()
        {
            RoomContext gallery = Room(PortfolioRegistrar.GalleryKey);
            var room = (GrandGallery)gallery.Room;
            Assert.AreEqual(8, room.Paintings.Count, "eight frames, four a side");
            Assert.Greater(PortfolioCatalog.Projects.Count, 0, "the catalogue has projects");
            Game.GoToRoom(gallery.Index);
            yield return Seconds(0.3f);
            yield return RunAll(gallery);
            Assert.AreEqual(Game.IndexOfKey(PortfolioRegistrar.FoyerKey), Game.CurrentRoom, "the far door leads home");
        }

        [UnityTest]
        public IEnumerator Arcade_CabinetIsPlayableWithE()
        {
            RoomContext gallery = Room(PortfolioRegistrar.GalleryKey);
            var room = (GrandGallery)gallery.Room;
            Assert.IsNotNull(room.Arcade, "the cabinet has a usable");
            StringAssert.StartsWith("../", room.Arcade.Url, "demos are site pages next to /play/");
            Game.GoToRoom(gallery.Index);
            yield return Seconds(0.3f);
            Player.Teleport(gallery.World(GrandGallery.ArcadeBase + new Vector3(0f, 0f, -1.6f)), gallery.WorldYaw(0f));
            yield return Seconds(0.2f);
            var interactor = Player.GetComponent<Ion.Gameplay.PlayerInteractor>();
            Assert.AreSame(room.Arcade, interactor.FocusedUsable, "standing at the cabinet focuses it");
            Assert.IsTrue(interactor.UseFocused());
            Assert.AreEqual(1, room.Arcade.Plays);
        }

        [Test]
        public void Catalogue_ParsesAndStaysWithinTheGallery()
        {
            Assert.LessOrEqual(PortfolioCatalog.Projects.Count, PortfolioCatalog.MaxProjects);
            foreach (ProjectEntry p in PortfolioCatalog.Projects)
            {
                Assert.IsFalse(string.IsNullOrEmpty(p.slug), "every project has a slug");
                Assert.IsFalse(string.IsNullOrEmpty(p.title), p.slug + ": title");
            }
        }

        [Test]
        public void StartKey_FollowsTheZoneParameter()
        {
            Assert.AreEqual(PortfolioRegistrar.FoyerKey, PortfolioRegistrar.StartKeyFromUrl("https://x.io/play/"));
            Assert.AreEqual(PortfolioRegistrar.GalleryKey, PortfolioRegistrar.StartKeyFromUrl("https://x.io/play/?zone=gallery"));
            Assert.AreEqual("t1", PortfolioRegistrar.StartKeyFromUrl("https://x.io/play/?debug=1&zone=game#top"));
            Assert.AreEqual("hub", PortfolioRegistrar.StartKeyFromUrl("https://x.io/play/?zone=hub"));
        }
    }
}

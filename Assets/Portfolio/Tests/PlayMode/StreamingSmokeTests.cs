using Ion.Portfolio;
using NUnit.Framework;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for M-C6: the load classes of M-D020.</summary>
    public sealed class StreamingSmokeTests
    {
        [Test]
        public void ZoneLoads_FollowTheLoadClasses()
        {
            foreach (string key in new[] { "t1", "t2", "hub", "stairs", "camera", "gallery",
                                           PortfolioRegistrar.FoyerKey, PortfolioRegistrar.GalleryKey, PortfolioRegistrar.LensKey })
                Assert.AreEqual(ZoneLoad.Eager, ZoneLoads.Of(key), key);
            foreach (string key in new[] { PortfolioRegistrar.HonoursKey, PortfolioRegistrar.WingKey,
                                           PortfolioRegistrar.WorkshopKey, PortfolioRegistrar.StudyKey })
                Assert.AreEqual(ZoneLoad.Lazy, ZoneLoads.Of(key), key);
            Assert.AreEqual(ZoneLoad.Unloadable, ZoneLoads.Of(StarryNightKeys.Key));
        }
    }
}

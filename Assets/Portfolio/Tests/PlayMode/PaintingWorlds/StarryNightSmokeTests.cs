using Ion.Portfolio;
using NUnit.Framework;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for G-C1: the pilot's key is a Painting World key after the Manor's orders.</summary>
    public sealed class StarryNightSmokeTests
    {
        [Test]
        public void StarryNight_KeyFollowsTheManor()
        {
            StringAssert.StartsWith(PaintingWorldRegistry.KeyPrefix, StarryNightKeys.Key);
            Assert.GreaterOrEqual(StarryNightKeys.Order, PaintingWorldRegistry.FirstOrder);
            foreach ((string key, int order) in PortfolioRegistrar.Zones)
                Assert.Less(order, StarryNightKeys.Order, key + " comes before the worlds");
        }
    }
}

using Ion.Portfolio;
using NUnit.Framework;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for G-C3: the PaintingWorldRegistry contract keeps nothing yet.</summary>
    public sealed class PortalSmokeTests
    {
        [Test]
        public void PaintingWorldRegistry_KeepsNothingYet()
        {
            Assert.IsFalse(PaintingWorldRegistry.Register(new WorldSpec { Key = StarryNightKeys.Key }));
            Assert.AreEqual(0, PaintingWorldRegistry.Worlds.Count);
            Assert.AreEqual(1100, PaintingWorldRegistry.FirstOrder);
        }
    }
}

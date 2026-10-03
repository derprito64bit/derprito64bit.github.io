using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for M-C4: the Manor look still loads, and the ManorGlow and RoomLighting contracts build nothing yet.</summary>
    public sealed class LookSmokeTests
    {
        [Test]
        public void LookContracts_BuildNothingYet()
        {
            var root = new GameObject("smoke look").transform;
            try
            {
                ManorGlow.Register(root, PoolKind.Candle);
                ManorGlow.Register(root, PoolKind.Safelight);
                RoomLighting.Light(root, null, PortfolioRegistrar.FoyerKey);
                Assert.AreEqual(0, root.childCount);
                Assert.IsNotNull(ManorLook.Candlelight);
                Assert.IsNotNull(ManorLook.Style);
            }
            finally
            {
                Object.Destroy(root.gameObject);
            }
        }
    }
}

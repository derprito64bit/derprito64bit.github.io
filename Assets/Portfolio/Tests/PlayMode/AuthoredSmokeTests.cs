using Ion.Levels.Arch;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for M-C8: no authored hero exists yet, so TryBuild builds nothing and HeroKit falls back.</summary>
    public sealed class AuthoredSmokeTests
    {
        [Test]
        public void AuthoredHero_HasNoModelsYet()
        {
            var root = new GameObject("smoke authored").transform;
            try
            {
                foreach (string id in new[] { "glass", "camera", "robot" })
                    Assert.IsFalse(AuthoredHero.TryBuild(id, root, Vector3.zero, Dir.NegZ), id);
                Assert.AreEqual(0, root.childCount);
            }
            finally
            {
                Object.Destroy(root.gameObject);
            }
        }
    }
}

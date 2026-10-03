using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for G-C2: the PaintLook contract changes nothing yet.</summary>
    public sealed class PaintLookSmokeTests
    {
        [Test]
        public void PaintLook_ChangesNothingYet()
        {
            var root = new GameObject("smoke paint look").transform;
            try
            {
                PaintLook.Apply(root, null, new PaintParams { Name = "smoke" });
                Assert.AreEqual(0, root.childCount);
            }
            finally
            {
                Object.Destroy(root.gameObject);
            }
        }
    }
}

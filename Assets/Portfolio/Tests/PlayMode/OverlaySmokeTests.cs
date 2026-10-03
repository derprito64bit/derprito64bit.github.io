using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.PlayMode
{
    /// <summary>M-F seed for M-C5: the ExhibitUsable contract attaches nothing yet, and the arcade overlay is browser-only.</summary>
    public sealed class OverlaySmokeTests
    {
        [Test]
        public void ExhibitUsable_AttachesNothingYet()
        {
            var host = new GameObject("smoke exhibit");
            try
            {
                Assert.IsNull(ExhibitUsable.Attach(host.transform, Vector3.up, ExhibitRef.Project("project-01")));
                Assert.IsNull(host.GetComponent<ExhibitUsable>());
                Assert.IsTrue(default(ExhibitRef).IsEmpty);
                Assert.AreEqual("honour:glass", ExhibitRef.Honour("glass").ToString());
                Assert.IsFalse(ArcadeOverlay.IsOpen, "the arcade overlay only exists in the browser");
            }
            finally
            {
                Object.Destroy(host);
            }
        }
    }
}

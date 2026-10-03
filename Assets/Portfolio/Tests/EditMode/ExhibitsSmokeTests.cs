using Ion.Levels.Arch;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;

namespace Ion.Tests.EditMode
{
    /// <summary>M-F seed for M-C1: the HeroKit and ExplodeSpec contracts compile, and the stubs build nothing.</summary>
    public sealed class ExhibitsSmokeTests
    {
        [Test]
        public void HeroKit_StubsBuildNothing()
        {
            var root = new GameObject("smoke exhibits").transform;
            try
            {
                var explode = new ExplodeSpec(new Vector3(0f, 1.62f, -1.5f));
                Assert.IsNull(HeroKit.GlassAward(root, Vector3.zero, Dir.NegZ));
                Assert.IsNull(HeroKit.MedalGantry(root, Vector3.zero, Dir.NegX, 3));
                Assert.IsNull(HeroKit.CameraMonument(root, Vector3.zero, Dir.NegZ));
                Assert.IsNull(HeroKit.RobotStatue(root, Vector3.zero, Dir.NegZ, explode));
                Assert.IsNull(HeroKit.Plinth(root, Vector3.zero, Vector2.one, 0.5f));
                Assert.AreEqual(0, root.childCount);
                Assert.AreEqual(0, HeroKit.Audit(null).Batches);
            }
            finally
            {
                Object.DestroyImmediate(root.gameObject);
            }
        }

        [Test]
        public void ExplodeSpec_KeepsEachPartOnItsLineOfSight()
        {
            var spec = new ExplodeSpec(new Vector3(0f, 1.62f, -1.5f));
            var p = new Vector3(0.4f, 2.2f, 8f);
            Assert.Less(Vector3.Distance(p, spec.Apply(p, 1f)), 1e-5f, "k = 1 leaves a part in place");
            Assert.Less(Vector3.Angle(spec.Apply(p, ExplodeSpec.KMin) - spec.Eye, p - spec.Eye), 0.01f, "seen from the eye it does not move");
            Assert.AreEqual(ExplodeSpec.KMax * (p - spec.Eye).magnitude, (spec.Apply(p, 2f) - spec.Eye).magnitude, 1e-4f, "k is clamped");
        }
    }
}

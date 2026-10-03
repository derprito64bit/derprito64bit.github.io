using Ion.Levels.Arch;
using UnityEngine;

namespace Ion.Portfolio
{
    using Arch = Ion.Levels.Arch.Arch;

    /// <summary>
    /// Statue-scale hero exhibits (M-D010). Contract stub seeded by M-F: every builder builds nothing and returns null
    /// until M-C1 implements it (each will try <see cref="AuthoredHero.TryBuild"/> first, then build the code hero).
    /// </summary>
    public static class HeroKit
    {
        /// <summary>The DECA glass award, 4.25 m with its plinth.</summary>
        public static GameObject GlassAward(Transform p, Vector3 basePos, Dir facing) => null;

        /// <summary>A medal gantry holding <paramref name="medals"/> medals (3 shown, room for 5).</summary>
        public static GameObject MedalGantry(Transform p, Vector3 basePos, Dir facing, int medals) => null;

        /// <summary>The X-T5 camera monument, 2.7 m.</summary>
        public static GameObject CameraMonument(Transform p, Vector3 basePos, Dir facing) => null;

        /// <summary>The robot/CAD statue, 3.5 m; <paramref name="explode"/> spreads its parts anamorphically.</summary>
        public static GameObject RobotStatue(Transform p, Vector3 basePos, Dir facing, ExplodeSpec explode = null) => null;

        /// <summary>A plinth with footprint <paramref name="size"/> (x, z) and <paramref name="height"/>.</summary>
        public static GameObject Plinth(Transform p, Vector3 basePos, Vector2 size, float height) => null;

        /// <summary>A built hero's batches and triangles (empty for null).</summary>
        public static Arch.ArtAudit Audit(GameObject hero) => hero != null ? Arch.Audit(hero.transform) : default;
    }
}

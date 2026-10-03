using Ion.Levels.Arch;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// Blender-authored heroes baked as convex parts (M-D010 b). Contract stub seeded by M-F: no authored hero exists
    /// yet, so <see cref="TryBuild"/> builds nothing and returns false, and HeroKit falls back to its code hero. M-C8
    /// owns it.
    /// </summary>
    public static class AuthoredHero
    {
        /// <summary>Builds hero <paramref name="id"/> under <paramref name="parent"/>; false when it has no authored model.</summary>
        public static bool TryBuild(string id, Transform parent, Vector3 basePos, Dir facing) => false;
    }
}

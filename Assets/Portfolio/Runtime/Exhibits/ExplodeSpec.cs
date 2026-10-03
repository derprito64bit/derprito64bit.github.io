using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Workshop robot's anamorphic explode (M-D026): a part at p moves along its line of sight from the eye E to
    /// p' = E + k (p - E), so from E the statue looks assembled. Seeded by M-F; M-C1 owns the per-part k values.
    /// </summary>
    public sealed class ExplodeSpec
    {
        public const float KMin = 0.68f, KMax = 1.16f;

        /// <summary>The viewpoint, room-local (the Workshop arrival eye is (0, 1.62, -1.5)).</summary>
        public Vector3 Eye;

        public ExplodeSpec(Vector3 eye) => Eye = eye;

        /// <summary>Where a part at <paramref name="p"/> goes for scale <paramref name="k"/> (clamped to KMin-KMax).</summary>
        public Vector3 Apply(Vector3 p, float k) => Eye + Mathf.Clamp(k, KMin, KMax) * (p - Eye);
    }
}

using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>Kinds of shader light pool (M-D014): candle flames, the Camera Room's amber safelight, the DECA sweep.</summary>
    public enum PoolKind { Candle, Safelight, Sweep }

    /// <summary>
    /// Feeds the FlatToon light pools (M-D014). Contract stub seeded by M-F: <see cref="Register"/> does nothing until
    /// up/palette adds the pools. M-C4 owns it.
    /// </summary>
    public static class ManorGlow
    {
        /// <summary>Registers a pool at <paramref name="anchor"/>.</summary>
        public static void Register(Transform anchor, PoolKind kind) { }
    }
}

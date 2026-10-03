using System;

namespace Ion.Portfolio
{
    /// <summary>When a zone is built (M-D020): at boot, on first entry, or on demand with unloading.</summary>
    public enum ZoneLoad { Eager, Lazy, Unloadable }

    /// <summary>
    /// The Manor's streaming load classes (M-D020). Seeded by M-F as plain data: nothing reads it until
    /// up/zone-streaming lands and M-C6 wires it up.
    /// </summary>
    public static class ZoneLoads
    {
        /// <summary>
        /// The core six, the Foyer, the Gallery and the Camera Room are Eager; the Hall, Wing, Workshop and Study Lazy;
        /// Painting Worlds (pw.*) Unloadable.
        /// </summary>
        public static ZoneLoad Of(string key)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("pw.", StringComparison.OrdinalIgnoreCase)) return ZoneLoad.Unloadable;
            switch (key)
            {
                case PortfolioRegistrar.HonoursKey:
                case PortfolioRegistrar.WingKey:
                case PortfolioRegistrar.WorkshopKey:
                case PortfolioRegistrar.StudyKey:
                    return ZoneLoad.Lazy;
                default:
                    return ZoneLoad.Eager;
            }
        }
    }
}

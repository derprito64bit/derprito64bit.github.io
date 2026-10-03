using System;
using System.Collections.Generic;

namespace Ion.Portfolio
{
    /// <summary>A Painting World as the Wing lists it: zone key (pw.*), title, credit line and card source.</summary>
    [Serializable]
    public sealed class WorldSpec
    {
        public string Key;
        public string Title;
        public string Credit;
        public string Card;
    }

    /// <summary>
    /// The Painting Worlds behind the Wing's frames. Worlds are zones keyed pw.* from order 1100, so they take index 13
    /// and up. Contract stub seeded by M-F: <see cref="Register"/> keeps nothing and the list stays empty until G-C3
    /// implements it.
    /// </summary>
    public static class PaintingWorldRegistry
    {
        public const string KeyPrefix = "pw.";
        public const int FirstOrder = 1100;

        public static IReadOnlyList<WorldSpec> Worlds => Array.Empty<WorldSpec>();

        /// <summary>Lists a world for the Wing; false while this is a stub.</summary>
        public static bool Register(WorldSpec spec) => false;
    }
}

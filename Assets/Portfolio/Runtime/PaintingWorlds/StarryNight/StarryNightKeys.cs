namespace Ion.Portfolio
{
    /// <summary>
    /// The Starry Night pilot's zone identity (G-D008): key pw.starry at order 1100 or more, so its index is 13 or
    /// more. Seeded by M-F; G-C1 owns this folder and registers the zone with its own registrar.
    /// </summary>
    public static class StarryNightKeys
    {
        public const string Key = "pw.starry";
        public const int Order = PaintingWorldRegistry.FirstOrder;
    }
}

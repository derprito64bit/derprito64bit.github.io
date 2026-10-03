using System;
using Ion.Levels;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The fork's single entry point: registers the seven Manor zones with <see cref="ZoneCatalog"/> before the game
    /// boots (M-D003), and in the Web player starts the visit in the Manor foyer (or wherever <c>?zone=</c> points).
    /// </summary>
    public static class PortfolioRegistrar
    {
        public const string FoyerKey = "pf.foyer";
        public const string GalleryKey = "pf.gallery";
        public const string HonoursKey = "pf.honours";
        public const string LensKey = "pf.lens";
        public const string WingKey = "pf.wing";
        public const string WorkshopKey = "pf.workshop";
        public const string StudyKey = "pf.study";

        /// <summary>
        /// The Manor zones in build order with their <see cref="ZoneCatalog"/> orders. They follow the six core zones,
        /// so their indices are 6-12 and their roots sit at x = 300-600. Painting Worlds (pw.*) start at order 1100.
        /// </summary>
        public static readonly (string Key, int Order)[] Zones =
        {
            (FoyerKey, ZoneCatalog.ExtensionOrderMin),
            (GalleryKey, ZoneCatalog.ExtensionOrderMin + 10),
            (HonoursKey, ZoneCatalog.ExtensionOrderMin + 20),
            (LensKey, ZoneCatalog.ExtensionOrderMin + 30),
            (WingKey, ZoneCatalog.ExtensionOrderMin + 40),
            (WorkshopKey, ZoneCatalog.ExtensionOrderMin + 50),
            (StudyKey, ZoneCatalog.ExtensionOrderMin + 60),
        };

        /// <summary>A new room for a Manor zone key (null for any other key).</summary>
        public static Room Create(string key)
        {
            switch (key)
            {
                case FoyerKey: return new ManorFoyer();
                case GalleryKey: return new GrandGallery();
                case HonoursKey: return new HallOfHonours();
                case LensKey: return new CameraRoom();
                case WingKey: return new PaintingWing();
                case WorkshopKey: return new ManorWorkshop();
                case StudyKey: return new ManorStudy();
                default: return null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            foreach ((string key, int order) in Zones)
                ZoneCatalog.Register(key, order, () => Create(key));
#if UNITY_WEBGL && !UNITY_EDITOR
            // Only the published site starts in the Manor; the Editor and the test runner keep T1 so the
            // collaboration repo's own tests run unchanged (fork tests set ZoneCatalog.StartKey themselves).
            ZoneCatalog.StartKey = StartKeyFromUrl(Application.absoluteURL);
#endif
        }

        /// <summary>
        /// The start zone for a page URL: <c>?zone=gallery</c> the Grand Gallery, <c>?zone=game</c> the game (T1), one of
        /// M-D003's room aliases (honours|hall, lens|lenses|camera-room, wing, workshop|robot|cad, study|about|contact)
        /// that room, any other <c>?zone=&lt;key&gt;</c> that zone (so <c>?zone=camera</c> stays the core camera wing),
        /// and the foyer otherwise.
        /// </summary>
        public static string StartKeyFromUrl(string url)
        {
            string zone = QueryValue(url, "zone");
            if (string.IsNullOrEmpty(zone)) return FoyerKey;
            switch (zone.ToLowerInvariant())
            {
                case "gallery": return GalleryKey;
                case "game":
                case "play": return "t1";
                case "foyer":
                case "manor": return FoyerKey;
                case "honours":
                case "hall": return HonoursKey;
                case "lens":
                case "lenses":
                case "camera-room": return LensKey;
                case "wing": return WingKey;
                case "workshop":
                case "robot":
                case "cad": return WorkshopKey;
                case "study":
                case "about":
                case "contact": return StudyKey;
                default: return zone;
            }
        }

        /// <summary>The first value of <paramref name="name"/> in the URL's query string (no decoding beyond '+').</summary>
        public static string QueryValue(string url, string name)
        {
            if (string.IsNullOrEmpty(url)) return null;
            int q = url.IndexOf('?');
            if (q < 0) return null;
            int hash = url.IndexOf('#', q);
            string query = hash < 0 ? url.Substring(q + 1) : url.Substring(q + 1, hash - q - 1);
            foreach (string pair in query.Split('&'))
            {
                int eq = pair.IndexOf('=');
                string key = eq < 0 ? pair : pair.Substring(0, eq);
                if (!string.Equals(key, name, StringComparison.OrdinalIgnoreCase)) continue;
                return eq < 0 ? "" : pair.Substring(eq + 1).Replace('+', ' ');
            }
            return null;
        }
    }
}

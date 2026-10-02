using System;
using Ion.Levels;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The fork's single entry point: registers the Manor zones with <see cref="ZoneCatalog"/> before the game boots,
    /// and in the Web player starts the visit in the Manor foyer (or wherever <c>?zone=</c> points).
    /// </summary>
    public static class PortfolioRegistrar
    {
        public const string FoyerKey = "pf.foyer";
        public const string GalleryKey = "pf.gallery";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            ZoneCatalog.Register(FoyerKey, ZoneCatalog.ExtensionOrderMin, () => new ManorFoyer());
            ZoneCatalog.Register(GalleryKey, ZoneCatalog.ExtensionOrderMin + 10, () => new GrandGallery());
#if UNITY_WEBGL && !UNITY_EDITOR
            // Only the published site starts in the Manor; the Editor and the test runner keep T1 so the
            // collaboration repo's own tests run unchanged (fork tests set ZoneCatalog.StartKey themselves).
            ZoneCatalog.StartKey = StartKeyFromUrl(Application.absoluteURL);
#endif
        }

        /// <summary>
        /// The start zone for a page URL: <c>?zone=gallery</c> the Grand Gallery, <c>?zone=game</c> the game (T1),
        /// any other <c>?zone=&lt;key&gt;</c> that zone, and the foyer otherwise.
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

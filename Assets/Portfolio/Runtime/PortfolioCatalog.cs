using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>One project as shown in the Grand Gallery (fields mirror Resources/Portfolio/projects.json).</summary>
    [Serializable]
    public sealed class ProjectEntry
    {
        public string slug;
        public string title;
        public string role;
        public string year;
        public string blurb;
        public string url;
        /// <summary>Hex colour for the placeholder painting (until real artwork exists).</summary>
        public string accent;
        /// <summary>Optional Resources path of the cover image (4:3), e.g. "Portfolio/covers/my-project".</summary>
        public string cover;
        /// <summary>
        /// Optional site path of a lightweight HTML5 demo (e.g. "arcade/my-game/"); the Grand Gallery's arcade
        /// cabinet plays the first project that has one.
        /// </summary>
        public string demo;

        public Color AccentColor =>
            ColorUtility.TryParseHtmlString(string.IsNullOrEmpty(accent) ? "#6B1E2A" : accent, out Color c) ? c : new Color(0.42f, 0.12f, 0.16f);

        /// <summary>"Title — role, year" (only the parts that exist).</summary>
        public string Caption
        {
            get
            {
                string s = string.IsNullOrEmpty(title) ? slug : title;
                string tail = string.IsNullOrEmpty(role) ? year : (string.IsNullOrEmpty(year) ? role : role + ", " + year);
                return string.IsNullOrEmpty(tail) ? s : s + " - " + tail;
            }
        }
    }

    [Serializable]
    sealed class CatalogFile
    {
        public string owner;
        public List<ProjectEntry> projects = new List<ProjectEntry>();
    }

    /// <summary>
    /// The owner's project list, read once from Resources/Portfolio/projects.json. The file is validated by
    /// PortfolioCatalogTests; at run time a missing or empty file yields an empty list (the gallery shows blanks).
    /// </summary>
    public static class PortfolioCatalog
    {
        public const string ResourcePath = "Portfolio/projects";
        public const int MaxProjects = 8;

        static List<ProjectEntry> s_projects;
        static string s_owner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            s_projects = null;
            s_owner = null;
        }

        public static string Owner { get { Load(); return s_owner; } }

        /// <summary>At most <see cref="MaxProjects"/> projects, in file order.</summary>
        public static IReadOnlyList<ProjectEntry> Projects { get { Load(); return s_projects; } }

        static void Load()
        {
            if (s_projects != null) return;
            s_projects = new List<ProjectEntry>();
            s_owner = "derprito64bit";
            var text = Resources.Load<TextAsset>(ResourcePath);
            if (text == null || string.IsNullOrEmpty(text.text)) return;
            CatalogFile file = JsonUtility.FromJson<CatalogFile>(text.text);
            if (file == null) return;
            if (!string.IsNullOrEmpty(file.owner)) s_owner = file.owner;
            if (file.projects == null) return;
            foreach (ProjectEntry p in file.projects)
            {
                if (p == null || s_projects.Count >= MaxProjects) continue;
                s_projects.Add(p);
            }
        }

        /// <summary>The project's cover from Resources, or a generated placeholder painting.</summary>
        public static Texture2D CoverOf(ProjectEntry p, int index)
        {
            if (p != null && !string.IsNullOrEmpty(p.cover))
            {
                var tex = Resources.Load<Texture2D>(p.cover);
                if (tex != null) return tex;
            }
            return ManorKit.PlaceholderPainting(p != null ? p.AccentColor : new Color(0.42f, 0.12f, 0.16f), index + 1);
        }
    }
}

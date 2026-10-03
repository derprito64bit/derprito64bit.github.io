using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ion.Portfolio
{
    // Content v2 (M-D011). Every type stays JsonUtility-compatible. An entry with placeholder = true shows only
    // "[PLACEHOLDER: what goes here]" text; a real entry names its owner-facts.md anchor in 'source'.

    /// <summary>A link out of an exhibit, shown as text on its wall label.</summary>
    [Serializable]
    public sealed class LinkRef
    {
        public string label;
        public string url;
    }

    /// <summary>One media item: <c>kind</c> is image, video, model or web; <c>src</c> a Resources or site path.</summary>
    [Serializable]
    public sealed class MediaRef
    {
        public string kind;
        public string src;
        public string caption;
    }

    /// <summary>An award or medal for the Hall of Honours. <c>kind</c> is glass (the hero), award or medal.</summary>
    [Serializable]
    public sealed class HonourEntry
    {
        public string id;
        public string kind;
        public string title;
        public string issuer;
        public string year;
        public string citation;
        public List<MediaRef> media = new List<MediaRef>();
        public bool placeholder;
        public string source;
    }

    [Serializable]
    public sealed class HonoursFile
    {
        public List<HonourEntry> honours = new List<HonourEntry>();
    }

    /// <summary>A statue-scale hero exhibit (the Workshop robot): what it is, plus its label text.</summary>
    [Serializable]
    public sealed class HeroSpec
    {
        public string id;
        public string title;
        public string blurb;
        public List<LinkRef> links = new List<LinkRef>();
        public bool placeholder;
        public string source;
    }

    /// <summary>A CAD bay or the 'on the bench' slot: a title, a blurb and an optional model path.</summary>
    [Serializable]
    public sealed class WorkshopEntry
    {
        public string id;
        public string title;
        public string blurb;
        public string model;
        public bool placeholder;
        public string source;
    }

    [Serializable]
    public sealed class WorkshopFile
    {
        public HeroSpec robot = new HeroSpec();
        public List<WorkshopEntry> cad = new List<WorkshopEntry>();
        public List<WorkshopEntry> bench = new List<WorkshopEntry>();
    }

    /// <summary>The Study bureau's contact label.</summary>
    [Serializable]
    public sealed class ContactCard
    {
        public string heading;
        public string email;
        public List<LinkRef> links = new List<LinkRef>();
        public bool placeholder;
        public string source;
    }

    /// <summary>One face-out chapter book in the Study.</summary>
    [Serializable]
    public sealed class ChapterEntry
    {
        public string id;
        public string title;
        public string body;
        public bool placeholder;
        public string source;
    }

    /// <summary>The portrait slot above the Study hearth; a Frost [ ] slide until the owner supplies one.</summary>
    [Serializable]
    public sealed class PortraitSlot
    {
        public string image;
        public string alt;
        public bool placeholder;
        public string source;
    }

    [Serializable]
    public sealed class StudyFile
    {
        public ContactCard contact = new ContactCard();
        public List<ChapterEntry> chapters = new List<ChapterEntry>();
        public PortraitSlot portrait = new PortraitSlot();
    }

    /// <summary>
    /// Content v2 beyond the project list: honours.json, workshop.json and study.json under Resources/Portfolio, each
    /// read once. Lists are capped at their slot counts, so rooms build slots from them and nothing empty exists.
    /// </summary>
    public static class PortfolioContent
    {
        public const string HonoursPath = "Portfolio/honours", WorkshopPath = "Portfolio/workshop", StudyPath = "Portfolio/study";
        public const int MaxHonours = 12, MaxCad = 3, MaxBench = 1, MaxChapters = 5;

        static HonoursFile s_honours;
        static WorkshopFile s_workshop;
        static StudyFile s_study;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            s_honours = null;
            s_workshop = null;
            s_study = null;
        }

        public static IReadOnlyList<HonourEntry> Honours
        {
            get
            {
                if (s_honours == null)
                {
                    s_honours = Load<HonoursFile>(HonoursPath);
                    s_honours.honours = Trim(s_honours.honours, MaxHonours);
                }
                return s_honours.honours;
            }
        }

        public static WorkshopFile Workshop
        {
            get
            {
                if (s_workshop == null)
                {
                    s_workshop = Load<WorkshopFile>(WorkshopPath);
                    s_workshop.cad = Trim(s_workshop.cad, MaxCad);
                    s_workshop.bench = Trim(s_workshop.bench, MaxBench);
                    if (s_workshop.robot == null) s_workshop.robot = new HeroSpec();
                }
                return s_workshop;
            }
        }

        public static StudyFile Study
        {
            get
            {
                if (s_study == null)
                {
                    s_study = Load<StudyFile>(StudyPath);
                    s_study.chapters = Trim(s_study.chapters, MaxChapters);
                    if (s_study.contact == null) s_study.contact = new ContactCard();
                    if (s_study.portrait == null) s_study.portrait = new PortraitSlot();
                }
                return s_study;
            }
        }

        /// <summary>Parses a content file from Resources (a missing or broken file yields an empty one).</summary>
        public static T Load<T>(string resourcePath) where T : class, new()
        {
            var text = Resources.Load<TextAsset>(resourcePath);
            if (text == null || string.IsNullOrEmpty(text.text)) return new T();
            try { return JsonUtility.FromJson<T>(text.text) ?? new T(); }
            catch (Exception e)
            {
                Debug.LogError("[PortfolioContent] " + resourcePath + ": " + e.Message);
                return new T();
            }
        }

        static List<TItem> Trim<TItem>(List<TItem> list, int max) where TItem : class
        {
            var kept = new List<TItem>();
            if (list == null) return kept;
            foreach (TItem item in list)
                if (item != null && kept.Count < max) kept.Add(item);
            return kept;
        }
    }
}

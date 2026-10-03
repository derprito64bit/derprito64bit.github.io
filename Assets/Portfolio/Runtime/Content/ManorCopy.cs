using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ion.Levels;
using Ion.Levels.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace Ion.Portfolio
{
    /// <summary>
    /// Marks a static string or string[] field as Manor wayfinding copy: a room name, door label or how-to line that
    /// states no fact about the owner. A crew declares its copy beside the code that shows it, in its own folder, for
    /// example <c>[ManorWayfinding] public const string Lectern = "Read everything as a page";</c>. The addition then
    /// shows in that crew's diff for review, and <see cref="ManorCopy.IsSourced(string, string)"/> accepts it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ManorWayfindingAttribute : Attribute
    {
    }

    /// <summary>
    /// Placeholder hygiene (M-D011) for every visible string in the Manor. Owner facts come only from the content
    /// JSON: an entry reads "[PLACEHOLDER: what goes here]" until owner-facts.md supplies it, and then names its
    /// source. All other visible text is wayfinding copy: room names, door labels and how-to lines, none of which
    /// states a fact about the owner. <see cref="IsSourced(string, string)"/> is the placeholder guard's rule.
    /// </summary>
    public static class ManorCopy
    {
        public const string PlaceholderPrefix = "[PLACEHOLDER";

        /// <summary>
        /// Wayfinding copy that is fixed by the plan or already in the Foyer and Gallery. Crews add their own with
        /// <see cref="ManorWayfindingAttribute"/> or <see cref="Allow"/> instead of editing this list. The titles of
        /// the registered pf.* rooms are added automatically.
        /// </summary>
        public static readonly string[] Wayfinding =
        {
            "The Manor", "Grand Gallery", "Hall of Honours", "Camera Room", "Painting Wing", "Workshop", "Study", "Foyer",
            "[ GRAND GALLERY ]", "[ PLAY THE GAME ]", "[ FOYER ]", "[ ARCADE ]",
            "every project, framed", "photo puzzles", "portfolio", "Coming soon",
            "Left: the Grand Gallery.  Right: the game.", "Every project, framed.",
            "Welcome. Left door: every project in the Grand Gallery. Right door: play the game.",
            "The arcade: press E at the cabinet to play.",
            // M-D005: the framed plan in the Gallery's arcade bay. M-D027: the Study lectern.
            "Plan of the house", "Read everything as a page",
        };

        static HashSet<string> s_known;
        static List<string> s_sourced;
        static readonly HashSet<string> s_allowed = new HashSet<string>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            s_known = null;
            s_sourced = null;
            s_allowed.Clear();
        }

        /// <summary>
        /// Allows wayfinding lines composed at run time, such as a label built from a Painting World's title. Call it
        /// from the room or kit code that shows the line. Prefer <see cref="ManorWayfindingAttribute"/> for fixed copy.
        /// </summary>
        public static void Allow(params string[] lines)
        {
            if (lines == null) return;
            foreach (string line in lines)
            {
                string k = Key(line);
                if (k.Length > 0) s_allowed.Add(k);
            }
        }

        public static bool IsPlaceholder(string text) =>
            text != null && text.TrimStart().StartsWith(PlaceholderPrefix, StringComparison.Ordinal);

        /// <summary>
        /// True if <paramref name="text"/> is wayfinding copy: a <see cref="Wayfinding"/> line, a registered pf.* room
        /// title, a declared or allowed line, in any case, on one or more lines, with or without "[ ]" label brackets
        /// (so "[ HALL OF HONOURS ]" passes as the Hall's door label).
        /// </summary>
        public static bool IsWayfinding(string text)
        {
            string k = Key(text);
            return k.Length > 0 && (Known.Contains(k) || s_allowed.Contains(k));
        }

        /// <summary>True if <paramref name="text"/> is the owner's handle (projects.json 'owner'), in any case, with or without brackets.</summary>
        public static bool IsOwnerHandle(string text, string ownerHandle)
        {
            string k = Key(ownerHandle);
            return k.Length > 0 && Key(text) == k;
        }

        /// <summary>
        /// True if <paramref name="text"/> may be shown: empty, wayfinding copy, the owner's handle, text from a
        /// sourced content entry, or a composition of those with "[PLACEHOLDER: ...]" parts, "[NN]" plaque numbers,
        /// the "[ ]" monogram and punctuation (a plaque, caption or frieze). Any other words fail, even next to a
        /// placeholder.
        /// </summary>
        public static bool IsSourced(string text, string ownerHandle = null) => IsSourced(text, ownerHandle, SourcedText);

        /// <summary><see cref="IsSourced(string, string)"/> against an explicit list of sourced content strings.</summary>
        public static bool IsSourced(string text, string ownerHandle, IReadOnlyList<string> sourced)
        {
            if (sourced != null && !ReferenceEquals(sourced, s_sourced)) sourced = LongestFirst(sourced);
            string t = Collapse(text);
            if (t.Length == 0 || IsWayfinding(t) || IsOwnerHandle(t, ownerHandle) || IsSourcedPhrase(t, sourced)) return true;
            int start = 0;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] != '[') continue;
                if (!SegmentOk(t.Substring(start, i - start), ownerHandle, sourced)) return false;
                int close = t.IndexOf(']', i);
                if (close < 0) return false;
                if (!TokenOk(t.Substring(i, close - i + 1), ownerHandle, sourced)) return false;
                i = close;
                start = close + 1;
            }
            return SegmentOk(t.Substring(start), ownerHandle, sourced);
        }

        /// <summary>
        /// The visible strings of every sourced content entry (placeholder false, with a source): what owner facts the
        /// rooms may show once owner-facts.md supplies them.
        /// </summary>
        public static IReadOnlyList<string> SourcedText
        {
            get
            {
                if (s_sourced != null) return s_sourced;
                var list = new List<string>();
                foreach (ProjectEntry p in PortfolioCatalog.Projects)
                    if (Real(p.placeholder, p.source))
                    {
                        Add(list, p.title, p.role, p.year, p.blurb);
                        Add(list, p.tags);
                        AddLinks(list, p.links);
                        AddMedia(list, p.media);
                    }
                foreach (HonourEntry h in PortfolioContent.Honours)
                    if (Real(h.placeholder, h.source))
                    {
                        Add(list, h.title, h.issuer, h.year, h.citation);
                        AddMedia(list, h.media);
                    }
                WorkshopFile w = PortfolioContent.Workshop;
                if (Real(w.robot.placeholder, w.robot.source))
                {
                    Add(list, w.robot.title, w.robot.blurb);
                    AddLinks(list, w.robot.links);
                }
                foreach (WorkshopEntry e in w.cad) if (Real(e.placeholder, e.source)) Add(list, e.title, e.blurb);
                foreach (WorkshopEntry e in w.bench) if (Real(e.placeholder, e.source)) Add(list, e.title, e.blurb);
                StudyFile s = PortfolioContent.Study;
                if (Real(s.contact.placeholder, s.contact.source))
                {
                    Add(list, s.contact.heading, s.contact.email);
                    AddLinks(list, s.contact.links);
                }
                foreach (ChapterEntry c in s.chapters) if (Real(c.placeholder, c.source)) Add(list, c.title, c.body);
                if (Real(s.portrait.placeholder, s.portrait.source)) Add(list, s.portrait.alt);
                s_sourced = LongestFirst(list);
                return s_sourced;
            }
        }

        // Longest first, so a title is matched before a shorter string inside it.
        static List<string> LongestFirst(IReadOnlyList<string> strings)
        {
            var list = new List<string>();
            foreach (string s in strings)
            {
                string c = Collapse(s);
                if (c.Length > 0) list.Add(c);
            }
            list.Sort((a, b) => b.Length.CompareTo(a.Length));
            return list;
        }

        /// <summary>Every string shown in the world under <paramref name="root"/>: text labels and hint toasts.</summary>
        public static List<string> VisibleText(Transform root)
        {
            var list = new List<string>();
            if (root == null) return list;
            foreach (Text t in root.GetComponentsInChildren<Text>(true)) list.Add(t.text);
            foreach (TextMesh t in root.GetComponentsInChildren<TextMesh>(true)) list.Add(t.text);
            foreach (ZoneHint h in root.GetComponentsInChildren<ZoneHint>(true)) list.Add(h.Text);
            return list;
        }

        // ------------------------------------------------------------------ the rule's parts

        // A bracketed part: a placeholder, a plaque number, the "[ ]" monogram, or a label of allowed words.
        static bool TokenOk(string token, string owner, IReadOnlyList<string> sourced) =>
            IsPlaceholder(token) || IsPlaqueNumber(token) || Key(token).Length == 0 || IsWayfinding(token) ||
            IsOwnerHandle(token, owner) || IsSourcedPhrase(token, sourced);

        // Words between bracketed parts: punctuation, allowed copy, or sourced strings joined by punctuation.
        static bool SegmentOk(string segment, string owner, IReadOnlyList<string> sourced)
        {
            if (Key(segment).Length == 0 || IsWayfinding(segment) || IsOwnerHandle(segment, owner)) return true;
            string rest = segment;
            if (sourced != null)
                foreach (string s in sourced)
                {
                    if (s.Length == 0) continue;
                    for (int at = rest.IndexOf(s, StringComparison.OrdinalIgnoreCase); at >= 0;
                         at = rest.IndexOf(s, StringComparison.OrdinalIgnoreCase))
                        rest = rest.Substring(0, at) + "\n" + rest.Substring(at + s.Length);
                }
            foreach (string piece in rest.Split('\n'))
                if (Key(piece).Length > 0 && !IsWayfinding(piece) && !IsOwnerHandle(piece, owner)) return false;
            return true;
        }

        static bool IsSourcedPhrase(string text, IReadOnlyList<string> sourced)
        {
            if (sourced == null) return false;
            string k = Key(text);
            if (k.Length == 0) return false;
            foreach (string s in sourced)
                if (Key(s) == k) return true;
            return false;
        }

        static bool IsPlaqueNumber(string bracketed)
        {
            if (bracketed.Length < 3) return false;
            for (int i = 1; i < bracketed.Length - 1; i++)
                if (!char.IsDigit(bracketed[i])) return false;
            return true;
        }

        // ------------------------------------------------------------------ normalising

        /// <summary>Whitespace runs (line breaks included) become one space, and the ends are trimmed.</summary>
        static string Collapse(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var sb = new StringBuilder(text.Length);
            bool space = false;
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    space = sb.Length > 0;
                    continue;
                }
                if (space) sb.Append(' ');
                space = false;
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>The comparison key: collapsed, lower case, without the punctuation or label brackets at either end.</summary>
        static string Key(string text)
        {
            string t = Collapse(text);
            int a = 0, b = t.Length;
            while (a < b && !char.IsLetterOrDigit(t[a])) a++;
            while (b > a && !char.IsLetterOrDigit(t[b - 1])) b--;
            return t.Substring(a, b - a).ToLowerInvariant();
        }

        static HashSet<string> Known
        {
            get
            {
                if (s_known != null) return s_known;
                var set = new HashSet<string>();
                foreach (string w in Wayfinding) AddKey(set, w);
                foreach (var zone in PortfolioRegistrar.Zones)
                {
                    Room room = PortfolioRegistrar.Create(zone.Key);
                    if (room != null) AddKey(set, room.Title);
                }
                foreach (string d in Declared()) AddKey(set, d);
                s_known = set;
                return s_known;
            }
        }

        static void AddKey(HashSet<string> set, string text)
        {
            string k = Key(text);
            if (k.Length > 0) set.Add(k);
        }

        /// <summary>
        /// Every <see cref="ManorWayfindingAttribute"/> field in this assembly and in the assemblies that reference it
        /// (the fork's runtime folders and tests).
        /// </summary>
        static List<string> Declared()
        {
            var found = new List<string>();
            Assembly home = typeof(ManorWayfindingAttribute).Assembly;
            string homeName = home.GetName().Name;
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm != home && !References(asm, homeName)) continue;
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException e) { types = e.Types; }
                foreach (Type type in types)
                {
                    if (type == null) continue;
                    foreach (FieldInfo f in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    {
                        if (!f.IsDefined(typeof(ManorWayfindingAttribute), false)) continue;
                        object value = f.IsLiteral ? f.GetRawConstantValue() : f.GetValue(null);
                        if (value is string s) found.Add(s);
                        else if (value is IEnumerable<string> many) found.AddRange(many);
                    }
                }
            }
            return found;
        }

        static bool References(Assembly asm, string name)
        {
            if (asm.IsDynamic) return false;
            try
            {
                foreach (AssemblyName r in asm.GetReferencedAssemblies())
                    if (r.Name == name) return true;
            }
            catch (Exception)
            {
                // An assembly that cannot list its references declares no Manor copy.
            }
            return false;
        }

        // ------------------------------------------------------------------ sourced content

        static bool Real(bool placeholder, string source) => !placeholder && !string.IsNullOrEmpty(source);

        static void Add(List<string> list, params string[] values)
        {
            foreach (string v in values)
            {
                string c = Collapse(v);
                if (Key(c).Length > 0) list.Add(c);
            }
        }

        static void Add(List<string> list, List<string> values)
        {
            if (values != null) Add(list, values.ToArray());
        }

        static void AddLinks(List<string> list, List<LinkRef> links)
        {
            if (links == null) return;
            foreach (LinkRef l in links) if (l != null) Add(list, l.label);
        }

        static void AddMedia(List<string> list, List<MediaRef> media)
        {
            if (media == null) return;
            foreach (MediaRef m in media) if (m != null) Add(list, m.caption);
        }
    }
}

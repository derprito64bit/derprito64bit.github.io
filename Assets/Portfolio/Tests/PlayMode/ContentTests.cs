using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Ion.Levels;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// M-F content v2 (M-D011) and the placeholder guard: every visible string in the content JSON and in the pf.*
    /// zones is sourced or a typed "[PLACEHOLDER: ...]", and no invented fact survives from the first catalogue. The
    /// checks hold for the launch placeholders and for the owner's real content alike.
    /// </summary>
    public sealed class ContentTests : IonPlayTestBase
    {
        static readonly string[] Files = { "Portfolio/projects", "Portfolio/honours", "Portfolio/workshop", "Portfolio/study" };

        /// <summary>String fields that hold data, not visible text (keys, links, colours, asset paths, provenance).</summary>
        static readonly HashSet<string> DataFields = new HashSet<string>
        {
            "owner", "slug", "id", "kind", "url", "accent", "cover", "demo", "src", "model", "image", "source",
        };

        static readonly Regex Typed = new Regex(@"^\[PLACEHOLDER: [^\[\]]+\]$");

        // Copy declared outside Content/, as a crew declares it beside the code that shows it.
        [ManorWayfinding] const string DeclaredLabel = "[ M-F DECLARED PROBE ]";
        [ManorWayfinding] static readonly string[] DeclaredLines = { "M-F declared probe line" };

        /// <summary>The files parse, and their lists stay within the plan's slot caps (the loader drops the rest).</summary>
        [Test]
        public void Content_V2FilesStayWithinThePlanCaps()
        {
            var catalog = PortfolioContent.Load<CatalogFile>(Files[0]);
            Assert.LessOrEqual(catalog.projects.Count, PortfolioCatalog.MaxProjects, "at most one project per Gallery frame");
            foreach (ProjectEntry p in catalog.projects)
            {
                Assert.IsNotNull(p.links, p.slug + " links");
                Assert.IsNotNull(p.media, p.slug + " media");
                Assert.IsNotNull(p.tags, p.slug + " tags");
            }
            Assert.AreEqual(catalog.projects.Count, PortfolioCatalog.Projects.Count, "every project is hung");

            var honours = PortfolioContent.Load<HonoursFile>(Files[1]).honours;
            Assert.AreEqual(PortfolioContent.MaxGlass, Count(honours, PortfolioContent.Glass), "one glass hero (M-D006)");
            Assert.LessOrEqual(Count(honours, PortfolioContent.Award), PortfolioContent.MaxAwards, "up to 3 plinth pairs");
            Assert.LessOrEqual(Count(honours, PortfolioContent.Medal), PortfolioContent.MaxMedals, "a gantry with room for 5");
            foreach (HonourEntry h in honours)
                Assert.Greater(PortfolioContent.MaxOfKind(h.kind), 0, h.id + ": kind '" + h.kind + "' is not glass, award or medal");
            Assert.AreEqual(honours.Count, PortfolioContent.Honours.Count, "every honour is shown");

            var workshop = PortfolioContent.Load<WorkshopFile>(Files[2]);
            Assert.IsFalse(string.IsNullOrEmpty(workshop.robot.id), "the robot hero spec (M-D026)");
            Assert.LessOrEqual(workshop.cad.Count, PortfolioContent.MaxCad, "up to 3 CAD bays");
            Assert.LessOrEqual(workshop.bench.Count, PortfolioContent.MaxBench, "one 'on the bench' slot");
            Assert.AreEqual(workshop.cad.Count, PortfolioContent.Workshop.cad.Count);
            Assert.AreEqual(workshop.bench.Count, PortfolioContent.Workshop.bench.Count);

            var study = PortfolioContent.Load<StudyFile>(Files[3]);
            Assert.LessOrEqual(study.chapters.Count, PortfolioContent.MaxChapters, "up to 5 chapter books (M-D027)");
            Assert.IsNotNull(study.contact, "the contact label");
            Assert.IsNotNull(study.portrait, "the portrait slot");
            Assert.AreEqual(study.chapters.Count, PortfolioContent.Study.chapters.Count);
        }

        static int Count(List<HonourEntry> list, string kind)
        {
            int n = 0;
            foreach (HonourEntry h in list) if (h.kind == kind) n++;
            return n;
        }

        /// <summary>The first catalogue's invented 'Project One', 'Your role' and '2026' never return as placeholders.</summary>
        [Test]
        public void Content_NoInventedTextFromTheFirstCatalogue()
        {
            var problems = new List<string>();
            void Banned(string field, string text, string at, bool placeholder)
            {
                if (!placeholder || text == null) return;
                foreach (string banned in new[] { "Project One", "Your role", "2026" })
                    if (text.Contains(banned)) problems.Add(at + " = '" + text + "' (a placeholder holding '" + banned + "')");
            }
            WalkAll(Banned);
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        /// <summary>The guard on the JSON: an entry without a source is a placeholder, and all its text is typed.</summary>
        [Test]
        public void PlaceholderGuard_ContentJson()
        {
            var problems = new List<string>();
            void Check(string field, string text, string at, bool placeholder)
            {
                if (DataFields.Contains(field) || string.IsNullOrEmpty(text) || !placeholder) return;
                if (!Typed.IsMatch(text)) problems.Add(at + " = '" + text + "' (want '[PLACEHOLDER: what goes here]')");
            }
            WalkAll(Check, problems);
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        delegate void Visit(string field, string text, string at, bool placeholder);

        static void WalkAll(Visit visit, List<string> problems = null)
        {
            Walk(PortfolioContent.Load<CatalogFile>(Files[0]), "projects", false, visit, problems);
            Walk(PortfolioContent.Load<HonoursFile>(Files[1]), "honours", false, visit, problems);
            Walk(PortfolioContent.Load<WorkshopFile>(Files[2]), "workshop", false, visit, problems);
            Walk(PortfolioContent.Load<StudyFile>(Files[3]), "study", false, visit, problems);
        }

        static void Walk(object node, string path, bool placeholder, Visit visit, List<string> problems)
        {
            if (node == null) return;
            System.Type type = node.GetType();
            FieldInfo flag = type.GetField("placeholder");
            if (flag != null)
            {
                placeholder = (bool)flag.GetValue(node);
                string source = type.GetField("source")?.GetValue(node) as string;
                if (!placeholder && string.IsNullOrEmpty(source)) problems?.Add(path + ": not a placeholder, but has no source");
            }
            foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object value = f.GetValue(node);
                string at = path + "." + f.Name;
                if (value is string s) visit(f.Name, s, at, placeholder);
                else if (value is IList list)
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is string item) visit(f.Name, item, at + "[" + i + "]", placeholder);
                        else Walk(list[i], at + "[" + i + "]", placeholder, visit, problems);
                    }
                else if (value != null && !f.FieldType.IsPrimitive && !f.FieldType.IsEnum) Walk(value, at, placeholder, visit, problems);
            }
        }

        /// <summary>The guard in the world: titles, intros, labels and hints of every pf.* zone.</summary>
        [UnityTest]
        public IEnumerator PlaceholderGuard_EveryManorStringIsSourced()
        {
            yield return null;
            var problems = new List<string>();
            int zones = 0, strings = 0;
            foreach (RoomContext ctx in Game.Rooms)
            {
                if (!ctx.Room.Key.StartsWith("pf.")) continue;
                zones++;
                var texts = new List<string> { ctx.Room.Title, ctx.Room.Intro };
                texts.AddRange(ManorCopy.VisibleText(ctx.WorldRoot));
                if (ctx.HasDiorama) texts.AddRange(ManorCopy.VisibleText(ctx.DioramaRoot));
                foreach (string s in texts)
                {
                    strings++;
                    if (!ManorCopy.IsSourced(s, PortfolioCatalog.Owner)) problems.Add(ctx.Room.Key + ": '" + s + "'");
                }
            }
            Debug.Log("[IonTest] placeholder guard: " + strings + " strings in " + zones + " zones");
            Assert.AreEqual(7, zones, "the seven Manor zones");
            Assert.IsEmpty(problems, "unsourced visible text:\n" + string.Join("\n", problems));
        }

        [Test]
        public void PlaceholderGuard_RejectsInventedText()
        {
            Assert.IsTrue(ManorCopy.IsSourced("[01]  [PLACEHOLDER: PROJECT 1 TITLE]"));
            Assert.IsTrue(ManorCopy.IsSourced("[PLACEHOLDER: title] - [PLACEHOLDER: role], [PLACEHOLDER: year]. [PLACEHOLDER: blurb]"));
            Assert.IsTrue(ManorCopy.IsSourced("[ GRAND GALLERY ]"));
            Assert.IsTrue(ManorCopy.IsSourced("[derprito64bit]", "derprito64bit"));
            Assert.IsFalse(ManorCopy.IsSourced("[01]  ROBOTICS CHAMPION"));
            Assert.IsFalse(ManorCopy.IsSourced("[PLACEHOLDER: title] - lead programmer, 2025"));
            Assert.IsFalse(ManorCopy.IsSourced("First place, 2025"));
            Assert.IsFalse(ManorCopy.IsSourced("[someone else]", "derprito64bit"));
        }

        /// <summary>
        /// Crews extend the wayfinding from their own folders, and the plan's own lines pass without any edit here.
        /// Invented text still fails, next to allowed copy or alone.
        /// </summary>
        [Test]
        public void PlaceholderGuard_CrewsExtendTheWayfinding()
        {
            const string owner = "derprito64bit";

            // Declared outside Content/ with [ManorWayfinding], or allowed at run time by the code that shows it.
            Assert.IsTrue(ManorCopy.IsSourced(DeclaredLabel), "a declared const");
            Assert.IsTrue(ManorCopy.IsSourced("m-f declared probe"), "any case, without the label brackets");
            Assert.IsTrue(ManorCopy.IsSourced(DeclaredLines[0]), "a declared string[]");
            string composed = "M-F runtime probe " + System.Guid.NewGuid().ToString("N");
            Assert.IsFalse(ManorCopy.IsSourced("[ " + composed.ToUpperInvariant() + " ]"), "not allowed yet");
            ManorCopy.Allow(composed);
            Assert.IsTrue(ManorCopy.IsSourced("[ " + composed.ToUpperInvariant() + " ]"), "allowed at run time");

            // The plan's door labels, frames and lines (M-D002, M-D005, M-D027), and every pf.* room title as a label.
            foreach (string line in new[]
                     {
                         "[ HALL OF HONOURS ]", "[ WORKSHOP ]", "[ CAMERA ROOM ]", "[ STUDY ]", "[ PAINTING WING ]",
                         "[ GRAND GALLERY ]", "[ FOYER ]", "Plan of the house", "[ PLAN OF THE HOUSE ]",
                         "Read everything as a page", "HALL OF\nHONOURS",
                     })
                Assert.IsTrue(ManorCopy.IsSourced(line, owner), "'" + line + "'");
            foreach (var zone in PortfolioRegistrar.Zones)
            {
                string title = PortfolioRegistrar.Create(zone.Key).Title;
                Assert.IsTrue(ManorCopy.IsSourced("[ " + title.ToUpperInvariant() + " ]", owner), zone.Key + " door label");
            }

            // The owner's handle on a frieze (M-D018): any case, with or without brackets, beside the [ ] monogram.
            foreach (string frieze in new[] { "DERPRITO64BIT", "derprito64bit", "[derprito64bit]", "[ DERPRITO64BIT ]", "[ ]  DERPRITO64BIT" })
                Assert.IsTrue(ManorCopy.IsSourced(frieze, owner), "'" + frieze + "'");

            // Invented text still fails.
            foreach (string invented in new[]
                     {
                         "First place, 2025", "[ FIRST PLACE ]", "Hall of Honours: first place", "[ HALL OF FAME ]",
                         "DERPRITO64BIT, world champion", "[PLACEHOLDER: title] - lead programmer", "[01]  ROBOTICS CHAMPION",
                         "[ STUDY ] since 2019", "[ ] Jane Doe",
                     })
                Assert.IsFalse(ManorCopy.IsSourced(invented, owner), "'" + invented + "' must fail");
        }

        /// <summary>Once owner-facts.md supplies an entry, its own text passes, alone or composed on a plaque or caption.</summary>
        [Test]
        public void PlaceholderGuard_SourcedContentPasses()
        {
            var sourced = new List<string> { "Robotics club captain", "2025", "Built the drive base" };
            Assert.IsTrue(ManorCopy.IsSourced("Robotics club captain", null, sourced));
            Assert.IsTrue(ManorCopy.IsSourced("[03]  ROBOTICS CLUB CAPTAIN", null, sourced), "a plaque in capitals");
            Assert.IsTrue(ManorCopy.IsSourced("Robotics club captain - 2025. Built the drive base", null, sourced), "a caption");
            Assert.IsTrue(ManorCopy.IsSourced("[ WORKSHOP ]  Robotics club captain", null, sourced), "beside a door label");
            Assert.IsFalse(ManorCopy.IsSourced("Robotics club captain - world champion", null, sourced));
            Assert.IsFalse(ManorCopy.IsSourced("Robotics club captain, 2026", null, sourced));
            Assert.IsFalse(ManorCopy.IsSourced("Robotics club captain", null, new List<string>()), "not sourced, not shown");
            foreach (string s in ManorCopy.SourcedText)
                Assert.IsFalse(ManorCopy.IsPlaceholder(s), "only sourced entries feed the list: '" + s + "'");
        }
    }
}

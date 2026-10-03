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
    /// zones is sourced or a typed "[PLACEHOLDER: ...]", and no invented fact survives from the first catalogue.
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

        [Test]
        public void Content_V2FilesParseWithTheirSlots()
        {
            IReadOnlyList<ProjectEntry> projects = PortfolioCatalog.Projects;
            Assert.AreEqual(8, projects.Count, "eight projects, one per Gallery frame");
            foreach (ProjectEntry p in projects)
            {
                Assert.IsNotNull(p.links, p.slug + " links");
                Assert.IsNotNull(p.media, p.slug + " media");
                Assert.IsNotNull(p.tags, p.slug + " tags");
            }
            IReadOnlyList<HonourEntry> honours = PortfolioContent.Honours;
            Assert.AreEqual(1, Count(honours, "glass"), "one glass hero");
            Assert.AreEqual(2, Count(honours, "award"), "one plinth pair at launch");
            Assert.AreEqual(3, Count(honours, "medal"), "three medals on the gantry");
            WorkshopFile workshop = PortfolioContent.Workshop;
            Assert.IsFalse(string.IsNullOrEmpty(workshop.robot.id), "the robot hero spec");
            Assert.AreEqual(2, workshop.cad.Count, "two CAD bays at launch");
            Assert.AreEqual(1, workshop.bench.Count, "the 'on the bench' slot");
            StudyFile study = PortfolioContent.Study;
            Assert.AreEqual(3, study.chapters.Count, "three chapters at launch");
            Assert.IsTrue(study.contact.placeholder && study.portrait.placeholder);
            Assert.LessOrEqual(workshop.cad.Count, PortfolioContent.MaxCad);
            Assert.LessOrEqual(study.chapters.Count, PortfolioContent.MaxChapters);
        }

        static int Count(IReadOnlyList<HonourEntry> list, string kind)
        {
            int n = 0;
            foreach (HonourEntry h in list) if (h.kind == kind) n++;
            return n;
        }

        [Test]
        public void Content_NoInventedTextFromTheFirstCatalogue()
        {
            foreach (string path in Files)
            {
                var asset = Resources.Load<TextAsset>(path);
                Assert.IsNotNull(asset, path);
                foreach (string banned in new[] { "Project One", "Your role", "2026" })
                    StringAssert.DoesNotContain(banned, asset.text, path);
            }
        }

        /// <summary>The guard on the JSON: an entry without a source is a placeholder, and all its text is typed.</summary>
        [Test]
        public void PlaceholderGuard_ContentJson()
        {
            var problems = new List<string>();
            Walk(PortfolioContent.Load<CatalogFile>(Files[0]), "projects", false, problems);
            Walk(PortfolioContent.Load<HonoursFile>(Files[1]), "honours", false, problems);
            Walk(PortfolioContent.Load<WorkshopFile>(Files[2]), "workshop", false, problems);
            Walk(PortfolioContent.Load<StudyFile>(Files[3]), "study", false, problems);
            Assert.IsEmpty(problems, string.Join("\n", problems));
        }

        static void Walk(object node, string path, bool placeholder, List<string> problems)
        {
            if (node == null) return;
            System.Type type = node.GetType();
            FieldInfo flag = type.GetField("placeholder");
            if (flag != null)
            {
                placeholder = (bool)flag.GetValue(node);
                string source = type.GetField("source")?.GetValue(node) as string;
                if (!placeholder && string.IsNullOrEmpty(source)) problems.Add(path + ": not a placeholder, but has no source");
            }
            foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object value = f.GetValue(node);
                string at = path + "." + f.Name;
                if (value is string s) Check(f.Name, s, at, placeholder, problems);
                else if (value is IList list)
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] is string item) Check(f.Name, item, at + "[" + i + "]", placeholder, problems);
                        else Walk(list[i], at + "[" + i + "]", placeholder, problems);
                    }
                else if (value != null && !f.FieldType.IsPrimitive && !f.FieldType.IsEnum) Walk(value, at, placeholder, problems);
            }
        }

        static void Check(string field, string text, string at, bool placeholder, List<string> problems)
        {
            if (DataFields.Contains(field) || string.IsNullOrEmpty(text) || !placeholder) return;
            if (!Typed.IsMatch(text)) problems.Add(at + " = '" + text + "' (want '[PLACEHOLDER: what goes here]')");
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
    }
}

using System.Collections;
using System.Collections.Generic;
using Ion.Levels;
using Ion.Portfolio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ion.Tests.PlayMode
{
    /// <summary>
    /// M-F: the seven Manor zones sit at their registry slots with their plan titles (M-D003), every URL alias opens
    /// its room, and the closed rooms stay within ±9 m of their root in X. What each room holds is its owner's to test
    /// (RoomsSmokeTests for M-C2, WorkshopStudySmokeTests for M-C7).
    /// </summary>
    public sealed class RegistryTests : IonPlayTestBase
    {
        const string Foyer = PortfolioRegistrar.FoyerKey, Gallery = PortfolioRegistrar.GalleryKey;

        /// <summary>The rooms behind doors (M-D003: "Closed rooms stay within ±9 m of their root in X").</summary>
        static readonly string[] ClosedRooms =
        {
            PortfolioRegistrar.HonoursKey, PortfolioRegistrar.LensKey, PortfolioRegistrar.WingKey,
            PortfolioRegistrar.WorkshopKey, PortfolioRegistrar.StudyKey,
        };

        [UnityTest]
        public IEnumerator Registry_SevenZonesAtTheirSlots()
        {
            yield return null;
            string[] keys =
            {
                Foyer, Gallery, PortfolioRegistrar.HonoursKey, PortfolioRegistrar.LensKey, PortfolioRegistrar.WingKey,
                PortfolioRegistrar.WorkshopKey, PortfolioRegistrar.StudyKey,
            };
            Assert.AreEqual(keys.Length, PortfolioRegistrar.Zones.Length);
            var registered = new List<string>();
            foreach (string k in ZoneCatalog.Keys)
                if (k.StartsWith("pf.")) registered.Add(k);
            CollectionAssert.AreEqual(keys, registered, "the catalogue builds the Manor zones in plan order");
            for (int i = 0; i < keys.Length; i++)
            {
                Assert.AreEqual(keys[i], PortfolioRegistrar.Zones[i].Key);
                Assert.AreEqual(1000 + 10 * i, PortfolioRegistrar.Zones[i].Order, keys[i] + " order");
                int index = Game.IndexOfKey(keys[i]);
                Assert.AreEqual(6 + i, index, keys[i] + " index");
                Assert.AreEqual(300f + 50f * i, Game.Rooms[index].WorldRoot.position.x, 1e-3f, keys[i] + " x");
            }
            int starry = Game.IndexOfKey(StarryNightKeys.Key);
            if (starry >= 0) Assert.GreaterOrEqual(starry, 13, "Painting Worlds follow the Manor");
            Assert.GreaterOrEqual(StarryNightKeys.Order, 1100);
            Assert.AreEqual(6, Game.Rooms.Count - ZoneCatalog.Count, "six core zones, then the extensions");
        }

        [Test]
        public void Registry_EveryKeyCreatesItsRoom()
        {
            var titles = new Dictionary<string, string>
            {
                { PortfolioRegistrar.LensKey, "Camera Room" },
                { PortfolioRegistrar.WorkshopKey, "Workshop" },
                { PortfolioRegistrar.StudyKey, "Study" },
            };
            foreach (var zone in PortfolioRegistrar.Zones)
            {
                Room room = PortfolioRegistrar.Create(zone.Key);
                Assert.IsNotNull(room, zone.Key);
                Assert.AreEqual(zone.Key, room.Key, "the room answers to its registry key");
                Assert.IsFalse(string.IsNullOrEmpty(room.Title), zone.Key + " has a title");
                Assert.AreNotSame(room, PortfolioRegistrar.Create(zone.Key), zone.Key + ": a new room per build");
                if (titles.TryGetValue(zone.Key, out string title)) Assert.AreEqual(title, room.Title, zone.Key + " title (M-D003)");
            }
            Assert.IsNull(PortfolioRegistrar.Create("camera"), "core keys are not the Manor's");
            Assert.IsNull(PortfolioRegistrar.Create("pf.unknown"));
        }

        [Test]
        public void StartKey_AliasesOpenEveryRoom()
        {
            // M-D003's aliases. The brief's "13" counts these 12; 'honors' is not in the plan.
            var aliases = new (string Alias, string Key)[]
            {
                ("honours", PortfolioRegistrar.HonoursKey), ("hall", PortfolioRegistrar.HonoursKey),
                ("lens", PortfolioRegistrar.LensKey), ("lenses", PortfolioRegistrar.LensKey), ("camera-room", PortfolioRegistrar.LensKey),
                ("wing", PortfolioRegistrar.WingKey),
                ("workshop", PortfolioRegistrar.WorkshopKey), ("robot", PortfolioRegistrar.WorkshopKey), ("cad", PortfolioRegistrar.WorkshopKey),
                ("study", PortfolioRegistrar.StudyKey), ("about", PortfolioRegistrar.StudyKey), ("contact", PortfolioRegistrar.StudyKey),
            };
            Assert.AreEqual(12, aliases.Length, "every M-D003 alias");
            foreach ((string alias, string key) in aliases)
            {
                Assert.AreEqual(key, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=" + alias), alias);
                Assert.AreEqual(key, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?debug=1&zone=" + alias.ToUpperInvariant() + "#top"),
                                alias + " in capitals, after another parameter");
            }
            Assert.AreEqual("camera", PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=camera"), "?zone=camera stays the core camera wing");
            Assert.AreEqual(Gallery, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=gallery"));
            Assert.AreEqual("t1", PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=game"));
            Assert.AreEqual(Foyer, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/"));
            Assert.AreEqual(Foyer, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=manor"));
            Assert.AreEqual(PortfolioRegistrar.StudyKey, PortfolioRegistrar.StartKeyFromUrl("https://x.io/manor/?zone=pf.study"));
        }

        /// <summary>Every renderer and collider of a closed room, wherever it is built, stays within ±9 m of the room's root in X.</summary>
        [UnityTest]
        public IEnumerator ClosedRooms_StayWithin9mOfTheirRoot()
        {
            yield return null;
            var problems = new List<string>();
            foreach (string key in ClosedRooms)
            {
                yield return GoTo(key);
                Transform root = Room(key).WorldRoot;
                float lo = float.MaxValue, hi = float.MinValue;
                var bounds = new List<(string Name, Bounds B)>();
                foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) bounds.Add((r.name, r.bounds));
                foreach (Collider c in root.GetComponentsInChildren<Collider>(true)) bounds.Add((c.name, c.bounds));
                foreach ((string name, Bounds b) in bounds)
                {
                    float min = b.min.x - root.position.x, max = b.max.x - root.position.x;
                    lo = Mathf.Min(lo, min);
                    hi = Mathf.Max(hi, max);
                    if (min < -9f || max > 9f) problems.Add(key + ": '" + name + "' spans x " + min.ToString("0.00") + ".." + max.ToString("0.00"));
                }
                Assert.Greater(bounds.Count, 0, key + " built nothing");
                Debug.Log("[IonTest] " + key + ": x extent " + lo.ToString("0.000") + ".." + hi.ToString("0.000") + " m (limit ±9)");
            }
            Assert.IsEmpty(problems, "outside ±9 m of the root (M-D003):\n" + string.Join("\n", problems));
        }
    }
}

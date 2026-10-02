using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ion.Levels
{
    /// <summary>
    /// Zone registration seam. The six core zones (T1, T2, hub, stairs, camera, gallery) are always built first, at
    /// indices 0-5, so every index assumption in the game and its tests still holds. Other code (a fork's content, a
    /// test) appends zones by registering a factory before <see cref="GameBootstrap"/> wakes, typically from a
    /// <c>[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]</c> method. Extension zones
    /// follow the core zones on the X line in ascending <c>order</c> (ties by key).
    ///
    /// <see cref="StartKey"/> picks the zone the player spawns in and returns to on "Play again"; null keeps T1.
    ///
    /// Registration never throws (the Web player runs with exceptions disabled): invalid entries are logged and
    /// ignored. Statics are reset at subsystem registration, so a play session starts empty.
    /// </summary>
    public static class ZoneCatalog
    {
        /// <summary>Extension orders start here; lower values are reserved for the core zones.</summary>
        public const int ExtensionOrderMin = 1000;

        struct Entry
        {
            public string Key;
            public int Order;
            public Func<Room> Factory;
        }

        static readonly List<Entry> s_entries = new List<Entry>();

        /// <summary>
        /// Key of the zone the player starts in and returns to on "Play again" (null or unknown: the first zone).
        /// </summary>
        public static string StartKey { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            s_entries.Clear();
            StartKey = null;
        }

        /// <summary>Number of registered extension zones.</summary>
        public static int Count => s_entries.Count;

        /// <summary>Registered extension keys, in build order.</summary>
        public static IReadOnlyList<string> Keys
        {
            get
            {
                Sort();
                var keys = new List<string>(s_entries.Count);
                foreach (Entry e in s_entries) keys.Add(e.Key);
                return keys;
            }
        }

        /// <summary>
        /// Registers an extension zone. <paramref name="factory"/> must return a new room whose <see cref="Room.Key"/>
        /// equals <paramref name="key"/>. Re-registering a key replaces the earlier entry. Returns false (and logs) when
        /// the entry is invalid.
        /// </summary>
        public static bool Register(string key, int order, Func<Room> factory)
        {
            if (string.IsNullOrEmpty(key) || factory == null)
            {
                Debug.LogError("[ZoneCatalog] Register needs a key and a factory.");
                return false;
            }
            if (order < ExtensionOrderMin)
            {
                Debug.LogError("[ZoneCatalog] '" + key + "': extension orders start at " + ExtensionOrderMin + ".");
                return false;
            }
            Unregister(key);
            s_entries.Add(new Entry { Key = key, Order = order, Factory = factory });
            return true;
        }

        /// <summary>Removes a registered extension zone. Returns true when it was registered.</summary>
        public static bool Unregister(string key)
        {
            for (int i = 0; i < s_entries.Count; i++)
            {
                if (string.Equals(s_entries[i].Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    s_entries.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the registered rooms in build order. A factory that fails, returns null or returns a room with a
        /// different key (or a key a core zone already uses) is skipped with an error.
        /// </summary>
        internal static List<Room> CreateExtensions(IList<Room> coreRooms)
        {
            Sort();
            var rooms = new List<Room>(s_entries.Count);
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (coreRooms != null)
                foreach (Room r in coreRooms) used.Add(r.Key);
            foreach (Entry e in s_entries)
            {
                Room room = null;
                try { room = e.Factory(); }
                catch (Exception ex) { Debug.LogException(ex); }
                if (room == null)
                {
                    Debug.LogError("[ZoneCatalog] '" + e.Key + "': the factory returned no room.");
                    continue;
                }
                if (!string.Equals(room.Key, e.Key, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError("[ZoneCatalog] '" + e.Key + "': the room's key is '" + room.Key + "'.");
                    continue;
                }
                if (!used.Add(room.Key))
                {
                    Debug.LogError("[ZoneCatalog] '" + e.Key + "': a zone with this key already exists.");
                    continue;
                }
                rooms.Add(room);
            }
            return rooms;
        }

        static void Sort()
        {
            s_entries.Sort((a, b) =>
            {
                int c = a.Order.CompareTo(b.Order);
                return c != 0 ? c : string.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase);
            });
        }
    }
}

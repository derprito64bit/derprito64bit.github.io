using System;
using System.Collections.Generic;
using Ion.Gameplay;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using UnityEngine;
using B = Ion.Levels.Props.PropBuild;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Manor's padless door (M-D024): a Niche opening in a <see cref="ManorHall"/> wall, a lettered lintel and the
    /// public <see cref="Teleporter"/> component on a fork GameObject at the threshold. Its trigger is 1.6 x 2.4 x 1.6 m,
    /// centred 0.8 m in front of the wall face. There is no PropKit teleporter pad and no pad light. The stub rooms use
    /// it now; the Foyer and the Gallery keep their pods until M-C2 swaps them. The casing, drapes and Graphite lining
    /// are M-C2's.
    /// </summary>
    public static class ManorDoor
    {
        /// <summary>Niche size: the plan's 2.4 x 4.2 m rounded onto the 0.25 plan and 0.0625 height grids.</summary>
        public const float Width = 2.5f, OpeningHeight = 4.25f;

        /// <summary>Niche depth: the deepest recess a 0.5 m ManorHall wall keeps on the grid.</summary>
        public const float Depth = 0.25f;

        /// <summary>How far in front of the wall face the trigger is centred.</summary>
        public const float TriggerInset = 0.8f;

        public static readonly Vector3 TriggerSize = new Vector3(1.6f, 2.4f, 1.6f);

        /// <summary>The lintel of every way back to the Grand Gallery (M-D002).</summary>
        public const string GalleryLabel = "[ GRAND GALLERY ]";

        /// <summary>The name of the GameObject that carries a door's teleporter.</summary>
        public static string NameFor(string label) => "ManorDoor " + label;

        /// <summary>
        /// Every padless door under <paramref name="root"/> lettered <paramref name="label"/>. Tests and the door graph
        /// find a door by its label, never as "the only teleporter" or "the last solution".
        /// </summary>
        public static List<Teleporter> Find(Transform root, string label)
        {
            var doors = new List<Teleporter>();
            if (root == null) return doors;
            string name = NameFor(label);
            foreach (Teleporter t in root.GetComponentsInChildren<Teleporter>(true))
                if (t.gameObject.name == name) doors.Add(t);
            return doors;
        }

        /// <summary>The door's Niche opening, for <see cref="ManorHall.Build"/>.</summary>
        public static WallOpening Opening(Dir facing, float along, float height = OpeningHeight) =>
            new WallOpening(facing, along, Ion.Levels.Arch.Opening.Niche(0f, Width, height, 0f, Depth).NoBracket());

        /// <summary>
        /// Builds the lintel lettering and the teleporter for a door whose niche centre sits on the wall face at
        /// <paramref name="face"/> (floor level, room-local) and whose front looks along <paramref name="facing"/>.
        /// Works with <c>ctx.Game == null</c>. Returns the threshold: the room-local feet point under the trigger
        /// centre, where a Teleport solution walks to.
        /// </summary>
        public static Vector3 Build(Transform p, RoomContext ctx, Vector3 face, Dir facing, string label, Action onEnter,
                                    float height = OpeningHeight)
        {
            Vector3 inward = B.Vec(facing);
            PropKit.Plaque(p, face + new Vector3(0f, height + 0.375f, 0f), facing, label);

            Vector3 threshold = face + inward * TriggerInset;
            var go = new GameObject(NameFor(label));
            go.transform.SetParent(p, false);
            go.transform.localPosition = threshold;
            go.transform.localRotation = Quaternion.Euler(0f, B.Yaw(facing), 0f);
            var trigger = go.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = TriggerSize;
            trigger.center = new Vector3(0f, TriggerSize.y * 0.5f, 0f);
            var teleporter = go.AddComponent<Teleporter>();
            teleporter.TriggerSize = TriggerSize;
            teleporter.OnEnter = onEnter;
            return threshold;
        }
    }
}

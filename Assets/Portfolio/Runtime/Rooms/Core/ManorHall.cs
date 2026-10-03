using System;
using System.Collections.Generic;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using UnityEngine;
using B = Ion.Levels.Props.PropBuild;

namespace Ion.Portfolio
{
    using Arch = Ion.Levels.Arch.Arch;

    /// <summary>
    /// An opening in one of a <see cref="ManorHall"/>'s four walls. The wall is named by the way its face looks into
    /// the room: <see cref="Dir.PosZ"/> the near (z0) wall, <see cref="Dir.NegZ"/> the far (z1) wall,
    /// <see cref="Dir.PosX"/> the left (-x) wall and <see cref="Dir.NegX"/> the right (+x) wall.
    /// </summary>
    public readonly struct WallOpening
    {
        public readonly Dir Facing;
        /// <summary>Centre of the opening: room-local x on the near and far walls, z on the side walls.</summary>
        public readonly float Along;
        /// <summary>The opening's shape (its <c>At</c> is ignored; <see cref="Along"/> places it).</summary>
        public readonly Opening Shape;

        public WallOpening(Dir facing, float along, Opening shape)
        {
            Facing = facing;
            Along = along;
            Shape = shape;
        }
    }

    /// <summary>Shared shell of the Manor's rooms: a floating marble floor, panelled walls, a coffered ceiling.</summary>
    public static class ManorHall
    {
        public const float Height = 6f;

        /// <summary>
        /// A closed hall with inner faces at x = ±<paramref name="halfX"/> and z = <paramref name="z0"/>..<paramref name="z1"/>
        /// (all on the 0.25 grid): floor, four walls with walnut wainscoting, a roof and walnut coffer beams every 4 m.
        /// <paramref name="openings"/> cut any of the four walls (the wainscot stops at them).
        /// </summary>
        public static void Build(Transform p, float halfX, float z0, float z1, float height = Height, params WallOpening[] openings)
        {
            Arch.Terrace(p, new RectXZ(-halfX - 0.5f, z0 - 0.5f, halfX + 0.5f, z1 + 0.5f), 0f, 3f, Arch.Underside.Stepped);
            float wx = halfX + 0.25f;
            HallWall(p, new Vector3(-wx, 0f, z0 - 0.5f), new Vector3(-wx, 0f, z1 + 0.5f), Dir.PosX, height, openings);
            HallWall(p, new Vector3(wx, 0f, z0 - 0.5f), new Vector3(wx, 0f, z1 + 0.5f), Dir.NegX, height, openings);
            HallWall(p, new Vector3(-halfX, 0f, z0 - 0.25f), new Vector3(halfX, 0f, z0 - 0.25f), Dir.PosZ, height, openings);
            HallWall(p, new Vector3(-halfX, 0f, z1 + 0.25f), new Vector3(halfX, 0f, z1 + 0.25f), Dir.NegZ, height, openings);
            Arch.Roof(p, new RectXZ(-halfX - 0.5f, z0 - 0.5f, halfX + 0.5f, z1 + 0.5f), height + 0.5f);

            var wood = new Surf(Mat.Walnut, Pat.Boards);
            for (float z = z0 + 4f; z < z1; z += 4f)
                Arch.Box(p, new Vector3(-halfX, height - 0.375f, z - 0.25f), new Vector3(halfX, height, z + 0.25f), wood, ArchFlags.Soft);
            float side = Mathf.Floor(halfX * 0.5f / 0.25f) * 0.25f;
            foreach (float x in new[] { -side, side })
                Arch.Box(p, new Vector3(x - 0.125f, height - 0.25f, z0), new Vector3(x + 0.125f, height, z1), wood, ArchFlags.Soft);

            Wainscot(p, new Vector3(-halfX, 0f, z0), new Vector3(-halfX, 0f, z1), Dir.PosX, openings);
            Wainscot(p, new Vector3(halfX, 0f, z0), new Vector3(halfX, 0f, z1), Dir.NegX, openings);
            Wainscot(p, new Vector3(-halfX, 0f, z0), new Vector3(halfX, 0f, z0), Dir.PosZ, openings);
            Wainscot(p, new Vector3(-halfX, 0f, z1), new Vector3(halfX, 0f, z1), Dir.NegZ, openings);
        }

        /// <summary>The point on the inner wall face (floor level) at the centre of <paramref name="o"/>.</summary>
        public static Vector3 Face(float halfX, float z0, float z1, WallOpening o)
        {
            switch (o.Facing)
            {
                case Dir.PosZ: return new Vector3(o.Along, 0f, z0);
                case Dir.NegZ: return new Vector3(o.Along, 0f, z1);
                case Dir.PosX: return new Vector3(-halfX, 0f, o.Along);
                default: return new Vector3(halfX, 0f, o.Along);
            }
        }

        // A wall without openings is built exactly as before. A wall with openings runs so that its front face (the
        // right-hand side from 'from' to 'to', where Arch.Wall recesses niches) looks into the room.
        static void HallWall(Transform p, Vector3 from, Vector3 to, Dir facing, float height, WallOpening[] openings)
        {
            var mine = new List<WallOpening>();
            if (openings != null)
                foreach (WallOpening o in openings)
                    if (o.Facing == facing) mine.Add(o);
            if (mine.Count == 0)
            {
                Arch.Wall(p, from, to, height, Arch.WallThickness, WallTrim.Default);
                return;
            }
            if (Vector3.Dot(Vector3.Cross(Vector3.up, to - from), B.Vec(facing)) < 0f) (from, to) = (to, from);
            bool alongX = facing == Dir.PosZ || facing == Dir.NegZ;
            var shapes = new Opening[mine.Count];
            for (int i = 0; i < mine.Count; i++)
            {
                Opening s = mine[i].Shape;
                s.At = Mathf.Abs(mine[i].Along - (alongX ? from.x : from.z));
                shapes[i] = s;
            }
            Arch.Wall(p, from, to, height, Arch.WallThickness, WallTrim.Default, shapes);
        }

        // Walnut wainscot along one inner face, broken at every opening in that wall.
        static void Wainscot(Transform p, Vector3 from, Vector3 to, Dir facing, WallOpening[] openings)
        {
            bool alongX = facing == Dir.PosZ || facing == Dir.NegZ;
            float a = alongX ? from.x : from.z, b = alongX ? to.x : to.z;
            var cuts = new List<Vector2>();
            if (openings != null)
                foreach (WallOpening o in openings)
                    if (o.Facing == facing) cuts.Add(new Vector2(o.Along - o.Shape.Width * 0.5f, o.Along + o.Shape.Width * 0.5f));
            if (cuts.Count == 0)
            {
                ManorKit.Wainscot(p, from, to, facing);
                return;
            }
            cuts.Sort((u, v) => u.x.CompareTo(v.x));
            float cur = a;
            foreach (Vector2 c in cuts)
            {
                if (c.x - cur > 0.1f) ManorKit.Wainscot(p, At(from, alongX, cur), At(from, alongX, c.x), facing);
                cur = Mathf.Max(cur, c.y);
            }
            if (b - cur > 0.1f) ManorKit.Wainscot(p, At(from, alongX, cur), At(from, alongX, b), facing);
        }

        static Vector3 At(Vector3 on, bool alongX, float u) => alongX ? new Vector3(u, on.y, on.z) : new Vector3(on.x, on.y, u);

        /// <summary>
        /// A faceted apse closing the far end of a hall: <paramref name="facets"/> straight wall slabs on the half-ellipse
        /// from (-<paramref name="halfX"/>, <paramref name="zStart"/>) through (0, <paramref name="zEnd"/>) to
        /// (<paramref name="halfX"/>, <paramref name="zStart"/>). The corners behind it are closed off.
        /// </summary>
        public static GameObject Apse(Transform p, float halfX, float zStart, float zEnd, float height, int facets = 3)
        {
            Transform g = B.Frame(p, "Apse", Vector3.zero, 0f);
            int n = Mathf.Max(2, facets);
            const float t = Arch.WallThickness;
            for (int i = 0; i < n; i++)
            {
                Vector3 a = ApsePoint(halfX, zStart, zEnd, i / (float)n), b = ApsePoint(halfX, zStart, zEnd, (i + 1) / (float)n);
                Vector3 d = b - a;
                float half = d.magnitude * 0.5f + t * 0.5f;   // overlap the joints
                Transform slab = B.Frame(g, "Facet", (a + b) * 0.5f, Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg);
                B.Box(slab, new Vector3(-t * 0.5f, 0f, -half), new Vector3(t * 0.5f, height, half), Arch.Style.Wall, ArchFlags.Solid);
            }
            B.Exempt(g);   // facets are not on the plan grid
            return g.gameObject;
        }

        static Vector3 ApsePoint(float halfX, float zStart, float zEnd, float u)
        {
            float angle = Mathf.PI * (1f - u);
            return new Vector3(Mathf.Cos(angle) * halfX, 0f, zStart + Mathf.Sin(angle) * (zEnd - zStart));
        }

        /// <summary>A doorway on the far wall: a teleporter pad inside a walnut bracket frame, with a sign above it.</summary>
        public static void Door(Transform p, RoomContext ctx, Vector3 pad, float wallZ, string sign, Action onEnter, Mat tint) =>
            Door(p, ctx, pad, Dir.NegZ, wallZ, sign, onEnter, tint);

        /// <summary>
        /// A doorway on any wall: a teleporter pad facing <paramref name="facing"/> (into the room) inside a walnut
        /// bracket frame, with a sign on the wall face at <paramref name="wall"/> (its z for the near and far walls,
        /// its x for the side walls).
        /// </summary>
        public static void Door(Transform p, RoomContext ctx, Vector3 pad, Dir facing, float wall, string sign, Action onEnter, Mat tint)
        {
            Arch.BracketFrame(p, pad, facing, 2f, 3f, 0.5f);
            ctx.Teleporter(pad, facing, onEnter, tint);
            bool alongX = facing == Dir.PosZ || facing == Dir.NegZ;
            PropKit.Plaque(p, alongX ? new Vector3(pad.x, 3.75f, wall) : new Vector3(wall, 3.75f, pad.z), facing, sign);
        }

        /// <summary>Moves the player to the zone with <paramref name="key"/> (no-op if it is missing).</summary>
        public static Action GoTo(RoomContext ctx, string key)
        {
            GameBootstrap game = ctx.Game;
            return () =>
            {
                if (game == null) return;
                int i = game.IndexOfKey(key);
                if (i >= 0) game.GoToRoom(i);
            };
        }
    }
}

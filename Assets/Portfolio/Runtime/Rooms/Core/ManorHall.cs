using System;
using System.Collections.Generic;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    using Arch = Ion.Levels.Arch.Arch;

    /// <summary>Shared shell of the Manor's rooms: a floating marble floor, panelled walls, a coffered ceiling.</summary>
    static class ManorHall
    {
        public const float Height = 6f;

        /// <summary>
        /// A closed hall with inner faces at x = ±<paramref name="halfX"/> and z = <paramref name="z0"/>..<paramref name="z1"/>
        /// (all on the 0.25 grid): floor, four walls with walnut wainscoting, a roof and walnut coffer beams every 4 m.
        /// </summary>
        public static void Build(Transform p, float halfX, float z0, float z1)
        {
            Arch.Terrace(p, new RectXZ(-halfX - 0.5f, z0 - 0.5f, halfX + 0.5f, z1 + 0.5f), 0f, 3f, Arch.Underside.Stepped);
            float wx = halfX + 0.25f;
            Arch.Wall(p, new Vector3(-wx, 0f, z0 - 0.5f), new Vector3(-wx, 0f, z1 + 0.5f), Height, Arch.WallThickness, WallTrim.Default);
            Arch.Wall(p, new Vector3(wx, 0f, z0 - 0.5f), new Vector3(wx, 0f, z1 + 0.5f), Height, Arch.WallThickness, WallTrim.Default);
            Arch.Wall(p, new Vector3(-halfX, 0f, z0 - 0.25f), new Vector3(halfX, 0f, z0 - 0.25f), Height, Arch.WallThickness, WallTrim.Default);
            Arch.Wall(p, new Vector3(-halfX, 0f, z1 + 0.25f), new Vector3(halfX, 0f, z1 + 0.25f), Height, Arch.WallThickness, WallTrim.Default);
            Arch.Roof(p, new RectXZ(-halfX - 0.5f, z0 - 0.5f, halfX + 0.5f, z1 + 0.5f), Height + 0.5f);

            var wood = new Surf(Mat.Walnut, Pat.Boards);
            for (float z = z0 + 4f; z < z1; z += 4f)
                Arch.Box(p, new Vector3(-halfX, Height - 0.375f, z - 0.25f), new Vector3(halfX, Height, z + 0.25f), wood, ArchFlags.Soft);
            float side = Mathf.Floor(halfX * 0.5f / 0.25f) * 0.25f;
            foreach (float x in new[] { -side, side })
                Arch.Box(p, new Vector3(x - 0.125f, Height - 0.25f, z0), new Vector3(x + 0.125f, Height, z1), wood, ArchFlags.Soft);

            ManorKit.Wainscot(p, new Vector3(-halfX, 0f, z0), new Vector3(-halfX, 0f, z1), Dir.PosX);
            ManorKit.Wainscot(p, new Vector3(halfX, 0f, z0), new Vector3(halfX, 0f, z1), Dir.NegX);
            ManorKit.Wainscot(p, new Vector3(-halfX, 0f, z0), new Vector3(halfX, 0f, z0), Dir.PosZ);
            ManorKit.Wainscot(p, new Vector3(-halfX, 0f, z1), new Vector3(halfX, 0f, z1), Dir.NegZ);
        }

        /// <summary>A doorway: a teleporter pad inside a walnut bracket frame, with a sign on the wall behind it.</summary>
        public static void Door(Transform p, RoomContext ctx, Vector3 pad, float wallZ, string sign, Action onEnter, Mat tint)
        {
            Arch.BracketFrame(p, pad, Dir.NegZ, 2f, 3f, 0.5f);
            ctx.Teleporter(pad, Dir.NegZ, onEnter, tint);
            PropKit.Plaque(p, new Vector3(pad.x, 3.75f, wallZ), Dir.NegZ, sign);
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

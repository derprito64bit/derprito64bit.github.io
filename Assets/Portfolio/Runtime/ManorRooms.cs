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

    /// <summary>
    /// The Manor foyer, where every visit starts: a candlelit marble hall with a red carpet that splits towards two
    /// doors. Left: the Grand Gallery (every project, no puzzles). Right: the game ([project]ion's tutorial).
    /// </summary>
    public sealed class ManorFoyer : Room
    {
        public override string Key => PortfolioRegistrar.FoyerKey;
        public override string Title => "The Manor";
        public override string Intro => "Left: the Grand Gallery.  Right: the game.";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 9f, Z0 = -4f, Z1 = 20f;
        public static readonly Vector3 SpawnFeet = new Vector3(0f, 0f, -2f);
        public static readonly Vector3 GalleryPad = new Vector3(-4f, 0f, 18.5f);
        public static readonly Vector3 GamePad = new Vector3(4f, 0f, 18.5f);

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1);
            ctx.SetSpawn(SpawnFeet, 0f);

            // The red carpet: up the middle, then across to both doors.
            ManorKit.CarpetRunner(root, new RectXZ(-1.25f, -3.5f, 1.25f, 14f));
            ManorKit.CarpetRunner(root, new RectXZ(-5.25f, 14f, 5.25f, 16.25f));
            ManorKit.CarpetRunner(root, new RectXZ(-5.25f, 16.25f, -2.75f, 17.25f));
            ManorKit.CarpetRunner(root, new RectXZ(2.75f, 16.25f, 5.25f, 17.25f));

            ManorHall.Door(root, ctx, GalleryPad, Z1, "[ GRAND GALLERY ]", ManorHall.GoTo(ctx, PortfolioRegistrar.GalleryKey), Mat.TextileRed);
            ManorHall.Door(root, ctx, GamePad, Z1, "[ PLAY THE GAME ]", ManorHall.GoTo(ctx, "t1"), Mat.Graphite);
            PropKit.Plaque(root, new Vector3(-6.75f, 1.75f, Z1), Dir.NegZ, "every project, framed");
            PropKit.Plaque(root, new Vector3(6.75f, 1.75f, Z1), Dir.NegZ, "photo puzzles");

            // Columns, light and seating.
            for (float z = 0f; z <= 12f; z += 4f)
            {
                Arch.Column(root, new Vector3(-6.5f, 0f, z), ManorHall.Height);
                Arch.Column(root, new Vector3(6.5f, 0f, z), ManorHall.Height);
            }
            ManorKit.Chandelier(root, new Vector3(0f, ManorHall.Height, 6f), 1.75f, 12);
            ManorKit.Chandelier(root, new Vector3(0f, ManorHall.Height, 14f), 1.75f, 12);
            foreach (float x in new[] { -5.75f, -2.25f, 2.25f, 5.75f })
                ManorKit.Candelabra(root, new Vector3(x, 0f, 17.25f), 0f);
            foreach (float z in new[] { 0f, 8f, 16f })
            {
                ManorKit.CandleSconce(root, new Vector3(-HalfX, 2.5f, z), Dir.PosX);
                ManorKit.CandleSconce(root, new Vector3(HalfX, 2.5f, z), Dir.NegX);
            }
            ManorKit.CandleSconce(root, new Vector3(0f, 2.5f, Z1), Dir.NegZ);
            PropKit.Bench(root, new Vector3(-7.75f, 0f, 6f), Dir.PosX, 2f);
            PropKit.Bench(root, new Vector3(7.75f, 0f, 6f), Dir.NegX, 2f);
            PropKit.Planter(root, new Vector3(-7.75f, 0f, -3f), Dir.PosX, PlanterStyle.Bowl, default, 0.5f, 701);
            PropKit.Planter(root, new Vector3(7.75f, 0f, -3f), Dir.NegX, PlanterStyle.Bowl, default, 0.5f, 702);

            // A welcome on two low pedestals either side of the carpet.
            foreach (int s in new[] { -1, 1 })
            {
                var basePos = new Vector3(s * 2.5f, 0f, 1.5f);
                PropKit.Pedestal(root, basePos, Dir.NegZ, PedestalSize.Low);
                PropKit.Plaque(root, basePos + new Vector3(0f, PropKit.PedestalTop(PedestalSize.Low), -0.25f), Dir.NegZ,
                               s < 0 ? "[" + PortfolioCatalog.Owner + "]" : "portfolio");
            }
            ctx.Hint(new Vector3(0f, 0f, 0f), 3f,
                     "Welcome. Left door: every project in the Grand Gallery. Right door: play the game.", 6f);

            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 10f));
            AddSolution("gallery", RoomSolution.Kind.Teleport, GalleryPad).Destination = PortfolioRegistrar.GalleryKey;
        }
    }

    /// <summary>
    /// The Grand Gallery: a long candlelit hall with a red carpet and every project hung in a gilt frame (four a
    /// side). Standing in front of a painting shows its caption and summary. At the far end: the arcade alcove and
    /// doors back to the foyer and on to the game.
    /// </summary>
    public sealed class GrandGallery : Room
    {
        public override string Key => PortfolioRegistrar.GalleryKey;
        public override string Title => "Grand Gallery";
        public override string Intro => "Every project, framed.";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 7.5f, Z0 = -4f, Z1 = 44f;
        public static readonly float[] BayZ = { 4f, 12f, 20f, 28f };
        public static readonly Vector2 PaintingSize = new Vector2(2.4f, 1.8f);
        public const float PaintingY = 2.75f;
        public static readonly Vector3 SpawnFeet = new Vector3(0f, 0f, -2f);
        public static readonly Vector3 FoyerPad = new Vector3(-4.5f, 0f, 40f);
        public static readonly Vector3 GamePad = new Vector3(4.5f, 0f, 40f);
        public static readonly Vector3 ArcadeBase = new Vector3(0f, 0f, 42.75f);

        /// <summary>The paintings in catalogue order (left wall front to back, then the right wall).</summary>
        public readonly List<GameObject> Paintings = new List<GameObject>();

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1);
            ctx.SetSpawn(SpawnFeet, 0f);
            ManorKit.CarpetRunner(root, new RectXZ(-1.5f, -3.5f, 1.5f, 38f));

            IReadOnlyList<ProjectEntry> projects = PortfolioCatalog.Projects;
            int slot = 0;
            foreach (int side in new[] { -1, 1 })
            {
                Dir facing = side < 0 ? Dir.PosX : Dir.NegX;
                foreach (float z in BayZ)
                {
                    ProjectEntry p = slot < projects.Count ? projects[slot] : null;
                    Texture2D cover = PortfolioCatalog.CoverOf(p, slot);
                    Paintings.Add(ManorKit.Painting(root, new Vector3(side * HalfX, PaintingY, z), facing, PaintingSize, cover));
                    string number = (slot + 1).ToString("00");
                    string title = p != null && !string.IsNullOrEmpty(p.title) ? p.title.ToUpperInvariant() : "COMING SOON";
                    PropKit.Plaque(root, new Vector3(side * HalfX, 1.25f, z), facing, PropKit.PlaqueText(number, title));
                    if (p != null)
                        ctx.Hint(new Vector3(side * (HalfX - 2f), 0f, z), 2f, p.Caption + ". " + p.blurb, 7f);
                    slot++;
                }
            }

            // Candlelight: chandeliers down the nave, sconces between the paintings, candelabras by the walls.
            for (float z = 2f; z <= 34f; z += 8f)
                ManorKit.Chandelier(root, new Vector3(0f, ManorHall.Height, z), 1.75f, 12);
            for (float z = 0f; z <= 40f; z += 8f)
            {
                ManorKit.CandleSconce(root, new Vector3(-HalfX, 2.5f, z), Dir.PosX);
                ManorKit.CandleSconce(root, new Vector3(HalfX, 2.5f, z), Dir.NegX);
            }
            foreach (float z in new[] { 8f, 16f, 24f })
            {
                ManorKit.Candelabra(root, new Vector3(-6.25f, 0f, z), 90f);
                ManorKit.Candelabra(root, new Vector3(6.25f, 0f, z), -90f);
            }
            foreach (float z in new[] { 8f, 24f })
            {
                PropKit.Bench(root, new Vector3(-2.5f, 0f, z), Dir.NegX, 2f);
                PropKit.Bench(root, new Vector3(2.5f, 0f, z), Dir.PosX, 2f);
            }

            // The far end: the arcade alcove between the two doors.
            ManorKit.ArcadeCabinet(root, ArcadeBase, Dir.NegZ);
            PropKit.Plaque(root, new Vector3(0f, 3.75f, Z1), Dir.NegZ, "[ ARCADE ]");
            ctx.Hint(new Vector3(0f, 0f, 40.5f), 1.75f, "The arcade: playable project demos will run on this cabinet.", 5f);
            ManorHall.Door(root, ctx, FoyerPad, Z1, "[ FOYER ]", ManorHall.GoTo(ctx, PortfolioRegistrar.FoyerKey), Mat.TextileRed);
            ManorHall.Door(root, ctx, GamePad, Z1, "[ PLAY THE GAME ]", ManorHall.GoTo(ctx, "t1"), Mat.Graphite);

            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 16f));
            AddSolution("far", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 34f));
            AddSolution("foyer", RoomSolution.Kind.Teleport, FoyerPad).Destination = PortfolioRegistrar.FoyerKey;
        }
    }
}

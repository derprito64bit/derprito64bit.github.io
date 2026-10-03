using System.Collections.Generic;
using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
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

        /// <summary>The arcade cabinet's usable (E: play the demo).</summary>
        public ArcadeMachine Arcade { get; private set; }

        /// <summary>The demo played when no project has one (site path, see ProjectEntry.demo).</summary>
        public const string DefaultDemo = "arcade/demo/";

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

            // The far end: the arcade alcove between the two doors. The cabinet plays the first project demo.
            ManorKit.ArcadeCabinet(root, ArcadeBase, Dir.NegZ);
            PropKit.Plaque(root, new Vector3(0f, 3.75f, Z1), Dir.NegZ, "[ ARCADE ]");
            ProjectEntry demoProject = null;
            foreach (ProjectEntry p in projects)
                if (p != null && !string.IsNullOrEmpty(p.demo)) { demoProject = p; break; }
            var arcade = new GameObject("Arcade machine");
            arcade.transform.SetParent(root, false);
            arcade.transform.localPosition = ArcadeBase + new Vector3(0f, 1.28f, -0.3f);   // the screen
            Arcade = arcade.AddComponent<ArcadeMachine>();
            Arcade.UsePrompt = "play";
            Arcade.UseRange = 2.25f;
            Arcade.Url = "../" + (demoProject != null ? demoProject.demo : DefaultDemo);
            Arcade.Title = demoProject != null ? demoProject.title : "Royal Breaker";
            ctx.Hint(new Vector3(0f, 0f, 40.5f), 1.75f, "The arcade: press E at the cabinet to play.", 5f);
            ManorHall.Door(root, ctx, FoyerPad, Z1, "[ FOYER ]", ManorHall.GoTo(ctx, PortfolioRegistrar.FoyerKey), Mat.TextileRed);
            ManorHall.Door(root, ctx, GamePad, Z1, "[ PLAY THE GAME ]", ManorHall.GoTo(ctx, "t1"), Mat.Graphite);

            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 16f));
            AddSolution("far", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 34f));
            AddSolution("foyer", RoomSolution.Kind.Teleport, FoyerPad).Destination = PortfolioRegistrar.FoyerKey;
        }
    }
}

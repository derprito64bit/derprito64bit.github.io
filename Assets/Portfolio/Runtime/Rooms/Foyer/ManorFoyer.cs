using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Levels.Props;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    using Arch = Ion.Levels.Arch.Arch;

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
}

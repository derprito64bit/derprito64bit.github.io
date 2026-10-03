using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Hall of Honours (pf.honours), a stub shell at its M-D004 size: 16 x 18 x 9 m (x ±8, z -4..14) with a faceted
    /// apse from z 8, the arrival spot and a padless [ GRAND GALLERY ] door. M-C2 furnishes it per M-D006.
    /// </summary>
    public sealed class HallOfHonours : Room
    {
        public override string Key => PortfolioRegistrar.HonoursKey;
        public override string Title => "Hall of Honours";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 8f, Z0 = -4f, Z1 = 14f, Height = 9f, ApseZ = 8f;
        public static readonly Vector3 Arrival = new Vector3(0f, 0f, -1.5f);
        public static readonly WallOpening GalleryDoor = ManorDoor.Opening(Dir.PosZ, -2f);

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1, Height, GalleryDoor);
            ManorHall.Apse(root, HalfX, ApseZ, Z1, Height);
            ctx.SetSpawn(Arrival, 0f);
            Vector3 door = ManorDoor.Build(root, ctx, ManorHall.Face(HalfX, Z0, Z1, GalleryDoor), GalleryDoor.Facing,
                                           "[ GRAND GALLERY ]", ManorHall.GoTo(ctx, PortfolioRegistrar.GalleryKey));
            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 4f));
            AddSolution("gallery", RoomSolution.Kind.Teleport, door).Destination = PortfolioRegistrar.GalleryKey;
        }
    }
}

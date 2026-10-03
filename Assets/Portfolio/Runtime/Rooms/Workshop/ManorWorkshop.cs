using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Workshop (pf.workshop), a stub shell at its M-D004 size: 14 x 16 x 7 m (x ±7, z -4..12), the arrival spot on
    /// the future viewpoint medallion and a padless [ GRAND GALLERY ] door. M-C7 builds it out per M-D026.
    /// </summary>
    public sealed class ManorWorkshop : Room
    {
        public override string Key => PortfolioRegistrar.WorkshopKey;
        public override string Title => "Workshop";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 7f, Z0 = -4f, Z1 = 12f, Height = 7f;
        public static readonly Vector3 Arrival = new Vector3(0f, 0f, -1.5f);
        public static readonly WallOpening GalleryDoor = ManorDoor.Opening(Dir.PosZ, -2f);

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1, Height, GalleryDoor);
            ctx.SetSpawn(Arrival, 0f);
            Vector3 door = ManorDoor.Build(root, ctx, ManorHall.Face(HalfX, Z0, Z1, GalleryDoor), GalleryDoor.Facing,
                                           "[ GRAND GALLERY ]", ManorHall.GoTo(ctx, PortfolioRegistrar.GalleryKey));
            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 4f));
            AddSolution("gallery", RoomSolution.Kind.Teleport, door).Destination = PortfolioRegistrar.GalleryKey;
        }
    }
}

using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Painting Wing (pf.wing), a stub shell at its M-D004 size: 12 x 14 x 7 m (x ±6, z -4..10), the arrival spot
    /// and a padless [ GRAND GALLERY ] door. M-C2 adds the portal anchors per M-D008; G-C3 fills them.
    /// </summary>
    public sealed class PaintingWing : Room
    {
        public override string Key => PortfolioRegistrar.WingKey;
        public override string Title => "Painting Wing";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 6f, Z0 = -4f, Z1 = 10f, Height = 7f;
        public static readonly Vector3 Arrival = new Vector3(0f, 0f, -2f);
        public static readonly WallOpening GalleryDoor = ManorDoor.Opening(Dir.PosZ, -2.25f);

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

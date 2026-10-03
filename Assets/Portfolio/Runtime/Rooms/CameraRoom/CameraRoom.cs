using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Camera Room (pf.lens), a stub shell at its M-D004 size: 16 x 24 x 6 m (x ±8, z -4..20), the arrival spot and
    /// a padless [ GRAND GALLERY ] door. M-C2 builds it out per M-D007 and calls CameraRoomKit.Furnish.
    /// </summary>
    public sealed class CameraRoom : Room
    {
        public override string Key => PortfolioRegistrar.LensKey;
        public override string Title => "Camera Room";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 8f, Z0 = -4f, Z1 = 20f, Height = 6f;
        public static readonly Vector3 Arrival = new Vector3(0f, 0f, -2f);
        public static readonly WallOpening GalleryDoor = ManorDoor.Opening(Dir.PosZ, -2.25f);

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1, Height, GalleryDoor);
            ctx.SetSpawn(Arrival, 0f);
            Vector3 door = ManorDoor.Build(root, ctx, ManorHall.Face(HalfX, Z0, Z1, GalleryDoor), GalleryDoor.Facing,
                                           "[ GRAND GALLERY ]", ManorHall.GoTo(ctx, PortfolioRegistrar.GalleryKey));
            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 6f));
            AddSolution("gallery", RoomSolution.Kind.Teleport, door).Destination = PortfolioRegistrar.GalleryKey;
        }
    }
}

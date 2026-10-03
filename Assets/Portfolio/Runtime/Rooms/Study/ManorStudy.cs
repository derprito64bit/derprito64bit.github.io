using Ion.Levels;
using Ion.Levels.Arch;
using Ion.Presentation;
using UnityEngine;

namespace Ion.Portfolio
{
    /// <summary>
    /// The Study (pf.study), a stub shell at its M-D004 size: 10 x 14 x 4.5 m (x ±5, z -4..10), the arrival spot and a
    /// padless [ GRAND GALLERY ] door (a 3.5 m niche under the low ceiling). M-C7 builds it out per M-D027.
    /// </summary>
    public sealed class ManorStudy : Room
    {
        public override string Key => PortfolioRegistrar.StudyKey;
        public override string Title => "Study";
        public override ZoneMood Mood => ManorLook.Candlelight;
        public override ArchStyle Style => ManorLook.Style;

        public const float HalfX = 5f, Z0 = -4f, Z1 = 10f, Height = 4.5f, DoorHeight = 3.5f;
        public static readonly Vector3 Arrival = new Vector3(0f, 0f, -2f);
        public static readonly WallOpening GalleryDoor = ManorDoor.Opening(Dir.PosZ, 2.25f, DoorHeight);

        public override void Build(Transform root, RoomContext ctx)
        {
            ManorHall.Build(root, HalfX, Z0, Z1, Height, GalleryDoor);
            ctx.SetSpawn(Arrival, 0f);
            Vector3 door = ManorDoor.Build(root, ctx, ManorHall.Face(HalfX, Z0, Z1, GalleryDoor), GalleryDoor.Facing,
                                           ManorDoor.GalleryLabel, ManorHall.GoTo(ctx, PortfolioRegistrar.GalleryKey), DoorHeight);
            AddSolution("walk", RoomSolution.Kind.Walk, new Vector3(0f, 0f, 4f));
            AddSolution("gallery", RoomSolution.Kind.Teleport, door).Destination = PortfolioRegistrar.GalleryKey;
        }
    }
}

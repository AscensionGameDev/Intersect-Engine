using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    [Union(0, typeof(BanAction))]
    [Union(1, typeof(KickAction))]
    [Union(2, typeof(KillAction))]
    [Union(3, typeof(MuteAction))]
    [Union(4, typeof(SetAccessAction))]
    [Union(5, typeof(SetFaceAction))]
    [Union(6, typeof(SetSpriteAction))]
    [Union(7, typeof(UnbanAction))]
    [Union(8, typeof(UnmuteAction))]
    [Union(9, typeof(WarpMeToAction))]
    [Union(10, typeof(WarpToLocationAction))]
    [Union(11, typeof(WarpToMapAction))]
    [Union(12, typeof(WarpToMeAction))]
    [Union(13, typeof(ReturnToOverworldAction))]
    public abstract partial class AdminAction
    {
        [Key(0)]
        public abstract Enums.AdminAction Action { get; }

    }

}

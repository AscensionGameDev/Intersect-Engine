using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
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
    [MessagePackObject]
    public abstract class AdminAction
    {
        protected AdminAction(AdminActions action)
        {
            Action = action;
        }

        [Key(0)]
        public AdminActions Action { get; }



    }

}

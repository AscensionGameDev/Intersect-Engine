using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToMapAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToMapAction()
        {

        }

        public WarpToMapAction(Guid mapId)
        {
            MapId = mapId;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.WarpTo;

        [Key(2)]
        public Guid MapId { get; set; }

    }

}

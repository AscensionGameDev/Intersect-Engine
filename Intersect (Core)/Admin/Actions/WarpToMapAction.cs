using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public partial class WarpToMapAction : AdminAction
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
        public override Enums.AdminAction Action { get; } = Enums.AdminAction.WarpTo;

        [Key(2)]
        public Guid MapId { get; set; }

    }

}

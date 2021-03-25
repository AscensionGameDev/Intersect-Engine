using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToMapAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToMapAction() : base(AdminActions.WarpTo)
        {

        }

        public WarpToMapAction(Guid mapId) : base(AdminActions.WarpTo)
        {
            MapId = mapId;
        }

        [Key(1)]
        public Guid MapId { get; set; }

    }

}

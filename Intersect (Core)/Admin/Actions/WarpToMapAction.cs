using System;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class WarpToMapAction : AdminAction
    {

        public WarpToMapAction(Guid mapId)
        {
            MapId = mapId;
        }

        public override AdminActions Action { get; } = AdminActions.WarpTo;

        public Guid MapId { get; set; }

    }

}

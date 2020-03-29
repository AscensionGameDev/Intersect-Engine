using System;

using Intersect.Enums;

namespace Intersect.Admin.Actions
{

    public class WarpToLocationAction : AdminAction
    {

        public WarpToLocationAction(Guid mapId, byte x, byte y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        public override AdminActions Action { get; } = AdminActions.WarpTo;

        public Guid MapId { get; set; }

        public byte X { get; set; }

        public byte Y { get; set; }

    }

}

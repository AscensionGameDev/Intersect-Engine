using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToLocationAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToLocationAction()
        {

        }

        public WarpToLocationAction(Guid mapId, byte x, byte y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        [Key(1)]
        public override AdminActions Action { get; } = AdminActions.WarpTo;

        [Key(2)]
        public Guid MapId { get; set; }

        [Key(3)]
        public byte X { get; set; }

        [Key(4)]
        public byte Y { get; set; }

    }

}

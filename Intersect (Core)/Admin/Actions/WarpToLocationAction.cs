using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Admin.Actions
{
    [MessagePackObject]
    public class WarpToLocationAction : AdminAction
    {
        //Parameterless Constructor for MessagePack
        public WarpToLocationAction() : base(AdminActions.WarpTo)
        {

        }

        public WarpToLocationAction(Guid mapId, byte x, byte y) : base(AdminActions.WarpTo)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }

        [Key(1)]
        public Guid MapId { get; set; }

        [Key(2)]
        public byte X { get; set; }

        [Key(3)]
        public byte Y { get; set; }

    }

}

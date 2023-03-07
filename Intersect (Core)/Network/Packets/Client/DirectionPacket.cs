using System;
using System.Collections.Generic;

using Intersect.Collections;
using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class DirectionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public DirectionPacket()
        {
        }

        public DirectionPacket(Direction dir)
        {
            Direction = dir;
        }

        [Key(0)]
        public Direction Direction { get; set; }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Direction = (Direction) sanitizer.Clamp(
                nameof(Direction), (byte)Direction, 0, Enum.GetValues(typeof(Direction)).Length
            );

            return sanitizer.Sanitized;
        }

    }

}

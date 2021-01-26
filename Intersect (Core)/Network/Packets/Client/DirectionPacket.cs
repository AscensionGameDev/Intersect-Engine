using System;
using System.Collections.Generic;

using Intersect.Collections;
using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class DirectionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public DirectionPacket()
        {
        }

        public DirectionPacket(byte dir)
        {
            Direction = dir;
        }

        [Key(0)]
        public byte Direction { get; set; }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Direction = (byte) sanitizer.Clamp(
                nameof(Direction), Direction, 0, Enum.GetValues(typeof(Directions)).Length
            );

            return sanitizer.Sanitized;
        }

    }

}

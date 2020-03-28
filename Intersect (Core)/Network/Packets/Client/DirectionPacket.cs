using System;
using System.Collections.Generic;

using Intersect.Collections;
using Intersect.Enums;

namespace Intersect.Network.Packets.Client
{

    public class DirectionPacket : CerasPacket
    {

        public DirectionPacket(byte dir)
        {
            Direction = dir;
        }

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

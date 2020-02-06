using Intersect.Collections;
using Intersect.Enums;

using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Client
{
    public class DirectionPacket : CerasPacket
    {
        public byte Direction { get; set; }

        public DirectionPacket(byte dir)
        {
            Direction = dir;
        }

        public override Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            var sanitizer = new Sanitizer();

            Direction = (byte)sanitizer.Clamp(nameof(Direction), Direction, 0, Enum.GetValues(typeof(Directions)).Length);

            return sanitizer.Sanitized;
        }
    }
}

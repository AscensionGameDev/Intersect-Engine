using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class LabelPacket : CerasPacket
    {
        public string Label;
        public Color Color;

        public LabelPacket(string label, Color color)
        {
            Label = label;
            Color = color;
        }
    }
}
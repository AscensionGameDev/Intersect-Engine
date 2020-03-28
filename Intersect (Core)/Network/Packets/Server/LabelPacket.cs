namespace Intersect.Network.Packets.Server
{

    public class LabelPacket : CerasPacket
    {

        public Color Color;

        public string Label;

        public LabelPacket(string label, Color color)
        {
            Label = label;
            Color = color;
        }

    }

}

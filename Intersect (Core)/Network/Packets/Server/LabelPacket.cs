using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class LabelPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public LabelPacket()
        {
        }


        [Key(0)]
        public Color Color;
        [Key(1)]
        public string Label;

        public LabelPacket(string label, Color color)
        {
            Label = label;
            Color = color;
        }

    }

}

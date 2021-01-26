using MessagePack;
using System;

namespace Intersect.Network.Packets.Editor
{

    [MessagePackObject]
    public class LinkMapPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public LinkMapPacket()
        {
        }

        public LinkMapPacket(Guid linkId, Guid adjacentId, int gridX, int gridY)
        {
            LinkMapId = linkId;
            AdjacentMapId = adjacentId;
            GridX = gridX;
            GridY = gridY;
        }

        [Key(0)]
        public Guid LinkMapId { get; set; }

        [Key(1)]
        public Guid AdjacentMapId { get; set; }

        [Key(2)]
        public int GridX { get; set; }

        [Key(3)]
        public int GridY { get; set; }

    }

}

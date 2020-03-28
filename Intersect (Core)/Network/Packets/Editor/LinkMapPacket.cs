using System;

namespace Intersect.Network.Packets.Editor
{

    public class LinkMapPacket : EditorPacket
    {

        public LinkMapPacket(Guid linkId, Guid adjacentId, int gridX, int gridY)
        {
            LinkMapId = linkId;
            AdjacentMapId = adjacentId;
            GridX = gridX;
            GridY = gridY;
        }

        public Guid LinkMapId { get; set; }

        public Guid AdjacentMapId { get; set; }

        public int GridX { get; set; }

        public int GridY { get; set; }

    }

}

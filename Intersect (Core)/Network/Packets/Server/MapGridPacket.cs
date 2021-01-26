using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapGridPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapGridPacket()
        {
        }

        public MapGridPacket(Guid[,] grid, string[,] editorGrid, bool clearKnownMaps)
        {
            Grid = grid;
            EditorGrid = editorGrid;
            ClearKnownMaps = clearKnownMaps;
        }

        [Key(0)]
        public Guid[,] Grid { get; set; }

        [Key(1)]
        public string[,] EditorGrid { get; set; }

        [Key(2)]
        public bool ClearKnownMaps { get; set; }

    }

}

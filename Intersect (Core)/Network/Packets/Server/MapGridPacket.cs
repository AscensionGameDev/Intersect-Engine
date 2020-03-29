using System;

namespace Intersect.Network.Packets.Server
{

    public class MapGridPacket : CerasPacket
    {

        public MapGridPacket(Guid[,] grid, string[,] editorGrid, bool clearKnownMaps)
        {
            Grid = grid;
            EditorGrid = editorGrid;
            ClearKnownMaps = clearKnownMaps;
        }

        public Guid[,] Grid { get; set; }

        public string[,] EditorGrid { get; set; }

        public bool ClearKnownMaps { get; set; }

    }

}

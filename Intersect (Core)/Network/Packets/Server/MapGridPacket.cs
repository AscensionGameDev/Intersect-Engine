using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapGridPacket : CerasPacket
    {
        public Guid[,] Grid { get; set; }
        public string [,] EditorGrid { get; set; }
        public bool ClearKnownMaps { get; set; }

        public MapGridPacket(Guid[,] grid, string[,] editorGrid, bool clearKnownMaps)
        {
            Grid = grid;
            EditorGrid = editorGrid;
            ClearKnownMaps = clearKnownMaps;
        }
    }
}

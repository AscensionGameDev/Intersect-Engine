using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class AddTilesetsPacket : EditorPacket
    {
        public string[] Tilesets { get; set; }

        public AddTilesetsPacket(string[] tilesets)
        {
            Tilesets = tilesets;
        }
    }
}

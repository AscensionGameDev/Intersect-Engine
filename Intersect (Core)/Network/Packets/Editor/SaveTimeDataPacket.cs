using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class SaveTimeDataPacket : EditorPacket
    {
        public byte[] Data { get; set; }

        public SaveTimeDataPacket(byte[] data)
        {
            Data = data;
        }
    }
}

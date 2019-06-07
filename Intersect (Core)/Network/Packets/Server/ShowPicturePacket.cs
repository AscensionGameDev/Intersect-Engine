using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ShowPicturePacket : CerasPacket
    {
        public string Picture { get; set; }
        public int Size { get; set; }
        public bool Clickable { get; set; }

        public ShowPicturePacket(string picture, int size, bool clickable)
        {
            Picture = picture;
            Size = size;
            Clickable = clickable;
        }
    }
}

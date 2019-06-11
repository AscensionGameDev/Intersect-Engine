using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class CreateCharacterPacket : CerasPacket
    {
        public string Name { get; set; }
        public Guid ClassId { get; set; }
        public int Sprite { get; set; }

        public CreateCharacterPacket(string name, Guid classId, int sprite)
        {
            Name = name;
            ClassId = classId;
            Sprite = sprite;
        }
    }
}

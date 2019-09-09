using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class UseSpellPacket : CerasPacket
    {
        public int Slot { get; set; }
        public Guid TargetId { get; set; }

        public UseSpellPacket(int slot, Guid targetId)
        {
            Slot = slot;
            TargetId = targetId;
        }
    }
}

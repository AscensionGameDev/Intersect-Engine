using System;

namespace Intersect.Network.Packets.Server
{

    public class BankUpdatePacket : InventoryUpdatePacket
    {

        public BankUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs) : base(
            slot, id, quantity, bagId, statBuffs
        )
        {
        }

    }

}

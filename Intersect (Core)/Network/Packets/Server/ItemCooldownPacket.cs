using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class ItemCooldownPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ItemCooldownPacket()
        {
        }


        [Key(0)]
        public Dictionary<Guid, long> ItemCds;

        public ItemCooldownPacket(Dictionary<Guid, long> itemCds)
        {
            ItemCds = itemCds;
        }

    }

}

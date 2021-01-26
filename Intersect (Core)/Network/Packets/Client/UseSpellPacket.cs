﻿using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UseSpellPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UseSpellPacket()
        {
        }

        public UseSpellPacket(int slot, Guid targetId)
        {
            Slot = slot;
            TargetId = targetId;
        }

        [Key(0)]
        public int Slot { get; set; }

        [Key(1)]
        public Guid TargetId { get; set; }

    }

}

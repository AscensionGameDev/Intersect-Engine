using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PartyMemberPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PartyMemberPacket()
        {
        }

        public PartyMemberPacket(Guid id, string name, int[] vital, int[] maxVital, int level)
        {
            Id = id;
            Name = name;
            Vital = vital;
            MaxVital = maxVital;
            Level = level;
        }

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public int[] Vital { get; set; }

        [Key(3)]
        public int[] MaxVital { get; set; }

        [Key(4)]
        public int Level { get; set; }

    }

}

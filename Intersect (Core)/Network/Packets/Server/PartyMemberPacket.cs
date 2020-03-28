using System;

namespace Intersect.Network.Packets.Server
{

    public class PartyMemberPacket : CerasPacket
    {

        public PartyMemberPacket(Guid id, string name, int[] vital, int[] maxVital, int level)
        {
            Id = id;
            Name = name;
            Vital = vital;
            MaxVital = maxVital;
            Level = level;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int[] Vital { get; set; }

        public int[] MaxVital { get; set; }

        public int Level { get; set; }

    }

}

using System;
using Intersect.Enums;

namespace Intersect.Client.Classes.Entities
{
    public class PartyMember
    {
        public Guid Id;
        public string Name;
        public int[] Vital = new int[(int)Vitals.VitalCount];
        public int[] MaxVital = new int[(int)Vitals.VitalCount];

        public PartyMember(ByteBuffer bf)
        {
            Id = bf.ReadGuid();
            Name = bf.ReadString();
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                Vital[i] = bf.ReadInteger();
            }
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                MaxVital[i] = bf.ReadInteger();
            }
        }
    }
}


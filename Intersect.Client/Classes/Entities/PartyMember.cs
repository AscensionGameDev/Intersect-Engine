using System;
using Intersect.Enums;

namespace Intersect.Client.Entities
{
    public class PartyMember
    {
        public Guid Id;
        public string Name;
        public int[] Vital = new int[(int)Vitals.VitalCount];
        public int[] MaxVital = new int[(int)Vitals.VitalCount];
        public int Level;

        public PartyMember(Guid id, string name, int[] vital, int[] maxVital, int level)
        {
            Id = id;
            Name = name;
            Vital = vital;
            MaxVital = maxVital;
            Level = level;
        }
    }
}


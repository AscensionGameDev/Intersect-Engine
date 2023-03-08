using System;
using Intersect.Client.Framework.Entities;
using Intersect.Enums;

namespace Intersect.Client.Entities
{

    public partial class PartyMember : IPartyMember
    {

        public Guid Id { get; set; }

        public int Level { get; set; }

        public int[] MaxVital { get; set; } = new int[(int)Enums.Vital.VitalCount];

        public string Name { get; set; }

        public int[] Vital { get; set; } = new int[(int)Enums.Vital.VitalCount];

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

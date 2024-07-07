using Intersect.Client.Framework.Entities;
using Intersect.Enums;

namespace Intersect.Client.Entities;


public partial class PartyMember : IPartyMember
{

    public Guid Id { get; set; }

    public int Level { get; set; }

    public long[] MaxVital { get; set; } = new long[Enum.GetValues<Vital>().Length];

    public string Name { get; set; }

    public long[] Vital { get; set; } = new long[Enum.GetValues<Vital>().Length];

    public PartyMember(Guid id, string name, long[] vital, long[] maxVital, int level)
    {
        Id = id;
        Name = name;
        Vital = vital;
        MaxVital = maxVital;
        Level = level;
    }

}

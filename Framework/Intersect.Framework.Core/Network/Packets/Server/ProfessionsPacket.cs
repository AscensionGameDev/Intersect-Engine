using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class ProfessionsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public ProfessionsPacket()
    {
    }

    public ProfessionsPacket(Dictionary<int, int> skills, List<Guid> knownRecipes)
    {
        Skills = skills;
        KnownRecipes = knownRecipes;
    }

    /// <summary>Profession id (<c>Profession</c> enum) → current skill, for every profession the player has learned.</summary>
    [Key(0)]
    public Dictionary<int, int> Skills { get; set; } = [];

    /// <summary>Recipes learned outside the trainer (recipe items, quests, bosses).</summary>
    [Key(1)]
    public List<Guid> KnownRecipes { get; set; } = [];
}

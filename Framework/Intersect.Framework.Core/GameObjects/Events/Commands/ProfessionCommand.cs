using Intersect.Framework.Core.GameObjects.Crafting;

namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public enum ProfessionAction : byte
{
    /// <summary>Teach the profession (skill 1). Refused when the player already knows two crafting professions.</summary>
    LearnProfession = 0,

    /// <summary>Make the player forget the profession and its skill.</summary>
    ForgetProfession = 1,

    /// <summary>Teach one specific recipe (recipe items, quests, bosses).</summary>
    LearnRecipe = 2,

    /// <summary>Gathering skill-up roll: <see cref="ProfessionCommand.Amount"/> is the skill the node requires (orange).</summary>
    GatheringSkillUp = 3,

    /// <summary>Add <see cref="ProfessionCommand.Amount"/> skill points to a learned profession (trainer rewards, testing).</summary>
    AddSkill = 4,
}

/// <summary>Everything an NPC trainer, a recipe item or a gathering node needs to do with the profession system.</summary>
public partial class ProfessionCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.Profession;

    public ProfessionAction Action { get; set; }

    public Profession Profession { get; set; }

    public Guid RecipeId { get; set; }

    public int Amount { get; set; }
}

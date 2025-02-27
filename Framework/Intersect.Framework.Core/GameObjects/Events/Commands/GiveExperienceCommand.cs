namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class GiveExperienceCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.GiveExperience;

    public long Exp { get; set; }

    /// <summary>
    /// Defines whether this event command will use a variable for processing or not.
    /// </summary>
    public bool UseVariable { get; set; } = false;

    /// <summary>
    /// Defines whether the variable used is a Player or Global variable.
    /// </summary>
    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    /// <summary>
    /// The Variable Id to use.
    /// </summary>
    public Guid VariableId { get; set; }

    /// <summary>
    /// If true, when a player have their experience reduced, they will be able to level down.
    /// </summary>
    public bool EnableLosingLevels { get; set; } = false;
}
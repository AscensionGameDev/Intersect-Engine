using Intersect.Enums;

namespace Intersect.GameObjects.Events;

/// <summary>
/// Defines the condition class used when checking for a player's free inventory slots.
/// </summary>
public partial class HasFreeInventorySlots : Condition
{
    /// <summary>
    /// Defines the type of condition.
    /// </summary>
    public override ConditionType Type { get; } = ConditionType.HasFreeInventorySlots;

    /// <summary>
    /// Defines the amount of inventory slots that need to be free to clear this condition.
    /// </summary>
    public int Quantity { get; set; }

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
}
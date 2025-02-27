namespace Intersect.GameObjects.Events.Commands;

public partial class OpenCraftingTableCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.OpenCraftingTable;

    public Guid CraftingTableId { get; set; }

    /// <summary>
    /// Does not allow crafting, but displays crafts and their requirements.
    /// </summary>
    public bool JournalMode { get; set; }
}
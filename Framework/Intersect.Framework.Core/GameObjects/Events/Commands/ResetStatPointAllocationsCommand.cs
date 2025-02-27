namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ResetStatPointAllocationsCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ResetStatPointAllocations;
}
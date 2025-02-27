namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ShowPlayerCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ShowPlayer;
}
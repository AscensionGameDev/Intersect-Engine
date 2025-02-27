namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class LevelUpCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.LevelUp;
}
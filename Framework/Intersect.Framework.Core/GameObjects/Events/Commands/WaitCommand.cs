namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class WaitCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.Wait;

    public int Time { get; set; }
}
namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class RestoreHpCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.RestoreHp;

    public int Amount { get; set; }
}
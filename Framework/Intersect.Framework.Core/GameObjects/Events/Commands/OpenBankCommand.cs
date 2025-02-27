namespace Intersect.GameObjects.Events.Commands;

public partial class OpenBankCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.OpenBank;
}
namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class OpenGuildBankCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.OpenGuildBank;
}
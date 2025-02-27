namespace Intersect.GameObjects.Events.Commands;

public partial class FadeoutBgmCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.FadeoutBgm;
}
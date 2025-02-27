namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class FadeoutBgmCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.FadeoutBgm;
}
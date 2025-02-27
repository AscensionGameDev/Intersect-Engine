namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class SetSelfSwitchCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetSelfSwitch;

    public int SwitchId { get; set; } //0 through 3

    public bool Value { get; set; }
}
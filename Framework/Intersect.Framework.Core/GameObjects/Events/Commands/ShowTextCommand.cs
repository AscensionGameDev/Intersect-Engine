namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ShowTextCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ShowText;

    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// English translation of <see cref="Text"/>. Optional - when empty, English-language players just see
    /// the base <see cref="Text"/> (whatever language it was written in) like everyone else.
    /// </summary>
    public string TextEn { get; set; } = string.Empty;

    public string Face { get; set; } = string.Empty;
}
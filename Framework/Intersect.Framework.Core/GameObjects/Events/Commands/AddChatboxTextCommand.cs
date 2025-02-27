using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class AddChatboxTextCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.AddChatboxText;

    public string Text { get; set; } = string.Empty;

    // TODO: Expose this option to the user?
    public ChatMessageType MessageType { get; set; } = ChatMessageType.Notice;

    public string Color { get; set; } = string.Empty;

    public ChatboxChannel Channel { get; set; } = ChatboxChannel.Player;

    public bool ShowChatBubble { get; set; }

    public bool ShowChatBubbleInProximity { get; set; }

}
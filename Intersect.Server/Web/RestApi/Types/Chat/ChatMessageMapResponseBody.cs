namespace Intersect.Server.Web.RestApi.Types.Chat;

public class ChatMessageMapResponseBody(Guid mapId, bool success, ChatMessage chatMessage) : ChatMessageResponseBody(success, chatMessage)
{
    public Guid MapId { get; set; } = mapId;
}
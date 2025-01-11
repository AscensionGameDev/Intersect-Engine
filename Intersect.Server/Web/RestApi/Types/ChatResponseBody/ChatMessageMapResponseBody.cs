using Intersect.Server.Web.RestApi.Payloads;

namespace Intersect.Server.Web.RestApi.Types.ChatResponseBody;

public class ChatMessageMapResponseBody(Guid mapId, bool success, ChatMessage chatMessage) : ChatMessageResponseBody(success, chatMessage)
{
    public Guid MapId { get; set; } = mapId;
}
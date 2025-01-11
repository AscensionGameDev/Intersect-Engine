using Intersect.Server.Web.RestApi.Payloads;

namespace Intersect.Server.Web.RestApi.Types.ChatResponseBody;

public class ChatMessageLookupKeyResponseBody(LookupKey lookupKey, bool success, ChatMessage chatMessage) : ChatMessageResponseBody(success, chatMessage)
{
    public LookupKey PlayerId { get; set; } = lookupKey;
}

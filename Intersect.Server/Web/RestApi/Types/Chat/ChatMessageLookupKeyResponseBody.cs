using Intersect.Server.Collections.Indexing;

namespace Intersect.Server.Web.RestApi.Types.Chat;

public class ChatMessageLookupKeyResponseBody(LookupKey lookupKey, bool success, ChatMessage chatMessage) : ChatMessageResponseBody(success, chatMessage)
{
    public LookupKey LookupKey { get; set; } = lookupKey;
}

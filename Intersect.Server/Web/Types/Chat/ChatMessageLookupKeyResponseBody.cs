using Intersect.Server.Collections.Indexing;

namespace Intersect.Server.Web.Types.Chat;

public record ChatMessageLookupKeyResponseBody(LookupKey LookupKey, bool Success, ChatMessage ChatMessage) : ChatMessageResponseBody(Success, ChatMessage);

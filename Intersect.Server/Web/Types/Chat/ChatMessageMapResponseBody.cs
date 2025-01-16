namespace Intersect.Server.Web.Types.Chat;

public record ChatMessageMapResponseBody(Guid MapId, bool Success, ChatMessage ChatMessage) : ChatMessageResponseBody(Success, ChatMessage);
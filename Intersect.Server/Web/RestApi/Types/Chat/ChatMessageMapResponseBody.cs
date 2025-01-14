namespace Intersect.Server.Web.RestApi.Types.Chat;

public record ChatMessageMapResponseBody(Guid MapId, bool Success, ChatMessage ChatMessage) : ChatMessageResponseBody(Success, ChatMessage);
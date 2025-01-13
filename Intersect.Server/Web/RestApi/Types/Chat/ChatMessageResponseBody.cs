namespace Intersect.Server.Web.RestApi.Types.Chat;

public class ChatMessageResponseBody(bool success, ChatMessage chatMessage)
{
    public bool Success { get; set; } = success;

    public ChatMessage ChatMessage { get; set; } = chatMessage;
}

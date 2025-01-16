namespace Intersect.Server.Web.Types.User;

public struct RegisterResponseBody(string username, string email)
{
    public string Username { get; set; } = username;

    public string Email { get; set; } = email;
}

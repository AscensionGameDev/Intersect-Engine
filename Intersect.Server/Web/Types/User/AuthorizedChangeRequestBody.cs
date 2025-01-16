namespace Intersect.Server.Web.Types.User;

public record struct AuthorizedChangeRequestBody(string Authorization, string New);

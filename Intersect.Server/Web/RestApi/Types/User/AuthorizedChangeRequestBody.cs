namespace Intersect.Server.Web.RestApi.Types.User;

public record struct AuthorizedChangeRequestBody(string Authorization, string New);

namespace Intersect.Server.Web.RestApi.Types.User;

public partial struct PasswordValidationRequestBody(string password)
{
    public string Password { get; set; } = password;
}

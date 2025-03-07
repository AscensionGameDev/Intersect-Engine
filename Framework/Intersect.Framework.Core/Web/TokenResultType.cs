namespace Intersect.Web;

public enum TokenResultType
{
    Unknown,
    TokenReceived,
    RequestError,
    InvalidUsername,
    InvalidPassword,
    ClientSideFailure,
    InvalidRefreshToken,
    InvalidCredentials,
    InvalidResponse,
    Failed,
}

namespace Intersect.Framework.Core.Security;

public enum PasswordResetResultType
{
    Unknown,
    Success,
    NoUserFound,
    InvalidRequest,
    InvalidToken,
}
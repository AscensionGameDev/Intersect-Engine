using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Intersect.Security;

public partial class PasswordUtils
{
    [return: NotNull]
    public static string ComputePasswordHash(string? password)
    {
        return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(password ?? string.Empty))).Replace("-", string.Empty);
    }

    public static bool IsValidClientPasswordHash([NotNullWhen(true)] string? hashToValidate) =>
            hashToValidate is { Length: 64 } && hashToValidate.All(char.IsAsciiHexDigit);
}

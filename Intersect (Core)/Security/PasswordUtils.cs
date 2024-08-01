using System.Security.Cryptography;
using System.Text;

namespace Intersect.Security;

public partial class PasswordUtils
{
    public static string ComputePasswordHash(string password)
    {
        return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(password ?? string.Empty))).Replace("-", string.Empty);
    }
}

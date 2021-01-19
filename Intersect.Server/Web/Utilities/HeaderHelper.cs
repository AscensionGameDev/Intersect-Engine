using System.Text.RegularExpressions;

namespace Intersect.Server.Web.Utilities
{

    public static class HeaderHelper
    {

        public static readonly Regex PatternAuthorizationBearer = new Regex(
            "^bearer .+$", RegexOptions.IgnoreCase
        );

        public static bool IsValidAuthorizationBearerHeader(string authorizationHeader)
        {
            return !string.IsNullOrWhiteSpace(authorizationHeader) &&
                   PatternAuthorizationBearer.IsMatch(authorizationHeader);
        }

    }

}

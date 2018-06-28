using Intersect.Localization;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.Utilities
{
    public static class TextUtils
    {
        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }

        public static bool IsNone(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            if (str.Trim() == string.Empty) return true;
            if (str == "None") return true;
            return str == Strings.Get("general", "none");
        }

        public static string NullToNone(string nullableString)
        {
            return IsNone(nullableString) ? Strings.Get("general", "none") : nullableString;
        }

        public static string SanitizeNone(string nullableString)
        {
            return IsNone(nullableString) ? null : nullableString;
        }

    }
}
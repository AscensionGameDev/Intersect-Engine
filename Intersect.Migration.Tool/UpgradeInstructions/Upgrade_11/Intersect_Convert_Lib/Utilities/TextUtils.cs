namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Utilities
{
    public static class TextUtils
    {
        public static string None = "None";

        public static void Init(string none)
        {
            None = none;
        }

        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }

        public static bool IsNone(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            if (str.Trim() == string.Empty) return true;
            if (str == "None") return true;
            return str == None;
        }

        public static string NullToNone(string nullableString)
        {
            return IsNone(nullableString) ? None : nullableString;
        }

        public static string SanitizeNone(string nullableString)
        {
            return IsNone(nullableString) ? null : nullableString;
        }

    }
}
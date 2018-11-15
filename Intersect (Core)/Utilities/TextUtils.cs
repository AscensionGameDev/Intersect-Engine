using System;

namespace Intersect.Utilities
{
    public static class TextUtils
    {
        public static string None { get; set; }

        static TextUtils()
        {
            None = "None";
        }

        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }

        public static bool IsNone(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            if (str.Trim() == string.Empty) return true;
            return string.Equals("None", StripToLower(str), StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(None, StripToLower(str), StringComparison.InvariantCultureIgnoreCase);
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
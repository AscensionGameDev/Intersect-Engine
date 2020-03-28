using System;

namespace Intersect.Utilities
{

    public static class TextUtils
    {

        static TextUtils()
        {
            None = "None";
        }

        public static string None { get; set; }

        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }

        public static bool IsNone(string str)
        {
            if (string.IsNullOrEmpty(str?.Trim()))
            {
                return true;
            }

            return string.Equals("None", StripToLower(str), StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(None, StripToLower(str), StringComparison.InvariantCultureIgnoreCase);
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

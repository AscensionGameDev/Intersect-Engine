namespace Intersect
{
    public static class TextUtils
    {
        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }
    }
}
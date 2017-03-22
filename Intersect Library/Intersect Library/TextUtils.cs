namespace Intersect_Library
{
    public static class TextUtils
    {
        public static bool IsEmpty(string str)
        {
            return (str == null || str.Length < 1);
        }
    }
}

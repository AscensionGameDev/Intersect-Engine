namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib
{
    public static class TextUtils
    {
        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }
    }
}
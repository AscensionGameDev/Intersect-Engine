namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib
{
    public static class TextUtils
    {
        public static string StripToLower(string source)
        {
            return source?.ToLowerInvariant().Replace(" ", "");
        }
    }
}
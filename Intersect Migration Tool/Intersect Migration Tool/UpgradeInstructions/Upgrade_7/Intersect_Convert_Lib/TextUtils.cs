namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib
{
    public static class TextUtils
    {
        public static bool IsEmpty(string str)
        {
            return (str == null || str.Length < 1);
        }
    }
}

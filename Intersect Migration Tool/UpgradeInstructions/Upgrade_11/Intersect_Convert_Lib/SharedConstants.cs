using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib
{
    public static class SharedConstants
    {
        public static readonly string VersionName = "Beta Delphini (201707100747PM)";
        public static readonly byte[] VersionData = Encoding.UTF8.GetBytes(VersionName);
    }
}
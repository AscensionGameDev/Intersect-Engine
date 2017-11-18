using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib
{
    public static class SharedConstants
    {
        public static readonly string VERSION_NAME = "Beta Delphini (201707100747PM)";
        public static readonly byte[] VERSION_DATA = Encoding.UTF8.GetBytes(VERSION_NAME);
    }
}
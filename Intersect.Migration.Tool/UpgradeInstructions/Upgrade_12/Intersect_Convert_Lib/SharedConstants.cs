using System.Text;
using JetBrains.Annotations;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib
{
    public static class SharedConstants
    {
        [NotNull] public static readonly string VersionName = "Beta Eridani (201802251101-05)";
        [NotNull] public static readonly byte[] VersionData = Encoding.UTF8.GetBytes(VersionName);
    }
}
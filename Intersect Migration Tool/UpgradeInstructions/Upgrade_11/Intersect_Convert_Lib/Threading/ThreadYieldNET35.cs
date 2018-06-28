using System.Threading;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Threading
{
    public class ThreadYieldNet35 : IThreadYield
    {
        public void Yield() => Thread.Sleep(0);
    }
}
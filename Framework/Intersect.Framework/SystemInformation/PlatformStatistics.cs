using Hardware.Info;
using Microsoft.Extensions.Logging;

namespace Intersect.Framework.SystemInformation;

public class PlatformStatistics
{
    private static readonly HardwareInfo HardwareInfo;

    public static IGPUStatisticsProvider? GPUStatisticsProvider { get; set; }

    public static ILogger? Logger { get; set; }

    public static long AvailablePhysicalMemory => (long)HardwareInfo.MemoryStatus.AvailablePhysical;

    public static long TotalPhysicalMemory => (long)HardwareInfo.MemoryStatus.TotalPhysical;

    public static long AvailableGPUMemory => GPUStatisticsProvider?.AvailableMemory ?? AvailableSystemMemory;

    public static long TotalGPUMemory => GPUStatisticsProvider?.TotalMemory ?? TotalSystemMemory;

    public static long AvailableSystemMemory => (long)HardwareInfo.MemoryStatus.AvailableVirtual;

    public static long TotalSystemMemory => (long)HardwareInfo.MemoryStatus.TotalVirtual;

    public static void Refresh()
    {
        try
        {
            HardwareInfo.RefreshMemoryStatus();
        }
        catch
        {
            // Do nothing
        }
    }

    static PlatformStatistics()
    {
        HardwareInfo = new HardwareInfo();

        Refresh();
    }
}
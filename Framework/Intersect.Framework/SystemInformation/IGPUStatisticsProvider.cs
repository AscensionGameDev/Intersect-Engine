namespace Intersect.Framework.SystemInformation;

public interface IGPUStatisticsProvider
{
    long? AvailableMemory { get; }
    
    long? TotalMemory { get; }
}
namespace Intersect.Config;

public partial class FloodThresholdOptions
{
    /// <summary>
    /// The largest a single packet should be before it's considered flooding.
    /// </summary>
    public int MaxPacketSize { get; set; } = 10240;

    /// <summary>
    /// The maximum number of packets we should receive from a cient before we consider them to be flooding.
    /// </summary>
    public int MaxPacketPerSec { get; set; } = 50;

    /// <summary>
    /// The number of packets received per second on average that we will accept before kicking the client for flooding.
    /// </summary>
    public int KickAvgPacketPerSec { get; set; } = 30;

    public static FloodThresholdOptions Editor()
    {
        return new FloodThresholdOptions()
        {
            MaxPacketSize = int.MaxValue,
            MaxPacketPerSec = int.MaxValue,
            KickAvgPacketPerSec = int.MaxValue
        };
    }

    public static FloodThresholdOptions NotLoggedIn()
    {
        return new FloodThresholdOptions()
        {
            MaxPacketSize = 10240,
            MaxPacketPerSec = 5,
            KickAvgPacketPerSec = 3,
        };
    }
}
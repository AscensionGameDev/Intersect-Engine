using Intersect.GameObjects.Events;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public class FadePacket : IntersectPacket
{

    public FadePacket()
    {
    }

    public FadePacket(FadeType fadeType, bool waitForCompletion, int speedMs)
    {
        FadeType = fadeType;
        WaitForCompletion = waitForCompletion;
        SpeedMs = speedMs;
    }

    [Key(0)]
    public FadeType FadeType { get; set; }

    [Key(1)]
    public bool WaitForCompletion { get; set; }

    [Key(2)]
    public int SpeedMs { get; set; }
}

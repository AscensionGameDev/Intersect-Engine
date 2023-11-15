using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public class FadePacket : IntersectPacket
{
    public FadePacket(bool fadeOut)
    {
        FadeOut = fadeOut;
    }

    public FadePacket()
    {
    }

    [Key(0)]
    public bool FadeOut { get; set; }
}

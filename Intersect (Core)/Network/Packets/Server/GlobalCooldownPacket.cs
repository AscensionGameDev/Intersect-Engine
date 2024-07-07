using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class GlobalCooldownPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public GlobalCooldownPacket()
    {
    }

    [Key(0)]
    public long GlobalCooldown { get; set; }

    public GlobalCooldownPacket(long cooldown)
    {
        GlobalCooldown = cooldown;
    }

}

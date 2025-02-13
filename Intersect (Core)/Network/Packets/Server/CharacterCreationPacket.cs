using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class CharacterCreationPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public CharacterCreationPacket()
    {
    }

    public CharacterCreationPacket(bool force)
    {
        Force = force;
    }

    [Key(0)]
    public bool Force { get; set; }
}

using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class PetEntityPacket : EntityPacket
{
    //Parameterless Constructor for MessagePack
    public PetEntityPacket()
    {
    }

    /// <summary>
    /// Entity id of the player this pet follows.
    /// </summary>
    [Key(24)]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Id of the <c>PetDescriptor</c> this pet was created from.
    /// </summary>
    [Key(25)]
    public Guid PetDescriptorId { get; set; }
}

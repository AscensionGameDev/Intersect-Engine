using MessagePack;

namespace Intersect.Network.Packets.Client;

/// <summary>
/// Sent by the client right after it finishes loading a map it doesn't have cached yet, to ask the server
/// to resend the current guild hall furniture for it. Needed because <see cref="Intersect.Network.Packets.Server.FurnitureInstancesPacket"/>
/// is pushed by the server as soon as the player enters an instance, which can race ahead of the client's
/// own (client-pulled) map data request - if that happens the furniture packet is silently dropped because
/// the client doesn't have a <c>MapInstance</c> to apply it to yet.
/// </summary>
[MessagePackObject]
public partial class RequestFurniturePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public RequestFurniturePacket()
    {
    }
}

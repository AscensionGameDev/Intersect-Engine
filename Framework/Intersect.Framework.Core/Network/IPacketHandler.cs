namespace Intersect.Network;

public interface IPacketHandler
{
    bool Handle(IPacketSender packetSender, IPacket packet);
}
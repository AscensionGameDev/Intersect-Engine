using Intersect.Framework.Reflection;

namespace Intersect.Network;

public interface IPacketHandler<in TPacket> : IPacketHandler where TPacket : class, IPacket
{

    bool IPacketHandler.Handle(IPacketSender packetSender, IPacket packet)
    {
        if (packet is TPacket typedPacket)
        {
            return Handle(packetSender, typedPacket);
        }

        var expectedPacketTypeName = typeof(TPacket).GetName(qualified: true);
        var packetTypeName = packet.GetType().GetName(qualified: true);
        throw new ArgumentException($"Expected a {expectedPacketTypeName} but got a {packetTypeName}", nameof(packet));
    }

    bool Handle(IPacketSender packetSender, TPacket packet);
}
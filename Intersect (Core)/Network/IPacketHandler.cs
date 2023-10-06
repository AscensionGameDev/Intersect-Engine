namespace Intersect.Network
{
    public interface IPacketHandler
    {
        bool Handle(IPacketSender packetSender, IPacket packet);
    }

    public interface IPacketHandler<TPacket> : IPacketHandler where TPacket : class, IPacket
    {

        bool Handle(IPacketSender packetSender, TPacket packet);
    }

    public abstract class AbstractPacketHandler<TPacket> : IPacketHandler<TPacket> where TPacket : class, IPacket
    {
        public abstract bool Handle(IPacketSender packetSender, TPacket packet);

        public virtual bool Handle(IPacketSender packetSender, IPacket packet)
        {
            return packet is TPacket typedPacket && Handle(packetSender, typedPacket);
        }
    }
}

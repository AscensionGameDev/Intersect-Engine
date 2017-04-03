namespace Intersect.Network
{
    public abstract class AbstractExtendedPacket<TType> : AbstractPacket, IExtendedPacket<TType>
    {
        public TType Type { get; }

        protected AbstractExtendedPacket(IConnection connection, PacketGroups group, TType type)
            : base(connection, group)
        {
            Type = type;
        }
    }
}
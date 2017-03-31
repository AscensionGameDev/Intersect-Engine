using System;
using Lidgren.Network;

namespace Intersect.Network
{
    public abstract class AbstractExtendedPacket<TType> : AbstractPacket, IExtendedPacket<TType>
    {
        public TType Type { get; }

        protected AbstractExtendedPacket(NetConnection connection, PacketGroups group, TType type)
            : base(connection, group)
        {
            Type = type;
        }
    }
}
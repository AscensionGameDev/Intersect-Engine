using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public abstract class AbstractExtendedPacket<TType> : AbstractPacket, IExtendedPacket<TType> where TType : struct, IConvertible
    {
        public TType Type { get; }

        protected AbstractExtendedPacket(IConnection connection, PacketGroups group, TType type)
            : base(connection, group)
        {
            Type = type;
        }

        public override bool Write(ref IBuffer buffer)
        {
            if (!base.Write(ref buffer)) return false;
            buffer.Write(Convert.ToByte(Type));
            return true;
        }
    }
}
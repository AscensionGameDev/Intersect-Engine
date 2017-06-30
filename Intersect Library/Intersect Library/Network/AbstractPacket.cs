using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public abstract class AbstractPacket : IPacket
    {
        public IConnection Connection { get; }

        public double Timestamp { get; set; }

        public virtual int EstimatedSize { get; }

        public PacketType Type => PacketType.Of(this);

        public PacketCodes Code { get; }

        protected AbstractPacket(IConnection connection, PacketCodes code)
        {
            Connection = connection;
            Code = code;
        }

        public virtual bool Read(ref IBuffer buffer) => (buffer != null);

        public virtual bool Write(ref IBuffer buffer)
        {
            buffer.Write((byte)Code);

            return true;
        }
    }
}
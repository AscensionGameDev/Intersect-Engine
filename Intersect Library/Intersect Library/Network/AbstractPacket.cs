using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public abstract class AbstractPacket : IPacket
    {
        protected AbstractPacket(IConnection connection, PacketCode code)
        {
            Connection = connection;
            Code = code;
        }

        public IConnection Connection { get; }

        public double Timestamp { get; set; }

        public virtual int EstimatedSize { get; }

        public PacketType Type => PacketType.Of(this);

        public PacketCode Code { get; }

        public virtual bool Read(ref IBuffer buffer) => (buffer != null);

        public virtual bool Write(ref IBuffer buffer)
        {
            buffer.Write((byte) Code);

            return true;
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using Intersect.Memory;

namespace Intersect.Network
{
    public abstract class AbstractPacket : IPacket
    {
        public IConnection Connection { get; }

        public PacketType Type => PacketType.Of(this);

        public PacketCodes Code { get; }

        protected AbstractPacket(IConnection connection, PacketCodes code)
        {
            Connection = connection;
            Code = code;
        }

        protected virtual IBuffer CreateNewMessage()
        {
            var buffer = Connection?.CreateBuffer();

            return WriteHeader(ref buffer);
        }

        protected virtual IBuffer WriteHeader(ref IBuffer buffer)
        {
            if (TryWriteHeader(ref buffer)) return buffer;
            throw new ArgumentNullException();
        }

        protected virtual bool TryWriteHeader(ref IBuffer buffer)
        {
            if (buffer == null) return false;

            buffer.Write((ushort)0);
            buffer.Write((byte)Code);

            return true;
        }

        public virtual bool Read(ref IBuffer buffer) => (buffer != null);

        public virtual bool Write(ref IBuffer buffer)
        {
            if (buffer == null) buffer = CreateNewMessage();

            buffer.Write((byte)Code);

            return true;
        }
    }
}
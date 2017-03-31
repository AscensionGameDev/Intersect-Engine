using System;
using Lidgren.Network;

namespace Intersect.Network
{
    public abstract class AbstractPacket : IPacket
    {
        public NetConnection Connection { get; }

        public PacketGroups Group { get; }

        protected AbstractPacket(NetConnection connection, PacketGroups group)
        {
            Connection = connection;
            Group = group;
        }

        protected virtual NetOutgoingMessage CreateNewMessage()
        {
            var message = Connection?.Peer?.CreateMessage();

            return WriteHeader(ref message);
        }

        protected virtual NetOutgoingMessage WriteHeader(ref NetOutgoingMessage message)
        {
            if (!TryWriteHeader(ref message)) throw new ArgumentNullException();
            
            return message;
        }

        protected virtual bool TryWriteHeader(ref NetOutgoingMessage message)
        {
            if (message == null) return false;

            message.Write((ushort)0);
            message.Write((byte)Group);

            return true;
        }

        public virtual bool Read(ref NetIncomingMessage message) => (message != null);

        public virtual bool Write(ref NetOutgoingMessage message)
        {
            if (message == null) message = CreateNewMessage();
            return true;
        }
    }
}
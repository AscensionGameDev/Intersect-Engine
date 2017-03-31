using Lidgren.Network;

namespace Intersect.Network.Packets.Utility
{
    public class ServerConfigurationPacket : AbstractUtilityPacket
    {
        private byte[] mData;

        public byte[] Data => mData;

        public ServerConfigurationPacket(NetConnection connection)
            : base(connection, UtilityPackets.Configuration)
        {
        }

        public override bool Read(ref NetIncomingMessage message)
            => message.ReadBytes(message.ReadInt32(), out mData);

        public override bool Write(ref NetOutgoingMessage message)
        {
            message.Write(mData?.Length ?? 0);

            if (mData != null && mData.Length > 0)
                message.Write(mData);

            return true;
        }
    }
}
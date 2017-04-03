using Intersect.Memory;

namespace Intersect.Network.Packets.Utility
{
    public class ServerConfigurationPacket : AbstractUtilityPacket
    {
        private byte[] mData;

        public byte[] Data => mData;

        public ServerConfigurationPacket(IConnection connection)
            : base(connection, UtilityPackets.Configuration)
        {
        }

        public override bool Read(ref IBuffer message)
            => message.Read(out mData);

        public override bool Write(ref IBuffer message)
        {
            message.Write(mData);

            return true;
        }
    }
}
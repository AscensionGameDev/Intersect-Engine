using System;
using Lidgren.Network;

namespace Intersect.Network.Packets.Utility
{
    public abstract class AbstractUtilityPacket : AbstractExtendedPacket<UtilityPackets>
    {
        protected AbstractUtilityPacket(NetConnection connection, UtilityPackets type)
            : base(connection, PacketGroups.Utility, type)
        {
        }
    }
}
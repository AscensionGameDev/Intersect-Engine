using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ActionMsgPackets : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ActionMsgPackets()
        {
        }

        public ActionMsgPackets(ActionMsgPacket[] packets)
        {
            Packets = packets;
        }

        [Key(0)]
        public ActionMsgPacket[] Packets { get; set; }

    }

}

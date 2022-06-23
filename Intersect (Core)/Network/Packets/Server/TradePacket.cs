using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class TradePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TradePacket()
        {
        }

        public TradePacket(string partnerName)
        {
            TradePartner = partnerName;
        }

        [Key(0)]
        public string TradePartner { get; set; }

    }

}

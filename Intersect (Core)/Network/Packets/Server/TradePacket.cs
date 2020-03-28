namespace Intersect.Network.Packets.Server
{

    public class TradePacket : CerasPacket
    {

        public TradePacket(string partnerName)
        {
            TradePartner = partnerName;
        }

        public string TradePartner { get; set; }

    }

}

namespace Intersect.Network.Packets.Server
{

    public class TimeDataPacket : CerasPacket
    {

        public TimeDataPacket(string timeJson)
        {
            TimeJson = timeJson;
        }

        public string TimeJson { get; set; }

    }

}

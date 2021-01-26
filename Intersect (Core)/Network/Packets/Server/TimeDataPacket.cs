using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class TimeDataPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public TimeDataPacket()
        {
        }

        public TimeDataPacket(string timeJson)
        {
            TimeJson = timeJson;
        }

        [Key(0)]
        public string TimeJson { get; set; }

    }

}

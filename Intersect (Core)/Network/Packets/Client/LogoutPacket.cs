using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class LogoutPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public LogoutPacket()
        {
        }

        public LogoutPacket(bool returnToCharSelect)
        {
            ReturningToCharSelect = returnToCharSelect;
        }

        [Key(0)]
        public bool ReturningToCharSelect { get; set; }

    }

}

namespace Intersect.Network.Packets.Client
{

    public class LogoutPacket : CerasPacket
    {

        public LogoutPacket(bool returnToCharSelect)
        {
            ReturningToCharSelect = returnToCharSelect;
        }

        public bool ReturningToCharSelect { get; set; }

    }

}

namespace Intersect.Network.Packets.Client
{
    public class LogoutPacket : CerasPacket
    {
        public bool ReturningToCharSelect { get; set; }

        public LogoutPacket(bool returnToCharSelect)
        {
            ReturningToCharSelect = returnToCharSelect;
        }
    }
}

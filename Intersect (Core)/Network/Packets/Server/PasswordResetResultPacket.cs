namespace Intersect.Network.Packets.Server
{

    public class PasswordResetResultPacket : CerasPacket
    {

        public PasswordResetResultPacket(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public bool Succeeded { get; set; }

    }

}

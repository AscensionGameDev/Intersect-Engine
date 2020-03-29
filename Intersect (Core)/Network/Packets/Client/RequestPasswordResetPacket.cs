namespace Intersect.Network.Packets.Client
{

    public class RequestPasswordResetPacket : CerasPacket
    {

        public RequestPasswordResetPacket(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }

        public string NameOrEmail { get; set; }

    }

}

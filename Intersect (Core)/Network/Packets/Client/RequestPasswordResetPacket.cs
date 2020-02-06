namespace Intersect.Network.Packets.Client
{
    public class RequestPasswordResetPacket : CerasPacket
    {
        public string NameOrEmail { get; set; }

        public RequestPasswordResetPacket(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }
    }
}

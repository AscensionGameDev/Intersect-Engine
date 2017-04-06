using Intersect.Memory;

namespace Intersect.Network.Packets.Authentication
{
    public class LoginPacket : AbstractAuthenticationPacket
    {
        public string Username;
        public string Hashword;

        public LoginPacket(IConnection connection)
            : base(connection, AuthenticationPackets.Login)
        {
            Username = null;
            Hashword = null;
        }

        public override bool Read(ref IBuffer buffer)
        {
            if (!base.Read(ref buffer)) return false;

            buffer.Read(out Username);
            buffer.Read(out Hashword);

            return true;
        }

        public override bool Write(ref IBuffer buffer)
        {
            if (!base.Write(ref buffer)) return false;

            buffer.Write(Username);
            buffer.Write(Hashword);

            return true;
        }
    }
}
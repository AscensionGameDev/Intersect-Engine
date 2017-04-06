using Intersect.Memory;

namespace Intersect.Network.Packets.Authentication
{
    public class LoginResponsePacket : AbstractAuthenticationPacket
    {
        public bool Success;
        public string Message;

        public LoginResponsePacket(IConnection connection)
            : base(connection, AuthenticationPackets.Login)
        {
            Success = false;
            Message = null;
        }

        public override bool Read(ref IBuffer buffer)
        {
            if (!base.Read(ref buffer)) return false;

            buffer.Read(out Success);

            if (!Success)
            {
                buffer.Read(out Message);
            }

            return true;
        }

        public override bool Write(ref IBuffer buffer)
        {
            if (!base.Write(ref buffer)) return false;

            buffer.Write(Success);

            if (!Success)
            {
                buffer.Write(Message);
            }

            return true;
        }
    }
}
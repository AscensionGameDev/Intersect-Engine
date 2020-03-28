namespace Intersect.Network.Packets.Client
{

    public class LoginPacket : CerasPacket
    {

        public LoginPacket(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }

        public string Password { get; set; }

    }

}

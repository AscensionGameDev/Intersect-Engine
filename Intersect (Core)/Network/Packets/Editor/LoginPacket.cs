namespace Intersect.Network.Packets.Editor
{

    //We inherit from CerasPacket here instead of EditorPacket because admin rights are not loaded until you successfully login.
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

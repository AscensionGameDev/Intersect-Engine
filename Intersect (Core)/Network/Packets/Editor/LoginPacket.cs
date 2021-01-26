using MessagePack;

namespace Intersect.Network.Packets.Editor
{

    //We inherit from CerasPacket here instead of EditorPacket because admin rights are not loaded until you successfully login.
    [MessagePackObject]
    public class LoginPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public LoginPacket()
        {
        }

        public LoginPacket(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [Key(0)]
        public string Username { get; set; }

        [Key(1)]
        public string Password { get; set; }

    }

}

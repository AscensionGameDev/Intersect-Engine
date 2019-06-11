using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    //We inherit from CerasPacket here instead of EditorPacket because admin rights are not loaded until you successfully login.
    public class LoginPacket : CerasPacket
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginPacket(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

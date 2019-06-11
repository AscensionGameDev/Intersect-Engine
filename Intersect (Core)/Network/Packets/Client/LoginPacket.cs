using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class CreateAccountPacket : CerasPacket
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public CreateAccountPacket(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }
    }
}

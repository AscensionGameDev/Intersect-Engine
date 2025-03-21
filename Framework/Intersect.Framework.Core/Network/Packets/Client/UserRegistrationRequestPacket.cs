﻿using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class UserRegistrationRequestPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UserRegistrationRequestPacket()
    {
    }

    public UserRegistrationRequestPacket(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }

    [Key(0)]
    public string Username { get; set; }

    [Key(1)]
    public string Password { get; set; }

    [Key(2)]
    public string Email { get; set; }

}

using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class CharactersPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public CharactersPacket()
    {
    }

    public CharactersPacket(string username, CharacterPacket[] characters, bool freeSlot)
    {
        Username = username;
        Characters = characters;
        FreeSlot = freeSlot;
    }

    [Key(0)]
    public string Username { get; set; }

    [Key(1)]
    public CharacterPacket[] Characters { get; set; }

    [Key(2)]
    public bool FreeSlot { get; set; }

}

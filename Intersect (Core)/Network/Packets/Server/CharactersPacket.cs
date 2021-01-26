using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class CharactersPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CharactersPacket()
        {
        }

        public CharactersPacket(CharacterPacket[] characters, bool freeSlot)
        {
            Characters = characters;
            FreeSlot = freeSlot;
        }

        [Key(0)]
        public CharacterPacket[] Characters { get; set; }

        [Key(1)]
        public bool FreeSlot { get; set; }

    }

}

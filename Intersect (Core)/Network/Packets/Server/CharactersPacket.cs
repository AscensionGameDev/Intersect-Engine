namespace Intersect.Network.Packets.Server
{

    public class CharactersPacket : CerasPacket
    {

        public CharactersPacket(CharacterPacket[] characters, bool freeSlot)
        {
            Characters = characters;
            FreeSlot = freeSlot;
        }

        public CharacterPacket[] Characters { get; set; }

        public bool FreeSlot { get; set; }

    }

}

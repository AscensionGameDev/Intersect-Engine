using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class AddTilesetsPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public AddTilesetsPacket()
        {
        }

        public AddTilesetsPacket(string[] tilesets)
        {
            Tilesets = tilesets;
        }

        [Key(0)]
        public string[] Tilesets { get; set; }

    }

}

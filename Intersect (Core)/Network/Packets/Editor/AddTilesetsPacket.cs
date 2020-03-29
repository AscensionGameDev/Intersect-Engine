namespace Intersect.Network.Packets.Editor
{

    public class AddTilesetsPacket : EditorPacket
    {

        public AddTilesetsPacket(string[] tilesets)
        {
            Tilesets = tilesets;
        }

        public string[] Tilesets { get; set; }

    }

}

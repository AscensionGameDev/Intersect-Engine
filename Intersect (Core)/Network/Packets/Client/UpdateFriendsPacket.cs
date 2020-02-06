namespace Intersect.Network.Packets.Client
{
    public class UpdateFriendsPacket : CerasPacket
    {
        public string Name { get; set; }
        public bool Adding { get; set; }

        public UpdateFriendsPacket(string name, bool adding)
        {
            Name = name;
            Adding = adding;
        }
    }
}

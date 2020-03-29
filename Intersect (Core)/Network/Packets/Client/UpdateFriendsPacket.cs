namespace Intersect.Network.Packets.Client
{

    public class UpdateFriendsPacket : CerasPacket
    {

        public UpdateFriendsPacket(string name, bool adding)
        {
            Name = name;
            Adding = adding;
        }

        public string Name { get; set; }

        public bool Adding { get; set; }

    }

}

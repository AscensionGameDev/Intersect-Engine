using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UpdateFriendsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UpdateFriendsPacket()
        {
        }

        public UpdateFriendsPacket(string name, bool adding)
        {
            Name = name;
            Adding = adding;
        }

        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public bool Adding { get; set; }

    }

}

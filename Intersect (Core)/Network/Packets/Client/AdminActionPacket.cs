using Intersect.Admin.Actions;
using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class AdminActionPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public AdminActionPacket()
        {

        }

        public AdminActionPacket(AdminAction action)
        {
            Action = action;
        }

        [Key(0)]
        public AdminAction Action { get; set; }

    }

}

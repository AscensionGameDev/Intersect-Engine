using Intersect.Admin.Actions;

namespace Intersect.Network.Packets.Client
{

    public class AdminActionPacket : CerasPacket
    {

        public AdminActionPacket(AdminAction action)
        {
            Action = action;
        }

        public AdminAction Action { get; set; }

    }

}

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{

    public class OpenEditorPacket : CerasPacket
    {

        public OpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }

        public GameObjectType Type { get; set; }

    }

}

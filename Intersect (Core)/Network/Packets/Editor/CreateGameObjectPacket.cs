using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{

    public class CreateGameObjectPacket : EditorPacket
    {

        public CreateGameObjectPacket(GameObjectType type)
        {
            Type = type;
        }

        public GameObjectType Type { get; set; }

    }

}

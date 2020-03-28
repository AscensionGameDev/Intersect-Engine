using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{

    public class RequestOpenEditorPacket : EditorPacket
    {

        public RequestOpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }

        public GameObjectType Type { get; set; }

    }

}

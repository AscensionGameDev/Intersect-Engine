using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class RequestOpenEditorPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public RequestOpenEditorPacket()
        {
        }

        public RequestOpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }

        [Key(0)]
        public GameObjectType Type { get; set; }

    }

}

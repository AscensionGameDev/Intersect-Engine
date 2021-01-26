using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class CreateGameObjectPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public CreateGameObjectPacket()
        {
        }

        public CreateGameObjectPacket(GameObjectType type)
        {
            Type = type;
        }

        [Key(0)]
        public GameObjectType Type { get; set; }

    }

}

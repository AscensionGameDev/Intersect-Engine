using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class OpenEditorPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public OpenEditorPacket()
        {
        }

        public OpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }

        [Key(0)]
        public GameObjectType Type { get; set; }

    }

}

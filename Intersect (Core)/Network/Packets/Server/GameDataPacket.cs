using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class GameDataPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public GameDataPacket()
        {
        }

        public GameDataPacket(GameObjectPacket[] gameObjects, string colorsJson)
        {
            GameObjects = gameObjects;
            ColorsJson = colorsJson;
        }

        [Key(0)]
        public GameObjectPacket[] GameObjects { get; set; }

        [Key(1)]
        public string ColorsJson { get; set; }

    }

}

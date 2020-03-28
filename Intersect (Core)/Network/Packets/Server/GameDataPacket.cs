namespace Intersect.Network.Packets.Server
{

    public class GameDataPacket : CerasPacket
    {

        public GameDataPacket(GameObjectPacket[] gameObjects, string colorsJson)
        {
            GameObjects = gameObjects;
            ColorsJson = colorsJson;
        }

        public GameObjectPacket[] GameObjects { get; set; }

        public string ColorsJson { get; set; }

    }

}

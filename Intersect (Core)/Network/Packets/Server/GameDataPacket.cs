using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class GameDataPacket : CerasPacket
    {
        public GameObjectPacket[] GameObjects { get; set; }
        public string ColorsJson { get; set; }

        public GameDataPacket(GameObjectPacket[] gameObjects, string colorsJson)
        {
            GameObjects = gameObjects;
            ColorsJson = colorsJson;
        }
    }
}

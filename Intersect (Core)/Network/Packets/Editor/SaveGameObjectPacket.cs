using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{
    public class SaveGameObjectPacket : EditorPacket
    {
        public GameObjectType Type { get; set; }
        public Guid Id { get; set; }
        public string Data { get; set; }

        public SaveGameObjectPacket(GameObjectType type, Guid id, string data)
        {
            Type = type;
            Id = id;
            Data = data;
        }
    }
}

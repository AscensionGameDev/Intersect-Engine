using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{
    public class DeleteGameObjectPacket : EditorPacket
    {
        public GameObjectType Type { get; set; }
        public Guid Id { get; set; }

        public DeleteGameObjectPacket(GameObjectType type, Guid id)
        {
            Type = type;
            Id = id;
        }
    }
}

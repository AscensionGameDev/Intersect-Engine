using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class GameObjectPacket : CerasPacket
    {
        public Guid Id { get; set; }
        public GameObjectType Type { get; set; }
        public bool AnotherFollowing { get; set; }
        public bool Deleted { get; set; }
        public string Data { get; set; }

        public GameObjectPacket(Guid id, GameObjectType type, string data, bool deleted, bool another)
        {
            Id = id;
            Type = type;
            Data = data;
            Deleted = deleted;
            AnotherFollowing = another;
        }
    }
}

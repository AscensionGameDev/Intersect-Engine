using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class OpenEditorPacket : CerasPacket
    {
        public GameObjectType Type { get; set; }

        public OpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }
    }
}

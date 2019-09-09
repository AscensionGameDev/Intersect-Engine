using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Editor
{
    public class RequestOpenEditorPacket : EditorPacket
    {
        public GameObjectType Type { get; set; }

        public RequestOpenEditorPacket(GameObjectType type)
        {
            Type = type;
        }
    }
}

using System;

namespace Intersect.Network.Packets.Server
{

    public class CustomSpriteLayersPacket : CerasPacket
    {

        public CustomSpriteLayersPacket(Guid entityId, string[] customSpriteLayers)
        {
            EntityId = entityId;
            CustomSpriteLayers = customSpriteLayers;
        }

        public string[] CustomSpriteLayers { get; set; }

        public Guid EntityId { get; set; }
    }
}

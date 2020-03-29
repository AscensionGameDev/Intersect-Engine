using Intersect.GameObjects.Events;

namespace Intersect.Network.Packets.Server
{

    public class EventEntityPacket : EntityPacket
    {

        public bool DirectionFix { get; set; }

        public bool WalkingAnim { get; set; }

        public bool DisablePreview { get; set; }

        public string Description { get; set; }

        public EventGraphic Graphic { get; set; }

        public byte RenderLayer { get; set; }

    }

}

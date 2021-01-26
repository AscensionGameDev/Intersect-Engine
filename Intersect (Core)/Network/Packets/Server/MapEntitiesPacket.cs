using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapEntitiesPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapEntitiesPacket()
        {
        }

        public MapEntitiesPacket(EntityPacket[] mapEntities)
        {
            MapEntities = mapEntities;
        }

        [Key(0)]
        public EntityPacket[] MapEntities { get; set; }

    }

}

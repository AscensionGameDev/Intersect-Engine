namespace Intersect.Network.Packets.Server
{

    public class MapEntitiesPacket : CerasPacket
    {

        public MapEntitiesPacket(EntityPacket[] mapEntities)
        {
            MapEntities = mapEntities;
        }

        public EntityPacket[] MapEntities { get; set; }

    }

}

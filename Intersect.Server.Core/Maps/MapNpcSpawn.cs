using Intersect.Server.Entities;

namespace Intersect.Server.Maps
{
    public partial class MapNpcSpawn
    {
        public Npc Entity { get; set; }

        public long RespawnTime { get; set; } = -1;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect_Server.Classes.Entities;

namespace Intersect_Server.Classes.Maps
{
    public class MapResourceSpawnInstance
    {
        public int EntityIndex = -1;
        public Resource Entity;
        public long RespawnTime = -1;
    }
}

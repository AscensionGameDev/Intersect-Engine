using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.GameObjects.Maps
{
    public class MapLayerBase
    {
        public int ID { get; set; }

        public int MapLayerRegionID { get; set; }

        public int IntersectLayerID { get; set; }

        public int OldLayerID { get; set; }
    }
}

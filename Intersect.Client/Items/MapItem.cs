using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Intersect.Client.Items
{
    public class MapItemInstance : Item
    {
        public int X;
        public int Y;

        public MapItemInstance() : base()
        {
        }

        public MapItemInstance(string data) : base()
        {
            JsonConvert.PopulateObject(data, this);
        }
    }
}

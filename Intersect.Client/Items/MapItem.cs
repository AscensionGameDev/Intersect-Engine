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

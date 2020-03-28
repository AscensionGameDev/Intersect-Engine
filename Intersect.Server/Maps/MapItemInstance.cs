using System;

using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{

    public class MapItem : Item
    {

        [JsonIgnore] public int AttributeSpawnX = -1;

        [JsonIgnore] public int AttributeSpawnY = -1;

        [JsonIgnore] public long DespawnTime;

        public int X = 0;

        public int Y = 0;

        public MapItem(Guid itemId, int quantity) : base(itemId, quantity, null, null)
        {
        }

        public MapItem(Guid itemId, int quantity, Guid? bagId, Bag bag) : base(itemId, quantity, bagId, bag)
        {
        }

        public string Data()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}

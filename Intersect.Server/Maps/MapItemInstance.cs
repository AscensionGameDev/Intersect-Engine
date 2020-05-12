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

        public Guid Owner;

        [JsonIgnore] public long OwnershipTime;

        // We need this mostly for the client-side.. They can't keep track of our timer after all!
        public bool VisibleToAll = true;

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

        /// <summary>
        /// Sets up the Stat Buffs on this map item from a supplied item.
        /// </summary>
        /// <param name="item">The item to take the Stat Buffs from and apply them to this MapItem.</param>
        public void SetupStatBuffs(Item item)
        {
            if (StatBuffs != null && item.StatBuffs != null)
            {
                for (var i = 0; i < StatBuffs.Length; ++i)
                {
                    StatBuffs[i] = item.StatBuffs.Length > i ? item.StatBuffs[i] : 0;
                }
            }
        }

    }

}

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.General;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Server.Database
{

    public class Item
    {

        [JsonIgnore, NotMapped] public double DropChance = 100;

        public Item()
        {
        }

        public Item(Guid itemId, int quantity, bool incStatBuffs = true) : this(
            itemId, quantity, null, null, incStatBuffs
        )
        {
        }

        public Item(Guid itemId, int quantity, Guid? bagId, Bag bag, bool includeStatBuffs = true)
        {
            ItemId = itemId;
            Quantity = quantity;
            BagId = bagId;
            Bag = bag;

            var descriptor = ItemBase.Get(ItemId);
            if (descriptor == null || !includeStatBuffs)
            {
                return;
            }

            if (descriptor.ItemType != ItemTypes.Equipment)
            {
                return;
            }

            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                // TODO: What the fuck?
                StatBuffs[i] = Randomization.Next(
                    -descriptor.StatGrowth,
                    descriptor.StatGrowth + 1
                );
            }
        }

        public Item(Item item) : this(item.ItemId, item.Quantity, item.BagId, item.Bag)
        {
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                StatBuffs[i] = item.StatBuffs[i];
            }
        }

        public Guid? BagId { get; set; }

        [JsonIgnore]
        public virtual Bag Bag { get; set; }

        public Guid ItemId { get; set; } = Guid.Empty;

        public int Quantity { get; set; }

        [Column("StatBuffs")]
        [JsonIgnore]
        public string StatBuffsJson
        {
            get => DatabaseUtils.SaveIntArray(StatBuffs, (int) Enums.Stats.StatCount);
            set => StatBuffs = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] StatBuffs { get; set; } = new int[(int) Enums.Stats.StatCount];

        [JsonIgnore, NotMapped]
        public ItemBase Descriptor => ItemBase.Get(ItemId);

        public static Item None => new Item();

        public virtual void Set(Item item)
        {
            ItemId = item.ItemId;
            Quantity = item.Quantity;
            BagId = item.BagId;
            Bag = item.Bag;
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                StatBuffs[i] = item.StatBuffs[i];
            }
        }

        public string Data()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Item Clone()
        {
            return new Item(this);
        }

    }

}

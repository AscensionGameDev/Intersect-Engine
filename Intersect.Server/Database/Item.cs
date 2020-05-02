using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public Item(Guid itemId, int quantity, Guid? bagId, Bag bag, bool incStatBuffs = true)
        {
            ItemId = itemId;
            Quantity = quantity;
            BagId = bagId;
            Bag = bag;
			if (Tags == null)
			{
				Tags = new Dictionary<string, int>();
			}

			ItemBase IB = ItemBase.Get(ItemId);
			if (IB != null)
            {
                if (IB.ItemType == ItemTypes.Equipment)
				{
					if (incStatBuffs)
					{
						for (var i = 0; i < (int)Stats.StatCount; i++)
						{
							// TODO: What the fuck?
							StatBuffs[i] = Globals.Rand.Next(
								-1 * IB.StatGrowth, IB.StatGrowth + 1
							);
						}
					}
					int totalTag = IB.tags.Count;
					if (totalTag > 0)
					{
						int nbTag = 1;
						if (IB.tags.ContainsKey("tagcount"))
						{
							totalTag -= 1;
							TagStat tag = IB.tags["tagcount"];
							nbTag = Math.Min(tag.RandomValue, totalTag);
						}
						int tryTag = 5;
						while (nbTag > 0 && tryTag > 0)
						{
							int index = Globals.Rand.Next(IB.tags.Count);
							KeyValuePair<string, TagStat> pair = IB.tags.ElementAt<KeyValuePair<string, TagStat>>(index);
							if (pair.Key != "tagcount")
							{
								if (Tags.ContainsKey(pair.Key))
								{
									tryTag -= 1;
								}
								else
								{
									Tags.Add(pair.Key, pair.Value.RandomValue);
									nbTag -= 1;
									tryTag = 5;
								}
							}
							else
							{
								tryTag -= 1;
							}
						}
					}
                }
            }
        }

        public Item(Item item) : this(item.ItemId, item.Quantity, item.BagId, item.Bag)
        {
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                StatBuffs[i] = item.StatBuffs[i];
            }
			foreach (KeyValuePair<string, int> tag in item.Tags)
			{
				Tags.Add(tag.Key, tag.Value);
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


		[NotMapped]
		public Dictionary<string, int> Tags = new Dictionary<string, int>();

		[Column("Tags")]
		[JsonIgnore]
		public string JsonTags
		{
			get => JsonConvert.SerializeObject(Tags);
			set
			{
				if (value == null)
					Tags = new Dictionary<string, int>();
				else
					Tags = JsonConvert.DeserializeObject<Dictionary<string, int>>(value);
			}
		}

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
			if (item.Tags != null)
			{
				Tags = new Dictionary<string, int>();
				foreach (KeyValuePair<string, int> tag in item.Tags)
				{
					//Console.WriteLine($"{tag.Key} => {tag.Value}");
					if (string.IsNullOrEmpty(tag.Key) == false)
					{
						Tags.Add(tag.Key, tag.Value);
					}
				}
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

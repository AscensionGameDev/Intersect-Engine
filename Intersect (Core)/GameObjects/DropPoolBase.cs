using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
	public class DropPoolBase : DatabaseObject<DropPoolBase>
	{
		[JsonConstructor]
		public DropPoolBase(Guid id) : base(id)
		{
			Name = "DropPool";
		}

		public DropPoolBase()
		{
			Name = "DropPool";
		}

		[NotMapped]
		public List<ItemPool> ItemPool { get; set; } = new List<ItemPool>();

		[Column("ItemListed")]
		[JsonIgnore]
		public string JsonPool
		{
			get => JsonConvert.SerializeObject(ItemPool);
			set
			{
				if (value == null)
					ItemPool = new List<ItemPool>();
				else
					ItemPool = JsonConvert.DeserializeObject<List<ItemPool>>(value);
			}
		}
	}



	public class ItemPool
	{
		public Guid ItemId;
		public int Quantity;
		public double Chance;

	}
}

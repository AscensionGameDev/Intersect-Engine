using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
	public class HDVBase : DatabaseObject<HDVBase>, IFolderable
	{
		[JsonConstructor]
		public HDVBase(Guid id) : base(id)
		{
			Name = "HDV";
		}
		
		public HDVBase()
		{
			Name = "HDV";
		}
				
		[Column("Currency")]
		[JsonProperty]
		public Guid CurrencyId { get; set; }

		[NotMapped]
		[JsonIgnore]
		public ItemBase Currency
		{
			get => ItemBase.Get(CurrencyId);
			set => CurrencyId = value?.Id ?? Guid.Empty;
		}

		public bool isWhiteList { get; set; } = false;

		[NotMapped] public List<Guid> ItemListed = new List<Guid>();

		[Column("ItemListed")]
		[JsonIgnore]
		public string JsonItemListed
		{
			get => JsonConvert.SerializeObject(ItemListed);
			set
			{
				if (value == null)
				{
					ItemListed = new List<Guid>();
				}
				else
				{
					ItemListed = JsonConvert.DeserializeObject<List<Guid>>(value);
				}
			}
		}

		/// <inheritdoc />
		public string Folder { get; set; } = "";

	}
}

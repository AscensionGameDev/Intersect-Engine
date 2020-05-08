using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class BankSlot : Item, ISlot, IPlayerOwned
    {

        public BankSlot()
        {
        }

        public BankSlot(int slot)
        {
            Slot = slot;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
        public Guid Id { get; private set; }

        [JsonIgnore]
        public Guid PlayerId { get; private set; }

        [JsonIgnore]
        public virtual Player Player { get; private set; }

        public int Slot { get; private set; }

		[NotMapped, JsonIgnore]
		public Dictionary<string, int> tags = new Dictionary<string, int>();

		[Column("Tags")]
		public string JsonTags
		{
			get => JsonConvert.SerializeObject(tags);
			set
			{
				if (value == null)
					tags = new Dictionary<string, int>();
				else
					tags = JsonConvert.DeserializeObject<Dictionary<string, int>>(value);
			}
		}
		[NotMapped]
		public Dictionary<string, string> StringTags = new Dictionary<string, string>();

		[Column("StringTags")]
		[JsonIgnore]
		public string JsonStringTags
		{
			get => JsonConvert.SerializeObject(StringTags);
			set
			{
				if (value == null)
					StringTags = new Dictionary<string, string>();
				else
					StringTags = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
			}
		}
	}

}

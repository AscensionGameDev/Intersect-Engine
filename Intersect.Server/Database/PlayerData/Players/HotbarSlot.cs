using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.Server.Entities;
using Intersect.Utilities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class HotbarSlot : ISlot, IPlayerOwned
    {

        public HotbarSlot()
        {
        }

        public HotbarSlot(int slot)
        {
            Slot = slot;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public Guid Id { get; private set; }

        public Guid ItemOrSpellId { get; set; } = Guid.Empty;

        public Guid BagId { get; set; } = Guid.Empty;

        [Column("PreferredStatBuffs")]
        [JsonIgnore]
        public string StatBuffsJson
        {
            get => DatabaseUtils.SaveIntArray(PreferredStatBuffs, (int) Enums.Stats.StatCount);
            set => PreferredStatBuffs = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] PreferredStatBuffs { get; set; } = new int[(int) Stats.StatCount];

        [JsonIgnore]
        public Guid PlayerId { get; private set; }

        [JsonIgnore]
        public virtual Player Player { get; private set; }

        [JsonIgnore]
        public int Slot { get; private set; }

        public string Data()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}

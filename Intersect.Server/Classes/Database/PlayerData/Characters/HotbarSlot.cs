using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Server.Entities;
using Intersect.Utilities;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class HotbarSlot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        [JsonIgnore] public virtual Player Character { get; private set; }
        public int Index { get; private set; }
        public Guid ItemOrSpellId { get; set; } = Guid.Empty;
        public Guid BagId { get; set; } = Guid.Empty;
        
        [Column("PreferredStatBuffs")]
        public string StatBuffsJson
        {
            get => DatabaseUtils.SaveIntArray(PreferredStatBuffs, (int)Enums.Stats.StatCount);
            set => PreferredStatBuffs = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] PreferredStatBuffs { get; set; } = new int[(int)Stats.StatCount];

        public HotbarSlot()
        {
            
        }

        public HotbarSlot(int index)
        {
            Index = index;
        }
    }
}

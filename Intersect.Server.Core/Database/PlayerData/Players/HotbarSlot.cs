using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Collections.Slotting;
using Intersect.Enums;
using Intersect.Server.Entities;
using Intersect.Utilities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players;


public partial class HotbarSlot : ISlot, IPlayerOwned
{
    public static HotbarSlot Create(int slotIndex) => new(slotIndex);

    public HotbarSlot()
    {
    }

    public HotbarSlot(int slot)
    {
        Slot = slot;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
    public Guid Id { get; private set; }

    [JsonIgnore]
    public bool IsEmpty => ItemOrSpellId == default;

    public Guid ItemOrSpellId { get; set; } = Guid.Empty;

    public Guid BagId { get; set; } = Guid.Empty;

    [Column("PreferredStatBuffs")]
    [JsonIgnore]
    public string StatBuffsJson
    {
        get => DatabaseUtils.SaveIntArray(PreferredStatBuffs, Enum.GetValues<Stat>().Length);
        set => PreferredStatBuffs = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
    }

    [NotMapped]
    public int[] PreferredStatBuffs { get; set; } = new int[Enum.GetValues<Stat>().Length];

    [JsonIgnore]
    public Guid PlayerId { get; private set; }

    [JsonIgnore]
    [ForeignKey(nameof(PlayerId))]
    public virtual Player Player { get; private set; }

    [JsonIgnore]
    public int Slot { get; private set; }

    public string Data()
    {
        return JsonConvert.SerializeObject(this);
    }

}

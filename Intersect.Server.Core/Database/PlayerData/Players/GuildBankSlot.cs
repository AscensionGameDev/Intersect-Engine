using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Collections.Slotting;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players;


public partial class GuildBankSlot : Item, ISlot
{
    public static GuildBankSlot Create(int slotIndex) => new(slotIndex);

    public GuildBankSlot()
    {
    }

    public GuildBankSlot(int slot)
    {
        Slot = slot;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
    public Guid Id { get; private set; }

    public bool IsEmpty => ItemId == default;

    [JsonIgnore]
    public Guid GuildId { get; private set; }

    [JsonIgnore]
    [ForeignKey(nameof(GuildId))]
    public virtual Guild Guild { get; private set; }

    public int Slot { get; private set; }
}

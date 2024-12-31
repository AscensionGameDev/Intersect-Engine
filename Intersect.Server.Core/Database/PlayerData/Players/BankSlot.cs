using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Collections.Slotting;
using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players;


public partial class BankSlot : Item, ISlot, IPlayerOwned
{
    public static BankSlot Create(int slotIndex) => new(slotIndex);

    public BankSlot()
    {
    }

    public BankSlot(int slot)
    {
        Slot = slot;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
    public Guid Id { get; private set; }

    public bool IsEmpty => ItemId == default;

    [JsonIgnore]
    public Guid PlayerId { get; private set; }

    [JsonIgnore]
    [ForeignKey(nameof(PlayerId))]
    public virtual Player Player { get; private set; }

    public int Slot { get; private set; }

}

using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Collections.Slotting;
using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players;


public partial class SpellSlot : Spell, ISlot, IPlayerOwned
{
    public static SpellSlot Create(int slotIndex) => new(slotIndex);

    public SpellSlot()
    {
    }

    public SpellSlot(int slot)
    {
        Slot = slot;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
    public Guid Id { get; private set; }

    public bool IsEmpty => SpellId == default;

    [JsonIgnore]
    public Guid PlayerId { get; private set; }

    [JsonIgnore]
    [ForeignKey(nameof(PlayerId))]
    public virtual Player Player { get; private set; }

    public int Slot { get; private set; }

}

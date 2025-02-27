using Intersect.Enums;
using Intersect.GameObjects.Ranges;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Intersect.Framework.Core.GameObjects.Items;

public partial class EquipmentProperties
{
    [JsonIgnore]
    public ItemRange StatRange_Attack
    {
        get => StatRanges.TryGetValue(Stat.Attack, out var range) ? range : StatRange_Attack = new ItemRange();
        set => StatRanges[Stat.Attack] = value;
    }

    [JsonIgnore]
    public ItemRange StatRange_AbilityPower
    {
        get =>
            StatRanges.TryGetValue(Stat.AbilityPower, out var range) ? range : StatRange_AbilityPower = new ItemRange();
        set => StatRanges[Stat.AbilityPower] = value;
    }

    [JsonIgnore]
    public ItemRange StatRange_Defense
    {
        get => StatRanges.TryGetValue(Stat.Defense, out var range) ? range : StatRange_Defense = new ItemRange();
        set => StatRanges[Stat.Defense] = value;
    }

    [JsonIgnore]
    public ItemRange StatRange_MagicResist
    {
        get =>
            StatRanges.TryGetValue(Stat.MagicResist, out var range) ? range : StatRange_MagicResist = new ItemRange();
        set => StatRanges[Stat.MagicResist] = value;
    }

    [JsonIgnore]
    public ItemRange StatRange_Speed
    {
        get => StatRanges.TryGetValue(Stat.Speed, out var range) ? range : StatRange_Speed = new ItemRange();
        set => StatRanges[Stat.Speed] = value;
    }
}
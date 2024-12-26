using Intersect.Enums;
using Intersect.GameObjects.Ranges;

// ReSharper disable InconsistentNaming

namespace Intersect.GameObjects;

public partial class EquipmentProperties
{
    public ItemRange StatRange_Attack
    {
        get => StatRanges.TryGetValue(Stat.Attack, out var range) ? range : StatRange_Attack = new ItemRange();
        set => StatRanges[Stat.Attack] = value;
    }

    public ItemRange StatRange_AbilityPower
    {
        get =>
            StatRanges.TryGetValue(Stat.AbilityPower, out var range) ? range : StatRange_AbilityPower = new ItemRange();
        set => StatRanges[Stat.AbilityPower] = value;
    }

    public ItemRange StatRange_Defense
    {
        get => StatRanges.TryGetValue(Stat.Defense, out var range) ? range : StatRange_Defense = new ItemRange();
        set => StatRanges[Stat.Defense] = value;
    }

    public ItemRange StatRange_MagicResist
    {
        get =>
            StatRanges.TryGetValue(Stat.MagicResist, out var range) ? range : StatRange_MagicResist = new ItemRange();
        set => StatRanges[Stat.MagicResist] = value;
    }

    public ItemRange StatRange_Speed
    {
        get => StatRanges.TryGetValue(Stat.Speed, out var range) ? range : StatRange_Speed = new ItemRange();
        set => StatRanges[Stat.Speed] = value;
    }
}
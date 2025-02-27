using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

[Owned]
public partial class SpellCombatDescriptor
{
    [NotMapped]
    public long[] VitalDiff = new long[Enum.GetValues<Vital>().Length];

    public int CritChance { get; set; }

    public double CritMultiplier { get; set; } = 1.5;

    public int DamageType { get; set; } = 1;

    public int HitRadius { get; set; }

    public bool Friendly { get; set; }

    public int CastRange { get; set; }

    //Extra Data, Teleport Coords, Custom Spells, Etc
    [Column("Projectile")]
    public Guid ProjectileId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public ProjectileDescriptor Projectile
    {
        get => ProjectileDescriptor.Get(ProjectileId);
        set => ProjectileId = value?.Id ?? Guid.Empty;
    }

    //Heal/Damage
    [Column("VitalDiff")]
    [JsonIgnore]
    public string VitalDiffJson
    {
        get => DatabaseUtils.SaveLongArray(VitalDiff, Enum.GetValues<Vital>().Length);
        set => VitalDiff = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
    }

    //Buff/Debuff Data
    [Column("StatDiff")]
    [JsonIgnore]
    public string StatDiffJson
    {
        get => DatabaseUtils.SaveIntArray(StatDiff, Enum.GetValues<Stat>().Length);
        set => StatDiff = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
    }

    [NotMapped]
    public int[] StatDiff { get; set; } = new int[Enum.GetValues<Stat>().Length];

    //Buff/Debuff Data
    [Column("PercentageStatDiff")]
    [JsonIgnore]
    public string PercentageStatDiffJson
    {
        get => DatabaseUtils.SaveIntArray(PercentageStatDiff, Enum.GetValues<Stat>().Length);
        set => PercentageStatDiff = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
    }

    [NotMapped]
    public int[] PercentageStatDiff { get; set; } = new int[Enum.GetValues<Stat>().Length];

    public int Scaling { get; set; } = 0;

    public int ScalingStat { get; set; }

    public SpellTargetType TargetType { get; set; }

    public bool HoTDoT { get; set; }

    public int HotDotInterval { get; set; }

    public int Duration { get; set; }

    public SpellEffect Effect { get; set; }

    public string TransformSprite { get; set; }

    [Column("OnHit")]
    public int OnHitDuration { get; set; }

    [Column("Trap")]
    public int TrapDuration { get; set; }
}
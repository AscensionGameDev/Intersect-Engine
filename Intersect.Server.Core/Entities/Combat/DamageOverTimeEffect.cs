using System.Diagnostics.CodeAnalysis;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.GameObjects;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Entities.Combat;


public partial class DamageOverTimeEffect
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Entity Attacker { get; set; }

    public int Count { get; set; }

    private long mInterval;

    public SpellDescriptor SpellDescriptor { get; }

    public static bool TryCreate(
        Entity attacker,
        Guid spellDescriptorId,
        Entity target,
        [NotNullWhen(true)] out DamageOverTimeEffect? damageOverTimeEffect
    )
    {
        if (SpellDescriptor.TryGet(spellDescriptorId, out var spellDescriptor))
        {
            return TryCreate(attacker, spellDescriptor, target, out damageOverTimeEffect);
        }

        ApplicationContext.CurrentContext.Logger.LogWarning(
            "Skipping creation of DamageOverTimeEffect because SpellDescriptor is missing {SpellDescriptorId}",
            spellDescriptor.Id
        );
        damageOverTimeEffect = null;
        return false;
    }

    public static bool TryCreate(
        Entity attacker,
        SpellDescriptor spellDescriptor,
        Entity target,
        [NotNullWhen(true)] out DamageOverTimeEffect? damageOverTimeEffect
    )
    {
        if (spellDescriptor.Combat.HotDotInterval < 1)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Skipping creation of DamageOverTimeEffect because the Heal/Damage-over-time interval is less than 1 for {SpellDescriptorId} ({SpellName})",
                spellDescriptor.Id,
                spellDescriptor.Name
            );
            damageOverTimeEffect = null;
            return false;
        }

        damageOverTimeEffect = new DamageOverTimeEffect(attacker, spellDescriptor, target);
        return damageOverTimeEffect != null;
    }

    private DamageOverTimeEffect(Entity attacker, SpellDescriptor spellDescriptor, Entity target)
    {
        ArgumentNullException.ThrowIfNull(attacker, nameof(attacker));
        ArgumentNullException.ThrowIfNull(spellDescriptor, nameof(spellDescriptor));
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        
        SpellDescriptor = spellDescriptor;

        Attacker = attacker;
        Target = target;

        // Does target have a cleanse buff? If so, do not allow this DoT when spell is unfriendly.
        if (!SpellDescriptor.Combat.Friendly)
        {
            foreach (var status in Target.CachedStatuses)
            {
                if (status.Type == SpellEffect.Cleanse)
                {
                    return;
                }
            }
        }
        

        mInterval = Timing.Global.Milliseconds + SpellDescriptor.Combat.HotDotInterval;
        // Subtract 1 since the first tick always occurs when the spell is cast.
        Count = (SpellDescriptor.Combat.Duration + SpellDescriptor.Combat.HotDotInterval - 1) / SpellDescriptor.Combat.HotDotInterval;
    }

    public Entity Target { get; }

    public void Expire()
    {
        if (Target != null)
        {
            Target.DamageOverTimeEffects?.TryRemove(Id, out DamageOverTimeEffect val);
            Target.CachedDamageOverTimeEffects = Target.DamageOverTimeEffects?.Values.ToArray() ?? new DamageOverTimeEffect[0];
        }
    }

    public bool CheckExpired()
    {
        if (Target != null && !Target.DamageOverTimeEffects.ContainsKey(Id))
        {
            return false;
        }

        if (SpellDescriptor == null || Count > 0)
        {
            return false;
        }

        Expire();

        return true;
    }

    public void Tick()
    {
        if (CheckExpired())
        {
            return;
        }

        if (mInterval > Timing.Global.Milliseconds)
        {
            return;
        }

        var deadAnimations = new List<KeyValuePair<Guid, Direction>>();
        var aliveAnimations = new List<KeyValuePair<Guid, Direction>>();
        if (SpellDescriptor.TickAnimationId != Guid.Empty)
        {
            var animation = new KeyValuePair<Guid, Direction>(SpellDescriptor.TickAnimationId, Direction.Up);
            deadAnimations.Add(animation);
            aliveAnimations.Add(animation);
        }
        else if (SpellDescriptor.HitAnimationId != Guid.Empty)
        {
            var animation = new KeyValuePair<Guid, Direction>(SpellDescriptor.HitAnimationId, Direction.Up);
            deadAnimations.Add(animation);
            aliveAnimations.Add(animation);
        }

        var damageHealth = SpellDescriptor.Combat.VitalDiff[(int)Vital.Health];
        var damageMana = SpellDescriptor.Combat.VitalDiff[(int)Vital.Mana];

        Attacker?.Attack(
            Target, damageHealth, damageMana,
            (DamageType)SpellDescriptor.Combat.DamageType, (Enums.Stat)SpellDescriptor.Combat.ScalingStat,
            SpellDescriptor.Combat.Scaling, SpellDescriptor.Combat.CritChance, SpellDescriptor.Combat.CritMultiplier, deadAnimations,
            aliveAnimations, false
        );

        mInterval = Timing.Global.Milliseconds + SpellDescriptor.Combat.HotDotInterval;
        Count--;
    }

}

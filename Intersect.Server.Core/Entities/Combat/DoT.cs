using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Server.Entities.Combat;


public partial class DoT
{
    public Guid Id = Guid.NewGuid();

    public Entity Attacker;

    public int Count;

    private long mInterval;

    public SpellDescriptor SpellDescriptor;

    public DoT(Entity attacker, Guid spellId, Entity target)
    {
        SpellDescriptor = SpellDescriptor.Get(spellId);

        Attacker = attacker;
        Target = target;

        if (SpellDescriptor == null || SpellDescriptor.Combat.HotDotInterval < 1)
        {
            return;
        }

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
        Count = (SpellDescriptor.Combat.Duration + SpellDescriptor.Combat.HotDotInterval - 1) / SpellDescriptor.Combat.HotDotInterval;
        target.DoT.TryAdd(Id, this);
        target.CachedDots = target.DoT.Values.ToArray();

        //Subtract 1 since the first tick always occurs when the spell is cast.
    }

    public Entity Target { get; }

    public void Expire()
    {
        if (Target != null)
        {
            Target.DoT?.TryRemove(Id, out DoT val);
            Target.CachedDots = Target.DoT?.Values.ToArray() ?? new DoT[0];
        }
    }

    public bool CheckExpired()
    {
        if (Target != null && !Target.DoT.ContainsKey(Id))
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

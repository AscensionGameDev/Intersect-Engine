using System;
using System.Collections.Generic;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{

    public class DoT
    {

        public Entity Attacker;

        public int Count;

        private long mInterval;

        public SpellBase SpellBase;

        public DoT(Entity attacker, Guid spellId, Entity target)
        {
            SpellBase = SpellBase.Get(spellId);

            Attacker = attacker;
            Target = target;

            if (SpellBase == null || SpellBase.Combat.HotDotInterval < 1)
            {
                return;
            }

            mInterval = Globals.Timing.Milliseconds + SpellBase.Combat.HotDotInterval;
            Count = SpellBase.Combat.Duration / SpellBase.Combat.HotDotInterval - 1;
            target.DoT.Add(this);

            //Subtract 1 since the first tick always occurs when the spell is cast.
        }

        public Entity Target { get; }

        public bool CheckExpired()
        {
            if (Target != null && !Target.DoT.Contains(this))
            {
                return false;
            }

            if (SpellBase == null || Count > 0)
            {
                return false;
            }

            Target?.DoT?.Remove(this);

            return true;
        }

        public void Tick()
        {
            if (CheckExpired())
            {
                return;
            }

            if (mInterval > Globals.Timing.Milliseconds)
            {
                return;
            }

            var deadAnimations = new List<KeyValuePair<Guid, sbyte>>();
            var aliveAnimations = new List<KeyValuePair<Guid, sbyte>>();
            if (SpellBase.HitAnimationId != Guid.Empty)
            {
                deadAnimations.Add(new KeyValuePair<Guid, sbyte>(SpellBase.HitAnimationId, (sbyte) Directions.Up));
                aliveAnimations.Add(new KeyValuePair<Guid, sbyte>(SpellBase.HitAnimationId, (sbyte) Directions.Up));
            }

            Attacker?.Attack(
                Target, SpellBase.Combat.VitalDiff[0], SpellBase.Combat.VitalDiff[1],
                (DamageType) SpellBase.Combat.DamageType, (Stats) SpellBase.Combat.ScalingStat,
                SpellBase.Combat.Scaling, SpellBase.Combat.CritChance, SpellBase.Combat.CritMultiplier, deadAnimations,
                aliveAnimations
            );

            mInterval = Globals.Timing.Milliseconds + SpellBase.Combat.HotDotInterval;
            Count--;
        }

    }

}

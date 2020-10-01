using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{

    public class Stat
    {

        private ConcurrentDictionary<SpellBase, Buff> mBuff = new ConcurrentDictionary<SpellBase, Buff>();

        private bool mChanged;

        private Entity mOwner;

        private Stats mStatType;

        public Stat(Stats statType, Entity owner)
        {
            mOwner = owner;
            mStatType = statType;
        }

        public int BaseStat
        {
            get => mOwner.BaseStats[(int) mStatType];
            set => mOwner.BaseStats[(int) mStatType] = value;
        }

        public int Value()
        {
            var s = BaseStat;

            s += mOwner.StatPointAllocations[(int) mStatType];
            s += mOwner.GetStatBuffs(mStatType);

            //Add buffs
            var buffs = mBuff.Values.ToArray();
            foreach (var buff in buffs)
            {
                s += buff.BuffType;
            }

            if (s <= 0)
            {
                s = 1; //No 0 or negative stats, will give errors elsewhere in the code (especially divide by 0 errors).
            }

            return s;
        }

        public bool Update()
        {
            var changed = false;
            foreach (var buff in mBuff)
            {
                if (buff.Value.Duration <= Globals.Timing.Milliseconds)
                {
                    changed |= mBuff.TryRemove(buff.Key, out Buff result);
                }
            }

            changed |= mChanged;
            mChanged = false;

            return changed;
        }

        public void AddBuff(Buff buff)
        {
            mBuff.AddOrUpdate(buff.Spell, buff, (key, val) => buff);
            mChanged = true;
        }

        public void Reset()
        {
            mBuff.Clear();
        }

    }

}

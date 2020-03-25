using System.Collections.Generic;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{
    public class Stat
    {

        private Entity mOwner;

        private Stats mStatType;

        private Dictionary<SpellBase, Buff> mBuff = new Dictionary<SpellBase, Buff>();

        private bool mChanged;

        public int BaseStat
        {
            get => mOwner.BaseStats[(int)mStatType];
            set => mOwner.BaseStats[(int)mStatType] = value;
        }

        public Stat(Stats statType, Entity owner)
        {
            mOwner = owner;
            mStatType = statType;
        }

        public int Value()
        {
            var s = BaseStat;

            s += mOwner.StatPointAllocations[(int)mStatType];
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
            var buffs = mBuff.ToArray();
            foreach (var buff in buffs)
            {
                if (buff.Value.Duration <= Globals.Timing.TimeMs)
                {
                    mBuff.Remove(buff.Key);
                    changed = true;
                }
            }

            changed |= mChanged;
            mChanged = false;

            return changed;
        }

        public void AddBuff(Buff buff)
        {
            if (mBuff.ContainsKey(buff.Spell))
            {
                mBuff[buff.Spell].Duration = buff.Duration;
            }
            else
            {
                mBuff.Add(buff.Spell, buff);
            }

            mChanged = true;
        }

        public void Reset()
        {
            mBuff.Clear();
        }

    }
}

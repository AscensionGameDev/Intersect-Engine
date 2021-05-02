using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{

    public partial class Stat
    {

        private ConcurrentDictionary<SpellBase, Buff> mBuff = new ConcurrentDictionary<SpellBase, Buff>();

        private Buff[] mCachedBuffs = new Buff[0];

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
            // Get our base flat and percentage stats to calculate with.
            var flatStats = BaseStat + mOwner.StatPointAllocations[(int)mStatType];
            var percentageStats = 0;

            // Add item buffs
            if (mOwner is Player player)
            {
                var statBuffs = player.GetItemStatBuffs(mStatType);
                flatStats += statBuffs.Item1;
                percentageStats += statBuffs.Item2;
            }

            //Add spell buffs
            foreach (var buff in mCachedBuffs)
            {
                flatStats += buff.FlatStatcount;
                percentageStats += buff.PercentageStatcount;
            }

            // Calculate our final stat
            var finalStat = (int)Math.Ceiling(flatStats + (flatStats * (percentageStats / 100f)));
            if (finalStat <= 0)
            {
                finalStat = 1; //No 0 or negative stats, will give errors elsewhere in the code (especially divide by 0 errors).
            }

            return finalStat;
        }

        public bool Update(long time)
        {
            var origVal = Value();
            var changed = false;
            foreach (var buff in mBuff)
            {
                if (buff.Value.ExpireTime <= time)
                {
                    changed |= mBuff.TryRemove(buff.Key, out Buff result);
                }
            }

            if (changed)
            {
                mCachedBuffs = mBuff.Values.ToArray();
            }

            changed |= Value() != origVal;
            changed |= mChanged;
            mChanged = false;

            return changed;
        }

        public void AddBuff(Buff buff)
        {
            var origVal = Value();
            mBuff.AddOrUpdate(buff.Spell, buff, (key, val) => buff);
            mCachedBuffs = mBuff.Values.ToArray();
            mChanged = Value() != origVal;
        }

        public void Reset()
        {
            mBuff.Clear();
            mCachedBuffs = mBuff.Values.ToArray();
        }

    }

}

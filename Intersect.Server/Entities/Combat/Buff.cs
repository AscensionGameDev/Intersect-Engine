using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{

    public partial class Buff
    {

        public int FlatStatcount;

        public int PercentageStatcount;

        public long ExpireTime;

        public SpellBase Spell;

        public Buff(SpellBase spell, int flatStats, int percentageStats, long expireTime)
        {
            Spell = spell;
            FlatStatcount = flatStats;
            PercentageStatcount = percentageStats;
            ExpireTime = expireTime;
        }

        public override string ToString()
        {
            return $"[{typeof(Buff).FullName}{{ExpireTime={ExpireTime}, FlatStatcount={FlatStatcount}, PercentageStatcount={PercentageStatcount}, Spell={Spell.Id}}}]";
        }

    }

}

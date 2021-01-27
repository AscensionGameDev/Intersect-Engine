using Intersect.GameObjects;
using Intersect.Server.General;

namespace Intersect.Server.Entities.Combat
{

    public class Buff
    {

        public int FlatStatcount;

        public int PercentageStatcount;

        public long Duration;

        public SpellBase Spell;

        public Buff(SpellBase spell, int flatStats, int percentageStats, int duration)
        {
            Spell = spell;
            FlatStatcount = flatStats;
            PercentageStatcount = percentageStats;
            Duration = Globals.Timing.Milliseconds + duration;
        }

    }

}

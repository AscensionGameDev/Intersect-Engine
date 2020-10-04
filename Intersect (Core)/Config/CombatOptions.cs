namespace Intersect.Config
{

    public class CombatOptions
    {

        public int BlockingSlow = 30; //Slow when moving with a shield. Default 30%

        public int CombatTime = 10000; //10 seconds

        public int MaxAttackRate = 200; //5 attacks per second

        public int MaxDashSpeed = 200;

        public int MinAttackRate = 500; //2 attacks per second

        //Combat
        public int RegenTime = 3000; //3 seconds

        // Cooldowns

        /// <summary>
        /// Configures whether cooldowns within cooldown groups should match.
        /// </summary>
        public bool MatchGroupCooldowns = true;

        /// <summary>
        /// Only used when <seealso cref="MatchGroupCooldowns"/> is enabled!
        /// Configures whether cooldowns are being matched to the highest cooldown within a cooldown group when true, or are matched to the current item being used when false.
        /// </summary>
        public bool MatchGroupCooldownHighest = true;

    }

}

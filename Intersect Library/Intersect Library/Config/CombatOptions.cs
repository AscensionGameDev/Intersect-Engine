namespace Intersect.Config
{
    public class CombatOptions
    {
        //Combat
        public int RegenTime = 3000; //3 seconds
        public int MinAttackRate = 500; //2 attacks per second
        public int MaxAttackRate = 200; //5 attacks per second
        public int BlockingSlow = 30; //Slow when moving with a shield. Default 30%
        public int CritChance = 20; //1 in 20 chance to critically strike.
        public int CritMultiplier = 150; //Critical strikes deal 1.5x damage.
        public int MaxDashSpeed = 200;
    }
}

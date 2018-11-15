namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects.Maps
{
    public class NpcSpawn
    {
        public int Dir;
        public int NpcNum;
        public int X;
        public int Y;

        public NpcSpawn()
        {
        }

        public NpcSpawn(NpcSpawn copy)
        {
            NpcNum = copy.NpcNum;
            X = copy.X;
            Y = copy.Y;
            Dir = copy.Dir;
        }
    }
}
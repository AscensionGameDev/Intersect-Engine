namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Maps
{
    public class NpcSpawn
    {
        public int NpcNum;
        public int X;
        public int Y;
        public int Dir;

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

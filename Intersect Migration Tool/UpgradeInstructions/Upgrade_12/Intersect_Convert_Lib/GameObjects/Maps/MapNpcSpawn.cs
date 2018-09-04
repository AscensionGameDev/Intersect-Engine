using System;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps
{
    public class NpcSpawn
    {
        public int Dir;
        public Guid NpcId;
        public int X;
        public int Y;

        public NpcSpawn()
        {
        }

        public NpcSpawn(NpcSpawn copy)
        {
            NpcId = copy.NpcId;
            X = copy.X;
            Y = copy.Y;
            Dir = copy.Dir;
        }
    }
}
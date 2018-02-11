using Intersect.GameObjects;
using Intersect_Client.Classes.Entities;

namespace Intersect_Client.Classes.Maps
{
    public class MapAnimationInstance : AnimationInstance
    {
        private int mDir;
        private int mTileX;
        private int mTileY;

        public MapAnimationInstance(AnimationBase animBase, int tileX, int tileY, int dir) : base(animBase, false)
        {
            mTileX = tileX;
            mTileY = tileY;
            mDir = dir;
        }
    }
}
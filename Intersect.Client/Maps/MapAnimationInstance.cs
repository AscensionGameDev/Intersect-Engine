using Intersect.Client.Entities;
using Intersect.GameObjects;

namespace Intersect.Client.Maps
{
    public class MapAnimationInstance : Animation
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
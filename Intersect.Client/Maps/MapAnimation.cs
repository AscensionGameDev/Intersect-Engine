using Intersect.Client.Entities;
using Intersect.Client.Framework;
using Intersect.GameObjects;

namespace Intersect.Client.Maps
{

    public class MapAnimation : Animation
    {

        private int mDir;

        private int mTileX;

        private int mTileY;

        public MapAnimation(IGameContext gameContext, AnimationBase animBase, int tileX, int tileY, int dir) : base(gameContext, animBase, false)
        {
            mTileX = tileX;
            mTileY = tileY;
            mDir = dir;
        }

    }

}

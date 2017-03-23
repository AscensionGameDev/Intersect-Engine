using Intersect.GameObjects;
using Intersect_Client.Classes.Entities;

namespace Intersect_Client.Classes.Maps
{
    public class MapAnimationInstance : AnimationInstance
    {
        private int _dir = -1;
        private int _tileX = 0;
        private int _tileY = 0;

        public MapAnimationInstance(AnimationBase animBase, int tileX, int tileY, int dir) : base(animBase, false)
        {
            _tileX = tileX;
            _tileY = tileY;
            _dir = dir;
        }
    }
}
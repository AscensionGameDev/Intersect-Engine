using Intersect.GameObjects;
using Intersect_Client.Classes.Entities;

namespace Intersect_Client.Classes.Maps
{
    public class MapAnimationInstance : AnimationInstance
    {
        private int _dir;
        private int _tileX;
        private int _tileY;

        public MapAnimationInstance(AnimationBase animBase, int tileX, int tileY, int dir) : base(animBase, false)
        {
            _tileX = tileX;
            _tileY = tileY;
            _dir = dir;
        }
    }
}
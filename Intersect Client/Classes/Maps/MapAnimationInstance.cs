using Intersect_Client.Classes.Entities;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.Maps
{
    public class MapAnimationInstance : AnimationInstance
    {
        private int _tileX = 0;
        private int _tileY = 0;
        private int _dir = -1;
        public MapAnimationInstance(AnimationBase animBase, int tileX, int tileY, int dir) : base(animBase, false)
        {
            _tileX = tileX;
            _tileY = tileY;
            _dir = dir;
        }
    }
}

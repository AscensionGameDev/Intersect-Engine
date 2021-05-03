using System;

using Intersect.Client.General;
using Intersect.Client.Maps;

namespace Intersect.Client.Entities
{

    public partial class Dash
    {

        private int mChangeDirection = -1;

        private int mDashTime;

        private Guid mEndMapId;

        private byte mEndX;

        private float mEndXCoord;

        private byte mEndY;

        private float mEndYCoord;

        private long mStartTime;

        private float mStartXCoord;

        private float mStartYCoord;

        public Dash(Entity en, Guid endMapId, byte endX, byte endY, int dashTime, int changeDirection = -1)
        {
            mChangeDirection = changeDirection;
            mEndMapId = endMapId;
            mEndX = endX;
            mEndY = endY;
            mDashTime = dashTime;
        }

        public void Start(Entity en)
        {
            if (MapInstance.Get(en.CurrentMap) == null ||
                MapInstance.Get(mEndMapId) == null ||
                mEndMapId == en.CurrentMap && mEndX == en.X && mEndY == en.Y)
            {
                en.Dashing = null;
            }
            else
            {
                var startMap = MapInstance.Get(en.CurrentMap);
                var endMap = MapInstance.Get(mEndMapId);
                mStartTime = Globals.System.GetTimeMs();
                mStartXCoord = en.OffsetX;
                mStartYCoord = en.OffsetY;
                mEndXCoord = endMap.GetX() + mEndX * Options.TileWidth - (startMap.GetX() + en.X * Options.TileWidth);
                mEndYCoord = endMap.GetY() + mEndY * Options.TileHeight - (startMap.GetY() + en.Y * Options.TileHeight);
                if (mChangeDirection > -1)
                {
                    en.Dir = (byte) mChangeDirection;
                }
            }
        }

        public float GetXOffset()
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                return mEndXCoord;
            }
            else
            {
                return (mEndXCoord - mStartXCoord) * ((Globals.System.GetTimeMs() - mStartTime) / (float) mDashTime);
            }
        }

        public float GetYOffset()
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                return mEndYCoord;
            }
            else
            {
                return (mEndYCoord - mStartYCoord) * ((Globals.System.GetTimeMs() - mStartTime) / (float) mDashTime);
            }
        }

        public bool Update(Entity en)
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                en.Dashing = null;
                en.OffsetX = 0;
                en.OffsetY = 0;
                en.CurrentMap = mEndMapId;
                en.X = mEndX;
                en.Y = mEndY;
            }

            return en.Dashing != null;
        }

    }

}

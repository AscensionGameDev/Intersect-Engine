using System;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Maps;
using Intersect.Utilities;

namespace Intersect.Client.Entities
{

    public partial class Dash : IDash
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

        public float OffsetX => GetXOffset();

        public float OffsetY => GetYOffset();

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
            if (MapInstance.Get(en.MapId) == null ||
                MapInstance.Get(mEndMapId) == null ||
                mEndMapId == en.MapId && mEndX == en.X && mEndY == en.Y)
            {
                en.Dashing = null;
            }
            else
            {
                var startMap = MapInstance.Get(en.MapId);
                var endMap = MapInstance.Get(mEndMapId);
                mStartTime = Timing.Global.Milliseconds;
                mStartXCoord = en.OffsetX;
                mStartYCoord = en.OffsetY;
                mEndXCoord = endMap.GetX() + mEndX * Options.TileWidth - (startMap.GetX() + en.X * Options.TileWidth);
                mEndYCoord = endMap.GetY() + mEndY * Options.TileHeight - (startMap.GetY() + en.Y * Options.TileHeight);
                if (mChangeDirection > -1)
                {
                    en.Dir = (byte)mChangeDirection;
                }
            }
        }

        public float GetXOffset()
        {
            if (Timing.Global.Milliseconds > mStartTime + mDashTime)
            {
                return mEndXCoord;
            }
            else
            {
                return (mEndXCoord - mStartXCoord) * ((Timing.Global.Milliseconds - mStartTime) / (float)mDashTime);
            }
        }

        public float GetYOffset()
        {
            if (Timing.Global.Milliseconds > mStartTime + mDashTime)
            {
                return mEndYCoord;
            }
            else
            {
                return (mEndYCoord - mStartYCoord) * ((Timing.Global.Milliseconds - mStartTime) / (float)mDashTime);
            }
        }

        public bool Update(Entity en)
        {
            if (Timing.Global.Milliseconds > mStartTime + mDashTime)
            {
                en.Dashing = null;
                en.OffsetX = 0;
                en.OffsetY = 0;
                en.MapId = mEndMapId;
                en.X = mEndX;
                en.Y = mEndY;
            }

            return en.Dashing != null;
        }

    }

}

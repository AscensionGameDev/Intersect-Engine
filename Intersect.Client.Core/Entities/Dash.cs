using Intersect.Client.Framework.Entities;
using Intersect.Client.Maps;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Dash : IDash
{
    private readonly Direction mChangeDirection = Direction.None;

    private readonly long _serverDashEndMilliseconds;

    private readonly long _dashLengthMilliseconds;

    private long _dashEndMilliseconds;

    private readonly Guid mEndMapId;

    private readonly byte mEndX;

    private float mEndXCoord;

    private readonly byte mEndY;

    private float mEndYCoord;

    private long mStartTime;

    private float mStartXCoord;

    private float mStartYCoord;

    public float OffsetX => GetXOffset();

    public float OffsetY => GetYOffset();

    public Dash(
        Guid endMapId,
        byte endX,
        byte endY,
        long dashEndMilliseconds,
        long dashLengthMilliseconds,
        Direction changeDirection = Direction.None
    )
    {
        mChangeDirection = changeDirection;
        mEndMapId = endMapId;
        mEndX = endX;
        mEndY = endY;
        _serverDashEndMilliseconds = dashEndMilliseconds;
        _dashLengthMilliseconds = dashLengthMilliseconds;
    }

    public void Start(Entity entity)
    {
        var now = Timing.Global.Milliseconds;

        if (_serverDashEndMilliseconds < now)
        {
            ApplicationContext.CurrentContext.Logger.LogDebug("Skipping already-expired dash");
            entity.Dashing = null;
            return;
        }

        if (mEndMapId == entity.MapId && mEndX == entity.X && mEndY == entity.Y)
        {
            entity.Dashing = null;
            return;
        }

        if (!MapInstance.TryGet(entity.MapId, out var currentMap) || !MapInstance.TryGet(mEndMapId, out var targetMap))
        {
            entity.Dashing = null;
            return;
        }

        mStartTime = now;
        _dashEndMilliseconds = Math.Min(_serverDashEndMilliseconds, mStartTime + _dashLengthMilliseconds);

        ApplicationContext.CurrentContext.Logger.LogDebug(
            "Starting dash that is {DashLength}ms long, and there are {RemainingTime}ms before it is completed on the server",
            _dashLengthMilliseconds,
            _serverDashEndMilliseconds - now
        );

        mStartXCoord = entity.X;
        mStartYCoord = entity.Y;
        mEndXCoord = targetMap.X + mEndX * Options.Instance.Map.TileWidth - (currentMap.X + entity.X * Options.Instance.Map.TileWidth);
        mEndYCoord = targetMap.Y + mEndY * Options.Instance.Map.TileHeight - (currentMap.Y + entity.Y * Options.Instance.Map.TileHeight);
        if (mChangeDirection > Direction.None)
        {
            entity.Dir = mChangeDirection;
        }
    }

    public float GetXOffset()
    {
        if (Timing.Global.Milliseconds > mStartTime + _dashLengthMilliseconds)
        {
            return mEndXCoord;
        }

        return (mEndXCoord - mStartXCoord) * ((Timing.Global.Milliseconds - mStartTime) / (float)_dashLengthMilliseconds);
    }

    public float GetYOffset()
    {
        if (Timing.Global.Milliseconds > mStartTime + _dashLengthMilliseconds)
        {
            return mEndYCoord;
        }

        return (mEndYCoord - mStartYCoord) * ((Timing.Global.Milliseconds - mStartTime) / (float)_dashLengthMilliseconds);
    }

    public bool Update(Entity entity)
    {
        if (Timing.Global.Milliseconds <= _dashEndMilliseconds)
        {
            return entity.Dashing != null;
        }

        ApplicationContext.CurrentContext.Logger.LogDebug("Dash finished");
        entity.Dashing = null;
        entity.OffsetX = 0;
        entity.OffsetY = 0;
        entity.MapId = mEndMapId;
        entity.X = mEndX;
        entity.Y = mEndY;
        return false;
    }
}

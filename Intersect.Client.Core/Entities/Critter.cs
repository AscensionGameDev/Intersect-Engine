using Intersect.Client.Core;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Maps.Attributes;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Entities;

public partial class Critter : Entity
{
    private readonly MapCritterAttribute mAttribute;
    
    // Critter's Movement
    private long _lastMove = -1;
    private byte _randomMoveRange;

    public Critter(MapInstance map, byte x, byte y, MapCritterAttribute att) : base(Guid.NewGuid(), null, EntityType.GlobalEntity)
    {
        mAttribute = att;

        //setup Sprite & Animation
        Sprite = att.Sprite;

        if (AnimationDescriptor.TryGet(att.AnimationId, out var animationDescriptor))
        {
            TryAddAnimation(new Animation(animationDescriptor, true));
        }

        //Define Location
        MapId = map?.Id ?? default;
        X = x;
        Y = y;

        //Determine Direction
        if (mAttribute.Direction == 0)
        {
            DirectionFacing = Randomization.NextDirection();
        }
        else
        {
            DirectionFacing = (Direction)(mAttribute.Direction - 1);
        }

        //Block Players?
        Passable = !att.BlockPlayers;
    }

    public override bool Update()
    {
        if (!base.Update())
        {
            return false;
        }

        // Only skip if we are NOT in the middle of a range-walk AND the frequency timer is active
        if (_randomMoveRange <= 0 && _lastMove >= Timing.Global.MillisecondsUtc)
        {
            return true;
        }

        switch (mAttribute.Movement)
        {
            case 0: // Move Randomly
                MoveRandomly();
                break;
            case 1: // Turn Randomly
                DirectionFacing = Randomization.NextDirection();
                // Set pause after turning
                _lastMove = Timing.Global.MillisecondsUtc + mAttribute.Frequency + Globals.Random.Next((int)(mAttribute.Frequency * .5f));
                break;
        }

        return true;
    }

    private void MoveRandomly()
    {
        // Don't start a new step if currently moving between tiles
        if (IsMoving || MoveTimer >= Timing.Global.MillisecondsUtc)
        {
            return;
        }

        // No range left: pick a new direction and range
        if (_randomMoveRange <= 0)
        {
            DirectionFacing = Randomization.NextDirection();
            _randomMoveRange = (byte)Randomization.Next(1, 5);
        }

        var deltaX = 0;
        var deltaY = 0;
        switch (DirectionFacing)
        {
            case Direction.Up:
                deltaY = -1;
                break;

            case Direction.Down:
                deltaY = 1;
                break;

            case Direction.Left:
                deltaX = -1;
                break;

            case Direction.Right:
                deltaX = 1;
                break;

            case Direction.UpLeft:
                deltaX = -1;
                deltaY = -1;
                break;

            case Direction.UpRight:
                deltaX = 1;
                deltaY = -1;
                break;

            case Direction.DownLeft:
                deltaX = -1;
                deltaY = 1;
                break;

            case Direction.DownRight:
                deltaX = 1;
                deltaY = 1;
                break;
        }

        var newX = (sbyte)X + deltaX;
        var newY = (sbyte)Y + deltaY;
        IEntity? blockedBy = null;

        // Boundary checks
        var isBlocked = -1 == IsTileBlocked(new Point(newX, newY), Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids);
        var playerOnTile = PlayerOnTile(MapId, newX, newY);

        if (isBlocked && !playerOnTile && 
            newX >= 0 && newX < Options.Instance.Map.MapWidth &&
            newY >= 0 && newY < Options.Instance.Map.MapHeight)
        {
            X = (byte)newX;
            Y = (byte)newY;
            IsMoving = true;
            OffsetX = deltaX == 0 ? 0 : (deltaX > 0 ? -Options.Instance.Map.TileWidth : Options.Instance.Map.TileWidth);
            OffsetY = deltaY == 0 ? 0 : (deltaY > 0 ? -Options.Instance.Map.TileHeight : Options.Instance.Map.TileHeight);
            MoveTimer = Timing.Global.MillisecondsUtc + (long)GetMovementTime();
            _randomMoveRange--;

            // Critter's last step: set an idle pause timer
            if (_randomMoveRange <= 0)
            {
                _lastMove = Timing.Global.MillisecondsUtc + mAttribute.Frequency + Globals.Random.Next((int)(mAttribute.Frequency * .5f));
            }
        }
        else
        {
            // Blocked by something: end range early and trigger pause
            _randomMoveRange = 0;
            _lastMove = Timing.Global.MillisecondsUtc + mAttribute.Frequency;
        }
    }

    public bool PlayerOnTile(Guid mapId, int x, int y)
    {
        foreach (var en in Globals.Entities)
        {
            if (en.Value == null)
            {
                continue;
            }

            if (en.Value.MapId == mapId &&
                    en.Value.X == x &&
                    en.Value.Y == y)
            {
                if (en.Value is Player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override HashSet<Entity>? DetermineRenderOrder(HashSet<Entity>? renderList, IMapInstance? map)
    {
        if (mAttribute.Layer == 1)
        {
            return base.DetermineRenderOrder(renderList, map);
        }

        _ = (renderList?.Remove(this));
        if (map == null || Globals.Me == null || Globals.Me.MapInstance == null || Globals.MapGrid == default)
        {
            return null;
        }

        var gridX = Globals.Me.MapInstance.GridX;
        var gridY = Globals.Me.MapInstance.GridY;
        for (var x = gridX - 1; x <= gridX + 1; x++)
        {
            for (var y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x >= 0 &&
                    x < Globals.MapGridWidth &&
                    y >= 0 &&
                    y < Globals.MapGridHeight &&
                    Globals.MapGrid[x, y] != Guid.Empty)
                {
                    if (Globals.MapGrid[x, y] == MapId)
                    {
                        if (mAttribute.Layer == 0)
                        {
                            y--;
                        }

                        if (mAttribute.Layer == 2)
                        {
                            y++;
                        }

                        var priority = mRenderPriority;
                        if (Z != 0)
                        {
                            priority += 3;
                        }

                        HashSet<Entity>? renderSet = null;

                        if (y == gridY - 2)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Y];
                        }
                        else if (y == gridY - 1)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight + Y];
                        }
                        else if (y == gridY)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight * 2 + Y];
                        }
                        else if (y == gridY + 1)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight * 3 + Y];
                        }
                        else if (y == gridY + 2)
                        {
                            renderSet = Graphics.RenderingEntities[priority, Options.Instance.Map.MapHeight * 4 + Y];
                        }

                        _ = (renderSet?.Add(this));
                        renderList = renderSet;
                        return renderList;
                    }
                }
            }
        }

        return renderList;
    }

    public override float GetMovementTime()
    {
        return mAttribute.Speed;
    }
}

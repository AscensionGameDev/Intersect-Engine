using Intersect.Client.Core;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Utilities;
using System;
using System.Collections.Generic;

namespace Intersect.Client.Entities
{
    public partial class Critter : Entity
    {
        private MapCritterAttribute mAttribute;
        private long mLastMove = -1;

        public Critter(MapInstance map, byte x, byte y, MapCritterAttribute att) : base(Guid.NewGuid(), null, EntityType.GlobalEntity)
        {
            mAttribute = att;

            //setup Sprite & Animation
            Sprite = att?.Sprite;
            var anim = AnimationBase.Get(att.AnimationId);
            if (anim != null)
            {
                var animInstance = new Animation(anim, true);
                Animations.Add(animInstance);
            }

            //Define Location
            MapId = map?.Id ?? default;
            X = x;
            Y = y;

            //Determine Direction
            if (mAttribute.Direction == 0)
            {
                Dir = Randomization.NextDirection();
            }
            else
            {
                Dir = (Direction)(mAttribute.Direction - 1);
            }

            //Block Players?
            Passable = !att.BlockPlayers;
        }

        public override bool Update()
        {
            if (base.Update())
            {
                if (mLastMove < Timing.Global.MillisecondsUtc)
                {
                    switch (mAttribute.Movement)
                    {
                        case 0: //Move Randomly
                            MoveRandomly();
                            break;
                        case 1: //Turn?
                            Dir = Randomization.NextDirection();
                            break;

                    }

                    mLastMove = Timing.Global.MillisecondsUtc + mAttribute.Frequency + Globals.Random.Next((int)(mAttribute.Frequency * .5f));
                }
                return true;
            }
            return false;
        }

        private void MoveRandomly()
        {
            MoveDir = Randomization.NextDirection();
            var tmpX = (sbyte)X;
            var tmpY = (sbyte)Y;
            IEntity blockedBy = null;

            if (IsMoving || MoveTimer >= Timing.Global.MillisecondsUtc)
            {
                return;
            }

            var deltaX = 0;
            var deltaY = 0;

            switch (MoveDir)
            {
                case Direction.Up:
                    deltaX = 0;
                    deltaY = -1;
                    break;

                case Direction.Down:
                    deltaX = 0;
                    deltaY = 1;
                    break;

                case Direction.Left:
                    deltaX = -1;
                    deltaY = 0;
                    break;

                case Direction.Right:
                    deltaX = 1;
                    deltaY = 0;
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

            if (deltaX != 0 || deltaY != 0)
            {
                var newX = tmpX + deltaX;
                var newY = tmpY + deltaY;
                var isBlocked =
                    IsTileBlocked(newX, newY, Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids) == -1;
                var playerOnTile = PlayerOnTile(MapId, newX, newY);

                if (isBlocked && newX >= 0 && newX < Options.MapWidth && newY >= 0 && newY < Options.MapHeight &&
                    (!mAttribute.BlockPlayers || !playerOnTile))
                {
                    tmpX += (sbyte)deltaX;
                    tmpY += (sbyte)deltaY;
                    IsMoving = true;
                    Dir = MoveDir;

                    if (deltaX == 0)
                    {
                        OffsetX = 0;
                    }
                    else
                    {
                        OffsetX = deltaX > 0 ? -Options.TileWidth : Options.TileWidth;
                    }

                    if (deltaY == 0)
                    {
                        OffsetY = 0;
                    }
                    else
                    {
                        OffsetY = deltaY > 0 ? -Options.TileHeight : Options.TileHeight;
                    }
                }
            }

            if (IsMoving)
            {
                X = (byte)tmpX;
                Y = (byte)tmpY;
                MoveTimer = Timing.Global.MillisecondsUtc + (long)GetMovementTime();
            }
            else if (MoveDir != Dir)
            {
                Dir = MoveDir;
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

        public override HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, IMapInstance map)
        {
            if (mAttribute.Layer == 1)
            {
                return base.DetermineRenderOrder(renderList, map);
            }

            renderList?.Remove(this);
            if (map == null || Globals.Me == null || Globals.Me.MapInstance == null)
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

                            HashSet<Entity> renderSet = null;

                            if (y == gridY - 2)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Y];
                            }
                            else if (y == gridY - 1)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight + Y];
                            }
                            else if (y == gridY)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 2 + Y];
                            }
                            else if (y == gridY + 1)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 3 + Y];
                            }
                            else if (y == gridY + 2)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 4 + Y];
                            }

                            renderSet?.Add(this);
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
}

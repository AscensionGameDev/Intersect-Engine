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

        public Critter(MapInstance map, byte x, byte y, MapCritterAttribute att) : base(Guid.NewGuid(), null, EntityTypes.GlobalEntity)
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
                Dir = (byte)Globals.Random.Next(Options.Instance.Sprites.Directions);
            }
            else
            {
                Dir = (byte)(mAttribute.Direction - 1);
            }

            //Block Players?
            Passable = !att.BlockPlayers;
        }

        public override bool Update()
        {
            if (base.Update())
            {
                if (mLastMove < Timing.Global.Milliseconds)
                {
                    switch (mAttribute.Movement)
                    {
                        case 0: //Move Randomly
                            MoveRandomly();
                            break;
                        case 1: //Turn?
                            Dir = (byte)Globals.Random.Next(Options.Instance.Sprites.Directions);
                            break;

                    }

                    mLastMove = Timing.Global.Milliseconds + mAttribute.Frequency + Globals.Random.Next((int)(mAttribute.Frequency * .5f));
                }
                return true;
            }
            return false;
        }

        private void MoveRandomly()
        {
            MoveDir = (byte)Globals.Random.Next(Options.Instance.Sprites.Directions);
            var tmpX = (sbyte)X;
            var tmpY = (sbyte)Y;
            IEntity blockedBy = null;

            if (!IsMoving && MoveTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond)
            {
                switch (MoveDir)
                {
                    case 0: // Up
                        if (IsTileBlocked(X, Y - 1, Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids) == -1 && Y > 0 && (!mAttribute.BlockPlayers || !PlayerOnTile(MapId, X, Y - 1)))
                        {
                            tmpY--;
                            IsMoving = true;
                            Dir = 0;
                            OffsetY = Options.TileHeight;
                            OffsetX = 0;
                        }

                        break;
                    case 1: // Down
                        if (IsTileBlocked(X, Y + 1, Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids) == -1 && Y < Options.MapHeight - 1 && (!mAttribute.BlockPlayers || !PlayerOnTile(MapId, X, Y + 1)))
                        {
                            tmpY++;
                            IsMoving = true;
                            Dir = 1;
                            OffsetY = -Options.TileHeight;
                            OffsetX = 0;
                        }

                        break;
                    case 2: // Left
                        if (IsTileBlocked(X - 1, Y, Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids) == -1 && X > 0 && (!mAttribute.BlockPlayers || !PlayerOnTile(MapId, X - 1, Y)))
                        {
                            tmpX--;
                            IsMoving = true;
                            Dir = 2;
                            OffsetY = 0;
                            OffsetX = Options.TileWidth;
                        }

                        break;
                    case 3: // Right
                        if (IsTileBlocked(X + 1, Y, Z, MapId, ref blockedBy, true, true, mAttribute.IgnoreNpcAvoids) == -1 && X < Options.MapWidth - 1 && (!mAttribute.BlockPlayers || !PlayerOnTile(MapId, X + 1, Y)))
                        {
                            //If BlockPlayers then make sure there is no player here
                            tmpX++;
                            IsMoving = true;
                            Dir = 3;
                            OffsetY = 0;
                            OffsetX = -Options.TileWidth;
                        }

                        break;
                }

                if (IsMoving)
                {
                    X = (byte)tmpX;
                    Y = (byte)tmpY;

                    //TryToChangeDimension();
                    MoveTimer = (Timing.Global.Ticks / TimeSpan.TicksPerMillisecond) + (long)GetMovementTime();
                }
                else
                {
                    if (MoveDir != Dir)
                    {
                        Dir = (byte)MoveDir;
                    }
                }
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

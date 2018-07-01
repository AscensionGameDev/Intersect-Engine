using System;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Graphics = Intersect.Client.Classes.Core.GameGraphics;

namespace Intersect_Client.Classes.Entities
{
    public class Projectile : Entity
    {
        private bool mLoaded;
        private ProjectileBase mMyBase;
        private int mSpawnCount;
        private int mSpawnedAmount;
        private int mTotalSpawns;
        private Guid mOwner;
        public Guid ProjectileId;
        private int mQuantity;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;

        private long mSpawnTime;
        public int Target;

        /// <summary>
        ///     The constructor for the inherated projectile class
        /// </summary>
        public Projectile(Guid id, ByteBuffer bf) : base(id, bf)
        {
            Vital[(int) Vitals.Health] = 1;
            MaxVital[(int) Vitals.Health] = 1;
            HideName = 1;
            Passable = 1;
            IsMoving = true;
        }

        public override void Load(ByteBuffer bf)
        {
            if (mLoaded) return;
            base.Load(bf);
            ProjectileId = bf.ReadGuid();
            Dir = bf.ReadInteger();
            Target = bf.ReadInteger();
            mOwner = bf.ReadGuid();
            mMyBase = ProjectileBase.Lookup.Get<ProjectileBase>(ProjectileId);
            if (mMyBase != null)
            {
                for (int x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
                {
                    for (int y = 0; y < ProjectileBase.SPAWN_LOCATIONS_WIDTH; y++)
                    {
                        for (int d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                        {
                            if (mMyBase.SpawnLocations[x, y].Directions[d] == true)
                            {
                                mTotalSpawns++;
                            }
                        }
                    }
                }
                mTotalSpawns *= mMyBase.Quantity;
            }
            Spawns = new ProjectileSpawns[mTotalSpawns];
            mLoaded = true;
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Projectile;
        }

        public override void Dispose()
        {
            if (Spawns != null)
            {
                foreach (ProjectileSpawns s in Spawns)
                {
                    if (s != null && s.Anim != null)
                    {
                        s.Anim.Dispose();
                    }
                }
            }
        }

        //Find out which animation data to load depending on what spawn wave we are on during projection.
        private int FindSpawnAnimationData()
        {
            int start = 0;
            int end = 1;
            for (int i = 0; i < mMyBase.Animations.Count; i++)
            {
                end = mMyBase.Animations[i].SpawnRange;
                if (mQuantity >= start && mQuantity < end)
                {
                    return i;
                }
                start = end;
            }
            //If reaches maximum and the developer(s) have fucked up the animation ranges on each spawn of projectiles somewhere, just assign it to the last animation state.
            return mMyBase.Animations.Count - 1;
        }

        private void AddProjectileSpawns()
        {
            int spawn = FindSpawnAnimationData();
            AnimationBase animBase = AnimationBase.Lookup.Get<AnimationBase>(mMyBase.Animations[spawn].AnimationId);

            for (int x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (int y = 0; y < ProjectileBase.SPAWN_LOCATIONS_WIDTH; y++)
                {
                    for (int d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (mMyBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d),
                                CurrentX + FindProjectileRotationX(Dir, x - 2, y - 2),
                                CurrentY + FindProjectileRotationY(Dir, x - 2, y - 2), CurrentZ, CurrentMap, animBase,
                                mMyBase.Animations[spawn].AutoRotate, mMyBase,this);
                            Spawns[mSpawnedAmount] = s;
                            if (Collided(mSpawnedAmount))
                            {
                                Spawns[mSpawnedAmount].Dispose();
                                Spawns[mSpawnedAmount] = null;
                                mSpawnCount--;
                            }
                            mSpawnedAmount++;
                            mSpawnCount++;
                        }
                    }
                }
            }
            mQuantity++;
            mSpawnTime = Globals.System.GetTimeMs() + mMyBase.Delay;
        }

        private int FindProjectileRotationX(int direction, int x, int y)
        {
            switch (direction)
            {
                case 0: //Up
                    return x;
                case 1: //Down
                    return -x;
                case 2: //Left
                    return y;
                case 3: //Right
                    return -y;
                default:
                    return x;
            }
        }

        private int FindProjectileRotationY(int direction, int x, int y)
        {
            switch (direction)
            {
                case 0: //Up
                    return y;
                case 1: //Down
                    return -y;
                case 2: //Left
                    return -x;
                case 3: //Right
                    return x;
                default:
                    return y;
            }
        }

        private int FindProjectileRotationDir(int entityDir, int projectionDir)
        {
            switch (entityDir)
            {
                case 0: //Up
                    return projectionDir;
                case 1: //Down
                    switch (projectionDir)
                    {
                        case 0: //Up
                            return 1;
                        case 1: //Down
                            return 0;
                        case 2: //Left
                            return 3;
                        case 3: //Right
                            return 2;
                        case 4: //UpLeft
                            return 7;
                        case 5: //UpRight
                            return 6;
                        case 6: //DownLeft
                            return 5;
                        case 7: //DownRight
                            return 4;
                        default:
                            return projectionDir;
                    }
                case 2: //Left
                    switch (projectionDir)
                    {
                        case 0: //Up
                            return 2;
                        case 1: //Down
                            return 3;
                        case 2: //Left
                            return 1;
                        case 3: //Right
                            return 0;
                        case 4: //UpLeft
                            return 6;
                        case 5: //UpRight
                            return 4;
                        case 6: //DownLeft
                            return 7;
                        case 7: //DownRight
                            return 5;
                        default:
                            return projectionDir;
                    }
                case 3: //Right
                    switch (projectionDir)
                    {
                        case 0: //Up
                            return 3;
                        case 1: //Down
                            return 2;
                        case 2: //Left
                            return 0;
                        case 3: //Right
                            return 1;
                        case 4: //UpLeft
                            return 5;
                        case 5: //UpRight
                            return 7;
                        case 6: //DownLeft
                            return 4;
                        case 7: //DownRight
                            return 6;
                        default:
                            return projectionDir;
                    }
                default:
                    return projectionDir;
            }
        }

        private float GetRangeX(int direction, float range)
        {
            //Left, UpLeft, DownLeft
            if (direction == 2 || direction == 4 || direction == 6)
            {
                return -range;
            }
            //Right, UpRight, DownRight
            else if (direction == 3 || direction == 5 || direction == 7)
            {
                return range;
            }
            //Up and Down
            else
            {
                return 0;
            }
        }

        private float GetRangeY(int direction, float range)
        {
            //Up, UpLeft, UpRight
            if (direction == 0 || direction == 4 || direction == 5)
            {
                return -range;
            }
            //Down, DownLeft, DownRight
            else if (direction == 1 || direction == 6 || direction == 7)
            {
                return range;
            }
            //Left and Right
            else
            {
                return 0;
            }
        }

        /// <summary>
        ///     Gets the displacement of the projectile during projection
        /// </summary>
        /// <returns>The displacement from the co-ordinates if placed on a Options.TileHeight grid.</returns>
        private float GetDisplacement(long spawnTime)
        {
            long elapsedTime = Globals.System.GetTimeMs() - spawnTime;
            float displacementPercent = elapsedTime / (float) mMyBase.Speed;
            return displacementPercent * Options.TileHeight * mMyBase.Range;
        }

        /// <summary>
        ///     Overwrite updating the offsets for projectile movement.
        /// </summary>
        public override bool Update()
        {
            if (mMyBase == null) return false;

            var tmpI = -1;
            var map = CurrentMap;
            var y = CurrentY;

            if (mQuantity < mMyBase.Quantity && mSpawnTime < Globals.System.GetTimeMs())
            {
                AddProjectileSpawns();
            }

            if (IsMoving)
            {
                for (int s = 0; s < mSpawnedAmount; s++)
                {
                    if (Spawns[s] != null && MapInstance.Lookup.Get<MapInstance>(Spawns[s].SpawnMapId) != null)
                    {
                        Spawns[s].OffsetX = GetRangeX(Spawns[s].Dir, GetDisplacement(Spawns[s].SpawnTime));
                        Spawns[s].OffsetY = GetRangeY(Spawns[s].Dir, GetDisplacement(Spawns[s].SpawnTime));
                        Spawns[s].Anim.SetPosition(
                            MapInstance.Lookup.Get<MapInstance>(Spawns[s].SpawnMapId).GetX() +
                            Spawns[s].SpawnX * Options.TileWidth +
                            Spawns[s].OffsetX +
                            Options.TileWidth / 2,
                            MapInstance.Lookup.Get<MapInstance>(Spawns[s].SpawnMapId).GetY() +
                            Spawns[s].SpawnY * Options.TileHeight +
                            Spawns[s].OffsetY +
                            Options.TileHeight / 2, CurrentX, CurrentY, CurrentMap,
                            Spawns[s].AutoRotate ? Spawns[s].Dir : 0, Spawns[s].Z);
                        Spawns[s].Anim.Update();
                    }
                }
            }
            CheckForCollision();

            return true;
        }

        public void CheckForCollision()
        {
            if (mSpawnCount != 0 || mQuantity < mMyBase.Quantity)
            {
                for (int i = 0; i < mSpawnedAmount; i++)
                {
                    if (Spawns[i] != null && Globals.System.GetTimeMs() > Spawns[i].TransmittionTimer)
                    {
                        var spawnMap = MapInstance.Lookup.Get<MapInstance>(Spawns[i].MapId);
                        if (spawnMap != null)
                        {
                            var newx = Spawns[i].X + (int) GetRangeX(Spawns[i].Dir, 1);
                            var newy = Spawns[i].Y + (int) GetRangeY(Spawns[i].Dir, 1);
                            var newMapId = Spawns[i].MapId;
                            bool killSpawn = false;

                            Spawns[i].Distance++;

                            if (newx < 0)
                            {
                                if (MapInstance.Lookup.Get<MapInstance>(spawnMap.Left) != null)
                                {
                                    newMapId = spawnMap.Left;
                                    newx = Options.MapWidth - 1;
                                }
                                else
                                {
                                    killSpawn = true;
                                }
                            }
                            if (newx > Options.MapWidth - 1)
                            {
                                if (MapInstance.Lookup.Get<MapInstance>(spawnMap.Right) != null)
                                {
                                    newMapId = spawnMap.Right;
                                    newx = 0;
                                }
                                else
                                {
                                    killSpawn = true;
                                }
                            }
                            if (newy < 0)
                            {
                                if (MapInstance.Lookup.Get<MapInstance>(spawnMap.Up) != null)
                                {
                                    newMapId = spawnMap.Up;
                                    newy = Options.MapHeight - 1;
                                }
                                else
                                {
                                    killSpawn = true;
                                }
                            }
                            if (newy > Options.MapHeight - 1)
                            {
                                if (MapInstance.Lookup.Get<MapInstance>(spawnMap.Down) != null)
                                {
                                    newMapId = spawnMap.Down;
                                    newy = 0;
                                }
                                else
                                {
                                    killSpawn = true;
                                }
                            }

                            if (killSpawn)
                            {
                                Spawns[i].Dispose();
                                Spawns[i] = null;
                                mSpawnCount--;
                                continue;
                            }

                            Spawns[i].X = newx;
                            Spawns[i].Y = newy;
                            Spawns[i].MapId = newMapId;
                            var newMap = MapInstance.Lookup.Get<MapInstance>(newMapId);

                            //Check for Z-Dimension
                            if (newMap.Attributes[Spawns[i].X, Spawns[i].Y] != null)
                            {
                                if (newMap.Attributes[Spawns[i].X, Spawns[i].Y].Value == (int) MapAttributes.ZDimension)
                                {
                                    if (newMap.Attributes[Spawns[i].X, Spawns[i].Y].Data1 > 0)
                                    {
                                        Spawns[i].Z = newMap.Attributes[Spawns[i].X, Spawns[i].Y].Data1 - 1;
                                    }
                                }
                            }

                            if (killSpawn == false) killSpawn = Collided(i);
                            
                            Spawns[i].TransmittionTimer = Globals.System.GetTimeMs() +
                                                          (long) ((float) mMyBase.Speed / (float) mMyBase.Range);
                            if (Spawns[i].Distance >= mMyBase.Range)
                            {
                                killSpawn = true;
                            }
                            if (killSpawn)
                            {
                                Spawns[i].Dispose();
                                Spawns[i] = null;
                                mSpawnCount--;
                            }
                        }
                    }
                }
            }
            else
            {
                Globals.Entities[Id].Dispose();
            }
        }

        private bool Collided(int i)
        {
            var killSpawn = false;
            var blockedBy = Guid.Empty;
            int tileBlocked =
                Globals.Me.IsTileBlocked(Spawns[i].X, Spawns[i].Y, CurrentZ, Spawns[i].MapId, ref blockedBy, Spawns[i].ProjectileBase.IgnoreActiveResources, Spawns[i].ProjectileBase.IgnoreExhaustedResources);

            if (tileBlocked != -1)
            {
                if (tileBlocked == -6 && blockedBy != mOwner && Globals.Entities.ContainsKey(blockedBy))
                {
                    if (Globals.Entities[blockedBy].GetType() == typeof(Resource))
                    {
                        killSpawn = true;
                    }
                }
                else
                {
                    if (tileBlocked == -2)
                    {
                        if (!Spawns[i].ProjectileBase.IgnoreMapBlocks)
                        {
                            killSpawn = true;
                        }
                    }
                    else if (tileBlocked == -3)
                    {
                        if (!Spawns[i].ProjectileBase.IgnoreZDimension)
                        {
                            killSpawn = true;
                        }
                    }
                    else if (tileBlocked == -5)
                    {
                        killSpawn = true;
                    }
                }
            }
            return killSpawn;
        }

        /// <summary>
        ///     Rendering all of the individual projectiles from a singular spawn to a map.
        /// </summary>
        public override void Draw()
        {
            if (MapInstance.Lookup.Get<MapInstance>(CurrentMap) == null ||
                !Globals.GridMaps.Contains(CurrentMap)) return;
        }

        public void SpawnDead(int spawnIndex)
        {
            if (spawnIndex < mSpawnedAmount && Spawns[spawnIndex] != null)
            {
                Spawns[spawnIndex].Dispose();
                Spawns[spawnIndex] = null;
            }
        }
    }

    public class ProjectileSpawns
    {
        public AnimationInstance Anim;
        public bool AutoRotate;
        public int Dir;
        public int Distance;
        public Guid MapId;

        //Clientside variables
        public float OffsetX;

        public float OffsetY;
        public ProjectileBase ProjectileBase;
        public Guid SpawnMapId;
        public long SpawnTime = Globals.System.GetTimeMs();
        public int SpawnX;
        public int SpawnY;
        public long TransmittionTimer = Globals.System.GetTimeMs();
        public int X;
        public int Y;
        public int Z;

        public ProjectileSpawns(int dir, int x, int y, int z, Guid mapId, AnimationBase animBase, bool autoRotate,
            ProjectileBase projectileBase, Entity parent)
        {
            X = x;
            Y = y;
            SpawnX = X;
            SpawnY = Y;
            Z = z;
            MapId = mapId;
            SpawnMapId = MapId;
            Dir = dir;
            Anim = new AnimationInstance(animBase, true, autoRotate, Z,parent);
            AutoRotate = autoRotate;
            ProjectileBase = projectileBase;
            TransmittionTimer = Globals.System.GetTimeMs() +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public void Dispose()
        {
            Anim.Dispose();
        }
    }
}
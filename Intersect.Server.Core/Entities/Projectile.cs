using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Maps;
using Intersect.Utilities;
using MapAttribute = Intersect.Enums.MapAttribute;

namespace Intersect.Server.Entities
{

    public partial class Projectile : Entity
    {

        public ProjectileBase Base;

        public bool HasGrappled;

        public ItemBase Item;

        private int mQuantity;

        private int mSpawnCount;

        private int mSpawnedAmount;

        private long mSpawnTime;

        private int mTotalSpawns;

        public Entity Owner;

        // Individual Spawns
        public ProjectileSpawn[] Spawns;

        public SpellBase Spell;

        public Entity Target;

        private int mLastTargetX = -1;

        private int mLastTargetY = -1;

        private Guid mLastTargetMapId = Guid.Empty;

        public Projectile(
            Entity owner,
            SpellBase parentSpell,
            ItemBase parentItem,
            ProjectileBase projectile,
            Guid mapId,
            byte X,
            byte Y,
            byte z,
            Direction direction,
            Entity target
        ) : base()
        {
            Base = projectile;
            Name = Base.Name;
            Owner = owner;
            Target = owner.Target;
            Stat = owner.Stat;
            MapId = mapId;
            base.X = X;
            base.Y = Y;
            Z = z;
            SetMaxVital(Vital.Health, 1);
            SetVital(Vital.Health, 1);
            Dir = direction;
            Spell = parentSpell;
            Item = parentItem;

            Passable = true;
            HideName = true;
            for (var x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (var y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (var d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (Base.SpawnLocations[x, y].Directions[d] == true)
                        {
                            mTotalSpawns++;
                        }
                    }
                }
            }

            mTotalSpawns *= Base.Quantity;
            Spawns = new ProjectileSpawn[mTotalSpawns];
        }

        private void AddProjectileSpawns(List<KeyValuePair<Guid, int>> spawnDeaths)
        {
            for (byte x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (byte y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (byte d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (Base.SpawnLocations[x, y].Directions[d] == true && mSpawnedAmount < Spawns.Length)
                        {
                            var s = new ProjectileSpawn(
                                FindProjectileRotationDir(Dir, (Direction)d),
                                (byte) (X + FindProjectileRotationX(Dir, x - 2, y - 2)),
                                (byte) (Y + FindProjectileRotationY(Dir, x - 2, y - 2)), (byte) Z, MapId, MapInstanceId, Base, this
                            );

                            Spawns[mSpawnedAmount] = s;
                            mSpawnedAmount++;
                            mSpawnCount++;
                            if (CheckForCollision(s))
                            {
                                s.Dead = true;
                            }
                        }
                    }
                }
            }

            mQuantity++;
            mSpawnTime = Timing.Global.Milliseconds + Base.Delay;
        }

        private static int FindProjectileRotationX(Direction direction, int x, int y)
        {
            switch (direction)
            {
                case Direction.Up:
                    return x;
                case Direction.Down:
                    return -x;
                case Direction.Left:
                case Direction.UpLeft:
                case Direction.DownLeft:
                    return y;
                case Direction.Right:
                case Direction.UpRight:
                case Direction.DownRight:
                    return -y;
                default:
                    return x;
            }
        }

        private static int FindProjectileRotationY(Direction direction, int x, int y)
        {
            switch (direction)
            {
                case Direction.Up:
                    return y;
                case Direction.Down:
                    return -y;
                case Direction.Left:
                case Direction.UpLeft:
                case Direction.DownLeft:
                    return -x;
                case Direction.Right:
                case Direction.UpRight:
                case Direction.DownRight:
                    return x;
                default:
                    return y;
            }
        }

        private static Direction FindProjectileRotationDir(Direction entityDir, Direction projectionDir) =>
            (Direction)ProjectileBase.ProjectileRotationDir[(int)entityDir * ProjectileBase.MAX_PROJECTILE_DIRECTIONS + (int)projectionDir];

        public void Update(List<Guid> projDeaths, List<KeyValuePair<Guid, int>> spawnDeaths)
        {
            if (mQuantity < Base.Quantity && Timing.Global.Milliseconds > mSpawnTime)
            {
                AddProjectileSpawns(spawnDeaths);
            }

            ProcessFragments(projDeaths, spawnDeaths);
        }

        private static float GetRangeX(Direction direction, float range)
        {
            switch (direction)
            {
                case Direction.Left:
                case Direction.UpLeft:
                case Direction.DownLeft:
                    return -range;
                case Direction.Right:
                case Direction.UpRight:
                case Direction.DownRight:
                    return range;
                case Direction.Up:
                case Direction.Down:
                default:
                    return 0;
            }
        }

        private static float GetRangeY(Direction direction, float range)
        {
            switch (direction)
            {
                case Direction.Up:
                case Direction.UpLeft:
                case Direction.UpRight:
                    return -range;
                case Direction.Down:
                case Direction.DownLeft:
                case Direction.DownRight:
                    return range;
                case Direction.Left:
                case Direction.Right:
                default:
                    return 0;
            }
        }

        public void ProcessFragments(List<Guid> projDeaths, List<KeyValuePair<Guid, int>> spawnDeaths)
        {
            if (Base == null)
            {
                return;
            }

            if (mSpawnCount != 0 || mQuantity < Base.Quantity)
            {
                for (var i = 0; i < mSpawnedAmount; i++)
                {
                    var spawn = Spawns[i];
                    if (spawn != null)
                    {
                        while (Timing.Global.Milliseconds > spawn.TransmittionTimer && Spawns[i] != null)
                        {
                            var x = spawn.X;
                            var y = spawn.Y;
                            var map = spawn.MapId;
                            var killSpawn = false;
                            if (!spawn.Dead)
                            {
                                killSpawn = MoveFragment(spawn);
                                if (!killSpawn && (x != spawn.X || y != spawn.Y || map != spawn.MapId))
                                {
                                    killSpawn = CheckForCollision(spawn);
                                }
                            }

                            if (killSpawn || spawn.Dead)
                            {
                                spawnDeaths.Add(new KeyValuePair<Guid, int>(Id, i));
                                Spawns[i] = null;
                                mSpawnCount--;
                            }
                        }
                    }
                }
            }
            else
            {
                lock (EntityLock)
                {
                    projDeaths.Add(Id);
                    Die(false);
                }
            }
        }

        public bool CheckForCollision(ProjectileSpawn spawn)
        {
            if(spawn == null)
            {
                return false;
            }

            var killSpawn = MoveFragment(spawn, false);

            //Check Map Entities For Hits
            var map = MapController.Get(spawn.MapId);
            if (Math.Round(spawn.X) < 0 || Math.Round(spawn.X) >= Options.Instance.MapOpts.MapWidth ||
                Math.Round(spawn.Y) < 0 || Math.Round(spawn.Y) >= Options.Instance.MapOpts.MapHeight)
            {
                return false;
            }
            var attribute = map.Attributes[(int)Math.Round(spawn.X), (int)Math.Round(spawn.Y)];

            if (!killSpawn && attribute != null)
            {
                //Check for Z-Dimension
                if (!spawn.ProjectileBase.IgnoreZDimension)
                {
                    if (attribute.Type == MapAttribute.ZDimension)
                    {
                        if (((MapZDimensionAttribute) attribute).GatewayTo > 0)
                        {
                            spawn.Z = (byte) (((MapZDimensionAttribute) attribute).GatewayTo - 1);
                        }
                    }
                }

                //Check for grapplehooks.
                if (attribute.Type == MapAttribute.GrappleStone &&
                    Base.GrappleHookOptions.Contains(GrappleOption.MapAttribute) &&
                    !spawn.Parent.HasGrappled &&
                    (spawn.X != Owner.X || spawn.Y != Owner.Y))
                {
                    if (!spawn.ProjectileBase.HomingBehavior && !spawn.ProjectileBase.DirectShotBehavior && 
                        (spawn.Dir <= Direction.Right || spawn.Dir != Direction.None && Options.Instance.MapOpts.EnableDiagonalMovement)
                    )
                    {
                        spawn.Parent.HasGrappled = true;

                        //Only grapple if the player hasnt left the firing position.. if they have then we assume they dont wanna grapple
                        if (Owner.X == X && Owner.Y == Y && Owner.MapId == MapId)
                        {
                            Owner.Dir = spawn.Dir;
                            new Dash(
                                Owner, spawn.Distance, Owner.Dir, Base.IgnoreMapBlocks,
                                Base.IgnoreActiveResources, Base.IgnoreExhaustedResources, Base.IgnoreZDimension
                            );
                        }

                        killSpawn = true;
                    }
                }

                if (!spawn.ProjectileBase.IgnoreMapBlocks &&
                    (attribute.Type == MapAttribute.Blocked || attribute.Type == MapAttribute.Animation && ((MapAnimationAttribute)attribute).IsBlock))
                {
                    killSpawn = true;
                }
            }

            if (!killSpawn && MapController.TryGetInstanceFromMap(map.Id, MapInstanceId, out var mapInstance))
            {
                var entities = mapInstance.GetEntities();
                for (var z = 0; z < entities.Count; z++)
                {
                    if (entities[z] != null && entities[z] != spawn.Parent.Owner && entities[z].Z == spawn.Z &&
                        (entities[z].X == Math.Round(spawn.X)) &&
                        (entities[z].Y == Math.Round(spawn.Y)) &&
                        (spawn.X != Owner.X || spawn.Y != Owner.Y))
                    {
                        killSpawn = spawn.HitEntity(entities[z]);
                        if (killSpawn && !spawn.ProjectileBase.PierceTarget)
                        {
                            return killSpawn;
                        }
                    }
                    else
                    {
                        if (z == entities.Count - 1)
                        {
                            if (spawn.Distance >= Base.Range)
                            {
                                killSpawn = true;
                            }
                        }
                    }
                }
            }

            return killSpawn;
        }

        private float GetProjectileX(ProjectileSpawn spawn)
        {
            if (mLastTargetMapId != Guid.Empty && mLastTargetMapId != spawn.MapId)
            {
                var map = MapController.Get(spawn.MapId);
                var grid = DbInterface.GetGrid(map.MapGrid);

                //loop through surrounding maps
                for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                {
                    for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
                    {
                        if (x < 0 || x >= grid.MapIdGrid.GetLength(0) || y < 0 || y >= grid.MapIdGrid.GetLength(1))
                        {
                            continue;
                        }

                        if (grid.MapIdGrid[x, y] != Guid.Empty && grid.MapIdGrid[x, y] == mLastTargetMapId)
                        {
                            var leftSide = x == map.MapGridX - 1;
                            var rightSide = x == map.MapGridX + 1;

                            if (leftSide)
                            {
                                return mLastTargetX - Options.MapWidth - spawn.X;
                            }

                            if (rightSide)
                            {
                                return mLastTargetX + Options.MapWidth - spawn.X;
                            }
                        }
                    }
                }
            }

            return mLastTargetX - spawn.X;
        }

        private float GetProjectileY(ProjectileSpawn spawn)
        {
            if (mLastTargetMapId != Guid.Empty && mLastTargetMapId != spawn.MapId)
            {
                var map = MapController.Get(spawn.MapId);
                var grid = DbInterface.GetGrid(map.MapGrid);

                //loop through surrounding maps
                for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                {
                    for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
                    {
                        if (x < 0 || x >= grid.MapIdGrid.GetLength(0) || y < 0 || y >= grid.MapIdGrid.GetLength(1))
                        {
                            continue;
                        }

                        if (grid.MapIdGrid[x, y] != Guid.Empty && grid.MapIdGrid[x, y] == mLastTargetMapId)
                        {
                            var topSide = y == map.MapGridY - 1;
                            var bottomSide = y == map.MapGridY + 1;

                            if (topSide)
                            {
                                return mLastTargetY - Options.MapHeight - spawn.Y;
                            }

                            if (bottomSide)
                            {
                                return mLastTargetY + Options.MapHeight - spawn.Y;
                            }
                        }
                    }
                }
            }

            return mLastTargetY - spawn.Y;
        }

        public bool MoveFragment(ProjectileSpawn spawn, bool move = true)
        {
            float newx = spawn.X;
            float newy = spawn.Y;
            var newMapId = spawn.MapId;

            if (move)
            {
                spawn.Distance++;
                spawn.TransmittionTimer += (long)(Base.Speed / (float)Base.Range);
                
                if (Target != default && Target.Id != Owner.Id && (Base.HomingBehavior || Base.DirectShotBehavior))
                {
                    //homing logic
                    mLastTargetX = Target.X;
                    mLastTargetY = Target.Y;
                    mLastTargetMapId = Target.MapId;
                    var directionX = GetProjectileX(spawn);
                    var directionY = GetProjectileY(spawn);
                    var length = Math.Sqrt(directionX * directionX + directionY * directionY);

                    newx += (float)(directionX / length);
                    newy += (float)(directionY / length);

                    if (Base.DirectShotBehavior)
                    {
                        Target = default;
                    }
                }
                else if (mLastTargetX != -1 && mLastTargetY != -1)
                {
                    //last target location logic
                    var directionX = GetProjectileX(spawn);
                    var directionY = GetProjectileY(spawn);
                    var length = Math.Sqrt(directionX * directionX + directionY * directionY);

                    newx += (float)(directionX / length);
                    newy += (float)(directionY / length);
                }
                else
                {
                    // Default logic
                    newx = spawn.X + GetRangeX(spawn.Dir, 1);
                    newy = spawn.Y + GetRangeY(spawn.Dir, 1);
                }
            }

            var killSpawn = false;
            var map = MapController.Get(spawn.MapId);

            if (Math.Floor(newx) < 0)
            {
                var leftMap = MapController.Get(map.Left);
                if (leftMap != null)
                {
                    newMapId = leftMap.Id;
                    newx = Options.MapWidth - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Ceiling(newx) > Options.MapWidth)
            {
                var rightMap = MapController.Get(map.Right);
                if (rightMap != null)
                {
                    newMapId = rightMap.Id;
                    newx = 0;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Floor(newy) < 0)
            {
                var upMap = MapController.Get(map.Up);
                if (upMap != null)
                {
                    newMapId = upMap.Id;
                    newy = Options.MapHeight - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Ceiling(newy) > Options.MapHeight)
            {
                var downMap = MapController.Get(map.Down);
                if (downMap != null)
                {
                    newMapId = downMap.Id;
                    newy = 0;
                }
                else
                {
                    killSpawn = true;
                }
            }

            spawn.X = newx;
            spawn.Y = newy;
            spawn.MapId = newMapId;

            return killSpawn;
        }

        public override void Die(bool dropItems = true, Entity killer = null)
        {
            for (var i = 0; i < Spawns.Length; i++)
            {
                Spawns[i] = null;
            }

            if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var mapInstance))
            {
                mapInstance.RemoveProjectile(this);
            }
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new ProjectileEntityPacket();
            }

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (ProjectileEntityPacket) packet;
            pkt.ProjectileId = Base.Id;
            pkt.ProjectileDirection = (byte) Dir;
            pkt.TargetId = Owner.Target != default && Owner.Target.Id != Guid.Empty ? Owner.Target.Id : Guid.Empty;
            pkt.OwnerId = Owner?.Id ?? Guid.Empty;

            return pkt;
        }

        public override EntityType GetEntityType()
        {
            return EntityType.Projectile;
        }

    }

}

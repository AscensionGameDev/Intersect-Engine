using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities.Combat;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

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

        public Projectile(
            Entity owner,
            SpellBase parentSpell,
            ItemBase parentItem,
            ProjectileBase projectile,
            Guid mapId,
            byte X,
            byte Y,
            byte z,
            byte direction,
            Entity target
        ) : base()
        {
            Base = projectile;
            Name = Base.Name;
            Owner = owner;
            Stat = owner.Stat;
            MapId = mapId;
            base.X = X;
            base.Y = Y;
            Z = z;
            SetMaxVital(Vitals.Health, 1);
            SetVital(Vitals.Health, 1);
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
                                FindProjectileRotationDir(Dir, d),
                                (byte) (X + FindProjectileRotationX(Dir, x - 2, y - 2)),
                                (byte) (Y + FindProjectileRotationY(Dir, x - 2, y - 2)), (byte) Z, MapId, Base, this
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
            mSpawnTime = Globals.Timing.Milliseconds + Base.Delay;
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

        private byte FindProjectileRotationDir(int entityDir, byte projectionDir)
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

        public void Update(List<Guid> projDeaths, List<KeyValuePair<Guid, int>> spawnDeaths)
        {
            if (mQuantity < Base.Quantity && Globals.Timing.Milliseconds > mSpawnTime)
            {
                AddProjectileSpawns(spawnDeaths);
            }

            ProcessFragments(projDeaths, spawnDeaths);
        }

        private int GetRangeX(int direction, int range)
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

        private int GetRangeY(int direction, int range)
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
                        while (Globals.Timing.Milliseconds > spawn.TransmittionTimer && Spawns[i] != null)
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
                    Die(0, null);
                }
            }
        }

        public bool CheckForCollision(ProjectileSpawn spawn)
        {
            var killSpawn = MoveFragment(spawn, false);

            //Check Map Entities For Hits
            var map = MapInstance.Get(spawn.MapId);
            if (!killSpawn && map != null)
            {
                var attribute = map.Attributes[(int)spawn.X, (int)spawn.Y];

                //Check for Z-Dimension
                if (!spawn.ProjectileBase.IgnoreZDimension)
                {
                    if (attribute != null && attribute.Type == MapAttributes.ZDimension)
                    {
                        if (((MapZDimensionAttribute) attribute).GatewayTo > 0)
                        {
                            spawn.Z = (byte) (((MapZDimensionAttribute) attribute).GatewayTo - 1);
                        }
                    }
                }

                //Check for grapplehooks.
                if (attribute != null &&
                    attribute.Type == MapAttributes.GrappleStone &&
                    Base.GrappleHook == true &&
                    !spawn.Parent.HasGrappled)
                {
                    if (spawn.Dir <= 3) //Don't handle directional projectile grapplehooks
                    {
                        spawn.Parent.HasGrappled = true;

                        //Only grapple if the player hasnt left the firing position.. if they have then we assume they dont wanna grapple
                        if (Owner.X == X && Owner.Y == Y && Owner.MapId == MapId)
                        {
                            Owner.Dir = spawn.Dir;
                            new Dash(
                                Owner, spawn.Distance, (byte) Owner.Dir, Base.IgnoreMapBlocks,
                                Base.IgnoreActiveResources, Base.IgnoreExhaustedResources, Base.IgnoreZDimension
                            );
                        }

                        killSpawn = true;
                    }
                }

                if (attribute != null &&
                    (attribute.Type == MapAttributes.Blocked || attribute.Type == MapAttributes.Animation && ((MapAnimationAttribute)attribute).IsBlock) &&
                    !spawn.ProjectileBase.IgnoreMapBlocks)
                {
                    killSpawn = true;
                }
            }

            if (!killSpawn && map != null)
            {
                var entities = map.GetEntities();
                for (var z = 0; z < entities.Count; z++)
                {
                    if (entities[z] != null &&
                        (entities[z].X == Math.Round(spawn.X) || entities[z].X == Math.Ceiling(spawn.X) || entities[z].X == Math.Floor(spawn.X)) &&
                        (entities[z].Y == Math.Round(spawn.Y) || entities[z].Y == Math.Ceiling(spawn.Y) || entities[z].Y == Math.Floor(spawn.Y)) &&
                        entities[z].Z == spawn.Z)
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

        public bool MoveFragment(ProjectileSpawn spawn, bool move = true)
        {
            float newx = spawn.X;
            float newy = spawn.Y;
            var newMapId = spawn.MapId;

            if (move)
            {
                spawn.Distance++;
                spawn.TransmittionTimer += (long)((float)Base.Speed / (float)Base.Range);
                newx = spawn.X + GetRangeX(spawn.Dir, 1);
                newy = spawn.Y + GetRangeY(spawn.Dir, 1);
            }

            var killSpawn = false;
            var map = MapInstance.Get(spawn.MapId);

            if (Math.Round(newx) < 0)
            {
                if (MapInstance.Get(map.Left) != null)
                {
                    newMapId = MapInstance.Get(spawn.MapId).Left;
                    newx = Options.MapWidth - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Round(newx) > Options.MapWidth - 1)
            {
                if (MapInstance.Get(map.Right) != null)
                {
                    newMapId = MapInstance.Get(spawn.MapId).Right;
                    newx = 0;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Round(newy) < 0)
            {
                if (MapInstance.Get(map.Up) != null)
                {
                    newMapId = MapInstance.Get(spawn.MapId).Up;
                    newy = Options.MapHeight - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }

            if (Math.Round(newy) > Options.MapHeight - 1)
            {
                if (MapInstance.Get(map.Down) != null)
                {
                    newMapId = MapInstance.Get(spawn.MapId).Down;
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

        public override void Die(int dropitems = 0, Entity killer = null)
        {
            for (var i = 0; i < Spawns.Length; i++)
            {
                Spawns[i] = null;
            }

            MapInstance.Get(MapId).RemoveProjectile(this);
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
            pkt.TargetId = Target?.Id ?? Guid.Empty;
            pkt.OwnerId = Owner?.Id ?? Guid.Empty;

            return pkt;
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Projectile;
        }

    }

}

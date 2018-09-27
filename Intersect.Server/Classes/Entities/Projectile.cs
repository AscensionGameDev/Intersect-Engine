using System;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using MapAttribute = Intersect.GameObjects.Maps.MapAttribute;

namespace Intersect.Server.Entities
{
    public class Projectile : EntityInstance
    {
        private int mSpawnCount;
        private int mSpawnedAmount;
        private int mTotalSpawns;
        public ItemBase Item;
        public ProjectileBase Base;
        public EntityInstance Owner;
        private int mQuantity;
        public bool HasGrappled;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;

        private long mSpawnTime;
        public SpellBase Spell;
        public EntityInstance Target;

        public Projectile(EntityInstance owner, SpellBase parentSpell, ItemBase parentItem, ProjectileBase projectile, Guid mapId, int X, int Y, int z, int direction, EntityInstance target) : base()
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
            for (int x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (int y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (int d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (Base.SpawnLocations[x, y].Directions[d] == true)
                        {
                            mTotalSpawns++;
                        }
                    }
                }
            }
            mTotalSpawns *= Base.Quantity;
            Spawns = new ProjectileSpawns[mTotalSpawns];
        }

        private void AddProjectileSpawns()
        {
            for (int x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (int y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (int d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (Base.SpawnLocations[x, y].Directions[d] == true && mSpawnedAmount < Spawns.Length)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d),
                                X + FindProjectileRotationX(Dir, x - 2, y - 2),
                                Y + FindProjectileRotationY(Dir, x - 2, y - 2), Z, MapId, Base,
                                this);
                            Spawns[mSpawnedAmount] = s;
                            mSpawnedAmount++;
                            mSpawnCount++;
                            if (CheckForCollision(s)) KillSpawn(s);
                        }
                    }
                }
            }
            mQuantity++;
            mSpawnTime = Globals.System.GetTimeMs() + Base.Delay;
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

        public void Update()
        {
            if (mQuantity < Base.Quantity && Globals.System.GetTimeMs() > mSpawnTime)
            {
                AddProjectileSpawns();
            }
            ProcessFragments();
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

        public void ProcessFragments()
        {
            if (mSpawnCount != 0 || mQuantity < Base.Quantity)
            {
                for (int i = 0; i < mSpawnedAmount; i++)
                {
                    if (Spawns[i] != null && Globals.System.GetTimeMs() > Spawns[i].TransmittionTimer)
                    {
                        var killSpawn = MoveFragment(Spawns[i]);
                        if (!killSpawn) killSpawn = CheckForCollision(Spawns[i]);

                        if (killSpawn)
                        {
                            Spawns[i].Dispose(i);
                            Spawns[i] = null;
                            mSpawnCount--;
                            continue;
                        }
                    }
                }
            }
            else
            {
                Die(0, null);
            }
        }

        public void KillSpawn(ProjectileSpawns spawn)
        {
            if (spawn != null && Spawns.Contains(spawn))
            {
                for (int i = 0; i < Spawns.Length; i++)
                {
                    if (spawn == Spawns[i])
                    {
                        Spawns[i].Dispose(i);
                        Spawns[i] = null;
                        mSpawnCount--;
                    }
                }
            }
        }

        public bool CheckForCollision(ProjectileSpawns spawn)
        {
            var killSpawn = MoveFragment(spawn, false);
            //Check Map Entities For Hits
            var map = MapInstance.Get(spawn.MapId);
            if (!killSpawn && map != null)
            {
                MapAttribute attribute = map.Attributes[spawn.X, spawn.Y];
                //Check for Z-Dimension
                if (!spawn.ProjectileBase.IgnoreZDimension)
                {
                    if (attribute != null && attribute.Type == MapAttributes.ZDimension)
                    {
                        if (((MapZDimensionAttribute)attribute).GatewayTo > 0)
                        {
                            spawn.Z = ((MapZDimensionAttribute)attribute).GatewayTo - 1;
                        }
                    }
                }
                //Check for grapplehooks.
                if (attribute != null && attribute.Type == MapAttributes.GrappleStone && Base.GrappleHook == true && !spawn.Parent.HasGrappled)
                {
                    if (spawn.Dir <= 3) //Don't handle directional projectile grapplehooks
                    {
                        spawn.Parent.HasGrappled = true;
                        Owner.Dir = spawn.Dir;
                        new DashInstance(Owner, spawn.Distance, Owner.Dir,Base.IgnoreMapBlocks,Base.IgnoreActiveResources,Base.IgnoreExhaustedResources, Base.IgnoreZDimension);
                        killSpawn = true;
                    }
                }
                if (attribute != null && attribute.Type == MapAttributes.Blocked && !spawn.ProjectileBase.IgnoreMapBlocks)
                {
                    killSpawn = true;
                }
            }

            if (!killSpawn && map != null)
            {
                var entities = map.GetEntities();
                for (int z = 0; z < entities.Count; z++)
                {
                    if (entities[z] != null && entities[z].X == spawn.X &&
                        entities[z].Y == spawn.Y && entities[z].Z == spawn.Z)
                    {
                        killSpawn = spawn.HitEntity(entities[z]);
                    }
                    else
                    {
                        if (z == entities.Count - 1)
                        {
                            spawn.TransmittionTimer = Globals.System.GetTimeMs() +
                                                      (long)
                                                      ((float) Base.Speed / (float) Base.Range);
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

        public bool MoveFragment(ProjectileSpawns spawn, bool move = true)
        {
            int newx = spawn.X;
            int newy = spawn.Y;
            Guid newMapId = spawn.MapId;
            if (move)
            {
                spawn.Distance++;
                newx = spawn.X + GetRangeX(spawn.Dir, 1);
                newy = spawn.Y + GetRangeY(spawn.Dir, 1);
            }
            var killSpawn = false;
            var map = MapInstance.Get(spawn.MapId);

            if (newx < 0)
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
            if (newx > Options.MapWidth - 1)
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
            if (newy < 0)
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
            if (newy > Options.MapHeight - 1)
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

        public override void Die(int dropitems = 0, EntityInstance killer = null)
        {
            for (int i = 0; i < Spawns.Length; i++)
            {
                if (Spawns[i] != null)
                {
                    Spawns[i].Dispose(i);
                    Spawns[i] = null;
                }
            }
            MapInstance.Get(MapId).RemoveProjectile(this);
            PacketSender.SendEntityLeave(Id, (int) EntityTypes.Projectile, MapId);
        }

        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteGuid(Base.Id);
            bf.WriteInteger(Dir);
            if (Target == null)
            {
                bf.WriteGuid(Guid.Empty);
            }
            else
            {
                bf.WriteGuid(Target.Id);
            }
            bf.WriteGuid(Owner != null ? Owner.Id : Guid.Empty);
            return bf.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Projectile;
        }
    }

    public class ProjectileSpawns
    {
        public int Dir;
        public int Distance;
        public Guid MapId;
        public Projectile Parent;
        public ProjectileBase ProjectileBase;
        public long TransmittionTimer = Globals.System.GetTimeMs();
        public int X;
        public int Y;
        public int Z;

        public ProjectileSpawns(int dir, int x, int y, int z, Guid mapId, ProjectileBase projectileBase, Projectile parent)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Z = z;
            Dir = dir;
            ProjectileBase = projectileBase;
            Parent = parent;
            TransmittionTimer = Globals.System.GetTimeMs() +
                                (long) ((float) ProjectileBase.Speed / (float) ProjectileBase.Range);
        }

        public bool HitEntity(EntityInstance en)
        {
            var targetEntity = en;
            if (targetEntity != null && targetEntity != Parent.Owner)
            {
                if (targetEntity.GetType() == typeof(Player)) //Player
                {
                    if (Parent.Owner != Parent.Target)
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks, Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources, Parent.Base.IgnoreZDimension);
                        }
                        return true;
                    }
                }
                else if (targetEntity.GetType() == typeof(Resource))
                {
                    if ((((Resource) targetEntity).IsDead &&
                         !ProjectileBase.IgnoreExhaustedResources) ||
                        (!((Resource) targetEntity).IsDead &&
                         !ProjectileBase.IgnoreActiveResources))
                    {
                        if (Parent.Owner.GetType() == typeof(Player) && !((Resource)targetEntity).IsDead)
                        {
                            Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                            if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                            {
                                Parent.HasGrappled = true;
                                Parent.Owner.Dir = Dir;
                                new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks, Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources, Parent.Base.IgnoreZDimension);
                            }
                        }
                        return true;
                    }
                }
                else //Any other Parent.Target
                {
                    var ownerNpc = Parent.Owner as Npc;
                    if (ownerNpc == null || ownerNpc.CanNpcCombat(targetEntity, Parent.Spell != null && Parent.Spell.Combat.Friendly))
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.Base, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.Base.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.Base.IgnoreMapBlocks, Parent.Base.IgnoreActiveResources, Parent.Base.IgnoreExhaustedResources, Parent.Base.IgnoreZDimension);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void Dispose(int spawnIndex)
        {
            PacketSender.SendRemoveProjectileSpawn(MapId, Parent.Id, spawnIndex);
        }
    }
}
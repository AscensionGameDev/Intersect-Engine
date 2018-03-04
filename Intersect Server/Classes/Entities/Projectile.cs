using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.Entities
{
    public class Projectile : EntityInstance
    {
        private int mSpawnCount;
        private int mSpawnedAmount;
        private int mTotalSpawns;
        public ItemBase Item;
        public ProjectileBase MyBase;
        public EntityInstance Owner;
        private int mQuantity;
        public bool HasGrappled;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;

        private long mSpawnTime;
        public SpellBase Spell;
        public EntityInstance Target;

        public Projectile(int index, EntityInstance owner, SpellBase parentSpell, ItemBase parentItem,
            ProjectileBase projectile,
            int mapIndex, int X, int Y, int z, int direction, EntityInstance target) : base(index, new EntityBase())
        {
            MyBase = projectile;
            Name = MyBase.Name;
            Owner = owner;
            Stat = owner.Stat;
            Vital[(int) Vitals.Health] = 1;
            MaxVital[(int) Vitals.Health] = 1;
            MapIndex = mapIndex;
            base.X = X;
            base.Y = Y;
            Z = z;
            Dir = direction;
            Spell = parentSpell;
            Item = parentItem;

            if (MyBase.Homing == true)
            {
                Target = target;
            }

            Passable = 1;
            HideName = 1;
            for (int x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (int y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (int d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (MyBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            mTotalSpawns++;
                        }
                    }
                }
            }
            mTotalSpawns *= MyBase.Quantity;
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
                        if (MyBase.SpawnLocations[x, y].Directions[d] == true && mSpawnedAmount < Spawns.Length)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d),
                                X + FindProjectileRotationX(Dir, x - 2, y - 2),
                                Y + FindProjectileRotationY(Dir, x - 2, y - 2), Z, MapIndex, MyBase,
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
            mSpawnTime = Globals.System.GetTimeMs() + MyBase.Delay;
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
            if (mQuantity < MyBase.Quantity && Globals.System.GetTimeMs() > mSpawnTime)
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
            if (mSpawnCount != 0 || mQuantity < MyBase.Quantity)
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
            var map = MapInstance.Lookup.Get<MapInstance>(spawn.Map);
            if (!killSpawn && map != null)
            {
                Attribute attribute = map.Attributes[spawn.X, spawn.Y];
                //Check for Z-Dimension
                if (!spawn.ProjectileBase.IgnoreZDimension)
                {
                    if (attribute != null && attribute.Value == (int) MapAttributes.ZDimension)
                    {
                        if (attribute.Data1 > 0)
                        {
                            spawn.Z = attribute.Data1 - 1;
                        }
                    }
                }
                //Check for grapplehooks.
                if (attribute != null && attribute.Value == (int) MapAttributes.GrappleStone &&
                    MyBase.GrappleHook == true && !spawn.Parent.HasGrappled)
                {
                    if (spawn.Dir <= 3) //Don't handle directional projectile grapplehooks
                    {
                        spawn.Parent.HasGrappled = true;
                        Owner.Dir = spawn.Dir;
                        new DashInstance(Owner, spawn.Distance, Owner.Dir,MyBase.IgnoreMapBlocks,MyBase.IgnoreActiveResources,MyBase.IgnoreExhaustedResources, MyBase.IgnoreZDimension);
                        killSpawn = true;
                    }
                }
                if (attribute != null && attribute.Value == (int) MapAttributes.Blocked &&
                    !spawn.ProjectileBase.IgnoreMapBlocks)
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
                                                      ((float) MyBase.Speed / (float) MyBase.Range);
                            if (spawn.Distance >= MyBase.Range)
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
            int newmap = spawn.Map;
            if (move)
            {
                spawn.Distance++;
                newx = spawn.X + GetRangeX(spawn.Dir, 1);
                newy = spawn.Y + GetRangeY(spawn.Dir, 1);
            }
            var killSpawn = false;
            var map = MapInstance.Lookup.Get<MapInstance>(spawn.Map);

            if (newx < 0)
            {
                if (MapInstance.Lookup.Get<MapInstance>(map.Left) != null)
                {
                    newmap = MapInstance.Lookup.Get<MapInstance>(spawn.Map).Left;
                    newx = Options.MapWidth - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }
            if (newx > Options.MapWidth - 1)
            {
                if (MapInstance.Lookup.Get<MapInstance>(map.Right) != null)
                {
                    newmap = MapInstance.Lookup.Get<MapInstance>(spawn.Map).Right;
                    newx = 0;
                }
                else
                {
                    killSpawn = true;
                }
            }
            if (newy < 0)
            {
                if (MapInstance.Lookup.Get<MapInstance>(map.Up) != null)
                {
                    newmap = MapInstance.Lookup.Get<MapInstance>(spawn.Map).Up;
                    newy = Options.MapHeight - 1;
                }
                else
                {
                    killSpawn = true;
                }
            }
            if (newy > Options.MapHeight - 1)
            {
                if (MapInstance.Lookup.Get<MapInstance>(map.Down) != null)
                {
                    newmap = MapInstance.Lookup.Get<MapInstance>(spawn.Map).Down;
                    newy = 0;
                }
                else
                {
                    killSpawn = true;
                }
            }
            spawn.X = newx;
            spawn.Y = newy;
            spawn.Map = newmap;
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
            MapInstance.Lookup.Get<MapInstance>(MapIndex).RemoveProjectile(this);
            PacketSender.SendEntityLeave(MyIndex, (int) EntityTypes.Projectile, MapIndex);
            Globals.Entities[MyIndex] = null;
        }

        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(MyBase.Index);
            bf.WriteInteger(Dir);
            if (Target == null)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                bf.WriteInteger(Target.MyIndex);
            }
            bf.WriteInteger(Owner != null ? Owner.MyIndex : -1);
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
        public int Map;
        public Projectile Parent;
        public ProjectileBase ProjectileBase;
        public long TransmittionTimer = Globals.System.GetTimeMs();
        public int X;
        public int Y;
        public int Z;

        public ProjectileSpawns(int dir, int x, int y, int z, int map, ProjectileBase projectileBase, Projectile parent)
        {
            Map = map;
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
                        Parent.Owner.TryAttack(targetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.MyBase.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.MyBase.IgnoreMapBlocks, Parent.MyBase.IgnoreActiveResources, Parent.MyBase.IgnoreExhaustedResources, Parent.MyBase.IgnoreZDimension);
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
                            Parent.Owner.TryAttack(targetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                            if (Dir <= 3 && Parent.MyBase.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                            {
                                Parent.HasGrappled = true;
                                Parent.Owner.Dir = Dir;
                                new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.MyBase.IgnoreMapBlocks, Parent.MyBase.IgnoreActiveResources, Parent.MyBase.IgnoreExhaustedResources, Parent.MyBase.IgnoreZDimension);
                            }
                        }
                        return true;
                    }
                }
                else //Any other Parent.Target
                {
                    var ownerNpc = Parent.Owner as Npc;
                    if (ownerNpc == null || ownerNpc.CanNpcCombat(targetEntity, Parent.Spell != null && Parent.Spell.Friendly == 1))
                    {
                        Parent.Owner.TryAttack(targetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.MyBase.GrappleHook && !Parent.HasGrappled) //Don't handle directional projectile grapplehooks
                        {
                            Parent.HasGrappled = true;
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir, Parent.MyBase.IgnoreMapBlocks, Parent.MyBase.IgnoreActiveResources, Parent.MyBase.IgnoreExhaustedResources, Parent.MyBase.IgnoreZDimension);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void Dispose(int spawnIndex)
        {
            PacketSender.SendRemoveProjectileSpawn(Map, Parent.MyIndex, spawnIndex);
        }
    }
}
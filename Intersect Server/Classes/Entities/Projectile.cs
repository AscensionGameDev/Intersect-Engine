using System.Collections.Generic;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.Entities
{
    public class Projectile : Entity
    {
        private int _spawnCount;
        private int _spawnedAmount;
        private int _totalSpawns;
        public ProjectileBase MyBase;
        public Entity Owner;
        public ItemBase Item;
        public SpellBase Spell;
        private int Quantity;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;
        private long SpawnTime;
        public Entity Target;

        public Projectile(int index, Entity owner, SpellBase parentSpell, ItemBase parentItem, ProjectileBase projectile,
            int Map, int X, int Y, int Z, int Direction, Entity target) : base(index)
        {
            MyBase = projectile;
            MyName = MyBase.Name;
            Owner = owner;
            Stat = owner.Stat;
            Vital[(int) Vitals.Health] = 1;
            MaxVital[(int) Vitals.Health] = 1;
            CurrentMap = Map;
            CurrentX = X;
            CurrentY = Y;
            CurrentZ = Z;
            Dir = Direction;
            Spell = parentSpell;
            Item = parentItem;

            if (MyBase.Homing == true)
            {
                Target = target;
            }

            Passable = 1;
            HideName = 1;
            for (int x = 0; x < ProjectileBase.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileBase.SpawnLocationsHeight; y++)
                {
                    for (int d = 0; d < ProjectileBase.MaxProjectileDirections; d++)
                    {
                        if (MyBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            _totalSpawns++;
                        }
                    }
                }
            }
            _totalSpawns *= MyBase.Quantity;
            Spawns = new ProjectileSpawns[_totalSpawns];
        }

        private void AddProjectileSpawns()
        {
            for (int x = 0; x < ProjectileBase.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileBase.SpawnLocationsHeight; y++)
                {
                    for (int d = 0; d < ProjectileBase.MaxProjectileDirections; d++)
                    {
                        if (MyBase.SpawnLocations[x, y].Directions[d] == true && _spawnedAmount < Spawns.Length)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d),
                                CurrentX + FindProjectileRotationX(Dir, x - 2, y - 2),
                                CurrentY + FindProjectileRotationY(Dir, x - 2, y - 2), CurrentZ, CurrentMap, MyBase,
                                this);
                            Spawns[_spawnedAmount] = s;
                            _spawnedAmount++;
                            _spawnCount++;
                        }
                    }
                }
            }
            Quantity++;
            SpawnTime = Globals.System.GetTimeMs() + MyBase.Delay;
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
            if (Quantity < MyBase.Quantity && Globals.System.GetTimeMs() > SpawnTime)
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
            if (_spawnCount != 0 || Quantity < MyBase.Quantity)
            {
                for (int i = 0; i < _spawnedAmount; i++)
                {
                    if (Spawns[i] != null && Globals.System.GetTimeMs() > Spawns[i].TransmittionTimer)
                    {
                        var killSpawn = MoveFragment(Spawns[i]);
                        if (!killSpawn) killSpawn = CheckForCollision(Spawns[i]);
                        

                        if (killSpawn)
                        {
                            Spawns[i].Dispose(i);
                            Spawns[i] = null;
                            _spawnCount--;
                            continue;
                        }
                    }
                }
            }
            else
            {
                Die(0,null);
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
                        _spawnCount--;
                    }
                }
            }
        }

        public bool CheckForCollision(ProjectileSpawns spawn)
        {
            var killSpawn = false;
            //Check Map Entities For Hits
            var map = MapInstance.Lookup.Get<MapInstance>(spawn.Map);
            Attribute attribute = map.Attributes[spawn.X, spawn.Y];
            //Check for Z-Dimension
            if (!spawn.ProjectileBase.IgnoreZDimension)
            {
                if (attribute != null && attribute.value == (int)MapAttributes.ZDimension)
                {
                    if (attribute.data1 > 0)
                    {
                        spawn.Z = attribute.data1 - 1;
                    }
                }
                //Check for grapplehooks.
                if (attribute != null && attribute.value == (int)MapAttributes.GrappleStone &&
                    MyBase.GrappleHook == true)
                {
                    if (spawn.Dir <= 3) //Don't handle directional projectile grapplehooks
                    {
                        Owner.Dir = spawn.Dir;
                        new DashInstance(Owner, spawn.Distance, Owner.Dir);
                        killSpawn = true;
                    }
                }
            }
            if (attribute != null && attribute.value == (int)MapAttributes.Blocked &&
                !spawn.ProjectileBase.IgnoreMapBlocks)
            {
                if (spawn.Dir <= 3) //Don't handle directional projectile grapplehooks
                {
                    Owner.Dir = spawn.Dir;
                    new DashInstance(Owner, spawn.Distance, Owner.Dir);
                    killSpawn = true;
                }
            }

            if (!killSpawn && map != null)
            {
                var entities = map.GetEntities();
                for (int z = 0; z < entities.Count; z++)
                {
                    if (entities[z] != null && entities[z].CurrentX == spawn.X &&
                        entities[z].CurrentY == spawn.Y && entities[z].CurrentZ == spawn.Z)
                    {
                        killSpawn = spawn.HitEntity(entities[z]);
                    }
                    else
                    {
                        if (z == entities.Count - 1)
                        {
                            spawn.TransmittionTimer = Globals.System.GetTimeMs() +
                                                          (long)
                                                          ((float)MyBase.Speed / (float)MyBase.Range);
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

        public bool MoveFragment(ProjectileSpawns spawn)
        {
            spawn.Distance++;
            int newx = spawn.X + GetRangeX(spawn.Dir, 1);
            int newy = spawn.Y + GetRangeY(spawn.Dir, 1);
            int newmap = spawn.Map;
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

        public override void Die(int dropitems = 0, Entity killer = null)
        {
            for (int i = 0; i < Spawns.Length; i++)
            {
                if (Spawns[i] != null)
                {
                    Spawns[i].Dispose(i);
                    Spawns[i] = null;
                }
            }
            MapInstance.Lookup.Get<MapInstance>(CurrentMap).RemoveProjectile(this);
            PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Projectile, CurrentMap);
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
        public ProjectileBase ProjectileBase;
        public long TransmittionTimer = Globals.System.GetTimeMs();
        public int X;
        public int Y;
        public int Z;
        public Projectile Parent;

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

        public bool HitEntity(Entity en)
        {
            var TargetEntity = en;
            if (TargetEntity != null && TargetEntity != Parent.Owner)
            {
                if (TargetEntity.GetType() == typeof(Player)) //Player
                {
                    if (Parent.Owner != Parent.Target)
                    {
                        Parent.Owner.TryAttack(TargetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.MyBase.GrappleHook == true) //Don't handle directional projectile grapplehooks
                        {
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir);
                        }
                        return true;
                    }
                }
                else if (TargetEntity.GetType() == typeof(Resource))
                {
                    if ((((Resource)TargetEntity).IsDead &&
                         !ProjectileBase.IgnoreExhaustedResources) ||
                        (!((Resource)TargetEntity).IsDead &&
                         !ProjectileBase.IgnoreActiveResources))
                    {
                        if (Parent.Owner.GetType() == typeof(Player))
                        {
                            Parent.Owner.TryAttack(TargetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                            if (Dir <= 3 && Parent.MyBase.GrappleHook == true) //Don't handle directional projectile grapplehooks
                            {
                                Parent.Owner.Dir = Dir;
                                new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir);
                            }
                            return true;
                        }
                    }
                }
                else //Any other Parent.Target
                {
                    var OwnerNpc = Parent.Owner as Npc;
                    if (OwnerNpc == null || OwnerNpc.CanNpcCombat(TargetEntity))
                    {
                        Parent.Owner.TryAttack(TargetEntity, Parent.MyBase, Parent.Spell, Parent.Item, Dir);
                        if (Dir <= 3 && Parent.MyBase.GrappleHook == true) //Don't handle directional projectile grapplehooks
                        {
                            Parent.Owner.Dir = Dir;
                            new DashInstance(Parent.Owner, Distance, Parent.Owner.Dir);
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
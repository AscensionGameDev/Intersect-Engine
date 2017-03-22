

using System;
using System.Collections.Generic;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;
using Attribute = Intersect_Library.GameObjects.Maps.Attribute;


namespace Intersect_Server.Classes.Entities
{
    public class Projectile : Entity
    {
        public Entity Owner;
        private ProjectileBase MyBase;
        private SpellBase ParentSpell;
        private ItemBase ParentItem;
        private int Quantity = 0;
        private long SpawnTime = 0;
        public int Target = -1;
        private int _spawnCount = 0;
        private int _totalSpawns = 0;
        private int _spawnedAmount = 0;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;

        public Projectile(int index, Entity owner, SpellBase parentSpell, ItemBase parentItem, ProjectileBase projectile, int Map, int X, int Y, int Z, int Direction, int target = 0) : base(index)
        {
            MyBase = projectile;
            MyName = MyBase.Name;
            Owner = owner;
            Stat = owner.Stat;
            Vital[(int)Vitals.Health] = 1;
            MaxVital[(int)Vitals.Health] = 1;
            CurrentMap = Map;
            CurrentX = X;
            CurrentY = Y;
            CurrentZ = Z;
            Dir = Direction;
            ParentSpell = parentSpell;
            ParentItem = parentItem;

            if (MyBase.Homing == true)
            {
                Target = target;
            }

            Passable = 1;
            HideName = 1;
            for (int x = 0; x < ProjectileBase.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileBase.SpawnLocationsWidth; y++)
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
                for (int y = 0; y < ProjectileBase.SpawnLocationsWidth; y++)
                {
                    for (int d = 0; d < ProjectileBase.MaxProjectileDirections; d++)
                    {
                        if (MyBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d), CurrentX + FindProjectileRotationX(Dir, x - 2, y - 2), CurrentY + FindProjectileRotationY(Dir, x - 2, y - 2), CurrentZ, CurrentMap, MyBase, MyIndex);
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
            CheckForCollision();
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

        public void CheckForCollision()
        {
            if (_spawnCount != 0 || Quantity < MyBase.Quantity)
            {
                for (int i = 0; i < _spawnedAmount; i++)
                {
                    if (Spawns[i] != null && Globals.System.GetTimeMs() > Spawns[i].TransmittionTimer)
                    {
                        Spawns[i].Distance++;
                        bool killSpawn = false;
                        int newx = Spawns[i].X + GetRangeX(Spawns[i].Dir, 1);
                        int newy = Spawns[i].Y + GetRangeY(Spawns[i].Dir, 1);
                        int newmap = Spawns[i].Map;
                        var map = MapInstance.GetMap(Spawns[i].Map);

                        if (newx < 0)
                        {
                            if (MapInstance.GetMap(map.Left) != null)
                            {
                                newmap = MapInstance.GetMap(Spawns[i].Map).Left;
                                newx = Options.MapWidth - 1;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newx > Options.MapWidth - 1)
                        {
                            if (MapInstance.GetMap(map.Right) != null)
                            {
                                newmap = MapInstance.GetMap(Spawns[i].Map).Right;
                                newx = 0;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newy < 0)
                        {
                            if (MapInstance.GetMap(map.Up) != null)
                            {
                                newmap = MapInstance.GetMap(Spawns[i].Map).Up;
                                newy = Options.MapHeight - 1;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newy > Options.MapHeight - 1)
                        {
                            if (MapInstance.GetMap(map.Down) != null)
                            {
                                newmap = MapInstance.GetMap(Spawns[i].Map).Down;
                                newy = 0;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }

                        if (killSpawn)
                        {
                            Spawns[i].Dispose(i);
                            Spawns[i] = null;
                            _spawnCount--;
                            continue;
                        }

                        Spawns[i].X = newx;
                        Spawns[i].Y = newy;
                        Spawns[i].Map = newmap;
                        
                        //Check Map Entities For Hits
                        map = MapInstance.GetMap(Spawns[i].Map);
                        Attribute attribute = map.Attributes[Spawns[i].X, Spawns[i].Y];
                        //Check for Z-Dimension
                        if (!Spawns[i].ProjectileBase.IgnoreZDimension)
                        {
                            if (attribute != null && attribute.value == (int) MapAttributes.ZDimension)
                            {
                                if (attribute.data1 > 0)
                                {
                                    Spawns[i].Z = attribute.data1 - 1;
                                }
                            }
                            //Check for grapplehooks.
                            if (attribute != null && attribute.value == (int)MapAttributes.GrappleStone && MyBase.GrappleHook == true)
                            {
                                if (Spawns[i].Dir <= 3 ) //Don't handle directional projectile grapplehooks
                                {
                                    Owner.Dir = Spawns[i].Dir;
                                    new DashInstance(Owner, Spawns[i].Distance, Owner.Dir);
                                    killSpawn = true;
                                }
                            }
                        }
                        if (attribute != null && attribute.value == (int) MapAttributes.Blocked &&
                            !Spawns[i].ProjectileBase.IgnoreMapBlocks)
                        {
                            killSpawn = true;
                        }
                        
                        if (!killSpawn && map != null)
                        {
                            var entities = map.GetEntities();
                            for (int z = 0; z < entities.Count; z++)
                            {
                                if (entities[z] != null && entities[z].CurrentX == Spawns[i].X &&
                                    entities[z].CurrentY == Spawns[i].Y && entities[z].CurrentZ == Spawns[i].Z)
                                {
                                    var TargetEntity = entities[z];
                                    if (TargetEntity != null && TargetEntity != Owner)
                                    {
                                        if (TargetEntity.GetType() == typeof(Player)) //Player
                                        {
                                            if (Owner.MyIndex != Target)
                                            {
                                                Owner.TryAttack(TargetEntity, MyBase, ParentSpell, ParentItem, Spawns[i].Dir,new List<KeyValuePair<int, int>>(), new List<KeyValuePair<int, int>>());
                                                killSpawn = true; //Remove from the list being processed
                                            }
                                        }
                                        else if (TargetEntity.GetType() == typeof(Resource))
                                        {
                                            if ((((Resource)TargetEntity).IsDead && !Spawns[i].ProjectileBase.IgnoreExhaustedResources) || (!((Resource)TargetEntity).IsDead && !Spawns[i].ProjectileBase.IgnoreActiveResources))
                                            {
                                                if (Owner.GetType() == typeof(Player))
                                                {

                                                    Owner.TryAttack(TargetEntity, MyBase, ParentSpell,ParentItem, Spawns[i].Dir, new List<KeyValuePair<int, int>>(), new List<KeyValuePair<int, int>>());
                                                    killSpawn = true; //Remove from the list being processed
                                                }
                                            }
                                        }
                                        else //Any other target
                                        {
                                            Owner.TryAttack(TargetEntity, MyBase, ParentSpell, ParentItem, Spawns[i].Dir, new List<KeyValuePair<int, int>>(), new List<KeyValuePair<int, int>>());
                                            killSpawn = true; //Remove from the list being processed
                                        }
                                    }
                                }
                                else
                                {
                                    if (z == entities.Count - 1)
                                    {
                                        Spawns[i].TransmittionTimer = Globals.System.GetTimeMs() + (long)((float)MyBase.Speed / (float)MyBase.Range);
                                        if (Spawns[i].Distance >= MyBase.Range)
                                        {
                                            killSpawn = true;
                                        }
                                    }
                                }
                            }
                        }

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
                MapInstance.GetMap(CurrentMap).RemoveProjectile(this);
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Projectile, CurrentMap);
                Globals.Entities[MyIndex] = null;
            }
        }

        public override byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(MyBase.Id);
            bf.WriteInteger(Dir);
            bf.WriteInteger(Target);
            return bf.ToArray();
        }
        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Projectile;
        }
    }

    public class ProjectileSpawns
    {
        public int X;
        public int Y;
        public int Z;
        public int Map;
        public int Dir;
        public int Distance = 0;
        public ProjectileBase ProjectileBase;
        public long TransmittionTimer = Globals.System.GetTimeMs();
        private int _baseEntityIndex;

        public ProjectileSpawns(int dir, int x, int y, int z, int map, ProjectileBase projectileBase, int parentIndex)
        {
            Map = map;
            X = x;
            Y = y;
            Z = z;
            Dir = dir;
            ProjectileBase = projectileBase;
            _baseEntityIndex = parentIndex;
            TransmittionTimer = Globals.System.GetTimeMs() + (long)((float)ProjectileBase.Speed / (float)ProjectileBase.Range);
        }

        public void Dispose(int spawnIndex)
        {
            PacketSender.SendRemoveProjectileSpawn(Map,_baseEntityIndex, spawnIndex);
        }
    }
}

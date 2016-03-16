/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Threading;

namespace Intersect_Server.Classes
{
    public class Projectile : Entity
    {
        public int OwnerID = 0;
        public Type OwnerType = null;
        private ProjectileStruct MyBase;
        private int ProjectileNum = 0;
        private int Quantity = 0;
        private long SpawnTime = 0;
        public int Target = 0;
        private bool IsSpell = false;

        // Individual Spawns
        public List<ProjectileSpawns> Spawns = new List<ProjectileSpawns>();

        public Projectile(int index, int ownerID, Type ownerType, int projectileNum, int Map, int X, int Y, int Z, int Direction, bool isSpell = false, int target = 0) : base(index)
        {
            ProjectileNum = projectileNum;
            MyBase = Globals.GameProjectiles[ProjectileNum];
            MyName = MyBase.Name;
            OwnerID = ownerID;
            OwnerType = ownerType;
            Stat = Globals.Entities[OwnerID].Stat;
            Vital[(int)Enums.Vitals.Health] = 1;
            MaxVital[(int)Enums.Vitals.Health] = 1;
            CurrentMap = Map;
            CurrentX = X;
            CurrentY = Y;
            CurrentZ = Z;
            Dir = Direction;
            Target = target;
            IsSpell = isSpell;
            Passable = 1;
            HideName = 1;
        }

        private void AddProjectileSpawns()
        {
            ProjectileStruct myBase = Globals.GameProjectiles[ProjectileNum];

            for (int x = 0; x < ProjectileStruct.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileStruct.SpawnLocationsWidth; y++)
                {
                    for (int d = 0; d < ProjectileStruct.MaxProjectileDirections; d++)
                    {
                        if (myBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d), CurrentX + FindProjectileRotationX(Dir, x - 2, y - 2), CurrentY + FindProjectileRotationY(Dir, x - 2, y - 2), CurrentZ, CurrentMap);
                            Spawns.Add(s);
                        }
                    }
                }
            }
            Quantity++;
            SpawnTime = Environment.TickCount + MyBase.Delay;
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
            if (Quantity < MyBase.Quantity && Environment.TickCount > SpawnTime)
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
            if (Spawns.Count != 0 || Quantity < MyBase.Quantity)
            {
                for (int i = 0; i < Spawns.Count; i++)
                {
                    if (Environment.TickCount > Spawns[i].TransmittionTimer)
                    {
                        Spawns[i].Distance++;

                        int newx = Spawns[i].X + GetRangeX(Spawns[i].Dir, 1);
                        int newy = Spawns[i].Y + GetRangeY(Spawns[i].Dir, 1);
                        int newmap = Spawns[i].Map;

                        if (newx < 0)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Left > -1 && Globals.GameMaps[Globals.GameMaps[Spawns[i].Map].Left] != null)
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Left;
                                newx = Globals.MapWidth - 1;
                            }
                            else
                            {
                                Spawns.Remove(Spawns[i]);
                                return;
                            }
                        }
                        if (newx > Globals.MapWidth - 1)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Right > -1 && Globals.GameMaps[Globals.GameMaps[Spawns[i].Map].Right] != null)
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Right;
                                newx = 0;
                            }
                            else
                            {
                                Spawns.Remove(Spawns[i]);
                                return;
                            }
                        }
                        if (newy < 0)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Up > -1 && Globals.GameMaps[Globals.GameMaps[Spawns[i].Map].Up] != null)
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Up;
                                newy = Globals.MapHeight - 1;
                            }
                            else
                            {
                                Spawns.Remove(Spawns[i]);
                                return;
                            }
                        }
                        if (newy > Globals.MapHeight - 1)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Down > -1 && Globals.GameMaps[Globals.GameMaps[Spawns[i].Map].Down] != null)
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Down;
                                newy = 0;
                            }
                            else
                            {
                                Spawns.Remove(Spawns[i]);
                                return;
                            }
                        }

                        Spawns[i].X = newx;
                        Spawns[i].Y = newy;
                        Spawns[i].Map = newmap;

                        //Check for Z-Dimension
                        Attribute attribute = Globals.GameMaps[Spawns[i].Map].Attributes[Spawns[i].X, Spawns[i].Y];
                        if (attribute != null && attribute.value == (int)Enums.MapAttributes.ZDimension)
                        {
                            if (attribute.data1 > 0)
                            {
                                Spawns[i].Z = attribute.data1 - 1;
                            }
                        }

                        Entity TempEntity = new Entity(OwnerID);
                        TempEntity.CurrentX = Spawns[i].X;
                        TempEntity.CurrentY = Spawns[i].Y;
                        TempEntity.CurrentZ = Spawns[i].Z;
                        TempEntity.CurrentMap = Spawns[i].Map;
                        int c = TempEntity.CanMove(Dir);
                        Target = (int)TempEntity.CollisionIndex;

                        if (c == 0) //No collision so increase the counter for the next collision detection.
                        {
                            Spawns[i].TransmittionTimer = Environment.TickCount + (long)((float)Globals.GameProjectiles[ProjectileNum].Speed / (float)Globals.GameProjectiles[ProjectileNum].Range);
                            if (Spawns[i].Distance >= Globals.GameProjectiles[ProjectileNum].Range)
                            {
                                Spawns.Remove(Spawns[i]);
                            }
                        }
                        else
                        {
                            if (c == 2) //Player
                            {
                                if (OwnerID != Target)
                                {
                                    TryAttack(Target, true, IsSpell);
                                }
                            }
                            else //Any other target
                            {
                                if (OwnerType == typeof(Player))
                                {
                                    TryAttack(Target, true, IsSpell);
                                }
                            }
                            Spawns.Remove(Spawns[i]); //Remove from the list being processed
                        }
                    }
                }
            }
            else
            {
                Globals.GameMaps[CurrentMap].RemoveProjectile(this);
                PacketSender.SendEntityLeave(MyIndex, (int)Enums.EntityTypes.Projectile, CurrentMap);
                Globals.Entities[MyIndex] = null;
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(ProjectileNum);
            bf.WriteInteger(Dir);
            bf.WriteInteger(Target);
            return bf.ToArray();
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
        public long TransmittionTimer = Environment.TickCount;

        public ProjectileSpawns(int dir, int x, int y, int z, int map)
        {
            Map = map;
            X = x;
            Y = y;
            Z = z;
            Dir = dir;
        }
    }
}

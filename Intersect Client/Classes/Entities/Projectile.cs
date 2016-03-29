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
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Game_Objects;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Graphics = Intersect_Client.Classes.Core.GameGraphics;

namespace Intersect_Client.Classes.Entities
{
    public class Projectile : Entity
    {
        public int ProjectileNum = 0;
        public int Target = 0;
        private int Quantity = 0;
        private long SpawnTime = 0;
        private int _spawnCount = 0;
        private int _totalSpawns = 0;
        private int _spawnedAmount = 0;

        // Individual Spawns
        public ProjectileSpawns[] Spawns;

        /// <summary>
        /// The constructor for the inherated projectile class
        /// </summary>
        public Projectile() : base()
        {
            Vital[(int)Enums.Vitals.Health] = 1;
            MaxVital[(int)Enums.Vitals.Health] = 1;
            HideName = 1;
            Passable = 1;
            IsMoving = true;
        }

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            ProjectileNum = bf.ReadInteger();
            Dir = bf.ReadInteger();
            Target = bf.ReadInteger();
            for (int x = 0; x < ProjectileStruct.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileStruct.SpawnLocationsWidth; y++)
                {
                    for (int d = 0; d < ProjectileStruct.MaxProjectileDirections; d++)
                    {
                        if (Globals.GameProjectiles[ProjectileNum].SpawnLocations[x, y].Directions[d] == true)
                        {
                            _totalSpawns++;
                        }
                    }
                }
            }
            Spawns = new ProjectileSpawns[_totalSpawns];
        }

        public override void Dispose()
        {
            foreach (ProjectileSpawns s in Spawns)
            {
                if (s != null)
                {
                    s.Anim.Dispose();
                }
            }
        }

        private void AddProjectileSpawns()
        {
            ProjectileStruct myBase = Globals.GameProjectiles[ProjectileNum];
            AnimationStruct animBase = null;
            if (myBase.Animation > -1 && myBase.Animation < Options.MaxAnimations)
            {
                animBase = Globals.GameAnimations[myBase.Animation];
            }

            for (int x = 0; x < ProjectileStruct.SpawnLocationsWidth; x++)
            {
                for (int y = 0; y < ProjectileStruct.SpawnLocationsWidth; y++)
                {
                    for (int d = 0; d < ProjectileStruct.MaxProjectileDirections; d++)
                    {
                        if (myBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d), CurrentX + FindProjectileRotationX(Dir, x - 2, y - 2), CurrentY + FindProjectileRotationY(Dir, x - 2, y - 2), CurrentZ, CurrentMap, animBase, myBase.AutoRotate,myBase);
                            Spawns[_spawnedAmount] = s;
                            _spawnedAmount++;
                            _spawnCount++;
                        }
                    }
                }
            }
            Quantity++;
            SpawnTime = Globals.System.GetTimeMS() + Globals.GameProjectiles[ProjectileNum].Delay;
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
        /// Gets the displacement of the projectile during projection
        /// </summary>
        /// <returns>The displacement from the co-ordinates if placed on a Options.TileHeight grid.</returns>
        private float getDisplacement(long spawnTime)
        {
            long elapsedTime = Globals.System.GetTimeMS() - spawnTime;
            float displacementPercent = elapsedTime / (float)Globals.GameProjectiles[ProjectileNum].Speed;
            return displacementPercent * Options.TileHeight * Globals.GameProjectiles[ProjectileNum].Range;
        }

        /// <summary>
        ///  Overwrite updating the offsets for projectile movement.
        /// </summary>
        public override bool Update()
        {
            var tmpI = -1;
            var map = CurrentMap;
            var y = CurrentY;

            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (tmpI == -1) return false;

            if (Quantity < Globals.GameProjectiles[ProjectileNum].Quantity && SpawnTime < Globals.System.GetTimeMS())
            {
                AddProjectileSpawns();
            }

            if (IsMoving)
            {
                for (int s = 0; s < _spawnedAmount; s++)
                {
                    if (Spawns[s] != null)
                    {
                        Spawns[s].OffsetX = GetRangeX(Spawns[s].Dir, getDisplacement(Spawns[s].SpawnTime));
                        Spawns[s].OffsetY = GetRangeY(Spawns[s].Dir, getDisplacement(Spawns[s].SpawnTime));
                        Spawns[s].Anim.SetPosition(
                            Globals.GameMaps[Spawns[s].SpawnMap].GetX() + Spawns[s].SpawnX*Options.TileWidth +
                            Spawns[s].OffsetX +
                            Options.TileWidth/2,
                            Globals.GameMaps[Spawns[s].SpawnMap].GetY() + Spawns[s].SpawnY*Options.TileHeight +
                            Spawns[s].OffsetY +
                            Options.TileHeight/2, Spawns[s].AutoRotate ? Spawns[s].Dir : 0);
                        Spawns[s].Anim.Update();
                    }
                }
            }
            CheckForCollision();

            return true;
        }

        public void CheckForCollision()
        {
            if (_spawnCount != 0 || Quantity < Globals.GameProjectiles[ProjectileNum].Quantity)
            {
                for (int i = 0; i < _spawnedAmount; i++)
                {
                    if (Spawns[i] != null && Globals.System.GetTimeMS() > Spawns[i].TransmittionTimer)
                    {                
                        int newx = Spawns[i].X + (int)GetRangeX(Spawns[i].Dir, 1);
                        int newy = Spawns[i].Y + (int)GetRangeY(Spawns[i].Dir, 1);
                        int newmap = Spawns[i].Map;
                        bool killSpawn = false;

                        Spawns[i].Distance++;

                        if (newx < 0)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Left > -1 && Globals.GameMaps.ContainsKey(Globals.GameMaps[Spawns[i].Map].Left))
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Left;
                                newx = Options.MapWidth - 1;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newx > Options.MapWidth - 1)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Right > -1 && Globals.GameMaps.ContainsKey(Globals.GameMaps[Spawns[i].Map].Right))
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Right;
                                newx = 0;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newy < 0)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Up > -1 && Globals.GameMaps.ContainsKey(Globals.GameMaps[Spawns[i].Map].Up))
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Up;
                                newy = Options.MapHeight - 1;
                            }
                            else
                            {
                                killSpawn = true;
                            }
                        }
                        if (newy > Options.MapHeight - 1)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Down > -1 && Globals.GameMaps.ContainsKey(Globals.GameMaps[Spawns[i].Map].Down))
                            {
                                newmap = Globals.GameMaps[Spawns[i].Map].Down;
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
                            _spawnCount--;
                            continue;
                        }

                        Spawns[i].X = newx;
                        Spawns[i].Y = newy;
                        Spawns[i].Map = newmap;

                        //Check for Z-Dimension
                        if (Globals.GameMaps[Spawns[i].Map].Attributes[Spawns[i].X, Spawns[i].Y].value == (int)Enums.MapAttributes.ZDimension)
                        {
                            if (Globals.GameMaps[Spawns[i].Map].Attributes[Spawns[i].X, Spawns[i].Y].data1 > 0)
                            {
                                Spawns[i].Z = Globals.GameMaps[Spawns[i].Map].Attributes[Spawns[i].X, Spawns[i].Y].data1 - 1;
                            }
                        }

                        int tileBlocked = ((Player) Globals.Entities[Globals.MyIndex]).IsTileBlocked(Spawns[i].X,
                            Spawns[i].Y, CurrentZ, Spawns[i].Map);
                        
                        if (tileBlocked != -1)
                        {
                            if (tileBlocked >= 0 && Globals.Entities.ContainsKey(tileBlocked))
                            {
                                if (Globals.Entities[tileBlocked].GetType() == typeof (Resource))
                                {
                                    if ((((Resource) Globals.Entities[tileBlocked]).IsDead && !Spawns[i].ProjectileBase.IgnoreExhaustedResources) ||
                                        (!((Resource) Globals.Entities[tileBlocked]).IsDead && !Spawns[i].ProjectileBase.IgnoreActiveResources))
                                    {
                                        killSpawn = true;
                                    }
                                }
                                else
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
                        Spawns[i].TransmittionTimer = Globals.System.GetTimeMS() + (long)((float)Globals.GameProjectiles[ProjectileNum].Speed / (float)Globals.GameProjectiles[ProjectileNum].Range);
                        if (Spawns[i].Distance >= Globals.GameProjectiles[ProjectileNum].Range)
                        {
                            killSpawn = true;
                        }
                        if (killSpawn)
                        {
                            Spawns[i].Dispose();
                            Spawns[i] = null;
                            _spawnCount--;
                        }
                    }
                }
            }
            else
            {
                EntityManager.RemoveEntity(MyIndex, (int)Enums.EntityTypes.Projectile, CurrentMap);
            }
        }
        /// <summary>
        /// Rendering all of the individual projectiles from a singular spawn to a map.
        /// </summary>
        override public void Draw()
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
        }

        public void SpawnDead(int spawnIndex)
        {
            if (spawnIndex <= _spawnedAmount && Spawns[spawnIndex] != null)
            {
               Spawns[spawnIndex].Dispose();
               Spawns[spawnIndex] = null;
            }
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
        public AnimationInstance Anim;
        public ProjectileStruct ProjectileBase;
        public bool AutoRotate = false;

        //Clientside variables
        public float OffsetX = 0;
        public float OffsetY = 0;
        public int SpawnX = 0;
        public int SpawnY = 0;
        public int SpawnMap = 0;
        public long SpawnTime = Globals.System.GetTimeMS();
        public long TransmittionTimer = Globals.System.GetTimeMS();

        public ProjectileSpawns(int dir, int x, int y, int z, int map, AnimationStruct animBase, bool autoRotate, ProjectileStruct projectileBase)
        {
            X = x;
            Y = y;
            SpawnX = X;
            SpawnY = Y;
            Z = z;
            Map = map;
            SpawnMap = Map;
            Dir = dir;
            Anim = new AnimationInstance(animBase, true);
            AutoRotate = autoRotate;
            ProjectileBase = projectileBase;
        }

        public void Dispose()
        {
            Anim.Dispose();
        }
    }
}

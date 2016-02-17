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
using System.Drawing;
using Intersect_Client.Classes;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Graphics = Intersect_Client.Classes.Graphics;
using SFML.System;
using System.IO;

namespace Intersect_Client.Classes
{
    public class Projectile : Entity
    {
        public int ProjectileNum = 0;
        public int Target = 0;
        private long _movementTimer = 0;

        // Individual Spawns
        public List<ProjectileSpawns> Spawns = new List<ProjectileSpawns>();

        /// <summary>
        /// The constructor for the inherated projectile class
        /// </summary>
        public Projectile() : base()
        {
            Vital[(int)Enums.Vitals.Health] = 1;
            MaxVital[(int)Enums.Vitals.Health] = 1;
            HideName = 1;
            Passable = 1;
        }

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            ProjectileNum = bf.ReadInteger();
            Dir = bf.ReadInteger();
            Target = bf.ReadInteger();
            _movementTimer = Environment.TickCount;
            AddProjectileSpawns();
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
                            ProjectileSpawns s = new ProjectileSpawns(FindProjectileRotationDir(Dir, d), FindProjectileRotationX(Dir, x, y), FindProjectileRotationY(Dir, x, y));
                            Spawns.Add(s);
                        }
                    }
                }
            }
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
        /// <returns>The displacement from the co-ordinates if placed on a Globals.TileHeight grid.</returns>
        private float getDisplacement()
        {
            return (float)(((Environment.TickCount - _movementTimer) / Globals.GameProjectiles[ProjectileNum].Speed) * (float)(Globals.TileHeight * Globals.GameProjectiles[ProjectileNum].Range));
        }

        /// <summary>
        ///  Overwrite updating the offsets for projectile movement.
        /// </summary>
        public override bool Update()
        {
            var tmpI = -1;

            DetermineRenderOrder();

            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (tmpI == -1) return false;

            if (IsMoving)
            {
                for (int s = 0; s < Spawns.Count; s++)
                {
                    Spawns[s].OffsetX = GetRangeX(Spawns[s].Dir, getDisplacement());
                    Spawns[s].OffsetY = GetRangeY(Spawns[s].Dir, getDisplacement());
                }
            }
            return true;
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

            for (int s = 0; s < Spawns.Count; s++)
            {
                Texture srcTexture;
                RectangleF srcRectangle = new Rectangle();
                RectangleF destRectangle = new Rectangle();
                
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                //COPIED CODE FROM ENTITY CLASS DRAW METHOD, REMOVE AND REPLACE WITH ANIMATIONS WHEN THEY ARE A THING. //
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                
                var d = 0;
                srcTexture = Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())];
                if (srcTexture.Size.Y / 4 > Globals.TileHeight)
                {
                    destRectangle.X = (Globals.GameMaps[CurrentMap].GetX() + Spawns[s].X * Globals.TileWidth + Spawns[s].OffsetX);
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + Spawns[s].Y * Globals.TileHeight + Spawns[s].OffsetY - ((srcTexture.Size.Y / 4) - Globals.TileHeight);
                }
                else
                {
                    destRectangle.X = Globals.GameMaps[CurrentMap].GetX() + Spawns[s].X * Globals.TileWidth + Spawns[s].OffsetX;
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + Spawns[s].Y * Globals.TileHeight + Spawns[s].OffsetY;
                }
                if (srcTexture.Size.X / 4 > Globals.TileWidth)
                {
                    destRectangle.X -= ((srcTexture.Size.X / 4) - Globals.TileWidth) / 2;
                }

                switch (Dir)
                {
                    case 0:
                        d = 3;
                        break;
                    case 1:
                        d = 0;
                        break;
                    case 2:
                        d = 1;
                        break;
                    case 3:
                        d = 2;
                        break;
                }
                destRectangle.X = (int)Math.Ceiling(destRectangle.X);
                destRectangle.Y = destRectangle.Y;
                srcRectangle = new Rectangle(0 * (int)srcTexture.Size.X / 4, d * (int)srcTexture.Size.Y / 4, (int)srcTexture.Size.X / 4, (int)srcTexture.Size.Y / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.RenderTexture(srcTexture, srcRectangle, destRectangle, Graphics.RenderWindow);

                //////////////
                // CODE END //
                //////////////
            }
        }
    }

    public class ProjectileSpawns
    {
        public int X;
        public int Y;
        public int Dir;

        //Clientside variables
        public float OffsetX = 0;
        public float OffsetY = 0;

        public ProjectileSpawns(int dir, int x, int y)
        {
            X = x;
            Y = y;
            Dir = dir;
        }
    }
}

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
using System.IO;

namespace Intersect_Server.Classes
{
    public class ProjectileStruct
    {
        public const int SpawnLocationsWidth = 5;
        public const int SpawnLocationsHeight = 5;
        public const int MaxProjectileDirections = 8;

        public string Name = "";
        public int Animation = 0;
        public int Speed = 1;
        public int Delay = 1;
        public int Quantity = 1;
        public int Range = 1;
        public int Spell = 0;
        public bool Homing = false;
        public bool AutoRotate = false;
        public Location[,] SpawnLocations = new Location[SpawnLocationsWidth, SpawnLocationsHeight];

        //Init
        public ProjectileStruct()
        {
            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    SpawnLocations[x, y] = new Location();
                }
            }
        }

        public void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Animation = myBuffer.ReadInteger();
            Speed = myBuffer.ReadInteger();
            Delay = myBuffer.ReadInteger();
            Quantity = myBuffer.ReadInteger();
            Range = myBuffer.ReadInteger();
            Spell = myBuffer.ReadInteger();
            Homing = Convert.ToBoolean(myBuffer.ReadInteger());
            AutoRotate = Convert.ToBoolean(myBuffer.ReadInteger());

            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    for (var i = 0; i < MaxProjectileDirections; i++)
                    {
                        SpawnLocations[x, y].Directions[i] = Convert.ToBoolean(myBuffer.ReadInteger());
                    }
                }
            }

            myBuffer.Dispose();
        }

        public byte[] ProjectileData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(Speed);
            myBuffer.WriteInteger(Delay);
            myBuffer.WriteInteger(Quantity);
            myBuffer.WriteInteger(Range);
            myBuffer.WriteInteger(Spell);
            myBuffer.WriteInteger(Convert.ToInt32(Homing));
            myBuffer.WriteInteger(Convert.ToInt32(AutoRotate));

            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    for (var i = 0; i < MaxProjectileDirections; i++)
                    {
                        myBuffer.WriteInteger(Convert.ToInt32(SpawnLocations[x, y].Directions[i]));
                    }
                }
            }

            return myBuffer.ToArray();
        }

        public void Save(int projectileNum)
        {
            byte[] data = ProjectileData();
            Stream stream = new FileStream("Resources/Projectiles/" + projectileNum + ".prj", FileMode.OpenOrCreate);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

    }

    public class Location
    {
        public bool[] Directions = new bool[ProjectileStruct.MaxProjectileDirections];
    }
}

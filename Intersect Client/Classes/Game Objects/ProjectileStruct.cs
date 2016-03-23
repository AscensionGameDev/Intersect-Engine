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
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
{
    public class ProjectileStruct
    {
        public const int SpawnLocationsWidth = 5;
        public const int SpawnLocationsHeight = 5;
        public const int MaxProjectileDirections = 8;

        public const string Version = "0.0.0.1";
        public string Name = "";
        public int Animation = 0;
        public int Speed = 1;
        public int Delay = 1;
        public int Quantity = 1;
        public int Range = 1;
        public int Spell = 0;
        public bool IgnoreMapBlocks = false;
        public bool IgnoreZDimension = false;
        public bool IgnoreActiveResources = false;
        public bool IgnoreExhaustedResources = false;
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

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Projectile #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
            Name = myBuffer.ReadString();
            Animation = myBuffer.ReadInteger();
            Speed = myBuffer.ReadInteger();
            Delay = myBuffer.ReadInteger();
            Quantity = myBuffer.ReadInteger();
            Range = myBuffer.ReadInteger();
            Spell = myBuffer.ReadInteger();
            IgnoreMapBlocks = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreActiveResources = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreExhaustedResources = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreZDimension = Convert.ToBoolean(myBuffer.ReadInteger());
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

    }

    public class Location
    {
        public bool[] Directions = new bool[ProjectileStruct.MaxProjectileDirections];
    }
}

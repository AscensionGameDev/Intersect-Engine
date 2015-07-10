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
    public class Resource : Entity
    {
        // Resource Number
        public int ResourceNum;

        //Respawn
        public long RespawnTime;

        //Tileset Locations
        public int X;
        public int Y;
        public int Width;
        public int Height;
        
        public Resource(int index, int ResourceNum) : base(index)
        {
            ResourceStruct myBase = Globals.GameResources[ResourceNum];
            MyName = myBase.Name;
            MySprite = myBase.InitialGraphic.Sprite;
            X = myBase.InitialGraphic.X;
            Y = myBase.InitialGraphic.Y;
            Width = myBase.InitialGraphic.Width;
            Height = myBase.InitialGraphic.Height;
            Vital[(int)Enums.Vitals.Health] = Globals.Rand.Next(myBase.MinHP, myBase.MaxHP + 1);
            MaxVital[(int)Enums.Vitals.Health] = Vital[(int)Enums.Vitals.Health];
            Passable = Convert.ToInt32(myBase.WalkableBefore);   
        }

        public override void Die()
        {
            ResourceStruct myBase = Globals.GameResources[ResourceNum];
            RespawnTime = Environment.TickCount + Globals.GameResources[ResourceNum].SpawnDuration * 1000;
            MySprite = myBase.EndGraphic.Sprite;
            X = myBase.EndGraphic.X;
            Y = myBase.EndGraphic.Y;
            Width = myBase.EndGraphic.Width;
            Height = myBase.EndGraphic.Height;
            Passable = Convert.ToInt32(myBase.WalkableAfter);  
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(X);
            bf.WriteInteger(Y);
            bf.WriteInteger(Width);
            bf.WriteInteger(Height);
            return bf.ToArray();
        }
    }
}

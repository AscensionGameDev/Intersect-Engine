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

namespace Intersect_Client.Classes
{
    public class ResourceStruct
    {
        // General
        public string Name = "";
        public int MinHP = 0;
        public int MaxHP = 0;
        public int Tool = 0;
        public int SpawnDuration = 0;
        public int Animation = 0;
        public bool WalkableBefore = false;
        public bool WalkableAfter = false;

        // Graphics
        public ResourceGraphic InitialGraphic = new ResourceGraphic();
        public ResourceGraphic EndGraphic = new ResourceGraphic();

        // Drops
        public List<ResourceDrop> Drops = new List<ResourceDrop>();

        public ResourceStruct()
        {
            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Drops.Add(new ResourceDrop());
            }

        }

        public void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            InitialGraphic.Sprite = myBuffer.ReadString();
            InitialGraphic.X = myBuffer.ReadInteger();
            InitialGraphic.Y = myBuffer.ReadInteger();
            InitialGraphic.Width = myBuffer.ReadInteger();
            InitialGraphic.Height = myBuffer.ReadInteger();
            EndGraphic.Sprite = myBuffer.ReadString();
            EndGraphic.X = myBuffer.ReadInteger();
            EndGraphic.Y = myBuffer.ReadInteger();
            EndGraphic.Width = myBuffer.ReadInteger();
            EndGraphic.Height = myBuffer.ReadInteger();
            MinHP = myBuffer.ReadInteger();
            MaxHP = myBuffer.ReadInteger();
            Tool = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            WalkableBefore = Convert.ToBoolean(myBuffer.ReadInteger());
            WalkableAfter = Convert.ToBoolean(myBuffer.ReadInteger());

            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Drops[i].ItemNum = myBuffer.ReadInteger();
                Drops[i].Amount = myBuffer.ReadInteger();
                Drops[i].Chance = myBuffer.ReadInteger();
            }

            myBuffer.Dispose();
        }

        public byte[] ResourceData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(InitialGraphic.Sprite);
            myBuffer.WriteInteger(InitialGraphic.X);
            myBuffer.WriteInteger(InitialGraphic.Y);
            myBuffer.WriteInteger(InitialGraphic.Width);
            myBuffer.WriteInteger(InitialGraphic.Height);
            myBuffer.WriteString(EndGraphic.Sprite);
            myBuffer.WriteInteger(EndGraphic.X);
            myBuffer.WriteInteger(EndGraphic.Y);
            myBuffer.WriteInteger(EndGraphic.Width);
            myBuffer.WriteInteger(EndGraphic.Height);
            myBuffer.WriteInteger(MinHP);
            myBuffer.WriteInteger(MaxHP);
            myBuffer.WriteInteger(Tool);
            myBuffer.WriteInteger(SpawnDuration);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(Convert.ToInt32(WalkableBefore));
            myBuffer.WriteInteger(Convert.ToInt32(WalkableAfter));

            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Drops[i].ItemNum);
                myBuffer.WriteInteger(Drops[i].Amount);
                myBuffer.WriteInteger(Drops[i].Chance);
            }

            return myBuffer.ToArray();
        }

        public class ResourceDrop
        {
            public int ItemNum;
            public int Amount;
            public int Chance;
        }

        public class ResourceGraphic
        {
            public string Sprite = "";
            public int X = 0;
            public int Y = 0;
            public int Width = 0;
            public int Height = 0;
        }
    }
}

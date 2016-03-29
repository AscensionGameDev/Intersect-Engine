/*
    Intersect Game Engine (Editor)
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
using Intersect_Editor.Classes.General;

namespace Intersect_Editor.Classes
{
    public class NpcStruct
    {
        //Core info
        public const string Version = "0.0.0.1";
        public string Name = "";
        public string Sprite = "";

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];
        public int Experience = 0;

        //Basic Info
        public int SpawnDuration = 0;
        public byte Behavior = 0;
        public int SightRange = 0;

        //Drops
        public List<NPCDrop> Drops = new List<NPCDrop>();


        public NpcStruct()
        {
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new NPCDrop());
            }

        }

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Npc #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
            Name = myBuffer.ReadString();
            Sprite = myBuffer.ReadString();
            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                Stat[i] = myBuffer.ReadInteger();
            }
            Experience = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Behavior = myBuffer.ReadByte();
            SightRange = myBuffer.ReadInteger();
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops[i].ItemNum = myBuffer.ReadInteger();
                Drops[i].Amount = myBuffer.ReadInteger();
                Drops[i].Chance = myBuffer.ReadInteger();
            }

            myBuffer.Dispose();
        }

        public byte[] NpcData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Version);
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Sprite);
            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(MaxVital[i]);
            }
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(Stat[i]);
            }
            myBuffer.WriteInteger(Experience);
            myBuffer.WriteInteger(SpawnDuration);
            myBuffer.WriteByte(Behavior);
            myBuffer.WriteInteger(SightRange);
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Drops[i].ItemNum);
                myBuffer.WriteInteger(Drops[i].Amount);
                myBuffer.WriteInteger(Drops[i].Chance);
            }

            return myBuffer.ToArray();
        }

        public void Save(int npcNum)
        {
            //Implement sending to server
        }

    }

    public class NPCDrop
    {
        public int ItemNum;
        public int Amount;
        public int Chance;
    }
}

/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
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
    }

    public class NPCDrop
    {
        public int ItemNum;
        public int Amount;
        public int Chance;

    }
}

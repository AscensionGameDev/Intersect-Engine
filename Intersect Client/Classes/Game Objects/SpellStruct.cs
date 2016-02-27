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
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Game_Objects
{
    public class SpellStruct
    {
        //Core Info
        public const string Version = "0.0.0.1";
        public string Name = "";
        public string Desc = "";
        public byte Type = 0;
        public int Cost = 0;
        public string Pic = "";

        //Spell Times
        public int CastDuration = 0;
        public int CooldownDuration = 0;

        //Animations
        public int CastAnimation = -1;
        public int HitAnimation = -1;

        //Targetting Stuff
        public int TargetType = 0;
        public int CastRange = 0;
        public int HitRadius = 0;

        //Costs
        public int[] VitalCost = new int[(int)Enums.Vitals.VitalCount];

        //Requirements
        public int LevelReq = 0;
        public int[] StatReq = new int[(int)Enums.Stats.StatCount];

        //Heal/Damage
        public int[] VitalDiff = new int[(int)Enums.Vitals.VitalCount];

        //Buff/Debuff Data
        public int[] StatDiff = new int[(int)Enums.Stats.StatCount];

        //Extra Data, Teleport Coords, Custom Spells, Etc
        public int Data1 = 0;
        public int Data2 = 0;
        public int Data3 = 0;
        public int Data4 = 0;

        public SpellStruct()
        {

        }

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Spell #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
            Name = myBuffer.ReadString();
            Desc = myBuffer.ReadString();
            Type = myBuffer.ReadByte();
            Cost = myBuffer.ReadInteger();
            Pic = myBuffer.ReadString();

            CastDuration = myBuffer.ReadInteger();
            CooldownDuration = myBuffer.ReadInteger();

            CastAnimation = myBuffer.ReadInteger();
            HitAnimation = myBuffer.ReadInteger();

            TargetType = myBuffer.ReadInteger();
            CastRange = myBuffer.ReadInteger();
            HitRadius = myBuffer.ReadInteger();

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                VitalCost[i] = myBuffer.ReadInteger();
            }

            LevelReq = myBuffer.ReadInteger();
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                StatReq[i] = myBuffer.ReadInteger();
            }

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                VitalDiff[i] = myBuffer.ReadInteger();
            }

            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                StatDiff[i] = myBuffer.ReadInteger();
            }

            Data1 = myBuffer.ReadInteger();
            Data2 = myBuffer.ReadInteger();
            Data3 = myBuffer.ReadInteger();
            Data4 = myBuffer.ReadInteger();

            myBuffer.Dispose();
        }
    }
}

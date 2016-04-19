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
using System.Linq;
using System.Text;

namespace Intersect_Editor.Classes
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
        public string Data5 = "";

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
            Data5 = myBuffer.ReadString();

            myBuffer.Dispose();
        }

        public byte[] SpellData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Version);
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Desc);
            myBuffer.WriteByte(Type);
            myBuffer.WriteInteger(Cost);
            myBuffer.WriteString(Pic);

            myBuffer.WriteInteger(CastDuration);
            myBuffer.WriteInteger(CooldownDuration);

            myBuffer.WriteInteger(CastAnimation);
            myBuffer.WriteInteger(HitAnimation);

            myBuffer.WriteInteger(TargetType);
            myBuffer.WriteInteger(CastRange);
            myBuffer.WriteInteger(HitRadius);

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalCost[i]);
            }

            myBuffer.WriteInteger(LevelReq);

            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatReq[i]);
            }

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalDiff[i]);
            }

            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatDiff[i]);
            }

            myBuffer.WriteInteger(Data1);
            myBuffer.WriteInteger(Data2);
            myBuffer.WriteInteger(Data3);
            myBuffer.WriteInteger(Data4);
            myBuffer.WriteString(Data5);
            return myBuffer.ToArray();
        }
    }
}

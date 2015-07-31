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
using System.Collections.Generic;
using System.IO;
namespace Intersect_Client.Classes
{
    public class ClassStruct
    {
        //Core info
        public string Name = "";

        //Sprites
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];
        public int Points = 0;

        //Starting Items
        public List<ClassItem> Items = new List<ClassItem>();

        //Starting Spells
        public List<ClassSpell> Spells = new List<ClassSpell>();

        public ClassStruct()
        {
            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Items.Add(new ClassItem());
            }
        }

        public void Load(byte[] packet)
        {
            var spriteCount = 0;
            ClassSprite TempSprite = new ClassSprite();
            var spellCount = 0;
            ClassSpell TempSpell = new ClassSpell();

            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();

            // Load Class Sprites
            Sprites.Clear();
            spriteCount = myBuffer.ReadInteger();
            for (var i = 0; i < spriteCount; i++)
            {
                TempSprite = new ClassSprite();
                TempSprite.Sprite = myBuffer.ReadString();
                TempSprite.Gender = myBuffer.ReadByte();
                Sprites.Add(TempSprite);
            }

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                Stat[i] = myBuffer.ReadInteger();
            }
            Points = myBuffer.ReadInteger();

            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Items[i].ItemNum = myBuffer.ReadInteger();
                Items[i].Amount = myBuffer.ReadInteger();
            }

            // Load Class Spells
            Spells.Clear();
            spellCount = myBuffer.ReadInteger();
            for (var i = 0; i < spellCount; i++)
            {
                TempSpell = new ClassSpell();
                TempSpell.SpellNum = myBuffer.ReadInteger();
                TempSpell.Level = myBuffer.ReadInteger();
                Spells.Add(TempSpell);
            }

            myBuffer.Dispose();
        }

        public byte[] ClassData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);

            //Sprites
            myBuffer.WriteInteger(Sprites.Count);
            for (var i = 0; i < Sprites.Count; i++)
            {
                myBuffer.WriteString(Sprites[i].Sprite);
                myBuffer.WriteByte(Sprites[i].Gender);
            }

            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(MaxVital[i]);
            }
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(Stat[i]);
            }
            myBuffer.WriteInteger(Points);

            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Items[i].ItemNum);
                myBuffer.WriteInteger(Items[i].Amount);
            }

            //Spells
            myBuffer.WriteInteger(Spells.Count);
            for (var i = 0; i < Spells.Count; i++)
            {
                myBuffer.WriteInteger(Spells[i].SpellNum);
                myBuffer.WriteInteger(Spells[i].Level);
            }

            return myBuffer.ToArray();
        }
    }

    public class ClassItem
    {
        public int ItemNum;
        public int Amount;
    }

    public class ClassSpell
    {
        public int SpellNum;
        public int Level;
    }

    public class ClassSprite
    {
        public string Sprite = "";
        public byte Gender;
    }
}

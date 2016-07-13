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
using System.Linq;

namespace Intersect_Library.GameObjects
{
    public class ClassBase : DatabaseObject
    {
        //Core info
        public new const string DatabaseTable = "classes";
        public new const GameObject Type = GameObject.Class;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        
        public string Name = "New Class";

        //Spawn Info
        public int SpawnMap = 0;
        public int SpawnX = 0;
        public int SpawnY = 0;
        public int SpawnDir = 0;

        //Sprites
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Vitals.VitalCount];
        public int[] Stat = new int[(int)Stats.StatCount];
        public int Points = 0;

        //Starting Items
        public List<ClassItem> Items = new List<ClassItem>();

        //Starting Spells
        public List<ClassSpell> Spells = new List<ClassSpell>();

        public ClassBase(int id) : base(id)
        {
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Items.Add(new ClassItem());
            }
        }

        public override void Load(byte[] packet)
        {
            var spriteCount = 0;
            ClassSprite TempSprite = new ClassSprite();
            var spellCount = 0;
            ClassSpell TempSpell = new ClassSpell();

            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();

            SpawnMap = myBuffer.ReadInteger();
            SpawnX = myBuffer.ReadInteger();
            SpawnY = myBuffer.ReadInteger();
            SpawnDir = myBuffer.ReadInteger();

            // Load Class Sprites
            Sprites.Clear();
            spriteCount = myBuffer.ReadInteger();
            for (var i = 0; i < spriteCount; i++)
            {
                TempSprite = new ClassSprite();
                TempSprite.Sprite = myBuffer.ReadString();
                TempSprite.Face = myBuffer.ReadString();
                TempSprite.Gender = myBuffer.ReadByte();
                Sprites.Add(TempSprite);
            }

            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                Stat[i] = myBuffer.ReadInteger();
            }
            Points = myBuffer.ReadInteger();

            for (int i = 0; i < Options.MaxNpcDrops; i++)
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

            myBuffer.WriteInteger(SpawnMap);
            myBuffer.WriteInteger(SpawnX);
            myBuffer.WriteInteger(SpawnY);
            myBuffer.WriteInteger(SpawnDir);

            //Sprites
            myBuffer.WriteInteger(Sprites.Count);
            for (var i = 0; i < Sprites.Count; i++)
            {
                myBuffer.WriteString(Sprites[i].Sprite);
                myBuffer.WriteString(Sprites[i].Face);
                myBuffer.WriteByte(Sprites[i].Gender);
            }

            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(MaxVital[i]);
            }
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(Stat[i]);
            }
            myBuffer.WriteInteger(Points);

            for (int i = 0; i < Options.MaxNpcDrops; i++)
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

        public static ClassBase GetClass(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (ClassBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ClassBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return ClassData();
        }

        public override string GetTable()
        {
            return DatabaseTable;
        }

        public override GameObject GetGameObjectType()
        {
            return Type;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }
        public override void Delete()
        {
            Objects.Remove(GetId());
        }
        public static void ClearObjects()
        {
            Objects.Clear();
        }
        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, ClassBase> GetObjects()
        {
            Dictionary<int, ClassBase> objects = Objects.ToDictionary(k => k.Key, v => (ClassBase)v.Value);
            return objects;
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
        public string Face = "";
        public byte Gender;
    }
}

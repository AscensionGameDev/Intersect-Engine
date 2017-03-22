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

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects
{
    public class BenchBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "crafts";
        public new const GameObject OBJECT_TYPE = GameObject.Bench;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Bench";
        public List<Bench> Crafts = new List<Bench>();

        public BenchBase(int id) : base(id)
        {

        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);

            Name = myBuffer.ReadString();
            int count = myBuffer.ReadInteger();

            Crafts.Clear();

            for (int i = 0; i < count; i++)
            {
                Crafts.Add(new Bench());
                Crafts[i].Item = myBuffer.ReadInteger();
                Crafts[i].Time = myBuffer.ReadInteger();

                // Load Ingredients
                Crafts[i].Ingredients.Clear();
                var ingredientCount = myBuffer.ReadInteger();
                for (var n = 0; n < ingredientCount; n++)
                {
                    Crafts[i].Ingredients.Add(new CraftIngredient(myBuffer.ReadInteger(), myBuffer.ReadInteger()));
                }
            }

            myBuffer.Dispose();
        }

        public byte[] CraftData()
        {
            var myBuffer = new ByteBuffer();

            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Crafts.Count);
            foreach (var craft in Crafts)
            {
                myBuffer.WriteInteger(craft.Item);
                myBuffer.WriteInteger(craft.Time);

                myBuffer.WriteInteger(craft.Ingredients.Count);
                for (var i = 0; i < craft.Ingredients.Count; i++)
                {
                    myBuffer.WriteInteger(craft.Ingredients[i].Item);
                    myBuffer.WriteInteger(craft.Ingredients[i].Quantity);
                }
            }

            return myBuffer.ToArray();
        }

        public static BenchBase GetCraft(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (BenchBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((BenchBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return CraftData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, BenchBase> GetObjects()
        {
            Dictionary<int, BenchBase> objects = Objects.ToDictionary(k => k.Key, v => (BenchBase)v.Value);
            return objects;
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
    }

    public class Bench
    {
        public int Time = 1;
        public int Item = -1;

        public List<CraftIngredient> Ingredients = new List<CraftIngredient>();
    }

    public class CraftIngredient
    {
        public int Item = -1;
        public int Quantity = 1;

        public CraftIngredient(int item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}

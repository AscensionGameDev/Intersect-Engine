using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class ResourceBase : DatabaseObject
    {
        // General
        public new const string DATABASE_TABLE = "resources";

        public new const GameObject OBJECT_TYPE = GameObject.Resource;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();
        public int Animation;

        // Drops
        public List<ResourceDrop> Drops = new List<ResourceDrop>();

        public string EndGraphic = "None";

        // Graphics
        public string InitialGraphic = "None";

        public int MaxHp;
        public int MinHp;

        public string Name = "New Resource";
        public int SpawnDuration;
        public int Tool = -1;
        public bool WalkableAfter;
        public bool WalkableBefore;

        public ResourceBase(int id) : base(id)
        {
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new ResourceDrop());
            }
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            InitialGraphic = myBuffer.ReadString();
            EndGraphic = myBuffer.ReadString();
            MinHp = myBuffer.ReadInteger();
            MaxHp = myBuffer.ReadInteger();
            Tool = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            WalkableBefore = Convert.ToBoolean(myBuffer.ReadInteger());
            WalkableAfter = Convert.ToBoolean(myBuffer.ReadInteger());

            for (int i = 0; i < Options.MaxNpcDrops; i++)
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
            myBuffer.WriteString(InitialGraphic);
            myBuffer.WriteString(EndGraphic);
            myBuffer.WriteInteger(MinHp);
            myBuffer.WriteInteger(MaxHp);
            myBuffer.WriteInteger(Tool);
            myBuffer.WriteInteger(SpawnDuration);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(Convert.ToInt32(WalkableBefore));
            myBuffer.WriteInteger(Convert.ToInt32(WalkableAfter));

            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                myBuffer.WriteInteger(Drops[i].ItemNum);
                myBuffer.WriteInteger(Drops[i].Amount);
                myBuffer.WriteInteger(Drops[i].Chance);
            }

            return myBuffer.ToArray();
        }

        public static ResourceBase GetResource(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (ResourceBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((ResourceBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return ResourceData();
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
            if (sObjects.ContainsKey(index))
            {
                return sObjects[index];
            }
            return null;
        }

        public override void Delete()
        {
            sObjects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            sObjects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            sObjects.Remove(index);
            sObjects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, ResourceBase> GetObjects()
        {
            Dictionary<int, ResourceBase> objects = sObjects.ToDictionary(k => k.Key, v => (ResourceBase) v.Value);
            return objects;
        }

        public class ResourceDrop
        {
            public int Amount;
            public int Chance;
            public int ItemNum;
        }
    }
}
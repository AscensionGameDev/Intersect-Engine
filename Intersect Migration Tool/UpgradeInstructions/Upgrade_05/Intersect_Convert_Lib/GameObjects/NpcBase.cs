using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects
{
    public class NpcBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "npcs";

        public new const GameObject OBJECT_TYPE = GameObject.Npc;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();
        public List<int> AggroList = new List<int>();
        public bool AttackAllies;
        public byte Behavior;

        //Drops
        public List<NpcDrop> Drops = new List<NpcDrop>();

        public int Experience;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        public string Name = "New Npc";

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled;

        public int SightRange;

        //Basic Info
        public int SpawnDuration;

        public int SpellFrequency = 2;

        //Spells
        public List<int> Spells = new List<int>();

        public string Sprite = "";
        public int[] Stat = new int[(int) Stats.StatCount];

        public NpcBase(int id) : base(id)
        {
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new NpcDrop());
            }
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Sprite = myBuffer.ReadString();
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
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
            Spells.Clear();
            var spellCount = myBuffer.ReadInteger();
            for (int i = 0; i < spellCount; i++)
            {
                Spells.Add(myBuffer.ReadInteger());
            }
            SpellFrequency = myBuffer.ReadInteger();

            AggroList.Clear();
            var aggroCount = myBuffer.ReadInteger();
            for (int i = 0; i < aggroCount; i++)
            {
                AggroList.Add(myBuffer.ReadInteger());
            }
            NpcVsNpcEnabled = Convert.ToBoolean(myBuffer.ReadInteger());
            AttackAllies = Convert.ToBoolean(myBuffer.ReadInteger());

            myBuffer.Dispose();
        }

        public byte[] NpcData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Sprite);
            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(MaxVital[i]);
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
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
            myBuffer.WriteInteger(Spells.Count);
            for (int i = 0; i < Spells.Count; i++)
            {
                myBuffer.WriteInteger(Spells[i]);
            }
            myBuffer.WriteInteger(SpellFrequency);

            myBuffer.WriteInteger(AggroList.Count);
            for (int i = 0; i < AggroList.Count; i++)
            {
                myBuffer.WriteInteger(AggroList[i]);
            }
            myBuffer.WriteInteger(Convert.ToInt32(NpcVsNpcEnabled));
            myBuffer.WriteInteger(Convert.ToInt32(AttackAllies));

            return myBuffer.ToArray();
        }

        public static NpcBase GetNpc(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (NpcBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((NpcBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return NpcData();
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

        public static Dictionary<int, NpcBase> GetObjects()
        {
            Dictionary<int, NpcBase> objects = sObjects.ToDictionary(k => k.Key, v => (NpcBase) v.Value);
            return objects;
        }
    }

    public class NpcDrop
    {
        public int Amount;
        public int Chance;
        public int ItemNum;
    }
}
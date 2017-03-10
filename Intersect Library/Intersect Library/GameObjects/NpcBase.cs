using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Intersect_Library.GameObjects
{
    public class NpcBase : DatabaseObject
    {
        //Core info
        public new const string DatabaseTable = "npcs";
        public new const GameObject Type = GameObject.Npc;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Npc";
        public string Sprite = "";

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Vitals.VitalCount];
        public int[] Stat = new int[(int)Stats.StatCount];
        public int Experience = 0;

        //Basic Info
        public int SpawnDuration = 0;
        public byte Behavior = 0;
        public int SightRange = 0;

        //Combat
        public int Damage;
        public int CritChance;
        public int DamageType;
        public int ScalingStat;
        public int Scaling;
        public int AttackAnimation = -1;

        //Spells
        public List<int> Spells = new List<int>();
        public int SpellFrequency = 2;

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled = false;
        public bool AttackAllies = false;
        public List<int> AggroList = new List<int>();
        
        //Drops
        public List<NPCDrop> Drops = new List<NPCDrop>();


		public NpcBase(int id) : base(id)
		{
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new NPCDrop());
            }
			
		}

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Sprite = myBuffer.ReadString();
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                Stat[i] = myBuffer.ReadInteger();
            }
            Experience = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Behavior = myBuffer.ReadByte();
            SightRange = myBuffer.ReadInteger();

            //Combat
            Damage = myBuffer.ReadInteger();
            DamageType = myBuffer.ReadInteger();
            CritChance = myBuffer.ReadInteger();
            ScalingStat = myBuffer.ReadInteger();
            Scaling = myBuffer.ReadInteger();
            AttackAnimation = myBuffer.ReadInteger();

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
            for (int i = 0; i < (int)Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(MaxVital[i]);
            }
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(Stat[i]);
            }
            myBuffer.WriteInteger(Experience);
            myBuffer.WriteInteger(SpawnDuration);
            myBuffer.WriteByte(Behavior);
            myBuffer.WriteInteger(SightRange);

            //Combat
            myBuffer.WriteInteger(Damage);
            myBuffer.WriteInteger(DamageType);
            myBuffer.WriteInteger(CritChance);
            myBuffer.WriteInteger(ScalingStat);
            myBuffer.WriteInteger(Scaling);
            myBuffer.WriteInteger(AttackAnimation);

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
            if (Objects.ContainsKey(index))
            {
                return (NpcBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((NpcBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return NpcData();
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
        public static Dictionary<int, NpcBase> GetObjects()
        {
            Dictionary<int, NpcBase> objects = Objects.ToDictionary(k => k.Key, v => (NpcBase)v.Value);
            return objects;
        }

    }

    public class NPCDrop
    {
        public int ItemNum;
        public int Amount;
        public int Chance;

    }
}

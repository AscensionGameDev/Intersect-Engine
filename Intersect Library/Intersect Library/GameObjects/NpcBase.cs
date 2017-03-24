using System;
using System.Collections.Generic;

namespace Intersect.GameObjects
{
    public class NpcBase : DatabaseObject<NpcBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "npcs";
        public new const GameObject OBJECT_TYPE = GameObject.Npc;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        public List<int> AggroList = new List<int>();
        public bool AttackAllies = false;
        public int AttackAnimation = -1;
        public byte Behavior = 0;
        public int CritChance;

        //Combat
        public int Damage;
        public int DamageType;

        //Drops
        public List<NPCDrop> Drops = new List<NPCDrop>();
        public int Experience = 0;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled = false;
        public int Scaling;
        public int ScalingStat;
        public int SightRange = 0;

        //Basic Info
        public int SpawnDuration = 0;
        public int SpellFrequency = 2;

        //Spells
        public List<int> Spells = new List<int>();

        public string Sprite = "";
        public int[] Stat = new int[(int) Stats.StatCount];

        public NpcBase(int id) : base(id)
        {
            Name = "New Npc";
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

        public override byte[] BinaryData => NpcData();
        public override string DatabaseTableName => DATABASE_TABLE;
        public override GameObject GameObjectType => OBJECT_TYPE;
    }

    public class NPCDrop
    {
        public int Amount;
        public int Chance;
        public int ItemNum;
    }
}
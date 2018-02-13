using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class SpellBase : DatabaseObject
    {
        //Core Info
        public new const string DATABASE_TABLE = "spells";

        public new const GameObject OBJECT_TYPE = GameObject.Spell;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();

        //Animations
        public int CastAnimation = -1;

        //Spell Times
        public int CastDuration;

        public int CastRange;
        public int CooldownDuration;
        public int Cost;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;
        public string Data5 = "";
        public string Desc = "";
        public int HitAnimation = -1;
        public int HitRadius;

        //Requirements
        public int LevelReq;

        public string Name = "New Spell";
        public string Pic = "";

        //Extra Data, Teleport Coords, Custom Spells, Etc
        public int Projectile;

        public byte SpellType;

        //Buff/Debuff Data
        public int[] StatDiff = new int[(int) Stats.StatCount];

        public int[] StatReq = new int[(int) Stats.StatCount];

        //Targetting Stuff
        public int TargetType;

        //Costs
        public int[] VitalCost = new int[(int) Vitals.VitalCount];

        //Heal/Damage
        public int[] VitalDiff = new int[(int) Vitals.VitalCount];

        public SpellBase(int id) : base(id)
        {
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Desc = myBuffer.ReadString();
            SpellType = myBuffer.ReadByte();
            Cost = myBuffer.ReadInteger();
            Pic = myBuffer.ReadString();

            CastDuration = myBuffer.ReadInteger();
            CooldownDuration = myBuffer.ReadInteger();

            CastAnimation = myBuffer.ReadInteger();
            HitAnimation = myBuffer.ReadInteger();

            TargetType = myBuffer.ReadInteger();
            CastRange = myBuffer.ReadInteger();
            HitRadius = myBuffer.ReadInteger();

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                VitalCost[i] = myBuffer.ReadInteger();
            }

            LevelReq = myBuffer.ReadInteger();
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                StatReq[i] = myBuffer.ReadInteger();
            }

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                VitalDiff[i] = myBuffer.ReadInteger();
            }

            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                StatDiff[i] = myBuffer.ReadInteger();
            }

            Projectile = myBuffer.ReadInteger();
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
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Desc);
            myBuffer.WriteByte(SpellType);
            myBuffer.WriteInteger(Cost);
            myBuffer.WriteString(Pic);

            myBuffer.WriteInteger(CastDuration);
            myBuffer.WriteInteger(CooldownDuration);

            myBuffer.WriteInteger(CastAnimation);
            myBuffer.WriteInteger(HitAnimation);

            myBuffer.WriteInteger(TargetType);
            myBuffer.WriteInteger(CastRange);
            myBuffer.WriteInteger(HitRadius);

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalCost[i]);
            }

            myBuffer.WriteInteger(LevelReq);

            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatReq[i]);
            }

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalDiff[i]);
            }

            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatDiff[i]);
            }

            myBuffer.WriteInteger(Projectile);
            myBuffer.WriteInteger(Data1);
            myBuffer.WriteInteger(Data2);
            myBuffer.WriteInteger(Data3);
            myBuffer.WriteInteger(Data4);
            myBuffer.WriteString(Data5);
            return myBuffer.ToArray();
        }

        public static SpellBase GetSpell(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (SpellBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((SpellBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return SpellData();
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

        public static Dictionary<int, SpellBase> GetObjects()
        {
            Dictionary<int, SpellBase> objects = sObjects.ToDictionary(k => k.Key, v => (SpellBase) v.Value);
            return objects;
        }
    }
}
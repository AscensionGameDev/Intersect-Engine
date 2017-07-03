using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;

namespace Intersect.GameObjects
{
    public class SpellBase : DatabaseObject<SpellBase>
    {
        //Animations
        public int CastAnimation = -1;

        //Spell Times
        public int CastDuration;

        //Requirements
        public ConditionLists CastingReqs = new ConditionLists();
        public int CastRange;
        public int CooldownDuration;
        public int Cost;

        //Damage
        public int CritChance;
        public int DamageType = 1;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;
        public string Data5 = "";

        public string Desc = "";
        public int Friendly;
        public int HitAnimation = -1;
        public int HitRadius;
        public string Pic = "";

        //Extra Data, Teleport Coords, Custom Spells, Etc
        public int Projectile;
        public int Scaling;
        public int ScalingStat;
        public byte SpellType;

        //Buff/Debuff Data
        public int[] StatDiff = new int[(int) Stats.StatCount];

        //Targetting Stuff
        public int TargetType;

        //Costs
        public int[] VitalCost = new int[(int) Vitals.VitalCount];

        //Heal/Damage
        public int[] VitalDiff = new int[(int) Vitals.VitalCount];

        public SpellBase(int id) : base(id)
        {
            Name = "New Spell";
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

            CastingReqs.Load(myBuffer);

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                VitalDiff[i] = myBuffer.ReadInteger();
            }

            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                StatDiff[i] = myBuffer.ReadInteger();
            }

            CritChance = myBuffer.ReadInteger();
            DamageType = myBuffer.ReadInteger();
            ScalingStat = myBuffer.ReadInteger();
            Scaling = myBuffer.ReadInteger();
            Friendly = myBuffer.ReadInteger();

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

            CastingReqs.Save(myBuffer);

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                myBuffer.WriteInteger(VitalDiff[i]);
            }

            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                myBuffer.WriteInteger(StatDiff[i]);
            }

            myBuffer.WriteInteger(CritChance);
            myBuffer.WriteInteger(DamageType);
            myBuffer.WriteInteger(ScalingStat);
            myBuffer.WriteInteger(Scaling);
            myBuffer.WriteInteger(Friendly);

            myBuffer.WriteInteger(Projectile);
            myBuffer.WriteInteger(Data1);
            myBuffer.WriteInteger(Data2);
            myBuffer.WriteInteger(Data3);
            myBuffer.WriteInteger(Data4);
            myBuffer.WriteString(Data5);
            return myBuffer.ToArray();
        }

        public override byte[] BinaryData => SpellData();
    }
}
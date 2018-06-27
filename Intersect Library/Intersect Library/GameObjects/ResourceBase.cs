using System;
using System.Collections.Generic;
using Intersect.GameObjects.Conditions;
using Intersect.Localization;
using Intersect.Models;
using Intersect.Utilities;

namespace Intersect.GameObjects
{
    public class ResourceBase : DatabaseObject<ResourceBase>
    {
        public int Animation;

        // Drops
        public List<ResourceDrop> Drops = new List<ResourceDrop>();

        public string EndGraphic = Strings.Get("general", "none");

        public ConditionLists HarvestingReqs = new ConditionLists();

        // Graphics
        public string InitialGraphic = Strings.Get("general", "none");

        public int MaxHp;

        public int MinHp;
        public int SpawnDuration;
        public int Tool = -1;
        public bool WalkableAfter;
        public bool WalkableBefore;

        public ResourceBase(int id) : base(id)
        {
            Name = "New Resource";
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new ResourceDrop());
            }
        }

        public override byte[] BinaryData => ResourceData();

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

            HarvestingReqs.Load(myBuffer);

            if (MinHp > MaxHp) MaxHp = MinHp;

            myBuffer.Dispose();
        }

        public byte[] ResourceData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(TextUtils.SanitizeNone(InitialGraphic));
            myBuffer.WriteString(TextUtils.SanitizeNone(EndGraphic));
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

            HarvestingReqs.Save(myBuffer);

            return myBuffer.ToArray();
        }

        public class ResourceDrop
        {
            public int Amount;
            public int Chance;
            public int ItemNum;
        }
    }
}
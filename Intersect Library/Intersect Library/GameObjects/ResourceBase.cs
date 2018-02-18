using System;
using System.Collections.Generic;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ResourceBase : DatabaseObject<ResourceBase>
    {
        public int Animation;

        // Drops
        public List<ResourceDrop> Drops = new List<ResourceDrop>();

        public string EndGraphic = null;

        public ConditionLists HarvestingReqs = new ConditionLists();

        // Graphics
        public string InitialGraphic = null;

        public int MaxHp;

        public int MinHp;
        public int SpawnDuration;
        public int Tool = -1;
        public bool WalkableAfter;
        public bool WalkableBefore;

        [JsonConstructor]
        public ResourceBase(int index) : base(index)
        {
            Name = "New Resource";
        }

        

        public class ResourceDrop
        {
            public int Amount;
            public double Chance;
            public int ItemNum;
        }
    }
}
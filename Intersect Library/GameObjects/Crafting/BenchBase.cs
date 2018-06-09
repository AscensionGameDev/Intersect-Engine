using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects.Crafting;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Crafting
{
    public class BenchBase : DatabaseObject<BenchBase>
    {
        public List<Craft> Crafts { get; set; } = new List<Craft>();

        [JsonConstructor]
        public BenchBase(int index) : base(index)
        {
            Name = "New Bench";
        }
    }
}
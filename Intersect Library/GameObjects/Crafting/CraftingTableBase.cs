using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class CraftingTableBase : DatabaseObject<CraftingTableBase>
    {
        public List<int> Crafts = new List<int>();

        [JsonConstructor]
        public CraftingTableBase(int index) : base(index)
        {
            Name = "New Table";
        }
    }
}

using Intersect.Client.Framework.Entities;
using Intersect.Enums;
using Newtonsoft.Json;
using System;

namespace Intersect.Client.Entities
{
    public partial class HotbarInstance : IHotbarInstance
    {

        public Guid BagId { get; set; } = Guid.Empty;

        public Guid ItemOrSpellId { get; set; } = Guid.Empty;

        public int[] PreferredStatBuffs { get; set; } = new int[(int)Stat.StatCount];

        public void Load(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }

    }
}

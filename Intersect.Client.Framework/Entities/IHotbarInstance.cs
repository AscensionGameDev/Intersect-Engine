using System;

namespace Intersect.Client.Framework.Entities
{
    public interface IHotbarInstance
    {
        Guid BagId { get; set; }
        Guid ItemOrSpellId { get; set; }
        int[] PreferredStatBuffs { get; set; }
    }
}
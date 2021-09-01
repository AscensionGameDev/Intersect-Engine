using System;

namespace Intersect.Client.Framework.Entities
{
    public interface IPartyMember
    {
        Guid Id { get; set; }
        int Level { get; set; }
        int[] MaxVital { get; set; }
        string Name { get; set; }
        int[] Vital { get; set; }
    }
}
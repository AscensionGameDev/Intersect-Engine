namespace Intersect.Client.Framework.Entities
{
    public interface IPartyMember
    {
        Guid Id { get; set; }
        int Level { get; set; }
        long[] MaxVital { get; set; }
        string Name { get; set; }
        long[] Vital { get; set; }
    }
}
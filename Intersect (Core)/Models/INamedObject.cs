namespace Intersect.Models;

public interface INamedObject : IWeaklyIdentifiedObject
{
    string Name { get; set; }
}

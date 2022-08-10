using Intersect.Framework;

namespace Intersect.Models;

public interface IStronglyIdentifiedObject<TValue>
{
    Id<TValue> Id { get; }
}


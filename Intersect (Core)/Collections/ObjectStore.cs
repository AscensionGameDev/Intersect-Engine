using Intersect.Models;

namespace Intersect.Collections;

public partial class ObjectStore<TValue> : GuidLookup<TValue>
    where TValue : IWeaklyIdentifiedObject
{
    public static readonly ObjectStore<TValue> Instance = new();

    private ObjectStore() : base() { }
}


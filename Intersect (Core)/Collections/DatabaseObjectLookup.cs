using System.Collections;

using Intersect.Models;

namespace Intersect.Collections;

public partial class DatabaseObjectLookup : GuidLookup<IDatabaseObject>, IGameObjectLookup<IDatabaseObject>
{
    private readonly Type _storedType;

    public DatabaseObjectLookup(Type storedType) : base()
    {
        _storedType = storedType;
    }

    public override Type StoredType => _storedType;

    public override List<IDatabaseObject> ValueList =>
        base.ValueList
            .OrderBy(databaseObject => databaseObject?.Name)
            .ToList();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


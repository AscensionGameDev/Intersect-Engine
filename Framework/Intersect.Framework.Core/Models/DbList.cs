using System.Diagnostics.CodeAnalysis;
using Intersect.Collections;

namespace Intersect.Models;

public partial class DbList<T> : List<Guid> where T : IDatabaseObject
{
    public List<T> GetAll()
    {
        var list = new List<T>();
        foreach (var l in ToArray())
        {
            list.Add((T)DatabaseObjectLookup.LookupMap[typeof(T)].Get(l));
        }

        return list;
    }

    public T Get(Guid id)
    {
        return (T)DatabaseObjectLookup.LookupMap[typeof(T)].Get(id);
    }

    public bool TryGet(Guid id, [NotNullWhen(true)] out T? value)
    {
        if (DatabaseObjectLookup.LookupMap.TryGetValue(typeof(T), out var lookupMap))
        {
            return lookupMap.TryGetValue<T>(id, out value);
        }

        value = default;
        return false;
    }
}
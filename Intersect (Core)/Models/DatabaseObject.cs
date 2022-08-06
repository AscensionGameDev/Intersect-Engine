using Intersect.Collections;
using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.Models;

public abstract partial class DatabaseObject<TObject>
    : Descriptor, IDatabaseObject
    where TObject : DatabaseObject<TObject>
{
    public const string Deleted = "ERR_DELETED";

    protected DatabaseObject() : base() { }

    [JsonConstructor]
    protected DatabaseObject(Guid guid) : base(guid) { }

    public override GameObjectType Type => LookupUtils.GetGameObjectType(typeof(TObject));

    public override void Delete() => Lookup.Delete(this as TObject);

    public int ListIndex() => ListIndex(Id);

    public static Guid IdFromList(int listIndex)
    {
        if (listIndex < 0 || listIndex > Lookup.KeyList.Count)
        {
            return Guid.Empty;
        }

        return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.Name).ToArray()[listIndex];
    }

    public static TObject FromList(int listIndex)
    {
        if (listIndex < 0 || listIndex > Lookup.ValueList.Count)
        {
            return null;
        }

        return (TObject)Lookup.ValueList.OrderBy(databaseObject => databaseObject?.Name).ToArray()[
            listIndex];
    }

    public static KeyValuePair<Guid, string>[] ItemPairs => Lookup.OrderBy(p => p.Value?.Name)
        .Select(pair => new KeyValuePair<Guid, string>(pair.Key, pair.Value?.Name ?? Deleted))
        .ToArray();

    public static string[] Names => Lookup.OrderBy(p => p.Value?.Name)
        .Select(pair => pair.Value?.Name ?? Deleted)
        .ToArray();

    public static DatabaseObjectLookup Lookup => LookupUtils.GetLookup(typeof(TObject));

    public static int ListIndex(Guid id) =>
        Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.Name).ToList().IndexOf(id);

    public static TObject Get(Guid id) => Lookup.Get<TObject>(id);

    public static string GetName(Guid id) => Lookup.Get(id)?.Name ?? Deleted;

    public static string[] GetNameList() =>
        Lookup.Select(pair => pair.Value?.Name ?? Deleted).ToArray();

    public static bool TryGet(Guid id, out TObject databaseObject)
    {
        databaseObject = Get(id);
        return databaseObject != default;
    }
}


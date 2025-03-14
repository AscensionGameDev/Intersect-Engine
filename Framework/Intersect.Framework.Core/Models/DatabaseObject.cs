using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.Framework.Core.Serialization;
using Newtonsoft.Json;

namespace Intersect.Models;


public abstract partial class DatabaseObject<TObject> : IDatabaseObject where TObject : DatabaseObject<TObject>
{

    public const string Deleted = "ERR_DELETED";

    private string mBackup;

    protected DatabaseObject() { }

    [JsonConstructor]
    protected DatabaseObject(Guid descriptorId)
    {
        Id = descriptorId;
        TimeCreated = DateTime.Now.ToBinary();
    }

    public static KeyValuePair<Guid, string>[] ItemPairs => Lookup.OrderBy(p => p.Value?.Name)
        .Select(pair => new KeyValuePair<Guid, string>(pair.Key, pair.Value?.Name ?? Deleted))
        .ToArray();

    public static string[] Names => Lookup.OrderBy(p => p.Value?.Name)
        .Select(pair => pair.Value?.Name ?? Deleted)
        .ToArray();

    public static readonly DatabaseObjectLookup Lookup = DatabaseObjectLookup.GetLookup(typeof(TObject));

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public long TimeCreated { get; set; }

    public static readonly GameObjectType ObjectType = typeof(TObject).TryGetGameObjectType(out var gameObjectType)
        ? gameObjectType
        : throw new InvalidOperationException($"{typeof(TObject).Name} not set up correctly");

    [NotMapped]
    public GameObjectType Type
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ObjectType;
    }

    [JsonIgnore]
    [NotMapped]
    public string DatabaseTable => Type.GetTable();

    [JsonProperty(Order = -4)]
    [Column(Order = 0)]
    public string Name { get; set; }

    [NotMapped, JsonIgnore] public bool IsDeleted => !Lookup.TryGetValue(Id, out var descriptor) || descriptor != this;

    public virtual void Load(string json, bool keepCreationTime = false)
    {
        var oldTime = TimeCreated;
        JsonConvert.PopulateObject(
            json, this, new JsonSerializerSettings
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );

        if (keepCreationTime)
        {
            TimeCreated = oldTime;
        }
    }

    // TODO: Can we remove this comment?
    // public virtual void Load(string json);

    public void MakeBackup()
    {
        mBackup = JsonData;
    }

    public void DeleteBackup()
    {
        mBackup = null;
    }

    public void RestoreBackup()
    {
        if (mBackup != null)
        {
            Load(mBackup);
        }
    }

    [JsonIgnore]
    [NotMapped]

    // TODO: Should eventually be formatting.none
    public virtual string JsonData => JsonConvert.SerializeObject(this, Formatting.Indented);

    public virtual void Delete()
    {
        Lookup.Delete(this as TObject);
    }

    public static Guid IdFromList(int listIndex)
    {
        if (listIndex < 0 || listIndex >= Lookup.KeyList.Count)
        {
            return Guid.Empty;
        }

        return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.Name).ToArray()[listIndex];
    }

    public static TObject FromList(int listIndex)
    {
        if (listIndex < 0 || listIndex >= Lookup.ValueList.Count)
        {
            return null;
        }

        return (TObject) Lookup.ValueList.OrderBy(databaseObject => databaseObject?.Name).ToArray()[
            listIndex];
    }

    public int ListIndex()
    {
        return ListIndex(Id);
    }

    public static int ListIndex(Guid id)
    {
        return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.Name).ToList().IndexOf(id);
    }

    public static TObject Get(Guid id)
    {
        return Lookup.Get<TObject>(id);
    }

    public static string GetName(Guid id)
    {
        return Lookup.Get(id)?.Name ?? Deleted;
    }

    public static string[] GetNameList()
    {
        return Lookup.Select(pair => pair.Value?.Name ?? Deleted).ToArray();
    }

    public static bool TryGet(Guid id, out TObject databaseObject)
    {
        return Lookup.TryGetValue(id, out databaseObject);
    }

}

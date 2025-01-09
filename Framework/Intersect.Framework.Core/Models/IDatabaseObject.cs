using Intersect.Collections;
using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.Models;


public interface IDatabaseObject : INamedObject
{
    GameObjectType Type { get; }

    [JsonIgnore]
    string DatabaseTable { get; }

    long TimeCreated { get; set; }

    [JsonIgnore]
    string JsonData { get; }

    void Load(string json, bool keepCreationTime = false);

    void MakeBackup();

    void RestoreBackup();

    void DeleteBackup();

    void Delete();

}

public partial class DbList<T> : List<Guid>
{

    public List<T> GetAll()
    {
        var list = new List<T>();
        foreach (var l in ToArray())
        {
            list.Add((T) DatabaseObjectLookup.LookupMap[typeof(T)].Get(l));
        }

        return list;
    }

    public T Get(Guid id)
    {
        return (T) DatabaseObjectLookup.LookupMap[typeof(T)].Get(id);
    }

}

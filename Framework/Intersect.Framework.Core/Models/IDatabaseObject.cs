using Intersect.Collections;
using Intersect.Enums;

namespace Intersect.Models;


public interface IDatabaseObject : INamedObject
{
    public GameObjectType Type { get; }

    public string DatabaseTable { get; }

    public long TimeCreated { get; set; }

    public string JsonData { get; }

    public void Load(string json, bool keepCreationTime = false);

    public void MakeBackup();

    public void RestoreBackup();

    public void DeleteBackup();

    public void Delete();

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

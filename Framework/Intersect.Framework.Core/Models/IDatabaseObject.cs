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
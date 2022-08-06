using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Intersect.Enums;
using Intersect.Framework;

using Newtonsoft.Json;

namespace Intersect.Models;

public abstract partial class Descriptor : IDatabaseObject
{
    private string? _backup;

    protected Descriptor() : this(Guid.Empty) { }

    [JsonConstructor]
    protected Descriptor(Guid guid)
    {
        Id = guid;
        TimeCreated = DateTime.Now.ToBinary();
    }

    [IgnoreDataMember, NotMapped]
    public string DatabaseTable => Type.GetTable();

    public string? Folder { get; set; }

    [IgnoreDataMember, NotMapped]
    public virtual string JsonData => JsonConvert.SerializeObject(
        this,
#if DEBUG
        Formatting.Indented,
#else
        Formatting.None,
#endif
        JsonSerializerSettings
    );

    [IgnoreDataMember, NotMapped]
    protected virtual JsonSerializerSettings? JsonSerializerSettings { get; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; protected set; } = Guid.NewGuid();

    [JsonProperty(Order = -4)]
    [Column(Order = 1)]
    public virtual string Name { get; set; }

    [Column(Order = 0)]
    public Id<Folder> ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    [IgnoreDataMember, NotMapped]
    public Folder? Parent { get; set; }

    public long TimeCreated { get; set; }

    [IgnoreDataMember, NotMapped]
    public abstract GameObjectType Type { get; }

    public abstract void Delete();

    public virtual void DeleteBackup() => _backup = default;

    public virtual void Load(string? json, bool keepTimeCreated = false)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        var previousTimeCreated = TimeCreated;
        JsonConvert.PopulateObject(
            json,
            this,
            new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );

        if (keepTimeCreated)
        {
            TimeCreated = previousTimeCreated;
        }
    }

    public virtual void MakeBackup() => _backup = JsonData;

    public virtual void RestoreBackup() => Load(_backup);
}


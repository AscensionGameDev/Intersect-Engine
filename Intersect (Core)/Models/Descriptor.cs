using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Intersect.Enums;
using Intersect.Framework;
using Intersect.Localization.Common;
using Intersect.Localization.Common.Descriptors;
using Intersect.Models.Annotations;
using Newtonsoft.Json;

namespace Intersect.Models;

public abstract partial class Descriptor : IDatabaseObject, IFolderable
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
    [Ignored]
    public string DatabaseTable => Type.GetTable();

    [IgnoreDataMember, NotMapped]
    [Ignored]
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
    [Group(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.General))]
    [InputText]
    [Label(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.Id))]
    [Order(0)]
    [Tooltip(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.IdTooltipOfTheX))]
    public Guid Id { get; protected set; } = Guid.NewGuid();

    [JsonProperty(Order = -4)]
    [Column(Order = 0)]
    [Group(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.General))]
    [InputText(MaximumLength = 255)]
    [Label(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.Name))]
    [Order(1)]
    [Tooltip(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.NameTooltipOfTheX))]
    public virtual string Name { get; set; }

    [Column(Order = 1)]
    [Ignored]
    public Id<Folder>? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    [Group(typeof(CommonGeneralNamespace), nameof(CommonGeneralNamespace.General))]
    [IgnoreDataMember, NotMapped]
    [InputLookup(typeof(Folder), nameof(Folder.Id), nameof(ParentId))]
    [Label(typeof(DescriptorsNamespace), nameof(DescriptorsNamespace.Folder))]
    [Order(2)]
    [Tooltip(typeof(DescriptorsNamespace), nameof(DescriptorsNamespace.FolderTooltipOfTheX))]
    public Folder? Parent { get; set; }

    public long TimeCreated { get; set; }

    [IgnoreDataMember, NotMapped]
    [Ignored]
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

    public bool Matches(Guid guid, bool matchParent = false, bool matchChildren = false) =>
        Id == guid ||
        (matchParent && (Parent?.Matches(guid, matchParent: true, matchChildren: false) ?? false));

    public bool Matches(string @string, StringComparison stringComparison, bool matchParent = false, bool matchChildren = false) =>
        Name.Contains(@string, stringComparison) ||
        (matchParent && (Parent?.Matches(@string, stringComparison, matchParent: true, matchChildren: false) ?? false));
}


using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Intersect.Enums;
using Intersect.Framework;
using Intersect.Framework.Collections;

namespace Intersect.Models;

[DataContract]
public partial class Folder :
    IComparable<Folder>,
    IFolderable,
    IStronglyIdentifiedObject<Folder>,
    IWeaklyIdentifiedObject
{
    public Folder() { }

    public Folder(Id<Folder> id)
    {
        Id = id;
    }

    [IgnoreDataMember]
    public virtual ICollection<IFolderable> Children { get; private set; }

    [Column(Order = 1)]
    [DataMember(Order = 1)]
    public GameObjectType DescriptorType { get; set; }

    [IgnoreDataMember, NotMapped]
    string? IFolderable.Folder
    {
        get => Parent?.Name;
        set => throw new NotImplementedException();
    }

    Guid IWeaklyIdentifiedObject.Id => Id.Guid;

    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DataMember(Order = 0)]
    public Id<Folder> Id { get; protected set; } = Id<Folder>.New();

    [Column(Order = 2)]
    [DataMember(Order = 2)]
    public Id<ContentString> NameId { get; set; }

    [ForeignKey(nameof(NameId))]
    [IgnoreDataMember, NotMapped]
    public ContentString Name { get; set; }

    [Column(Order = 3)]
    [DataMember(Order = 3)]
    public Id<Folder>? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    [IgnoreDataMember, NotMapped]
    public Folder? Parent { get; set; }

    public int CompareTo(Folder? other) =>
        other == default ? -1 : Name.CompareTo(other.Name);

    public void LinkChildren(IEnumerable<IFolderable> folderables)
    {
        Children ??= new HashSet<IFolderable>();

        Children.AddRange(
            folderables.Where(
                folderable =>
                    Id == folderable.ParentId && (
                        folderable is Descriptor descriptor && DescriptorType == descriptor.Type
                        || folderable is Folder folder && DescriptorType == folder.DescriptorType
                    )
            )
        );
    }

    public bool Matches(Guid guid, bool matchParent = false, bool matchChildren = false) =>
        Id.Guid == guid ||
        (matchParent && (Parent?.Matches(guid, matchParent: true, matchChildren: false) ?? false)) ||
        (matchChildren && Children.Any(child => child.Matches(guid, matchParent: false, matchChildren: true)));

    public bool Matches(string @string, StringComparison stringComparison, bool matchParent = false, bool matchChildren = false) =>
        (Name?.Matches(@string, stringComparison) ?? false) ||
        (matchParent && (Parent?.Matches(@string, stringComparison, matchParent: true, matchChildren: false) ?? false)) ||
        (matchChildren && Children.Any(child => child.Matches(@string, stringComparison, matchParent: false, matchChildren: true)));
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Intersect.Enums;
using Intersect.Framework;

namespace Intersect.Models;

public partial class Folder : IFolderable
{
    public virtual ICollection<IFolderable> Children { get; private set; }

    [Column(Order = 1)]
    public GameObjectType DescriptorType { get; set; }

    [IgnoreDataMember, NotMapped]
    string? IFolderable.Folder
    {
        get => Parent?.Name;
        set => throw new NotImplementedException();
    }

    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Id<Folder> Id { get; protected set; } = Id<Folder>.New();

    [Column(Order = 2)]
    protected Id<ContentString> NameId { get; set; }

    [ForeignKey(nameof(NameId))]
    [IgnoreDataMember, NotMapped]
    public ContentString Name { get; set; }

    [Column(Order = 3)]
    public Id<Folder>? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    [IgnoreDataMember, NotMapped]
    public Folder? Parent { get; set; }
}

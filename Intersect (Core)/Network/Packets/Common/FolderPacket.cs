using Intersect.Enums;
using Intersect.Framework;
using Intersect.Models;

using MessagePack;

namespace Intersect.Network.Packets.Common;

[MessagePackObject]
public class FolderPacket : IntersectPacket
{
    [IgnoreMember]
    private Folder _folder = default!;

    public FolderPacket() { }

    public FolderPacket(Folder folder) => Folder = folder;

    [IgnoreMember]
    public Folder? Folder
    {
        get => _folder;
        set
        {
            _folder = value ?? throw new ArgumentNullException(nameof(value));
            Id = _folder.Id;
            DescriptorType = Folder.DescriptorType;
            NameId = Folder.Name.Id;
            ParentId = Folder.ParentId;
        }
    }

    [IgnoreMember]
    public Id<Folder> Id
    {
        get => new(IdGuid);
        set => IdGuid = value.Guid;
    }

    [Key(0)]
    private Guid IdGuid { get; set; }

    [Key(1)]
    public GameObjectType DescriptorType { get; set; }

    [IgnoreMember]
    public Id<ContentString> NameId
    {
        get => new(NameIdGuid);
        set => NameIdGuid = value.Guid;
    }

    [Key(2)]
    private Guid NameIdGuid { get; set; }

    [IgnoreMember]
    public Id<Folder>? ParentId { get; set; }

    [Key(3)]
    private Guid? ParentIdGuid
    {
        get => ParentId?.Guid;
        set => ParentId = value.HasValue ? new(value.Value) : default;
    }
}

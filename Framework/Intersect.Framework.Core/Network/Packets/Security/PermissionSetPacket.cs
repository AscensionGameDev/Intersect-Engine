using Intersect.Framework.Core.Security;
using Intersect.Network;
using MessagePack;

namespace Intersect.Framework.Core.Network.Packets.Security;

[MessagePackObject]
public partial class PermissionSetPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public PermissionSetPacket()
    {
    }

    public PermissionSetPacket(PermissionSet permissionSet)
    {
        PermissionSet = permissionSet;
    }

    [Key(0)]
    public PermissionSet PermissionSet { get; set; }
}

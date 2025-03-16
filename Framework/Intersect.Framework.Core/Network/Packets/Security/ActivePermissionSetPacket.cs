using Intersect.Network;
using MessagePack;

namespace Intersect.Framework.Core.Network.Packets.Security;

[MessagePackObject]
public partial class ActivePermissionSetPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public ActivePermissionSetPacket()
    {
    }

    public ActivePermissionSetPacket(string permissionSetName)
    {
        PermissionSetName = permissionSetName;
    }

    [Key(0)]
    public string PermissionSetName { get; set; }
}
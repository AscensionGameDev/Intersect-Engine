using Intersect.Framework.Core.Security;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class PasswordChangeResultPacket : IntersectPacket
{

    //Parameterless Constructor for MessagePack
    public PasswordChangeResultPacket()
    {
    }

    public PasswordChangeResultPacket(PasswordResetResultType succeeded)
    {
        ResultType = succeeded;
    }

    [Key(0)]
    public PasswordResetResultType ResultType { get; set; }

}

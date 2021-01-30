using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PasswordResetResultPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PasswordResetResultPacket()
        {
        }

        public PasswordResetResultPacket(bool succeeded)
        {
            Succeeded = succeeded;
        }

        [Key(0)]
        public bool Succeeded { get; set; }

    }

}

using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class RequestPasswordResetPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public RequestPasswordResetPacket()
        {
        }

        public RequestPasswordResetPacket(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }

        [Key(0)]
        public string NameOrEmail { get; set; }

    }

}

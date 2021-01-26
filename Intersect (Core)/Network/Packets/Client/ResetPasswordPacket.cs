using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class ResetPasswordPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ResetPasswordPacket()
        {
        }

        public ResetPasswordPacket(string nameOrEmail, string resetCode, string newPassword)
        {
            NameOrEmail = nameOrEmail;
            ResetCode = resetCode;
            NewPassword = newPassword;
        }

        [Key(0)]
        public string NameOrEmail { get; set; }

        [Key(1)]
        public string ResetCode { get; set; }

        [Key(2)]
        public string NewPassword { get; set; }

    }

}

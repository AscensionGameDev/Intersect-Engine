namespace Intersect.Network.Packets.Client
{

    public class ResetPasswordPacket : CerasPacket
    {

        public ResetPasswordPacket(string nameOrEmail, string resetCode, string newPassword)
        {
            NameOrEmail = nameOrEmail;
            ResetCode = resetCode;
            NewPassword = newPassword;
        }

        public string NameOrEmail { get; set; }

        public string ResetCode { get; set; }

        public string NewPassword { get; set; }

    }

}

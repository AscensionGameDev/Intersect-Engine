using System;
using System.Linq;

using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using Intersect.Utilities;

namespace Intersect.Server.Notifications
{

    public class PasswordResetEmail : Notification
    {

        public PasswordResetEmail(User user) : base(user.Email)
        {
            LoadFromTemplate("PasswordReset", user.Name);
            Subject = Strings.PasswordResetNotification.subject;
            var resetCode = GenerateResetCode(6);
            Body = Body.Replace("{{code}}", resetCode);
            Body = Body.Replace("{{expiration}}", Options.PasswordResetExpirationMinutes.ToString());
            user.PasswordResetCode = resetCode;
            user.PasswordResetTime = DateTime.UtcNow.AddMinutes(Options.PasswordResetExpirationMinutes);
            user.Save();
        }

        private string GenerateResetCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[Randomization.Next(s.Length)]).ToArray());
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.General;
using Intersect.Server.Localization;

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
        }

        private string GenerateResetCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Globals.Rand.Next(s.Length)]).ToArray());
        }
    }
}

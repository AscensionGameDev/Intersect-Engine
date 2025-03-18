using System.Diagnostics.CodeAnalysis;
using Intersect.Core;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using Intersect.Utilities;

namespace Intersect.Server.Notifications;

public partial class PasswordResetEmail : Notification
{
    private PasswordResetEmail(User user) : base(user.Email)
    {
        Recipient = user.Name;
        Subject = Strings.PasswordResetNotification.Subject;
    }

    public static bool TryCreate(User user, [NotNullWhen(true)] out PasswordResetEmail? passwordResetEmail)
    {
        if (!TryLoadTemplate("PasswordReset", out var template))
        {
            passwordResetEmail = null;
            return false;
        }

        var applicationContext = ApplicationContext.CurrentContext;
        if (string.IsNullOrWhiteSpace(template))
        {
            applicationContext.Logger.LogError("Unable to create a password reset email because the template is empty");
            passwordResetEmail = null;
            return false;
        }

        var resetCode = GenerateResetCode(6);
        user.PasswordResetCode = resetCode;
        var passwordResetTimeMinutes = Options.Instance.ValidPasswordResetTimeMinutes;
        user.PasswordResetTime = DateTime.UtcNow.AddMinutes(passwordResetTimeMinutes);
        user.Save();

        var body = PopulatePasswordResetTemplate(template, user, resetCode, passwordResetTimeMinutes);
        if (string.IsNullOrWhiteSpace(body))
        {
            applicationContext.Logger.LogError(
                "Unable to create a password reset email because the body is empty after populating the template for user '{UserName}' ({UserId})",
                user.Name,
                user.Id
            );
            passwordResetEmail = null;
            return false;
        }

        var isHtml = IsTemplateHTML(body);
        passwordResetEmail = new PasswordResetEmail(user)
        {
            Body = body, IsHtml = isHtml,
        };
        return true;
    }

    private static string PopulatePasswordResetTemplate(
        string template,
        User user,
        string resetCode,
        int resetCodeExpirationMinutes
    )
    {
        var body = PopulateBasicTemplate(template, user)
            .Replace("{{code}}", resetCode)
            .Replace("{{expiration}}", resetCodeExpirationMinutes.ToString());
        return body;
    }

    private static string GenerateResetCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(chars, length).Select(s => s[Randomization.Next(s.Length)]).ToArray());
    }
}
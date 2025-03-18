using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Intersect.Core;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Localization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Intersect.Server.Notifications
{
    public partial class Notification
    {
        public Notification(string to, string? subject = null, bool html = false)
        {
            ToAddress = to;
            Subject = subject;
            IsHtml = html;
        }

        public string? Recipient { get; init; }

        public string ToAddress { get; init; }

        public string? Subject { get; init; }

        public string? Body { get; set; }

        public bool IsHtml { get; set; }

        public bool TrySend()
        {
            // If there is no subject log an error
            if (string.IsNullOrWhiteSpace(Subject))
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Unable to send email to '{SenderAddress}' because the subject is empty (or whitespace).",
                    ToAddress
                );
                return false;
            }

            // If there is no message body log an error
            if (string.IsNullOrWhiteSpace(Body))
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Unable to send email ({Subject}) to '{SenderAddress}' because the body is empty (or whitespace).",
                    Subject,
                    ToAddress
                );
                return false;
            }

            // If SMTP is not set up correctly log an error
            var smtpSettings = Options.Instance.SmtpSettings;
            if (!smtpSettings.IsValid())
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Unable to send email ({Subject}) to '{SenderAddress}' because SMTP is not correctly configured.",
                    Subject,
                    ToAddress
                );
                return false;
            }

            var username = smtpSettings.Username;
            var password = smtpSettings.Password;
            var shouldAuthenticate = !(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password));

            try
            {
                using SmtpClient client = new();
                client.Connect(
                    smtpSettings.Host,
                    smtpSettings.Port,
                    smtpSettings.UseSsl
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.Auto
                );

                if (shouldAuthenticate)
                {
                    client.Authenticate(username, password);
                }

                var fromAddress = new MailboxAddress(smtpSettings.FromName, smtpSettings.FromAddress);
                var toAddress = new MailboxAddress(ToAddress, ToAddress);

                var message = new MimeMessage();
                message.To.Add(toAddress);
                message.From.Add(fromAddress);
                message.Subject = Subject;

                var bodyBuilder = new BodyBuilder();
                if (IsHtml)
                {
                    bodyBuilder.HtmlBody = Body;
                }
                else
                {
                    bodyBuilder.TextBody = Body;
                }

                message.Body = bodyBuilder.ToMessageBody();

                client.Send(message);
                client.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    "Failed to send email (Subject: " +
                    Subject +
                    ") to " +
                    ToAddress +
                    ". Reason: Uncaught Error" +
                    Environment.NewLine +
                    ex.ToString()
                );
                return false;
            }
        }

        protected static bool TryLoadTemplate(string templateName, [NotNullWhen(true)] out string? template)
        {
            var pathToTemplates = Path.Combine("resources", "notifications");
            if (!Directory.Exists(pathToTemplates))
            {
                Directory.CreateDirectory(pathToTemplates);
            }

            var pathToTemplate = Path.Combine("resources", "notifications", $"{templateName}.html");
            if (!File.Exists(pathToTemplate))
            {
                template = null;
                return false;
            }

            try
            {
                template = File.ReadAllText(pathToTemplate);
                if (!string.IsNullOrWhiteSpace(template))
                {
                    return true;
                }

                template = null;
                return false;

            }
            catch (Exception exception)
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    exception,
                    "Failed to load email template '{TemplateName}'",
                    templateName
                );
                template = null;
                return false;
            }
        }

        protected static string PopulateBasicTemplate(string template, User user)
        {
            return template.Replace("{{product}}", Strings.Notifications.Product)
                .Replace("{{copyright}}", Strings.Notifications.Copyright)
                .Replace("{{name}}", user.Name);
        }

        protected static bool IsTemplateHTML(string template) => PatternHtmlTag.IsMatch(template);

        private static readonly Regex PatternHtmlTag = CompilePatternHtmlTag();

        [GeneratedRegex(@"\<[\s\n\r]*html[^\>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled, "en-US")]
        private static partial Regex CompilePatternHtmlTag();
    }
}

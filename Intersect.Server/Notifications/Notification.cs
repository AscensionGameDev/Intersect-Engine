using Intersect.Core;
using Intersect.Server.Localization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Intersect.Server.Notifications
{

    public partial class Notification
    {

        public Notification(string to, string subject = "", bool html = false)
        {
            ToAddress = to;
            Subject = subject;
            IsHtml = html;
        }

        public string ToAddress { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = false;

        public bool Send()
        {
            //Check and see if smtp is even setup
            if (Options.Instance.SmtpSettings.IsValid())
            {
                //Make sure we have a body
                if (!string.IsNullOrEmpty(Body))
                {
                    try
                    {
                        //Send the email
                        var fromAddress = new MailboxAddress(Options.Instance.SmtpSettings.FromName, Options.Instance.SmtpSettings.FromAddress);
                        var toAddress = new MailboxAddress(ToAddress, ToAddress);

                        using (var client = new SmtpClient())
                        {
                            client.Connect(Options.Instance.SmtpSettings.Host, Options.Instance.SmtpSettings.Port, Options.Instance.SmtpSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                            client.Authenticate(Options.Instance.SmtpSettings.Username, Options.Instance.SmtpSettings.Password);

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
                        }

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
                else
                {
                    ApplicationContext.Context.Value?.Logger.LogWarning(
                        "Failed to send email (Subject: " +
                        Subject +
                        ") to " +
                        ToAddress +
                        ". Reason: SMTP not configured!"
                    );
                    return false;
                }
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Failed to send email (Subject: " + Subject + ") to " + ToAddress + ". Reason: SMTP not configured!"
                );
                return false;
            }
        }

        protected bool LoadFromTemplate(string templatename, string username)
        {
            var templatesDir = Path.Combine("resources", "notifications");
            if (!Directory.Exists(templatesDir))
            {
                Directory.CreateDirectory(templatesDir);
            }

            var filepath = Path.Combine("resources", "notifications", templatename + ".html");
            if (File.Exists(filepath))
            {
                IsHtml = true;
                Body = File.ReadAllText(filepath);
                Body = Body.Replace("{{product}}", Strings.Notifications.Product);
                Body = Body.Replace("{{copyright}}", Strings.Notifications.Copyright);
                Body = Body.Replace("{{name}}", username);

                return true;
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    "Failed to load email template (Subject: " +
                    Subject +
                    ") for " +
                    ToAddress +
                    ". Reason: Template " +
                    templatename +
                    ".html not found!"
                );
            }

            return false;
        }

    }

}

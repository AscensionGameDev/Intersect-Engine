using System;
using System.IO;
using System.Net;
using Intersect.Logging;
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
            if (Options.Smtp.IsValid())
            {
                //Make sure we have a body
                if (!string.IsNullOrEmpty(Body))
                {
                    try
                    {
                        //Send the email
                        var fromAddress = new MailboxAddress(Options.Smtp.FromName, Options.Smtp.FromAddress);
                        var toAddress = new MailboxAddress(ToAddress, ToAddress);

                        using (var client = new SmtpClient())
                        {
                            client.Connect(Options.Smtp.Host, Options.Smtp.Port, Options.Smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                            client.Authenticate(Options.Smtp.Username, Options.Smtp.Password);

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
                        Log.Error(
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
                    Log.Warn(
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
                Log.Warn(
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
                Log.Warn(
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

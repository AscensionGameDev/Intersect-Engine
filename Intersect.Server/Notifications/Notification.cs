using System;
using System.IO;
using System.Net;
using System.Net.Mail;

using Intersect.Logging;
using Intersect.Server.Localization;

namespace Intersect.Server.Notifications
{

    public class Notification
    {

        public Notification(string to, string subject = "", bool html = false)
        {
            ToAddress = to;
            Subject = subject;
            IsHtml = html;
        }

        public string ToAddress { get; set; } = "";

        public string Subject { get; set; } = "";

        public string Body { get; set; } = "";

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
                        var fromAddress = new MailAddress(Options.Smtp.FromAddress, Options.Smtp.FromName);
                        var toAddress = new MailAddress(ToAddress);

                        var smtp = new SmtpClient
                        {
                            Host = Options.Smtp.Host,
                            Port = Options.Smtp.Port,
                            EnableSsl = Options.Smtp.UseSsl,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(Options.Smtp.Username, Options.Smtp.Password)
                        };

                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = Subject,
                            Body = Body,
                            IsBodyHtml = IsHtml
                        })
                        {
                            smtp.Send(message);
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
                Body = Body.Replace("{{product}}", Strings.Notifications.product);
                Body = Body.Replace("{{copyright}}", Strings.Notifications.copyright);
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

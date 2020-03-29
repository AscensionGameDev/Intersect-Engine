namespace Intersect.Config
{

    public class SmtpSettings
    {

        public string FromAddress { get; set; } = "";

        public string FromName { get; set; } = "";

        public string Host { get; set; } = "";

        public int Port { get; set; } = 587;

        public bool UseSsl { get; set; } = true;

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(FromAddress) && !string.IsNullOrEmpty(Host);
        }

    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Classes.Networking
{
    public static class PortChecker
    {
        public static bool CanYouSeeMe(int port, out string externalIp)
        {
            externalIp = "";
            try
            {
                WebRequest request = WebRequest.Create("https://www.ascensiongamedev.com/resources/canyouseeme.php?port=" + port + "&time=" + DateTime.Now.ToBinary().ToString());
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream data = response.GetResponseStream();
                    string html = String.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        var serverStatus = sr.ReadToEnd();
                        if (serverStatus.Contains("/"))
                        {
                            var onlineStatus = serverStatus.Split("/".ToCharArray())[0];
                            var ip = serverStatus.Split("/".ToCharArray())[1];
                            IPAddress address;
                            if (IPAddress.TryParse(ip, out address))
                            {
                                externalIp = ip;
                            }
                            if (onlineStatus.Contains("Online"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}

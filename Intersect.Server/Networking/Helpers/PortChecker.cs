using System;
using System.Linq;
using System.Net;

using Intersect.Logging;

using WebSocketSharp;

namespace Intersect.Server.Networking.Helpers
{

    public static class PortChecker
    {

        public static bool CanYouSeeMe(int port, out string externalIp)
        {
            externalIp = "";
            try
            {
                var request = WebRequest.Create(
                    "http://status.freemmorpgmaker.com:5400/?time=" + DateTime.Now.ToBinary().ToString()
                );

                request.Headers.Add("port", port.ToString());
                request.Timeout = 4000;
                var response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Headers.HasKeys())
                    {
                        if (response.Headers.AllKeys.Contains("ip"))
                        {
                            externalIp = response.Headers["ip"];
                        }

                        if (response.Headers.Contains("players"))
                        {
                            if (int.Parse(response.Headers["players"]) > -1)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Debug(exception);
            }

            return false;
        }

    }

}

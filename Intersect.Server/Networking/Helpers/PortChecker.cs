using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Intersect.Logging;
using WebSocketSharp;

namespace Intersect.Server.Networking.Helpers
{
    public enum PortCheckResult
    {
        Unknown,
        Open,
        PossiblyOpen,
        IntersectResponseNoPlayerCount,
        IntersectResponseInvalidPlayerCount,
        InvalidPortCheckerRequest,
        PortCheckerServerError,
        PortCheckerServerDown,
        PortCheckerServerUnexpectedResponse,
        Inaccessible
    }

    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class PortChecker
    {
        public static PortCheckResult CanYouSeeMe(int port, out string externalIp)
        {
            externalIp = string.Empty;
            try
            {
                var request = WebRequest.Create(
                    $"http://status.freemmorpgmaker.com:5400/?time={DateTime.Now.ToBinary()}"
                );

                request.Headers.Add("port", port.ToString(CultureInfo.InvariantCulture));
                request.Timeout = 4000;
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var statusCodePrefix = (int)response.StatusCode / 100;
                    switch (statusCodePrefix)
                    {
                        case 1:
                            return PortCheckResult.PortCheckerServerUnexpectedResponse;

                        case 2:
                            return PortCheckResult.PossiblyOpen;

                        case 3:
                            return PortCheckResult.PortCheckerServerUnexpectedResponse;

                        case 4:
                            return PortCheckResult.InvalidPortCheckerRequest;

                        case 5:
                            return PortCheckResult.PortCheckerServerError;

                        default:
                            return PortCheckResult.Unknown;
                    }
                }

                if (!response.Headers.HasKeys())
                {
                    return PortCheckResult.Inaccessible;
                }

                if (response.Headers.AllKeys.Contains("ip"))
                {
                    externalIp = response.Headers["ip"];
                }

                if (!response.Headers.Contains("players"))
                {
                    return PortCheckResult.IntersectResponseNoPlayerCount;
                }

                if (!int.TryParse(response.Headers["players"], out var players) || players < 0)
                {
                    return PortCheckResult.IntersectResponseInvalidPlayerCount;
                }

                return PortCheckResult.Open;
            }
            catch (WebException webException)
            {
                Log.Debug(webException);
                return PortCheckResult.PortCheckerServerDown;
            }
            catch (Exception exception)
            {
                Log.Debug(exception);
                return PortCheckResult.PortCheckerServerError;
            }
        }
    }
}
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using Intersect.Logging;
using Microsoft.AspNetCore.WebUtilities;

namespace Intersect.Server.Networking.Helpers;

public static class PortChecker
{
    internal static readonly string Secret = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    private static DateTime DateTimeFrom(IEnumerable<string> values)
    {
        var binaryDateTime = values.Select(value => long.TryParse(value, out var ticks) ? ticks : 0).Max();
        return DateTime.FromBinary(binaryDateTime);
    }

    public static PortCheckResult CanYouSeeMe(int port, out string externalIp)
    {
        externalIp = string.Empty;
        try
        {
            var portCheckerUrl = Options.Instance.PortCheckerUrl;
            if (string.IsNullOrWhiteSpace(portCheckerUrl))
            {
                portCheckerUrl = "http://status.freemmorpgmaker.com:5400/";
            }

            UriBuilder portCheckerUriBuilder = new(portCheckerUrl)
            {
                Query = $"?time={DateTime.UtcNow.ToBinary()}",
            };

            var portCheckerUri = portCheckerUriBuilder.Uri;

            HttpClientHandler httpClientHandler = new();
#if DEBUG
            httpClientHandler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
#endif
            using var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromMilliseconds(10000);

            HttpRequestMessage requestMessage = new(HttpMethod.Get, portCheckerUri)
            {
                Headers = { { "port", port.ToString(CultureInfo.InvariantCulture) } },
            };

            var responseMessage = httpClient.Send(requestMessage);

            DateTime receivedResponseTime = DateTime.UtcNow;

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                var statusCodePrefix = (int)responseMessage.StatusCode / 100;
                return statusCodePrefix switch
                {
                    1 => PortCheckResult.PortCheckerServerUnexpectedResponse,
                    2 => PortCheckResult.PossiblyOpen,
                    3 => PortCheckResult.PortCheckerServerUnexpectedResponse,
                    4 => PortCheckResult.InvalidPortCheckerRequest,
                    5 => PortCheckResult.PortCheckerServerError,
                    _ => PortCheckResult.Unknown,
                };
            }

            if (!responseMessage.Headers.Any())
            {
                Log.Debug("Received no headers from port checker service.");
                return PortCheckResult.Inaccessible;
            }

            DateTime time = default;
            if (responseMessage.Headers.TryGetValues("time", out var timeValues))
            {
                time = DateTimeFrom(timeValues);
            }

            DateTime requestTime = default;
            if (responseMessage.Headers.TryGetValues("request_time", out var requestTimeValues))
            {
                requestTime = DateTimeFrom(requestTimeValues);
            }

            DateTime responseTime = default;
            if (responseMessage.Headers.TryGetValues("response_time", out var responseTimeValues))
            {
                responseTime = DateTimeFrom(responseTimeValues);
            }

            Log.Verbose($"Port checker service received request after {(requestTime - time).TotalMilliseconds}ms.");
            Log.Verbose($"Port checker service responded to the request after {(responseTime - requestTime).TotalMilliseconds}ms.");
            Log.Verbose($"Port checker service response was received after {(receivedResponseTime - responseTime).TotalMilliseconds}ms.");

            if (!responseMessage.Headers.TryGetValues("ip", out var ipValues))
            {
                Log.Debug("Received no 'ip' header from port checker service.");
                return PortCheckResult.InvalidPortCheckerResponse;
            }

            externalIp = ipValues.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
            if (string.IsNullOrWhiteSpace(externalIp))
            {
                Log.Debug("Received empty 'ip' header from port checker service.");
                return PortCheckResult.InvalidPortCheckerResponse;
            }

            if (!responseMessage.Headers.TryGetValues("secret", out var portCheckerSecretValues))
            {
                Log.Debug("Received no 'secret' header from port checker service.");
                return PortCheckResult.InvalidPortCheckerResponse;
            }

            var portCheckerSecret = portCheckerSecretValues.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
            if (string.IsNullOrWhiteSpace(portCheckerSecret))
            {
                Log.Debug("Received empty 'secret' header from port checker service.");
                return PortCheckResult.InvalidPortCheckerResponse;
            }

            if (string.Equals(Secret, portCheckerSecret, StringComparison.Ordinal))
            {
                return PortCheckResult.Open;
            }

            Log.Debug($"Received invalid 'secret' header from port checker service: {portCheckerSecret}");
            return PortCheckResult.InvalidPortCheckerResponse;
        }
        catch (HttpRequestException httpRequestException)
        {
            Log.Debug(httpRequestException);

            return httpRequestException.StatusCode.HasValue
                ? PortCheckResult.PortCheckerServerError
                : PortCheckResult.PortCheckerServerDown;
        }
        catch (TaskCanceledException taskCanceledException)
        {
            Log.Debug(taskCanceledException.Message);
            return PortCheckResult.PortCheckerServerError;
        }
        catch (Exception exception)
        {
            Log.Debug(exception);
            return PortCheckResult.PortCheckerServerError;
        }
    }
}
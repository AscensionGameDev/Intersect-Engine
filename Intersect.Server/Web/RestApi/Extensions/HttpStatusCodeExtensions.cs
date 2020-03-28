using System.Net;
using System.Net.Http;

using Intersect.Logging;

namespace Intersect.Server.Web.RestApi.Extensions
{

    public static class HttpStatusCodeExtensions
    {

        public static LogLevel ToIntersectLogLevel(this HttpStatusCode httpStatusCode, HttpMethod httpMethod = null)
        {
            // 1xx
            if (httpStatusCode < HttpStatusCode.OK)
            {
                return LogLevel.Diagnostic;
            }

            // 2xx
            if (httpStatusCode < HttpStatusCode.MultipleChoices)
            {
                if (httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Head || httpMethod == HttpMethod.Options)
                {
                    return LogLevel.Debug;
                }

                return LogLevel.Info;
            }

            // 3xx
            if (httpStatusCode < HttpStatusCode.BadRequest)
            {
                return LogLevel.Info;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (httpStatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    if (httpMethod == HttpMethod.Get ||
                        httpMethod == HttpMethod.Head ||
                        httpMethod == HttpMethod.Options)
                    {
                        return LogLevel.Info;
                    }

                    return LogLevel.Warn;

                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                    if (httpMethod == HttpMethod.Get ||
                        httpMethod == HttpMethod.Head ||
                        httpMethod == HttpMethod.Options)
                    {
                        return LogLevel.Warn;
                    }

                    return LogLevel.Error;

                case (HttpStatusCode) 429:
                    return LogLevel.Error;
            }

            // 4xx
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (httpStatusCode < HttpStatusCode.InternalServerError)
            {
                return LogLevel.Trace;
            }

            return LogLevel.Error;
        }

    }

}

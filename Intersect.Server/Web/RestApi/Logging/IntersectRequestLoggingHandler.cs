using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Server.Database.Logging;
using Intersect.Server.Web.RestApi.Extensions;

namespace Intersect.Server.Web.RestApi.Logging
{

    // TODO: Probably a good idea to remove this since it was replaced with middleware
    public class IntersectRequestLoggingHandler : DelegatingHandler
    {

        public LogLevel LogLevel { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Method == null)
            {
                throw new ArgumentNullException(nameof(request.Method));
            }

            if (request.RequestUri == null)
            {
                throw new ArgumentNullException(nameof(request.RequestUri));
            }

            var requestMethod = request.Method;
            var requestHeaders = request.Headers?.ToDictionary(pair => pair.Key, pair => pair.Value?.ToList()) ??
                                 new Dictionary<string, List<string>>();

            var requestUri = new Uri(request.RequestUri?.OriginalString);

            var response = await (base.SendAsync(request, cancellationToken) ?? throw new InvalidOperationException());
            var responseStatusCode = response.StatusCode;
            var responseHeaders = response.Headers?.ToDictionary(pair => pair.Key, pair => pair.Value?.ToList()) ??
                                  new Dictionary<string, List<string>>();

            var responseReasonPhrase = response.ReasonPhrase;

            var logLevel = responseStatusCode.ToIntersectLogLevel(requestMethod);

            // ReSharper disable once InvertIf
            if (logLevel < LogLevel)
            {
                var log = new RequestLog
                {
                    Time = DateTime.UtcNow,
                    Level = logLevel,
                    Method = requestMethod.Method,
                    StatusCode = (int) responseStatusCode,
                    StatusMessage = responseReasonPhrase,
                    Uri = requestUri.OriginalString,
                    RequestHeaders = requestHeaders,
                    ResponseHeaders = responseHeaders
                };

                using (var context = LoggingContext.Create())
                {
                    context.Add(log);
                    try
                    {
                        await (context.SaveChangesAsync(cancellationToken) ?? throw new InvalidOperationException());
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);

                        throw;
                    }
                }
            }

            return response;
        }

    }

}

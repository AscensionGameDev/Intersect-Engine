using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Server.Database.Logging;
using Intersect.Server.Web.RestApi.Extensions;

using Microsoft.Owin;

namespace Intersect.Server.Web.RestApi.Middleware
{

    public class IntersectRequestLoggingMiddleware : OwinMiddleware
    {

        /// <inheritdoc />
        public IntersectRequestLoggingMiddleware(OwinMiddleware next, LogLevel logLevel = LogLevel.Info) : base(next)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; set; }

        /// <inheritdoc />
        public override async Task Invoke(IOwinContext owinContext)
        {
            try
            {
                if (owinContext == null)
                {
                    throw new ArgumentNullException(nameof(owinContext));
                }

                if (owinContext.Request == null)
                {
                    throw new ArgumentNullException(nameof(owinContext.Request));
                }

                var request = owinContext.Request;
                var requestMethod = new HttpMethod(request.Method?.ToUpperInvariant() ?? HttpMethod.Get?.Method);
                var requestHeaders = request.Headers?.ToDictionary(pair => pair.Key, pair => pair.Value?.ToList()) ??
                                     new Dictionary<string, List<string>>();

                var requestUri = new Uri(
                    request.Uri?.OriginalString ?? throw new ArgumentNullException(nameof(request.Uri))
                );

                var internalServerError = false;
                if (Next != null)
                {
                    try
                    {
                        var task = Next.Invoke(owinContext) ?? throw new InvalidOperationException(@"Task is null");
                        await task;
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        internalServerError = true;
                    }
                }

                if (owinContext.Response == null)
                {
                    throw new ArgumentNullException(nameof(owinContext.Response));
                }

                var response = owinContext.Response;
                var responseStatusCode = internalServerError
                    ? HttpStatusCode.InternalServerError
                    : (HttpStatusCode) response.StatusCode;

                var responseHeaders = new Dictionary<string, List<string>>();
                if (response.Headers != null)
                {
                    // This is necessary because *apparently* the response headers
                    // dictionary does not combine duplicate header keys into a
                    // single entry's value array, and regular dictionaries
                    // most definitely do not appreciate that, nor will JSON.
                    foreach (var responseHeaderPair in response.Headers)
                    {
                        var key = responseHeaderPair.Key;
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            continue;
                        }

                        var aggregateValue = new List<string>();
                        if (responseHeaders.TryGetValue(key, out var existingValue) && existingValue != null)
                        {
                            aggregateValue.AddRange(existingValue);
                        }

                        var value = responseHeaderPair.Value;
                        if (value != null)
                        {
                            aggregateValue.AddRange(value);
                        }

                        responseHeaders[key] = aggregateValue;
                    }
                }

                var logLevel = responseStatusCode.ToIntersectLogLevel();

                // ReSharper disable once InvertIf
                if (logLevel < LogLevel)
                {
                    var log = new RequestLog
                    {
                        Time = DateTime.UtcNow,
                        Level = logLevel,
                        Method = requestMethod.Method,
                        StatusCode = (int) responseStatusCode,
                        StatusMessage = response.ReasonPhrase,
                        Uri = requestUri.OriginalString,
                        RequestHeaders = requestHeaders,
                        ResponseHeaders = responseHeaders
                    };

                    using (var context = LoggingContext.Create())
                    {
                        context.Add(log);
                        await (context.SaveChangesAsync() ?? throw new InvalidOperationException());
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                throw;
            }
        }

    }

}

using System.Net;
using Intersect.Logging;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging;
using Intersect.Server.Web.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using LogLevel = Intersect.Logging.LogLevel;

namespace Intersect.Server.Web.Middleware;

public static class IntersectRequestLogging
{
    public static void UseIntersectRequestLogging(this WebApplication app, LogLevel logLevel = LogLevel.Info)
    {
        app.Use(
            async (httpContext, next) =>
            {
                try
                {
                    if (httpContext?.Request == null)
                    {
                        throw new ArgumentNullException(nameof(httpContext));
                    }

                    var request = httpContext.Request;
                    var requestMethod = new HttpMethod(request.Method.ToUpperInvariant());
                    var requestHeaders = request.Headers.ToDictionary(
                        pair => pair.Key,
                        pair => pair.Value.ToList()
                    );
                    var requestUri = new Uri(request.GetDisplayUrl());

                    var internalServerError = false;

                    try
                    {
                        await next.Invoke();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception);
                        internalServerError = true;
                    }

                    var response = httpContext.Response;
                    var responseStatusCode = internalServerError
                        ? HttpStatusCode.InternalServerError
                        : (HttpStatusCode)response.StatusCode;

                    var responseHeaders = new Dictionary<string, List<string>>();

                    // This is necessary because *apparently* the response headers
                    // dictionary does not combine duplicate header keys into a
                    // single entry's value array, and regular dictionaries
                    // most definitely do not appreciate that, nor will JSON.
                    foreach (var (key, value) in response.Headers)
                    {
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            continue;
                        }

                        var aggregateValue = new List<string>();
                        if (responseHeaders.TryGetValue(key, out var existingValue) && existingValue != null)
                        {
                            aggregateValue.AddRange(existingValue);
                        }

                        if (value.Count > 0)
                        {
                            aggregateValue.AddRange(value);
                        }

                        responseHeaders[key] = aggregateValue;
                    }

                    var logLevelForStatusCode = responseStatusCode.ToIntersectLogLevel();

                    // ReSharper disable once InvertIf
                    if (logLevelForStatusCode < logLevel)
                    {
                        var log = new RequestLog
                        {
                            Time = DateTime.UtcNow,
                            Level = logLevelForStatusCode,
                            Method = requestMethod.Method,
                            StatusCode = (int)responseStatusCode,
                            StatusMessage = ReasonPhrases.GetReasonPhrase((int)responseStatusCode),
                            Uri = requestUri.OriginalString,
                            RequestHeaders = requestHeaders,
                            ResponseHeaders = responseHeaders
                        };

                        await using var context = DbInterface.CreateLoggingContext();
                        context.Add(log);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception);

                    throw;
                }
            }
        );
    }
}
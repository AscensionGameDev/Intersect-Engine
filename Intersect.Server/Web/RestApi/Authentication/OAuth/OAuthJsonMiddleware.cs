using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Owin;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{
    public sealed class OAuthJsonMiddleware : OwinMiddleware
    {
        public JsonBodyToUrlEncodedBodyMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (string.Equals(context.Request.ContentType, "application/json")
                && string.Equals(context.Request.Method, "POST", StringComparison.InvariantCultureIgnoreCase)
                && (context.Request.Path == new PathString("/api/oauth/token") || context.Request.Path == new PathString("/api/oauth/token/")))
            {
                try
                {
                    await ReplaceJsonBodyWithUrlEncodedBody(context);
                    await Next.Invoke(context);
                }
                catch (Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Write("Invalid JSON format.");
                }
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private async Task ReplaceJsonBodyWithUrlEncodedBody(IOwinContext context)
        {
            var requestParams = await GetFormCollectionFromJsonBody(context);
            var urlEncodedParams = string.Join("&", requestParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            var decryptedContent = new StringContent(urlEncodedParams, Encoding.UTF8, "application/x-www-form-urlencoded");
            var requestStream = await decryptedContent.ReadAsStreamAsync();
            context.Request.Body = requestStream;
        }

        private static async Task<Dictionary<string, string>> GetFormCollectionFromJsonBody(IOwinContext context)
        {
            //context.Request.Body.Position = 0;
            var jsonString = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var requestParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            return requestParams;
        }
    }
}

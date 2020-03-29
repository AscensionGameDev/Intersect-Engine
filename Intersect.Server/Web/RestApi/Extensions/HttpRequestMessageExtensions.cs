using System.Net;
using System.Net.Http;

namespace Intersect.Server.Web.RestApi.Extensions
{

    internal static class HttpRequestMessageExtensions
    {

        public static HttpResponseMessage CreateMessageResponse(
            this HttpRequestMessage request,
            HttpStatusCode statusCode,
            string message
        )
        {
            return request.CreateResponse(statusCode, new {Message = message});
        }

    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace Intersect.Server.Web.RestApi.Middleware
{

    using RequestMap = IDictionary<(PathString, string, string), RequestMapFunc>;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="owinContext"></param>
    /// <returns></returns>
    public delegate Task RequestMapFunc(IOwinContext owinContext);

    /// <inheritdoc />
    public sealed class ContentTypeMappingMiddleware : OwinMiddleware
    {

        private readonly RequestMap mRequestMap;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        /// <param name="requestMap"></param>
        public ContentTypeMappingMiddleware(OwinMiddleware next, RequestMap requestMap) : base(next)
        {
            mRequestMap = requestMap ?? new Dictionary<(PathString, string, string), RequestMapFunc>();
        }

        /// <inheritdoc />
        public override async Task Invoke(IOwinContext owinContext)
        {
            var request = owinContext.Request;
            Debug.Assert(request != null);

            var path = request.Path;
            var method = request.Method;
            var contentType = request.ContentType;

            if (mRequestMap.TryGetValue((path, method, contentType), out var handler))
            {
                await (handler(owinContext) ?? throw new InvalidOperationException(@"Task is null"));
            }

            if (Next == null)
            {
                return;
            }

            await (Next.Invoke(owinContext) ?? throw new InvalidOperationException(@"Task is null"));

            owinContext.ToString();
        }

    }

}

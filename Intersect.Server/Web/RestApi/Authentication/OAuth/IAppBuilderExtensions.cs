using System.Collections.Generic;

using Intersect.Server.Web.RestApi.Middleware;

using Microsoft.Owin;

using Owin;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public static class IAppBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="requestMap"></param>
        /// <returns></returns>
        public static IAppBuilder UseContentTypeMappingMiddleware(
            this IAppBuilder appBuilder,
            IDictionary<(PathString, string, string), RequestMapFunc> requestMap
        )
        {
            return appBuilder.Use<ContentTypeMappingMiddleware>(requestMap);
        }

    }

}

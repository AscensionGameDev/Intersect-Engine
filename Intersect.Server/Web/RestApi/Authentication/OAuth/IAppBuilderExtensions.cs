using System.Collections.Generic;

using JetBrains.Annotations;

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
            [NotNull] this IAppBuilder appBuilder,
            [NotNull] IDictionary<(PathString, string, string), RequestMapFunc> requestMap
        ) => appBuilder.Use<ContentTypeMappingMiddleware>(requestMap);

    }

}
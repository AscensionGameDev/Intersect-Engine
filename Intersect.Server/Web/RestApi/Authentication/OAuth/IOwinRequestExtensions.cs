using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using JetBrains.Annotations;

using Microsoft.Owin;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public static class IOwinRequestExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owinRequest"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task<Dictionary<string, string>> JsonBodyToMap([NotNull] this IOwinRequest owinRequest)
        {
            Debug.Assert(owinRequest.Body != null);
            using (var streamReader = new StreamReader(owinRequest.Body))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    await (streamReader.ReadToEndAsync() ?? throw new InvalidOperationException(@"Task is null"))
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owinRequest"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task<IEnumerable<string>> JsonBodyToEncodedParameterStrings(
            [NotNull] this IOwinRequest owinRequest
        )
        {
            return (await owinRequest.JsonBodyToMap()).Select(
                parameter =>
                    $"{HttpUtility.UrlEncode((string) parameter.Key)}={HttpUtility.UrlEncode((string) parameter.Value)}"
            );
        }

    }

}

using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public static class IOwinContextExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owinContext"></param>
        /// <returns></returns>
        public static async Task ConvertFromJsonToFormBody(this IOwinContext owinContext)
        {
            Debug.Assert(owinContext.Request != null);
            var request = owinContext.Request;

            var formParameters = string.Join("&", await request.JsonBodyToEncodedParameterStrings());
            var formParametersBody = new StringContent(
                formParameters, Encoding.UTF8, "application/x-www-form-urlencoded"
            );

            request.Body = await formParametersBody.ReadAsStreamAsync();
        }

    }

}

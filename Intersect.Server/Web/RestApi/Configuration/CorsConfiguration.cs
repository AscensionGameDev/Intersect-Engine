using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Cors;

using Intersect.Serialization.Json;

using Microsoft.Owin.Cors;

using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Configuration
{

    public struct CorsConfiguration
    {

        public string Origin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Headers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Methods { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> ExposedHeaders { get; set; }

    }

    public static class CorsConfigurationExtensions
    {

        public static CorsOptions AsCorsOptions(this CorsConfiguration corsConfiguration)
        {
            return new CorsOptions {PolicyProvider = corsConfiguration.AsOwinPolicyProvider()};
        }

        public static ICorsPolicyProvider AsOwinPolicyProvider(this CorsConfiguration corsConfiguration)
        {
            var attribute = new CorsPolicyProvider
            {
                PolicyResolver = context => Task.FromResult(
                    CreatePolicy(
                        corsConfiguration.Origin, string.Join(",", corsConfiguration.Methods ?? new List<string>()),
                        string.Join(",", corsConfiguration.Headers ?? new List<string>()),
                        string.Join(",", corsConfiguration.ExposedHeaders ?? new List<string>())
                    )
                )
            };

            return attribute;
        }

        private static CorsPolicy CreatePolicy(string origins, string headers, string methods, string exposedHeaders)
        {
            if (string.IsNullOrEmpty(origins))
            {
                throw new ArgumentNullException(nameof(origins));
            }

            var policy = new CorsPolicy();
            if (origins == "*")
            {
                policy.AllowAnyOrigin = true;
            }
            else
            {
                AddCommaSeparatedValuesToCollection(origins, policy.Origins);
            }

            if (!string.IsNullOrEmpty(headers))
            {
                if (headers == "*")
                {
                    policy.AllowAnyHeader = true;
                }
                else
                {
                    AddCommaSeparatedValuesToCollection(headers, policy.Headers);
                }
            }

            if (!string.IsNullOrEmpty(methods))
            {
                if (methods == "*")
                {
                    policy.AllowAnyMethod = true;
                }
                else
                {
                    AddCommaSeparatedValuesToCollection(methods, policy.Methods);
                }
            }

            if (!string.IsNullOrEmpty(exposedHeaders))
            {
                AddCommaSeparatedValuesToCollection(exposedHeaders, policy.ExposedHeaders);
            }

            return policy;
        }

        private static void AddCommaSeparatedValuesToCollection(
            string commaSeparatedValues,
            ICollection<string> collection
        )
        {
            commaSeparatedValues?.Split(',')
                .Where(part => !string.IsNullOrWhiteSpace(part?.Trim()))
                .ToList()
                .ForEach(collection.Add);
        }

    }

}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;

using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Server.Web.RestApi.Authentication.OAuth;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using WebApiThrottle;

namespace Intersect.Server.Web.RestApi.Configuration
{

    /// <inheritdoc />
    /// <summary>
    /// Configuration options for <see cref="T:Intersect.Server.Web.RestApi.RestApi" />.
    /// </summary>
    public sealed class ApiConfiguration : IConfiguration<ApiConfiguration>
    {

        #region Constants

        public const string DefaultPath = @"resources/config/api.config.json";

#if DEBUG
        public const uint DefaultRefreshTokenLifetime = 15;
#else
        public const uint DefaultRefreshTokenLifetime = 10080;
#endif

        public static readonly ThrottlePolicy DefaultThrottlePolicy = new ThrottlePolicy
        {
            ClientRules =
            {
                {
                    IntersectThrottlingHandler.DefaultFallbackClientKey, new RateLimits
                    {
                        PerSecond = 1,
                        PerMinute = 10
                    }
                }
            },
            ClientThrottling = true,
            EndpointRules =
            {
                {
                    OAuthProvider.TokenEndpoint, new RateLimits
                    {
                        PerSecond = 1,
                        PerMinute = 5
                    }
                }
            },
            EndpointThrottling = true,
            IpThrottling = true,
            StackBlockedRequests = true,
            Rates =
            {
                {RateLimitPeriod.Second, 1},
                {RateLimitPeriod.Minute, 60}
            }
        };

        public const LogLevel DefaultRequestLogLevel = LogLevel.Trace;

        #endregion

        #region Configuration fields

        [JsonIgnore] private ImmutableArray<string> mHosts;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(new[] {"http://localhost:5400"})]
        public ImmutableArray<string> Hosts
        {
            get => mHosts;
            set => mHosts = value.IsDefaultOrEmpty ? ImmutableArray.Create(new[] {"http://localhost:5400"}) : value;
        }

        [JsonIgnore]
        public ImmutableArray<int> Ports
        {
            get => Hosts.Select(host => new Uri(host?.Replace("*", "localhost") ?? "http://localhost:5400").Port)
                .ToImmutableArray();
            set => Hosts = ImmutableArray.Create(value.Select(port => $@"http://localhost:{port}")?.ToArray());
        }

        [JsonIgnore] private ImmutableArray<CorsConfiguration> mCorsConfigurations;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ImmutableArray<CorsConfiguration> Cors
        {
            get => mCorsConfigurations;
            set => mCorsConfigurations = value.IsDefaultOrEmpty ? ImmutableArray.Create<CorsConfiguration>() : value;
        }

        [JsonProperty(nameof(RouteAuthorization), NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> mRouteAuthorization;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DataProtectionKey { get; private set; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, object> RouteAuthorization =>
            new ReadOnlyDictionary<string, object>(mRouteAuthorization);

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; private set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DebugMode { get; private set; } = false;

#if DEBUG
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SeedMode { get; private set; }
#endif

        [JsonProperty(
            NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include
        )]
        [DefaultValue(DefaultRefreshTokenLifetime)]
        public uint RefreshTokenLifetime { get; private set; } = DefaultRefreshTokenLifetime;

        [JsonProperty(
            NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include
        )]
        public ThrottlePolicy ThrottlePolicy { get; private set; } = DefaultThrottlePolicy;

        [JsonProperty(
            NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include
        )]
        [DefaultValue(IntersectThrottlingHandler.DefaultFallbackClientKey)]
        public string FallbackClientKey { get; private set; } = IntersectThrottlingHandler.DefaultFallbackClientKey;

        [JsonProperty(
            NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include,
            ItemConverterType = typeof(StringEnumConverter)
        )]
        [DefaultValue(DefaultRequestLogLevel)]
        public LogLevel RequestLogLevel { get; set; } = DefaultRequestLogLevel;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(false)]
        public bool RequestLogging { get; set; }

        #endregion

        #region Initialization

        public ApiConfiguration()
        {
            mRouteAuthorization = new Dictionary<string, object>();

            using (var csp = new AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.GenerateKey();
                DataProtectionKey = BitConverter.ToString(csp.Key).Replace("-", "");
            }

            Hosts = ImmutableArray.Create(new[] {"http://localhost:5400"});
            Cors = ImmutableArray.Create<CorsConfiguration>();
        }

        #endregion

        #region I/O

        /// <inheritdoc />
        public ApiConfiguration Load(string filePath = DefaultPath, bool failQuietly = false)
        {
            return ConfigurationHelper.Load(this, filePath, failQuietly);
        }

        /// <inheritdoc />
        public ApiConfiguration Save(string filePath = DefaultPath, bool failQuietly = false)
        {
            return ConfigurationHelper.Save(this, filePath, failQuietly);
        }

        #endregion

        public static ApiConfiguration Create(string filePath = DefaultPath)
        {
            var configuration = new ApiConfiguration();

            ConfigurationHelper.LoadSafely(configuration, filePath);

            return configuration;
        }

    }

}

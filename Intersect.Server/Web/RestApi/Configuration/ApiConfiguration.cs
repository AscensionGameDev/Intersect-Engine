using Intersect.Configuration;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;

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

        #endregion

        #region Configuration fields

        [JsonIgnore] private ImmutableArray<string> mHosts;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(new[] {"http://localhost:5401"})]
        public ImmutableArray<string> Hosts
        {
            get => mHosts;
            set => mHosts = value.IsDefaultOrEmpty ? ImmutableArray.Create(new[] {"http://localhost:5401"}) : value;
        }

        [JsonIgnore]
        public ImmutableArray<int> Ports
        {
            get => Hosts.Select(host => new Uri(host ?? "http://localhost:5401").Port).ToImmutableArray();
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
        [NotNull] public string DataProtectionKey { get; private set; }

        [JsonIgnore]
        [NotNull]
        public IReadOnlyDictionary<string, object> RouteAuthorization =>
            new ReadOnlyDictionary<string, object>(mRouteAuthorization);

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DebugMode { get; private set; }

#if DEBUG
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SeedMode { get; private set; }
#endif

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue(DefaultRefreshTokenLifetime)]
        public uint RefreshTokenLifetime { get; private set; }

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

            Enabled = false;
            DebugMode = false;
            Hosts = ImmutableArray.Create(new[] { "http://localhost:5401" });
            Cors = ImmutableArray.Create<CorsConfiguration>();
            RefreshTokenLifetime = DefaultRefreshTokenLifetime;
        }

        #endregion

        #region I/O

        /// <inheritdoc />
        public ApiConfiguration Load(string filePath = DefaultPath, bool failQuietly = false) =>
            ConfigurationHelper.Load(this, filePath, failQuietly);

        /// <inheritdoc />
        public ApiConfiguration Save(string filePath = DefaultPath, bool failQuietly = false) =>
            ConfigurationHelper.Save(this, filePath, failQuietly);

        #endregion

        [NotNull]
        public static ApiConfiguration Create([CanBeNull] string filePath = DefaultPath)
        {
            var configuration = new ApiConfiguration();

            ConfigurationHelper.LoadSafely(configuration, filePath);

            return configuration;
        }

    }

}

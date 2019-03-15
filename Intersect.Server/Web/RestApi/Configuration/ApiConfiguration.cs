using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Intersect.IO.FileSystem;
using Intersect.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Server.Web.RestApi.Configuration
{

    public sealed class ApiConfiguration
    {

#if DEBUG
        public const uint DefaultRefreshTokenLifetime = 15;
#else
        public const uint DefaultRefreshTokenLifetime = 10080;
#endif

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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue(DefaultRefreshTokenLifetime)]
        public uint RefreshTokenLifetime { get; private set; }


        private ApiConfiguration()
        {
            mRouteAuthorization = new Dictionary<string, object>();

            using (var csp = new System.Security.Cryptography.AesCryptoServiceProvider())
            {
                csp.KeySize = 256;
                csp.GenerateKey();
                DataProtectionKey = BitConverter.ToString(csp.Key).Replace("-", "");
            }

            Enabled = false;
            DebugMode = false;
            Hosts = ImmutableArray.Create(new[] { "http://localhost:5401" });
            RefreshTokenLifetime = DefaultRefreshTokenLifetime;
        }

#region Static

        public const string DefaultPath = @"resources/config/api.config.json";

        public static ApiConfiguration Load([NotNull] string filePath = DefaultPath)
        {
            if (!File.Exists(filePath))
            {
                return new ApiConfiguration();
            }

            try
            {
                var json = File.ReadAllText(filePath, Encoding.UTF8);

                return JsonConvert.DeserializeObject<ApiConfiguration>(json);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

            return null;
        }

        public static bool Save([NotNull] ApiConfiguration apiConfiguration, [NotNull] string filePath = DefaultPath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath == null)
            {
                return false;
            }

            if (!FileSystemHelper.EnsureDirectoryExists(directoryPath))
            {
                return false;
            }

            try
            {
                var json = JsonConvert.SerializeObject(apiConfiguration, Formatting.Indented);
                File.WriteAllText(filePath, json, Encoding.UTF8);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return false;
            }
        }

#endregion

    }

}

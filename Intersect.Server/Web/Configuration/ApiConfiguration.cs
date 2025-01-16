using System.Collections.Immutable;
using System.ComponentModel;
using Intersect.Framework.Net;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using LogLevel = Intersect.Logging.LogLevel;

namespace Intersect.Server.Web.Configuration;

/// <summary>
/// Configuration options for <see cref="T:Intersect.Server.Web.RestApi.RestApi" />.
/// </summary>
public sealed partial class ApiConfiguration
{
    #region Constants

    public const LogLevel DefaultRequestLogLevel = LogLevel.Trace;

    #endregion

    #region Configuration fields

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool Enabled { get; set; }

#if DEBUG
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool EnableAuthenticationDebugging { get; set; }
#endif

    [JsonIgnore] private ImmutableDictionary<string, CorsPolicy>? _corsPolicies;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ImmutableDictionary<string, CorsPolicy>? Cors
    {
        get => _corsPolicies ??= ImmutableDictionary.Create<string, CorsPolicy>();
        set => _corsPolicies = value;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public NetworkTypes AllowedNetworkTypes { get; set; } = NetworkTypes.Loopback;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? AllowedNonHttpOnlyCookies { get; set; }

    [JsonProperty(
        NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Include,
        ItemConverterType = typeof(StringEnumConverter)
    )]
    [DefaultValue(DefaultRequestLogLevel)]
    public LogLevel RequestLogLevel { get; set; } = DefaultRequestLogLevel;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(false)]
    public bool RequestLogging { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TokenGenerationOptions? TokenGenerationOptions { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? KnownProxies { get; set; } = new();

    [JsonProperty(
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Include,
        ObjectCreationHandling = ObjectCreationHandling.Replace
    )]
    public List<StaticFilePathOptions> StaticFilePaths { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [DefaultValue(false)]
    public bool EnableSwaggerUI { get; set; }

    #endregion
}
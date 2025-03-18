using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Intersect;

public sealed class OptionsContractResolver(bool serializePrivateProperties, bool serializePublicProperties) : DefaultContractResolver
{
    private readonly bool _serializePrivateProperties = serializePrivateProperties;
    private readonly bool _serializePublicProperties = serializePublicProperties;

    private static readonly HashSet<PropertyInfo> PrivateProperties =
    [
        typeof(Options).GetProperty(nameof(Options.AdminOnly)),
        typeof(Options).GetProperty(nameof(Options.EventWatchdogKillThreshold)),
        typeof(Options).GetProperty(nameof(Options.GameDatabase)),
        typeof(Options).GetProperty(nameof(Options.Logging)),
        typeof(Options).GetProperty(nameof(Options.LoggingDatabase)),
        typeof(Options).GetProperty(nameof(Options.MaxClientConnections)),
        typeof(Options).GetProperty(nameof(Options.MaximumLoggedInUsers)),
        typeof(Options).GetProperty(nameof(Options.Metrics)),
        typeof(Options).GetProperty(nameof(Options.OpenPortChecker)),
        typeof(Options).GetProperty(nameof(Options.PlayerDatabase)),
        typeof(Options).GetProperty(nameof(Options.PortCheckerUrl)),
        typeof(Options).GetProperty(nameof(Options.Security)),
        typeof(Options).GetProperty(nameof(Options.ServerPort)),
        typeof(Options).GetProperty(nameof(Options.SmtpSettings)),
        typeof(Options).GetProperty(nameof(Options.UPnP)),
        typeof(Options).GetProperty(nameof(Options.ValidPasswordResetTimeMinutes)),
    ];

    private static readonly HashSet<PropertyInfo> PublicProperties =
    [
        typeof(Options).GetProperty(nameof(Options.SmtpValid)),
    ];

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (PrivateProperties.Contains(member))
        {
            property.ShouldDeserialize = AlwaysSerialize;
            property.ShouldSerialize = ShouldSerializePrivateProperty;
            property.Writable = true;
        }

        if (PublicProperties.Contains(member))
        {
            property.ShouldDeserialize = AlwaysSerialize;
            property.ShouldSerialize = ShouldSerializePublicProperty;
            property.Writable = true;
        }

        return property;
    }

    private static bool AlwaysSerialize(object _) => true;

    private bool ShouldSerializePublicProperty(object _) => _serializePublicProperties;

    private bool ShouldSerializePrivateProperty(object _) => _serializePrivateProperties;
}
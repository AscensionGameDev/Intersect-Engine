using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Server.Web.Configuration;

public record PartialKestrelEndpointCertificate(
    string? Path,
    string? KeyPath,
    [property: JsonConverter(typeof(StringEnumConverter))]
    PartialKestrelEndpointCertificateType SelfSignedCertificateType
);
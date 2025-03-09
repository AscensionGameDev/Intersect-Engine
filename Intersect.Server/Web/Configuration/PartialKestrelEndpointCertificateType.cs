using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Server.Web.Configuration;

[JsonConverter(typeof(StringEnumConverter))]
public enum PartialKestrelEndpointCertificateType
{
    None,
    ECDSA,
    RSA,
}
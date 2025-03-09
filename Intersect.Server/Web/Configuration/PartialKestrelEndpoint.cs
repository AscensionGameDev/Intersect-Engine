namespace Intersect.Server.Web.Configuration;

public sealed record PartialKestrelEndpoint(string Url, PartialKestrelEndpointCertificate? Certificate);
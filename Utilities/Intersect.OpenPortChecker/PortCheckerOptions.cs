namespace Intersect.OpenPortChecker;

public sealed class PortCheckerOptions
{
    public bool EnableIPv6 { get; set; } = true;

    public List<string>? KnownProxies { get; set; }
}
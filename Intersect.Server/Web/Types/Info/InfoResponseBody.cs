namespace Intersect.Server.Web.Types.Info;

public readonly struct InfoResponseBody(string name, int port)
{
    public string Name { get; } = name;

    public int Port { get; } = port;
}

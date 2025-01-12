namespace Intersect.Server.Web.RestApi.Types.InfoResponseBody;

public readonly struct InfoResponseBody(string name, int port)
{
    public string Name { get; } = name;

    public int Port { get; } = port;
}

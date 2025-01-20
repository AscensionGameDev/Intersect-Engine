namespace Intersect.Core;

public static class ApplicationContext
{
    public static readonly AsyncLocal<IApplicationContext?> Context = new();
}
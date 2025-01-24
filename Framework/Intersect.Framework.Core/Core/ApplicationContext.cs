namespace Intersect.Core;

public static class ApplicationContext
{
    public static readonly AsyncLocal<IApplicationContext?> Context = new();

    public static IApplicationContext CurrentContext => Context.Value ??
                                                        throw new InvalidOperationException(
                                                            "No context has been created for this call stack."
                                                        );
}
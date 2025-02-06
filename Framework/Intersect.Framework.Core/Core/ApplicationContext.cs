namespace Intersect.Core;

public static class ApplicationContext
{
    public static readonly AsyncLocal<IApplicationContext?> Context = new();

    public static IApplicationContext CurrentContext => Context.Value ??
                                                        throw new InvalidOperationException(
                                                            "No context has been created for this call stack."
                                                        );

    public static TContext? GetContext<TContext>() where TContext : class, IApplicationContext
    {
        return Context.Value as TContext;
    }

    public static TContext GetCurrentContext<TContext>() where TContext : class, IApplicationContext
    {
        return GetContext<TContext>() ??
               throw new InvalidOperationException("No context has been created for this call stack.");
    }
}
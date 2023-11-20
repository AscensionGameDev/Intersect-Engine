namespace Intersect.Framework.Services.ExceptionHandling;

public interface IExceptionHandlingService
{
    event EventHandler<UnhandledExceptionEventArgs>? UnhandledException;

    event EventHandler<UnobservedTaskExceptionEventArgs>? UnobservedTaskException;

    void DispatchUnhandledException(Exception exception, bool isTerminating);
}
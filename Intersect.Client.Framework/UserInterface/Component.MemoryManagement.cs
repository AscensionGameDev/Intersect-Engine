namespace Intersect.Client.Framework.UserInterface;

public partial class Component : IDisposable
{
    private bool _disposed;

    ~Component()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(Name ?? (GetType() ?? typeof(Component)).FullName);
        }

        foreach (var child in Children)
        {
            child.Dispose(disposing: disposing);
        }

        if (disposing)
        {
        }

        _disposed = true;
    }
}

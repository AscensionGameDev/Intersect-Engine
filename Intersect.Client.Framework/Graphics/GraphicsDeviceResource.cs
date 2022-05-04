namespace Intersect.Client.Framework.Graphics;

public abstract class GraphicsDeviceResource : IDisposable
{
    private GraphicsDevice? _device;
    private bool _disposed;
    private WeakReference? _selfReference;

    protected GraphicsDeviceResource(GraphicsDevice? graphicsDevice, string? name = default)
    {
        InternalSetGraphicsDevice(graphicsDevice, constructor: true);
        Name = name ?? GetType().Name;
    }

    ~GraphicsDeviceResource()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public GraphicsDevice? Device
    {
        get => _device;
        internal set => InternalSetGraphicsDevice(value, constructor: false);
    }

    public event EventHandler<EventArgs> Disposing;

    public bool IsDisposed => _disposed;

    public string Name { get; set; }

    protected string ResourceTypeName => GetType().FullName!;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"{nameof(ResourceTypeName)}[{Name ?? _selfReference?.GetHashCode().ToString()}]");
        }

        if (disposing)
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        if (_selfReference != default)
        {
            _device?.RemoveResource(_selfReference);
        }

        // Free unmanaged
        _selfReference = default;
        _device = default;
        _disposed = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void InternalSetGraphicsDevice(GraphicsDevice graphicsDevice, bool constructor)
    {
        if (!constructor)
        {
            if (graphicsDevice == default)
            {
                throw new ArgumentNullException(nameof(graphicsDevice));
            }
        }

        if (_device == graphicsDevice)
        {
            return;
        }

        _device?.RemoveResource(_selfReference ?? throw new InvalidOperationException());
        _selfReference = default;

        _device = graphicsDevice;

        _selfReference = new(graphicsDevice);
        _device.AddResource(_selfReference);
    }
}

namespace Intersect.Client.Framework.Content;

public interface IAsset
{
    public event Action<IAsset>? Disposed;

    public event Action<IAsset>? Loaded;

    public event Action<IAsset>? Unloaded;

    public bool IsDisposed { get; }

    public bool IsLoaded { get; }

    string Id { get; }

    public string Name { get; }
}

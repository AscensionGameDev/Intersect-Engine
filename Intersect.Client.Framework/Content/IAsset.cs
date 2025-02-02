namespace Intersect.Client.Framework.Content;

public interface IAsset
{
    event Action<IAsset>? Disposed;

    event Action<IAsset>? Loaded;

    event Action<IAsset>? Unloaded;

    bool IsDisposed { get; }

    bool IsLoaded { get; }

    string Id { get; }

    string? Name { get; }
}

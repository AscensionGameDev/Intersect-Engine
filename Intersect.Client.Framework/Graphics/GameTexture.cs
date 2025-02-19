using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Compression;
using Intersect.Core;
using Intersect.Framework.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Graphics;

public static class GameTexture
{
    public static IGameTexture? GetBoundingTexture(BoundsComparison boundsComparison, params IGameTexture[] textures)
    {
        IGameTexture? boundingTexture = default;

        foreach (var texture in textures)
        {
            if (boundingTexture == default)
            {
                boundingTexture = texture;
                continue;
            }

            var select = false;
            switch (boundsComparison)
            {
                case BoundsComparison.Width:
                    select = texture.Width > boundingTexture.Width;
                    break;

                case BoundsComparison.Height:
                    select = texture.Height > boundingTexture.Height;
                    break;

                case BoundsComparison.Dimensions:
                    select = texture.Width >= boundingTexture.Width && texture.Height >= boundingTexture.Height;
                    break;

                case BoundsComparison.Area:
                    select = texture.Area > boundingTexture.Area;
                    break;

                default:
                    ApplicationContext.Context.Value?.Logger.LogError(
                        new ArgumentOutOfRangeException(nameof(boundsComparison), boundsComparison.ToString()),
                        "Failed to get bounding texture"
                    );
                    break;
            }

            if (select)
            {
                boundingTexture = texture;
            }
        }

        return boundingTexture;
    }
}

public abstract partial class GameTexture<TPlatformTexture, TPlatformRenderer> : IGameTexture
    where TPlatformTexture : class, IDisposable where TPlatformRenderer : GameRenderer
{
    // ReSharper disable once StaticMemberInGenericType
    private static ulong _nextId;

    protected readonly ulong Id = ++_nextId;

    protected readonly TPlatformRenderer Renderer;

    private bool _disposed;
    private Func<Stream>? _streamFactory;
    private TPlatformTexture? _platformTexture;

    private GameTexture(TPlatformRenderer renderer, string? name, bool pinned)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        Renderer = renderer;
        Name = name ?? $"{GetType().GetName(qualified: true)}#{Id}";

        IsPinned = pinned;
        if (pinned)
        {
            AccessTime = long.MaxValue;
        }

        Renderer.MarkConstructed(this);
    }

    protected GameTexture(
        TPlatformRenderer renderer,
        string? name,
        TPlatformTexture platformTexture
    ) : this(renderer: renderer, name: name, pinned: true)
    {
        ArgumentNullException.ThrowIfNull(platformTexture);

        _platformTexture = platformTexture;

        Renderer.MarkAllocated(this);
    }

    protected GameTexture(
        TPlatformRenderer renderer,
        string name,
        Func<Stream> streamFactory,
        bool pinned = false
    ) : this(renderer, name, pinned)
    {
        ArgumentNullException.ThrowIfNull(streamFactory);

        _streamFactory = streamFactory;
    }

    protected GameTexture(
        TPlatformRenderer renderer,
        string name,
        AtlasReference atlasReference
    ) : this(renderer, name, pinned: true)
    {
        ArgumentNullException.ThrowIfNull(atlasReference);

        AtlasReference = atlasReference;
    }

    protected TPlatformTexture? PlatformTexture
    {
        get
        {
            if (AtlasReference is not null)
            {
                return AtlasReference.Texture.GetTexture<TPlatformTexture>();
            }

            if (_platformTexture == null)
            {
                LoadPlatformTexture();
            }
            else
            {
                UpdateAccessTime();
            }

            return _platformTexture;
        }
        set
        {
            if (_platformTexture is not null && value is not null)
            {
                throw new InvalidOperationException();
            }

            _platformTexture = value;
        }
    }

    public event Action<IAsset>? Disposed;
    public event Action<IAsset>? Loaded;
    public event Action<IAsset>? Unloaded;

    public Color this[int x, int y] => GetPixel(x, y);

    public Color this[Point point] => GetPixel(point.X, point.Y);

    public long AccessTime { get; private set; }

    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public bool IsDisposed => _disposed;

    public bool IsLoaded => !_disposed && _platformTexture != null;

    public bool IsMissingOrCorrupt { get; private set; }

    public bool IsPinned { get; }

    string IAsset.Id => Name;

    public string Name { get; }

    public int Area => Width * Height;

    public FloatRect Bounds => new(0, 0, Width, Height);

    public Pointf Dimensions => new(Width, Height);

    public Pointf Center => Dimensions / 2;

    public AtlasReference? AtlasReference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public abstract int Width { get; }

    public abstract int Height { get; }

    public int CompareTo(IGameTexture? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is not GameTexture<TPlatformTexture, TPlatformRenderer> otherGameTexture)
        {
            return -1;
        }

        var accessTimeComparison = (int)Math.Clamp(AccessTime - other.AccessTime, int.MinValue, int.MaxValue);
        if (accessTimeComparison != 0)
        {
            return accessTimeComparison;
        }

        if (Id != otherGameTexture.Id)
        {
            return Id < otherGameTexture.Id ? -1 : 1;
        }

        ApplicationContext.Context.Value?.Logger.LogError(
            "Texture '{NameA}' and '{NameB}' both have the same Id '{Id}' which is unexpected",
            Name,
            other.Name,
            Id
        );
        return 0;
    }

    protected abstract TPlatformTexture? CreatePlatformTextureFromStream(Stream stream);

    private void LoadPlatformTexture(bool force = false)
    {
        if (!force)
        {
            if (_platformTexture != null)
            {
                return;
            }

            if (IsMissingOrCorrupt)
            {
                return;
            }
        }

        if (AtlasReference != null)
        {
            if (AtlasReference.Texture is not GameTexture<TPlatformTexture, TPlatformRenderer> atlasTexture)
            {
                IsMissingOrCorrupt = true;
                throw new InvalidOperationException();
            }

            atlasTexture.LoadPlatformTexture(force);
            return;
        }

        if (_streamFactory == null)
        {
            IsMissingOrCorrupt = true;
            throw new InvalidOperationException();
        }

        try
        {
            _platformTexture = null;

            using var stream = _streamFactory();

            if (Name.EndsWith(".asset"))
            {
                using var gzipStream = GzipCompression.CreateDecompressedFileStream(stream);
                _platformTexture = CreatePlatformTextureFromStream(gzipStream);
            }
            else
            {
                _platformTexture = CreatePlatformTextureFromStream(stream);
            }

            Renderer.MarkAllocated(this);
            UpdateAccessTime();
            EmitLoaded();
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Exception thrown while trying to load '{TextureName}'",
                Name
            );
        }
        finally
        {
            IsMissingOrCorrupt = _platformTexture == null;
        }
    }

    public bool Unload()
    {
        if (AccessTime == long.MaxValue)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Tried to unload pinned texture {TextureId} ({TextureName})",
                Id,
                Name
            );
            return false;
        }

        _platformTexture?.Dispose();
        _platformTexture = null;

        AccessTime = 0;
        EmitUnloaded();
        OnUnload();
        return true;
    }

    public object? GetTexture() => PlatformTexture;

    public void Reload() => InternalReload();

    private protected virtual void InternalReload() => LoadPlatformTexture(force: true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRequestedPlatformTexture? GetTexture<TRequestedPlatformTexture>() where TRequestedPlatformTexture : class
    {
        if (typeof(TRequestedPlatformTexture) != typeof(TPlatformTexture))
        {
            return null;
        }

        return GetTexture() as TRequestedPlatformTexture;
    }

    public abstract Color GetPixel(int x, int y);

    public override string ToString()
    {
        return $"{GetType().Name} ({Name})";
    }

    public void Dispose()
    {
        Dispose(true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void UpdateAccessTime()
    {
        if (AccessTime == long.MaxValue)
        {
            return;
        }

        AccessTime = Timing.Global.MillisecondsUtc;
        Renderer.UpdateAccessTime(this);
    }

    protected virtual void OnUnload() { }

    private void EmitDisposed()
    {
        try
        {
            Disposed?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Disposed)
            );
        }
    }

    protected void EmitLoaded()
    {
        try
        {
            Loaded?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Loaded)
            );
        }
    }

    private void EmitUnloaded()
    {
        try
        {
            Unloaded?.Invoke(this);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown in event handlers registered for {EventName}()",
                nameof(Unloaded)
            );
        }
    }

    ~GameTexture()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;

        try
        {
            _platformTexture?.Dispose();
            _platformTexture = null;

            Renderer.MarkDisposed(this);
        }
        catch
        {
            throw;
        }

        EmitDisposed();
        OnDispose(true);
    }

    protected virtual void OnDispose(bool disposing)
    {
    }
}
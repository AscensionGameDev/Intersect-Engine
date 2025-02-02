using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Graphics;


public abstract partial class GameTexture : IAsset, IDisposable
{
    private static ulong _nextId;

    private readonly ulong _id = ++_nextId;

    private bool _disposed;

    public event Action<IAsset>? Disposed;
    public event Action<IAsset>? Loaded;
    public event Action<IAsset>? Unloaded;

    protected GameTexture(string name)
    {
        Name = name;
    }

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

    protected void EmitUnloaded()
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

    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public bool IsDisposed => _disposed;

    public virtual bool IsLoaded => !_disposed;

    public string Id => Name ?? _id.ToString();

    public string? Name { get; }

    public int Area => Width * Height;

    public Pointf Dimensions => new(Width, Height);

    public FloatRect Bounds => new(0, 0, Width, Height);

    public Pointf Center => Dimensions / 2;

    public object? PlatformTextureObject
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetTexture();
    }

    public GameTexturePackFrame? TexturePackFrame
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetTexturePackFrame();
    }

    public abstract int Width { get; }

    public abstract int Height { get; }

    public abstract object? GetTexture();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TPlatformTexture? GetTexture<TPlatformTexture>() where TPlatformTexture : class =>
        GetTexture() as TPlatformTexture;

    public abstract Color GetPixel(int x1, int y1);

    public abstract GameTexturePackFrame? GetTexturePackFrame();

    public static GameTexture? GetBoundingTexture(BoundsComparison boundsComparison, params GameTexture[] textures)
    {
        GameTexture? boundingTexture = default;

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

    public override string ToString() => $"{GetType().Name} ({Name})";

    public virtual void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;

        EmitDisposed();
    }
}

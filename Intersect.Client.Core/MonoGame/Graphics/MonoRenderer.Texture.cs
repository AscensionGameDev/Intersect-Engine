using System.Collections.Concurrent;
using Intersect.Client.Framework.Graphics;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer
{
    private readonly ConcurrentDictionary<Texture2D, IGameTexture> _allocatedTextures = [];

    private Texture2D? CreatePlatformTextureFromStream(MonoTexture gameTexture, Stream stream)
    {
        var platformTexture = Texture2D.FromStream(_graphicsDevice, stream);
        if (platformTexture is null)
        {
            return platformTexture;
        }

        platformTexture.Disposing += Texture2DOnDisposing;
        if (!_allocatedTextures.TryAdd(platformTexture, gameTexture))
        {
            throw new InvalidOperationException("Failed to record allocated texture");
        }

        return platformTexture;
    }

    private void Texture2DOnDisposing(object? sender, EventArgs args)
    {
        if (sender is Texture2D platformTexture)
        {
            OnPlatformTextureDisposal(platformTexture);
            return;
        }

        ApplicationContext.CurrentContext.Logger.LogError(
            "Received disposal event but it was not from an instance of {ExpectedType} ({ActualType})",
            typeof(Texture2D).GetName(qualified: true),
            sender?.GetType().GetName(qualified: true) ?? "null"
        );
    }

    private IGameTexture CreateGameTextureFromPlatformTexture(string assetName, Texture2D platformTexture)
    {
        var gameTexture = new MonoTexture(this, assetName, platformTexture);
        return gameTexture;
    }

    protected override IGameTexture CreateGameTextureFromAtlasReference(string assetName, AtlasReference atlasReference)
    {
        var gameTexture = new MonoTexture(this, assetName, atlasReference);
        return gameTexture;
    }

    public override IGameTexture CreateTextureFromStreamFactory(string assetName, Func<Stream> streamFactory)
    {
        var gameTexture = new MonoTexture(this, assetName, streamFactory);
        return gameTexture;
    }

    private void OnPlatformTextureDisposal(Texture2D platformTexture)
    {
        if (_allocatedTextures.TryRemove(platformTexture, out var gameTexture))
        {
            MarkFreed(gameTexture);
        }
    }

    private class MonoTexture : GameTexture<Texture2D, MonoRenderer>
    {
        internal MonoTexture(
            MonoRenderer renderer,
            string? name,
            Texture2D platformTexture
        ) : base(renderer: renderer, name: name, platformTexture: platformTexture)
        {
        }

        internal MonoTexture(
            MonoRenderer renderer,
            string name,
            Func<Stream> streamFactory,
            bool pinned = false
        ) : base(renderer: renderer, name: name, streamFactory: streamFactory, pinned: pinned)
        {
        }

        internal MonoTexture(
            MonoRenderer renderer,
            string name,
            AtlasReference atlasReference
        ) : base(renderer: renderer, name: name, atlasReference: atlasReference)
        {
        }

        public override int Width => PlatformTexture?.Width ?? 0;

        public override int Height => PlatformTexture?.Height ?? 0;

        protected override Texture2D? CreatePlatformTextureFromStream(Stream stream)
        {
            var platformTexture = Renderer.CreatePlatformTextureFromStream(this, stream);
            if (platformTexture is { IsDisposed: false })
            {
                platformTexture.Disposing += (_, _) => PlatformTexture = null;
            }
            return platformTexture;
        }

        public override Color GetPixel(int x, int y)
        {
            if (PlatformTexture is not { } platformTexture)
            {
                return Color.White;
            }

            if (AtlasReference is not null)
            {
                if (AtlasReference.IsRotated)
                {
                    var z = x;
                    x = AtlasReference.Bounds.Right - y - AtlasReference.Bounds.Height;
                    y = AtlasReference.Bounds.Top + z;
                }
                else
                {
                    x += AtlasReference.Bounds.X;
                    y += AtlasReference.Bounds.Y;
                }
            }

            var buffer = new Microsoft.Xna.Framework.Color[1];
            var pixelBounds = new Rectangle(x, y, 1, 1);
            platformTexture.GetData(level: 0, rect: pixelBounds, data: buffer, startIndex: 0, elementCount: 1);
            var pixelColor = buffer[0];
            return new Color(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B);
        }
    }
}
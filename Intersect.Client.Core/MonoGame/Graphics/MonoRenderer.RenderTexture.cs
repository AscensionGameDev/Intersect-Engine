using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;

internal partial class MonoRenderer
{
    public override IGameRenderTexture CreateRenderTexture(int width, int height)
    {
        var platformRenderTexture = CreatePlatformRenderTexture(width, height);
        MonoRenderTexture gameRenderTexture = new(this, platformRenderTexture);

        if (!_allocatedTextures.TryAdd(platformRenderTexture, gameRenderTexture))
        {
            throw new InvalidOperationException("Failed to record allocated texture");
        }

        return gameRenderTexture;
    }

    private RenderTarget2D CreatePlatformRenderTexture(int width, int height)
    {
        var platformRenderTexture = new RenderTarget2D(
            _graphicsDevice,
            width,
            height,
            false,
            _graphicsDevice.PresentationParameters.BackBufferFormat,
            _graphicsDevice.PresentationParameters.DepthStencilFormat,
            /* For whatever reason if this isn't zero everything breaks in .NET 7 on MacOS and most Windows devices */
            0, // graphicsDevice.PresentationParameters.MultiSampleCount,
            RenderTargetUsage.PreserveContents
        );

        platformRenderTexture.Disposing += Texture2DOnDisposing;

        return platformRenderTexture;
    }

    private class MonoRenderTexture : GameRenderTexture<RenderTarget2D, MonoRenderer>
    {
        internal MonoRenderTexture(MonoRenderer renderer, RenderTarget2D renderTarget2D) : base(
            renderer,
            platformRenderTexture: renderTarget2D
        )
        {
            Width = renderTarget2D.Width;
            Height = renderTarget2D.Height;
        }

        public override int Width { get; }

        public override int Height { get; }

        protected override RenderTarget2D CreatePlatformTextureFromStream(Stream stream) =>
            throw new NotSupportedException("Render targets cannot be created from streams");

        public override Color GetPixel(int x, int y)
        {
            var pixel = new Microsoft.Xna.Framework.Color[1];
            PlatformTexture?.GetData(0, new Rectangle(x, y, 1, 1), pixel, 0, 1);
            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Begin()
        {
            return true;
        }

        public override void End()
        {
            (Core.Graphics.Renderer as MonoRenderer)?.EndSpriteBatch();
        }

        public override void Clear(Color color)
        {
            Renderer.EndSpriteBatch();
            Renderer._graphicsDevice.SetRenderTarget(PlatformTexture);
            Renderer._graphicsDevice.Clear(ConvertColor(color));
            Renderer._graphicsDevice.SetRenderTarget(default);
        }
    }
}
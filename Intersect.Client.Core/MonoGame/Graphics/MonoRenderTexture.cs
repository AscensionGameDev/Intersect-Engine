using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;


public partial class MonoRenderTexture : GameRenderTexture
{
    private readonly GraphicsDevice _graphicsDevice;

    private RenderTarget2D? _renderTexture;

    public MonoRenderTexture(GraphicsDevice graphicsDevice, int width, int height) : base(width, height)
    {
        _renderTexture = new RenderTarget2D(
            graphicsDevice,
            width,
            height,
            mipMap: false,
            preferredFormat: graphicsDevice.PresentationParameters.BackBufferFormat,
            preferredDepthFormat: graphicsDevice.PresentationParameters.DepthStencilFormat,
            /* For whatever reason if this isn't zero everything breaks in .NET 7 on MacOS and most Windows devices */
            preferredMultiSampleCount: 0, // graphicsDevice.PresentationParameters.MultiSampleCount,
            usage: RenderTargetUsage.PreserveContents
        );

        _graphicsDevice = graphicsDevice;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object? GetTexture() => _renderTexture;

    public override Color GetPixel(int x1, int y1)
    {
        var pixel = new Microsoft.Xna.Framework.Color[1];
        _renderTexture?.GetData(0, new Rectangle(x1, y1, 1, 1), pixel, 0, 1);
        return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override GameTexturePackFrame? GetTexturePackFrame() => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Begin() => true;

    public override void End()
    {
        (Core.Graphics.Renderer as MonoRenderer)?.EndSpriteBatch();
    }

    public override void Clear(Color color)
    {
        (Core.Graphics.Renderer as MonoRenderer)?.EndSpriteBatch();
        _graphicsDevice.SetRenderTarget(_renderTexture);
        _graphicsDevice.Clear(MonoRenderer.ConvertColor(color));
        _graphicsDevice.SetRenderTarget(default);
    }

    public override void Dispose()
    {
        base.Dispose();

        _renderTexture?.Dispose();
        _renderTexture = default;
    }
}

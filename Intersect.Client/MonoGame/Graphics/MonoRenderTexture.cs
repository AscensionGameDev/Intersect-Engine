using Intersect.Client.Framework;
using Intersect.Client.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics
{
    internal class MonoRenderTexture : GameRenderTexture<RenderTarget2D, MonoGameRenderer>
    {
        public MonoRenderTexture(IGameContext gameContext, MonoGameRenderer renderer, RenderTarget2D renderTarget) :
            base(gameContext, renderer, renderTarget)
        {
        }

        public override Color GetPixel(int x, int y)
        {
            var pixels = new Microsoft.Xna.Framework.Color[1];
            PlatformTexture.GetData(0, new Rectangle(x, y, 1, 1), pixels, 0, 1);
            return new Color(pixels[0].A, pixels[0].R, pixels[0].G, pixels[0].B);
        }

        public override void Clear(Color color)
        {
            Renderer.End(false);
            Renderer.SetRenderTexture(this);
            Renderer.Clear(color);
            Renderer.SetRenderTexture(null);
        }

        public override int Width => PlatformTexture.Width;

        public override int Height => PlatformTexture.Height;

        public override bool BeginFrame() => true;

        public override void EndFrame() => Renderer.End(false);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                PlatformTexture.Dispose();
            }
        }
    }
}

using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoRenderTexture : GameRenderTexture
    {
        private GraphicsDevice _graphicsDevice;
        private int _height = 0;
        private RenderTarget2D _renderTexture;
        private int _width = 0;

        public MonoRenderTexture(GraphicsDevice graphicsDevice, int width, int height) : base(width, height)
        {
            _renderTexture = new RenderTarget2D(graphicsDevice, width, height, false,
                SurfaceFormat.Color,
                DepthFormat.Depth16,
                0,
                RenderTargetUsage.PreserveContents);
            _graphicsDevice = graphicsDevice;
            _width = width;
            _height = height;
        }

        public override int GetWidth()
        {
            return _width;
        }

        public override int GetHeight()
        {
            return _height;
        }

        public override object GetTexture()
        {
            return _renderTexture;
        }

        public override Color GetPixel(int x1, int y1)
        {
            Microsoft.Xna.Framework.Color[] pixel = new Microsoft.Xna.Framework.Color[1];
            _renderTexture.GetData(0, new Rectangle(x1, y1, 1, 1), pixel, 0, 1);
            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        public override bool Begin()
        {
            return true;
        }

        public override void End()
        {
        }

        public override void Clear(Color color)
        {
            _graphicsDevice.SetRenderTarget(_renderTexture);
            _graphicsDevice.Clear(MonoRenderer.ConvertColor(color));
            _graphicsDevice.SetRenderTarget(null);
        }
    }
}
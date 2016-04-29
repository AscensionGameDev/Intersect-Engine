/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using IntersectClientExtras.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoRenderTexture : GameRenderTexture
    {
        private int _width = 0;
        private int _height = 0;
        private RenderTarget2D _renderTexture;
        private GraphicsDevice _graphicsDevice;
        public MonoRenderTexture(GraphicsDevice graphicsDevice,int width, int height) : base(width, height)
        {
            _renderTexture = new RenderTarget2D(graphicsDevice, width, height,false,
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
            _renderTexture.GetData(0,new Rectangle(x1,y1,1,1),pixel,0,1);
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

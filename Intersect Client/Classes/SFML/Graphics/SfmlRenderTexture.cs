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
using Intersect_Client.Classes.Core;
using SFML.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics
{
    public class SfmlRenderTexture : GameRenderTexture
    {
        private RenderTexture _renderTexture;
        private int _width;
        private int _height;
        public SfmlRenderTexture(int width, int height) : base(width, height)
        {
            bool convertToPot = false;
            if (convertToPot)
            {
                int targetWidth = width;
                int targetHeight = height;
                if (!IsPowerOfTwo(targetWidth))
                {
                    int val = 1;
                    while (val < targetWidth)
                        val *= 2;
                    targetWidth = val;
                }
                if (!IsPowerOfTwo(targetHeight))
                {
                    int val = 1;
                    while (val < targetHeight)
                        val *= 2;
                    targetHeight = val;
                }
                _renderTexture = new RenderTexture((uint) targetWidth, (uint) targetHeight);
            }
            else
            {
                _renderTexture = new RenderTexture((uint)width, (uint)height);
            }
            _width = width;
            _height = height;
        }
        bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
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
            return _renderTexture.Texture;
        }

        public RenderTexture GetRenderTexture()
        {
            return _renderTexture;
        }

        public override Color GetPixel(int x1, int y1)
        {
            global::SFML.Graphics.Color clr = _renderTexture.Texture.CopyToImage().GetPixel((uint) x1, (uint) y1);
            return new Color(clr.A,clr.R,clr.G,clr.B);
        }

        public override bool Begin()
        {
            return true;
        }

        public override void End()
        {
            _renderTexture.Display();
        }

        public override void Clear(Color color)
        {
            _renderTexture.Clear(new global::SFML.Graphics.Color(color.R, color.G, color.B, color.A));
        }

        ~SfmlRenderTexture()
        {
            ((SfmlRenderer) GameGraphics.Renderer).RenderTextureDisposed();
            _renderTexture.Dispose();
        }
    }
}

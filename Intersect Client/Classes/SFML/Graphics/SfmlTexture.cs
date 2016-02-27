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
using SFML.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics
{
    public class SfmlTexture : GameTexture
    {
        private Texture _texture;
        private string _path;
        public SfmlTexture(Texture tex)
        {
            _texture = tex;
        }
        public SfmlTexture(string path)
        {
            _path = path;
            _texture = new Texture(_path);
        }

        public override int GetWidth()
        {
            return (int)_texture.Size.X;
        }

        public override int GetHeight()
        {
            return (int)_texture.Size.Y;
        }

        public override object GetTexture()
        {
            return _texture;
        }

        public override Color GetPixel(int x1, int y1)
        {
            global::SFML.Graphics.Color clr = _texture.CopyToImage().GetPixel((uint)x1, (uint)y1);
            return new Color(clr.A,clr.R,clr.G,clr.B);
        }
    }
}

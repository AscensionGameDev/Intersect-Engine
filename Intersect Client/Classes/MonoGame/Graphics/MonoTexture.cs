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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoTexture : GameTexture
    {
        private Texture2D _tex;
        public MonoTexture(GraphicsDevice graphicsDevice, string filename)
        {
            using (var fileStream = new FileStream(filename, FileMode.Open))
            {
                _tex = Texture2D.FromStream(graphicsDevice, fileStream);
            }
        }
        public override int GetHeight()
        {
            return _tex.Height;
        }

        public override Color GetPixel(int x1, int y1)
        {
            Microsoft.Xna.Framework.Color[] pixel = new Microsoft.Xna.Framework.Color[1];
            _tex.GetData(0, new Microsoft.Xna.Framework.Rectangle(x1, y1, 1, 1), pixel, 0, 1);
            return new Color(pixel[0].A, pixel[0].R, pixel[0].G, pixel[0].B);
        }

        public override object GetTexture()
        {
            return _tex;
        }

        public override int GetWidth()
        {
            return _tex.Width;
        }
    }
}

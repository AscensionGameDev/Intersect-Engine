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
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Sys;
using Intersect_Client.Classes.General;
using SFML.Graphics;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics
{
    public class SfmlTexture : GameTexture
    {
        private Texture _texture;
        private string _path;
        private long lastAccess = 0;
        public SfmlTexture(Texture tex)
        {
            _texture = tex;
        }
        public SfmlTexture(string path)
        {
            _path = path;
        }

        private void LoadTexture()
        {
            _texture = new Texture(_path);
        }
        public override int GetWidth()
        {
            return (int)GetSfmlTexture().Size.X;
        }

        public override int GetHeight()
        {
            return (int)GetSfmlTexture().Size.Y;
        }

        public override object GetTexture()
        {
            return GetSfmlTexture();
        }

        private Texture GetSfmlTexture()
        {
            if (_texture == null)
            {
                LoadTexture();
            }
            lastAccess = Environment.TickCount;
            return _texture;
        }

        public bool Update()
        {
            if (_texture != null && !string.IsNullOrEmpty(_path) && Globals.System.GetTimeMS() - 15000 > lastAccess)
            {
                _texture.Dispose();
                _texture = null;
            }

            return _texture != null;
        }

        public string GetPath()
        {
            return _path;
        }

        public override Color GetPixel(int x1, int y1)
        {
            global::SFML.Graphics.Color clr = GetSfmlTexture().CopyToImage().GetPixel((uint)x1, (uint)y1);
            return new Color(clr.A,clr.R,clr.G,clr.B);
        }
    }
}

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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private bool hasLoaded = false;
        private int _width = 0;
        private int _height = 0;

        public SfmlTexture(string path)
        {
            _path = path;
        }

        private void LoadTexture()
        {
            bool convertToPot = false;
            if (convertToPot)
            {
                Bitmap image = new Bitmap(_path);
                int targetWidth = image.Width;
                int targetHeight = image.Height;
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
                _width = image.Width;
                _height = image.Height;
                if (targetWidth == image.Width && targetHeight == image.Height)
                {
                    _texture = new Texture(ImageToByte(image));
                }
                else
                {
                    Bitmap newImage = new Bitmap(targetWidth, targetHeight);
                    global::System.Drawing.Graphics g = global::System.Drawing.Graphics.FromImage(newImage);
                    g.DrawImage(image,new Rectangle(0,0,image.Width,image.Height),new Rectangle(0,0,image.Width,image.Height),GraphicsUnit.Pixel);
                    g.Dispose();
                    _texture = new Texture(ImageToByte(newImage));
                    newImage.Dispose();
                }
                image.Dispose();
            }
            else
            {
                _texture = new Texture(_path);
                _width = (int)_texture.Size.X;
                _height = (int)_texture.Size.Y;
            }
            hasLoaded = true;
        }
        public Byte[] ImageToByte(Bitmap imageSource)
        {
            Stream stream = new MemoryStream();
            imageSource.Save(stream,ImageFormat.Png);
            Byte[] buffer = null;
            stream.Position = 0;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }
        bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }
        public override int GetWidth()
        {
            if (!hasLoaded) LoadTexture();
            return _width;
        }

        public override int GetHeight()
        {
            if (!hasLoaded) LoadTexture();
            return _height;
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

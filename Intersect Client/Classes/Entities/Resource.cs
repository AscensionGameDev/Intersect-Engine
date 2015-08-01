/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Drawing;
using Intersect_Client.Classes;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Graphics = Intersect_Client.Classes.Graphics;
using SFML.System;
using System.IO;

namespace Intersect_Client.Classes
{
    public class Resource : Entity
    {
        //Tileset Locations
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Resource() : base()
        {

        }

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            X = bf.ReadInteger();
            Y = bf.ReadInteger();
            Width = bf.ReadInteger();
            Height = bf.ReadInteger();
            HideName = 1;
        }

        //Rendering Resources
       override public void Draw(int i)
        {
            Rectangle srcRectangle = new Rectangle();
            Rectangle destRectangle = new Rectangle();
            Texture srcTexture;
            if (File.Exists("Resources/Tilesets/" + MySprite.ToLower()))
            {
                var str = MySprite.ToLower();
                var charsToRemove = new string[] { "tileset", ".png"};
                foreach (var c in charsToRemove)
                {
                    str = str.Replace(c, string.Empty);
                }
                srcTexture = Graphics.Tilesets[Convert.ToInt32(str) - 1];

                if ((Height + 1) * Constants.TileHeight > Constants.TileHeight)
                {
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * Constants.TileHeight + OffsetY - (((Height + 1) * Constants.TileHeight) - Constants.TileHeight));
                }
                else
                {
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * Constants.TileHeight + OffsetY);
                }
                if ((Width + 1) * Constants.TileWidth > Constants.TileWidth)
                {
                    destRectangle.X = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * Constants.TileWidth + OffsetX - (((Width + 1) * 16) - 16));
                }
                else
                {
                    destRectangle.X = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * Constants.TileWidth + OffsetX);
                }

                srcRectangle = new Rectangle(X * Constants.TileWidth, Y * Constants.TileHeight, (Width + 1) * Constants.TileWidth, (Height + 1) * Constants.TileHeight);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.RenderTexture(srcTexture, srcRectangle, destRectangle, Graphics.RenderWindow);
            }
        }
    }
}

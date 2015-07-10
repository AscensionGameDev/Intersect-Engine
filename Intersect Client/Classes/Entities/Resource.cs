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
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                srcTexture = Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())];

                if (Height * 32 > 32)
                {
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((Height * 32) - 32));
                }
                else
                {
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY);
                }
                if (Width * 32 > 32)
                {
                    destRectangle.X = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * 32 + OffsetX - ((Width * 32) - 32));
                }
                else
                {
                    destRectangle.X = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * 32 + OffsetX);
                }

                srcRectangle = new Rectangle(X * 32, Y * 32, Width * 32, Height * 32);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.RenderTexture(srcTexture, srcRectangle, destRectangle, Graphics.RenderWindow);
            }
        }
    }
}

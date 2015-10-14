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
        private bool _hasRenderBounds;
        RectangleF srcRectangle = new RectangleF();
        RectangleF destRectangle = new RectangleF();

        public Resource() : base()
        {

        }

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            HideName = 1;
        }

        override public bool Update()
        {
            bool result = base.Update();
            if (!_hasRenderBounds) { CalculateRenderBounds(); }
            if (result && !Graphics.CurrentView.Intersects(new FloatRect(destRectangle.Left,destRectangle.Top,destRectangle.Width,destRectangle.Height)))
            {
                if (RenderList != null)
                {
                    RenderList.Remove(this);
                }
            }
            return result;
        }

        private void CalculateRenderBounds()
        {
            if (!Globals.GameMaps.ContainsKey(CurrentMap)) return;
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
            Texture srcTexture;
            if (Graphics.ResourceFileNames.IndexOf(MySprite) > -1)
            {
                srcTexture = Graphics.ResourceTextures[Graphics.ResourceFileNames.IndexOf(MySprite)];
                srcRectangle = new RectangleF(0, 0, Graphics.ResourceTextures[Graphics.ResourceFileNames.IndexOf(MySprite)].Size.X, Graphics.ResourceTextures[Graphics.ResourceFileNames.IndexOf(MySprite)].Size.Y);
                destRectangle.Y = (int)(Globals.GameMaps[CurrentMap].GetY() + CurrentY * Globals.TileHeight + OffsetY);
                destRectangle.X = (int)(Globals.GameMaps[CurrentMap].GetX() + CurrentX * Globals.TileWidth + OffsetX);
                if (srcRectangle.Height > 32) { destRectangle.Y -= srcRectangle.Height - 32; }
                if (srcRectangle.Width > 32) { destRectangle.X -= (srcRectangle.Width - 32) / 2; }
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                _hasRenderBounds = true;
            }
        }

        //Rendering Resources
       override public void Draw()
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
            Texture srcTexture;
            if (Graphics.ResourceFileNames.IndexOf(MySprite) > -1)
            {
                srcTexture = Graphics.ResourceTextures[Graphics.ResourceFileNames.IndexOf(MySprite)];
                Graphics.RenderTexture(srcTexture, srcRectangle, destRectangle, Graphics.RenderWindow);
            }
        }
    }
}

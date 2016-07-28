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
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using IntersectClientExtras.Graphics;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;
using Intersect_Client.Classes.Maps;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.Entities
{
    public class Resource : Entity
    {
        private bool _hasRenderBounds;
        FloatRect srcRectangle = new FloatRect();
        FloatRect destRectangle = new FloatRect();
        public bool IsDead;
        public ResourceBase _baseResource;

        public Resource() : base()
        {

        }

        public ResourceBase GetResourceBase()
        {
            return _baseResource;
        }

        public void Load(ByteBuffer bf)
        {
            IsDead = Convert.ToBoolean(bf.ReadInteger());
            var baseIndex = bf.ReadInteger();
            _baseResource = ResourceBase.GetResource(baseIndex);
            base.Load(bf);
            HideName = 1;
            CalculateRenderBounds();
        }

        override public bool Update()
        {
            bool result = base.Update();
            if (!_hasRenderBounds) { CalculateRenderBounds(); }
            if (result && !GameGraphics.CurrentView.IntersectsWith(new FloatRect(destRectangle.Left,destRectangle.Top,destRectangle.Width,destRectangle.Height)))
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
            var map = MapInstance.GetMap(CurrentMap);
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || map == null)
            {
                return;
            }
            GameTexture srcTexture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, MySprite);
            if (srcTexture != null)
            {
                srcRectangle = new FloatRect(0, 0, srcTexture.GetWidth(), srcTexture.GetHeight());
                destRectangle.Y = (int)(map.GetY() + CurrentY * Options.TileHeight + OffsetY);
                destRectangle.X = (int)(map.GetX() + CurrentX * Options.TileWidth + OffsetX);
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
            GameTexture srcTexture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, MySprite);
            if (srcTexture != null)
            {
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);
            }
        }
    }
}

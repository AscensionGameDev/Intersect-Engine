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

        public Resource(int index,long spawnTime,ByteBuffer bf) : base(index,spawnTime,bf)
        {
        }

        public ResourceBase GetResourceBase()
        {
            return _baseResource;
        }

        public override void Load(ByteBuffer bf)
        {
            base.Load(bf);
            IsDead = Convert.ToBoolean(bf.ReadInteger());
            var baseIndex = bf.ReadInteger();
            _baseResource = ResourceBase.GetResource(baseIndex);
            HideName = 1;
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }

        override public bool Update()
        {
            CalculateRenderBounds();
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
            if (map == null)
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
            if (MapInstance.GetMap(CurrentMap) == null || !Globals.GridMaps.Contains(CurrentMap)) return;
            GameTexture srcTexture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, MySprite);
            if (srcTexture != null)
            {
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);
            }
        }
    }
}

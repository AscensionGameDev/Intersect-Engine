using System;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;

namespace Intersect_Client.Classes.Entities
{
    public class Resource : Entity
    {
        public ResourceBase _baseResource;
        private bool _hasRenderBounds;
        FloatRect destRectangle = FloatRect.Empty;
        FloatRect srcRectangle = FloatRect.Empty;

        public override string MySprite
        {
            get { return _mySprite; }
            set
            {
                _mySprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, _mySprite);
                _hasRenderBounds = false;
            }
        }
        public bool IsDead;

        public Resource(int index, long spawnTime, ByteBuffer bf) : base(index, spawnTime, bf)
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
            _baseResource = ResourceBase.Lookup.Get<ResourceBase>(baseIndex);
            HideName = 1;
            if (IsDead)
            {
                MySprite = _baseResource.EndGraphic;
            }
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }

        public override bool Update()
        {
            if (!_hasRenderBounds)
            {
                CalculateRenderBounds();
            }
            if (!GameGraphics.CurrentView.IntersectsWith(destRectangle))
            {
                if (RenderList != null)
                {
                    RenderList.Remove(this);
                }
                return true;
            }
            bool result = base.Update();
            if (!result)
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
            var map = MapInstance;
            if (map == null)
            {
                return;
            }
            if (Texture != null)
            { 
                srcRectangle.Width = Texture.GetWidth();
                srcRectangle.Height = Texture.GetHeight();
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                destRectangle.Y = (int) (map.GetY() + CurrentY * Options.TileHeight + OffsetY);
                destRectangle.X = (int) (map.GetX() + CurrentX * Options.TileWidth + OffsetX);
                if (srcRectangle.Height > 32)
                {
                    destRectangle.Y -= srcRectangle.Height - 32;
                }
                if (srcRectangle.Width > 32)
                {
                    destRectangle.X -= (srcRectangle.Width - 32) / 2;
                }
                _hasRenderBounds = true;
            }
        }

        //Rendering Resources
        public override void Draw()
        {
            if (MapInstance == null) return;
            if (Texture != null)
            {
                GameGraphics.DrawGameTexture(Texture, srcRectangle, destRectangle, Intersect.Color.White);
            }
        }
    }
}
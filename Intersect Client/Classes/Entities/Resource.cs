using System;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Entities
{
    public class Resource : Entity
    {
        public ResourceBase BaseResource;
        private bool mHasRenderBounds;
        public bool IsDead;
        FloatRect mDestRectangle = FloatRect.Empty;
        FloatRect mSrcRectangle = FloatRect.Empty;

        public Resource(int index, long spawnTime, ByteBuffer bf) : base(index, spawnTime, bf)
        {
            mRenderPriority = 0;
        }

        public override string MySprite
        {
            get => mMySprite;
            set
            {
                mMySprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, mMySprite);
                mHasRenderBounds = false;
            }
        }
        public ResourceBase GetResourceBase()
        {
            return BaseResource;
        }

        public override void Load(ByteBuffer bf)
        {
            base.Load(bf);
            IsDead = Convert.ToBoolean(bf.ReadInteger());
            var baseIndex = bf.ReadInteger();
            BaseResource = ResourceBase.Lookup.Get<ResourceBase>(baseIndex);
            HideName = 1;
            if (IsDead)
            {
                MySprite = BaseResource?.EndGraphic;
            }
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }

        public override bool Update()
        {
            if (!mHasRenderBounds)
            {
                CalculateRenderBounds();
            }
            if (!GameGraphics.CurrentView.IntersectsWith(mDestRectangle))
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
                mSrcRectangle.Width = Texture.GetWidth();
                mSrcRectangle.Height = Texture.GetHeight();
                mDestRectangle.Width = mSrcRectangle.Width;
                mDestRectangle.Height = mSrcRectangle.Height;
                mDestRectangle.Y = (int) (map.GetY() + CurrentY * Options.TileHeight + OffsetY);
                mDestRectangle.X = (int) (map.GetX() + CurrentX * Options.TileWidth + OffsetX);
                if (mSrcRectangle.Height > Options.TileHeight)
                {
                    mDestRectangle.Y -= mSrcRectangle.Height - Options.TileHeight;
                }
                if (mSrcRectangle.Width > Options.TileWidth)
                {
                    mDestRectangle.X -= (mSrcRectangle.Width - Options.TileWidth) / 2;
                }
                mHasRenderBounds = true;
            }
        }

        //Rendering Resources
        public override void Draw()
        {
            if (MapInstance == null) return;
            if (Texture != null)
            {
                GameGraphics.DrawGameTexture(Texture, mSrcRectangle, mDestRectangle, Intersect.Color.White);
            }
        }
    }
}
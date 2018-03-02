using System;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
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
        private bool _waitingForTilesets;

        public Resource(int index, long spawnTime, ByteBuffer bf) : base(index, spawnTime, bf)
        {
            mRenderPriority = 0;
        }

        public override string MySprite
        {
            get => mMySprite;
            set
            {
                if (BaseResource == null) return;
                mMySprite = value;
                if ((IsDead && BaseResource.EndGraphicFromTileset) || (!IsDead && BaseResource.InitialGraphicFromTileset))
                {
                    if (GameContentManager.Current.TilesetsLoaded)
                    {
                        Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset, mMySprite);
                    }
                    else
                    {
                        _waitingForTilesets = true;
                    }
                }
                else
                {
                    Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Resource, mMySprite);
                }
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
            else
            {
                MySprite = BaseResource?.InitialGraphic;
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
            if (_waitingForTilesets && !GameContentManager.Current.TilesetsLoaded) return;
            if (_waitingForTilesets && GameContentManager.Current.TilesetsLoaded)
            {
                _waitingForTilesets = false;
                MySprite = MySprite;
            }
            if (Texture != null)
            {
                mSrcRectangle.X = 0;
                mSrcRectangle.Y = 0;
                if (IsDead && BaseResource.EndGraphicFromTileset)
                {
                    mSrcRectangle.X = BaseResource.EndTilesetX * Options.TileWidth;
                    mSrcRectangle.Y = BaseResource.EndTilesetY * Options.TileHeight;
                    mSrcRectangle.Width = (BaseResource.EndTilesetWidth + 1) * Options.TileWidth;
                    mSrcRectangle.Height = (BaseResource.EndTilesetHeight + 1) * Options.TileHeight;
                }
                else if (!IsDead && BaseResource.InitialGraphicFromTileset)
                {
                    mSrcRectangle.X = BaseResource.InitialTilesetX * Options.TileWidth;
                    mSrcRectangle.Y = BaseResource.InitialTilesetY * Options.TileHeight;
                    mSrcRectangle.Width = (BaseResource.InitialTilesetWidth + 1) * Options.TileWidth;
                    mSrcRectangle.Height = (BaseResource.InitialTilesetHeight + 1) * Options.TileHeight;
                }
                else
                {
                    mSrcRectangle.Width = Texture.GetWidth();
                    mSrcRectangle.Height = Texture.GetHeight();
                }
                mDestRectangle.Width = mSrcRectangle.Width;
                mDestRectangle.Height = mSrcRectangle.Height;
                mDestRectangle.Y = (int)(map.GetY() + CurrentY * Options.TileHeight + OffsetY);
                mDestRectangle.X = (int)(map.GetX() + CurrentX * Options.TileWidth + OffsetX);
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
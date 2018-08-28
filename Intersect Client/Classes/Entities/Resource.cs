using System;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using System.Collections.Generic;

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

        public Resource(Guid id, ByteBuffer bf) : base(id, bf)
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
                if ((IsDead && BaseResource.Exhausted.GraphicFromTileset) || (!IsDead && BaseResource.Initial.GraphicFromTileset))
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
            var baseId = bf.ReadGuid();
            BaseResource = ResourceBase.Get(baseId);
            HideName = true;
            if (IsDead)
            {
                MySprite = BaseResource?.Exhausted.Graphic;
            }
            else
            {
                MySprite = BaseResource?.Initial.Graphic;
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

        public override HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, MapInstance map)
        {
            if (IsDead && !BaseResource.Exhausted.RenderBelowEntities) return base.DetermineRenderOrder(renderList, map);
            if (!IsDead && !BaseResource.Initial.RenderBelowEntities) return base.DetermineRenderOrder(renderList, map);
            //Otherwise we are alive or dead and we want to render below players/npcs
            if (renderList != null)
            {
                renderList.Remove(this);
            }

            if (map == null || Globals.Me == null || Globals.Me.MapInstance == null)
            {
                return null;
            }
            var gridX = Globals.Me.MapInstance.MapGridX;
            var gridY = Globals.Me.MapInstance.MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            var priority = mRenderPriority;
                            if (CurrentZ != 0)
                            {
                                priority += 3;
                            }
                            if (y == gridY - 1)
                            {
                                GameGraphics.RenderingEntities[priority, CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY];
                                return renderList;
                            }
                            else if (y == gridY)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY];
                                return renderList;
                            }
                            else
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 3 + CurrentY];
                                return renderList;
                            }
                        }
                    }
                }
            }
            return renderList;
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
                if (IsDead && BaseResource.Exhausted.GraphicFromTileset)
                {
                    mSrcRectangle.X = BaseResource.Exhausted.X * Options.TileWidth;
                    mSrcRectangle.Y = BaseResource.Exhausted.Y * Options.TileHeight;
                    mSrcRectangle.Width = (BaseResource.Exhausted.Width + 1) * Options.TileWidth;
                    mSrcRectangle.Height = (BaseResource.Exhausted.Height + 1) * Options.TileHeight;
                }
                else if (!IsDead && BaseResource.Initial.GraphicFromTileset)
                {
                    mSrcRectangle.X = BaseResource.Initial.X * Options.TileWidth;
                    mSrcRectangle.Y = BaseResource.Initial.Y * Options.TileHeight;
                    mSrcRectangle.Width = (BaseResource.Initial.Width + 1) * Options.TileWidth;
                    mSrcRectangle.Height = (BaseResource.Initial.Height + 1) * Options.TileHeight;
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
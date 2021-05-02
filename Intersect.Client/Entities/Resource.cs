using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Entities
{

    public partial class Resource : Entity
    {

        private bool _waitingForTilesets;

        public ResourceBase BaseResource;

        public bool IsDead;

        FloatRect mDestRectangle = FloatRect.Empty;

        private bool mHasRenderBounds;

        FloatRect mSrcRectangle = FloatRect.Empty;

        public Resource(Guid id, ResourceEntityPacket packet) : base(id, packet)
        {
            mRenderPriority = 0;
        }

        public override string MySprite
        {
            get => mMySprite;
            set
            {
                if (BaseResource == null)
                {
                    return;
                }

                mMySprite = value;
                if (IsDead && BaseResource.Exhausted.GraphicFromTileset ||
                    !IsDead && BaseResource.Initial.GraphicFromTileset)
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

        public override void Load(EntityPacket packet)
        {
            base.Load(packet);
            var pkt = (ResourceEntityPacket) packet;
            IsDead = pkt.IsDead;
            var baseId = pkt.ResourceId;
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

        public override void Dispose()
        {
            if (RenderList != null)
            {
                RenderList.Remove(this);
                RenderList = null;
            }

            ClearAnimations(null);
            mDisposed = true;
        }

        public override bool Update()
        {
            if (mDisposed)
            {
                LatestMap = null;

                return false;
            }
            else
            {
                var map = MapInstance.Get(CurrentMap);
                LatestMap = map;
                if (map == null || !map.InView())
                {
                    Globals.EntitiesToDispose.Add(Id);

                    return false;
                }
            }

            if (!mHasRenderBounds)
            {
                CalculateRenderBounds();
            }

            if (!Graphics.CurrentView.IntersectsWith(mDestRectangle))
            {
                if (RenderList != null)
                {
                    RenderList.Remove(this);
                }

                return true;
            }

            var result = base.Update();
            if (!result)
            {
                if (RenderList != null)
                {
                    RenderList.Remove(this);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override bool CanBeAttacked()
        {
            return !IsDead;
        }

        public override HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, MapInstance map)
        {
            if (IsDead && !BaseResource.Exhausted.RenderBelowEntities)
            {
                return base.DetermineRenderOrder(renderList, map);
            }

            if (!IsDead && !BaseResource.Initial.RenderBelowEntities)
            {
                return base.DetermineRenderOrder(renderList, map);
            }

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
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            var priority = mRenderPriority;
                            if (Z != 0)
                            {
                                priority += 3;
                            }

                            HashSet<Entity> renderSet;

                            if (y == gridY - 1)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Y];
                            }
                            else if (y == gridY)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight + Y];
                            }
                            else
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 2 + Y];
                            }

                            renderSet.Add(this);
                            renderList = renderSet;

                            return renderList;

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

            if (_waitingForTilesets && !GameContentManager.Current.TilesetsLoaded)
            {
                return;
            }

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
                mDestRectangle.Y = (int) (map.GetY() + Y * Options.TileHeight + OffsetY);
                mDestRectangle.X = (int) (map.GetX() + X * Options.TileWidth + OffsetX);
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
            if (MapInstance == null)
            {
                return;
            }

            if (Texture != null)
            {
                Graphics.DrawGameTexture(Texture, mSrcRectangle, mDestRectangle, Intersect.Color.White);
            }
        }

    }

}

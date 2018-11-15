using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Entities.Events
{
    public class Event : Entity
    {
        private GameTexture mCachedTileset;

        private string mCachedTilesetName;
        public string Desc = "";
        public bool DirectionFix;
        public bool DisablePreview;
        public string FaceGraphic = "";
        public string Graphic = "";
        public string GraphicFile = "";
        public int GraphicHeight;
        public int GraphicType;
        public int GraphicWidth;
        public int GraphicX;
        public int GraphicY;
        public int Layer;
        private int mOldRenderLevel;

        private MapInstance mOldRenderMap;
        private int mOldRenderY;
        public int RenderLevel = 1;
        public bool WalkingAnim = true;

        public Event(Guid id, ByteBuffer bf) : base(id, bf, true)
        {
            mRenderPriority = 1;
        }

        public override string ToString()
        {
            return Name;
        }

        public override void Load(ByteBuffer bf)
        {
            base.Load(bf);
            HideName = bf.ReadBoolean();
            DirectionFix = bf.ReadBoolean();
            WalkingAnim = bf.ReadBoolean();
            DisablePreview = bf.ReadBoolean();
            Desc = bf.ReadString();
            GraphicType = bf.ReadInteger();
            GraphicFile = bf.ReadString().ToLower();
            GraphicX = bf.ReadInteger();
            GraphicY = bf.ReadInteger();
            GraphicWidth = bf.ReadInteger();
            GraphicHeight = bf.ReadInteger();
            RenderLevel = bf.ReadInteger();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Event;
        }

        public override bool Update()
        {
            bool success = base.Update();
            if (!WalkingAnim) WalkFrame = 0;
            return success;
        }

        public override void Draw()
        {
            WorldPos.Reset();
            if (MapInstance.Get(CurrentMap) == null ||
                !Globals.GridMaps.Contains(CurrentMap)) return;
            var map = MapInstance.Get(CurrentMap);
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture srcTexture = null;
            int height = 0;
            int width = 0;
            var d = 0;
            switch (GraphicType)
            {
                case 1: //Sprite
                    GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                        GraphicFile);
                    if (entityTex != null)
                    {
                        srcTexture = entityTex;
                        height = srcTexture.GetHeight() / 4;
                        width = srcTexture.GetWidth() / 4;
                        d = GraphicY;
                        if (!DirectionFix)
                        {
                            switch (Dir)
                            {
                                case 0:
                                    d = 3;
                                    break;
                                case 1:
                                    d = 0;
                                    break;
                                case 2:
                                    d = 1;
                                    break;
                                case 3:
                                    d = 2;
                                    break;
                            }
                        }
                        int frame = GraphicX;
                        if (WalkingAnim) frame = WalkFrame;
                        if (Options.AnimatedSprites.Contains(GraphicFile.ToLower()))
                        {
                            srcRectangle = new FloatRect(AnimationFrame * (int) entityTex.GetWidth() / 4,
                                d * (int) entityTex.GetHeight() / 4,
                                (int) entityTex.GetWidth() / 4, (int) entityTex.GetHeight() / 4);
                        }
                        else
                        {
                            srcRectangle = new FloatRect(frame * (int) srcTexture.GetWidth() / 4,
                                d * (int) srcTexture.GetHeight() / 4, (int) srcTexture.GetWidth() / 4,
                                (int) srcTexture.GetHeight() / 4);
                        }
                    }
                    break;
                case 2: //Tile
                    if (mCachedTilesetName != GraphicFile)
                    {
                        mCachedTilesetName = GraphicFile;
                        mCachedTileset = null;
                        foreach (var tilesetName in TilesetBase.GetNameList())
                        {
                            if (tilesetName.ToLower() == GraphicFile.ToLower())
                            {
                                mCachedTileset =
                                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                                        GraphicFile);
                                break;
                            }
                        }
                    }
                    GameTexture tileset = mCachedTileset;
                    if (tileset != null)
                    {
                        srcTexture = tileset;
                        width = (GraphicWidth + 1) * Options.TileWidth;
                        height = (GraphicHeight + 1) * Options.TileHeight;
                        srcRectangle = new FloatRect(GraphicX * Options.TileWidth, GraphicY * Options.TileHeight,
                            (GraphicWidth + 1) * Options.TileWidth, (GraphicHeight + 1) * Options.TileHeight);
                    }
                    break;
            }
            if (srcTexture != null)
            {
                destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX;
                if (height > Options.TileHeight)
                {
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY -
                                      ((height) - Options.TileHeight);
                }
                else
                {
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (width > Options.TileWidth)
                {
                    destRectangle.X -= ((width) - Options.TileWidth) / 2;
                }
                destRectangle.X = (int) Math.Ceiling(destRectangle.X);
                destRectangle.Y = (int) Math.Ceiling(destRectangle.Y);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                WorldPos = destRectangle;
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Intersect.Color.White);
            }
        }

        public override HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, MapInstance map)
        {
            if (RenderLevel == 1) return base.DetermineRenderOrder(renderList,map);
            renderList?.Remove(this);
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
                            if (RenderLevel == 0) y--;
                            if (RenderLevel == 2) y++;
                            var priority = mRenderPriority;
                            if (CurrentZ != 0)
                            {
                                priority += 3;
                            }
                            if (y == gridY - 2)
                            {
                                GameGraphics.RenderingEntities[priority, CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, CurrentY];
                                return renderList;
                            }
                            else if (y == gridY - 1)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY];
                                return renderList;
                            }
                            else if (y == gridY)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY];
                                return renderList;
                            }
                            else if (y == gridY + 1)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 3 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 3 + CurrentY];
                                return renderList;
                            }
                            else if (y == gridY + 2)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 4 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 4 + CurrentY];
                                return renderList;
                            }
                        }
                    }
                }
            }
            return renderList;
        }

        public override void DrawName(Color textColor, Color borderColor, Color backgroundColor)
        {
            if (HideName || Name.Trim().Length == 0)
            {
                return;
            }
            if (MapInstance.Get(CurrentMap) == null ||
                !Globals.GridMaps.Contains(CurrentMap)) return;
            var y = (int) Math.Ceiling(GetCenterPos().Y);
            var x = (int) Math.Ceiling(GetCenterPos().X);
            var height = Options.TileHeight;
            switch (GraphicType)
            {
                case 1: //Sprite
                    GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                        GraphicFile);
                    if (entityTex != null)
                    {
                        height = entityTex.GetHeight();
                    }
                    break;
                case 2: //Tile
                    if (mCachedTilesetName != GraphicFile)
                    {
                        mCachedTilesetName = GraphicFile;
                        mCachedTileset = null;
                        foreach (var tileset in TilesetBase.GetNameList())
                        {
                            if (tileset == GraphicFile)
                            {
                                mCachedTileset =
                                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                                        GraphicFile);
                                break;
                            }
                        }
                    }
                    if (mCachedTileset != null)
                    {
                        height = (GraphicHeight + 1) * Options.TileHeight;
                    }
                    break;
            }

            y = (int) GetTopPos(height) - 12;
            x = (int) Math.Ceiling(GetCenterPos().X);

            Pointf textSize = GameGraphics.Renderer.MeasureText(Name, GameGraphics.GameFont, 1);

            if (CustomColors.EventNameBackground != Color.Transparent)
                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((x - textSize.X / 2f) - 4, y, textSize.X + 8, textSize.Y),
                    CustomColors.EventNameBackground);
            GameGraphics.Renderer.DrawString(Name, GameGraphics.GameFont,
                (int) (x - (int) Math.Ceiling(textSize.X / 2f)), (int) (y), 1,
                Framework.GenericClasses.Color.FromArgb(CustomColors.EventName.ToArgb()), true, null,
                Framework.GenericClasses.Color.FromArgb(CustomColors.EventNameBorder.ToArgb()));
        }

        public override Pointf GetCenterPos()
        {
            var map = MapInstance.Get(CurrentMap);
            if (map == null)
            {
                return new Pointf(0, 0);
            }
            Pointf pos = new Pointf(map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                map.GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            switch (GraphicType)
            {
                case 1: //Sprite
                    GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                        MySprite);
                    if (entityTex != null)
                    {
                        pos.Y += Options.TileHeight / 2;
                        pos.Y -= entityTex.GetHeight() / 4 / 2;
                    }
                    break;
                case 2: //Tile
                    if (mCachedTilesetName != GraphicFile)
                    {
                        mCachedTilesetName = GraphicFile;
                        mCachedTileset = null;
                        foreach (var tileset in TilesetBase.GetNameList())
                        {
                            if (tileset == GraphicFile)
                            {
                                mCachedTileset =
                                    Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                                        GraphicFile);
                                break;
                            }
                        }
                    }
                    if (mCachedTileset != null)
                    {
                        pos.Y -= ((GraphicHeight + 1) * Options.TileHeight) / 2;
                        pos.Y -= 12;
                    }
                    break;
            }
            return pos;
        }

        ~Event()
        {
            Dispose();
        }
    }
}
/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;
using Intersect_Client.Classes.Maps;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.Entities
{
    public class Event : Entity
    {
        public string Desc = "";
        public int Layer;
        public int GraphicType;
        public string Graphic = "";
        public string FaceGraphic = "";
        public int GraphicX;
        public int GraphicY;
        public string GraphicFile = "";
        public int GraphicWidth;
        public int GraphicHeight;
        public int DisablePreview;
        public int DirectionFix;
        public int WalkingAnim = 1;
        public int RenderLevel = 1;

        public Event(int index, long spawnTime, int mapNum, ByteBuffer bf) : base(-1,spawnTime,null)
        {
            MyIndex = index;
            var map = MapInstance.GetMap(mapNum);
            if (map != null)
            {
                map.AddEvent(this);
                Load(bf);
            }
        }

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            HideName = bf.ReadInteger();
            DirectionFix = bf.ReadInteger();
            WalkingAnim = bf.ReadInteger();
            DisablePreview = bf.ReadInteger();
            Desc = bf.ReadString();
            GraphicType = bf.ReadInteger();
            GraphicFile = bf.ReadString();
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
            if (WalkingAnim == 0) WalkFrame = 0;
            return success;
        }

        public override void Draw()
        {
            if (MapInstance.GetMap(CurrentMap) == null || !Globals.GridMaps.Contains(CurrentMap)) return;
            var map = MapInstance.GetMap(CurrentMap);
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
                        if (DirectionFix != 1)
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
                        if (WalkingAnim == 1) frame = WalkFrame;
                        srcRectangle = new FloatRect(frame * (int)srcTexture.GetWidth() / 4, d * (int)srcTexture.GetHeight() / 4, (int)srcTexture.GetWidth() / 4, (int)srcTexture.GetHeight() / 4);
                    }
                    break;
                case 2: //Tile
                    GameTexture tileset = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                        GraphicFile);
                    if (tileset != null)
                    {
                        srcTexture = tileset;
                        width = (GraphicWidth + 1) * Options.TileWidth;
                        height = (GraphicHeight + 1) * Options.TileHeight;
                        srcRectangle = new FloatRect(GraphicX * Options.TileWidth, GraphicY * Options.TileHeight, (GraphicWidth + 1) * Options.TileWidth, (GraphicHeight + 1) * Options.TileHeight);
                    }
                    break;
            }
            if (srcTexture != null)
            {
                destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX;
                if (height > Options.TileHeight)
                {
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY - ((height) - Options.TileHeight);
                }
                else
                {
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (width > Options.TileWidth)
                {
                    destRectangle.X -= ((width) - Options.TileWidth) / 2;
                }
                destRectangle.X = (int)Math.Ceiling(destRectangle.X);
                destRectangle.Y = (int)Math.Ceiling(destRectangle.Y);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);
            }
        }

        public override List<Entity> DetermineRenderOrder(List<Entity> renderList)
        {
            if (RenderLevel == 1) return base.DetermineRenderOrder(renderList);
            if (renderList != null)
            {
                renderList.Remove(this);
            }
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
            {
                return null;
            }
            var gridX = MapInstance.GetMap(CurrentMap).MapGridX;
            var gridY = MapInstance.GetMap(CurrentMap).MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight && Globals.MapGrid[x, y] != -1)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            if (RenderLevel == 0) y-=1;
                            if (RenderLevel == 2) y += 1;
                            List<Entity>[] outerList;
                            if (CurrentZ == 0)
                            {
                                outerList = GameGraphics.Layer1Entities;
                            }
                            else
                            {
                                outerList = GameGraphics.Layer2Entities;
                            }
                            if (y == gridY - 1)
                            {
                                outerList[CurrentY].Add(this);
                                renderList = outerList[CurrentY];
                            }
                            else if (y == gridY)
                            {
                                outerList[Options.MapHeight + CurrentY].Add(this);
                                renderList = outerList[Options.MapHeight + CurrentY];
                            }
                            else
                            {
                                outerList[Options.MapHeight * 2 + CurrentY].Add(this);
                                renderList = outerList[Options.MapHeight * 2 + CurrentY];
                            }
                            break;
                        }
                    }
                }
            }
            return renderList;
        }

        public override void DrawName(Color color)
        {
            if (HideName == 1) { return; }
            if (MapInstance.GetMap(CurrentMap) == null || !Globals.GridMaps.Contains(CurrentMap)) return;
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            switch (GraphicType)
            {
                case 1: //Sprite
                    GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                       GraphicFile);
                    if (entityTex != null)
                    {
                        y -= entityTex.GetHeight()/4/2;
                        y -= 12;
                    }
                    break;
                case 2: //Tile
                    foreach (var tileset in DatabaseObject.GetGameObjectList(GameObject.Tileset))
                    {
                        if (tileset == GraphicFile)
                        {
                            y -= ((GraphicHeight + 1) * Options.TileHeight) / 2;
                            y -= 12;
                            break;
                        }
                    }
                    break;
            }

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 1).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 1, Color.White);
        }

        public override Pointf GetCenterPos()
        {
            var map = MapInstance.GetMap(CurrentMap);
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
                        pos.Y += Options.TileHeight/2;
                        pos.Y -= entityTex.GetHeight()/4/2;
                    }
                    break;
                case 2: //Tile
                    foreach (var tileset in DatabaseObject.GetGameObjectList(GameObject.Tileset))
                    {
                        if (tileset == GraphicFile)
                        {
                            pos.Y += Options.TileHeight / 2;
                            pos.Y -= ((GraphicHeight + 1) * Options.TileHeight) / 2;
                            break;
                        }
                    }
                    break;
            }
            return pos;
        }
    }


}

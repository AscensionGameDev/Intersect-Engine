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
using Options = Intersect_Client.Classes.General.Options;

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

        public Event() : base()
        {

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

        public override bool Update()
        {
            bool success = base.Update();
            if (WalkingAnim == 0) WalkFrame = 0;
            return success;
        }

        public override void Draw()
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
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
                destRectangle.X = Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX;
                if (height > Options.TileHeight)
                {
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY - ((height) - Options.TileHeight);
                }
                else
                {
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY;
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

            if (!Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return null;
            }

            int mapLoc = -1;
            for (int i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    List<Entity>[] outerList;
                    if (CurrentZ == 0)
                    {
                        outerList = GameGraphics.Layer1Entities;
                    }
                    else
                    {
                        outerList = GameGraphics.Layer2Entities;
                    }
                    if (RenderLevel == 0) i -= 3;
                    if (RenderLevel == 2) i += 3;
                    if (i < 3 && i > -1)
                    {
                        outerList[CurrentY].Add(this);
                        renderList = outerList[CurrentY];
                    }
                    else if (i < 6)
                    {
                        outerList[Options.MapHeight + CurrentY].Add(this);
                        renderList = outerList[Options.MapHeight + CurrentY];
                    }
                    else if (i <= 8)
                    {
                        outerList[Options.MapHeight * 2 + CurrentY].Add(this);
                        renderList = outerList[Options.MapHeight * 2 + CurrentY];
                    }
                    break;
                }
            }
            return renderList;
        }

        public override void DrawName()
        {
            if (HideName == 1) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            int width = 0;
            int height = 0;
            GameTexture srcTexture = null;
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
                    for (int z = 0; z < Globals.Tilesets.Length; z++)
                    {
                        if (Globals.Tilesets[z] == GraphicFile)
                        {
                            y -= ((GraphicHeight + 1) * Options.TileHeight) / 2;
                            y -= 12;
                        }
                    }
                    break;
            }

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 10).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 10, Color.White);
        }

        public override Pointf GetCenterPos()
        {
            if (!Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return new Pointf(0, 0);
            }
            Pointf pos = new Pointf(Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                    Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            int width = 0, height = 0;
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
                    for (int z = 0; z < Globals.Tilesets.Length; z++)
                    {
                        if (Globals.Tilesets[z] == GraphicFile)
                        {
                            pos.Y += Options.TileHeight / 2;
                            pos.Y -= ((GraphicHeight + 1) * Options.TileHeight) / 2;
                        }
                    }
                    break;
            }
            return pos;
        }
    }


}

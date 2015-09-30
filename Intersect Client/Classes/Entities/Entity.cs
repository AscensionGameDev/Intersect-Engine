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
using System.Drawing;
using Intersect_Client.Classes;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Graphics = Intersect_Client.Classes.Graphics;
using SFML.System;

// ReSharper disable All

namespace Intersect_Client
{
    public class Entity
    {
        //Core Values
        public int MyIndex;
        public bool IsLocal = false;
        public string MyName = "";
        public string MySprite = "";
        public bool InView = true;
        public int Passable = 0;
        public int HideName = 0;
        public int Level = 1;

        //Extras
        public string Face = "";

        //Location Info
        public int CurrentX;
        public int CurrentY;
        public int CurrentZ = 0;
        public int CurrentMap = -1;
        public int Dir;
        public bool IsMoving;
        public float OffsetX;
        public float OffsetY;
        public int MoveDir = -1;
        public float MoveTimer;
        public float SpriteWidth;
        public float SpriteHeight;

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Vital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];

        private long _lastUpdate;
        private long _walkTimer;
        private int _walkFrame;

        //Rendering Variables
        private List<Entity> RenderList = null;

        //Deserializing
        public void Load(ByteBuffer bf)
        {
            MyName = bf.ReadString();
            MySprite = bf.ReadString();
            Face = bf.ReadString();
            CurrentX = bf.ReadInteger();
            CurrentY = bf.ReadInteger();
            CurrentMap = bf.ReadInteger();
            CurrentZ = bf.ReadInteger();
        }

        //Movement Processing
        public bool Update()
        {
            DetermineRenderOrder();
            if (_lastUpdate == 0) { _lastUpdate = Environment.TickCount; }
            float ecTime = (float)(Environment.TickCount - _lastUpdate);
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (tmpI == -1) return false;
            if (_walkTimer < Environment.TickCount)
            {
                if (IsMoving)
                {
                    _walkFrame++;
                    if (_walkFrame > 3) { _walkFrame = 0; }
                }
                else
                {
                    _walkFrame = 0;
                }
                _walkTimer = Environment.TickCount + 200;
            }
            if (IsMoving)
            {

                switch (Dir)
                {
                    case 0:
                        OffsetY -= (float)ecTime * (40f / 10f * (float)Globals.TileHeight) / 1000f;
                        if (OffsetY < 0) { OffsetY = 0; }
                        break;

                    case 1:
                        OffsetY += (float)ecTime * (40f / 10f * (float)Globals.TileHeight) / 1000f;
                        if (OffsetY > 0) { OffsetY = 0; }
                        break;

                    case 2:
                        OffsetX -= (float)ecTime * (40f / 10f * (float)Globals.TileHeight) / 1000f;
                        if (OffsetX < 0) { OffsetX = 0; }
                        break;

                    case 3:
                        OffsetX += (float)ecTime * (40f / 10f * (float)Globals.TileHeight) / 1000f;
                        if (OffsetX > 0) { OffsetX = 0; }
                        break;
                }
                if (OffsetX == 0 && OffsetY == 0)
                {
                    IsMoving = false;
                }
            }
            _lastUpdate = Environment.TickCount;
            return true;
        }

        public void DetermineRenderOrder()
        {
            if (RenderList != null)
            {
                RenderList.Remove(this);
            }

            int mapLoc = -1;
            for (int i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    List<Entity>[] outerList;
                    if (CurrentZ == 0)
                    {
                        outerList = Graphics.Layer1Entities;
                    }
                    else
                    {
                        outerList = Graphics.Layer2Entities;
                    }
                    if (i < 3)
                    {
                        outerList[CurrentY].Add(this);
                        RenderList = outerList[CurrentY];
                    }
                    else if (i < 6)
                    {
                       outerList[Globals.MapHeight + CurrentY].Add(this);
                       RenderList = outerList[Globals.MapHeight + CurrentY];
                    }
                    else
                    {
                        outerList[Globals.MapHeight * 2 + CurrentY].Add(this);
                        RenderList = outerList[Globals.MapHeight*2 + CurrentY];
                    }
                    break;
                }
            }
        }

        //Rendering Functions
        public virtual void Draw()
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
            RectangleF srcRectangle = new Rectangle();
            RectangleF destRectangle = new Rectangle();
            Texture srcTexture;
            var d = 0;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                srcTexture = Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())];
                if (srcTexture.Size.Y / 4 > Globals.TileHeight)
                {
                    destRectangle.X = (Graphics.CalcMapOffsetX(i) + CurrentX * Globals.TileWidth + OffsetX);
                    destRectangle.Y = Graphics.CalcMapOffsetY(i) + CurrentY * Globals.TileHeight + OffsetY - ((srcTexture.Size.Y / 4) - Globals.TileHeight);
                }
                else
                {
                    destRectangle.X = Graphics.CalcMapOffsetX(i) + CurrentX * Globals.TileWidth + OffsetX;
                    destRectangle.Y = Graphics.CalcMapOffsetY(i) + CurrentY * Globals.TileHeight + OffsetY;
                }
                if (srcTexture.Size.X / 4 > Globals.TileWidth)
                {
                    destRectangle.X -= ((srcTexture.Size.X / 4) - Globals.TileWidth) / 2;
                }
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
                destRectangle.X = (int)Math.Ceiling( destRectangle.X);
                destRectangle.Y = (int) Math.Ceiling(destRectangle.Y);
                srcRectangle = new Rectangle(_walkFrame * (int)srcTexture.Size.X / 4, d * (int)srcTexture.Size.Y / 4, (int)srcTexture.Size.X / 4, (int)srcTexture.Size.Y / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.RenderTexture(srcTexture, srcRectangle, destRectangle, Graphics.RenderWindow);

            }
        }

        public int GetLocalPos(int map)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == map)
                {
                    return i;
                }
            }
            return -1;
        }

        //returns the point on the screen that is the center of the player sprite
        public Vector2f GetCenterPos(int mapPos)
        {
            Sprite tmpSprite;
            Vector2f pos;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                tmpSprite = new Sprite(Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())]);
                pos = new Vector2f(Graphics.CalcMapOffsetX(mapPos) + CurrentX * Globals.TileWidth + OffsetX + tmpSprite.Texture.Size.X / 8f,
                    Graphics.CalcMapOffsetY(mapPos) + CurrentY * Globals.TileHeight + OffsetY - ((tmpSprite.Texture.Size.Y / 4) - Globals.TileHeight) + tmpSprite.Texture.Size.Y / 8f);

            }
            else
            {
                pos = new Vector2f(Graphics.CalcMapOffsetX(mapPos) + CurrentX * Globals.TileWidth + OffsetX + 16,
                    Graphics.CalcMapOffsetY(mapPos) + CurrentY * Globals.TileHeight + OffsetY - Globals.TileHeight + 32);
            }
            return pos;
        }
        public void DrawName()
        {
            if (HideName == 1) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
            var nameText = new Text(MyName, Graphics.GameFont);
            var y = (int)Math.Ceiling(GetCenterPos(i).Y);
            var x = (int)Math.Ceiling(GetCenterPos(i).X);
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y / 4 > Globals.TileHeight)
                {
                    y = y -(int) ((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y/8));
                    y = y - (int)((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y / 4) - Globals.TileHeight) - 04;
                }
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.X / 4 > Globals.TileWidth)
                {
                    x = x - (int)((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.X / 4) - Globals.TileWidth)/2;
                }
            }
            if (this.GetType() != typeof(Intersect_Client.Classes.Event)) { y -= 10; } //Need room for HP bar if not an event.
            nameText.CharacterSize = 10;
            nameText.Position = new Vector2f((int)(x - (int)Math.Ceiling(nameText.GetLocalBounds().Width) / 2), (int)(y));
            //nameText.Position = new Vector2f((int)(x), (int)(y));
            Graphics.RenderWindow.Draw(nameText);
        }
        public void DrawHpBar()
        {
            if (HideName == 1 && Vital[(int)Enums.Vitals.Health] == MaxVital[(int)Enums.Vitals.Health]) { return; }
            if (Vital[(int)Enums.Vitals.Health] <= 0) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1)
            {
                return;
            }
            var width = Globals.TileWidth;
            var bgRect = new RectangleShape(new Vector2f(width, 6));
            var fgRect = new RectangleShape(new Vector2f(width - 2, 4));
            var y = (int)Math.Ceiling(GetCenterPos(i).Y);
            var x = (int)Math.Ceiling(GetCenterPos(i).X);
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y / 4 > Globals.TileHeight)
                {
                    y = y - (int)((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y / 8));
                    y -= (int)((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.Y / 4) - Globals.TileHeight) - 8;
                }
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.X / 4 > Globals.TileWidth)
                {
                    x = x - (int)((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower())].Size.X / 4) - Globals.TileWidth) / 2;
                }
            }
            bgRect.FillColor = Color.Black;
            fgRect.FillColor = Color.Red;
            fgRect.Size = new Vector2f((float)Math.Ceiling((1f * Vital[(int)Enums.Vitals.Health] / MaxVital[(int)Enums.Vitals.Health]) * (width - 2)), 4f);
            bgRect.Position = new Vector2f((int)(x - 1 - width / 2),(int)( y - 1));
            fgRect.Position = new Vector2f((int)(x - width / 2),(int)( y));
            Graphics.RenderWindow.Draw(bgRect);
            Graphics.RenderWindow.Draw(fgRect);
        }
    }
}

using System;
using System.Drawing;
using Intersect_Client.Classes;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Graphics = Intersect_Client.Classes.Graphics;

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

        //Location Info
        public int CurrentX;
        public int CurrentY;
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

        public void Update()
        {
            if (_lastUpdate == 0) { _lastUpdate = Environment.TickCount; }
            var ecTime = Environment.TickCount - _lastUpdate;
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
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
                        OffsetY -= ecTime * (Stat[(int)Enums.Stats.Speed] / 10f * 32) / 1000;
                        if (OffsetY < 0) { OffsetY = 0; }
                        break;

                    case 1:
                        OffsetY += ecTime * (Stat[(int)Enums.Stats.Speed] / 10f * 32) / 1000;
                        if (OffsetY > 0) { OffsetY = 0; }
                        break;

                    case 2:
                        OffsetX -= ecTime * (Stat[(int)Enums.Stats.Speed] / 10f * 32) / 1000;
                        if (OffsetX < 0) { OffsetX = 0; }
                        break;

                    case 3:
                        OffsetX += ecTime * (Stat[(int)Enums.Stats.Speed] / 10f * 32) / 1000;
                        if (OffsetX > 0) { OffsetX = 0; }
                        break;
                }
                if (OffsetX == 0 && OffsetY == 0)
                {
                    IsMoving = false;
                }
            }
            _lastUpdate = Environment.TickCount;
        }

        public void Draw(int i)
        {
            Rectangle srcRectangle = new Rectangle();
            Rectangle destRectangle = new Rectangle();
            Texture srcTexture;
            var d = 0;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                srcTexture = Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")];
                if (srcTexture.Size.Y / 4 > 32)
                {
                    destRectangle.X = (int) Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX*32 + OffsetX);
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((srcTexture.Size.Y / 4) - 32));
                }
                else
                {
                    destRectangle.X = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * 32 + OffsetX);
                    destRectangle.Y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY);
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
                srcRectangle = new Rectangle(_walkFrame * (int)srcTexture.Size.X / 4, d * (int)srcTexture.Size.Y / 4, (int)srcTexture.Size.X / 4, (int)srcTexture.Size.Y / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.RenderTexture(srcTexture,srcRectangle,destRectangle,Graphics.RenderWindow );

            }
        }

        //returns the point on the screen that is the center of the player sprite
        public Vector2f GetCenterPos(int mapPos)
        {
            Sprite tmpSprite;
            Vector2f pos;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                tmpSprite = new Sprite(Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")]);
                pos = new Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(mapPos, true) + CurrentX * 32 + OffsetX + tmpSprite.Texture.Size.X / 8f), (int)Math.Ceiling(Graphics.CalcMapOffsetY(mapPos, true) + CurrentY * 32 + OffsetY - ((tmpSprite.Texture.Size.Y / 4) - 32) + tmpSprite.Texture.Size.Y / 8f));
            }
            else
            {
                pos = new Vector2f((int)Math.Ceiling(Graphics.CalcMapOffsetX(mapPos, true) + CurrentX * 32 + OffsetX), (int)Math.Ceiling(Graphics.CalcMapOffsetY(mapPos, true) + CurrentY * 32 + OffsetY - 32));
            }
            return pos;
        }

        public void DrawName(int i, bool isEvent)
        {
            if (HideName == 1) { return; }
            var nameText = new Text(MyName, Graphics.GameFont);
            var y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY);
            var x = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * 32 + OffsetX) + 16;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 14;
                }
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
                }
            }
            if (!isEvent) { y -= 8; } //Need room for HP bar if not an event.
            nameText.CharacterSize = 10;
            nameText.Position = new Vector2f(x - nameText.GetLocalBounds().Width / 2, y);
            Graphics.RenderWindow.Draw(nameText);
        }

        public void DrawHpBar(int i)
        {
            var width = 32;
            var bgRect = new RectangleShape(new Vector2f(width, 6));
            var fgRect = new RectangleShape(new Vector2f(width-2, 4));
            var y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY) + 6;
            var x = (int)Math.Ceiling(Graphics.CalcMapOffsetX(i) + CurrentX * 32 + OffsetX) + 16;
            if (Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 8;
                }
                if (Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.EntityTextures[Graphics.EntityFileNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
                }
            }
            bgRect.FillColor = Color.Black;
            fgRect.FillColor = Color.Red;
            fgRect.Size = new Vector2f((float)Math.Ceiling((1f * Vital[(int)Enums.Vitals.Health] / MaxVital[(int)Enums.Vitals.Health]) * (width - 2)), 4f);
            bgRect.Position = new Vector2f(x - 1 - width/2, y - 1);
            fgRect.Position = new Vector2f(x - width/2, y);
            Graphics.RenderWindow.Draw(bgRect);
            Graphics.RenderWindow.Draw(fgRect);
        }
    }
}

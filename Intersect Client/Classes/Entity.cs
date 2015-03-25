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

        public void Update(bool local = false)
        {
            var didMove = false;
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
            else
            {
                if (local)
                {
                    if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
                    {
                        //Try to move
                        if (MoveTimer < Environment.TickCount)
                        {
                            switch (MoveDir)
                            {
                                case 0:
                                    if (!IsTileBlocked(CurrentX, CurrentY - 1, CurrentMap))
                                    {
                                        CurrentY--;
                                        Dir = 0;
                                        IsMoving = true;
                                        OffsetY = 32;
                                        OffsetX = 0;
                                    }
                                    break;
                                case 1:
                                    if (!IsTileBlocked(CurrentX, CurrentY + 1, CurrentMap))
                                    {
                                        CurrentY++;
                                        Dir = 1;
                                        IsMoving = true;
                                        OffsetY = -32;
                                        OffsetX = 0;
                                    }
                                    break;
                                case 2:
                                    if (!IsTileBlocked(CurrentX - 1, CurrentY, CurrentMap))
                                    {
                                        CurrentX--;
                                        Dir = 2;
                                        IsMoving = true;
                                        OffsetY = 0;
                                        OffsetX = 32;
                                    }
                                    break;
                                case 3:
                                    if (!IsTileBlocked(CurrentX + 1, CurrentY, CurrentMap))
                                    {
                                        CurrentX++;
                                        Dir = 3;
                                        IsMoving = true;
                                        OffsetY = 0;
                                        OffsetX = -32;
                                    }
                                    break;
                            }

                            if (IsMoving)
                            {
                                MoveTimer = Environment.TickCount + (Stat[(int)Enums.Stats.Speed] / 10f);
                                didMove = true;
                                if (CurrentX < 0 || CurrentY < 0 || CurrentX > 29 || CurrentY > 29)
                                {
                                    if (tmpI != -1)
                                    {
                                        try
                                        {
                                            //At each of these cases, we have switched chunks. We need to re-number the chunk renderers.
                                            if (CurrentX < 0)
                                            {

                                                if (CurrentY < 0)
                                                {
                                                    if (Globals.LocalMaps[tmpI - 4] > -1)
                                                    {
                                                        CurrentMap = Globals.LocalMaps[tmpI - 4];
                                                        CurrentX = 29;
                                                        CurrentY = 29;
                                                        UpdateMapRenderers(0);
                                                        UpdateMapRenderers(2);
                                                    }
                                                }
                                                else if (CurrentY > 29)
                                                {
                                                    if (Globals.LocalMaps[tmpI + 2] > -1)
                                                    {
                                                        CurrentMap = Globals.LocalMaps[tmpI + 2];
                                                        CurrentX = 29;
                                                        CurrentY = 0;
                                                        UpdateMapRenderers(1);
                                                        UpdateMapRenderers(2);
                                                    }

                                                }
                                                else
                                                {
                                                    if (Globals.LocalMaps[tmpI - 1] > -1)
                                                    {
                                                        CurrentMap = Globals.LocalMaps[tmpI - 1];
                                                        CurrentX = 29;
                                                        UpdateMapRenderers(2);
                                                    }
                                                }

                                            }
                                            else if (CurrentX > 29)
                                            {
                                                if (CurrentY < 0)
                                                {
                                                    if (Globals.LocalMaps[tmpI - 2] > -1)
                                                    {
                                                        CurrentMap = Globals.LocalMaps[tmpI - 2];
                                                        CurrentX = 0;
                                                        CurrentY = 29;
                                                        UpdateMapRenderers(0);
                                                        UpdateMapRenderers(3);
                                                    }
                                                }
                                                else if (CurrentY > 29)
                                                {
                                                    if (Globals.LocalMaps[tmpI + 4] > -1)
                                                    {
                                                        CurrentMap = Globals.LocalMaps[tmpI + 4];
                                                        CurrentX = 0;
                                                        CurrentY = 0;
                                                        UpdateMapRenderers(1);
                                                        UpdateMapRenderers(3);
                                                    }

                                                }
                                                else
                                                {
                                                    if (Globals.LocalMaps[tmpI + 1] > -1)
                                                    {
                                                        CurrentX = 0;
                                                        CurrentMap = Globals.LocalMaps[tmpI + 1];
                                                        UpdateMapRenderers(3);
                                                    }
                                                }
                                            }
                                            else if (CurrentY < 0)
                                            {
                                                if (Globals.LocalMaps[tmpI - 3] > -1)
                                                {
                                                    CurrentY = 29;
                                                    CurrentMap = Globals.LocalMaps[tmpI - 3];
                                                    UpdateMapRenderers(0);
                                                }
                                            }
                                            else if (CurrentY > 29)
                                            {
                                                if (Globals.LocalMaps[tmpI + 3] > -1)
                                                {
                                                    CurrentY = 0;
                                                    CurrentMap = Globals.LocalMaps[tmpI + 3];
                                                    UpdateMapRenderers(1);
                                                }

                                            }
                                        }
                                        catch (Exception)
                                        {
                                            //player out of bounds
                                            //Debug.Log("Detected player out of bounds.");
                                        }


                                    }
                                    else
                                    {
                                        //player out of bounds
                                        //.Log("Detected player out of bounds.");
                                    }
                                }
                            }
                            else
                            {
                                if (MoveDir != Dir)
                                {
                                    Dir = MoveDir;
                                    PacketSender.SendDir(Dir);
                                }
                            }
                        }
                    }
                }
            }

            if (local)
            {
                Globals.MyX = CurrentX;
                Globals.MyY = CurrentY;
                if (didMove)
                {
                    PacketSender.SendMove();
                }
            }
            _lastUpdate = Environment.TickCount;
        }

        bool IsTileBlocked(int x, int y, int map)
        {
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] != map) continue;
                tmpI = i;
                i = 9;
            }
            if (tmpI == -1)
            {
                return true;
            }
            try
            {
                int tmpX;
                int tmpY;
                int tmpMap;
                if (x < 0)
                {
                    tmpX = 29 - (x * -1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 4] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 4]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 2] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 2]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI - 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (x > 29)
                {
                    tmpX = x - 29;
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 2] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 2]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 4] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 4]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI + 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (y < 0)
                {
                    tmpX = x;
                    tmpY = 29 - (y * -1);
                    if (Globals.LocalMaps[tmpI - 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI - 3]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;

                        }
                        tmpMap = Globals.LocalMaps[tmpI - 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (y > 29)
                {
                    tmpX = x;
                    tmpY = y - 29;
                    if (Globals.LocalMaps[tmpI + 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI + 3]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;
                        }
                        tmpMap = Globals.LocalMaps[tmpI + 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                    if (Globals.LocalMaps[tmpI] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;
                        }
                        tmpMap = Globals.LocalMaps[tmpI];
                    }
                    else
                    {
                        return true;
                    }
                }
                for (var i = 0; i < Globals.Entities.Count; i++)
                {
                    if (i == MyIndex) continue;
                    if (Globals.Entities[i] == null) continue;
                    if (Globals.Entities[i].CurrentMap == tmpMap && Globals.Entities[i].CurrentX == tmpX && Globals.Entities[i].CurrentY == tmpY && Globals.Entities[i].Passable == 0)
                    {
                        return true;
                    }
                }
                for (var i = 0; i < Globals.Events.Count; i++)
                {
                    if (Globals.Events[i] != null)
                    {
                        if (Globals.Events[i].CurrentMap == tmpMap && Globals.Events[i].CurrentX == tmpX && Globals.Events[i].CurrentY == tmpY && Globals.Events[i].Passable == 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return true;

            }

        }

        private void UpdateMapRenderers(int dir)
        {
            if (!IsLocal)
            {
                return;
            }
            if (dir == 2)
            {
                if (Globals.LocalMaps[3] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[3];
                    Globals.LocalMaps[2] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = Globals.LocalMaps[0];
                    Globals.LocalMaps[0] = -1;
                    Globals.LocalMaps[5] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = -1;
                    Globals.LocalMaps[8] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = Globals.LocalMaps[6];
                    Globals.LocalMaps[6] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
            else if (dir == 3)
            {
                if (Globals.LocalMaps[5] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[5];
                    Globals.LocalMaps[0] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = Globals.LocalMaps[2];
                    Globals.LocalMaps[2] = -1;
                    Globals.LocalMaps[3] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = -1;
                    Globals.LocalMaps[6] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = Globals.LocalMaps[8];
                    Globals.LocalMaps[8] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    PacketSender.SendEnterMap();
                }

            }
            else if (dir == 1)
            {
                if (Globals.LocalMaps[7] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[7];
                    Globals.LocalMaps[0] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = Globals.LocalMaps[6];
                    Globals.LocalMaps[6] = -1;
                    Globals.LocalMaps[1] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = -1;
                    Globals.LocalMaps[2] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = Globals.LocalMaps[8];
                    Globals.LocalMaps[8] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
            else
            {
                if (Globals.LocalMaps[1] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[1];
                    Globals.LocalMaps[6] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = Globals.LocalMaps[0];
                    Globals.LocalMaps[0] = -1;
                    Globals.LocalMaps[7] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = -1;
                    Globals.LocalMaps[8] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = Globals.LocalMaps[2];
                    Globals.LocalMaps[2] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    PacketSender.SendEnterMap();
                }
            }
        }

        public void Draw(int i)
        {
            Rectangle srcRectangle = new Rectangle();
            Rectangle destRectangle = new Rectangle();
            Texture srcTexture;
            var d = 0;
            if (Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                srcTexture = Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")];
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
            if (Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                tmpSprite = new Sprite(Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")]);
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
            if (Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 14;
                }
                if (Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
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
            if (Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png") >= 0)
            {
                if (Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4 > 32)
                {
                    y = (int)Math.Ceiling(Graphics.CalcMapOffsetY(i) + CurrentY * 32 + OffsetY - ((Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4) - 32)) - 8;
                }
                if (Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.X / 4 > 32)
                {
                    x += (int)Math.Ceiling((Graphics.Entities[Graphics.EntityNames.IndexOf(MySprite.ToLower() + ".png")].Size.Y / 4f) - 32) / 2;
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

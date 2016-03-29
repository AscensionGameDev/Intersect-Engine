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
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Game_Objects;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Spells;
using GameGraphics = Intersect_Client.Classes.Core.GameGraphics;

// ReSharper disable All

namespace Intersect_Client.Classes.Entities
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

        //Combat Status
        public long CastTime = 0;
        public int SpellCast = 0;

        //Inventory/Spells/Equipment
        public ItemInstance[] Inventory = new ItemInstance[Options.MaxInvItems];
        public SpellInstance[] Spells = new SpellInstance[Options.MaxPlayerSkills];
        public int[] Equipment = new int[Options.EquipmentSlots.Count];

        //Entity Animations
        public List<AnimationInstance> Animations = new List<AnimationInstance>();

        private long _lastUpdate;
        private long _walkTimer;
        public int WalkFrame;

        private bool _disposed;

        //Rendering Variables
        public List<Entity> RenderList = null;

        public Entity()
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Inventory[i] = new ItemInstance();
            }
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Spells[i] = new SpellInstance();
            }

        }

        //Deserializing
        public void Load(ByteBuffer bf)
        {
            CurrentMap = bf.ReadInteger();
            MyName = bf.ReadString();
            MySprite = bf.ReadString();
            Face = bf.ReadString();
            CurrentX = bf.ReadInteger();
            CurrentY = bf.ReadInteger();
            CurrentZ = bf.ReadInteger();
            ClearAnimations();
            int animCount = bf.ReadInteger();
            for (int i = 0; i < animCount; i++)
            {
                Animations.Add(new AnimationInstance(Globals.GameAnimations[bf.ReadInteger()], true));
            }
        }

        public void ClearAnimations()
        {
            if (Animations.Count > 0)
            {
                for (int i = 0; i < Animations.Count; i++)
                {
                    Animations[i].Dispose();
                }
                Animations.Clear();
            }
        }

        public virtual void Dispose()
        {
            if (RenderList != null)
            {
                RenderList.Remove(this);
            }
            ClearAnimations();
            _disposed = true;
        }

        //Movement Processing
        public virtual bool Update()
        {
            if (_disposed)
            {
                return false;
            }
            RenderList = DetermineRenderOrder(RenderList);
            if (_lastUpdate == 0) { _lastUpdate = Globals.System.GetTimeMS(); }
            float ecTime = (float)(Globals.System.GetTimeMS() - _lastUpdate);
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
            if (_walkTimer < Globals.System.GetTimeMS())
            {
                if (IsMoving)
                {
                    WalkFrame++;
                    if (WalkFrame > 3) { WalkFrame = 0; }
                }
                else
                {
                    WalkFrame = 0;
                }
                _walkTimer = Globals.System.GetTimeMS() + 200;
            }
            if (IsMoving)
            {

                switch (Dir)
                {
                    case 0:
                        OffsetY -= (float)ecTime * (40f / 10f * (float)Options.TileHeight) / 1000f;
                        OffsetX = 0;
                        if (OffsetY < 0) { OffsetY = 0; }
                        break;

                    case 1:
                        OffsetY += (float)ecTime * (40f / 10f * (float)Options.TileHeight) / 1000f;
                        OffsetX = 0;
                        if (OffsetY > 0) { OffsetY = 0; }
                        break;

                    case 2:
                        OffsetX -= (float)ecTime * (40f / 10f * (float)Options.TileHeight) / 1000f;
                        OffsetY = 0;
                        if (OffsetX < 0) { OffsetX = 0; }
                        break;

                    case 3:
                        OffsetX += (float)ecTime * (40f / 10f * (float)Options.TileHeight) / 1000f;
                        OffsetY = 0;
                        if (OffsetX > 0) { OffsetX = 0; }
                        break;
                }
                if (OffsetX == 0 && OffsetY == 0)
                {
                    IsMoving = false;
                }
            }
            foreach (AnimationInstance animInstance in Animations)
            {
                animInstance.Update();
                if (animInstance.AutoRotate)
                {
                    animInstance.SetPosition((int)GetCenterPos().X, (int)GetCenterPos().Y, Dir);
                }
                else
                {
                    animInstance.SetPosition((int)GetCenterPos().X, (int)GetCenterPos().Y, -1);
                }

            }
            _lastUpdate = Globals.System.GetTimeMS();
            return true;
        }

        public virtual List<Entity> DetermineRenderOrder(List<Entity> renderList)
        {
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
                    if (i < 3)
                    {
                        outerList[CurrentY].Add(this);
                        renderList = outerList[CurrentY];
                    }
                    else if (i < 6)
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
            return renderList;
        }

        //Rendering Functions
        public virtual void Draw()
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture srcTexture;
            var d = 0;
            if (GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                srcTexture = GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())];
                if (srcTexture.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX);
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY - ((srcTexture.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX;
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (srcTexture.GetWidth() / 4 > Options.TileWidth)
                {
                    destRectangle.X -= ((srcTexture.GetWidth() / 4) - Options.TileWidth) / 2;
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
                destRectangle.X = (int)Math.Ceiling(destRectangle.X);
                destRectangle.Y = (int)Math.Ceiling(destRectangle.Y);
                srcRectangle = new FloatRect(WalkFrame * (int)srcTexture.GetWidth() / 4, d * (int)srcTexture.GetHeight() / 4, (int)srcTexture.GetWidth() / 4, (int)srcTexture.GetHeight() / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);

                //Draw the equipment/paperdolls
                for (int z = 0; z < Options.PaperdollOrder.Count; z++)
                {
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z]) > -1)
                    {
                        if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])] > -1 && Inventory[Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]].ItemNum > -1)
                        {
                            DrawEquipment(Globals.GameItems[Inventory[Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]].ItemNum].Paperdoll);
                        }
                    }
                }

            }
        }

        public virtual void DrawEquipment(string filename)
        {
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap)) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture srcTexture;
            var d = 0;
            if (GameGraphics.PaperdollFileNames.IndexOf(filename.ToLower()) >= 0)
            {
                srcTexture = GameGraphics.PaperdollTextures[GameGraphics.PaperdollFileNames.IndexOf(filename)];
                if (srcTexture.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX);
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY - ((srcTexture.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX;
                    destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (srcTexture.GetWidth() / 4 > Options.TileWidth)
                {
                    destRectangle.X -= ((srcTexture.GetWidth() / 4) - Options.TileWidth) / 2;
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
                destRectangle.X = (int)Math.Ceiling(destRectangle.X);
                destRectangle.Y = destRectangle.Y;
                srcRectangle = new FloatRect(WalkFrame * (int)srcTexture.GetWidth() / 4, d * (int)srcTexture.GetHeight() / 4, (int)srcTexture.GetWidth() / 4, (int)srcTexture.GetHeight() / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);
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
        public virtual Pointf GetCenterPos()
        {
            if (!Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return new Pointf(0, 0);
            }
            Pointf pos = new Pointf(Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                    Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            if (GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                pos.Y += Options.TileHeight / 2;
                pos.Y -= GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetHeight() / 4 / 2;
            }
            return pos;
        }
        public virtual void DrawName()
        {
            if (HideName == 1) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            if (GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                y = y - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetHeight() / 8));
                y -= 6;
                if (GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4 > Options.TileWidth)
                {
                    x = x - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4) - Options.TileWidth) / 2;
                }
            }
            if (this.GetType() != typeof(Event)) { y -= 10; } //Need room for HP bar if not an event.

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 10).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 10, Color.White);
        }
        public void DrawHpBar()
        {
            if (HideName == 1 && Vital[(int)Enums.Vitals.Health] == MaxVital[(int)Enums.Vitals.Health]) { return; }
            if (Vital[(int)Enums.Vitals.Health] <= 0) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
            var width = Options.TileWidth;
            var fillWidth = ((float)Vital[(int)Enums.Vitals.Health] / MaxVital[(int)Enums.Vitals.Health]) * width;
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            if (GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                y = y - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetHeight() / 8));

                if (GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4 > Options.TileWidth)
                {
                    x = x - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4) - Options.TileWidth) / 2;
                }
            }

            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - 1 - width / 2), (int)(y - 1), width, 6), Color.Black);
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - width / 2), (int)(y), fillWidth - 2, 4), Color.Red);
        }
        public void DrawCastingBar()
        {
            if (CastTime < Globals.System.GetTimeMS()) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap))
            {
                return;
            }
            var width = Options.TileWidth;
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            if (GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower()) >= 0)
            {
                y = y - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetHeight() / 8));
                y -= 18;

                if (GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4 > Options.TileWidth)
                {
                    x = x - (int)((GameGraphics.EntityTextures[GameGraphics.EntityFileNames.IndexOf(MySprite.ToLower())].GetWidth() / 4) - Options.TileWidth) / 2;
                }
            }

            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - 1 - width / 2), (int)(y - 1), width, 6), Color.Black);
            GameGraphics.DrawGameTexture(GameGraphics.WhiteTex, new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - width / 2), (int)(y), width - 2, 4), Color.White);
        }

        //
        public void DrawTarget(int Priority)
        {
            if (this.GetType() == typeof(Projectile)) return;
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || !Globals.GameMaps.ContainsKey(CurrentMap)) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture srcTexture = GameGraphics.TargetTexture;

            destRectangle.X = Globals.GameMaps[CurrentMap].GetX() + CurrentX * Options.TileWidth + OffsetX;
            destRectangle.Y = Globals.GameMaps[CurrentMap].GetY() + CurrentY * Options.TileHeight + OffsetY;
            destRectangle.X = (int)Math.Ceiling(destRectangle.X - (int)srcTexture.GetWidth() / 8);

            srcRectangle = new FloatRect(Priority * (int)srcTexture.GetWidth() / 2, 0, (int)srcTexture.GetWidth() / 2, (int)srcTexture.GetHeight());
            destRectangle.Width = srcRectangle.Width;
            destRectangle.Height = srcRectangle.Height;

            GameGraphics.DrawGameTexture(srcTexture, srcRectangle, destRectangle, Color.White);
        }

        ~Entity()
        {
            Dispose();
        }
    }
}
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
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Spells;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Color = IntersectClientExtras.GenericClasses.Color;
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
        public bool IsPlayer = false;

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
        public int[] MaxVital = new int[(int)Vitals.VitalCount];
        public int[] Vital = new int[(int)Vitals.VitalCount];
        public int[] Stat = new int[(int)Stats.StatCount];

        //Combat Status
        public long CastTime = 0;
        public int SpellCast = 0;

        //Inventory/Spells/Equipment
        public ItemInstance[] Inventory = new ItemInstance[Options.MaxInvItems];
        public SpellInstance[] Spells = new SpellInstance[Options.MaxPlayerSkills];
        public int[] Equipment = new int[Options.EquipmentSlots.Count];

        //Entity Animations
        public List<AnimationInstance> Animations = new List<AnimationInstance>();

        //Status effects
        public List<StatusInstance> Status = new List<StatusInstance>();

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
        public virtual void Load(ByteBuffer bf)
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
                var anim = AnimationBase.GetAnim(bf.ReadInteger());
                if (anim != null)
                    Animations.Add(new AnimationInstance(anim, true));
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

            if (MapInstance.GetMap(CurrentMap) == null)
            {
                return null;
            }
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
            if (i == -1 || MapInstance.GetMap(CurrentMap) == null)
            {
                return;
            }
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            var d = 0;

            string sprite = MySprite;
            int alpha = 255;

            //If the entity has transformed, apply that sprite instead.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int)StatusTypes.Transform)
                {
                    sprite = Status[n].Data;
                }
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int)StatusTypes.Stealth)
                {
                    if (Globals.MyIndex != MyIndex)
                    {
                        return;
                    }
                    else
                    {
                        alpha = 125;
                    }
                }
            }

            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, sprite);
            if (entityTex != null)
            {
                var map = MapInstance.GetMap(CurrentMap);
                if (entityTex.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (map.GetX() + CurrentX * Options.TileWidth + OffsetX);
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY - ((entityTex.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX;
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (entityTex.GetWidth() / 4 > Options.TileWidth)
                {
                    destRectangle.X -= ((entityTex.GetWidth() / 4) - Options.TileWidth) / 2;
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
                srcRectangle = new FloatRect(WalkFrame * (int)entityTex.GetWidth() / 4, d * (int)entityTex.GetHeight() / 4, (int)entityTex.GetWidth() / 4, (int)entityTex.GetHeight() / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(entityTex, srcRectangle, destRectangle, new Color(alpha, 255, 255, 255));

                //Don't render the paperdolls if they have transformed.
                if (sprite == MySprite)
                {
                    //Draw the equipment/paperdolls
                    for (int z = 0; z < Options.PaperdollOrder.Count; z++)
                    {
                        if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z]) > -1)
                        {
                            if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])] > -1 && ItemBase.GetItem(Inventory[Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]].ItemNum) != null)
                            {
                                DrawEquipment(ItemBase.GetItem(Inventory[Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]].ItemNum).Paperdoll, alpha);
                            }
                        }
                    }
                }

            }
        }

        public virtual void DrawEquipment(string filename, int alpha)
        {
            int i = GetLocalPos(CurrentMap);
            var map = MapInstance.GetMap(CurrentMap);
            if (i == -1 || map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            var d = 0;
            GameTexture paperdollTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll, filename);
            if (paperdollTex != null)
            {
                if (paperdollTex.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (map.GetX() + CurrentX * Options.TileWidth + OffsetX);
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY - ((paperdollTex.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX;
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                if (paperdollTex.GetWidth() / 4 > Options.TileWidth)
                {
                    destRectangle.X -= ((paperdollTex.GetWidth() / 4) - Options.TileWidth) / 2;
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
                srcRectangle = new FloatRect(WalkFrame * (int)paperdollTex.GetWidth() / 4, d * (int)paperdollTex.GetHeight() / 4, (int)paperdollTex.GetWidth() / 4, (int)paperdollTex.GetHeight() / 4);
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(paperdollTex, srcRectangle, destRectangle, new Color(alpha, 255, 255, 255));
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
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
            {
                return new Pointf(0, 0);
            }
            Pointf pos = new Pointf(map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                    map.GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                pos.Y += Options.TileHeight / 2;
                pos.Y -= entityTex.GetHeight() / 4 / 2;
                pos.X += entityTex.GetWidth()/8 - (Options.TileWidth /2);
            }
            return pos;
        }
        public virtual void DrawName()
        {
            if (HideName == 1) { return; }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int)StatusTypes.Stealth)
                {
                    if (Globals.MyIndex != MyIndex)
                    {
                        return;
                    }
                }
            }

            int i = GetLocalPos(CurrentMap);
            var map = MapInstance.GetMap(CurrentMap);
            if (i == -1 || map == null)
            {
                return;
            }
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                y = y - (int)((entityTex.GetHeight() / 8));
                y -= 12;
                if (entityTex.GetWidth() / 4 > Options.TileWidth)
                {
                    x = x - (int)((entityTex.GetWidth() / 4) - Options.TileWidth) / 2;
                }
            }
            if (this.GetType() != typeof(Event)) { y -= 10; } //Need room for HP bar if not an event.

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 1).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 1, Color.White);
        }
        public void DrawHpBar()
        {
            if (HideName == 1 && Vital[(int)Vitals.Health] == MaxVital[(int)Vitals.Health]) { return; }
            if (Vital[(int)Vitals.Health] <= 0) { return; }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int)StatusTypes.Stealth)
                {
                    if (Globals.MyIndex != MyIndex)
                    {
                        return;
                    }
                }
            }

            int i = GetLocalPos(CurrentMap);
            var map = MapInstance.GetMap(CurrentMap);
            if (i == -1 || map == null)
            {
                return;
            }
            var width = Options.TileWidth;
            var fillWidth = ((float)Vital[(int)Vitals.Health] / MaxVital[(int)Vitals.Health]) * width;
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            var x = (int)Math.Ceiling(GetCenterPos().X);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                y = y - (int)((entityTex.GetHeight() / 8));
                y -= 8;
                if (entityTex.GetWidth() / 4 > Options.TileWidth)
                {
                    x = x - (int)((entityTex.GetWidth() / 4) - Options.TileWidth) / 2;
                }
            }

            GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - 1 - width / 2), (int)(y - 1), width, 6), Color.Black);
            GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                new FloatRect((int)(x - width / 2), (int)(y), fillWidth - 2, 4), Color.Red);
        }
        public void DrawCastingBar()
        {
            if (CastTime < Globals.System.GetTimeMS()) { return; }
            int i = GetLocalPos(CurrentMap);
            if (i == -1 || MapInstance.GetMap(CurrentMap) == null)
            {
                return;
            }
            var castSpell = SpellBase.GetSpell(SpellCast);
            if (castSpell != null)
            {
                var width = Options.TileWidth;
                var fillWidth = ((castSpell.CastDuration*100 -
                                  (CastTime - Globals.System.GetTimeMS()))/
                                 (float) (castSpell.CastDuration*100)*width);
                var y = (int) Math.Ceiling(GetCenterPos().Y);
                var x = (int) Math.Ceiling(GetCenterPos().X);
                GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                    MySprite);
                if (entityTex != null)
                {
                    y = y + (int) ((entityTex.GetHeight()/8));
                    y += 3;

                    if (entityTex.GetWidth()/4 > Options.TileWidth)
                    {
                        x = x - (int) ((entityTex.GetWidth()/4) - Options.TileWidth)/2;
                    }
                }

                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((int) (x - 1 - width/2), (int) (y - 1), width, 6), Color.Black);
                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((int) (x - width/2), (int) (y), fillWidth - 2, 4), new Color(255, 0, 255, 255));
            }
        }

        //
        public void DrawTarget(int Priority)
        {
            if (this.GetType() == typeof(Projectile)) return;
            int i = GetLocalPos(CurrentMap);
            var map = MapInstance.GetMap(CurrentMap);
            if (i == -1 || map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture targetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "target.png");
            if (targetTex != null)
            {
                destRectangle.X = map.GetX() + CurrentX*Options.TileWidth + OffsetX;
                destRectangle.Y = map.GetY() + CurrentY*Options.TileHeight + OffsetY;
                destRectangle.X = (int) Math.Ceiling(destRectangle.X - (int)targetTex.GetWidth()/8);

                srcRectangle = new FloatRect(Priority*(int)targetTex.GetWidth()/2, 0, (int)targetTex.GetWidth()/2,
                    (int)targetTex.GetHeight());
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;

                GameGraphics.DrawGameTexture(targetTex, srcRectangle, destRectangle, Color.White);
            }
        }

        ~Entity()
        {
            Dispose();
        }
    }

    public class StatusInstance
    {
        public int Type = -1;
        public string Data = "";

        public StatusInstance(int type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}
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
using Intersect_Client.Classes.Networking;
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
        public int Gender = 0;

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

        //Action Msg's
        public List<ActionMsgInstance> ActionMsgs = new List<ActionMsgInstance>();

        //Dashing instance
        public DashInstance Dashing = null;
        public Queue<DashInstance> DashQueue = new Queue<DashInstance>();

        //Combat
        public long AttackTimer = 0;
        public bool Blocking = false;

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
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = -1;
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

        public virtual bool IsDisposed()
        {
            return _disposed;
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

        //Returns the amount of time required to traverse 1 tile
        public virtual float GetMovementTime()
        {
            var time = 1000f/(float) (1 + Math.Log(Stat[(int) Stats.Speed]));
            if (Blocking) { time += time * (float)Options.BlockingSlow; }
            if (time > 1000f) time = 1000f;
            return time;
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
            if (Dashing != null)
            {
                WalkFrame = 1; //Fix the frame whilst dashing
            }
            else if (_walkTimer < Globals.System.GetTimeMS())
            {
                if (DashQueue.Count > 0)
                {
                    Dashing = DashQueue.Dequeue();
                    Dashing.Start();
                }
                else
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
            }
            if (Dashing != null)
            {
                if (Dashing.Update())
                {
                    OffsetX = Dashing.GetXOffset();
                    OffsetY = Dashing.GetYOffset();
                }
                else
                {
                    OffsetX = 0;
                    OffsetY = 0;
                }
            }
            else if (IsMoving)
            {

                switch (Dir)
                {
                    case 0:
                        OffsetY -= (float)ecTime * ((float)Options.TileHeight) / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY < 0) { OffsetY = 0; }
                        break;

                    case 1:
                        OffsetY += (float)ecTime * ((float)Options.TileHeight) / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY > 0) { OffsetY = 0; }
                        break;

                    case 2:
                        OffsetX -= (float)ecTime * ((float)Options.TileHeight) / GetMovementTime();
                        OffsetY = 0;
                        if (OffsetX < 0) { OffsetX = 0; }
                        break;

                    case 3:
                        OffsetX += (float)ecTime * ((float)Options.TileHeight) / GetMovementTime();
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
                if (IsStealthed())
                {
                    animInstance.Hide();
                }
                else
                {
                    animInstance.Show();
                }
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

        public int CalculateAttackTime()
        {
            return (int)(Options.MaxAttackRate + (float)((Options.MinAttackRate - Options.MaxAttackRate) * (((float)Options.MaxStatValue - Stat[(int)Stats.Speed]) / (float)Options.MaxStatValue)));
        }

        public virtual bool IsStealthed()
        {
            //If the entity has transformed, apply that sprite instead.
            if (this == Globals.Me) return false;
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int)StatusTypes.Stealth)
                {
                    return true;
                }
            }
            return false;
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
                if (AttackTimer - CalculateAttackTime()/2 > Environment.TickCount || Blocking)
                {
                    srcRectangle = new FloatRect(3 * (int)entityTex.GetWidth() / 4, d * (int)entityTex.GetHeight() / 4, (int)entityTex.GetWidth() / 4, (int)entityTex.GetHeight() / 4);
                }
                else
                {
                    srcRectangle = new FloatRect(WalkFrame * (int)entityTex.GetWidth() / 4, d * (int)entityTex.GetHeight() / 4, (int)entityTex.GetWidth() / 4, (int)entityTex.GetHeight() / 4);
                }
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
                                if (Gender == 0)
                                {
                                    DrawEquipment(
                                        ItemBase.GetItem(
                                            Inventory[
                                                Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]]
                                                .ItemNum).MalePaperdoll, alpha);
                                }
                                else
                                {
                                    DrawEquipment(
                                        ItemBase.GetItem(
                                            Inventory[
                                                Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]]
                                                .ItemNum).FemalePaperdoll, alpha);
                                }
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
                    destRectangle.Y = GetCenterPos().Y - paperdollTex.GetHeight() / 8;
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
                if (AttackTimer > Environment.TickCount || Blocking)
                {
                    srcRectangle = new FloatRect(3 * (int)paperdollTex.GetWidth() / 4, d * (int)paperdollTex.GetHeight() / 4, (int)paperdollTex.GetWidth() / 4, (int)paperdollTex.GetHeight() / 4);
                }
                else
                {
                    srcRectangle = new FloatRect(WalkFrame * (int)paperdollTex.GetWidth() / 4, d * (int)paperdollTex.GetHeight() / 4, (int)paperdollTex.GetWidth() / 4, (int)paperdollTex.GetHeight() / 4);
                }
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
            }
            if (this.GetType() != typeof(Event)) { y -= 10; } //Need room for HP bar if not an event.

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 1).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 1, Color.White);
        }

        public void DrawActionMsgs()
        {
            int i = GetLocalPos(CurrentMap);
            var map = MapInstance.GetMap(CurrentMap);
            if (i == -1 || map == null)
            {
                return;
            }

            for (int n = ActionMsgs.Count - 1; n > -1; n--)
            {
                var y = (int)Math.Ceiling(GetCenterPos().Y - ((Options.TileHeight * 2) * (1000 - (ActionMsgs[n].TransmittionTimer - Globals.System.GetTimeMS())) / 1000));
                var x = (int)Math.Ceiling(GetCenterPos().X + ActionMsgs[n].xOffset);
                float textWidth = GameGraphics.Renderer.MeasureText(ActionMsgs[n].msg, GameGraphics.GameFont, 1).X;
                GameGraphics.Renderer.DrawString(ActionMsgs[n].msg, GameGraphics.GameFont, (int)(x) - textWidth/2f, (int)(y), 1, ActionMsgs[n].clr);

                //Try to remove
                ActionMsgs[n].TryRemove();
            }
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

    public class ActionMsgInstance
    {
        public Entity Entity = null;
        public string msg = "";
        public Color clr = new Color();
        public long TransmittionTimer = 0;
        public long xOffset = 0;

        public ActionMsgInstance(Entity entity, string message, Color color)
        {
            Random rnd = new Random();

            Entity = entity;
            msg = message;
            clr = color;
            xOffset = rnd.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
            TransmittionTimer = Globals.System.GetTimeMS() + 1000;
        }

        public void TryRemove()
        {
            if (TransmittionTimer <= Globals.System.GetTimeMS())
            {
                Entity.ActionMsgs.Remove(this);
            }
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

    public class DashInstance
    {
        public int EntityID = 0;
        public int Range = 0;
        public int DistanceTraveled = 0;
        public long TransmittionTimer = 0;
        public long SpawnTime = 0;
        public float XOffset = 0f;
        public float YOffset = 0f;
        public int Direction = 0;
        public bool ChangeDirection = false;

        public DashInstance(int entityID, int range, int direction, bool changeDirection = true)
        {
            EntityID = entityID;
            DistanceTraveled = 0;
            Range = range;
            Direction = direction;
            ChangeDirection = changeDirection;
        }

        public void Start()
        {
            if (Range <= 0) { Globals.Entities[EntityID].Dashing = null; } //Remove dash instance if no where to dash
            TransmittionTimer = Globals.System.GetTimeMS() + (long)((float)Options.MaxDashSpeed / (float)Range);
            SpawnTime = Globals.System.GetTimeMS();
            if (ChangeDirection) Globals.Entities[EntityID].Dir = Direction;
        }

        public float GetXOffset()
        {
            if (Direction == 2){return -XOffset; }
            if (Direction == 3) { return XOffset; }
            return 0;
        }

        public float GetYOffset()
        {
            if (Direction == 0) { return -YOffset; }
            if (Direction == 1) { return YOffset; }
            return 0;
        }

        public bool Update()
        {
            if (Globals.System.GetTimeMS() > TransmittionTimer)
            {
                switch (Direction)
                {
                    case 0:
                        Globals.Entities[EntityID].CurrentY--;
                        break;
                    case 1:
                        Globals.Entities[EntityID].CurrentY++;
                        break;
                    case 2:
                        Globals.Entities[EntityID].CurrentX--;
                        break;
                    case 3:
                        Globals.Entities[EntityID].CurrentX++;
                        break;
                }

                if (Globals.Entities[EntityID].CurrentX < 0)
                {
                    if (MapInstance.GetMap(MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Left) != null)
                    {
                        Globals.Entities[EntityID].CurrentMap = MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Left;
                        Globals.Entities[EntityID].CurrentX = Options.MapWidth - 1;
                        if (Globals.Entities[EntityID] == Globals.Me)
                        {
                            Globals.Me.UpdateMapRenderers((int)Directions.Left);
                        }
                    }
                    else
                    {
                        Globals.Entities[EntityID].Dashing = null;
                        Globals.Entities[EntityID].CurrentX = 0;
                    }
                }
                if (Globals.Entities[EntityID].CurrentX > Options.MapWidth - 1)
                {
                    if (MapInstance.GetMap(MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Right) != null)
                    {
                        Globals.Entities[EntityID].CurrentMap = MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Right;
                        Globals.Entities[EntityID].CurrentX = 0;
                        if (Globals.Entities[EntityID] == Globals.Me)
                        {
                            Globals.Me.UpdateMapRenderers((int)Directions.Right);
                        }
                    }
                    else
                    {
                        Globals.Entities[EntityID].Dashing = null;
                        Globals.Entities[EntityID].CurrentX = Options.MapWidth - 1;
                    }
                }
                if (Globals.Entities[EntityID].CurrentY < 0)
                {
                    if (MapInstance.GetMap(MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Up) != null)
                    {
                        Globals.Entities[EntityID].CurrentMap = MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Up;
                        Globals.Entities[EntityID].CurrentY = Options.MapHeight - 1;
                        if (Globals.Entities[EntityID] == Globals.Me)
                        {
                            Globals.Me.UpdateMapRenderers((int)Directions.Up);
                        }
                    }
                    else
                    {
                        Globals.Entities[EntityID].Dashing = null;
                        Globals.Entities[EntityID].CurrentY = 0;
                    }
                }
                if (Globals.Entities[EntityID].CurrentY > Options.MapHeight - 1)
                {
                    if (MapInstance.GetMap(MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Down) != null)
                    {
                        Globals.Entities[EntityID].CurrentMap = MapInstance.GetMap(Globals.Entities[EntityID].CurrentMap).Down;
                        Globals.Entities[EntityID].CurrentY = 0;
                        if (Globals.Entities[EntityID] == Globals.Me)
                        {
                            Globals.Me.UpdateMapRenderers((int)Directions.Down);
                        }
                    }
                    else
                    {
                        Globals.Entities[EntityID].Dashing = null;
                        Globals.Entities[EntityID].CurrentY = Options.MapHeight - 1;
                    }
                }

                TransmittionTimer = Globals.System.GetTimeMS() + (long)(Options.MaxDashSpeed / (float)Range);
                DistanceTraveled++;
            }
            XOffset = Options.TileWidth * (((Options.MaxDashSpeed / (float)Range) - (TransmittionTimer - Globals.System.GetTimeMS())) / (Options.MaxDashSpeed / (float)Range));
            YOffset = Options.TileHeight * (((Options.MaxDashSpeed / (float)Range) - (TransmittionTimer - Globals.System.GetTimeMS())) / (Options.MaxDashSpeed / (float)Range));
            if (DistanceTraveled >= Range)
            {
                Globals.Entities[EntityID].Dashing = null;
                Globals.Entities[EntityID].OffsetX = 0;
                Globals.Entities[EntityID].OffsetY = 0;
            } //Dash no more once reached destination
            return Globals.Entities[EntityID].Dashing != null;
        }
    }
}
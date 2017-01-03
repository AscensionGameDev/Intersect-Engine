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
namespace Intersect_Client.Classes.Entities
{
    public class Entity
    {
        //Core Values
        public int MyIndex;
        public long SpawnTime;
        public bool IsLocal = false;
        public string MyName = "";
        public string MySprite = "";
        public bool InView = true;
        public int Passable = 0;
        public int HideName = 0;
        public int Level = 1;
        public int Gender = 0;
        public int Target = -1;

        //Extras
        public string Face = "";

        //Location Info
        public int CurrentX;
        public int CurrentY;
        public int CurrentZ = 0;
        public virtual int CurrentMap { get; set; } = -1;
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
        public long DashTimer = 0;

        //Chat
        private List<ChatBubble> _chatBubbles = new List<ChatBubble>();

        //Combat
        public long AttackTimer = 0;
        public bool Blocking = false;

        private long _lastUpdate;
        private long _walkTimer;
        public int WalkFrame;

        private bool _disposed;

        //Rendering Variables
        public List<Entity> RenderList = null;
        public int type = 0;

        public Entity(int index, long spawnTime, ByteBuffer bf)
        {
            SpawnTime = spawnTime;
            if (index > -1)
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
                MyIndex = index;
                Load(bf);
            }
        }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
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
            Dir = bf.ReadInteger();
            Passable = bf.ReadInteger();
            HideName = bf.ReadInteger();
            ClearAnimations();
            int animCount = bf.ReadInteger();
            for (int i = 0; i < animCount; i++)
            {
                var anim = AnimationBase.GetAnim(bf.ReadInteger());
                if (anim != null)
                    Animations.Add(new AnimationInstance(anim, true));
            }
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                MaxVital[i] = bf.ReadInteger();
                Vital[i] = bf.ReadInteger();
            }
            //Update status effects
            var count = bf.ReadInteger();
            Status.Clear();
            for (int i = 0; i < count; i++)
            {
                Status.Add(new StatusInstance(bf.ReadInteger(), bf.ReadString()));
            }
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                Stat[i] = bf.ReadInteger();
            }
            type = bf.ReadInteger();

            _disposed = false;
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
            else
            {
                if (MapInstance.GetMap(CurrentMap) == null || !MapInstance.GetMap(CurrentMap).InView())
                {
                    Globals.EntitiesToDispose.Add(MyIndex);
                    return false;
                }
            }
            RenderList = DetermineRenderOrder(RenderList);
            if (_lastUpdate == 0) { _lastUpdate = Globals.System.GetTimeMS(); }
            float ecTime = (float)(Globals.System.GetTimeMS() - _lastUpdate);
            if (Dashing != null)
            {
                WalkFrame = 1; //Fix the frame whilst dashing
            }
            else if (_walkTimer < Globals.System.GetTimeMS())
            {
                if (!IsMoving && DashQueue.Count > 0)
                {
                    Dashing = DashQueue.Dequeue();
                    Dashing.Start(this);
                    OffsetX = 0;
                    OffsetY = 0;
                    DashTimer = Globals.System.GetTimeMS() + Options.MaxDashSpeed;
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
                if (Dashing.Update(this))
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
            var chatbubbles = _chatBubbles.ToArray();
            foreach (var chatbubble in chatbubbles)
            {
                if (!chatbubble.Update())
                {
                    _chatBubbles.Remove(chatbubble);
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

            if (MapInstance.GetMap(Globals.Me.CurrentMap) == null)
            {
                return null;
            }
            var gridX = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridX;
            var gridY = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight && Globals.MapGrid[x, y] != -1)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
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
                            if (y == gridY-1)
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

        //Rendering Functions
        public virtual void Draw()
        {
            if (MapInstance.GetMap(CurrentMap) == null) return;
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
                    if (this != Globals.Me)
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
                    destRectangle.X = (map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth/2);
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY - ((entityTex.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2;
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                destRectangle.X -= ((entityTex.GetWidth()/8));
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
                if (AttackTimer - CalculateAttackTime()/2 > Globals.System.GetTimeMS() || Blocking)
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
                            if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])] > -1)
                            {
                                var itemNum = -1;
                                if (this == Globals.Me)
                                {
                                    itemNum =
                                        Inventory[Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])]]
                                            .ItemNum;
                                }
                                else
                                {
                                    itemNum = Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[z])];
                                }
                                if (ItemBase.GetItem(itemNum) != null)
                                {
                                    if (Gender == 0)
                                    {
                                        DrawEquipment(
                                            ItemBase.GetItem(itemNum).MalePaperdoll, alpha);
                                    }
                                    else
                                    {
                                        DrawEquipment(
                                            ItemBase.GetItem(itemNum).FemalePaperdoll, alpha);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            var chatbubbles = _chatBubbles.ToArray();
            var bubbleoffset = 0f;
            for (int i = chatbubbles.Length - 1; i > -1; i--)
            {
                bubbleoffset = chatbubbles[i].Draw(bubbleoffset);
            }
        }

        public virtual void DrawEquipment(string filename, int alpha)
        {
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null) return;
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
                if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMS() || Blocking)
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

        public virtual float GetTopPos()
        {
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
            {
                return 0f;
            }
            var y = (int)Math.Ceiling(GetCenterPos().Y);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                y = y - (int)((entityTex.GetHeight() / 8));
                y -= 12;
            }
            if (this.GetType() != typeof(Event)) { y -= 10; } //Need room for HP bar if not an event.
            return y;
        }
        public virtual void DrawName(Color color)
        {
            if (HideName == 1) { return; }

            //Check for npc colors
            if (color == null)
            {
                switch (type)
                {
                    case -1: //When entity has a target (showing aggression)
                        color = Color.Red;
                        break;
                    case 0: //Attack when attacked
                        color = new Color(128, 128, 128); // Gray
                        break;
                    case 1: //Attack on sight
                        color = new Color(128, 0, 0); //Maroon
                        break;
                    case 2: //Neutral
                        color = Color.White;
                        break;
                    case 3: //Guard
                        color = Color.Black;
                        break;
                    default:
                        color = Color.White;
                        break;
                }
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int)StatusTypes.Stealth)
                {
                    if (this != Globals.Me)
                    {
                        return;
                    }
                }
            }
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
            {
                return;
            }
            var y = GetTopPos();
            var x = (int)Math.Ceiling(GetCenterPos().X);

            float textWidth = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 1).X;
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int)(x - (int)Math.Ceiling(textWidth / 2)), (int)(y), 1, color);
        }

        public void DrawActionMsgs()
        {
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
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
                    if (this != Globals.Me)
                    {
                        return;
                    }
                }
            }

            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
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
            if (MapInstance.GetMap(CurrentMap) == null)
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
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture targetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "target.png");
            if (targetTex != null)
            {
                destRectangle.X = GetCenterPos().X - (int)targetTex.GetWidth() / 4;
                destRectangle.Y = GetCenterPos().Y - (int)targetTex.GetHeight()/2;

                srcRectangle = new FloatRect(Priority*(int)targetTex.GetWidth()/2, 0, (int)targetTex.GetWidth()/2,
                    (int)targetTex.GetHeight());
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;

                GameGraphics.DrawGameTexture(targetTex, srcRectangle, destRectangle, Color.White);
            }
        }

        //Chatting
        public void AddChatBubble(string text)
        {
            _chatBubbles.Add(new ChatBubble(this,text));
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
        private int _changeDirection = -1;
        private float _startXCoord;
        private float _startYCoord;
        private float _endXCoord;
        private float _endYCoord;
        private int _endMap;
        private int _endX;
        private int _endY;
        private long _startTime;
        private int _dashTime;

        public DashInstance(Entity en, int endMap, int endX, int endY, int dashTime, int changeDirection = -1)
        {
            _changeDirection = changeDirection;
            _endMap = endMap;
            _endX = endX;
            _endY = endY;
            _dashTime = dashTime;
        }

        public void Start(Entity en)
        {
            if (MapInstance.GetMap(en.CurrentMap) == null || MapInstance.GetMap(_endMap) == null ||
                (_endMap == en.CurrentMap) && (_endX == en.CurrentX) && (_endY == en.CurrentY))
            {
                en.Dashing = null;
            }
            else
            {
                var startMap = MapInstance.GetMap(en.CurrentMap);
                var endMap = MapInstance.GetMap(_endMap);
                _startTime = Globals.System.GetTimeMS();
                _startXCoord = en.OffsetX;
                _startYCoord = en.OffsetY;
                _endXCoord = (endMap.GetX() + _endX * Options.TileWidth) - (startMap.GetX() + en.CurrentX * Options.TileWidth);
                _endYCoord = (endMap.GetY() + _endY * Options.TileHeight) - (startMap.GetY() + en.CurrentY * Options.TileHeight);
                if (_changeDirection > -1) en.Dir = _changeDirection;
            }
        }

        public float GetXOffset()
        {
            if (Globals.System.GetTimeMS() > _startTime + _dashTime)
            {
                return _endXCoord;
            }
            else
            {
                return (_endXCoord - _startXCoord) * ((Globals.System.GetTimeMS() - _startTime) /(float)_dashTime);
            }
        }

        public float GetYOffset()
        {
            if (Globals.System.GetTimeMS() > _startTime + _dashTime)
            {
                return _endYCoord;
            }
            else
            {
                return (_endYCoord - _startYCoord) * ((Globals.System.GetTimeMS() - _startTime)/ (float)_dashTime);
            }
        }

        public bool Update(Entity en)
        {
            if (Globals.System.GetTimeMS() > _startTime + _dashTime)
            {
                en.Dashing = null;
                en.OffsetX = 0;
                en.OffsetY = 0;
                en.CurrentMap = _endMap;
                en.CurrentX = _endX;
                en.CurrentY = _endY;
            }
            return en.Dashing != null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Spells;
using Color = Intersect.Color;

namespace Intersect_Client.Classes.Entities
{
    public class Entity
    {
        //Chat
        private List<ChatBubble> _chatBubbles = new List<ChatBubble>();

        private bool _disposed;

        private long _lastUpdate;
        private long _walkTimer;

        //Entity Animations
        public List<AnimationInstance> Animations = new List<AnimationInstance>();

        //Combat
        public long AttackTimer = 0;
        public bool Blocking = false;

        //Combat Status
        public long CastTime = 0;

        //Location Info
        public int CurrentX;
        public int CurrentY;
        public int CurrentZ;

        //Dashing instance
        public DashInstance Dashing;
        public Queue<DashInstance> DashQueue = new Queue<DashInstance>();
        public long DashTimer;

        //Caching
        public MapInstance latestMap;

        private int _dir;
        public int Dir
        {
            get { return _dir; }
            set { _dir = (value + 4) % 4; }
        }

        public int[] Equipment = new int[Options.EquipmentSlots.Count];

        //Extras
        public string Face = "";
        public int Gender = 0;
        public int HideName;

        //Inventory/Spells/Equipment
        public ItemInstance[] Inventory = new ItemInstance[Options.MaxInvItems];
        public bool InView = true;
        public bool IsLocal = false;
        public bool IsMoving;
        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];
        public int MoveDir = -1;
        public float MoveTimer;
        //Core Values
        public int MyIndex;
        public string MyName = "";

        protected string _mySprite = "";
        public GameTexture Texture;
        public virtual string MySprite
        {
            get { return _mySprite; }
            set
            {
                _mySprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, _mySprite);
            }
        }
        public float OffsetX;
        public float OffsetY;
        public int Passable;

        private int cachedMapId = -1;
        private MapInstance cachedMapInstance;
        public MapInstance MapInstance
        {
            get
            {
                if (cachedMapId == CurrentMap && cachedMapInstance != null)
                {
                    return cachedMapInstance;
                }
                else
                {
                    cachedMapInstance = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                    cachedMapId = CurrentMap;
                    return cachedMapInstance;
                }
            }
        }

        //Rendering Variables
        public HashSet<Entity> RenderList;
        public long SpawnTime;
        public int SpellCast = 0;
        public SpellInstance[] Spells = new SpellInstance[Options.MaxPlayerSkills];
        public int[] Stat = new int[(int) Stats.StatCount];

        //Status effects
        public List<StatusInstance> Status = new List<StatusInstance>();
        public int Target = -1;
        public int type;
        public int[] Vital = new int[(int) Vitals.VitalCount];
        public int WalkFrame;

        //Animation Timer (for animated sprites)
        public long AnimationTimer;
        public int AnimationFrame;

        public Entity(int index, long spawnTime, ByteBuffer bf, bool isEvent = false)
        {
            CurrentMap = -1;
            SpawnTime = spawnTime;
            if (index > -1 && !isEvent)
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
            AnimationTimer = Globals.System.GetTimeMS() + Globals.Random.Next(0, 500);
            MyIndex = index;
            Load(bf);
        }

        public virtual int CurrentMap { get; set; }

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
            Level = bf.ReadInteger();
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
                var anim = AnimationBase.Lookup.Get<AnimationBase>(bf.ReadInteger());
                if (anim != null)
                    Animations.Add(new AnimationInstance(anim, true));
            }
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                MaxVital[i] = bf.ReadInteger();
                Vital[i] = bf.ReadInteger();
            }
            //Update status effects
            var count = bf.ReadInteger();
            Status.Clear();
            for (int i = 0; i < count; i++)
            {
                Status.Add(new StatusInstance(bf.ReadInteger(), bf.ReadInteger(), bf.ReadString(), bf.ReadInteger(), bf.ReadInteger()));
            }
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                Stat[i] = bf.ReadInteger();
            }
            type = bf.ReadInteger();

            _disposed = false;

			//Status effects box update
			if (Globals.Me != null)
			{
				if (MyIndex == Globals.Me.MyIndex)
				{
					if (Gui.GameUI != null) { Gui.GameUI._playerBox.UpdateStatuses = true; }
				}
				else if (MyIndex > -1 && MyIndex == Globals.Me._targetIndex)
				{
					Globals.Me._targetBox.UpdateStatuses = true;
                }
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
            var time = 1000f / (float) (1 + Math.Log(Stat[(int) Stats.Speed]));
            if (Blocking)
            {
                time += time * (float) Options.BlockingSlow;
            }
            return Math.Min(1000f, time);
        }

        //Movement Processing
        public virtual bool Update()
        {
            MapInstance map = null;
            if (_disposed)
            {
                latestMap = null;
                return false;
            }
            else
            {
                map = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                latestMap = map;
                if ((map == null || !map.InView()) &&
                    !Globals.Me.Party.Contains(MyIndex))
                {
                    Globals.EntitiesToDispose.Add(MyIndex);
                    return false;
                }
            }
            RenderList = DetermineRenderOrder(RenderList,map);
            if (_lastUpdate == 0)
            {
                _lastUpdate = Globals.System.GetTimeMS();
            }
            float ecTime = (float) (Globals.System.GetTimeMS() - _lastUpdate);
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
                        if (WalkFrame > 3)
                        {
                            WalkFrame = 0;
                        }
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
                        OffsetY -= (float) ecTime * ((float) Options.TileHeight) / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY < 0)
                        {
                            OffsetY = 0;
                        }
                        break;

                    case 1:
                        OffsetY += (float) ecTime * ((float) Options.TileHeight) / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY > 0)
                        {
                            OffsetY = 0;
                        }
                        break;

                    case 2:
                        OffsetX -= (float) ecTime * ((float) Options.TileHeight) / GetMovementTime();
                        OffsetY = 0;
                        if (OffsetX < 0)
                        {
                            OffsetX = 0;
                        }
                        break;

                    case 3:
                        OffsetX += (float) ecTime * ((float) Options.TileHeight) / GetMovementTime();
                        OffsetY = 0;
                        if (OffsetX > 0)
                        {
                            OffsetX = 0;
                        }
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
                    animInstance.SetPosition((int) GetCenterPos().X, (int) GetCenterPos().Y, CurrentX, CurrentY,
                        CurrentMap, Dir, CurrentZ);
                }
                else
                {
                    animInstance.SetPosition((int) GetCenterPos().X, (int) GetCenterPos().Y, CurrentX, CurrentY,
                        CurrentMap, -1, CurrentZ);
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
            if (AnimationTimer < Globals.System.GetTimeMS())
            {
                AnimationTimer = Globals.System.GetTimeMS() + 200;
                AnimationFrame++;
                if (AnimationFrame >= 4) AnimationFrame = 0;
            }
            _lastUpdate = Globals.System.GetTimeMS();
            return true;
        }

        public int CalculateAttackTime()
        {
            return
                (int)
                (Options.MaxAttackRate +
                 (float)
                 ((Options.MinAttackRate - Options.MaxAttackRate) *
                  (((float) Options.MaxStatValue - Stat[(int) Stats.Speed]) / (float) Options.MaxStatValue)));
        }

        public virtual bool IsStealthed()
        {
            //If the entity has transformed, apply that sprite instead.
            if (this == Globals.Me) return false;
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Stealth)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, MapInstance map)
        {
            if (renderList != null)
            {
                renderList.Remove(this);
            }

            if (map == null)
            {
                return null;
            }
            var gridX = map.MapGridX;
            var gridY = map.MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != -1)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            HashSet<Entity>[] outerList;
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
                                return renderList;
                            }
                            else if (y == gridY)
                            {
                                outerList[Options.MapHeight + CurrentY].Add(this);
                                renderList = outerList[Options.MapHeight + CurrentY];
                                return renderList;
                            }
                            else
                            {
                                outerList[Options.MapHeight * 2 + CurrentY].Add(this);
                                renderList = outerList[Options.MapHeight * 2 + CurrentY];
                                return renderList;
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
            if (MapInstance.Lookup.Get<MapInstance>(CurrentMap) == null || !Globals.GridMaps.Contains(CurrentMap)) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            var d = 0;

            string sprite = MySprite;
            int alpha = 255;

            //If the entity has transformed, apply that sprite instead.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Transform)
                {
                    sprite = Status[n].Data;
                }
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int) StatusTypes.Stealth)
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
            if (Texture != null)
            {
                var map = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                if (Texture.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2);
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY -
                                      ((Texture.GetHeight() / 4) - Options.TileHeight);
                }
                else
                {
                    destRectangle.X = map.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2;
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY;
                }
                destRectangle.X -= ((Texture.GetWidth() / 8));
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
                    default:
                        Dir = 0;
                        d = 3;
                        break;
                }
                destRectangle.X = (int) Math.Ceiling(destRectangle.X);
                destRectangle.Y = (int) Math.Ceiling(destRectangle.Y);
                if (Options.AnimatedSprites.Contains(sprite))
                {
                    srcRectangle = new FloatRect(AnimationFrame * (int)Texture.GetWidth() / 4, d * (int)Texture.GetHeight() / 4,
                            (int)Texture.GetWidth() / 4, (int)Texture.GetHeight() / 4);
                }
                else
                {
                    if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMS() || Blocking)
                    {
                        srcRectangle = new FloatRect(3 * (int)Texture.GetWidth() / 4, d * (int)Texture.GetHeight() / 4,
                            (int)Texture.GetWidth() / 4, (int)Texture.GetHeight() / 4);
                    }
                    else
                    {
                        srcRectangle = new FloatRect(WalkFrame * (int)Texture.GetWidth() / 4,
                            d * (int)Texture.GetHeight() / 4, (int)Texture.GetWidth() / 4,
                            (int)Texture.GetHeight() / 4);
                    }
                }
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(Texture, srcRectangle, destRectangle, new Intersect.Color(alpha, 255, 255, 255));

                //Don't render the paperdolls if they have transformed.
                if (sprite == MySprite && Equipment.Length == Options.EquipmentSlots.Count)
                {
                    //Draw the equipment/paperdolls
                    for (int z = 0; z < Options.PaperdollOrder[Dir].Count; z++)
                    {
                        if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[Dir][z]) > -1)
                        {
                            if (Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[Dir][z])] > -1 &&
                                (this != Globals.Me ||
                                 Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[Dir][z])] <
                                 Options.MaxInvItems))
                            {
                                var itemNum = -1;
                                if (this == Globals.Me)
                                {
                                    itemNum =
                                        Inventory[
                                                Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[Dir][z])
                                                ]]
                                            .ItemNum;
                                }
                                else
                                {
                                    itemNum = Equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[Dir][z])];
                                }
                                if (ItemBase.Lookup.Get<ItemBase>(itemNum) != null)
                                {
                                    var itemdata = ItemBase.Lookup.Get<ItemBase>(itemNum);
                                    if (Gender == 0)
                                    {
                                        DrawEquipment(itemdata.MalePaperdoll, alpha);
                                    }
                                    else
                                    {
                                        DrawEquipment(itemdata.FemalePaperdoll, alpha);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawChatBubbles()
        {
            var chatbubbles = _chatBubbles.ToArray();
            var bubbleoffset = 0f;
            for (int i = chatbubbles.Length - 1; i > -1; i--)
            {
                bubbleoffset = chatbubbles[i].Draw(bubbleoffset);
            }
        }

        public virtual void DrawEquipment(string filename, int alpha)
        {
            var map = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
            if (map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            var d = 0;
            GameTexture paperdollTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll,
                filename);
            if (paperdollTex != null)
            {
                if (paperdollTex.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = (map.GetX() + CurrentX * Options.TileWidth + OffsetX);
                    destRectangle.Y = map.GetY() + CurrentY * Options.TileHeight + OffsetY -
                                      ((paperdollTex.GetHeight() / 4) - Options.TileHeight);
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
                destRectangle.X = (int) Math.Ceiling(destRectangle.X);
                destRectangle.Y = (int) Math.Ceiling(destRectangle.Y);
                if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMS() || Blocking)
                {
                    srcRectangle = new FloatRect(3 * (int) paperdollTex.GetWidth() / 4,
                        d * (int) paperdollTex.GetHeight() / 4, (int) paperdollTex.GetWidth() / 4,
                        (int) paperdollTex.GetHeight() / 4);
                }
                else
                {
                    srcRectangle = new FloatRect(WalkFrame * (int) paperdollTex.GetWidth() / 4,
                        d * (int) paperdollTex.GetHeight() / 4, (int) paperdollTex.GetWidth() / 4,
                        (int) paperdollTex.GetHeight() / 4);
                }
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                GameGraphics.DrawGameTexture(paperdollTex, srcRectangle, destRectangle, new Intersect.Color(alpha, 255, 255, 255));
            }
        }

        //returns the point on the screen that is the center of the player sprite
        public virtual Pointf GetCenterPos()
        {
            if (latestMap == null)
            {
                return new Pointf(0, 0);
            }
            Pointf pos = new Pointf(latestMap.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                latestMap.GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            if (Texture != null)
            {
                pos.Y += Options.TileHeight / 2;
                pos.Y -= Texture.GetHeight() / 4 / 2;
            }
            return pos;
        }

        public virtual float GetTopPos()
        {
            var map = latestMap;
            if (map == null)
            {
                return 0f;
            }
            var y = (int) Math.Ceiling(GetCenterPos().Y);
            if (Texture != null)
            {
                y = y - (int) ((Texture.GetHeight() / 8));
                y -= 12;
            }
            if (GetType() != typeof(Event))
            {
                y -= 10;
            } //Need room for HP bar if not an event.
            return y;
        }

        public virtual void DrawName(Color textColor, Color borderColor = null, Color backgroundColor = null)
        {
            if (HideName == 1 || MyName.Trim().Length == 0)
            {
                return;
            }
            if (borderColor == null) borderColor = Color.Transparent;
            if (backgroundColor == null) backgroundColor = Color.Transparent;
            //Check for npc colors
            if (textColor == null)
            {
                switch (type)
                {
                    case -1: //When entity has a target (showing aggression)
                        textColor = CustomColors.AgressiveNpcName;
                        borderColor = CustomColors.AgressiveNpcNameBorder;
                        backgroundColor = CustomColors.AgressiveNpcNameBackground;
                        break;
                    case 0: //Attack when attacked
                        textColor = Intersect.CustomColors.AttackWhenAttackedName;
                        borderColor = CustomColors.AttackWhenAttackedNameBorder;
                        backgroundColor = CustomColors.AttackWhenAttackedNameBackground;
                        break;
                    case 1: //Attack on sight
                        textColor = CustomColors.AttackOnSightName;
                        borderColor = CustomColors.AttackOnSightNameBorder;
                        backgroundColor = CustomColors.AttackOnSightNameBackground;
                        break;
                    case 3: //Guard
                        textColor = CustomColors.GuardName;
                        borderColor = CustomColors.GuardNameBorder;
                        backgroundColor = CustomColors.GuardNameBackground;
                        break;
                    case 2: //Neutral
                    default:
                        textColor = CustomColors.NeutralName;
                        borderColor = CustomColors.NeutralNameBorder;
                        backgroundColor = CustomColors.NeutralNameBackground;
                        break;
                }
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int) StatusTypes.Stealth)
                {
                    if (this != Globals.Me)
                    {
                        return;
                    }
                }
            }
            var map = MapInstance;
            if (map == null)
            {
                return;
            }
            var y = GetTopPos() - 4;
            var x = (int) Math.Ceiling(GetCenterPos().X);

            Pointf textSize = GameGraphics.Renderer.MeasureText(MyName, GameGraphics.GameFont, 1);

            if (backgroundColor != Color.Transparent) GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(),new FloatRect(0,0,1,1),new FloatRect((x - textSize.X / 2f) - 4, y, textSize.X + 8, textSize.Y),backgroundColor);
            GameGraphics.Renderer.DrawString(MyName, GameGraphics.GameFont,
                (int) (x - (int) Math.Ceiling(textSize.X / 2f)), (int) (y), 1, IntersectClientExtras.GenericClasses.Color.FromArgb(textColor.ToArgb()),true,null, IntersectClientExtras.GenericClasses.Color.FromArgb(borderColor.ToArgb()));
        }

        public void DrawHpBar()
        {
            if (HideName == 1 && Vital[(int) Vitals.Health] == MaxVital[(int) Vitals.Health])
            {
                return;
            }
            if (Vital[(int) Vitals.Health] <= 0)
            {
                return;
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int) StatusTypes.Stealth)
                {
                    if (this != Globals.Me)
                    {
                        return;
                    }
                }
            }

            var map = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
            if (map == null)
            {
                return;
            }
            var width = Options.TileWidth;
            var fillRatio = ((float) Vital[(int) Vitals.Health] / MaxVital[(int) Vitals.Health]);
            fillRatio = Math.Min(1, Math.Max(0, fillRatio));
            var fillWidth = fillRatio * width;
            var y = (int) Math.Ceiling(GetCenterPos().Y);
            var x = (int) Math.Ceiling(GetCenterPos().X);
            GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                y = y - (int) ((entityTex.GetHeight() / 8));
                y -= 8;
            }

            GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                new FloatRect((int) (x - 1 - width / 2), (int) (y - 1), width, 6), CustomColors.HpBackground);
            GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                new FloatRect((int) (x - width / 2), (int) (y), fillWidth - 2, 4), CustomColors.HpForeground);
        }

        public void DrawCastingBar()
        {
            if (CastTime < Globals.System.GetTimeMS())
            {
                return;
            }
            if (MapInstance.Lookup.Get<MapInstance>(CurrentMap) == null)
            {
                return;
            }
            var castSpell = SpellBase.Lookup.Get<SpellBase>(SpellCast);
            if (castSpell != null)
            {
                var width = Options.TileWidth;
                var fillWidth = ((castSpell.CastDuration * 100 -
                                  (CastTime - Globals.System.GetTimeMS())) /
                                 (float) (castSpell.CastDuration * 100) * width);
                var y = (int) Math.Ceiling(GetCenterPos().Y);
                var x = (int) Math.Ceiling(GetCenterPos().X);
                GameTexture entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity,
                    MySprite);
                if (entityTex != null)
                {
                    y = y + (int) ((entityTex.GetHeight() / 8));
                    y += 3;
                }

                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((int) (x - 1 - width / 2), (int) (y - 1), width, 6), CustomColors.CastingBackground);
                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((int) (x - width / 2), (int) (y), fillWidth - 2, 4), CustomColors.CastingForeground);
            }
        }

        //
        public void DrawTarget(int Priority)
        {
            if (GetType() == typeof(Projectile)) return;
            var map = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
            if (map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture targetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "target.png");
            if (targetTex != null)
            {
                destRectangle.X = GetCenterPos().X - (int) targetTex.GetWidth() / 4;
                destRectangle.Y = GetCenterPos().Y - (int) targetTex.GetHeight() / 2;

                srcRectangle = new FloatRect(Priority * (int) targetTex.GetWidth() / 2, 0,
                    (int) targetTex.GetWidth() / 2,
                    (int) targetTex.GetHeight());
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;

                GameGraphics.DrawGameTexture(targetTex, srcRectangle, destRectangle, Intersect.Color.White);
            }
        }

        //Chatting
        public void AddChatBubble(string text)
        {
            _chatBubbles.Add(new ChatBubble(this, text));
        }

        ~Entity()
        {
            Dispose();
        }
    }

    public class StatusInstance
    {
		public int SpellNum;
        public string Data = "";
        public int Type = -1;
        public int TimeRemaining = 0;
        public int TotalDuration = 1;
        public long TimeRecevied = 0;

        public StatusInstance(int spellNum, int type, string data, int timeRemaining, int totalDuration)
        {
			SpellNum = spellNum;
			Type = type;
            Data = data;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            TimeRecevied = Globals.System.GetTimeMS();
        }
    }

    public class DashInstance
    {
        private int _changeDirection = -1;
        private int _dashTime;
        private int _endMap;
        private int _endX;
        private float _endXCoord;
        private int _endY;
        private float _endYCoord;
        private long _startTime;
        private float _startXCoord;
        private float _startYCoord;

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
            if (MapInstance.Lookup.Get<MapInstance>(en.CurrentMap) == null || MapInstance.Lookup.Get<MapInstance>(_endMap) == null ||
                (_endMap == en.CurrentMap) && (_endX == en.CurrentX) && (_endY == en.CurrentY))
            {
                en.Dashing = null;
            }
            else
            {
                var startMap = MapInstance.Lookup.Get<MapInstance>(en.CurrentMap);
                var endMap = MapInstance.Lookup.Get<MapInstance>(_endMap);
                _startTime = Globals.System.GetTimeMS();
                _startXCoord = en.OffsetX;
                _startYCoord = en.OffsetY;
                _endXCoord = (endMap.GetX() + _endX * Options.TileWidth) -
                             (startMap.GetX() + en.CurrentX * Options.TileWidth);
                _endYCoord = (endMap.GetY() + _endY * Options.TileHeight) -
                             (startMap.GetY() + en.CurrentY * Options.TileHeight);
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
                return (_endXCoord - _startXCoord) * ((Globals.System.GetTimeMS() - _startTime) / (float) _dashTime);
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
                return (_endYCoord - _startYCoord) * ((Globals.System.GetTimeMS() - _startTime) / (float) _dashTime);
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
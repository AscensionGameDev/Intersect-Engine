using System;
using System.Collections.Generic;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.Maps;
using Intersect.Client.Spells;
using Intersect.Client.UI;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Client.Entities
{
    public class Entity
    {
        //Core Values
        public Guid Id;

        //Chat
        private List<ChatBubble> mChatBubbles = new List<ChatBubble>();

        private int mDir;

        protected bool mDisposed;

        private long mLastUpdate;

        protected string mMySprite = "";
        protected string mTransformedSprite = "";
        private long mWalkTimer;
        protected byte mRenderPriority = 1;
        public int AnimationFrame;
        public float elapsedtime; //to be removed

        //Entity Animations
        public List<AnimationInstance> Animations = new List<AnimationInstance>();

        //Animation Timer (for animated sprites)
        public long AnimationTimer;

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

        public Guid[] Equipment = new Guid[Options.EquipmentSlots.Count];
        public int[] MyEquipment = new int[Options.EquipmentSlots.Count];
        public AnimationInstance[] EquipmentAnimations = new AnimationInstance[Options.EquipmentSlots.Count];

        //Extras
        public string Face = "";

        public int Gender = 0;
        public bool HideName;
        public bool HideEntity = false;

        //Inventory/Spells/Equipment
        public ItemInstance[] Inventory = new ItemInstance[Options.MaxInvItems];

        public bool InView = true;
        public bool IsLocal = false;
        public bool IsMoving;

        //Caching
        public MapInstance LatestMap;

        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        public int MoveDir = -1;

        public float MoveTimer;

        public string Name = "";
        public float OffsetX;
        public float OffsetY;
        public bool Passable;

        //Rendering Variables
        public HashSet<Entity> RenderList;
        public FloatRect WorldPos = new FloatRect();
        
        public Guid SpellCast;
        public SpellInstance[] Spells = new SpellInstance[Options.MaxPlayerSkills];
        public int[] Stat = new int[(int) Stats.StatCount];

        //Status effects
        public List<StatusInstance> Status = new List<StatusInstance>();

        public int Target = -1;
        public GameTexture Texture;
        public int Type;
        public int[] Vital = new int[(int) Vitals.VitalCount];
        public int WalkFrame;

        protected Pointf mCenterPos = Pointf.Empty;

        public Entity(Guid id, ByteBuffer bf, bool isEvent = false)
        {
            Id = id;
            CurrentMap = Guid.Empty;
            if (id != Guid.Empty && !isEvent)
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
                    Equipment[i] = Guid.Empty;
                    MyEquipment[i] = -1;
                }
            }
            AnimationTimer = Globals.System.GetTimeMs() + Globals.Random.Next(0, 500);
            //TODO Remove because fixed orrrrr change the exception text
            if (Options.EquipmentSlots.Count == 0) throw new Exception("What the fuck is going on!?!?!?!?!?!");
            Load(bf);
        }

        public int Dir
        {
            get { return mDir; }
            set { mDir = (value + 4) % 4; }
        }

        public virtual string TransformedSprite
        {
            get { return mTransformedSprite; }
            set
            {
                mTransformedSprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, mTransformedSprite);
                if (value == "") MySprite = mMySprite;
            }
        }

        public virtual string MySprite
        {
            get { return mMySprite; }
            set
            {
                mMySprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, mMySprite);
            }
        }

        public MapInstance MapInstance
        {
            get
            {
                return MapInstance.Get(CurrentMap);
            }
        }

        public virtual Guid CurrentMap { get; set; }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        //Deserializing
        public virtual void Load(ByteBuffer bf)
        {
            CurrentMap = bf.ReadGuid();
            Name = bf.ReadString();
            MySprite = bf.ReadString();
            Face = bf.ReadString();
            Level = bf.ReadInteger();
            CurrentX = bf.ReadInteger();
            CurrentY = bf.ReadInteger();
            CurrentZ = bf.ReadInteger();
            Dir = bf.ReadInteger();
            Passable = bf.ReadBoolean();
            HideName = bf.ReadBoolean();
            HideEntity = bf.ReadBoolean();
            var animsToClear = new List<AnimationInstance>();
            var animsToAdd = new List<AnimationBase>();
            int animCount = bf.ReadInteger();
            for (int i = 0; i < animCount; i++)
            {
                var anim = AnimationBase.Get(bf.ReadGuid());
                if (anim != null)
                    animsToAdd.Add(anim);
            }
            foreach (var anim in Animations)
            {
                animsToClear.Add(anim);
                if (!anim.InfiniteLoop)
                {
                    animsToClear.Remove(anim);
                }
                else
                {
                    foreach (var addedAnim in animsToAdd)
                    {
                        if (addedAnim.Id == anim.MyBase.Id)
                        {
                            animsToClear.Remove(anim);
                            animsToAdd.Remove(addedAnim);
                            break;
                        }
                    }
                }
            }
            ClearAnimations(animsToClear);
            AddAnimations(animsToAdd);
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
                Status.Add(new StatusInstance(bf.ReadGuid(), bf.ReadInteger(), bf.ReadString(), bf.ReadInteger(),
                    bf.ReadInteger()));
            }
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                Stat[i] = bf.ReadInteger();
            }
            Type = bf.ReadInteger();

            mDisposed = false;

            //Status effects box update
            if (Globals.Me != null)
            {
                if (Id == Globals.Me.Id)
                {
                    if (Gui.GameUi != null)
                    {
                        Gui.GameUi.PlayerBox.UpdateStatuses = true;
                    }
                }
                else if (Id != Guid.Empty && Id == Globals.Me.TargetIndex)
                {
                    Globals.Me.TargetBox.UpdateStatuses = true;
                }
            }
        }

        public void AddAnimations(List<AnimationBase> anims)
        {
            foreach (var anim in anims)
            {
                Animations.Add(new AnimationInstance(anim,true,false,-1,this));
            }
        }

        public void ClearAnimations(List<AnimationInstance> anims)
        {
            if (anims == null) anims = Animations;
            if (anims.Count > 0)
            {
                for (int i = 0; i < anims.Count; i++)
                {
                    anims[i].Dispose();
                    Animations.Remove(anims[i]);
                }
            }
        }

        public virtual bool IsDisposed()
        {
            return mDisposed;
        }

        public virtual void Dispose()
        {
            if (RenderList != null)
            {
                RenderList.Remove(this);
            }
            ClearAnimations(null);
            mDisposed = true;
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
            if (mDisposed)
            {
                LatestMap = null;
                return false;
            }
            else
            {
                map = MapInstance.Get(CurrentMap);
                LatestMap = map;
                if ((map == null || !map.InView()))
                {
                    Globals.EntitiesToDispose.Add(Id);
                    return false;
                }
            }
            RenderList = DetermineRenderOrder(RenderList, map);
            if (mLastUpdate == 0)
            {
                mLastUpdate = Globals.System.GetTimeMs();
            }
            float ecTime = (float) (Globals.System.GetTimeMs() - mLastUpdate);
            elapsedtime = ecTime;
            if (Dashing != null)
            {
                WalkFrame = 1; //Fix the frame whilst dashing
            }
            else if (mWalkTimer < Globals.System.GetTimeMs())
            {
                if (!IsMoving && DashQueue.Count > 0)
                {
                    Dashing = DashQueue.Dequeue();
                    Dashing.Start(this);
                    OffsetX = 0;
                    OffsetY = 0;
                    DashTimer = Globals.System.GetTimeMs() + Options.MaxDashSpeed;
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
                        if (WalkFrame == 1 || WalkFrame == 2)
                        {
                            WalkFrame = 2;
                        }
                        else
                        {
                            WalkFrame = 0;
                        }
                    }
                    mWalkTimer = Globals.System.GetTimeMs() + 200;
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

            //Check to see if we should start or stop equipment animations
            if (Equipment.Length == Options.EquipmentSlots.Count)
            {
                for (int z = 0; z < Options.EquipmentSlots.Count; z++)
                {
                    if (Equipment[z] != Guid.Empty && (this != Globals.Me || MyEquipment[z] < Options.MaxInvItems))
                    {
                        var itemId = Guid.Empty;
                        if (this == Globals.Me)
                        {
                            var slot = MyEquipment[z];
                            if (slot > -1)
                                itemId = Inventory[slot].ItemId;
                        }
                        else
                        {
                            itemId = Equipment[z];
                        }
                        var itm = ItemBase.Get(itemId);
                        AnimationBase anim = null;
                        if (itm != null)
                        {
                            anim = itm.EquipmentAnimation;
                        }
                        if (anim != null)
                        {
                            if (EquipmentAnimations[z] != null && EquipmentAnimations[z].MyBase != anim)
                            {
                                EquipmentAnimations[z].Dispose();
                                Animations.Remove(EquipmentAnimations[z]);
                                EquipmentAnimations[z] = null;
                            }
                            if (EquipmentAnimations[z] == null)
                            {
                                EquipmentAnimations[z] = new AnimationInstance(anim, true, true, -1, this);
                                Animations.Add(EquipmentAnimations[z]);
                            }
                        }
                        else
                        {
                            if (EquipmentAnimations[z] != null)
                            {
                                EquipmentAnimations[z].Dispose();
                                Animations.Remove(EquipmentAnimations[z]);
                                EquipmentAnimations[z] = null;
                            }
                        }
                    }
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
            var chatbubbles = mChatBubbles.ToArray();
            foreach (var chatbubble in chatbubbles)
            {
                if (!chatbubble.Update())
                {
                    mChatBubbles.Remove(chatbubble);
                }
            }
            if (AnimationTimer < Globals.System.GetTimeMs())
            {
                AnimationTimer = Globals.System.GetTimeMs() + 200;
                AnimationFrame++;
                if (AnimationFrame >= 4) AnimationFrame = 0;
            }

            CalculateCenterPos();
            mLastUpdate = Globals.System.GetTimeMs();
            return true;
        }

        public virtual int CalculateAttackTime()
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

            if (map == null || Globals.Me == null || Globals.Me.MapInstance == null)
            {
                return null;
            }
            var gridX = Globals.Me.MapInstance.MapGridX;
            var gridY = Globals.Me.MapInstance.MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            var priority = mRenderPriority;
                            if (CurrentZ != 0)
                            {
                                priority += 3;
                            }
                            if (y == gridY - 1)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight + CurrentY];
                                return renderList;
                            }
                            else if (y == gridY)
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 2 + CurrentY];
                                return renderList;
                            }
                            else
                            {
                                GameGraphics.RenderingEntities[priority, Options.MapHeight * 3 + CurrentY].Add(this);
                                renderList = GameGraphics.RenderingEntities[priority, Options.MapHeight * 3 + CurrentY];
                                return renderList;
                            }
                        }
                    }
                }
            }
            return renderList;
        }

        //Rendering Functions
        public virtual void Draw()
        {
            if (HideEntity) return; //Don't draw if the entity is hidden
            WorldPos.Reset();
            var map = MapInstance.Get(CurrentMap);
            if (map == null ||  !Globals.GridMaps.Contains(CurrentMap)) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            var d = 0;

            string sprite = "";
            int alpha = 255;

            //If the entity has transformed, apply that sprite instead.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Transform)
                {
                    sprite = Status[n].Data;
					TransformedSprite = sprite;
				}
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int) StatusTypes.Stealth)
                {
                    if (this != Globals.Me && !Globals.Me.IsInMyParty(this))
                    {
                        return;
                    }
                    else
                    {
                        alpha = 125;
                    }
                }
            }

			//Check if there is no transformed sprite set
			if (string.IsNullOrEmpty(sprite))
			{
				sprite = MySprite;
				MySprite = sprite;
			}

            if (Texture != null)
            {
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
                if (Options.AnimatedSprites.Contains(sprite.ToLower()))
                {
                    srcRectangle = new FloatRect(AnimationFrame * (int) Texture.GetWidth() / 4,
                        d * (int) Texture.GetHeight() / 4,
                        (int) Texture.GetWidth() / 4, (int) Texture.GetHeight() / 4);
                }
                else
                {
                    var attackTime = CalculateAttackTime();
                    if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMs() || Blocking)
                    {
                        srcRectangle = new FloatRect(3 * (int) Texture.GetWidth() / 4,
                            d * (int) Texture.GetHeight() / 4,
                            (int) Texture.GetWidth() / 4, (int) Texture.GetHeight() / 4);
                    }
                    else
                    {
                        srcRectangle = new FloatRect(WalkFrame * (int) Texture.GetWidth() / 4,
                            d * (int) Texture.GetHeight() / 4, (int) Texture.GetWidth() / 4,
                            (int) Texture.GetHeight() / 4);
                    }
                }
                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                //GameGraphics.DrawGameTexture(Texture, srcRectangle, destRectangle,
                //    new Intersect.Color(alpha, 255, 255, 255));

                WorldPos = destRectangle;

				//Order the layers of paperdolls and sprites
				for (int z = 0; z < Options.PaperdollOrder[Dir].Count; z++)
				{
				    var paperdoll = Options.PaperdollOrder[Dir][z];
				    var equipSlot = Options.EquipmentSlots.IndexOf(paperdoll);
                    //Check for player
                    if (paperdoll == "Player")
					{
						GameGraphics.DrawGameTexture(Texture, srcRectangle, destRectangle, new Intersect.Color(alpha, 255, 255, 255));
					}
					else if (equipSlot > -1)
					{
						//Don't render the paperdolls if they have transformed.
						if (sprite == MySprite && Equipment.Length == Options.EquipmentSlots.Count)
						{
							if ((Equipment[equipSlot] != Guid.Empty && this != Globals.Me) || MyEquipment[equipSlot] < Options.MaxInvItems)
							{
								var itemId = Guid.Empty;
								if (this == Globals.Me)
								{
                                    var slot = MyEquipment[equipSlot];
                                    if (slot > -1)
                                        itemId = Inventory[slot].ItemId;
								}
								else
								{
									itemId = Equipment[equipSlot];
								}
							    var item = ItemBase.Get(itemId);
								if (item != null)
								{
									if (Gender == 0)
									{
										DrawEquipment(item.MalePaperdoll, alpha);
									}
									else
									{
										DrawEquipment(item.FemalePaperdoll, alpha);
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
            var chatbubbles = mChatBubbles.ToArray();
            var bubbleoffset = 0f;
            for (int i = chatbubbles.Length - 1; i > -1; i--)
            {
                bubbleoffset = chatbubbles[i].Draw(bubbleoffset);
            }
        }

        public virtual void DrawEquipment(string filename, int alpha)
        {
            var map = MapInstance.Get(CurrentMap);
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
                if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMs() || Blocking)
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

        protected virtual void CalculateCenterPos()
        {
            Pointf pos = new Pointf(LatestMap.GetX() + CurrentX * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                LatestMap.GetY() + CurrentY * Options.TileHeight + OffsetY + Options.TileHeight / 2);
            if (Texture != null)
            {
                pos.Y += Options.TileHeight / 2;
                pos.Y -= Texture.GetHeight() / 4 / 2;
            }

            mCenterPos = pos;
        }

        //returns the point on the screen that is the center of the player sprite
        public Pointf GetCenterPos()
        {
            if (LatestMap == null)
            {
                return new Pointf(0, 0);
            }

            return mCenterPos;
        }

        public virtual float GetTopPos(int overrideHeight = 0)
        {
            var map = LatestMap;
            if (map == null)
            {
                return 0f;
            }
            var y = (int) Math.Ceiling(GetCenterPos().Y);
            if (overrideHeight != 0)
            {
                y = y - (int)((overrideHeight / 8));
                y -= 12;
            }
            else
            {
                if (Texture != null)
                {
                    y = y - (int) ((Texture.GetHeight() / 8));
                    y -= 12;
                }
            }
            if (GetType() != typeof(Event))
            {
                y -= 10;
            } //Need room for HP bar if not an event.
            return y;
        }

        public virtual void DrawName(Color textColor, Color borderColor = null, Color backgroundColor = null)
        {
            if (HideName || Name.Trim().Length == 0)
            {
                return;
            }
            if (borderColor == null) borderColor = Color.Transparent;
            if (backgroundColor == null) backgroundColor = Color.Transparent;
            //Check for npc colors
            if (textColor == null)
            {
                switch (Type)
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
                    if (this != Globals.Me && !Globals.Me.IsInMyParty(this))
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

            Pointf textSize = GameGraphics.Renderer.MeasureText(Name, GameGraphics.GameFont, 1);

            if (backgroundColor != Color.Transparent)
                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((x - textSize.X / 2f) - 4, y, textSize.X + 8, textSize.Y), backgroundColor);
            GameGraphics.Renderer.DrawString(Name, GameGraphics.GameFont,
                (int) (x - (int) Math.Ceiling(textSize.X / 2f)), (int) (y), 1,
                Framework.GenericClasses.Color.FromArgb(textColor.ToArgb()), true, null,
                Framework.GenericClasses.Color.FromArgb(borderColor.ToArgb()));
        }

        public void DrawHpBar()
        {
            if (HideName && HideEntity) return;
            if (Vital[(int)Vitals.Health] <= 0) return;

            var maxVital = MaxVital[(int)Vitals.Health];
            int shieldSize = 0;

            //Check for shields
            foreach (var status in Status)
            {
                if (status.Type == (int)StatusTypes.Shield)
                {
                    shieldSize += status.Shield[(int)Vitals.Health];
                    maxVital += status.Shield[(int)Vitals.Health];
                }
            }

            if (Vital[(int)Vitals.Health] == MaxVital[(int)Vitals.Health] && shieldSize <= 0) return;

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == (int) StatusTypes.Stealth)
                {
                    if (this != Globals.Me && !Globals.Me.IsInMyParty(this))
                    {
                        return;
                    }
                }
            }

            var map = MapInstance.Get(CurrentMap);
            if (map == null)
            {
                return;
            }
            var width = Options.TileWidth;

            var hpfillRatio = ((float) Vital[(int) Vitals.Health] / maxVital);
            hpfillRatio = Math.Min(1, Math.Max(0, hpfillRatio));
            var hpfillWidth = hpfillRatio * width;

            var shieldfillRatio = ((float)shieldSize / maxVital);
            shieldfillRatio = Math.Min(1, Math.Max(0, shieldfillRatio));
            var shieldfillWidth = shieldfillRatio * width;

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
                new FloatRect((int) (x - width / 2), (int) (y), hpfillWidth - 2, 4), CustomColors.HpForeground);

            if (shieldSize > 0) //Check for a shield to render
                GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect((int)(x - width / 2) + hpfillWidth - 2, (int)(y), shieldfillWidth, 4), CustomColors.ShieldForeground);
        }

        public void DrawCastingBar()
        {
            if (CastTime < Globals.System.GetTimeMs())
            {
                return;
            }
            if (MapInstance.Get(CurrentMap) == null)
            {
                return;
            }
            var castSpell = SpellBase.Get(SpellCast);
            if (castSpell != null)
            {
                var width = Options.TileWidth;
                var fillWidth = ((castSpell.CastDuration -
                                  (CastTime - Globals.System.GetTimeMs())) /
                                 (float) (castSpell.CastDuration) * width);
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
        public void DrawTarget(int priority)
        {
            if (GetType() == typeof(Projectile)) return;
            var map = MapInstance.Get(CurrentMap);
            if (map == null) return;
            FloatRect srcRectangle = new FloatRect();
            FloatRect destRectangle = new FloatRect();
            GameTexture targetTex =
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "target.png");
            if (targetTex != null)
            {
                destRectangle.X = GetCenterPos().X - (int) targetTex.GetWidth() / 4;
                destRectangle.Y = GetCenterPos().Y - (int) targetTex.GetHeight() / 2;

                srcRectangle = new FloatRect(priority * (int) targetTex.GetWidth() / 2, 0,
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
            mChatBubbles.Add(new ChatBubble(this, text));
        }

        ~Entity()
        {
            Dispose();
        }
    }

    public class StatusInstance
    {
        public string Data = "";
        public Guid SpellId;
        public long TimeRecevied = 0;
        public int TimeRemaining = 0;
        public int TotalDuration = 1;
        public int Type = -1;
        public int[] Shield = new int[(int)Vitals.VitalCount];

        public StatusInstance(Guid spellId, int type, string data, int timeRemaining, int totalDuration)
        {
            SpellId = spellId;
            Type = type;
            Data = data;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            TimeRecevied = Globals.System.GetTimeMs();
        }
    }

    public class DashInstance
    {
        private int mChangeDirection = -1;
        private int mDashTime;
        private Guid mEndMapId;
        private int mEndX;
        private float mEndXCoord;
        private int mEndY;
        private float mEndYCoord;
        private long mStartTime;
        private float mStartXCoord;
        private float mStartYCoord;

        public DashInstance(Entity en, Guid endMapId, int endX, int endY, int dashTime, int changeDirection = -1)
        {
            mChangeDirection = changeDirection;
            mEndMapId = endMapId;
            mEndX = endX;
            mEndY = endY;
            mDashTime = dashTime;
        }

        public void Start(Entity en)
        {
            if (MapInstance.Get(en.CurrentMap) == null ||
                MapInstance.Get(mEndMapId) == null ||
                (mEndMapId == en.CurrentMap) && (mEndX == en.CurrentX) && (mEndY == en.CurrentY))
            {
                en.Dashing = null;
            }
            else
            {
                var startMap = MapInstance.Get(en.CurrentMap);
                var endMap = MapInstance.Get(mEndMapId);
                mStartTime = Globals.System.GetTimeMs();
                mStartXCoord = en.OffsetX;
                mStartYCoord = en.OffsetY;
                mEndXCoord = (endMap.GetX() + mEndX * Options.TileWidth) -
                             (startMap.GetX() + en.CurrentX * Options.TileWidth);
                mEndYCoord = (endMap.GetY() + mEndY * Options.TileHeight) -
                             (startMap.GetY() + en.CurrentY * Options.TileHeight);
                if (mChangeDirection > -1) en.Dir = mChangeDirection;
            }
        }

        public float GetXOffset()
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                return mEndXCoord;
            }
            else
            {
                return (mEndXCoord - mStartXCoord) * ((Globals.System.GetTimeMs() - mStartTime) / (float) mDashTime);
            }
        }

        public float GetYOffset()
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                return mEndYCoord;
            }
            else
            {
                return (mEndYCoord - mStartYCoord) * ((Globals.System.GetTimeMs() - mStartTime) / (float) mDashTime);
            }
        }

        public bool Update(Entity en)
        {
            if (Globals.System.GetTimeMs() > mStartTime + mDashTime)
            {
                en.Dashing = null;
                en.OffsetX = 0;
                en.OffsetY = 0;
                en.CurrentMap = mEndMapId;
                en.CurrentX = mEndX;
                en.CurrentY = mEndY;
            }
            return en.Dashing != null;
        }
    }
}
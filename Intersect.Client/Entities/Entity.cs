﻿using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.Maps;
using Intersect.Client.Spells;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Network.Packets.Server;

using JetBrains.Annotations;

namespace Intersect.Client.Entities
{

    public class Entity
    {

        public enum LabelType
        {

            Header = 0,

            Footer,

            Name,

            ChatBubble

        }

        public int AnimationFrame;

        //Entity Animations
        public List<Animation> Animations = new List<Animation>();

        //Animation Timer (for animated sprites)
        public long AnimationTimer;

        //Combat
        public long AttackTimer = 0;

        public bool Blocking = false;

        //Combat Status
        public long CastTime = 0;

        //Dashing instance
        public Dash Dashing;

        public Queue<Dash> DashQueue = new Queue<Dash>();

        public long DashTimer;

        public float elapsedtime; //to be removed

        public Guid[] Equipment = new Guid[Options.EquipmentSlots.Count];

        public Animation[] EquipmentAnimations = new Animation[Options.EquipmentSlots.Count];

        //Extras
        public string Face = "";

        public Label FooterLabel;

        public Gender Gender = Gender.Male;

        public Label HeaderLabel;

        public bool HideEntity = false;

        public bool HideName;

        //Core Values
        public Guid Id;

        //Inventory/Spells/Equipment
        public Item[] Inventory = new Item[Options.MaxInvItems];

        public bool InView = true;

        public bool IsLocal = false;

        public bool IsMoving;

        //Caching
        public MapInstance LatestMap;

        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        protected Pointf mCenterPos = Pointf.Empty;

        //Chat
        private List<ChatBubble> mChatBubbles = new List<ChatBubble>();

        private byte mDir;

        protected bool mDisposed;

        private long mLastUpdate;

        protected string mMySprite = "";

        public int MoveDir = -1;

        public float MoveTimer;

        protected byte mRenderPriority = 1;

        protected string mTransformedSprite = "";

        private long mWalkTimer;

        public int[] MyEquipment = new int[Options.EquipmentSlots.Count];

        public string Name = "";

        public Color NameColor = null;

        public float OffsetX;

        public float OffsetY;

        public bool Passable;

        //Rendering Variables
        public HashSet<Entity> RenderList;

        public Guid SpellCast;

        public Spell[] Spells = new Spell[Options.MaxPlayerSkills];

        public int[] Stat = new int[(int) Stats.StatCount];

        public int Target = -1;

        public GameTexture Texture;

        public int Type;

        public int[] Vital = new int[(int) Vitals.VitalCount];

        public int WalkFrame;

        public FloatRect WorldPos = new FloatRect();

        //Location Info
        public byte X;

        public byte Y;

        public byte Z;

        public Entity(Guid id, EntityPacket packet, bool isEvent = false)
        {
            Id = id;
            CurrentMap = Guid.Empty;
            if (id != Guid.Empty && !isEvent)
            {
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    Inventory[i] = new Item();
                }

                for (var i = 0; i < Options.MaxPlayerSkills; i++)
                {
                    Spells[i] = new Spell();
                }

                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    Equipment[i] = Guid.Empty;
                    MyEquipment[i] = -1;
                }
            }

            AnimationTimer = Globals.System.GetTimeMs() + Globals.Random.Next(0, 500);

            //TODO Remove because fixed orrrrr change the exception text
            if (Options.EquipmentSlots.Count == 0)
            {
                throw new Exception("What the fuck is going on!?!?!?!?!?!");
            }

            Load(packet);
        }

        //Status effects
        [NotNull]
        public List<Status> Status { get; private set; } = new List<Status>();

        public byte Dir
        {
            get => mDir;
            set => mDir = (byte) ((value + 4) % 4);
        }

        public virtual string TransformedSprite
        {
            get => mTransformedSprite;
            set
            {
                mTransformedSprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, mTransformedSprite);
                if (value == "")
                {
                    MySprite = mMySprite;
                }
            }
        }

        public virtual string MySprite
        {
            get => mMySprite;
            set
            {
                mMySprite = value;
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, mMySprite);
            }
        }

        public MapInstance MapInstance => MapInstance.Get(CurrentMap);

        public virtual Guid CurrentMap { get; set; }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        //Deserializing
        public virtual void Load(EntityPacket packet)
        {
            CurrentMap = packet.MapId;
            Name = packet.Name;
            MySprite = packet.Sprite;
            Face = packet.Face;
            Level = packet.Level;
            X = packet.X;
            Y = packet.Y;
            Z = packet.Z;
            Dir = packet.Dir;
            Passable = packet.Passable;
            HideName = packet.HideName;
            HideEntity = packet.HideEntity;
            NameColor = packet.NameColor;
            HeaderLabel = new Label(packet.HeaderLabel.Label, packet.HeaderLabel.Color);
            FooterLabel = new Label(packet.FooterLabel.Label, packet.FooterLabel.Color);

            var animsToClear = new List<Animation>();
            var animsToAdd = new List<AnimationBase>();
            for (var i = 0; i < packet.Animations.Length; i++)
            {
                var anim = AnimationBase.Get(packet.Animations[i]);
                if (anim != null)
                {
                    animsToAdd.Add(anim);
                }
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

                    foreach (var equipAnim in EquipmentAnimations)
                    {
                        if (equipAnim == anim)
                        {
                            animsToClear.Remove(anim);
                        }
                    }
                }
            }

            ClearAnimations(animsToClear);
            AddAnimations(animsToAdd);

            Vital = packet.Vital;
            MaxVital = packet.MaxVital;

            //Update status effects
            Status.Clear();

            if (packet.StatusEffects == null)
            {
                Log.Warn($"'{nameof(packet)}.{nameof(packet.StatusEffects)}' is null.");
            }
            else
            {
                foreach (var status in packet.StatusEffects)
                {
                    var instance = new Status(
                        status.SpellId, status.Type, status.TransformSprite, status.TimeRemaining, status.TotalDuration
                    );

                    Status?.Add(instance);

                    if (instance.Type == StatusTypes.Shield)
                    {
                        instance.Shield = status.VitalShields;
                    }
                }
            }

            SortStatuses();
            Stat = packet.Stats;

            mDisposed = false;

            //Status effects box update
            if (Globals.Me == null)
            {
                Log.Warn($"'{nameof(Globals.Me)}' is null.");
            }
            else
            {
                if (Id == Globals.Me.Id)
                {
                    if (Interface.Interface.GameUi == null)
                    {
                        Log.Warn($"'{nameof(Interface.Interface.GameUi)}' is null.");
                    }
                    else
                    {
                        if (Interface.Interface.GameUi.PlayerBox == null)
                        {
                            Log.Warn($"'{nameof(Interface.Interface.GameUi.PlayerBox)}' is null.");
                        }
                        else
                        {
                            Interface.Interface.GameUi.PlayerBox.UpdateStatuses = true;
                        }
                    }
                }
                else if (Id != Guid.Empty && Id == Globals.Me.TargetIndex)
                {
                    if (Globals.Me.TargetBox == null)
                    {
                        Log.Warn($"'{nameof(Globals.Me.TargetBox)}' is null.");
                    }
                    else
                    {
                        Globals.Me.TargetBox.UpdateStatuses = true;
                    }
                }
            }
        }

        public void AddAnimations(List<AnimationBase> anims)
        {
            foreach (var anim in anims)
            {
                Animations.Add(new Animation(anim, true, false, -1, this));
            }
        }

        public void ClearAnimations(List<Animation> anims)
        {
            if (anims == null)
            {
                anims = Animations;
            }

            if (anims.Count > 0)
            {
                for (var i = 0; i < anims.Count; i++)
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
                if (map == null || !map.InView())
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

            var ecTime = (float) (Globals.System.GetTimeMs() - mLastUpdate);
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
                        OffsetY -= (float) ecTime * (float) Options.TileHeight / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY < 0)
                        {
                            OffsetY = 0;
                        }

                        break;

                    case 1:
                        OffsetY += (float) ecTime * (float) Options.TileHeight / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY > 0)
                        {
                            OffsetY = 0;
                        }

                        break;

                    case 2:
                        OffsetX -= (float) ecTime * (float) Options.TileHeight / GetMovementTime();
                        OffsetY = 0;
                        if (OffsetX < 0)
                        {
                            OffsetX = 0;
                        }

                        break;

                    case 3:
                        OffsetX += (float) ecTime * (float) Options.TileHeight / GetMovementTime();
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
                for (var z = 0; z < Options.EquipmentSlots.Count; z++)
                {
                    if (Equipment[z] != Guid.Empty && (this != Globals.Me || MyEquipment[z] < Options.MaxInvItems))
                    {
                        var itemId = Guid.Empty;
                        if (this == Globals.Me)
                        {
                            var slot = MyEquipment[z];
                            if (slot > -1)
                            {
                                itemId = Inventory[slot].ItemId;
                            }
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
                            if (EquipmentAnimations[z] != null &&
                                (EquipmentAnimations[z].MyBase != anim || EquipmentAnimations[z].Disposed()))
                            {
                                EquipmentAnimations[z].Dispose();
                                Animations.Remove(EquipmentAnimations[z]);
                                EquipmentAnimations[z] = null;
                            }

                            if (EquipmentAnimations[z] == null)
                            {
                                EquipmentAnimations[z] = new Animation(anim, true, true, -1, this);
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
                if (AnimationFrame >= 4)
                {
                    AnimationFrame = 0;
                }
            }

            CalculateCenterPos();

            List<Animation> animsToRemove = null;
            foreach (var animInstance in Animations)
            {
                animInstance.Update();

                //If disposed mark to be removed and continue onward
                if (animInstance.Disposed())
                {
                    if (animsToRemove == null)
                    {
                        animsToRemove = new List<Animation>();
                    }

                    animsToRemove.Add(animInstance);

                    continue;
                }

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
                    animInstance.SetPosition(
                        (int) Math.Ceiling(GetCenterPos().X), (int) Math.Ceiling(GetCenterPos().Y), X, Y, CurrentMap,
                        Dir, Z
                    );
                }
                else
                {
                    animInstance.SetPosition(
                        (int) Math.Ceiling(GetCenterPos().X), (int) Math.Ceiling(GetCenterPos().Y), X, Y, CurrentMap,
                        -1, Z
                    );
                }
            }

            if (animsToRemove != null)
            {
                foreach (var anim in animsToRemove)
                {
                    Animations.Remove(anim);
                }
            }

            mLastUpdate = Globals.System.GetTimeMs();

            return true;
        }

        public virtual int CalculateAttackTime()
        {
            return (int) (Options.MaxAttackRate +
                          (float) ((Options.MinAttackRate - Options.MaxAttackRate) *
                                   (((float) Options.MaxStatValue - Stat[(int) Stats.Speed]) /
                                    (float) Options.MaxStatValue)));
        }

        public virtual bool IsStealthed()
        {
            //If the entity has transformed, apply that sprite instead.
            if (this == Globals.Me)
            {
                return false;
            }

            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Stealth)
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
            for (var x = gridX - 1; x <= gridX + 1; x++)
            {
                for (var y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 &&
                        x < Globals.MapGridWidth &&
                        y >= 0 &&
                        y < Globals.MapGridHeight &&
                        Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        if (Globals.MapGrid[x, y] == CurrentMap)
                        {
                            var priority = mRenderPriority;
                            if (Z != 0)
                            {
                                priority += 3;
                            }

                            HashSet<Entity> renderSet;

                            if (y == gridY - 1)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight + Y];
                            }
                            else if (y == gridY)
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 2 + Y];
                            }
                            else
                            {
                                renderSet = Graphics.RenderingEntities[priority, Options.MapHeight * 3 + Y];
                            }

                            renderSet.Add(this);
                            renderList = renderSet;

                            return renderList;
                        }
                    }
                }
            }

            return renderList;
        }

        //Rendering Functions
        public virtual void Draw()
        {
            if (HideEntity)
            {
                return; //Don't draw if the entity is hidden
            }

            WorldPos.Reset();
            var map = MapInstance.Get(CurrentMap);
            if (map == null || !Globals.GridMaps.Contains(CurrentMap))
            {
                return;
            }

            var srcRectangle = new FloatRect();
            var destRectangle = new FloatRect();
            var d = 0;

            var sprite = "";
            var alpha = 255;

            //If the entity has transformed, apply that sprite instead.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Transform)
                {
                    sprite = Status[n].Data;
                    TransformedSprite = sprite;
                }

                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
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
                    destRectangle.X = map.GetX() + X * Options.TileWidth + OffsetX + Options.TileWidth / 2;
                    destRectangle.Y = GetCenterPos().Y - Texture.GetHeight() / 8;
                }
                else
                {
                    destRectangle.X = map.GetX() + X * Options.TileWidth + OffsetX + Options.TileWidth / 2;
                    destRectangle.Y = map.GetY() + Y * Options.TileHeight + OffsetY;
                }

                destRectangle.X -= Texture.GetWidth() / 8;
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
                    srcRectangle = new FloatRect(
                        AnimationFrame * (int) Texture.GetWidth() / 4, d * (int) Texture.GetHeight() / 4,
                        (int) Texture.GetWidth() / 4, (int) Texture.GetHeight() / 4
                    );
                }
                else
                {
                    var attackTime = CalculateAttackTime();
                    if (AttackTimer - CalculateAttackTime() / 2 > Globals.System.GetTimeMs() || Blocking)
                    {
                        srcRectangle = new FloatRect(
                            3 * (int) Texture.GetWidth() / 4, d * (int) Texture.GetHeight() / 4,
                            (int) Texture.GetWidth() / 4, (int) Texture.GetHeight() / 4
                        );
                    }
                    else
                    {
                        srcRectangle = new FloatRect(
                            WalkFrame * (int) Texture.GetWidth() / 4, d * (int) Texture.GetHeight() / 4,
                            (int) Texture.GetWidth() / 4, (int) Texture.GetHeight() / 4
                        );
                    }
                }

                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;

                //GameGraphics.DrawGameTexture(Texture, srcRectangle, destRectangle,
                //    new Intersect.Color(alpha, 255, 255, 255));

                WorldPos = destRectangle;

                //Order the layers of paperdolls and sprites
                for (var z = 0; z < Options.PaperdollOrder[Dir].Count; z++)
                {
                    var paperdoll = Options.PaperdollOrder[Dir][z];
                    var equipSlot = Options.EquipmentSlots.IndexOf(paperdoll);

                    //Check for player
                    if (paperdoll == "Player")
                    {
                        Graphics.DrawGameTexture(
                            Texture, srcRectangle, destRectangle, new Intersect.Color(alpha, 255, 255, 255)
                        );
                    }
                    else if (equipSlot > -1)
                    {
                        //Don't render the paperdolls if they have transformed.
                        if (sprite == MySprite && Equipment.Length == Options.EquipmentSlots.Count)
                        {
                            if (Equipment[equipSlot] != Guid.Empty && this != Globals.Me ||
                                MyEquipment[equipSlot] < Options.MaxInvItems)
                            {
                                var itemId = Guid.Empty;
                                if (this == Globals.Me)
                                {
                                    var slot = MyEquipment[equipSlot];
                                    if (slot > -1)
                                    {
                                        itemId = Inventory[slot].ItemId;
                                    }
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
            for (var i = chatbubbles.Length - 1; i > -1; i--)
            {
                bubbleoffset = chatbubbles[i].Draw(bubbleoffset);
            }
        }

        public virtual void DrawEquipment(string filename, int alpha)
        {
            var map = MapInstance.Get(CurrentMap);
            if (map == null)
            {
                return;
            }

            var srcRectangle = new FloatRect();
            var destRectangle = new FloatRect();
            var d = 0;
            var paperdollTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Paperdoll, filename);
            if (paperdollTex != null)
            {
                if (paperdollTex.GetHeight() / 4 > Options.TileHeight)
                {
                    destRectangle.X = map.GetX() + X * Options.TileWidth + OffsetX;
                    destRectangle.Y = GetCenterPos().Y - paperdollTex.GetHeight() / 8;
                }
                else
                {
                    destRectangle.X = map.GetX() + X * Options.TileWidth + OffsetX;
                    destRectangle.Y = map.GetY() + Y * Options.TileHeight + OffsetY;
                }

                if (paperdollTex.GetWidth() / 4 > Options.TileWidth)
                {
                    destRectangle.X -= (paperdollTex.GetWidth() / 4 - Options.TileWidth) / 2;
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
                    srcRectangle = new FloatRect(
                        3 * (int) paperdollTex.GetWidth() / 4, d * (int) paperdollTex.GetHeight() / 4,
                        (int) paperdollTex.GetWidth() / 4, (int) paperdollTex.GetHeight() / 4
                    );
                }
                else
                {
                    srcRectangle = new FloatRect(
                        WalkFrame * (int) paperdollTex.GetWidth() / 4, d * (int) paperdollTex.GetHeight() / 4,
                        (int) paperdollTex.GetWidth() / 4, (int) paperdollTex.GetHeight() / 4
                    );
                }

                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;
                Graphics.DrawGameTexture(
                    paperdollTex, srcRectangle, destRectangle, new Intersect.Color(alpha, 255, 255, 255)
                );
            }
        }

        protected virtual void CalculateCenterPos()
        {
            var pos = new Pointf(
                LatestMap.GetX() + X * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                LatestMap.GetY() + Y * Options.TileHeight + OffsetY + Options.TileHeight / 2
            );

            if (Texture != null)
            {
                pos.Y += Options.TileHeight / 2;
                pos.Y -= Texture.GetHeight() / 8;
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
                y = y - (int) (overrideHeight / 8);
                y -= 12;
            }
            else
            {
                if (Texture != null)
                {
                    y = y - (int) (Texture.GetHeight() / 8);
                    y -= 12;
                }
            }

            if (GetType() != typeof(Event))
            {
                y -= 10;
            } //Need room for HP bar if not an event.

            return y;
        }

        public void DrawLabels(
            string label,
            int position,
            Color labelColor,
            Color textColor,
            Color borderColor = null,
            Color backgroundColor = null
        )
        {
            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            if (label.Trim().Length == 0)
            {
                return;
            }

            if (HideName)
            {
                return;
            }

            if (borderColor == null)
            {
                borderColor = Color.Transparent;
            }

            if (backgroundColor == null)
            {
                backgroundColor = Color.Transparent;
            }

            //If we have a non-transparent label color then use it, otherwise use the players name color
            if (labelColor != null && labelColor.A != 0)
            {
                textColor = labelColor;
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
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

            var textSize = Graphics.Renderer.MeasureText(label, Graphics.EntityNameFont, 1);

            var x = (int) Math.Ceiling(GetCenterPos().X);
            var y = position == 0 ? GetLabelLocation(LabelType.Header) : GetLabelLocation(LabelType.Footer);

            if (backgroundColor != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y), backgroundColor
                );
            }

            Graphics.Renderer.DrawString(
                label, Graphics.EntityNameFont, (int) (x - (int) Math.Ceiling(textSize.X / 2f)), (int) y, 1,
                Color.FromArgb(textColor.ToArgb()), true, null, Color.FromArgb(borderColor.ToArgb())
            );
        }

        public virtual void DrawName(Color textColor, Color borderColor = null, Color backgroundColor = null)
        {
            if (HideName || Name.Trim().Length == 0)
            {
                return;
            }

            if (borderColor == null)
            {
                borderColor = Color.Transparent;
            }

            if (backgroundColor == null)
            {
                backgroundColor = Color.Transparent;
            }

            //Check for npc colors
            if (textColor == null)
            {
                LabelColor? color = null;
                switch (Type)
                {
                    case -1: //When entity has a target (showing aggression)
                        color = CustomColors.Names.Npcs["Aggressive"];

                        break;
                    case 0: //Attack when attacked
                        color = CustomColors.Names.Npcs["AttackWhenAttacked"];

                        break;
                    case 1: //Attack on sight
                        color = CustomColors.Names.Npcs["AttackOnSight"];

                        break;
                    case 3: //Guard
                        color = CustomColors.Names.Npcs["Guard"];

                        break;
                    case 2: //Neutral
                    default:
                        color = CustomColors.Names.Npcs["Neutral"];

                        break;
                }

                if (color != null)
                {
                    textColor = color?.Name;
                    backgroundColor = color?.Background;
                    borderColor = color?.Outline;
                }
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
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

            var textSize = Graphics.Renderer.MeasureText(Name, Graphics.EntityNameFont, 1);

            var x = (int) Math.Ceiling(GetCenterPos().X);
            var y = GetLabelLocation(LabelType.Name);

            if (backgroundColor != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y), backgroundColor
                );
            }

            Graphics.Renderer.DrawString(
                Name, Graphics.EntityNameFont, (int) (x - (int) Math.Ceiling(textSize.X / 2f)), (int) y, 1,
                Color.FromArgb(textColor.ToArgb()), true, null, Color.FromArgb(borderColor.ToArgb())
            );
        }

        public float GetLabelLocation(LabelType type)
        {
            var y = GetTopPos() - 4;
            switch (type)
            {
                case LabelType.Header:
                    if (string.IsNullOrWhiteSpace(HeaderLabel.Text))
                    {
                        return GetLabelLocation(LabelType.Name);
                    }

                    y = GetLabelLocation(LabelType.Name);
                    var headerSize = Graphics.Renderer.MeasureText(HeaderLabel.Text, Graphics.EntityNameFont, 1);
                    y -= headerSize.Y;

                    break;
                case LabelType.Footer:
                    if (string.IsNullOrWhiteSpace(FooterLabel.Text))
                    {
                        break;
                    }

                    var footerSize = Graphics.Renderer.MeasureText(FooterLabel.Text, Graphics.EntityNameFont, 1);
                    y -= footerSize.Y - 8;

                    break;
                case LabelType.Name:
                    y = GetLabelLocation(LabelType.Footer);
                    var nameSize = Graphics.Renderer.MeasureText(Name, Graphics.EntityNameFont, 1);
                    if (string.IsNullOrEmpty(FooterLabel.Text))
                    {
                        y -= nameSize.Y - 8;
                    }
                    else
                    {
                        y -= nameSize.Y;
                    }

                    break;
                case LabelType.ChatBubble:
                    y = GetLabelLocation(LabelType.Header) - 4;

                    break;
            }

            return y;
        }

        public void DrawHpBar()
        {
            if (HideName && HideEntity)
            {
                return;
            }

            if (Vital[(int) Vitals.Health] <= 0)
            {
                return;
            }

            var maxVital = MaxVital[(int) Vitals.Health];
            var shieldSize = 0;

            //Check for shields
            foreach (var status in Status)
            {
                if (status.Type == StatusTypes.Shield)
                {
                    shieldSize += status.Shield[(int) Vitals.Health];
                    maxVital += status.Shield[(int) Vitals.Health];
                }
            }

            if (shieldSize + Vital[(int) Vitals.Health] > maxVital)
            {
                maxVital = shieldSize + Vital[(int) Vitals.Health];
            }

            if (Vital[(int) Vitals.Health] == MaxVital[(int) Vitals.Health] && shieldSize <= 0)
            {
                return;
            }

            //Check for stealth amoungst status effects.
            for (var n = 0; n < Status.Count; n++)
            {
                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
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

            var hpfillRatio = (float) Vital[(int) Vitals.Health] / maxVital;
            hpfillRatio = Math.Min(1, Math.Max(0, hpfillRatio));
            var hpfillWidth = (float) Math.Ceiling(hpfillRatio * width);

            var shieldfillRatio = (float) shieldSize / maxVital;
            shieldfillRatio = Math.Min(1, Math.Max(0, shieldfillRatio));
            var shieldfillWidth = (float) Math.Floor(shieldfillRatio * width);

            var y = (int) Math.Ceiling(GetCenterPos().Y);
            var x = (int) Math.Ceiling(GetCenterPos().X);
            var entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
            if (entityTex != null)
            {
                y = y - (int) (entityTex.GetHeight() / 8);
                y -= 8;
            }

            var hpBackground = Globals.ContentManager.GetTexture(
                GameContentManager.TextureType.Misc, "hpbackground.png"
            );

            var hpForeground = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "hpbar.png");
            var shieldForeground = Globals.ContentManager.GetTexture(
                GameContentManager.TextureType.Misc, "shieldbar.png"
            );

            if (hpBackground != null)
            {
                Graphics.DrawGameTexture(
                    hpBackground, new FloatRect(0, 0, hpBackground.GetWidth(), hpBackground.GetHeight()),
                    new FloatRect((int) (x - width / 2), (int) (y - 1), width, 6), Color.White
                );
            }

            if (hpForeground != null)
            {
                Graphics.DrawGameTexture(
                    hpForeground, new FloatRect(0, 0, hpfillWidth, hpForeground.GetHeight()),
                    new FloatRect((int) (x - width / 2), (int) (y - 1), hpfillWidth, 6), Color.White
                );
            }

            if (shieldSize > 0 && shieldForeground != null) //Check for a shield to render
            {
                Graphics.DrawGameTexture(
                    shieldForeground,
                    new FloatRect((float) (width - shieldfillWidth), 0, shieldfillWidth, shieldForeground.GetHeight()),
                    new FloatRect((int) (x - width / 2) + hpfillWidth, (int) (y - 1), shieldfillWidth, 6), Color.White
                );
            }
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
                var fillratio = (castSpell.CastDuration - (CastTime - Globals.System.GetTimeMs())) /
                                (float) castSpell.CastDuration;

                var castFillWidth = fillratio * width;
                var y = (int) Math.Ceiling(GetCenterPos().Y);
                var x = (int) Math.Ceiling(GetCenterPos().X);
                var entityTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Entity, MySprite);
                if (entityTex != null)
                {
                    y = y + (int) (entityTex.GetHeight() / 8);
                    y += 3;
                }

                var castBackground = Globals.ContentManager.GetTexture(
                    GameContentManager.TextureType.Misc, "castbackground.png"
                );

                var castForeground = Globals.ContentManager.GetTexture(
                    GameContentManager.TextureType.Misc, "castbar.png"
                );

                if (castBackground != null)
                {
                    Graphics.DrawGameTexture(
                        castBackground, new FloatRect(0, 0, castBackground.GetWidth(), castBackground.GetHeight()),
                        new FloatRect((int) (x - width / 2), (int) (y - 1), width, 6), Color.White
                    );
                }

                if (castForeground != null)
                {
                    Graphics.DrawGameTexture(
                        castForeground,
                        new FloatRect(0, 0, castForeground.GetWidth() * fillratio, castForeground.GetHeight()),
                        new FloatRect((int) (x - width / 2), (int) (y - 1), castFillWidth, 6), Color.White
                    );
                }
            }
        }

        //
        public void DrawTarget(int priority)
        {
            if (GetType() == typeof(Projectile))
            {
                return;
            }

            var map = MapInstance.Get(CurrentMap);
            if (map == null)
            {
                return;
            }

            var srcRectangle = new FloatRect();
            var destRectangle = new FloatRect();
            var targetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "target.png");
            if (targetTex != null)
            {
                destRectangle.X = GetCenterPos().X - (int) targetTex.GetWidth() / 4;
                destRectangle.Y = GetCenterPos().Y - (int) targetTex.GetHeight() / 2;

                srcRectangle = new FloatRect(
                    priority * (int) targetTex.GetWidth() / 2, 0, (int) targetTex.GetWidth() / 2,
                    (int) targetTex.GetHeight()
                );

                destRectangle.Width = srcRectangle.Width;
                destRectangle.Height = srcRectangle.Height;

                Graphics.DrawGameTexture(targetTex, srcRectangle, destRectangle, Intersect.Color.White);
            }
        }

        public virtual bool CanBeAttacked()
        {
            return true;
        }

        //Chatting
        public void AddChatBubble(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            mChatBubbles.Add(new ChatBubble(this, text));
        }

        //Statuses
        public bool StatusActive(Guid guid)
        {
            foreach (var status in Status)
            {
                if (status.SpellId == guid && status.IsActive())
                {
                    return true;
                }
            }

            return false;
        }

        public Status GetStatus(Guid guid)
        {
            foreach (var status in Status)
            {
                if (status.SpellId == guid && status.IsActive())
                {
                    return status;
                }
            }

            return null;
        }

        public void SortStatuses()
        {
            //Sort Status effects by remaining time
            Status = Status.OrderByDescending(x => x.RemainingMs()).ToList();
        }

        ~Entity()
        {
            Dispose();
        }

    }

}

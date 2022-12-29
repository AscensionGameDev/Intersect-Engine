using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Items;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.Spells;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

namespace Intersect.Client.Entities
{

    public partial class Entity : IEntity
    {
        public int AnimationFrame { get; set; }

        //Entity Animations
        public List<Animation> Animations { get; set; } = new List<Animation>();

        //Animation Timer (for animated sprites)
        public long AnimationTimer { get; set; }

        //Combat
        public long AttackTimer { get; set; } = 0;
        public int AttackTime { get; set; } = -1;

        public bool IsBlocking { get; set; } = false;

        //Combat Status
        public long CastTime { get; set; } = 0;

        public bool IsCasting => CastTime > Timing.Global.Milliseconds;

        public bool IsDashing => Dashing != null;

        //Dashing instance
        public Dash Dashing { get; set; }

        public IDash CurrentDash => Dashing as IDash;

        public Queue<Dash> DashQueue { get; set; } = new Queue<Dash>();

        public long DashTimer { get; set; }

        public float elapsedtime { get; set; } //to be removed

        private Guid[] _equipment = new Guid[Options.EquipmentSlots.Count];

        public Guid[] Equipment
        {
            get => _equipment;
            set
            {
                if (_equipment == value)
                {
                    return;
                }

                _equipment = value;
                LoadAnimationTexture(Sprite ?? TransformedSprite, SpriteAnimations.Weapon);
            }
        }

        IReadOnlyList<int> IEntity.EquipmentSlots => MyEquipment.ToList();

        public Animation[] EquipmentAnimations { get; set; } = new Animation[Options.EquipmentSlots.Count];

        //Extras
        public string Face { get; set; } = "";

        public Label FooterLabel { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        public Label HeaderLabel { get; set; }

        public bool IsHidden { get; set; } = false;

        public bool HideName { get; set; }

        //Core Values
        public Guid Id { get; set; }

        //Inventory/Spells/Equipment
        public IItem[] Inventory { get; set; } = new IItem[Options.MaxInvItems];

        IReadOnlyList<IItem> IEntity.Items => Inventory.ToList();

        public bool InView { get; set; } = true;

        public bool IsLocal { get; set; } = false;

        public bool IsMoving { get; set; }

        //Caching
        public IMapInstance LatestMap { get; set; }

        public int Level { get; set; } = 1;

        //Vitals & Stats
        public int[] MaxVital { get; set; } = new int[(int)Vitals.VitalCount];

        IReadOnlyList<int> IEntity.MaxVitals => MaxVital.ToList();

        protected Pointf mOrigin = Pointf.Empty;

        //Chat
        private List<ChatBubble> mChatBubbles = new List<ChatBubble>();

        private byte mDir;

        protected bool mDisposed;

        private long mLastUpdate;

        protected string mMySprite = "";

        public Color Color { get; set; } = new Color(255, 255, 255, 255);

        public int MoveDir { get; set; } = -1;

        public long MoveTimer { get; set; }

        protected byte mRenderPriority = 1;

        protected string mTransformedSprite = "";

        private long mWalkTimer;

        public int[] MyEquipment { get; set; } = new int[Options.EquipmentSlots.Count];

        public string Name { get; set; } = "";

        public Color NameColor { get; set; } = null;

        public float OffsetX { get; set; }

        public float OffsetY { get; set; }

        public bool Passable { get; set; }

        //Rendering Variables
        public HashSet<Entity> RenderList { get; set; }

        private Guid _spellCast;

        public Guid SpellCast
        {
            get => _spellCast;
            set
            {
                if (value == SpellCast)
                {
                    return;
                }

                _spellCast = value;
                LoadAnimationTexture(Sprite ?? TransformedSprite, SpriteAnimations.Cast);
            }
        }

        public Spell[] Spells { get; set; } = new Spell[Options.MaxPlayerSkills];

        IReadOnlyList<Guid> IEntity.Spells => Spells.Select(x => x.Id).ToList();

        public int[] Stat { get; set; } = new int[(int)Stats.StatCount];

        IReadOnlyList<int> IEntity.Stats => Stat.ToList();

        public int Target { get; set; } = -1;

        public GameTexture Texture { get; set; }

        #region "Animation Textures and Timing"
        public SpriteAnimations SpriteAnimation { get; set; } = SpriteAnimations.Normal;

        public Dictionary<SpriteAnimations, GameTexture> AnimatedTextures { get; set; } = new Dictionary<SpriteAnimations, GameTexture>();

        public int SpriteFrame { get; set; } = 0;

        public long SpriteFrameTimer { get; set; } = -1;

        public long LastActionTime { get; set; } = -1;

        public long AutoTurnToTargetTimer { get; set; } = -1;

        #endregion

        public EntityTypes Type { get; }

        public int Aggression { get; set; }

        public int[] Vital { get; set; } = new int[(int)Vitals.VitalCount];

        IReadOnlyList<int> IEntity.Vitals => Vital.ToList();

        public int WalkFrame { get; set; }

        public FloatRect WorldPos { get; set; } = new FloatRect();
        
        public bool IsHovered { get; set; }

        //Location Info
        public byte X { get; set; }

        public byte Y { get; set; }

        public byte Z { get; set; }

        public Entity(Guid id, EntityPacket packet, EntityTypes entityType)
        {
            Id = id;
            Type = entityType;
            MapId = Guid.Empty;

            if (Id != Guid.Empty && Type != EntityTypes.Event)
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

            AnimationTimer = Timing.Global.Milliseconds + Globals.Random.Next(0, 500);

            //TODO Remove because fixed orrrrr change the exception text
            if (Options.EquipmentSlots.Count == 0)
            {
                throw new Exception("What the fuck is going on!?!?!?!?!?!");
            }

            Load(packet);
        }

        //Status effects
        public List<IStatus> Status { get; private set; } = new List<IStatus>();

        IReadOnlyList<IStatus> IEntity.Status => Status;

        public Pointf Origin => LatestMap == default ? Pointf.Empty : mOrigin;

        protected virtual Pointf CenterOffset => (Texture == default) ? Pointf.Empty : (Pointf.UnitY * Texture.Center.Y / Options.Instance.Sprites.Directions);

        public Pointf Center => Origin - CenterOffset;

        public byte Dir
        {
            get => mDir;
            set => mDir = (byte)((value + Options.Instance.Sprites.Directions) % Options.Instance.Sprites.Directions);
        }

        public virtual string TransformedSprite
        {
            get => mTransformedSprite;
            set
            {
                if (mTransformedSprite == value)
                {
                    return;
                }

                mTransformedSprite = value;

                var textureName = string.IsNullOrEmpty(mTransformedSprite) ? mMySprite : mTransformedSprite;
                LoadTextures(textureName);
            }
        }

        public virtual string Sprite
        {
            get => mMySprite;
            set
            {
                if (mMySprite == value)
                {
                    return;
                }

                mMySprite = value;
                LoadTextures(mMySprite);
            }
        }

        public virtual int SpriteFrames
        {
            get
            {
                switch (SpriteAnimation)
                {
                    case SpriteAnimations.Normal:
                        return Options.Instance.Sprites.NormalFrames;
                    case SpriteAnimations.Idle:
                        return Options.Instance.Sprites.IdleFrames;
                    case SpriteAnimations.Attack:
                        return Options.Instance.Sprites.AttackFrames;
                    case SpriteAnimations.Shoot:
                        return Options.Instance.Sprites.ShootFrames;
                    case SpriteAnimations.Cast:
                        return Options.Instance.Sprites.CastFrames;
                    case SpriteAnimations.Weapon:
                        return Options.Instance.Sprites.WeaponFrames;
                }

                return Options.Instance.Sprites.NormalFrames;

            }
        }

        public IMapInstance MapInstance => Maps.MapInstance.Get(MapId);

        public virtual Guid MapId { get; set; }

        //Deserializing
        public virtual void Load(EntityPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            MapId = packet.MapId;
            Name = packet.Name;
            Sprite = packet.Sprite;
            Color = packet.Color;
            Face = packet.Face;
            Level = packet.Level;
            X = packet.X;
            Y = packet.Y;
            Z = packet.Z;
            Dir = packet.Dir;
            Passable = packet.Passable;
            HideName = packet.HideName;
            IsHidden = packet.HideEntity;
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
            if (IsBlocking)
            {
                time += time * (float)Options.BlockingSlow;
            }

            return Math.Min(1000f, time);
        }

        public override string ToString() => Name;

        //Movement Processing
        public virtual bool Update()
        {
            if (mDisposed)
            {
                LatestMap = null;

                return false;
            }
            else
            {
                if (LatestMap?.Id != MapId)
                {
                    LatestMap = Maps.MapInstance.Get(MapId);
                }

                if (LatestMap == null || !LatestMap.InView())
                {
                    Globals.EntitiesToDispose.Add(Id);

                    return false;
                }
            }

            RenderList = DetermineRenderOrder(RenderList, LatestMap);
            if (mLastUpdate == 0)
            {
                mLastUpdate = Timing.Global.Milliseconds;
            }

            var ecTime = (float)(Timing.Global.Milliseconds - mLastUpdate);
            elapsedtime = ecTime;
            if (Dashing != null)
            {
                WalkFrame = Options.Instance.Sprites.NormalSheetDashFrame; //Fix the frame whilst dashing
            }
            else if (mWalkTimer < Timing.Global.Milliseconds)
            {
                if (!IsMoving && DashQueue.Count > 0)
                {
                    Dashing = DashQueue.Dequeue();
                    Dashing.Start(this);
                    OffsetX = 0;
                    OffsetY = 0;
                    DashTimer = Timing.Global.Milliseconds + Options.MaxDashSpeed;
                }
                else
                {
                    if (IsMoving)
                    {
                        WalkFrame++;
                        if (WalkFrame >= SpriteFrames)
                        {
                            WalkFrame = 0;
                        }
                    }
                    else
                    {
                        if (WalkFrame > 0 && WalkFrame / SpriteFrames < 0.7f)
                        {
                            WalkFrame = (int)SpriteFrames / 2;
                        }
                        else
                        {
                            WalkFrame = 0;
                        }
                    }

                    mWalkTimer = Timing.Global.Milliseconds + Options.Instance.Sprites.MovingFrameDuration;
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
                        OffsetY -= (float)ecTime * (float)Options.TileHeight / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY < 0)
                        {
                            OffsetY = 0;
                        }

                        break;

                    case 1:
                        OffsetY += (float)ecTime * (float)Options.TileHeight / GetMovementTime();
                        OffsetX = 0;
                        if (OffsetY > 0)
                        {
                            OffsetY = 0;
                        }

                        break;

                    case 2:
                        OffsetX -= (float)ecTime * (float)Options.TileHeight / GetMovementTime();
                        OffsetY = 0;
                        if (OffsetX < 0)
                        {
                            OffsetX = 0;
                        }

                        break;

                    case 3:
                        OffsetX += (float)ecTime * (float)Options.TileHeight / GetMovementTime();
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

            if (AnimationTimer < Timing.Global.Milliseconds)
            {
                AnimationTimer = Timing.Global.Milliseconds + 200;
                AnimationFrame++;
                if (AnimationFrame >= SpriteFrames)
                {
                    AnimationFrame = 0;
                }
            }

            CalculateOrigin();

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

                if (IsStealthed)
                {
                    animInstance.Hide();
                }
                else
                {
                    animInstance.Show();
                }

                var animationDirection = animInstance.AutoRotate ? Dir : -1;
                animInstance.SetPosition(
                    (int)Math.Ceiling(Center.X),
                    (int)Math.Ceiling(Center.Y),
                    X,
                    Y,
                    MapId,
                    animationDirection, Z
                );
            }

            if (animsToRemove != null)
            {
                foreach (var anim in animsToRemove)
                {
                    Animations.Remove(anim);
                }
            }

            mLastUpdate = Timing.Global.Milliseconds;

            UpdateSpriteAnimation();

            return true;
        }

        public virtual int CalculateAttackTime()
        {
            //If this is an npc we don't know it's attack time. Luckily the server provided it!
            if (this != Globals.Me && AttackTime > -1)
            {
                return AttackTime;
            }

            //Otherwise return the legacy attack speed calculation
            return (int)(Options.MaxAttackRate +
                          (float)((Options.MinAttackRate - Options.MaxAttackRate) *
                                   (((float)Options.MaxStatValue - Stat[(int)Stats.Speed]) /
                                    (float)Options.MaxStatValue)));
        }

        /// <summary>
        /// Returns whether this entity is Stealthed or not.
        /// </summary>
        public virtual bool IsStealthed
        {
            get
            {
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
        }

        /// <summary>
        /// Returns whether this entity should be drawn.
        /// </summary>
        public virtual bool ShouldDraw => !IsHidden && (!IsStealthed || this == Globals.Me || Globals.Me.IsInMyParty(Id));

        /// <summary>
        /// Returns whether the name of this entity should be drawn.
        /// </summary>
        public virtual bool ShouldDrawName => !HideName && ShouldDraw;

        public virtual HashSet<Entity> DetermineRenderOrder(HashSet<Entity> renderList, IMapInstance map)
        {
            if (renderList != null)
            {
                renderList.Remove(this);
            }

            if (map == null || Globals.Me == null || Globals.Me.MapInstance == null)
            {
                return null;
            }

            var gridX = Globals.Me.MapInstance.GridX;
            var gridY = Globals.Me.MapInstance.GridY;
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
                        if (Globals.MapGrid[x, y] == MapId)
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
            if (IsHidden)
            {
                return; //Don't draw if the entity is hidden
            }

            WorldPos.Reset();
            var map = Maps.MapInstance.Get(MapId);
            if (map == null || !Globals.GridMaps.Contains(MapId))
            {
                return;
            }

            var sprite = "";
            // Copy the actual render color, because we'll be editing it later and don't want to overwrite it.
            var renderColor = new Color(Color.A, Color.R, Color.G, Color.B);

            string transformedSprite = "";

            //If the entity has transformed, apply that sprite instead.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == StatusTypes.Transform)
                {
                    sprite = Status[n].Data;
                    transformedSprite = sprite;
                }

                //If unit is stealthed, don't render unless the entity is the player.
                if (Status[n].Type == StatusTypes.Stealth)
                {
                    if (this != Globals.Me && !(this is Player player && Globals.Me.IsInMyParty(player)))
                    {
                        return;
                    }
                    else
                    {
                        renderColor.A /= 2;
                    }
                }
            }

            if (transformedSprite != TransformedSprite)
            {
                TransformedSprite = transformedSprite;
            }

            //Check if there is no transformed sprite set
            if (string.IsNullOrEmpty(sprite))
            {
                sprite = Sprite;
                Sprite = sprite;
            }

            if (!AnimatedTextures.TryGetValue(SpriteAnimation, out var texture))
            {
                texture = Texture;
            }

            if (texture == default)
            {
                // We don't have a texture to render, but we still want this to be targetable.
                WorldPos = new FloatRect(
                    map.GetX() + X * Options.TileWidth + OffsetX,
                    map.GetY() + Y * Options.TileHeight + OffsetY,
                    Options.TileWidth,
                    Options.TileHeight);
                return;
            }

            var d = (Dir + (Options.Instance.Sprites.Directions - 1)) % Options.Instance.Sprites.Directions;

            var frameWidth = texture.GetWidth() / SpriteFrames;
            var frameHeight = texture.GetHeight() / Options.Instance.Sprites.Directions;

            var frame = SpriteFrame;
            if (Options.AnimatedSprites.Contains(sprite.ToLower()))
            {
                frame = AnimationFrame;
            }
            else if (SpriteAnimation == SpriteAnimations.Normal)
            {
                frame = AttackTimer - CalculateAttackTime() / 2 > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond || IsBlocking
                    ? Options.Instance.Sprites.NormalSheetAttackFrame
                    : WalkFrame;
            }

            var srcRectangle = new FloatRect(frame * frameWidth, d * frameHeight, frameWidth, frameHeight);
            var destRectangle = new FloatRect(
                (int)Math.Ceiling(Origin.X - frameWidth / 2f),
                (int)Math.Ceiling(Origin.Y - frameHeight),
                srcRectangle.Width,
                srcRectangle.Height
            );

            WorldPos = destRectangle;

            //Order the layers of paperdolls and sprites
            for (var z = 0; z < Options.PaperdollOrder[Dir].Count; z++)
            {
                var paperdoll = Options.PaperdollOrder[Dir][z];
                var equipSlot = Options.EquipmentSlots.IndexOf(paperdoll);

                //Check for player
                if (string.Equals("Player", paperdoll, StringComparison.Ordinal))
                {
                    Graphics.DrawGameTexture(texture, srcRectangle, destRectangle, renderColor);
                }
                else if (equipSlot > -1)
                {
                    //Don't render the paperdolls if they have transformed.
                    if (sprite == Sprite && Equipment.Length == Options.EquipmentSlots.Count)
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
                            if (ItemBase.TryGet(itemId, out var itemDescriptor))
                            {
                                var itemPaperdoll = Gender == 0 ? itemDescriptor.MalePaperdoll : itemDescriptor.FemalePaperdoll;
                                DrawEquipment(itemPaperdoll, item.Color * renderColor, destRectangle);
                            }
                        }
                    }
                }
            }
        }

        public void DrawChatBubbles()
        {
            //Don't draw if the entity is hidden
            if (IsHidden)
            {
                return;
            }

            //If unit is stealthed, don't render unless the entity is the player or party member.
            if (!ShouldDraw)
            {
                return;
            }

            var chatbubbles = mChatBubbles.ToArray();
            var bubbleoffset = 0f;
            for (var i = chatbubbles.Length - 1; i > -1; i--)
            {
                bubbleoffset = chatbubbles[i].Draw(bubbleoffset);
            }
        }

        public virtual void DrawEquipment(string filename, Color renderColor, FloatRect entityRect)
        {
            var map = Maps.MapInstance.Get(MapId);
            if (map == null || map.HideEquipment)
            {
                return;
            }

            var filenameNoExt = Path.GetFileNameWithoutExtension(filename);
            string filenameSuffix = SpriteAnimation.ToString();

            if (SpriteAnimation == SpriteAnimations.Attack ||
                SpriteAnimation == SpriteAnimations.Cast ||
                SpriteAnimation == SpriteAnimations.Weapon)
            {
                var animationName = Path.GetFileNameWithoutExtension(AnimatedTextures[SpriteAnimation].Name);
                int separatorIndex = animationName.IndexOf('_') + 1;
                filenameSuffix = animationName.Substring(separatorIndex);
            }

            var paperdollTex = Globals.ContentManager.GetTexture(
                TextureType.Paperdoll, $"{filenameNoExt}_{filenameSuffix}.png"
            );

            var spriteFrames = SpriteFrames;

            if (paperdollTex == null)
            {
                paperdollTex = Globals.ContentManager.GetTexture(TextureType.Paperdoll, filename);
                spriteFrames = Options.Instance.Sprites.NormalFrames;
            }

            if (paperdollTex == null)
            {
                return;
            }

            var d = (Dir + (Options.Instance.Sprites.Directions -1)) % Options.Instance.Sprites.Directions;

            var frameWidth = paperdollTex.GetWidth() / spriteFrames;
            var frameHeight = paperdollTex.GetHeight() / Options.Instance.Sprites.Directions;

            var frame = SpriteFrame;
            if (SpriteAnimation == SpriteAnimations.Normal)
            {
                frame = (AttackTimer - CalculateAttackTime() / 2 > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond || IsBlocking) ? 3 : WalkFrame;
            }

            var srcRectangle = new FloatRect(frame * frameWidth, d * frameHeight, frameWidth, frameHeight);
            var destRectangle = new FloatRect(
                (int)Math.Ceiling(Center.X - frameWidth / 2f),
                (int)Math.Ceiling(Center.Y - frameHeight / 2f),
                srcRectangle.Width,
                srcRectangle.Height
            );

            Graphics.DrawGameTexture(paperdollTex, srcRectangle, destRectangle, renderColor);
        }

        protected virtual bool CalculateOrigin()
        {
            if (LatestMap == default)
            {
                mOrigin = default;
                return false;
            }

            mOrigin = new Pointf(
                LatestMap.X + X * Options.TileWidth + OffsetX + Options.TileWidth / 2,
                LatestMap.Y + Y * Options.TileHeight + OffsetY + Options.TileHeight
            );

            return true;
        }

        public virtual float GetTop(int overrideHeight = -1)
        {
            if (LatestMap == default)
            {
                return 0f;
            }

            var y = (int)Math.Ceiling(Origin.Y);

            if (overrideHeight > -1)
            {
                y -= overrideHeight / Options.Instance.Sprites.Directions;
            }
            else if (Texture != null)
            {
                y -= Texture.Height / Options.Instance.Sprites.Directions;
            }

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
            // Are we supposed to hide this Label?
            if (!ShouldDrawName || string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            var map = MapInstance;
            if (map == null)
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

            var textSize = Graphics.Renderer.MeasureText(label, Graphics.EntityNameFont, 1);

            var x = (int)Math.Ceiling(Origin.X);
            var y = position == 0 ? GetLabelLocation(LabelType.Header) : GetLabelLocation(LabelType.Footer);

            if (backgroundColor != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y), backgroundColor
                );
            }

            Graphics.Renderer.DrawString(
                label, Graphics.EntityNameFont, (int)(x - (int)Math.Ceiling(textSize.X / 2f)), (int)y, 1,
                Color.FromArgb(textColor.ToArgb()), true, null, Color.FromArgb(borderColor.ToArgb())
            );
        }

        public virtual void DrawName(Color textColor, Color borderColor = null, Color backgroundColor = null)
        {
            // Are we supposed to hide this name?
            if (!ShouldDrawName || string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            //Check for npc colors
            if (textColor == null)
            {
                LabelColor? color;
                switch (Aggression)
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
                    backgroundColor = backgroundColor ?? color?.Background;
                    borderColor = borderColor ?? color?.Outline;
                }
            }

            if (borderColor == null)
            {
                borderColor = Color.Transparent;
            }

            if (backgroundColor == null)
            {
                backgroundColor = Color.Transparent;
            }

            var map = MapInstance;
            if (map == null)
            {
                return;
            }

            var name = Name;
            if ((this is Player && Options.Player.ShowLevelByName) || (Type == EntityTypes.GlobalEntity && Options.Npc.ShowLevelByName))
            {
                name = Strings.GameWindow.EntityNameAndLevel.ToString(Name, Level);
            }

            var textSize = Graphics.Renderer.MeasureText(name, Graphics.EntityNameFont, 1);

            var x = (int)Math.Ceiling(Origin.X);
            var y = GetLabelLocation(LabelType.Name);

            if (backgroundColor != Color.Transparent)
            {
                Graphics.DrawGameTexture(
                    Graphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                    new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y), backgroundColor
                );
            }

            Graphics.Renderer.DrawString(
                name, Graphics.EntityNameFont, (int)(x - (int)Math.Ceiling(textSize.X / 2f)), (int)y, 1,
                Color.FromArgb(textColor.ToArgb()), true, null, Color.FromArgb(borderColor.ToArgb())
            );
        }

        public float GetLabelLocation(LabelType type)
        {
            var y = GetTop() - 8;

            //Need room for HP bar if not an event.
            if (!(this is Event) && ShouldDrawHpBar)
            {
                y -= GetBoundingHpBarTexture().Height + 2;
            }

            switch (type)
            {
                case LabelType.Header:
                    y = GetLabelLocation(LabelType.Name);

                    if (string.IsNullOrWhiteSpace(HeaderLabel.Text))
                    {
                        break;
                    }

                    var headerSize = Graphics.Renderer.MeasureText(HeaderLabel.Text, Graphics.EntityNameFont, 1);
                    y -= headerSize.Y + 2;
                    break;

                case LabelType.Footer:
                    if (string.IsNullOrWhiteSpace(FooterLabel.Text))
                    {
                        break;
                    }

                    var footerSize = Graphics.Renderer.MeasureText(FooterLabel.Text, Graphics.EntityNameFont, 1);
                    y -= footerSize.Y - 6;
                    break;

                case LabelType.Name:
                    y = GetLabelLocation(LabelType.Footer);
                    var nameSize = Graphics.Renderer.MeasureText(Name, Graphics.EntityNameFont, 1);
                    y -= nameSize.Y + (string.IsNullOrEmpty(FooterLabel.Text) ? -6 : 2);
                    break;

                case LabelType.ChatBubble:
                    y = GetLabelLocation(LabelType.Guild) - 2;
                    break;

                case LabelType.Guild:
                    if (this is Player player)
                    {
                        // Do we have a header? If so, slightly change the position!
                        if (string.IsNullOrWhiteSpace(HeaderLabel.Text))
                        {
                            y = GetLabelLocation(LabelType.Name);
                        }
                        else
                        {
                            y = GetLabelLocation(LabelType.Header);
                        }

                        if (string.IsNullOrWhiteSpace(player.Guild))
                        {
                            break;
                        }

                        var guildSize = Graphics.Renderer.MeasureText(player.Guild, Graphics.EntityNameFont, 1);
                        y -= 2 + guildSize.Y;
                    }
                    break;
            }

            return y;
        }

        public int GetShieldSize()
        {
            var shieldSize = 0;
            foreach (var status in Status)
            {
                if (status.Type == StatusTypes.Shield)
                {
                    shieldSize += status.Shield[(int)Vitals.Health];
                }
            }
            return shieldSize;
        }

        protected virtual bool ShouldDrawHpBar
        {
            get
            {
                if (!ShouldNotDrawHpBar || IsHovered)
                {
                    return true;
                }

                return GetType() == typeof(Entity) && Globals.Database.NpcOverheadHpBar;
            }
        }

        protected bool ShouldNotDrawHpBar
        {
            get
            {
                if (LatestMap == default || !ShouldDraw)
                {
                    return true;
                }

                int health = Vital[(int)Vitals.Health],
                    maxHealth = MaxVital[(int)Vitals.Health],
                    shieldSize = GetShieldSize();

                return health < 1 || health == maxHealth || shieldSize > 0;
            }
        }

        public GameTexture GetBoundingHpBarTexture()
        {
            return GameTexture.GetBoundingTexture(
                BoundsComparison.Height,
                Globals.ContentManager.GetTexture(TextureType.Misc, "hpbackground.png"),
                Globals.ContentManager.GetTexture(TextureType.Misc, "hpbar.png"),
                Globals.ContentManager.GetTexture(TextureType.Misc, "shieldbar.png")
            );
        }

        public void DrawHpBar()
        {
            // Are we supposed to hide this HP bar?
            if (!ShouldDrawHpBar)
            {
                return;
            }

            var hpBackground = Globals.ContentManager.GetTexture(TextureType.Misc, "hpbackground.png");
            var hpForeground = Globals.ContentManager.GetTexture(TextureType.Misc, "hpbar.png");
            var shieldForeground = Globals.ContentManager.GetTexture(TextureType.Misc, "shieldbar.png");

            var boundingTeture = GameTexture.GetBoundingTexture(
                BoundsComparison.Height,
                hpBackground,
                hpForeground,
                shieldForeground
            );

            var foregroundBoundingTexture = hpForeground;

            // Check for shields
            var maxVital = MaxVital[(int)Vitals.Health];
            var shieldSize = GetShieldSize();

            if (shieldSize + Vital[(int)Vitals.Health] > maxVital)
            {
                maxVital = shieldSize + Vital[(int)Vitals.Health];
            }

            var hpfillRatio = (float)Vital[(int)Vitals.Health] / maxVital;
            hpfillRatio = Math.Min(1, Math.Max(0, hpfillRatio));

            var hpFillWidth = (int)Math.Round(hpfillRatio * foregroundBoundingTexture.Width);
            var shieldPixelFillRatio = (foregroundBoundingTexture.Width - hpFillWidth) / ((float)foregroundBoundingTexture.Width);
            var shieldFillWidth = (int)Math.Floor(foregroundBoundingTexture.Width * shieldPixelFillRatio);

            if (foregroundBoundingTexture.Width < hpFillWidth + shieldFillWidth)
            {
                hpFillWidth = Math.Max(1, foregroundBoundingTexture.Width - shieldFillWidth);
                shieldFillWidth = Math.Max(1, foregroundBoundingTexture.Width - hpFillWidth);
            }

            if (shieldSize > 0 && shieldFillWidth < 1)
            {
                hpFillWidth -= 1 - shieldFillWidth;
                shieldFillWidth = 1;
            }

            var x = (int)Math.Ceiling(Origin.X);
            var y = (int)Math.Ceiling(Origin.Y);

            var sprite = Globals.ContentManager.GetTexture(TextureType.Entity, Sprite);
            if (sprite != null)
            {
                y -= sprite.Height / Options.Instance.Sprites.Directions;
                y -= boundingTeture.Height + 2;
            }

            y += boundingTeture.Height / 2;

            if (hpBackground != null)
            {
                Graphics.DrawGameTexture(
                    hpBackground, new FloatRect(0, 0, hpBackground.Width, hpBackground.Height),
                    new FloatRect(x - hpBackground.Width / 2, y - hpBackground.Height / 2, hpBackground.Width, hpBackground.Height), Color.White
                );
            }

            if (hpForeground != null)
            {
                Graphics.DrawGameTexture(
                    hpForeground,
                    new FloatRect(0, 0, hpFillWidth, hpForeground.Height),
                    new FloatRect(x - foregroundBoundingTexture.Width / 2, y - hpForeground.Height / 2, hpFillWidth, hpForeground.Height), Color.White
                );
            }

            if (shieldSize > 0 && shieldForeground != null) //Check for a shield to render
            {
                var shieldFillRatio = shieldFillWidth / (float)foregroundBoundingTexture.Width;
                Graphics.DrawGameTexture(
                    shieldForeground,
                    new FloatRect(shieldForeground.Width * (1 - shieldFillRatio), 0, shieldForeground.Width * shieldFillRatio, shieldForeground.Height),
                    new FloatRect(x - foregroundBoundingTexture.Width / 2 + hpFillWidth, y - shieldForeground.Height / 2, shieldFillWidth, shieldForeground.Height), Color.White
                );
            }
        }

        public bool ShouldDrawCastingBar => ShouldDraw && IsCasting && LatestMap != default;

        public void DrawCastingBar()
        {
            // Are we supposed to hide this cast bar?
            if (!ShouldDrawCastingBar)
            {
                return;
            }

            var castingSpell = SpellBase.Get(SpellCast);
            if (castingSpell == null)
            {
                return;
            }

            var castBackground = Globals.ContentManager.GetTexture(TextureType.Misc, "castbackground.png");
            var castForeground = Globals.ContentManager.GetTexture(TextureType.Misc, "castbar.png");
            var boundingTexture = GameTexture.GetBoundingTexture(BoundsComparison.Height, castBackground, castForeground);

            float remainingTime = CastTime - Timing.Global.Milliseconds;
            var fillRatio = 1 - remainingTime / castingSpell.CastDuration;

            var x = (int)Math.Ceiling(Origin.X);
            var y = (int)Math.Ceiling(Origin.Y);

            y += 2 + boundingTexture.Height / 2;

            if (castBackground != null)
            {
                Graphics.DrawGameTexture(
                    castBackground, new FloatRect(0, 0, castBackground.Width, castBackground.Height),
                    new FloatRect(x - castBackground.Width / 2, y - castBackground.Height / 2, castBackground.Width, castBackground.Height), Color.White
                );
            }

            if (castForeground != null)
            {
                Graphics.DrawGameTexture(
                    castForeground,
                    new FloatRect(0, 0, castForeground.GetWidth() * fillRatio, castForeground.Height),
                    new FloatRect(x - castForeground.Width / 2, y - castForeground.Height / 2, castForeground.Width * fillRatio, castForeground.Height), Color.White
                );
            }
        }

        public bool ShouldDrawTarget => !(this is Projectile) && LatestMap != default;

        public void DrawTarget(int priority)
        {
            // Should we draw the target?
            if (!ShouldDrawTarget)
            {
                return;
            }

            var targetTexture = Globals.ContentManager.GetTexture(TextureType.Misc, "target.png");
            if (targetTexture == null)
            {
                return;
            }

            var srcRectangle = new FloatRect(
                priority * targetTexture.GetWidth() / 2f,
                0,
                targetTexture.GetWidth() / 2f,
                targetTexture.GetHeight()
            );

            var destRectangle = new FloatRect(
                (float)Math.Ceiling(WorldPos.X + (WorldPos.Width - targetTexture.GetWidth() / 2) / 2),
                (float)Math.Ceiling(WorldPos.Y + (WorldPos.Height - targetTexture.GetHeight()) / 2),
                srcRectangle.Width,
                srcRectangle.Height
            );

            Graphics.DrawGameTexture(targetTexture, srcRectangle, destRectangle, Color.White);
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
                if (status.SpellId == guid && status.IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        public IStatus GetStatus(Guid guid)
        {
            foreach (var status in Status)
            {
                if (status.SpellId == guid && status.IsActive)
                {
                    return status;
                }
            }

            return null;
        }

        public void SortStatuses()
        {
            //Sort Status effects by remaining time
            Status = Status.OrderByDescending(x => x.RemainingMs).ToList();
        }

        public void UpdateSpriteAnimation()
        {
            //Exit if textures haven't been loaded yet
            if (AnimatedTextures.Count == 0)
            {
                return;
            }

            SpriteAnimation = SpriteAnimations.Normal;
            if (AnimatedTextures.TryGetValue(SpriteAnimations.Idle, out _) && LastActionTime + Options.Instance.Sprites.TimeBeforeIdle < Timing.Global.Milliseconds)
            {
                SpriteAnimation = SpriteAnimations.Idle;
            }

            if (IsMoving)
            {
                SpriteAnimation = SpriteAnimations.Normal;
                LastActionTime = Timing.Global.Milliseconds;
            }
            else if (AttackTimer > Timing.Global.Ticks / TimeSpan.TicksPerMillisecond && !IsBlocking) //Attacking
            {
                var timeIn = CalculateAttackTime() - (AttackTimer - Timing.Global.Ticks / TimeSpan.TicksPerMillisecond);
                LastActionTime = Timing.Global.Milliseconds;

                if (AnimatedTextures.TryGetValue(SpriteAnimations.Attack, out _))
                {
                    SpriteAnimation = SpriteAnimations.Attack;
                }

                if (Options.WeaponIndex > -1 && Options.WeaponIndex < Equipment.Length)
                {
                    if (Equipment[Options.WeaponIndex] != Guid.Empty && this != Globals.Me ||
                        MyEquipment[Options.WeaponIndex] < Options.MaxInvItems)
                    {
                        var itemId = Guid.Empty;
                        if (this == Globals.Me)
                        {
                            var slot = MyEquipment[Options.WeaponIndex];
                            if (slot > -1)
                            {
                                itemId = Inventory[slot].ItemId;
                            }
                        }
                        else
                        {
                            itemId = Equipment[Options.WeaponIndex];
                        }

                        var item = ItemBase.Get(itemId);
                        if (item != null)
                        {
                            if (AnimatedTextures.TryGetValue(SpriteAnimations.Weapon, out _))
                            {
                                SpriteAnimation = SpriteAnimations.Weapon;
                            }

                            if (AnimatedTextures.TryGetValue(SpriteAnimations.Shoot, out _) &&
                                item.ProjectileId != Guid.Empty &&
                                item.WeaponSpriteOverride == null)
                            {
                                SpriteAnimation = SpriteAnimations.Shoot;
                            }
                        }
                    }
                }

                if (SpriteAnimation != SpriteAnimations.Normal && SpriteAnimation != SpriteAnimations.Idle)
                {
                    SpriteFrame = (int)Math.Floor((timeIn / (CalculateAttackTime() / (float)SpriteFrames)));
                }
            }
            else if (IsCasting)
            {
                var spell = SpellBase.Get(SpellCast);
                if (spell != null)
                {
                    var duration = spell.CastDuration;
                    var timeIn = duration - (CastTime - Timing.Global.Milliseconds);

                    if (AnimatedTextures.TryGetValue(SpriteAnimations.Cast, out _))
                    {
                        SpriteAnimation = SpriteAnimations.Cast;
                    }

                    if (spell.SpellType == SpellTypes.CombatSpell &&
                        spell.Combat.TargetType == SpellTargetTypes.Projectile &&
                        spell.CastSpriteOverride == null)
                    {
                        if (AnimatedTextures.TryGetValue(SpriteAnimations.Shoot, out _))
                        {
                            SpriteAnimation = SpriteAnimations.Shoot;
                        }
                    }

                    SpriteFrame = (int)Math.Floor((timeIn / (duration / (float)SpriteFrames)));
                }
                LastActionTime = Timing.Global.Milliseconds;
            }

            if (SpriteAnimation == SpriteAnimations.Normal)
            {
                ResetSpriteFrame();
            }
            else if (SpriteAnimation == SpriteAnimations.Idle)
            {
                if (SpriteFrameTimer + Options.Instance.Sprites.IdleFrameDuration < Timing.Global.Milliseconds)
                {
                    SpriteFrame++;
                    if (SpriteFrame >= SpriteFrames)
                    {
                        SpriteFrame = 0;
                    }
                    SpriteFrameTimer = Timing.Global.Milliseconds;
                }
            }
        }

        public void ResetSpriteFrame()
        {
            SpriteFrame = 0;
            SpriteFrameTimer = Timing.Global.Milliseconds;
        }

        public virtual void LoadTextures(string textureName)
        {
            AnimatedTextures.Clear();
            foreach (SpriteAnimations spriteAnimation in Enum.GetValues(typeof(SpriteAnimations)))
            {
                if (spriteAnimation == SpriteAnimations.Normal)
                {
                    Texture = Globals.ContentManager.GetTexture(TextureType.Entity, textureName);
                }
                else
                {
                    LoadAnimationTexture(textureName, spriteAnimation);
                }
            }
        }

        protected virtual void LoadAnimationTexture(string textureName, SpriteAnimations spriteAnimation)
        {
            SpriteAnimations spriteAnimationOveride = spriteAnimation;
            string textureOverride = default;

            switch (spriteAnimation)
            {
                // No override for this
                case SpriteAnimations.Normal: break;

                case SpriteAnimations.Idle: break;
                case SpriteAnimations.Attack:
                    if (this is Player player && ClassBase.TryGet(player.Class, out var classDescriptor))
                    {
                        textureOverride = classDescriptor.AttackSpriteOverride;
                    }
                    break;

                case SpriteAnimations.Shoot:
                    {
                        if (Equipment.Length <= Options.WeaponIndex)
                        {
                            break;
                        }

                        var weaponId = Equipment[Options.WeaponIndex];
                        if (ItemBase.TryGet(weaponId, out var itemDescriptor))
                        {
                            textureOverride = itemDescriptor.WeaponSpriteOverride;
                        }

                        if (!string.IsNullOrWhiteSpace(textureOverride))
                        {
                            spriteAnimationOveride = SpriteAnimations.Weapon;
                        }
                    }
                    break;

                case SpriteAnimations.Cast:
                    if (SpellBase.TryGet(SpellCast, out var spellDescriptor))
                    {
                        textureOverride = spellDescriptor.CastSpriteOverride;
                    }
                    break;

                case SpriteAnimations.Weapon:
                    {
                        if (Equipment.Length <= Options.WeaponIndex)
                        {
                            break;
                        }

                        var weaponId = Equipment[Options.WeaponIndex];
                        if (ItemBase.TryGet(weaponId, out var itemDescriptor))
                        {
                            textureOverride = itemDescriptor.WeaponSpriteOverride;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(spriteAnimation));
            }

            if (TryGetAnimationTexture(textureName, spriteAnimationOveride, textureOverride, out var texture))
            {
                AnimatedTextures[spriteAnimation] = texture;
            }
        }

        protected virtual bool TryGetAnimationTexture(string textureName, SpriteAnimations spriteAnimation, string textureOverride, out GameTexture texture)
        {
            var baseFilename = Path.GetFileNameWithoutExtension(textureName);
            var extension = Path.GetExtension(textureName);
            var animationTextureName = $"{baseFilename}_{spriteAnimation.ToString()?.ToLowerInvariant() ?? string.Empty}";

            if (!string.IsNullOrWhiteSpace(textureOverride))
            {
                animationTextureName = $"{animationTextureName}_{textureOverride}";
            }

            texture = Globals.ContentManager.GetTexture(TextureType.Entity, $"{animationTextureName}{extension}");
            return texture != default;
        }

        /// <summary>
        /// <para>Returns the direction to a player's selected target.
        /// Thanks to Daywalkr (Middle Ages: Online) for this !</para>
        /// </summary>
        /// <param name="en">entity's target</param>
        /// <returns>direction to player's selected target</returns>
        protected byte DirectionToTarget(Entity en)
        {
            byte newDir = Dir;

            if (en == null)
            {
                return newDir;
            }

            int originY = Y;
            int originX = X;
            int targetY = en.Y;
            int targetX = en.X;

            if (en.MapInstance.Id != MapInstance.Id)
            {
                if (en.MapInstance.GridY < MapInstance.GridY)
                {
                    originY += Options.MapHeight - 1;
                }
                else if (en.MapInstance.GridY > MapInstance.GridY)
                {
                    targetY += Options.MapHeight - 1;
                }

                if (en.MapInstance.GridX < MapInstance.GridX)
                {
                    originX += Options.MapWidth - 1;
                }
                else if (en.MapInstance.GridX > MapInstance.GridX)
                {
                    targetX += (Options.MapWidth - 1);
                }
            }

            var yOffset = originY - targetY;
            var xOffset = originX - targetX;
            var xDir = xOffset == 0 ? newDir : (xOffset < 0 ? (byte)Directions.Right : (byte)Directions.Left);
            var yDir = yOffset == 0 ? newDir : (yOffset < 0 ? (byte)Directions.Down : (byte)Directions.Up);

            newDir = Math.Abs(yOffset) > Math.Abs(xOffset) ? yDir : xDir;

            return newDir;
        }

        //Movement
        /// <summary>
        ///     Returns -6 if the tile is blocked by a global (non-event) entity
        ///     Returns -5 if the tile is completely out of bounds.
        ///     Returns -4 if a tile is blocked because of a local event.
        ///     Returns -3 if a tile is blocked because of a Z dimension tile
        ///     Returns -2 if a tile does not exist or is blocked by a map attribute.
        ///     Returns -1 is a tile is passable.
        ///     Returns any value zero or greater matching the entity index that is in the way.
        /// </summary>
        /// <returns></returns>
        public int IsTileBlocked(
            int x,
            int y,
            int z,
            Guid mapId,
            ref IEntity blockedBy,
            bool ignoreAliveResources = true,
            bool ignoreDeadResources = true,
            bool ignoreNpcAvoids = true,
            bool projectileTrigger = false
        )
        {
            var mapInstance = Maps.MapInstance.Get(mapId);
            if (mapInstance == null)
            {
                return -2;
            }

            var gridX = mapInstance.GridX;
            var gridY = mapInstance.GridY;
            try
            {
                var tmpX = x;
                var tmpY = y;
                var tmpMapId = Guid.Empty;
                if (x < 0)
                {
                    gridX--;
                    tmpX = Options.MapWidth - x * -1;
                }

                if (y < 0)
                {
                    gridY--;
                    tmpY = Options.MapHeight - y * -1;
                }

                if (x > Options.MapWidth - 1)
                {
                    gridX++;
                    tmpX = x - Options.MapWidth;
                }

                if (y > Options.MapHeight - 1)
                {
                    gridY++;
                    tmpY = y - Options.MapHeight;
                }

                if (gridX < 0 || gridY < 0 || gridX >= Globals.MapGridWidth || gridY >= Globals.MapGridHeight)
                {
                    return -2;
                }

                tmpMapId = Globals.MapGrid[gridX, gridY];

                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    if (en.Value == Globals.Me)
                    {
                        continue;
                    }
                    else
                    {
                        if (en.Value.MapId == tmpMapId &&
                            en.Value.X == tmpX &&
                            en.Value.Y == tmpY &&
                            en.Value.Z == Z)
                        {
                            if (!(en.Value is Projectile))
                            {
                                switch (en.Value)
                                {
                                    case Resource resource:
                                        var resourceBase = resource.BaseResource;
                                        if (resourceBase != null)
                                        {
                                            if (projectileTrigger)
                                            {
                                                bool isDead = resource.IsDead;
                                                if (!ignoreAliveResources && !isDead || !ignoreDeadResources && isDead)
                                                {
                                                    blockedBy = en.Value;

                                                    return -6;
                                                }

                                                return -1;
                                            }

                                            if (resourceBase.WalkableAfter && resource.IsDead ||
                                                resourceBase.WalkableBefore && !resource.IsDead)
                                            {
                                                continue;
                                            }
                                        }
                                        break;

                                    case Player player:
                                        //Return the entity key as this should block the player.  Only exception is if the MapZone this entity is on is passable.
                                        var entityMap = Maps.MapInstance.Get(player.MapId);
                                        if (Options.Instance.Passability.Passable[(int)entityMap.ZoneType])
                                        {
                                            continue;
                                        }
                                        break;
                                }

                                blockedBy = en.Value;

                                return -6;
                            }
                        }
                    }
                }

                if (Maps.MapInstance.Get(tmpMapId) != null)
                {
                    foreach (var en in Maps.MapInstance.Get(tmpMapId).LocalEntities)
                    {
                        if (en.Value == null)
                        {
                            continue;
                        }

                        if (en.Value.MapId == tmpMapId &&
                            en.Value.X == tmpX &&
                            en.Value.Y == tmpY &&
                            en.Value.Z == Z &&
                            !en.Value.Passable)
                        {
                            blockedBy = en.Value;

                            return -4;
                        }
                    }

                    foreach (var en in Maps.MapInstance.Get(tmpMapId).LocalCritters)
                    {
                        if (en.Value == null)
                        {
                            continue;
                        }

                        if (en.Value.MapId == tmpMapId &&
                            en.Value.X == tmpX &&
                            en.Value.Y == tmpY &&
                            en.Value.Z == Z &&
                            !en.Value.Passable)
                        {
                            blockedBy = en.Value as Critter;

                            return -4;
                        }
                    }
                }

                var gameMap = Maps.MapInstance.Get(Globals.MapGrid[gridX, gridY]);
                if (gameMap != null)
                {
                    if (gameMap.Attributes[tmpX, tmpY] != null)
                    {
                        if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.Blocked || (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.Animation && ((MapAnimationAttribute)gameMap.Attributes[tmpX, tmpY]).IsBlock))
                        {
                            return -2;
                        }
                        else if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.ZDimension)
                        {
                            if (((MapZDimensionAttribute)gameMap.Attributes[tmpX, tmpY]).BlockedLevel - 1 == z)
                            {
                                return -3;
                            }
                        }
                        else if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributes.NpcAvoid)
                        {
                            if (!ignoreNpcAvoids)
                            {
                                return -2;
                            }
                        }
                    }
                }
                else
                {
                    return -5;
                }

                return -1;
            }
            catch
            {
                return -2;
            }
        }

        ~Entity()
        {
            Dispose();
        }




    }

}

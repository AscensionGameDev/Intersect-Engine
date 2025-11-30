using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.Framework.Core.GameObjects.Maps.Attributes;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Entities;

public partial class Entity : IEntity
{
    public int AnimationFrame { get; set; }

    private readonly List<Animation> _animations = [];
    private readonly Dictionary<AnimationSource, Animation> _animationsBySource = [];

    //Animation Timer (for animated sprites)
    public long AnimationTimer { get; set; }

    //Combat
    public long AttackTimer { get; set; } = 0;

    public int AttackTime { get; set; } = -1;

    public long CastTime { get; set; } = 0;

    //Combat Status
    public bool IsAttacking => AttackTimer > Timing.Global.Milliseconds;

    public bool IsBlocking { get; set; } = false;

    public bool IsCasting => CastTime > Timing.Global.Milliseconds;

    protected readonly bool EnableTurnAroundWhileCasting = Options.Instance.Combat.EnableTurnAroundWhileCasting;

    public bool CanTurnAround => !IsMoving && (!IsCasting || EnableTurnAroundWhileCasting);

    public bool IsDashing => Dashing != null;

    //Dashing instance
    public Dash? Dashing { get; set; }

    public IDash? CurrentDash => Dashing;

    public Queue<Dash> DashQueue { get; set; } = new Queue<Dash>();

    public long DashTimer { get; set; }

    public float elapsedtime { get; set; } //to be removed

    private Guid[] _equipment = new Guid[Options.Instance.Equipment.Slots.Count];

    public bool HasAnimations => _animations.Count > 0;

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
            LoadAnimationTexture(
                string.IsNullOrWhiteSpace(TransformedSprite) ? Sprite : TransformedSprite,
                SpriteAnimations.Weapon
            );
        }
    }

    IReadOnlyList<int> IEntity.EquipmentSlots => [..MyEquipment];

    public Animation?[] EquipmentAnimations { get; set; } = new Animation[Options.Instance.Equipment.Slots.Count];

    //Extras
    public string Face { get; set; } = string.Empty;

    public Label FooterLabel { get; set; } = new(string.Empty, Color.White);

    public Gender Gender { get; set; } = Gender.Male;

    public Label HeaderLabel { get; set; } = new(string.Empty, Color.White);

    public bool IsHidden { get; set; } = false;

    public bool HideName { get; set; }

    //Core Values
    public Guid Id { get; set; }

    //Inventory/Spells/Equipment
    public IItem[] Inventory { get; } = new IItem[Options.Instance.Player.MaxInventory];

    IReadOnlyList<IItem> IEntity.Items => [.. Inventory];

    public bool InView { get; set; } = true;

    public bool IsMoving { get; set; }

    //Caching
    public IMapInstance? LatestMap { get; set; }

    public int Level { get; set; } = 1;

    //Vitals & Stats
    public long[] MaxVital { get; set; } = new long[Enum.GetValues<Vital>().Length];

    IReadOnlyDictionary<Vital, long> IEntity.MaxVitals =>
        Enum.GetValues<Vital>().ToDictionary(vital => vital, vital => MaxVital[(int)vital]);

    protected Vector2 mOrigin = Vector2.Zero;

    //Chat
    private readonly List<ChatBubble> mChatBubbles = [];

    private Direction _directionFacing;

    protected bool mDisposed;

    private long mLastUpdate;

    protected string _sprite = string.Empty;

    public Color Color { get; set; } = new Color(255, 255, 255, 255);

    public virtual Direction DirectionMoving { get; set; } = Direction.None;

    public long MoveTimer { get; set; }

    protected byte mRenderPriority = 1;

    protected string mTransformedSprite = string.Empty;

    private long mWalkTimer;

    public int[] MyEquipment { get; set; } = new int[Options.Instance.Equipment.Slots.Count];

    public string Name { get; set; } = string.Empty;

    public Color? NameColor { get; set; } = null;

    public bool Passable { get; set; }

    //Rendering Variables
    public HashSet<Entity>? RenderList { get; set; }

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
            LoadAnimationTexture(string.IsNullOrWhiteSpace(TransformedSprite) ? Sprite : TransformedSprite, SpriteAnimations.Cast);
        }
    }

    public Spell[] Spells { get; set; } = new Spell[Options.Instance.Player.MaxSpells];

    IReadOnlyList<Guid> IEntity.Spells => Spells.Select(x => x.Id).ToList();

    public int[] Stat { get; set; } = new int[Enum.GetValues<Stat>().Length];

    IReadOnlyDictionary<Stat, int> IEntity.Stats =>
        Enum.GetValues<Stat>().ToDictionary(stat => stat, stat => Stat[(int)stat]);

    public IGameTexture? Texture { get; set; }

    #region "Animation Textures and Timing"

    public SpriteAnimations SpriteAnimation { get; set; } = SpriteAnimations.Normal;

    public Dictionary<SpriteAnimations, IGameTexture> AnimatedTextures { get; set; } = [];

    public int SpriteFrame { get; set; } = 0;

    public long SpriteFrameTimer { get; set; } = -1;

    public long LastActionTime { get; set; } = -1;

    public long AutoTurnToTargetTimer { get; set; } = -1;

    #endregion

    public EntityType Type { get; }

    public NpcAggression Aggression { get; set; }

    public long[] Vital { get; set; } = new long[Enum.GetValues<Vital>().Length];

    IReadOnlyDictionary<Vital, long> IEntity.Vitals =>
        Enum.GetValues<Vital>().ToDictionary(vital => vital, vital => Vital[(int)vital]);

    public int WalkFrame { get; set; }

    public FloatRect WorldPos { get; set; } = new FloatRect();

    public bool IsHovered { get; set; }

    private Vector3 _position;

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position == value)
            {
                return;
            }

            var oldValue = _position;
            var delta = value - oldValue;
            _position = value;
            OnPositionChanged(value, oldValue);
        }
    }

    protected virtual void OnPositionChanged(Vector3 newPosition, Vector3 oldPosition) {}

    public int TileX => (int)float.Floor(Position.X);
    public int TileY => (int)float.Floor(Position.Y);
    public int TileZ => (int)float.Floor(Position.Z);

    //Location Info
    public byte X
    {
        get => (byte)float.Floor(Position.X);
        set => Position = Position with { X = value };
    }

    public byte Y
    {
        get => (byte)float.Floor(Position.Y);
        set => Position = Position with { Y = value };
    }

    public byte Z
    {
        get => (byte)float.Floor(Position.Z);
        set => Position = Position with { Z = value };
    }

    public float OffsetX { get; set; }
    // {
    //     get => (Position.X % 1) * TileWidth;
    //     set => Position = Position with { X = X + (value / TileWidth) };
    // }

    public float OffsetY { get; set; }
    // {
    //     get => (Position.Y % 1) * TileHeight;
    //     set => Position = Position with { Y = Y + (value / TileHeight) };
    // }

    public Entity(Guid id, EntityPacket? packet, EntityType entityType)
    {
        Id = id;
        Type = entityType;
        MapId = Guid.Empty;

        if (Id != Guid.Empty && Type != EntityType.Event)
        {
            for (var i = 0; i < Options.Instance.Player.MaxInventory; i++)
            {
                Inventory[i] = new Item();
            }

            for (var i = 0; i < Options.Instance.Player.MaxSpells; i++)
            {
                Spells[i] = new Spell();
            }

            for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
            {
                Equipment[i] = Guid.Empty;
                MyEquipment[i] = -1;
            }
        }

        AnimationTimer = Timing.Global.MillisecondsUtc + Globals.Random.Next(0, 500);

        //TODO Remove because fixed orrrrr change the exception text
        if (Options.Instance.Equipment.Slots.Count == 0)
        {
            throw new Exception("What the fuck is going on!?!?!?!?!?!");
        }

        Load(packet);
    }

    //Status effects
    public List<IStatus> Status { get; private set; } = [];

    IReadOnlyList<IStatus> IEntity.Status => Status;

    public Vector2 Origin => LatestMap == default ? Vector2.Zero : mOrigin;

    protected virtual Vector2 CenterOffset => (Texture == default) ? Vector2.Zero : (Vector2.UnitY * Texture.Center.Y / Options.Instance.Sprites.Directions);

    public Vector2 Center => Origin - CenterOffset;

    public Direction DirectionFacing
    {
        get => _directionFacing;
        set => _directionFacing = (Direction)((int)(value + Options.Instance.Map.MovementDirections) % Options.Instance.Map.MovementDirections);
    }

    private Direction _lastDirection = Direction.Down;

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

            var textureName = string.IsNullOrEmpty(mTransformedSprite) ? _sprite : mTransformedSprite;
            LoadTextures(textureName);
        }
    }

    public virtual string Sprite
    {
        get => _sprite;
        set
        {
            if (_sprite == value)
            {
                return;
            }

            _sprite = value;
            LoadTextures(_sprite);
        }
    }

    public virtual int SpriteFrames
    {
        get
        {
            return SpriteAnimation switch
            {
                SpriteAnimations.Normal => Options.Instance.Sprites.NormalFrames,
                SpriteAnimations.Idle => Options.Instance.Sprites.IdleFrames,
                SpriteAnimations.Attack => Options.Instance.Sprites.AttackFrames,
                SpriteAnimations.Shoot => Options.Instance.Sprites.ShootFrames,
                SpriteAnimations.Cast => Options.Instance.Sprites.CastFrames,
                SpriteAnimations.Weapon => Options.Instance.Sprites.WeaponFrames,
                _ => Options.Instance.Sprites.NormalFrames,
            };
        }
    }

    public IMapInstance? MapInstance => Maps.MapInstance.Get(MapId);

    public virtual Guid MapId { get; set; }

    //Deserializing
    public virtual void Load(EntityPacket? packet)
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
        Position = packet.Position;
        DirectionFacing = (Direction)packet.Dir;
        Passable = packet.Passable;
        HideName = packet.HideName;
        IsHidden = packet.HideEntity;
        NameColor = packet.NameColor;
        HeaderLabel = new Label(packet.HeaderLabel.Label, packet.HeaderLabel.Color);
        FooterLabel = new Label(packet.FooterLabel.Label, packet.FooterLabel.Color);

        var animsToClear = new List<Animation>();
        var animsToAdd = new List<AnimationDescriptor>();
        for (var i = 0; i < packet.Animations.Length; i++)
        {
            var anim = AnimationDescriptor.Get(packet.Animations[i]);
            if (anim != null)
            {
                animsToAdd.Add(anim);
            }
        }

        foreach (var anim in _animations)
        {
            animsToClear.Add(anim);
            if (!anim.InfiniteLoop)
            {
                _ = animsToClear.Remove(anim);
            }
            else
            {
                foreach (var addedAnim in animsToAdd)
                {
                    if (addedAnim.Id == anim?.Descriptor?.Id)
                    {
                        _ = animsToClear.Remove(anim);
                        _ = animsToAdd.Remove(addedAnim);

                        break;
                    }
                }

                foreach (var equipAnim in EquipmentAnimations)
                {
                    if (equipAnim == anim && anim != null)
                    {
                        _ = animsToClear.Remove(anim);
                    }
                }
            }
        }

        RemoveAnimations(animsToClear);
        AddAnimations(animsToAdd);

        Vital = packet.Vital;
        MaxVital = packet.MaxVital;

        //Update status effects
        Status.Clear();

        if (packet.StatusEffects == null)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"'{nameof(packet)}.{nameof(packet.StatusEffects)}' is null.");
        }
        else
        {
            foreach (var status in packet.StatusEffects)
            {
                var instance = new Status(
                    status.SpellId, status.Type, status.TransformSprite, status.TimeRemaining, status.TotalDuration
                );

                Status?.Add(instance);

                if (instance.Type == SpellEffect.Shield)
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
            ApplicationContext.Context.Value?.Logger.LogWarning($"'{nameof(Globals.Me)}' is null.");
        }
        else
        {
            if (Id == Globals.Me.Id)
            {
                Interface.Interface.EnqueueInGame(
                    gameInterface =>
                    {
                        if (gameInterface.PlayerStatusWindow == null)
                        {
                            ApplicationContext.Context.Value?.Logger.LogWarning(
                                $"'{nameof(gameInterface.PlayerStatusWindow)}' is null."
                            );
                        }
                        else
                        {
                            gameInterface.PlayerStatusWindow.ShouldUpdateStatuses = true;
                        }
                    },
                    (entityId, entityName) => ApplicationContext.CurrentContext.Logger.LogWarning(
                        "Tried to load entity {EntityId} ({EntityName}) from packet before in-game UI was ready",
                        entityId,
                        entityName
                    ),
                    packet.EntityId,
                    packet.Name
                );
            }
            else if (Id != Guid.Empty && Id == Globals.Me.TargetId)
            {
                if (Globals.Me.TargetBox == null)
                {
                    ApplicationContext.Context.Value?.Logger.LogWarning($"'{nameof(Globals.Me.TargetBox)}' is null.");
                }
                else
                {
                    Globals.Me.TargetBox.ShouldUpdateStatuses = true;
                }
            }
        }
    }

    public void AddAnimations(List<AnimationDescriptor> animationDescriptors)
    {
        foreach (var animationDescriptor in animationDescriptors)
        {
            TryAddAnimation(new Animation(animationDescriptor, true, false, -1, this));
        }
    }

    public virtual bool IsAllyOf(Player en)
    {
        if (en == null || MapInstance == default || en.MapInstance == default)
        {
            return false;
        }

        // Resources have no allies
        if (Type == EntityType.Resource)
        {
            return false;
        }

        // Events ONLY have allies
        if (Type == EntityType.Event)
        {
            return true;
        }

        // Yourself is always an ally
        if (en.Id == Id)
        {
            return true;
        }

        return false;
    }

    public void ClearAnimations() => RemoveAnimations(_animations);

    public void RemoveAnimations(IEnumerable<Animation> animations, bool dispose = true)
    {
        var animationsToRemove = animations.ToArray();
        foreach (var animation in animationsToRemove)
        {
            if (dispose)
            {
                if (!animation.IsDisposed)
                {
                    animation.Dispose();
                }
            }

            _ = TryRemoveAnimation(animation);
        }
    }

    public bool IsDisposed => mDisposed;

    public virtual void Dispose()
    {
        if (RenderList != null)
        {
            _ = RenderList.Remove(this);
        }

        ClearAnimations();
        GC.SuppressFinalize(this);
        mDisposed = true;
    }

    //Returns the amount of time required to traverse 1 tile
    public virtual float GetMovementTime()
    {
        var time = 1000f / (float)(1 + Math.Log(Stat[(int)Enums.Stat.Speed]));
        if (DirectionFacing > Direction.Right)
        {
            time *= MathHelper.UnitDiagonalLength;
        }

        if (IsBlocking)
        {
            time += time * Options.Instance.Combat.BlockingSlow;
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

        if (LatestMap?.Id != MapId)
        {
            LatestMap = Maps.MapInstance.Get(MapId);
        }

        if (LatestMap == null || !LatestMap.InView())
        {
            Globals.EntitiesToDispose.Add(Id);
            return false;
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
            WalkFrame = Options.Instance.Sprites.NormalDashFrame; //Fix the frame whilst dashing
        }
        else if (mWalkTimer < Timing.Global.Milliseconds)
        {
            if (!IsMoving && DashQueue.Count > 0)
            {
                Dashing = DashQueue.Dequeue();
                Dashing.Start(this);
                OffsetX = 0;
                OffsetY = 0;
                DashTimer = Timing.Global.Milliseconds + Options.Instance.Combat.MaxDashSpeed;
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
                        WalkFrame = SpriteFrames / 2;
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
            var displacementTime = ecTime * TileHeight / GetMovementTime();

            PickLastDirection(DirectionFacing);

            switch (DirectionFacing)
            {
                case Direction.Up:
                    OffsetY -= displacementTime;
                    OffsetX = 0;
                    if (OffsetY < 0)
                    {
                        OffsetY = 0;
                    }

                    break;

                case Direction.Down:
                    OffsetY += displacementTime;
                    OffsetX = 0;
                    if (OffsetY > 0)
                    {
                        OffsetY = 0;
                    }

                    break;

                case Direction.Left:
                    OffsetX -= displacementTime;
                    OffsetY = 0;
                    if (OffsetX < 0)
                    {
                        OffsetX = 0;
                    }

                    break;

                case Direction.Right:
                    OffsetX += displacementTime;
                    OffsetY = 0;
                    if (OffsetX > 0)
                    {
                        OffsetX = 0;
                    }

                    break;
                case Direction.UpLeft:
                    OffsetY -= displacementTime;
                    OffsetX -= displacementTime;
                    if (OffsetY < 0)
                    {
                        OffsetY = 0;
                    }

                    if (OffsetX < 0)
                    {
                        OffsetX = 0;
                    }

                    break;
                case Direction.UpRight:
                    OffsetY -= displacementTime;
                    OffsetX += displacementTime;
                    if (OffsetY < 0)
                    {
                        OffsetY = 0;
                    }

                    if (OffsetX > 0)
                    {
                        OffsetX = 0;
                    }

                    break;
                case Direction.DownLeft:
                    OffsetY += displacementTime;
                    OffsetX -= displacementTime;
                    if (OffsetY > 0)
                    {
                        OffsetY = 0;
                    }

                    if (OffsetX < 0)
                    {
                        OffsetX = 0;
                    }

                    break;
                case Direction.DownRight:
                    OffsetY += displacementTime;
                    OffsetX += displacementTime;
                    if (OffsetY > 0)
                    {
                        OffsetY = 0;
                    }

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
        if (Equipment.Length == Options.Instance.Equipment.Slots.Count)
        {
            for (var z = 0; z < Options.Instance.Equipment.Slots.Count; z++)
            {
                var equipmentAnimation = EquipmentAnimations[z];
                if (Equipment[z] != Guid.Empty && (this != Globals.Me || MyEquipment[z] < Options.Instance.Player.MaxInventory))
                {
                    var itemId = (this == Globals.Me && MyEquipment[z] > -1)
                        ? Inventory[MyEquipment[z]].ItemId
                        : Equipment[z];

                    if (ItemDescriptor.TryGet(itemId, out var itemDescriptor) && itemDescriptor.EquipmentAnimation is { } animationDescriptor)
                    {
                        if (equipmentAnimation == null || equipmentAnimation.Descriptor != animationDescriptor || equipmentAnimation.IsDisposed)
                        {
                            if (equipmentAnimation != null)
                            {
                                TryRemoveAnimation(equipmentAnimation, dispose: true);
                            }

                            var newAnimation = new Animation(animationDescriptor, true, true, -1, this);
                            EquipmentAnimations[z] = newAnimation;
                            _animations.Add(newAnimation);
                        }
                    }
                    else if (equipmentAnimation != null)
                    {
                        TryRemoveAnimation(equipmentAnimation, dispose: true);
                        EquipmentAnimations[z] = null;
                    }
                }
                else if (equipmentAnimation != null)
                {
                    TryRemoveAnimation(equipmentAnimation, dispose: true);
                    EquipmentAnimations[z] = null;
                }
            }
        }

        var chatbubbles = mChatBubbles.ToArray();
        foreach (var chatbubble in chatbubbles)
        {
            if (!chatbubble.Update())
            {
                _ = mChatBubbles.Remove(chatbubble);
            }
        }

        if (AnimationTimer < Timing.Global.MillisecondsUtc)
        {
            AnimationTimer = Timing.Global.MillisecondsUtc + 200;
            AnimationFrame++;
            if (AnimationFrame >= SpriteFrames)
            {
                AnimationFrame = 0;
            }
        }

        CalculateOrigin();

        List<Animation>? disposedAnimations = null;
        foreach (var animation in _animations)
        {
            animation.Update();

            //If disposed mark to be removed and continue onward
            if (animation.IsDisposed)
            {
                disposedAnimations ??= [];
                disposedAnimations.Add(animation);
                continue;
            }

            if (IsStealthed || IsHidden)
            {
                animation.Hide();
            }
            else
            {
                animation.Show();
            }

            var animationDirection = animation.AutoRotate ? DirectionFacing : default;
            animation.SetPosition(
                (int)Math.Ceiling(Center.X),
                (int)Math.Ceiling(Center.Y),
                X,
                Y,
                MapId,
                animationDirection, Z
            );
        }

        if (disposedAnimations != null)
        {
            RemoveAnimations(disposedAnimations);
        }

        mLastUpdate = Timing.Global.Milliseconds;

        UpdateSpriteAnimation();

        return true;
    }

    public bool TryAddAnimation(Animation animation, AnimationSource animationSource = default)
    {
        if (animationSource != default)
        {
            if (_animationsBySource.TryGetValue(animationSource, out var existingAnimation))
            {
                if (!TryRemoveAnimation(existingAnimation, animationSource))
                {
                    return false;
                }
            }
        }

        _animationsBySource[animationSource] = animation;
        _animations.Add(animation);
        return true;
    }

    public bool TryRemoveAnimation(AnimationSource animationSource, bool dispose = false) => TryRemoveAnimation(
        animationSource: animationSource,
        dispose: dispose,
        animation: out _
    );

    public bool TryRemoveAnimation(AnimationSource animationSource, [NotNullWhen(true)] out Animation? animation) =>
        TryRemoveAnimation(animationSource: animationSource, dispose: false, animation: out animation);

    public bool TryRemoveAnimation(
        AnimationSource animationSource,
        bool dispose,
        [NotNullWhen(true)] out Animation? animation
    )
    {
        if (!_animationsBySource.Remove(animationSource, out animation))
        {
            return false;
        }

        _animations.Remove(animation);

        if (dispose)
        {
            animation.Dispose();
        }

        return true;
    }

    public bool RemoveAnimationIfExists(AnimationSource animationSource, bool dispose = false)
    {
        return !_animationsBySource.ContainsKey(animationSource) ||
               TryRemoveAnimation(animationSource: animationSource, dispose: dispose);
    }

    public bool TryRemoveAnimation(Animation animation, bool dispose = false) => TryRemoveAnimation(
        animation: animation,
        animationSource: animation.Source,
        dispose: dispose
    );

    public bool TryRemoveAnimation(Animation animation, AnimationSource animationSource, bool dispose = false)
    {
        var removedFromSourceLookup = animationSource != default && _animationsBySource.Remove(animationSource);
        var removedFromList = _animations.Remove(animation);

        if (dispose)
        {
            animation.Dispose();
        }

        return removedFromList || removedFromSourceLookup;
    }

    public virtual int CalculateAttackTime()
    {
        //If this is an npc we don't know it's attack time. Luckily the server provided it!
        if (this != Globals.Me && AttackTime > -1)
        {
            return AttackTime;
        }

        //Otherwise return the legacy attack speed calculation
        return (int)(Options.Instance.Combat.MaxAttackRate +
                     (Options.Instance.Combat.MinAttackRate - Options.Instance.Combat.MaxAttackRate) *
                     (((float)Options.Instance.Player.MaxStat - Stat[(int)Enums.Stat.Speed]) /
                      Options.Instance.Player.MaxStat));
    }

    /// <summary>
    /// Returns whether this entity is Stealthed or not.
    /// </summary>
    public virtual bool IsStealthed => this != Globals.Me && Status.Any(t => t.Type == SpellEffect.Stealth);

    /// <summary>
    /// Returns whether this entity should be drawn.
    /// </summary>
    public virtual bool ShouldDraw => !IsHidden && (!IsStealthed || this == Globals.Me || Globals.Me?.IsInMyParty(Id) == true);

    /// <summary>
    /// Returns whether the name of this entity should be drawn.
    /// </summary>
    public virtual bool ShouldDrawName
    {
        get
        {
            // Return if the map instance is null or the name is empty or not set to be drawn.
            if (MapInstance == null || string.IsNullOrWhiteSpace(Name) || HideName || !ShouldDraw)
            {
                return false;
            }

            // Look up the mouse position and convert it to a world point.
            var mousePos = Graphics.ConvertToWorldPoint(Globals.InputManager.MousePosition);

            // Entity is considered hovered if the mouse is over its world position and not hovering over the GUI.
            IsHovered = WorldPos.Contains(mousePos.X, mousePos.Y) && !Interface.Interface.DoesMouseHitInterface();

            // Check the type of entity and return whether its name should be drawn based on settings and conditions.
            switch (this)
            {
                case Projectile _:
                case Resource _:
                    return false;

                case Event _:
                    return true;

                case Player player:
                    if (IsHovered)
                    {
                        return true;
                    }

                    var me = Globals.Me;

                    if (me == default)
                    {
                        return false;
                    }

                    if (Globals.Database.MyOverheadInfo && player.Id == me.Id)
                    {
                        return true;
                    }

                    if (Globals.Database.PlayerOverheadInfo && player.Id != me.Id)
                    {
                        return true;
                    }

                    if (Globals.Database.PartyMemberOverheadInfo && me.IsInMyParty(player))
                    {
                        return true;
                    }

                    if (Globals.Database.FriendOverheadInfo && me.IsFriend(player))
                    {
                        return true;
                    }

                    return Globals.Database.GuildMemberOverheadInfo && me.IsGuildMate(player);

                default:
                    return IsHovered || Globals.Database.NpcOverheadInfo;
            }
        }
    }

    public virtual HashSet<Entity>? DetermineRenderOrder(HashSet<Entity>? existingRenderSet, IMapInstance? map)
    {
        _ = existingRenderSet?.Remove(this);

        if (map == null)
        {
            return null;
        }

        var mapGrid = Globals.MapGrid;
        if (mapGrid == null)
        {
            return null;
        }

        var renderingEntities = Graphics.RenderingEntities;
        if (renderingEntities == null)
        {
            return null;
        }

        var playerMap = Globals.Me?.MapInstance;
        if (playerMap == default)
        {
            return default;
        }

        var gridX = playerMap.GridX;
        var gridY = playerMap.GridY;
        var startX = Math.Max(0, gridX - 1);
        var startY = Math.Max(0, gridY - 1);

        var mapGridWidth = Globals.MapGridWidth;
        var mapGridHeight = Globals.MapGridHeight;
        var mapGridLimitX = mapGrid.GetLength(0);
        var mapGridLimitY = mapGrid.GetLength(1);
        var endX = Math.Min(Math.Min(mapGridWidth, mapGridLimitX) - 1, gridX + 1);
        var endY = Math.Min(Math.Min(mapGridHeight, mapGridLimitY) - 1, gridY + 1);

        if (mapGridWidth != mapGridLimitX)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Possible map grid race condition detected, MapGrid's X dimension is {XLength} but MapGridWidth is {Width}",
                mapGridLimitX,
                mapGridWidth
            );
        }

        if (mapGridHeight != mapGridLimitY)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Possible map grid race condition detected, MapGrid's Y dimension is {YLength} but MapGridHeight is {Height}",
                mapGridLimitY,
                mapGridHeight
            );
        }

        for (var x = startX; x <= endX; ++x)
        {
            for (var y = startY; y <= endY; ++y)
            {
                var mapIdOnGrid = mapGrid[x, y];
                if (mapIdOnGrid != MapId)
                {
                    continue;
                }

                var priority = mRenderPriority;
                if (Z != 0)
                {
                    priority += 3;
                }

                HashSet<Entity>? renderSet = default;
                int entityY;

                if (y == gridY - 1)
                {
                    entityY = MapHeight + Y;
                }
                else if (y == gridY)
                {
                    entityY = MapHeight * 2 + Y;
                }
                else
                {
                    entityY = MapHeight * 3 + Y;
                }

                if (IsMoving && DirectionMoving == Direction.Up || DirectionMoving == Direction.UpLeft || DirectionMoving == Direction.UpRight)
                {
                    entityY = entityY - 1;
                }
                else if (IsMoving && DirectionMoving == Direction.Down || DirectionMoving == Direction.DownLeft || DirectionMoving == Direction.DownRight)
                {
                    entityY = entityY + 1;
                }

                if (priority >= renderingEntities.GetLength(0) || entityY >= renderingEntities.GetLength(1))
                {
                    return renderSet;
                }

                renderSet = renderingEntities[priority, entityY];
                _ = renderSet.Add(this);

                return renderSet;
            }
        }

        return existingRenderSet;
    }

    //Rendering Functions
    public virtual void Draw()
    {
        if (IsHidden)
        {
            return; //Don't draw if the entity is hidden
        }

        WorldPos.Reset();

        if (!Globals.GridMaps.ContainsKey(MapId) || !Maps.MapInstance.TryGet(MapId, out var map))
        {
            return;
        }

        var sprite = string.Empty;
        // Copy the actual render color, because we'll be editing it later and don't want to overwrite it.
        var renderColor = new Color(Color.A, Color.R, Color.G, Color.B);

        string transformedSprite = string.Empty;

        // Loop through the entity status list.
        for (var n = 0; n < Status.Count; n++)
        {
            var status = Status[n];

            switch (status.Type)
            {
                // If the entity is transformed: apply that sprite instead.
                case SpellEffect.Transform:
                    transformedSprite = sprite = status.Data;
                    break;

                // If entity is stealth, don't render unless the entity is the player or is within their party.
                case SpellEffect.Stealth:
                    if (this != Globals.Me && !(this is Player player && Globals.Me?.IsInMyParty(player) == true))
                    {
                        return;
                    }

                    renderColor.A /= 2;
                    break;
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
                map.X + X * TileWidth + OffsetX,
                map.Y + Y * TileHeight + OffsetY,
                TileWidth,
                TileHeight);
            return;
        }

        var spriteRow = PickSpriteRow(DirectionFacing);

        var frameWidth = texture.Width / SpriteFrames;
        var frameHeight = texture.Height / Options.Instance.Sprites.Directions;

        var frame = SpriteFrame;
        if (Options.Instance.AnimatedSprites.Contains(sprite.ToLower()))
        {
            frame = AnimationFrame;
        }
        else if (SpriteAnimation == SpriteAnimations.Normal)
        {
            frame = NormalSpriteAnimationFrame;
        }

        var srcRectangle = new FloatRect(frame * frameWidth, spriteRow * frameHeight, frameWidth, frameHeight);
        var destRectangle = new FloatRect(
            (int)Math.Ceiling(Origin.X - frameWidth / 2f),
            (int)Math.Ceiling(Origin.Y - frameHeight),
            srcRectangle.Width,
            srcRectangle.Height
        );

        WorldPos = destRectangle;

        //Order the layers of paperdolls and sprites
        for (var z = 0; z < Options.Instance.Equipment.Paperdoll.Directions[(int)_lastDirection].Count; z++)
        {
            var paperdoll = Options.Instance.Equipment.Paperdoll.Directions[(int)_lastDirection][z];
            var equipSlot = Options.Instance.Equipment.Slots.IndexOf(paperdoll);

            //Check for player
            if (string.Equals("Player", paperdoll, StringComparison.Ordinal))
            {
                Graphics.DrawGameTexture(texture, srcRectangle, destRectangle, renderColor);
            }
            else if (equipSlot > -1)
            {
                //Don't render the paperdolls if they have transformed.
                if (sprite == Sprite && Equipment.Length == Options.Instance.Equipment.Slots.Count)
                {
                    if (Equipment[equipSlot] != Guid.Empty && this != Globals.Me ||
                        MyEquipment[equipSlot] < Options.Instance.Player.MaxInventory)
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

                        var item = ItemDescriptor.Get(itemId);
                        if (ItemDescriptor.TryGet(itemId, out var itemDescriptor))
                        {
                            var itemPaperdoll = Gender == 0
                                ? itemDescriptor.MalePaperdoll
                                : itemDescriptor.FemalePaperdoll;
                            DrawEquipment(itemPaperdoll, item.Color * renderColor);
                        }
                    }
                }
            }
        }
    }

    private int NormalSpriteAnimationFrame
    {
        get
        {
            var frame = WalkFrame;

            if (IsBlocking)
            {
                frame = Options.Instance.Sprites.NormalBlockFrame;
            }
            else if (IsCasting)
            {
                frame = Options.Instance.Sprites.NormalCastFrame;
            }
            // Checks if the entity is attacking or not.
            // Note: the calculation differs with IsAttacking because
            // frames are intended to behave differently with normal sprite-sheets.
            else if (AttackTimer - (CalculateAttackTime() / 2) > Timing.Global.Milliseconds)
            {
                frame = Options.Instance.Sprites.NormalAttackFrame;
            }

            if (frame >= SpriteFrames)
            {
                frame = SpriteFrames / 2;
            }

            return frame;
        }
    }

    private int PickSpriteRow(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
            case Direction.DownLeft when _lastDirection == Direction.Left:
            case Direction.UpLeft when _lastDirection == Direction.Left:
                return 1;

            case Direction.Right:
            case Direction.DownRight when _lastDirection == Direction.Right:
            case Direction.UpRight when _lastDirection == Direction.Right:
                return 2;

            case Direction.Up:
            case Direction.UpLeft when _lastDirection != Direction.Left:
            case Direction.UpRight when _lastDirection != Direction.Right:
                return 3;

            case Direction.Down:
            case Direction.DownLeft when _lastDirection != Direction.Left:
            case Direction.DownRight when _lastDirection != Direction.Right:
            default:
                return 0;
        }
    }

    public void PickLastDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
            case Direction.DownLeft when _lastDirection == Direction.Left:
            case Direction.UpLeft when _lastDirection == Direction.Left:
                _lastDirection = Direction.Left;
                break;

            case Direction.Right:
            case Direction.DownRight when _lastDirection == Direction.Right:
            case Direction.UpRight when _lastDirection == Direction.Right:
                _lastDirection = Direction.Right;
                break;

            case Direction.Up:
            case Direction.UpLeft when _lastDirection != Direction.Left:
            case Direction.UpRight when _lastDirection != Direction.Right:
                _lastDirection = Direction.Up;
                break;

            case Direction.Down:
            case Direction.DownLeft when _lastDirection != Direction.Left:
            case Direction.DownRight when _lastDirection != Direction.Right:
            default:
                _lastDirection = Direction.Down;
                break;
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

    /// <summary>
    /// Draws the equipment of entities using paperdoll texture files.
    /// </summary>
    public virtual void DrawEquipment(string filename, Color renderColor)
    {
        // Get the current map.
        var map = Maps.MapInstance.Get(MapId);

        // If the map is null or hideEquipment is true: do nothing.
        if (map == null || map.HideEquipment)
        {
            return;
        }

        // Paperdoll textures and Frames.
        IGameTexture? paperdollTex = null;
        var spriteFrames = SpriteFrames;

        // Extract filename without it's extension.
        var filenameNoExt = Path.GetFileNameWithoutExtension(filename);

        // Equipment's custom paperdoll texture.
        if (SpriteAnimation is SpriteAnimations.Attack or
            SpriteAnimations.Cast or SpriteAnimations.Weapon or SpriteAnimations.Shoot)
        {
            // Extract animation name from the AnimatedTextures list.
            var animationName = Path.GetFileNameWithoutExtension(AnimatedTextures[SpriteAnimation].Name);

            // Extract the substring after the separator.
            var separatorIndex = animationName.IndexOf('_') + 1;
            var customAnimationName = animationName[separatorIndex..];

            // Try to get custom paperdoll texture.
            var customPaperdollTex =
                Globals.ContentManager.GetTexture(TextureType.Paperdoll,
                    $"{filenameNoExt}_{customAnimationName}.png");

            // If custom paperdoll texture exists, use it.
            if (customPaperdollTex != null)
            {
                paperdollTex = customPaperdollTex;
            }
        }

        // If there's no custom paperdoll: use the paperdoll texture based on the SpriteAnimation.
        if (paperdollTex == null && !string.IsNullOrEmpty($"{SpriteAnimation}"))
        {
            paperdollTex = Globals.ContentManager.GetTexture(TextureType.Paperdoll,
                $"{filenameNoExt}_{SpriteAnimation}.png");
        }

        // If the paperdoll texture is still null: try to get the default texture.
        if (paperdollTex == null)
        {
            paperdollTex = Globals.ContentManager.GetTexture(TextureType.Paperdoll, filename);
            spriteFrames = Options.Instance.Sprites.NormalFrames;
        }

        // If the paperdoll texture is null at this point: do nothing.
        if (paperdollTex == null)
        {
            return;
        }

        // Calculate: direction, frame width and frame height.
        var spriteRow = PickSpriteRow(DirectionFacing);
        var frameWidth = paperdollTex.Width / spriteFrames;
        var frameHeight = paperdollTex.Height / Options.Instance.Sprites.Directions;

        // Calculate: source and destination rectangles.
        var frame = SpriteFrame;
        if (SpriteAnimation == SpriteAnimations.Normal)
        {
            frame = NormalSpriteAnimationFrame;
        }

        var srcRectangle = new FloatRect(frame * frameWidth, spriteRow * frameHeight, frameWidth, frameHeight);
        var destRectangle = new FloatRect(
            (int)Math.Ceiling(Center.X - frameWidth / 2f),
            (int)Math.Ceiling(Center.Y - frameHeight / 2f),
            srcRectangle.Width,
            srcRectangle.Height
        );

        // Draw the paperdoll texture using: source and destination rectangles along with the render color.
        Graphics.DrawGameTexture(paperdollTex, srcRectangle, destRectangle, renderColor);
    }

    protected virtual void CalculateOrigin()
    {
        if (LatestMap?.IsDisposed ?? false)
        {
            LatestMap = Maps.MapInstance.TryGet(MapId, out MapInstance updatedInstance) ? updatedInstance : null;
        }

        if (LatestMap == default)
        {
            mOrigin = default;
            return;
        }

        mOrigin = new Vector2(
            LatestMap.X + X * TileWidth + OffsetX + TileWidth / 2,
            LatestMap.Y + Y * TileHeight + OffsetY + TileHeight
        );
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
        Color? borderColor = null,
        Color? backgroundColor = null
    )
    {
        // Are we supposed to hide this Label?
        if (!ShouldDrawName || string.IsNullOrWhiteSpace(label) || Graphics.Renderer == default)
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

        var textSize = Graphics.Renderer.MeasureText(label, Graphics.EntityNameFont, Graphics.EntityNameFontSize, 1);

        var x = (int)Math.Ceiling(Origin.X);
        var y = position == 0 ? GetLabelLocation(LabelType.Header) : GetLabelLocation(LabelType.Footer);

        if (backgroundColor != Color.Transparent)
        {
            Graphics.DrawGameTexture(
                Graphics.Renderer.WhitePixel,
                new FloatRect(0, 0, 1, 1),
                new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y),
                backgroundColor
            );
        }

        Graphics.Renderer.DrawString(
            label,
            Graphics.EntityNameFont,
            Graphics.EntityNameFontSize,
            x - (int)Math.Ceiling(textSize.X / 2f),
            (int)y,
            1,
            Color.FromArgb(textColor.ToArgb()),
            true,
            null,
            Color.FromArgb(borderColor.ToArgb())
        );
    }

    public virtual void DrawName(Color? textColor, Color? borderColor = null, Color? backgroundColor = null)
    {
        // Are we really supposed to draw this name?
        if (!ShouldDrawName || Graphics.Renderer == default)
        {
            return;
        }

        //Check for npc colors
        if (textColor == null)
        {
            var color = Aggression switch
            {
                NpcAggression.Aggressive => (LabelColor?)CustomColors.Names.Npcs["Aggressive"],
                NpcAggression.AttackWhenAttacked => (LabelColor?)CustomColors.Names.Npcs["AttackWhenAttacked"],
                NpcAggression.AttackOnSight => (LabelColor?)CustomColors.Names.Npcs["AttackOnSight"],
                NpcAggression.Guard => (LabelColor?)CustomColors.Names.Npcs["Guard"],
                _ => (LabelColor?)CustomColors.Names.Npcs["Neutral"],
            };

            if (color != null)
            {
                textColor = color?.Name;
                backgroundColor ??= color?.Background;
                borderColor ??= color?.Outline;
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

        var name = Name;
        if ((this is Player && Options.Instance.Player.ShowLevelByName) ||
            (Type == EntityType.GlobalEntity && Options.Instance.Npc.ShowLevelByName))
        {
            name = Strings.GameWindow.EntityNameAndLevel.ToString(Name, Level);
        }

        var textSize = Graphics.Renderer.MeasureText(name, Graphics.EntityNameFont, Graphics.EntityNameFontSize, 1);

        var x = (int)Math.Ceiling(Origin.X);
        var y = GetLabelLocation(LabelType.Name);

        if (backgroundColor != Color.Transparent)
        {
            Graphics.DrawGameTexture(
                Graphics.Renderer.WhitePixel,
                new FloatRect(0, 0, 1, 1),
                new FloatRect(x - textSize.X / 2f - 4, y, textSize.X + 8, textSize.Y),
                backgroundColor
            );
        }

        Graphics.Renderer.DrawString(
            name,
            Graphics.EntityNameFont,
            Graphics.EntityNameFontSize,
            x - (int)Math.Ceiling(textSize.X / 2f),
            (int)y,
            1,
            textColor,
            true,
            null,
            Color.FromArgb(borderColor.ToArgb())
        );
    }

    public float GetLabelLocation(LabelType type)
    {
        if (Graphics.Renderer == default)
        {
            return 0f;
        }

        var y = GetTop() - 8;

        //Need room for HP bar if not an event.
        if (this is not Event && ShouldDrawHpBar)
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

                var headerSize = Graphics.Renderer.MeasureText(
                    HeaderLabel.Text,
                    Graphics.EntityNameFont,
                    Graphics.EntityNameFontSize,
                    1
                );
                y -= headerSize.Y + 2;
                break;

            case LabelType.Footer:
                if (string.IsNullOrWhiteSpace(FooterLabel.Text))
                {
                    break;
                }

                var footerSize = Graphics.Renderer.MeasureText(
                    FooterLabel.Text,
                    Graphics.EntityNameFont,
                    Graphics.EntityNameFontSize,
                    1
                );
                y -= footerSize.Y - 6;
                break;

            case LabelType.Name:
                y = GetLabelLocation(LabelType.Footer);
                var nameSize = Graphics.Renderer.MeasureText(
                    Name,
                    Graphics.EntityNameFont,
                    Graphics.EntityNameFontSize,
                    1
                );
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

                    var guildSize = Graphics.Renderer.MeasureText(
                        player.Guild,
                        Graphics.EntityNameFont,
                        Graphics.EntityNameFontSize,
                        1
                    );
                    y -= 2 + guildSize.Y;
                }

                break;
        }

        return y;
    }

    public long ShieldSize =>
        Status.Sum(status => status.Type == SpellEffect.Shield ? status.Shield[(int)Enums.Vital.Health] : 0);

    protected virtual bool ShouldDrawHpBar
    {
        get
        {
            if (ShouldNotDrawHpBar)
            {
                return false;
            }

            if (IsHovered)
            {
                return true;
            }

            if (ShieldSize > 0)
            {
                return true;
            }

            if (Vital[(int)Enums.Vital.Health] != MaxVital[(int)Enums.Vital.Health])
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
            return LatestMap == default || !ShouldDraw || Vital[(int)Enums.Vital.Health] < 1;
        }
    }

    public IGameTexture GetBoundingHpBarTexture()
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
        var maxVital = MaxVital[(int)Enums.Vital.Health];
        var shieldSize = ShieldSize;

        if (shieldSize + Vital[(int)Enums.Vital.Health] > maxVital)
        {
            maxVital = shieldSize + Vital[(int)Enums.Vital.Health];
        }

        var hpfillRatio = (float)Vital[(int)Enums.Vital.Health] / maxVital;
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

        var castingSpell = SpellDescriptor.Get(SpellCast);
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
                new FloatRect(0, 0, castForeground.Width * fillRatio, castForeground.Height),
                new FloatRect(x - castForeground.Width / 2, y - castForeground.Height / 2, castForeground.Width * fillRatio, castForeground.Height), Color.White
            );
        }
    }

    public bool ShouldDrawTarget => this is not Projectile && LatestMap != default;

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
            priority * targetTexture.Width / 2f,
            0,
            targetTexture.Width / 2f,
            targetTexture.Height
        );

        var destRectangle = new FloatRect(
            (float)Math.Ceiling(WorldPos.X + (WorldPos.Width - targetTexture.Width / 2) / 2),
            (float)Math.Ceiling(WorldPos.Y + (WorldPos.Height - targetTexture.Height) / 2),
            srcRectangle.Width,
            srcRectangle.Height
        );

        Graphics.DrawGameTexture(targetTexture, srcRectangle, destRectangle, Color.White);
    }

    public virtual bool CanBeAttacked => true;

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

    public IStatus? GetStatus(Guid guid)
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
        Status = [.. Status.OrderByDescending(x => x.RemainingMs)];
    }

    private void UpdateSpriteAnimation()
    {
        //Exit if textures haven't been loaded yet
        if (AnimatedTextures.Count == 0)
        {
            return;
        }

        var timingMilliseconds = Timing.Global.Milliseconds;
        var isNotBlockingAndCasting = !IsBlocking && !IsCasting;

        SpriteAnimation = SpriteAnimations.Normal;
        if (AnimatedTextures.TryGetValue(SpriteAnimations.Idle, out _) &&
            LastActionTime + Options.Instance.Sprites.IdleStartDelay < timingMilliseconds &&
            isNotBlockingAndCasting)
        {
            SpriteAnimation = SpriteAnimations.Idle;
        }

        if (IsMoving && !IsAttacking && isNotBlockingAndCasting)
        {
            SpriteAnimation = SpriteAnimations.Normal;
            LastActionTime = timingMilliseconds;
        }

        if (IsAttacking && isNotBlockingAndCasting)
        {
            // Normal sprite-sheets has their own handler for attacking frames.
            if (AnimatedTextures.TryGetValue(SpriteAnimations.Normal, out _))
            {
                return;
            }

            var timeInAttack = CalculateAttackTime() - (AttackTimer - timingMilliseconds);
            LastActionTime = Timing.Global.Milliseconds;

            if (AnimatedTextures.TryGetValue(SpriteAnimations.Attack, out _))
            {
                SpriteAnimation = SpriteAnimations.Attack;
            }

            if (Options.Instance.Equipment.WeaponSlot > -1 && Options.Instance.Equipment.WeaponSlot < Equipment.Length)
            {
                if (Equipment[Options.Instance.Equipment.WeaponSlot] != Guid.Empty && this != Globals.Me ||
                    MyEquipment[Options.Instance.Equipment.WeaponSlot] < Options.Instance.Player.MaxInventory)
                {
                    var itemId = Guid.Empty;
                    if (this == Globals.Me)
                    {
                        var slot = MyEquipment[Options.Instance.Equipment.WeaponSlot];
                        if (slot > -1)
                        {
                            itemId = Inventory[slot].ItemId;
                        }
                    }
                    else
                    {
                        itemId = Equipment[Options.Instance.Equipment.WeaponSlot];
                    }

                    var item = ItemDescriptor.Get(itemId);
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

            switch (SpriteAnimation)
            {
                case SpriteAnimations.Cast:
                case SpriteAnimations.Idle:
                case SpriteAnimations.Normal:
                    break;
                case SpriteAnimations.Attack:
                case SpriteAnimations.Shoot:
                case SpriteAnimations.Weapon:
                default:
                    SpriteFrame = (int)Math.Floor(timeInAttack / (CalculateAttackTime() / (float)SpriteFrames));
                    break;
            }
        }

        if (IsCasting)
        {
            var spell = SpellDescriptor.Get(SpellCast);
            if (spell != null)
            {
                var duration = spell.CastDuration;
                var timeInCast = duration - (CastTime - timingMilliseconds);

                // Normal sprite-sheets has their own handler for attacking frames.
                if (AnimatedTextures.TryGetValue(SpriteAnimations.Cast, out _))
                {
                    SpriteAnimation = SpriteAnimations.Cast;
                }

                if (spell.SpellType == SpellType.CombatSpell &&
                    spell.Combat.TargetType == SpellTargetType.Projectile &&
                    spell.CastSpriteOverride == null)
                {
                    if (AnimatedTextures.TryGetValue(SpriteAnimations.Shoot, out _))
                    {
                        SpriteAnimation = SpriteAnimations.Shoot;
                    }
                }

                SpriteFrame = (int)Math.Floor(timeInCast / (duration / (float)SpriteFrames));
            }

            LastActionTime = timingMilliseconds;
        }

        if (SpriteAnimation == SpriteAnimations.Normal)
        {
            ResetSpriteFrame();
        }
        else if (SpriteAnimation == SpriteAnimations.Idle)
        {
            if (SpriteFrameTimer + Options.Instance.Sprites.IdleFrameDuration < timingMilliseconds)
            {
                SpriteFrame++;
                if (SpriteFrame >= SpriteFrames)
                {
                    SpriteFrame = 0;
                }

                SpriteFrameTimer = timingMilliseconds;
            }
        }
    }

    public void ResetSpriteFrame()
    {
        SpriteFrame = 0;
        SpriteFrameTimer = 0;
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
        var textureOverride = string.Empty;
        var weaponId = Equipment[Options.Instance.Equipment.WeaponSlot];

        switch (spriteAnimation)
        {
            // No override for these animations.
            case SpriteAnimations.Normal:
            case SpriteAnimations.Idle:

                break;

            case SpriteAnimations.Attack:
                if (this is Player player && ClassDescriptor.TryGet(player.Class, out var classDescriptor))
                {
                    textureOverride = classDescriptor.AttackSpriteOverride;
                }

                break;

            case SpriteAnimations.Shoot:
                if (Equipment.Length <= Options.Instance.Equipment.WeaponSlot)
                {
                    break;
                }

                if (ItemDescriptor.TryGet(weaponId, out var shootItemDescriptor))
                {
                    textureOverride = shootItemDescriptor.WeaponSpriteOverride;
                }

                if (!string.IsNullOrWhiteSpace(textureOverride))
                {
                    spriteAnimationOveride = SpriteAnimations.Shoot;
                }

                break;

            case SpriteAnimations.Cast:
                if (SpellDescriptor.TryGet(SpellCast, out var spellDescriptor))
                {
                    textureOverride = spellDescriptor.CastSpriteOverride;
                }

                if (!string.IsNullOrWhiteSpace(textureOverride))
                {
                    spriteAnimationOveride = SpriteAnimations.Cast;
                }

                break;

            case SpriteAnimations.Weapon:
                if (Equipment.Length <= Options.Instance.Equipment.WeaponSlot)
                {
                    break;
                }

                if (ItemDescriptor.TryGet(weaponId, out var weaponItemDescriptor))
                {
                    textureOverride = weaponItemDescriptor.WeaponSpriteOverride;
                }

                if (!string.IsNullOrWhiteSpace(textureOverride))
                {
                    spriteAnimationOveride = SpriteAnimations.Weapon;
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

    protected virtual bool TryGetAnimationTexture(string textureName, SpriteAnimations spriteAnimation, string textureOverride, out IGameTexture texture)
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
    /// <para>Returns the direction to a player's selected target.</para>
    /// <para>Original logic made by Daywalkr (Middle Ages: Online), thanks for sharing!</para>
    /// <para>Modified by Arufonsu for the Intersect Engine along with support for 8 directions when enabled.</para>
    /// </summary>
    /// <param name="en">entity's target</param>
    /// <returns>direction to player's selected target</returns>
    public Direction DirectionToTarget(Entity en)
    {
        if (en == null)
        {
            return DirectionFacing;
        }

        var originMapController = MapInstance;
        var targetMapController = en.MapInstance;

        if (originMapController == null || targetMapController == null)
        {
            return DirectionFacing;
        }

        var originY = Y + originMapController.GridY * MapHeight;
        var originX = X + originMapController.GridX * MapWidth;
        var targetY = en.Y + targetMapController.GridY * MapHeight;
        var targetX = en.X + targetMapController.GridX * MapWidth;

        // Calculate the offset between origin and target along both of their axis.
        var yDiff = originY - targetY;
        var xDiff = originX - targetX;

        // If Y offset is 0, direction is determined by X offset.
        if (yDiff == 0)
        {
            return xDiff > 0 ? Direction.Left : Direction.Right;
        }

        // If X offset is 0 or If diagonal movement is disabled, direction is determined by Y offset.
        if (xDiff == 0 || !Options.Instance.Map.EnableDiagonalMovement)
        {
            return yDiff > 0 ? Direction.Up : Direction.Down;
        }

        // If both X and Y offset are non-zero, direction is determined by both offsets.
        var xPositive = xDiff > 0;
        var yPositive = yDiff > 0;

        if (xPositive)
        {
            return yPositive ? Direction.UpLeft : Direction.DownLeft;
        }

        return yPositive ? Direction.UpRight : Direction.DownRight;
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
        Point delta,
        int z,
        Guid mapId,
        ref IEntity? blockedBy,
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
            var tmpX = delta.X;
            var tmpY = delta.Y;
            var tmpMapId = Guid.Empty;
            if (delta.X < 0)
            {
                gridX--;
                tmpX = MapWidth - delta.X * -1;
            }

            if (delta.Y < 0)
            {
                gridY--;
                tmpY = MapHeight - delta.Y * -1;
            }

            if (delta.X > MapWidth - 1)
            {
                gridX++;
                tmpX = delta.X - MapWidth;
            }

            if (delta.Y > MapHeight - 1)
            {
                gridY++;
                tmpY = delta.Y - MapHeight;
            }

            if (Globals.MapGrid == default || gridX < 0 || gridY < 0 || gridX >= Globals.MapGridWidth || gridY >= Globals.MapGridHeight)
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
                        if (en.Value is not Projectile)
                        {
                            switch (en.Value)
                            {
                                case Resource resource:
                                    var resourceDescriptor = resource.Descriptor;
                                    if (resourceDescriptor == null)
                                    {
                                        break;
                                    }

                                    if (projectileTrigger)
                                    {
                                        bool isDead = resource.IsDead;
                                        if ((ignoreAliveResources || isDead) && (ignoreDeadResources || !isDead))
                                        {
                                            return -1;
                                        }

                                        blockedBy = en.Value;

                                        return -6;

                                    }

                                    if (resourceDescriptor.WalkableAfter && resource.IsDead ||
                                        resourceDescriptor.WalkableBefore && !resource.IsDead)
                                    {
                                        continue;
                                    }

                                    break;

                                case Player player:
                                    // Return the entity key as this should block the player.  Only exception is if the MapZone this entity is on is passable.
                                    if (Maps.MapInstance.TryGet(player.MapId, out var playerMapInstance) &&
                                        Options.Instance.Passability.IsPassable(playerMapInstance.ZoneType))
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
                        blockedBy = en.Value;

                        return -4;
                    }
                }
            }

            var gameMap = Maps.MapInstance.Get(Globals.MapGrid[gridX, gridY]);
            if (gameMap != null)
            {
                if (gameMap.Attributes[tmpX, tmpY] != null)
                {
                    if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributeType.Blocked || (gameMap.Attributes[tmpX, tmpY].Type == MapAttributeType.Animation && ((MapAnimationAttribute)gameMap.Attributes[tmpX, tmpY]).IsBlock))
                    {
                        return -2;
                    }
                    else if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributeType.ZDimension)
                    {
                        if (((MapZDimensionAttribute)gameMap.Attributes[tmpX, tmpY]).BlockedLevel - 1 == z)
                        {
                            return -3;
                        }
                    }
                    else if (gameMap.Attributes[tmpX, tmpY].Type == MapAttributeType.NpcAvoid)
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

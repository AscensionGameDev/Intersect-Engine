using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Newtonsoft.Json;
using MapAttribute = Intersect.Enums.MapAttribute;
using Stat = Intersect.Enums.Stat;

namespace Intersect.Server.Entities
{
    public abstract partial class Entity : IDisposable
    {
        //Instance Values
        private Guid _id = Guid.NewGuid();

        public Guid MapInstanceId = Guid.Empty;

        [JsonProperty("MaxVitals"), NotMapped] private int[] _maxVital = new int[Enum.GetValues<Vital>().Length];

        [NotMapped, JsonIgnore] public Combat.Stat[] Stat = new Combat.Stat[Enum.GetValues<Stat>().Length];

        [NotMapped, JsonIgnore] public Entity Target { get; set; } = null;

        public Entity() : this(Guid.NewGuid(), Guid.Empty)
        {
        }

        //Initialization
        public Entity(Guid instanceId, Guid mapInstanceId)
        {
            if (!(this is EventPageInstance) && !(this is Projectile))
            {
                for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
                {
                    Stat[i] = new Combat.Stat((Stat)i, this);
                }
            }
            MapInstanceId = mapInstanceId;

            Id = instanceId;
        }

        [Column(Order = 1), JsonProperty(Order = -2)]
        public string Name { get; set; }

        public Guid MapId { get; set; }

        [NotMapped]
        public string MapName => MapController.GetName(MapId);

        [JsonIgnore]
        [NotMapped]
        public MapController Map => MapController.Get(MapId);

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Direction Dir { get; set; }

        public string Sprite { get; set; }

        /// <summary>
        /// The database compatible version of <see cref="Color"/>
        /// </summary>
        [JsonIgnore, Column(nameof(Color))]
        public string JsonColor
        {
            get => JsonConvert.SerializeObject(Color);
            set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
        }

        /// <summary>
        /// Defines the ARGB color settings for this Entity.
        /// </summary>
        [NotMapped]
        public Color Color { get; set; } = new Color(255, 255, 255, 255);

        public string Face { get; set; }

        public int Level { get; set; }

        [JsonIgnore, Column("Vitals")]
        public string VitalsJson
        {
            get => DatabaseUtils.SaveIntArray(mVitals, Enum.GetValues<Vital>().Length);
            set => mVitals = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Vital>().Length);
        }

        [JsonProperty("Vitals"), NotMapped]
        private int[] mVitals { get; set; } = new int[Enum.GetValues<Vital>().Length];

        [JsonIgnore, NotMapped]
        private int[] mOldVitals { get; set; } = new int[Enum.GetValues<Vital>().Length];

        [JsonIgnore, NotMapped]
        private int[] mOldMaxVitals { get; set; } = new int[Enum.GetValues<Vital>().Length];

        //Stats based on npc settings, class settings, etc for quick calculations
        [JsonIgnore, Column(nameof(BaseStats))]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(BaseStats, Enum.GetValues<Stat>().Length);
            set => BaseStats = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
        }

        // TODO: Why can this be BaseStats while Vitals is _vital and MaxVitals is _maxVital?
        [NotMapped]
        public int[] BaseStats { get; set; } = new int[Enum.GetValues<Stat>().Length];

        [JsonIgnore, Column(nameof(StatPointAllocations))]
        public string StatPointsJson
        {
            get => DatabaseUtils.SaveIntArray(StatPointAllocations, Enum.GetValues<Stat>().Length);
            set => StatPointAllocations = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
        }

        [NotMapped]
        public int[] StatPointAllocations { get; set; } = new int[Enum.GetValues<Stat>().Length];

        //Inventory
        [JsonIgnore]
        public virtual List<InventorySlot> Items { get; set; } = new List<InventorySlot>();

        //Spells
        [JsonIgnore]
        public virtual List<SpellSlot> Spells { get; set; } = new List<SpellSlot>();

        [JsonIgnore, Column(nameof(NameColor))]
        public string NameColorJson
        {
            get => DatabaseUtils.SaveColor(NameColor);
            set => NameColor = DatabaseUtils.LoadColor(value);
        }

        [NotMapped]
        public Color NameColor { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public Guid Id { get => _id; set => _id = value; }

        [NotMapped]
        public Label HeaderLabel { get; set; }

        [JsonIgnore, Column(nameof(HeaderLabel))]
        public string HeaderLabelJson
        {
            get => JsonConvert.SerializeObject(HeaderLabel);
            set => HeaderLabel = value != null ? JsonConvert.DeserializeObject<Label>(value) : new Label();
        }

        [NotMapped]
        public Label FooterLabel { get; set; }

        [JsonIgnore, Column(nameof(FooterLabel))]
        public string FooterLabelJson
        {
            get => JsonConvert.SerializeObject(FooterLabel);
            set => FooterLabel = value != null ? JsonConvert.DeserializeObject<Label>(value) : new Label();
        }

        [NotMapped]
        public bool Dead { get; set; }

        //Combat
        [NotMapped, JsonIgnore]
        public long CastTime { get; set; }

        [NotMapped, JsonIgnore]
        public long AttackTimer { get; set; }

        [NotMapped, JsonIgnore]
        public Entity CastTarget { get; set; }

        [NotMapped, JsonIgnore]
        public Guid CollisionIndex { get; set; }

        [NotMapped, JsonIgnore]
        public long CombatTimer { get; set; }

        //Combat Status
        [NotMapped, JsonIgnore]
        public bool IsAttacking => AttackTimer > Timing.Global.Milliseconds;

        [NotMapped, JsonIgnore]
        public bool IsBlocking { get; set; }

        [NotMapped, JsonIgnore]
        public bool IsCasting => CastTime > Timing.Global.Milliseconds;

        [NotMapped, JsonIgnore]
        public bool IsTurnAroundWhileCastingDisabled => !Options.Instance.CombatOpts.EnableTurnAroundWhileCasting && IsCasting;

        //Visuals
        [NotMapped, JsonIgnore]
        public bool HideName { get; set; }

        [NotMapped, JsonIgnore]
        public bool HideEntity { get; set; } = false;

        [NotMapped, JsonIgnore]
        public List<Guid> Animations { get; set; } = new List<Guid>();

        //DoT/HoT Spells
        [NotMapped, JsonIgnore]
        public ConcurrentDictionary<Guid, DoT> DoT { get; set; } = new ConcurrentDictionary<Guid, DoT>();

        [NotMapped, JsonIgnore]
        public DoT[] CachedDots { get; set; } = new DoT[0];

        [NotMapped, JsonIgnore]
        public EventMoveRoute MoveRoute { get; set; } = null;

        [NotMapped, JsonIgnore]
        public EventPageInstance MoveRouteSetter { get; set; } = null;

        [NotMapped, JsonIgnore]
        public long MoveTimer { get; set; }

        [NotMapped, JsonIgnore]
        public bool Passable { get; set; } = false;

        [NotMapped, JsonIgnore]
        public long RegenTimer { get; set; } = Timing.Global.Milliseconds;

        [NotMapped, JsonIgnore]
        public int SpellCastSlot { get; set; } = 0;

        //Status effects
        [NotMapped, JsonIgnore]
        public ConcurrentDictionary<SpellBase, Status> Statuses { get; } = new ConcurrentDictionary<SpellBase, Status>();

        [JsonIgnore, NotMapped]
        public Status[] CachedStatuses = new Status[0];

        [JsonIgnore, NotMapped]
        private Status[] mOldStatuses = new Status[0];

        [JsonIgnore, NotMapped]
        public List<SpellEffect> Immunities = new List<SpellEffect>();

        [NotMapped, JsonIgnore]
        public bool IsDisposed { get; protected set; }

        [NotMapped, JsonIgnore]
        public object EntityLock = new object();

        [NotMapped, JsonIgnore]
        public bool VitalsUpdated
        {
            get => !GetVitals().SequenceEqual(mOldVitals) || !GetMaxVitals().SequenceEqual(mOldMaxVitals);

            set
            {
                if (value == false)
                {
                    mOldVitals = GetVitals();
                    mOldMaxVitals = GetMaxVitals();
                }
            }
        }

        [NotMapped, JsonIgnore]
        public bool StatusesUpdated
        {
            get => CachedStatuses != mOldStatuses; //The whole CachedStatuses assignment gets changed when a status is added, removed, or updated (time remaining changes, so we only check for reference equivity here)

            set
            {
                if (value == false)
                {
                    mOldStatuses = CachedStatuses;
                }
            }
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        public virtual void Update(long timeMs)
        {
            var lockObtained = false;
            try
            {
                Monitor.TryEnter(EntityLock, ref lockObtained);
                if (lockObtained)
                {
                    if (Target?.IsDisposed ?? false)
                    {
                        Target = default;
                    }

                    //Cast timers
                    if (CastTime != 0 && !IsCasting && SpellCastSlot < Spells.Count && SpellCastSlot >= 0)
                    {
                        CastTime = 0;
                        CastSpell(Spells[SpellCastSlot].SpellId, SpellCastSlot);
                        CastTarget = null;
                    }

                    //DoT/HoT timers
                    foreach (var dot in CachedDots)
                    {
                        dot.Tick();
                    }

                    if (!(this is EventPageInstance) && !(this is Projectile))
                    {
                        var statsUpdated = false;
                        var statTime = Timing.Global.Milliseconds;
                        for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
                        {
                            var stat = Stat[i];
                            if (stat == default)
                            {
                                var allStats = string.Join(",\n", Stat.Select(s => s == default ? "\tnull" : $"\t{s}"));
                                Log.Error($"Stat[{i}] == default for '{GetType().FullName}', Stat=[\n{allStats}\n]");
                            }
                            statsUpdated |= stat.Update(statTime);
                        }

                        if (statsUpdated)
                        {
                            PacketSender.SendEntityStats(this);
                        }
                    }

                    //Regen Timers and regen in combat validation
                    if ((timeMs > CombatTimer || Options.Instance.CombatOpts.RegenVitalsInCombat) && timeMs > RegenTimer)
                    {
                        ProcessRegen();
                        RegenTimer = timeMs + Options.RegenTime;
                    }

                    //Status timers
                    var statusArray = CachedStatuses;
                    foreach (var status in statusArray)
                    {
                        status.TryRemoveStatus();
                    }

                    //Blocking timers
                    if (IsBlocking && !IsAttacking)
                    {
                        IsBlocking = false;
                        PacketSender.SendEntityAttack(this, -1);
                    }
                }
            }
            finally
            {
                if (lockObtained)
                {
                    Monitor.Exit(EntityLock);
                }
            }
        }

        /// <summary>
        ///     Updates the entity's spell cooldown for the specified <paramref name="spellBase"/>.
        ///     <para> This method is called when a spell is casted by an entity. </para>
        /// </summary>
        public virtual void UpdateSpellCooldown(SpellBase spellBase, int spellSlot)
        {
            if (spellSlot < 0 || spellSlot >= Options.MaxPlayerSkills)
            {
                return;
            }

            SpellCooldowns[Spells[spellSlot].SpellId] = Timing.Global.MillisecondsUtc + spellBase.CooldownDuration;
        }

        /// <summary>
        ///     Determines if this entity can move in the specified <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The <see cref="Direction"/> the entity is attempting to move in.</param>
        /// <returns>If the entity is able to move in the specified direction.</returns>
        public bool CanMoveInDirection(Direction direction) => CanMoveInDirection(direction, out _, out _);

        /// <summary>
        ///     Determines if this entity can move in the specified <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The <see cref="Direction"/> the entity is attempting to move in.</param>
        /// <param name="blockerType">The type of blocker, if any.</param>
        /// <param name="entityType">
        ///     The type of entity that is blocking movement, if <paramref name="blockerType"/> is set to <see cref="MovementBlockerType.Entity"/>.
        /// </param>
        /// <returns>If the entity is able to move in the specified direction.</returns>
        public virtual bool CanMoveInDirection(
            Direction direction,
            out MovementBlockerType blockerType,
            out EntityType entityType
        )
        {
            entityType = default;

            var xOffset = 0;
            var yOffset = 0;

            var tileHelper = new TileHelper(MapId, X, Y);
            switch (direction)
            {
                case Direction.Up:
                    yOffset--;
                    break;

                case Direction.Down:
                    yOffset++;
                    break;

                case Direction.Left:
                    xOffset--;
                    break;

                case Direction.Right:
                    xOffset++;
                    break;

                case Direction.UpLeft:
                    yOffset--;
                    xOffset--;
                    break;

                case Direction.UpRight:
                    yOffset--;
                    xOffset++;
                    break;

                case Direction.DownRight:
                    yOffset++;
                    xOffset++;
                    break;

                case Direction.DownLeft:
                    yOffset++;
                    xOffset--;
                    break;

                case Direction.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (!tileHelper.Translate(xOffset, yOffset))
            {
                blockerType = MovementBlockerType.OutOfBounds;
                return false;
            }

            if (!MapController.TryGet(tileHelper.GetMapId(), out var mapController))
            {
                blockerType = MovementBlockerType.OutOfBounds;
                return false;
            }

            int tileX = tileHelper.GetX();
            int tileY = tileHelper.GetY();

            if (IsBlockedByMapAttribute(direction, mapController.Attributes[tileX, tileY], out blockerType))
            {
                return false;
            }

            var enableCrossingDiagonalBlocks = Options.Instance.MapOpts.EnableCrossingDiagonalBlocks;
            if (!enableCrossingDiagonalBlocks && direction.IsDiagonal())
            {
                MovementBlockerType componentBlockerType = default;
                EntityType componentBlockingEntityType = default;

                if (direction.GetComponentDirections()
                    .All(
                        componentDirection => !CanMoveInDirection(
                            componentDirection,
                            out componentBlockerType,
                            out componentBlockingEntityType
                        )
                    ))
                {
                    blockerType = componentBlockerType;
                    entityType = componentBlockingEntityType;
                    return false;
                }
            }

            if (Passable)
            {
                return !TryGetBlockerOnTile(
                    tileHelper.GetMap(),
                    tileX,
                    tileY,
                    Z,
                    out blockerType,
                    out entityType
                );
            }

            var mapEntities = new List<Entity>();
            if (mapController.TryGetInstance(MapInstanceId, out var mapInstance))
            {
                mapEntities.AddRange(mapInstance.GetCachedEntities().Where(entity => entity != default && !entity.Passable));
            }

            foreach (var mapEntity in mapEntities.Where(en => en.X == tileHelper.GetX() && en.Y == tileHelper.GetY() && en.Z == Z))
            {
                // Set a target if a projectile
                CollisionIndex = mapEntity.Id;
                switch (mapEntity)
                {
                    case Player _ when !CanPassPlayer(mapController):
                        blockerType = MovementBlockerType.Entity;
                        entityType = EntityType.Player;
                        return false;
                    case Npc _:
                        // There should honestly be an Npc EntityType...
                        blockerType = MovementBlockerType.Entity;
                        entityType = EntityType.Player;
                        return false;
                    case Resource resource when !resource.IsPassable():
                        blockerType = MovementBlockerType.Entity;
                        entityType = EntityType.Resource;
                        return false;
                }
            }

            if (IsBlockedByEvent(mapInstance, tileHelper.GetX(), tileHelper.GetY()))
            {
                blockerType = MovementBlockerType.Entity;
                entityType = EntityType.Event;
                return false;
            }

            if (TryGetBlockerOnTile(
                    tileHelper.GetMap(),
                    tileHelper.GetX(),
                    tileHelper.GetY(),
                    Z,
                    out blockerType,
                    out entityType
                ))
            {
                return false;
            }

            return blockerType == MovementBlockerType.NotBlocked;
        }

        protected virtual bool CanMoveOntoSlide(Direction movementDirection, Direction slideDirection) =>
            !movementDirection.IsOppositeOf(slideDirection);

        protected virtual bool IgnoresNpcAvoid => true;

        private bool IsBlockedByMapAttribute(
            Direction direction,
            Intersect.GameObjects.Maps.MapAttribute mapAttribute,
            out MovementBlockerType blockerType
        )
        {
            blockerType = MovementBlockerType.NotBlocked;
            if (mapAttribute == default)
            {
                return false;
            }

            switch (mapAttribute)
            {
                case MapBlockedAttribute _:
                case MapAnimationAttribute animationAttribute when animationAttribute.IsBlock:
                case MapNpcAvoidAttribute _ when !IgnoresNpcAvoid:
                    blockerType = MovementBlockerType.MapAttribute;
                    break;

                case MapZDimensionAttribute zDimensionAttribute when zDimensionAttribute.BlockedLevel > 0 && zDimensionAttribute.BlockedLevel - 1 == Z:
                    blockerType = MovementBlockerType.ZDimension;
                    break;

                case MapSlideAttribute slideAttribute when !CanMoveOntoSlide(direction, slideAttribute.Direction):
                    blockerType = MovementBlockerType.Slide;
                    break;
            }

            return blockerType != MovementBlockerType.NotBlocked;
        }

        protected virtual bool CanPassPlayer(MapController targetMap) => false;

        protected virtual bool IsBlockedByEvent(MapInstance mapInstance, int tileX, int tileY)
        {
            if (mapInstance == default)
            {
                return false;
            }

            return mapInstance.GlobalEventInstances.Values
                .SelectMany(globalEventInstance => globalEventInstance.GlobalPageInstance)
                .Where(instance => instance != default && !instance.Passable)
                .Any(instance => instance.X == tileX && instance.Y == tileY && instance.Z == Z);
        }

        protected virtual bool TryGetBlockerOnTile(
            MapController map,
            int x,
            int y,
            int z,
            out MovementBlockerType blockerType,
            out EntityType entityType
        )
        {
            blockerType = map == default ? MovementBlockerType.OutOfBounds : MovementBlockerType.NotBlocked;
            entityType = default;
            return blockerType != MovementBlockerType.NotBlocked;
        }

        protected virtual bool ProcessMoveRoute(Player forPlayer, long timeMs)
        {
            var moved = false;
            Direction lookDir = 0;
            Direction moveDir = 0;
            if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
            {
                switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                {
                    case MoveRouteEnum.MoveUp:
                        if (CanMoveInDirection(Direction.Up))
                        {
                            Move(Direction.Up, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveDown:
                        if (CanMoveInDirection(Direction.Down))
                        {
                            Move(Direction.Down, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveLeft:
                        if (CanMoveInDirection(Direction.Left))
                        {
                            Move(Direction.Left, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveRight:
                        if (CanMoveInDirection(Direction.Right))
                        {
                            Move(Direction.Right, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveUpLeft:
                        if (CanMoveInDirection(Direction.UpLeft))
                        {
                            Move(Direction.UpLeft, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveUpRight:
                        if (CanMoveInDirection(Direction.UpRight))
                        {
                            Move(Direction.UpRight, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveDownRight:
                        if (CanMoveInDirection(Direction.DownRight))
                        {
                            Move(Direction.DownRight, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveDownLeft:
                        if (CanMoveInDirection(Direction.DownLeft))
                        {
                            Move(Direction.DownLeft, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveRandomly:
                        var dir = Randomization.NextDirection();
                        if (CanMoveInDirection(dir))
                        {
                            Move(dir, forPlayer);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.StepForward:
                        if (CanMoveInDirection(Dir))
                        {
                            Move(Dir, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.StepBack:
                        switch (Dir)
                        {
                            case Direction.Up:
                                moveDir = Direction.Down;

                                break;
                            case Direction.Down:
                                moveDir = Direction.Up;

                                break;
                            case Direction.Left:
                                moveDir = Direction.Right;

                                break;
                            case Direction.Right:
                                moveDir = Direction.Left;

                                break;
                            case Direction.UpLeft:
                                moveDir = Direction.DownRight;

                                break;
                            case Direction.UpRight:
                                moveDir = Direction.DownLeft;

                                break;
                            case Direction.DownRight:
                                moveDir = Direction.UpLeft;

                                break;
                            case Direction.DownLeft:
                                moveDir = Direction.UpRight;

                                break;
                        }

                        if (CanMoveInDirection(moveDir))
                        {
                            Move(moveDir, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.FaceUp:
                        ChangeDir(Direction.Up);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceDown:
                        ChangeDir(Direction.Down);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceLeft:
                        ChangeDir(Direction.Left);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceRight:
                        ChangeDir(Direction.Right);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn90Clockwise:
                        switch (Dir)
                        {
                            case Direction.Up:
                                lookDir = Direction.Right;

                                break;
                            case Direction.Down:
                                lookDir = Direction.Left;

                                break;
                            case Direction.Left:
                                lookDir = Direction.Up;

                                break;
                            case Direction.Right:
                                lookDir = Direction.Down;

                                break;
                            case Direction.UpLeft:
                                lookDir = Direction.UpRight;

                                break;
                            case Direction.UpRight:
                                lookDir = Direction.DownRight;

                                break;
                            case Direction.DownRight:
                                lookDir = Direction.DownLeft;

                                break;
                            case Direction.DownLeft:
                                lookDir = Direction.UpLeft;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn90CounterClockwise:
                        switch (Dir)
                        {
                            case Direction.Up:
                                lookDir = Direction.Left;

                                break;
                            case Direction.Down:
                                lookDir = Direction.Right;

                                break;
                            case Direction.Left:
                                lookDir = Direction.Down;

                                break;
                            case Direction.Right:
                                lookDir = Direction.Up;

                                break;
                            case Direction.UpLeft:
                                lookDir = Direction.DownLeft;

                                break;
                            case Direction.UpRight:
                                lookDir = Direction.UpLeft;

                                break;
                            case Direction.DownRight:
                                lookDir = Direction.UpRight;

                                break;
                            case Direction.DownLeft:
                                lookDir = Direction.DownRight;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn180:
                        switch (Dir)
                        {
                            case Direction.Up:
                                lookDir = Direction.Down;

                                break;
                            case Direction.Down:
                                lookDir = Direction.Up;

                                break;
                            case Direction.Left:
                                lookDir = Direction.Right;

                                break;
                            case Direction.Right:
                                lookDir = Direction.Left;

                                break;
                            case Direction.UpLeft:
                                lookDir = Direction.DownRight;

                                break;
                            case Direction.UpRight:
                                lookDir = Direction.DownLeft;

                                break;
                            case Direction.DownRight:
                                lookDir = Direction.UpLeft;

                                break;
                            case Direction.DownLeft:
                                lookDir = Direction.UpRight;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.TurnRandomly:
                        ChangeDir(Randomization.NextDirection());
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait100:
                        MoveTimer = Timing.Global.Milliseconds + 100;
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait500:
                        MoveTimer = Timing.Global.Milliseconds + 500;
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait1000:
                        MoveTimer = Timing.Global.Milliseconds + 1000;
                        moved = true;

                        break;
                    default:
                        //Gonna end up returning false because command not found
                        return false;
                }

                if (moved || MoveRoute.IgnoreIfBlocked)
                {
                    MoveRoute.ActionIndex++;
                    if (MoveRoute.ActionIndex >= MoveRoute.Actions.Count)
                    {
                        if (MoveRoute.RepeatRoute)
                        {
                            MoveRoute.ActionIndex = 0;
                        }

                        MoveRoute.Complete = true;
                    }
                }

                if (moved && MoveTimer < Timing.Global.Milliseconds)
                {
                    MoveTimer = Timing.Global.Milliseconds + (long)GetMovementTime();
                }
            }

            return true;
        }

        public virtual bool IsPassable()
        {
            return Passable;
        }

        //Returns the amount of time required to traverse 1 tile
        public virtual float GetMovementTime()
        {
            var time = 1000f / (float)(1 + Math.Log(Stat[(int)Enums.Stat.Speed].Value()));
            if (Dir > Direction.Right)
            {
                time *= MathHelper.UnitDiagonalLength;
            }

            if (IsBlocking)
            {
                time += time * Options.BlockingSlow;
            }

            return Math.Min(1000f, time);
        }

        public virtual EntityType GetEntityType()
        {
            return EntityType.GlobalEntity;
        }

        public virtual void Move(Direction moveDir, Player forPlayer, bool doNotUpdate = false,
            bool correction = false)
        {
            if (Timing.Global.Milliseconds < MoveTimer || (!Options.Combat.MovementCancelsCast && IsCasting))
            {
                return;
            }

            lock (EntityLock)
            {
                if (this is Player && IsCasting && Options.Combat.MovementCancelsCast)
                {
                    CastTime = 0;
                    CastTarget = null;
                }

                var xOffset = 0;
                var yOffset = 0;
                switch (moveDir)
                {
                    case Direction.Up:
                        --yOffset;

                        break;
                    case Direction.Down:
                        ++yOffset;

                        break;
                    case Direction.Left:
                        --xOffset;

                        break;
                    case Direction.Right:
                        ++xOffset;

                        break;
                    case Direction.UpLeft:
                        --yOffset;
                        --xOffset;

                        break;
                    case Direction.UpRight:
                        --yOffset;
                        ++xOffset;

                        break;
                    case Direction.DownRight:
                        ++yOffset;
                        ++xOffset;

                        break;
                    case Direction.DownLeft:
                        ++yOffset;
                        --xOffset;

                        break;

                    default:
                        Log.Warn(
                            new ArgumentOutOfRangeException(nameof(moveDir), $@"Bogus move attempt in direction {moveDir}.")
                        );

                        return;
                }

                Dir = moveDir;


                var tile = new TileHelper(MapId, X, Y);

                // ReSharper disable once InvertIf
                if (tile.Translate(xOffset, yOffset))
                {
                    X = tile.GetX();
                    Y = tile.GetY();

                    var currentMap = MapController.Get(tile.GetMapId());
                    if (MapId != tile.GetMapId())
                    {
                        var oldMap = MapController.Get(MapId);
                        if (oldMap.TryGetInstance(MapInstanceId, out var oldInstance))
                        {
                            oldInstance.RemoveEntity(this);
                        }

                        if (currentMap.TryGetInstance(MapInstanceId, out var newInstance))
                        {
                            newInstance.AddEntity(this);
                        }

                        //Send Left Map Packet To the Maps that we are no longer with
                        var oldMaps = oldMap?.GetSurroundingMaps(true);
                        var newMaps = currentMap?.GetSurroundingMaps(true);

                        MapId = tile.GetMapId();

                        if (oldMaps != null)
                        {
                            foreach (var map in oldMaps)
                            {
                                if (newMaps == null || !newMaps.Contains(map))
                                {
                                    PacketSender.SendEntityLeaveMap(this, map.Id);
                                }
                            }
                        }


                        if (newMaps != null)
                        {
                            foreach (var map in newMaps)
                            {
                                if (oldMaps == null || !oldMaps.Contains(map))
                                {
                                    PacketSender.SendEntityDataToMap(this, map, this as Player);
                                }
                            }
                        }

                    }



                    if (doNotUpdate == false)
                    {
                        if (this is EventPageInstance)
                        {
                            if (forPlayer != null)
                            {
                                PacketSender.SendEntityMoveTo(forPlayer, this, correction);
                            }
                            else
                            {
                                PacketSender.SendEntityMove(this, correction);
                            }
                        }
                        else
                        {
                            PacketSender.SendEntityMove(this, correction);
                        }

                        //Check if moving into a projectile.. if so this npc needs to be hit
                        if (currentMap != null)
                        {
                            foreach (var instance in MapController.GetSurroundingMapInstances(currentMap.Id, MapInstanceId, true))
                            {
                                var projectiles = instance.MapProjectilesCached;
                                foreach (var projectile in projectiles)
                                {
                                    var spawns = projectile?.Spawns?.ToArray() ?? Array.Empty<ProjectileSpawn>();
                                    foreach (var spawn in spawns)
                                    {
                                        // TODO: Filter in Spawns variable, there should be no nulls. See #78 for evidence it is null.
                                        if (spawn == null)
                                        {
                                            continue;
                                        }

                                        if (spawn.IsAtLocation(MapId, X, Y, Z) && spawn.HitEntity(this))
                                        {
                                            spawn.Dead = true;
                                        }
                                    }
                                }
                            }
                        }

                        MoveTimer = Timing.Global.Milliseconds + (long)GetMovementTime();
                    }

                    if (TryToChangeDimension() && doNotUpdate == true)
                    {
                        PacketSender.UpdateEntityZDimension(this, (byte)Z);
                    }

                    //Check for traps
                    if (MapController.TryGetInstanceFromMap(currentMap.Id, MapInstanceId, out var mapInstance))
                    {
                        foreach (var trap in mapInstance.MapTrapsCached)
                        {
                            trap.CheckEntityHasDetonatedTrap(this);
                        }
                    }

                    // TODO: Why was this scoped to only Event entities?
                    //                if (currentMap != null && this is EventPageInstance)
                    var attribute = currentMap?.Attributes[X, Y];

                    // ReSharper disable once InvertIf
                    //Check for slide tiles
                    if (attribute?.Type == MapAttribute.Slide)
                    {
                        // If sets direction, set it.
                        if (((MapSlideAttribute)attribute).Direction > 0)
                        {
                            //Check for slide tiles
                            if (attribute != null && attribute.Type == MapAttribute.Slide)
                            {
                                if (((MapSlideAttribute)attribute).Direction > 0)
                                {
                                    Dir = (Direction)(((MapSlideAttribute)attribute).Direction - 1);
                                }
                            }
                        }

                        var dash = new Dash(this, 1, Dir);
                    }
                }
            }
        }

        protected virtual bool CanLookInDirection(Direction direction) => true;

        public void ChangeDir(Direction dir)
        {
            if (!CanLookInDirection(dir))
            {
                return;
            }

            if (dir == Direction.None || Dir == dir)
            {
                return;
            }

            Dir = dir;

            if (this is EventPageInstance eventPageInstance && eventPageInstance.Player != null)
            {
                if (((EventPageInstance)this).Player != null)
                {
                    PacketSender.SendEntityDirTo(((EventPageInstance)this).Player, this);
                }
                else
                {
                    PacketSender.SendEntityDir(this);
                }
            }
            else
            {
                PacketSender.SendEntityDir(this);
            }
        }

        // Change the dimension if the player is on a gateway
        public bool TryToChangeDimension()
        {
            if (X < Options.MapWidth && X >= 0)
            {
                if (Y < Options.MapHeight && Y >= 0)
                {
                    var attribute = MapController.Get(MapId).Attributes[X, Y];
                    if (attribute != null && attribute.Type == MapAttribute.ZDimension)
                    {
                        if (((MapZDimensionAttribute)attribute).GatewayTo > 0)
                        {
                            Z = (byte)(((MapZDimensionAttribute)attribute).GatewayTo - 1);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //Misc
        public Direction GetDirectionTo(Entity target)
        {
            int xDiff = 0, yDiff = 0;

            var map = MapController.Get(MapId);
            var gridId = map.MapGrid;
            var grid = DbInterface.GetGrid(gridId);

            //Loop through surrouding maps to generate a array of open and blocked points.
            for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
            {
                if (x == -1 || x >= grid.Width)
                {
                    continue;
                }

                for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
                {
                    if (y == -1 || y >= grid.Height)
                    {
                        continue;
                    }

                    if (grid.MapIdGrid[x, y] != Guid.Empty &&
                        grid.MapIdGrid[x, y] == target.MapId)
                    {
                        xDiff = (x - map.MapGridX) * Options.MapWidth + target.X - X;
                        yDiff = (y - map.MapGridY) * Options.MapHeight + target.Y - Y;
                        if (Math.Abs(xDiff) > Math.Abs(yDiff))
                        {
                            if (xDiff < 0)
                            {
                                return Direction.Left;
                            }

                            if (xDiff > 0)
                            {
                                return Direction.Right;
                            }
                        }
                        else
                        {
                            if (yDiff < 0)
                            {
                                return Direction.Up;
                            }

                            if (yDiff > 0)
                            {
                                return Direction.Down;
                            }
                        }
                    }
                }
            }

            return default;
        }

        //Combat
        public virtual int CalculateAttackTime()
        {
            return (int)(Options.MaxAttackRate +
                          (Options.MinAttackRate - Options.MaxAttackRate) *
                          (((float)Options.MaxStatValue - Stat[(int)Enums.Stat.Speed].Value()) /
                           Options.MaxStatValue));
        }

        public void TryBlock(bool blocking)
        {
            if (IsAttacking)
            {
                return;
            }

            if (!blocking || IsBlocking)
            {
                return;
            }

            IsBlocking = true;
            AttackTimer = Timing.Global.Milliseconds + CalculateAttackTime();
            PacketSender.SendEntityAttack(this, CalculateAttackTime(), true);
        }

        public virtual int GetWeaponDamage()
        {
            return 0;
        }

        public virtual bool CanAttack(Entity entity, SpellBase spell) => !IsCasting;

        public virtual void ProcessRegen()
        {
        }

        public int GetVital(int vital)
        {
            return mVitals[vital];
        }

        public int[] GetVitals()
        {
            var vitals = new int[Enum.GetValues<Vital>().Length];
            Array.Copy(mVitals, 0, vitals, 0, Enum.GetValues<Vital>().Length);

            return vitals;
        }

        public int GetVital(Vital vital)
        {
            return GetVital((int)vital);
        }

        public void SetVital(int vital, int value)
        {
            if (value < 0)
            {
                value = 0;
            }

            if (GetMaxVital(vital) < value)
            {
                value = GetMaxVital(vital);
            }

            mVitals[vital] = value;
        }

        public void SetVital(Vital vital, int value)
        {
            SetVital((int)vital, value);
        }

        public virtual int GetMaxVital(int vital)
        {
            return _maxVital[vital];
        }

        public virtual int GetMaxVital(Vital vital)
        {
            return GetMaxVital((int)vital);
        }

        public int[] GetMaxVitals()
        {
            var vitals = new int[Enum.GetValues<Vital>().Length];
            for (var vitalIndex = 0; vitalIndex < vitals.Length; ++vitalIndex)
            {
                vitals[vitalIndex] = GetMaxVital(vitalIndex);
            }

            return vitals;
        }

        public void SetMaxVital(int vital, int value)
        {
            if (value <= 0 && vital == (int)Vital.Health)
            {
                value = 1; //Must have at least 1 hp
            }

            if (value < 0 && vital == (int)Vital.Mana)
            {
                value = 0; //Can't have less than 0 mana
            }

            _maxVital[vital] = value;
            if (value < GetVital(vital))
            {
                SetVital(vital, value);
            }
        }

        public void SetMaxVital(Vital vital, int value)
        {
            SetMaxVital((int)vital, value);
        }

        public bool HasVital(Vital vital)
        {
            return GetVital(vital) > 0;
        }

        public bool IsFullVital(Vital vital)
        {
            return GetVital(vital) == GetMaxVital(vital);
        }

        //Vitals
        public void RestoreVital(Vital vital)
        {
            SetVital(vital, GetMaxVital(vital));
        }

        public void AddVital(Vital vital, int amount)
        {
            if (!Enum.IsDefined(vital))
            {
                return;
            }

            var vitalId = (int)vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, int.MaxValue - maxVitalValue);
            SetVital(vital, GetVital(vital) + safeAmount);
        }

        public void SubVital(Vital vital, int amount)
        {
            if (!Enum.IsDefined(vital))
            {
                return;
            }

            //Check for any shields.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == SpellEffect.Shield)
                {
                    status.DamageShield(vital, ref amount);
                }
            }

            var vitalId = (int)vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, GetVital(vital));
            SetVital(vital, GetVital(vital) - safeAmount);
            ReactToDamage();
        }

        protected virtual void ReactToDamage()
        {
            return;
        }

        public virtual int[] GetStatValues()
        {
            var stats = new int[Enum.GetValues<Stat>().Length];
            for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
            {
                stats[i] = Stat[i].Value();
            }

            return stats;
        }

        public virtual bool IsAllyOf(Entity otherEntity)
        {
            return this == otherEntity;
        }

        //Attacking with projectile
        public virtual void TryAttack(Entity target,
            ProjectileBase projectile,
            SpellBase parentSpell,
            ItemBase parentItem,
            Direction projectileDir)
        {
            if (target is Resource && parentSpell != null)
            {
                return;
            }

            if (parentSpell != null)
            {
                TryAttack(target, parentSpell);
            }

            var targetPlayer = target as Player;

            if (this is Player player && targetPlayer != null)
            {
                //Player interaction common events
                if (projectile == null && parentSpell == null)
                {
                    targetPlayer.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerInteract, "", this.Name);
                }

                if (MapController.Get(MapId).ZoneType == MapZone.Safe)
                {
                    return;
                }

                if (MapController.Get(target.MapId).ZoneType == MapZone.Safe)
                {
                    return;
                }

                if (player.InParty(targetPlayer))
                {
                    return;
                }
            }

            if (parentSpell == null)
            {
                Attack(
                    target, parentItem.Damage, 0, (DamageType)parentItem.DamageType, (Stat)parentItem.ScalingStat,
                    parentItem.Scaling, parentItem.CritChance, parentItem.CritMultiplier, null, null, true
                );
            }

            //If projectile, check if a splash spell is applied
            if (projectile == null)
            {
                return;
            }

            if (projectile.SpellId != Guid.Empty)
            {
                var s = projectile.Spell;
                if (s != null)
                {
                    HandleAoESpell(projectile.SpellId, s.Combat.HitRadius, target.MapId, target.X, target.Y, null);
                }

                //Check that the npc has not been destroyed by the splash spell
                //TODO: Actually implement this, since null check is wrong.
                if (target == null)
                {
                    return;
                }
            }

            if (targetPlayer == null && !(target is Npc) || target.IsDead())
            {
                return;
            }

            // If there is knock-back: knock them backwards.
            if (projectile.Knockback > 0 && ((int)projectileDir < Options.Instance.MapOpts.MovementDirections) && !target.Immunities.Contains(SpellEffect.Knockback))
            {
                var dash = new Dash(target, projectile.Knockback, projectileDir, false, false, false, false);
            }
        }

        //Attacking with spell
        public virtual void TryAttack(
            Entity target,
            SpellBase spellBase,
            bool onHitTrigger = false,
            bool trapTrigger = false
        )
        {
            if (target is Resource || target is EventPageInstance)
            {
                return;
            }

            if (spellBase == null)
            {
                return;
            }

            var deadAnimations = new List<KeyValuePair<Guid, Direction>>();
            var aliveAnimations = new List<KeyValuePair<Guid, Direction>>();

            //Only count safe zones and friendly fire if its a dangerous spell! (If one has been used)
            if (!spellBase.Combat.Friendly &&
                (spellBase.Combat.TargetType != (int)SpellTargetType.Self || onHitTrigger))
            {
                //If about to hit self with an unfriendly spell (maybe aoe?) return
                if (target == this && spellBase.Combat.Effect != SpellEffect.OnHit)
                {
                    return;
                }

                //Check for parties and safe zones, friendly fire off (unless its healing)
                if (target is Npc && this is Npc npc)
                {
                    if (!npc.CanNpcCombat(target, spellBase.Combat.Friendly))
                    {
                        return;
                    }
                }

                if (target is Player targetPlayer && this is Player player)
                {
                    if (player.IsAllyOf(targetPlayer))
                    {
                        return;
                    }

                    // Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
                    if (MapController.Get(MapId).ZoneType == MapZone.Safe)
                    {
                        return;
                    }

                    if (MapController.Get(target.MapId).ZoneType == MapZone.Safe)
                    {
                        return;
                    }
                }

                if (!CanAttack(target, spellBase))
                {
                    return;
                }
            }
            else
            {
                // Friendly Spell! Do not attack other players/npcs around us.
                switch (target)
                {
                    case Player targetPlayer
                        when this is Player player && !IsAllyOf(targetPlayer) && this != target:
                    case Npc _ when this is Npc npc && !npc.CanNpcCombat(target, spellBase.Combat.Friendly):
                        return;
                }

                if (target?.GetType() != GetType())
                {
                    return; // Don't let players aoe heal npcs. Don't let npcs aoe heal players.
                }
            }

            if (spellBase.HitAnimationId != Guid.Empty &&
                (spellBase.Combat.Effect != SpellEffect.OnHit || onHitTrigger))
            {
                deadAnimations.Add(new KeyValuePair<Guid, Direction>(spellBase.HitAnimationId, Direction.Up));
                aliveAnimations.Add(new KeyValuePair<Guid, Direction>(spellBase.HitAnimationId, Direction.Up));
            }

            var statBuffTime = -1;
            var expireTime = Timing.Global.Milliseconds + spellBase.Combat.Duration;
            for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
            {
                target.Stat[i]
                    .AddBuff(
                        new Buff(spellBase, spellBase.Combat.StatDiff[i], spellBase.Combat.PercentageStatDiff[i], expireTime)
                    );

                if (spellBase.Combat.StatDiff[i] != 0 || spellBase.Combat.PercentageStatDiff[i] != 0)
                {
                    statBuffTime = spellBase.Combat.Duration;
                }
            }

            if (statBuffTime == -1)
            {
                if (spellBase.Combat.HoTDoT && spellBase.Combat.HotDotInterval > 0)
                {
                    statBuffTime = spellBase.Combat.Duration;
                }
            }

            var damageHealth = spellBase.Combat.VitalDiff[(int)Vital.Health];
            var damageMana = spellBase.Combat.VitalDiff[(int)Vital.Mana];

            if ((spellBase.Combat.Effect != SpellEffect.OnHit || onHitTrigger) &&
                spellBase.Combat.Effect != SpellEffect.Shield)
            {
                Attack(
                    target, damageHealth, damageMana, (DamageType)spellBase.Combat.DamageType,
                    (Stat)spellBase.Combat.ScalingStat, spellBase.Combat.Scaling, spellBase.Combat.CritChance,
                    spellBase.Combat.CritMultiplier, deadAnimations, aliveAnimations, false
                );
            }

            if (spellBase.Combat.Effect > 0) //Handle status effects
            {
                //Check for onhit effect to avoid the onhit effect recycling.
                if (!(onHitTrigger && spellBase.Combat.Effect == SpellEffect.OnHit))
                {
                    // If the entity is immune to some status, then just inform the client of such
                    if (target.Immunities.Contains(spellBase.Combat.Effect))
                    {
                        PacketSender.SendActionMsg(
                            target, Strings.Combat.ImmuneToEffect, CustomColors.Combat.Status
                        );
                    }
                    else
                    {
                        // Else, apply the status
                        new Status(
                            target, this, spellBase, spellBase.Combat.Effect, spellBase.Combat.Duration,
                            spellBase.Combat.TransformSprite
                        );

                        if (target is Npc npc)
                        {
                            npc.AssignTarget(this);
                        }

                        PacketSender.SendActionMsg(
                            target, Strings.Combat.status[(int)spellBase.Combat.Effect], CustomColors.Combat.Status
                        );

                        //If an onhit or shield status bail out as we don't want to do any damage.
                        if (spellBase.Combat.Effect == SpellEffect.OnHit || spellBase.Combat.Effect == SpellEffect.Shield)
                        {
                            Animate(target, aliveAnimations);

                            return;
                        }
                    }
                }
            }
            else
            {
                if (statBuffTime > -1)
                {
                    if (!target.Immunities.Contains(spellBase.Combat.Effect))
                    {
                        new Status(target, this, spellBase, spellBase.Combat.Effect, statBuffTime, "");

                        if (target is Npc npc)
                        {
                            npc.AssignTarget(this);
                        }
                    }
                    else
                    {
                        PacketSender.SendActionMsg(target, Strings.Combat.ImmuneToEffect, CustomColors.Combat.Status);
                    }
                }
            }

            //Handle DoT/HoT spells]
            if (spellBase.Combat.HoTDoT)
            {
                foreach (var dot in target.CachedDots)
                {
                    if (dot.SpellBase.Id == spellBase.Id && dot.Attacker == this)
                    {
                        dot.Expire();
                    }
                }

                new DoT(this, spellBase.Id, target);
            }
        }

        private void Animate(Entity target, List<KeyValuePair<Guid, Direction>> animations)
        {
            foreach (var anim in animations)
            {
                PacketSender.SendAnimationToProximity(anim.Key, 1, target.Id, target.MapId, 0, 0, anim.Value, MapInstanceId);
            }
        }

        //Attacking with weapon or unarmed.
        public virtual void TryAttack(Entity target)
        {
            //See player and npc override of this virtual void
        }

        //Attack using a weapon or unarmed
        public virtual void TryAttack(Entity target,
            int baseDamage,
            DamageType damageType,
            Stat scalingStat,
            int scaling,
            int critChance,
            double critMultiplier,
            List<KeyValuePair<Guid, Direction>> deadAnimations = null,
            List<KeyValuePair<Guid, Direction>> aliveAnimations = null,
            ItemBase weapon = null)
        {
            if (IsAttacking)
            {
                return;
            }

            //Check for parties and safe zones, friendly fire off (unless its healing)
            if (target is Player targetPlayer && this is Player player)
            {
                if (player.InParty(targetPlayer))
                {
                    return;
                }

                //Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
                //Player interaction common events
                targetPlayer.StartCommonEventsWithTrigger(CommonEventTrigger.PlayerInteract, "", this.Name);

                if (MapController.Get(MapId)?.ZoneType == MapZone.Safe)
                {
                    return;
                }

                if (MapController.Get(target.MapId)?.ZoneType == MapZone.Safe)
                {
                    return;
                }
            }

            //Check for taunt status and trying to attack a target that has not taunted you.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == SpellEffect.Taunt)
                {
                    if (Target != target)
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Combat.Missed);

                        return;
                    }
                }
            }

            AttackTimer = Timing.Global.Milliseconds + CalculateAttackTime();

            //Check if the attacker is blinded.
            if (IsOneBlockAway(target))
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == SpellEffect.Stun ||
                        status.Type == SpellEffect.Blind ||
                        status.Type == SpellEffect.Sleep)
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Combat.Missed);
                        PacketSender.SendEntityAttack(this, CalculateAttackTime());

                        return;
                    }
                }
            }

            Attack(
                target, baseDamage, 0, damageType, scalingStat, scaling, critChance, critMultiplier, deadAnimations,
                aliveAnimations, true
            );
        }

        public void Attack(
            Entity enemy,
            int baseDamage,
            int secondaryDamage,
            DamageType damageType,
            Stat scalingStat,
            int scaling,
            int critChance,
            double critMultiplier,
            List<KeyValuePair<Guid, Direction>> deadAnimations = null,
            List<KeyValuePair<Guid, Direction>> aliveAnimations = null,
            bool isAutoAttack = false
        )
        {
            var damagingAttack = baseDamage > 0;
            var secondaryDamagingAttack = secondaryDamage > 0;

            if (enemy == null)
            {
                return;
            }

            //Let's save the entity's vitals before they takes damage to use in lifesteal/manasteal
            var enemyVitals = enemy.GetVitals();
            var invulnerable = enemy.CachedStatuses.Any(status => status.Type == SpellEffect.Invulnerable);

            bool isCrit = false;
            //Is this a critical hit?
            if (Randomization.Next(1, 101) > critChance)
            {
                critMultiplier = 1;
            }
            else
            {
                isCrit = true;
            }

            //If the enemy is a resource, the original base damage value will be used on "Calculate Damages", if not, we need change...
            if (!(enemy is Resource))
            {
                baseDamage = Formulas.CalculateDamage(
                baseDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy
            );
            }

            //Check on each attack if the enemy is a player AND if they are blocking.
            if (enemy is Player player && player.IsBlocking)
            {
                if (player.TryGetEquipmentSlot(Options.ShieldIndex, out var slot) && player.TryGetItemAt(slot, out var itm))
                {
                    var item = itm.Descriptor;
                    var originalBaseDamage = baseDamage;
                    var blockChance = item.BlockChance;
                    var blockAmount = item.BlockAmount / 100.0;
                    var blockAbsorption = item.BlockAbsorption / 100.0;

                    //Generate a new attempt to block
                    if (Randomization.Next(0, 101) < blockChance)
                    {
                        if (item.BlockAmount < 100)
                        {
                            baseDamage -= (int)Math.Round(baseDamage * blockAmount);
                        }
                        else
                        {
                            baseDamage = 0;
                        }

                        var absorptionAmount = (int)Math.Round(baseDamage * blockAbsorption);

                        if (absorptionAmount == 0)
                        {
                            absorptionAmount = (int)Math.Round(originalBaseDamage * blockAbsorption);
                        }

                        if (blockAbsorption > 0)
                        {
                            player.AddVital(Vital.Health, absorptionAmount);

                            PacketSender.SendActionMsg(
                            enemy, Strings.Combat.addsymbol + Math.Abs(absorptionAmount),
                            CustomColors.Combat.Heal
                            );
                        }

                        PacketSender.SendActionMsg(enemy, Strings.Combat.blocked, CustomColors.Combat.Blocked);
                    }
                }
            }

            //Calculate Damages
            if (baseDamage != 0)
            {

                if (baseDamage < 0 && damagingAttack)
                {
                    baseDamage = 0;
                }

                if (baseDamage > 0 && enemy.HasVital(Vital.Health) && !invulnerable)
                {
                    if (isCrit)
                    {
                        PacketSender.SendActionMsg(enemy, Strings.Combat.critical, CustomColors.Combat.Critical);
                    }

                    enemy.SubVital(Vital.Health, baseDamage);
                    switch (damageType)
                    {
                        case DamageType.Physical:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + baseDamage,
                                CustomColors.Combat.PhysicalDamage
                            );

                            break;
                        case DamageType.Magic:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + baseDamage, CustomColors.Combat.MagicDamage
                            );

                            break;
                        case DamageType.True:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + baseDamage, CustomColors.Combat.TrueDamage
                            );

                            break;
                    }

                    var toRemove = new List<Status>();
                    foreach (var status in enemy.CachedStatuses.ToArray())  // ToArray the Array since removing a status will.. you know, change the collection.
                    {
                        //Wake up any sleeping targets targets and take stealthed entities out of stealth
                        if (status.Type == SpellEffect.Sleep || status.Type == SpellEffect.Stealth)
                        {
                            status.RemoveStatus();
                        }
                    }

                    // Add the attacker to the Npcs threat and loot table.
                    if (enemy is Npc enemyNpc)
                    {
                        var dmgMap = enemyNpc.DamageMap;
                        dmgMap.TryGetValue(this, out var damage);
                        dmgMap[this] = damage + baseDamage;

                        enemyNpc.LootMap.TryAdd(Id, true);
                        enemyNpc.LootMapCache = enemyNpc.LootMap.Keys.ToArray();
                        enemyNpc.TryFindNewTarget(Timing.Global.Milliseconds, default, false, this);
                    }

                    enemy.NotifySwarm(this);
                }
                else if (baseDamage < 0 && !enemy.IsFullVital(Vital.Health))
                {
                    enemy.AddVital(Vital.Health, -baseDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.addsymbol + Math.Abs(baseDamage), CustomColors.Combat.Heal
                    );
                }
            }

            if (secondaryDamage != 0)
            {
                secondaryDamage = Formulas.CalculateDamage(
                    secondaryDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy
                );

                if (secondaryDamage < 0 && secondaryDamagingAttack)
                {
                    secondaryDamage = 0;
                }

                if (secondaryDamage > 0 && enemy.HasVital(Vital.Mana) && !invulnerable)
                {
                    //If we took damage lets reset our combat timer
                    enemy.SubVital(Vital.Mana, secondaryDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.removesymbol + secondaryDamage, CustomColors.Combat.RemoveMana
                    );

                    //No Matter what, if we attack the entitiy, make them chase us
                    if (enemy is Npc enemyNpc)
                    {
                        enemyNpc.TryFindNewTarget(Timing.Global.Milliseconds, default, false, this);
                    }

                    enemy.NotifySwarm(this);
                }
                else if (secondaryDamage < 0 && !enemy.IsFullVital(Vital.Mana))
                {
                    enemy.AddVital(Vital.Mana, -secondaryDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.addsymbol + Math.Abs(secondaryDamage), CustomColors.Combat.AddMana
                    );
                }
            }

            // Set combat timers!
            enemy.CombatTimer = Timing.Global.Milliseconds + Options.CombatTime;
            CombatTimer = Timing.Global.Milliseconds + Options.CombatTime;

            var thisPlayer = this as Player;

            //Check for lifesteal/manasteal
            if (this is Player && !(enemy is Resource))
            {
                var lifestealRate = thisPlayer.GetEquipmentBonusEffect(ItemEffect.Lifesteal) / 100f;
                var idealHealthRecovered = lifestealRate * baseDamage;
                var actualHealthRecovered = Math.Min(enemyVitals[(int)Vital.Health], idealHealthRecovered);

                if (actualHealthRecovered > 0)
                {
                    // Don't send any +0 msg's.
                    AddVital(Vital.Health, (int)actualHealthRecovered);
                    PacketSender.SendActionMsg(
                        this,
                        Strings.Combat.addsymbol + (int)actualHealthRecovered,
                        CustomColors.Combat.Heal
                    );
                }

                var manastealRate = (thisPlayer.GetEquipmentBonusEffect(ItemEffect.Manasteal) / 100f);
                var idealManaRecovered = manastealRate * baseDamage;
                var actualManaRecovered = Math.Min(enemyVitals[(int)Vital.Mana], idealManaRecovered);

                if (actualManaRecovered > 0)
                {
                    // Don't send any +0 msg's.
                    AddVital(Vital.Mana, (int)actualManaRecovered);
                    enemy.SubVital(Vital.Mana, (int)actualManaRecovered);
                    PacketSender.SendActionMsg(
                        this,
                        Strings.Combat.addsymbol + (int)actualManaRecovered,
                        CustomColors.Combat.AddMana
                    );
                }

                var remainingManaRecovery = idealManaRecovered - actualManaRecovered;
                if (remainingManaRecovery > 0)
                {
                    // If the mana recovered is less than it should be, deal the remainder as bonus damage
                    enemy.SubVital(Vital.Health, (int)remainingManaRecovery);
                    PacketSender.SendActionMsg(
                        enemy,
                        Strings.Combat.removesymbol + remainingManaRecovery,
                        CustomColors.Combat.TrueDamage
                    );
                }
            }

            //Dead entity check
            if (enemy.GetVital(Vital.Health) <= 0)
            {
                if (enemy is Npc || enemy is Resource)
                {
                    lock (enemy.EntityLock)
                    {
                        enemy.Die(true, this);
                    }
                }
                else
                {
                    //PVP Kill common events
                    if (!enemy.Dead && enemy is Player enemyPlayer && this is Player)
                    {
                        thisPlayer.StartCommonEventsWithTrigger(CommonEventTrigger.PVPKill, "", enemy.Name);
                        enemyPlayer.StartCommonEventsWithTrigger(CommonEventTrigger.PVPDeath, "", this.Name);
                    }

                    lock (enemy.EntityLock)
                    {
                        enemy.Die(true, this);
                    }
                }

                if (deadAnimations != null)
                {
                    foreach (var anim in deadAnimations)
                    {
                        PacketSender.SendAnimationToProximity(
                            anim.Key, -1, Id, enemy.MapId, enemy.X, enemy.Y, anim.Value, MapInstanceId
                        );
                    }
                }
            }
            else
            {
                //Hit him, make him mad and send the vital update.
                if (aliveAnimations?.Count > 0)
                {
                    Animate(enemy, aliveAnimations);
                }
            }

            //Check for any onhit damage bonus effects!
            CheckForOnhitAttack(enemy, isAutoAttack);

            // Add a timer before able to make the next move.
            if (this is Npc thisNpc)
            {
                thisNpc.MoveTimer = Timing.Global.Milliseconds + (long)GetMovementTime();
            }
        }

        protected virtual void CheckForOnhitAttack(Entity enemy, bool isAutoAttack)
        {
            if (isAutoAttack && !enemy.IsDead()) //Ignore spell damage.
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == SpellEffect.OnHit)
                    {
                        TryAttack(enemy, status.Spell, true);
                        status.RemoveStatus();
                    }
                }
            }
        }

        public virtual void KilledEntity(Entity entity)
        {
        }

        public virtual bool CanCastSpell(SpellBase spell, Entity target, bool checkVitalReqs, out SpellCastFailureReason reason)
        {
            // Is this a valid spell?
            if (spell == null)
            {
                reason = SpellCastFailureReason.InvalidSpell;
                return false;
            }

            // Is this spell on cooldown?
            if (SpellCooldowns.TryGetValue(spell.Id, out var spellCooldown) &&
                spellCooldown > Timing.Global.MillisecondsUtc)
            {
                reason = SpellCastFailureReason.OnCooldown;
                return false;
            }

            // Do we meet the vital requirements?
            if (checkVitalReqs)
            {
                if (spell.VitalCost[(int)Vital.Mana] > GetVital(Vital.Mana))
                {
                    reason = SpellCastFailureReason.InsufficientMP;
                    return false;
                }

                if (spell.VitalCost[(int)Vital.Health] >= GetVital(Vital.Health))
                {
                    reason = SpellCastFailureReason.InsufficientHP;
                    return false;
                }
            }

            // Check if the caster has any status effects that need to apply.
            // Ignore if the current spell is a Cleanse, this will ignore any and all status effects.
            if (spell.Combat.Effect != SpellEffect.Cleanse)
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == SpellEffect.Silence)
                    {
                        reason = SpellCastFailureReason.Silenced;
                        return false;
                    }

                    if (status.Type == SpellEffect.Stun)
                    {
                        reason = SpellCastFailureReason.Stunned;
                        return false;
                    }

                    if (status.Type == SpellEffect.Sleep)
                    {
                        reason = SpellCastFailureReason.Asleep;
                        return false;
                    }

                    if (status.Type == SpellEffect.Snare)
                    {
                        // If this spell is a Dash or Warp type ability, we can not use it while snared.
                        if (spell.SpellType == SpellType.Dash || spell.SpellType == SpellType.Warp || spell.SpellType == SpellType.WarpTo)
                        {
                            reason = SpellCastFailureReason.Snared;
                            return false;
                        }
                    }
                }
            }

            // Check for target validity
            var singleTargetSpell = (spell.SpellType == SpellType.CombatSpell && spell.Combat.TargetType == SpellTargetType.Single) || spell.SpellType == SpellType.WarpTo;
            if (target == null && singleTargetSpell)
            {
                reason = SpellCastFailureReason.InvalidTarget;
                return false;
            }

            if (target == this && spell.SpellType == SpellType.WarpTo)
            {
                reason = SpellCastFailureReason.InvalidTarget;
                return false;
            }

            if (target != null && singleTargetSpell)
            {
                if (spell.Combat.Friendly != IsAllyOf(target) || !CanAttack(target, spell))
                {
                    reason = SpellCastFailureReason.InvalidTarget;
                    return false;
                }
            }

            //Check for range of a single target spell
            if (singleTargetSpell && target != this)
            {
                if (!InRangeOf(target, spell.Combat.CastRange))
                {
                    reason = SpellCastFailureReason.OutOfRange;
                    return false;
                }
            }

            // We have no reason to stop the entity from casting!
            reason = SpellCastFailureReason.None;
            return true;
        }

        public virtual void CastSpell(Guid spellId, int spellSlot = -1)
        {
            if (!SpellBase.TryGet(spellId, out var spellBase))
            {
                return;
            }

            if (!CanCastSpell(spellBase, CastTarget, false, out _))
            {
                return;
            }

            if (spellBase.VitalCost[(int)Vital.Mana] > 0)
            {
                SubVital(Vital.Mana, spellBase.VitalCost[(int)Vital.Mana]);
            }
            else
            {
                AddVital(Vital.Mana, -spellBase.VitalCost[(int)Vital.Mana]);
            }

            if (spellBase.VitalCost[(int)Vital.Health] > 0)
            {
                SubVital(Vital.Health, spellBase.VitalCost[(int)Vital.Health]);
            }
            else
            {
                AddVital(Vital.Health, -spellBase.VitalCost[(int)Vital.Health]);
            }

            try
            {
                switch (spellBase.SpellType)
                {
                    case SpellType.CombatSpell:
                    case SpellType.Event:

                        switch (spellBase.Combat.TargetType)
                        {
                            case SpellTargetType.Self:
                                if (spellBase.HitAnimationId != Guid.Empty &&
                                    spellBase.Combat.Effect != SpellEffect.OnHit)
                                {
                                    PacketSender.SendAnimationToProximity(
                                        spellBase.HitAnimationId,
                                        1,
                                        Id,
                                        MapId,
                                        0,
                                        0,
                                        Dir,
                                        MapInstanceId
                                    ); //Target Type 1 will be global entity
                                }

                                TryAttack(this, spellBase);

                                break;
                            case SpellTargetType.Single:
                                if (CastTarget == null)
                                {
                                    return;
                                }

                                //If target has stealthed we cannot hit the spell.
                                foreach (var status in CastTarget.CachedStatuses)
                                {
                                    if (status.Type == SpellEffect.Stealth)
                                    {
                                        return;
                                    }
                                }

                                if (spellBase.Combat.HitRadius > 0) //Single target spells with AoE hit radius'
                                {
                                    HandleAoESpell(
                                        spellId,
                                        spellBase.Combat.HitRadius,
                                        CastTarget.MapId,
                                        CastTarget.X,
                                        CastTarget.Y,
                                        null
                                    );
                                }
                                else
                                {
                                    TryAttack(CastTarget, spellBase);
                                }

                                break;
                            case SpellTargetType.AoE:
                                HandleAoESpell(spellId, spellBase.Combat.HitRadius, MapId, X, Y, null);

                                break;
                            case SpellTargetType.Projectile:
                                var projectileBase = spellBase.Combat.Projectile;
                                if (projectileBase != null)
                                {
                                    if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var mapInstance))
                                    {
                                        mapInstance.SpawnMapProjectile(
                                            this,
                                            projectileBase,
                                            spellBase,
                                            null,
                                            MapId,
                                            (byte)X,
                                            (byte)Y,
                                            (byte)Z,
                                            Dir,
                                            CastTarget
                                        );
                                    }
                                }

                                break;
                            case SpellTargetType.OnHit:
                                if (spellBase.Combat.Effect == SpellEffect.OnHit)
                                {
                                    new Status(
                                        this,
                                        this,
                                        spellBase,
                                        SpellEffect.OnHit,
                                        spellBase.Combat.OnHitDuration,
                                        spellBase.Combat.TransformSprite
                                    );

                                    PacketSender.SendActionMsg(
                                        this,
                                        Strings.Combat.status[(int)spellBase.Combat.Effect],
                                        CustomColors.Combat.Status
                                    );
                                }

                                break;
                            case SpellTargetType.Trap:
                                if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
                                {
                                    instance.SpawnTrap(this, spellBase, (byte)X, (byte)Y, (byte)Z);
                                }

                                break;
                            default:
                                break;
                        }

                        break;
                    case SpellType.Warp:
                        if (this is Player)
                        {
                            Warp(
                                spellBase.Warp.MapId,
                                spellBase.Warp.X,
                                spellBase.Warp.Y,
                                spellBase.Warp.Dir - 1 == -1 ? this.Dir : (Direction)(spellBase.Warp.Dir - 1)
                            );
                        }

                        break;
                    case SpellType.WarpTo:
                        if (CastTarget != null)
                        {
                            HandleAoESpell(spellId, spellBase.Combat.CastRange, MapId, X, Y, CastTarget);
                        }

                        break;
                    case SpellType.Dash:
                        PacketSender.SendActionMsg(this, Strings.Combat.dash, CustomColors.Combat.Dash);
                        var dash = new Dash(
                            this,
                            spellBase.Combat.CastRange,
                            Dir,
                            Convert.ToBoolean(spellBase.Dash.IgnoreMapBlocks),
                            Convert.ToBoolean(spellBase.Dash.IgnoreActiveResources),
                            Convert.ToBoolean(spellBase.Dash.IgnoreInactiveResources),
                            Convert.ToBoolean(spellBase.Dash.IgnoreZDimensionAttributes)
                        );

                        break;
                    default:
                        break;
                }

                UpdateSpellCooldown(spellBase, spellSlot);
            }
            finally
            {
                if (GetVital(Vital.Health) < 1)
                {
                    Die(true, this);
                }
            }
        }

        private void HandleAoESpell(
            Guid spellId,
            int range,
            Guid startMapId,
            int startX,
            int startY,
            Entity spellTarget
        )
        {
            var spellBase = SpellBase.Get(spellId);
            if (spellBase != null)
            {
                var startMap = MapController.Get(startMapId);
                foreach (var instance in MapController.GetSurroundingMapInstances(startMapId, MapInstanceId, true))
                {
                    foreach (var entity in instance.GetCachedEntities())
                    {
                        if (entity != null && (entity is Player || entity is Npc))
                        {
                            if (spellTarget == null || spellTarget == entity)
                            {
                                if (entity.GetDistanceTo(startMap, startX, startY) <= range)
                                {
                                    //Check to handle a warp to spell
                                    if (spellBase.SpellType == SpellType.WarpTo)
                                    {
                                        if (spellTarget != null)
                                        {
                                            //Spelltarget used to be Target. I don't know if this is correct or not.
                                            int[] position = GetPositionNearTarget(spellTarget.MapId, spellTarget.X, spellTarget.Y);
                                            Warp(spellTarget.MapId, (byte)position[0], (byte)position[1], Dir);
                                            ChangeDir(DirectionToTarget(spellTarget));
                                        }
                                    }

                                    TryAttack(entity, spellBase); //Handle damage
                                }
                            }
                        }
                    }
                }
            }
        }

        private int[] GetPositionNearTarget(Guid mapId, int x, int y)
        {
            if (MapController.TryGetInstanceFromMap(mapId, MapInstanceId, out var instance))
            {
                List<int[]> validPosition = new List<int[]>();

                // Start by north, west, est and south
                for (int col = -1; col < 2; col++)
                {
                    for (int row = -1; row < 2; row++)
                    {
                        if (Math.Abs(col % 2) != Math.Abs(row % 2))
                        {
                            int newX = x + row;
                            int newY = y + col;

                            if (newX >= 0 && newX <= Options.MapWidth &&
                                newY >= 0 && newY <= Options.MapHeight &&
                                !instance.TileBlocked(newX, newY))
                            {
                                validPosition.Add(new int[] { newX, newY });
                            }
                        }
                    }
                }

                if (validPosition.Count > 0)
                {
                    return validPosition[Randomization.Next(0, validPosition.Count)];
                }

                // If nothing found, diagonal direction
                for (int col = -1; col < 2; col++)
                {
                    for (int row = -1; row < 2; row++)
                    {
                        if (Math.Abs(col % 2) == Math.Abs(row % 2))
                        {
                            int newX = x + row;
                            int newY = y + col;

                            // Tile must not be the target position
                            if (newX >= 0 && newX <= Options.MapWidth &&
                                newY >= 0 && newY <= Options.MapHeight &&
                                !(x + row == x && y + col == y) &&
                                !instance.TileBlocked(newX, newY))
                            {
                                validPosition.Add(new int[] { newX, newY });
                            }
                        }
                    }
                }

                if (validPosition.Count > 0)
                {
                    return validPosition[Randomization.Next(0, validPosition.Count)];
                }

                // If nothing found, return target position
                return new int[] { x, y };
            }
            else
            {
                return new int[] { x, y };
            }
        }

        // Check if the target is one tile away and within the same Z dimension.
        protected bool IsOneBlockAway(Entity target)
        {
            if (Z != target.Z)
            {
                return false;
            }

            var myTile = new TileHelper(MapId, X, Y);
            var enemyTile = new TileHelper(target.MapId, target.X, target.Y);

            myTile.Translate(0, -1); // Target Up
            if (myTile.Matches(enemyTile))
            {
                return true;
            }

            myTile.Translate(0, 2); // Target Down
            if (myTile.Matches(enemyTile))
            {
                return true;
            }

            myTile.Translate(-1, -1); // Target Left
            if (myTile.Matches(enemyTile))
            {
                return true;
            }

            myTile.Translate(2, 0); // Target Right
            if (myTile.Matches(enemyTile))
            {
                return true;
            }

            if (Options.Instance.MapOpts.EnableDiagonalMovement)
            {
                myTile.Translate(-2, -1); // Target UpLeft
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(2, 0); // Target UpRight
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(-2, 2); // Target DownLeft
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(2, 0); // Target DownRight
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }
            }

            return false;
        }

        //These functions only work when one block away.
        protected bool IsFacingTarget(Entity target)
        {
            if (!IsOneBlockAway(target.MapId, target.X, target.Y))
            {
                return false;
            }

            if (!MapController.TryGet(MapId, out var originMapController) ||
                !MapController.TryGet(target.MapId, out var targetMapController))
            {
                return false;
            }

            var originY = Y + originMapController.MapGridY * Options.MapHeight;
            var originX = X + originMapController.MapGridX * Options.MapWidth;
            var targetY = target.Y + targetMapController.MapGridY * Options.MapHeight;
            var targetX = target.X + targetMapController.MapGridX * Options.MapWidth;

            var xDiff = targetX - originX;
            var yDiff = targetY - originY;
            var diagonalMovement = Options.Instance.MapOpts.EnableDiagonalMovement;

            switch (xDiff)
            {
                case 0 when yDiff == -1 && Dir == Direction.Up:
                case 0 when yDiff == 1 && Dir == Direction.Down:
                case 1 when yDiff == 0 && Dir == Direction.Right:
                case -1 when yDiff == 0 && Dir == Direction.Left:
                case 1 when diagonalMovement && yDiff == -1 && Dir == Direction.UpRight:
                case -1 when diagonalMovement && yDiff == -1 && Dir == Direction.UpLeft:
                case 1 when diagonalMovement && yDiff == 1 && Dir == Direction.DownRight:
                case -1 when diagonalMovement && yDiff == 1 && Dir == Direction.DownLeft:
                    return true;
                default:
                    return false;
            }
        }

        public int GetDistanceTo(Entity target)
        {
            if (target != null)
            {
                return GetDistanceTo(target.Map, target.X, target.Y);
            }
            //Something is null.. return a value that is out of range :)
            return 9999;
        }

        public int GetDistanceTo(MapController targetMap, int targetX, int targetY)
        {
            return GetDistanceBetween(Map, targetMap, X, targetX, Y, targetY);
        }

        public int GetDistanceBetween(MapController mapA, MapController mapB, int xTileA, int xTileB, int yTileA, int yTileB)
        {
            if (mapA != null && mapB != null && mapA.MapGrid == mapB.MapGrid
            ) //Make sure both maps exist and that they are in the same dimension
            {
                //Calculate World Tile of Me
                var x1 = xTileA + mapA.MapGridX * Options.MapWidth;
                var y1 = yTileA + mapA.MapGridY * Options.MapHeight;

                //Calculate world tile of target
                var x2 = xTileB + mapB.MapGridX * Options.MapWidth;
                var y2 = yTileB + mapB.MapGridY * Options.MapHeight;

                return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            //Something is null.. return a value that is out of range :)
            return 9999;
        }

        public bool InRangeOf(Entity target, int range)
        {
            var dist = GetDistanceTo(target);
            if (dist == 9999)
            {
                return false;
            }

            if (dist <= range)
            {
                return true;
            }

            return false;
        }

        public virtual void NotifySwarm(Entity attacker)
        {
        }

        protected Direction DirectionToTarget(Entity en)
        {
            if (en == null || IsTurnAroundWhileCastingDisabled)
            {
                return Dir;
            }

            if (!MapController.TryGet(MapId, out var originMapController) ||
                !MapController.TryGet(en.MapId, out var targetMapController))
            {
                return Dir;
            }

            var originY = Y + originMapController.MapGridY * Options.MapHeight;
            var originX = X + originMapController.MapGridX * Options.MapWidth;
            var targetY = en.Y + targetMapController.MapGridY * Options.MapHeight;
            var targetX = en.X + targetMapController.MapGridX * Options.MapWidth;

            var yDiff = originY - targetY;
            var xDiff = originX - targetX;

            // If Y offset is 0, direction is determined by X offset.
            if (yDiff == 0)
            {
                return xDiff > 0 ? Direction.Left : Direction.Right;
            }

            // If X offset is 0 or If diagonal movement is disabled, direction is determined by Y offset.
            if (xDiff == 0 || !Options.Instance.MapOpts.EnableDiagonalMovement)
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

        protected bool IsOneBlockAway(Guid mapId, int x, int y, int z = 0)
        {
            if (z != Z)
            {
                return false;
            }

            if (!MapController.TryGet(MapId, out var originMapController) ||
                !MapController.TryGet(mapId, out var targetMapController))
            {
                return false;
            }

            // Adjust coordinates based on map grid positions.
            var originY = Y + originMapController.MapGridY * Options.MapHeight;
            var originX = X + originMapController.MapGridX * Options.MapWidth;
            var targetY = y + targetMapController.MapGridY * Options.MapHeight;
            var targetX = x + targetMapController.MapGridX * Options.MapWidth;

            // Calculate the absolute differences in coordinates.
            var yDiff = Math.Abs(originY - targetY);
            var xDiff = Math.Abs(originX - targetX);

            // Check if entities are one block away.
            return (xDiff == 1 && yDiff == 0) || (xDiff == 0 && yDiff == 1) ||
                   (Options.Instance.MapOpts.EnableDiagonalMovement && xDiff == 1 && yDiff == 1);
        }

        //Spawning/Dying
        public virtual void Die(bool dropItems = true, Entity killer = null)
        {
            if (IsDead() || Items == null)
            {
                return;
            }

            // Run events and other things.
            killer?.KilledEntity(this);

            if (dropItems)
            {
                var lootGenerated = new List<Player>();
                // If this is an NPC, drop loot for every single player that participated in the fight.
                if (this is Npc npc && npc.Base.IndividualizedLoot)
                {
                    // Generate loot for every player that has helped damage this monster, as well as their party members.
                    // Keep track of who already got loot generated for them though, or this gets messy!
                    foreach (var entityEntry in npc.LootMapCache)
                    {
                        var player = Player.FindOnline(entityEntry);
                        if (player != null)
                        {

                            // Cache our value for further processing.
                            var party = player.Party.ToArray();

                            // is this player in a party?
                            if (party.Length > 0 && Options.Instance.LootOpts.IndividualizedLootAutoIncludePartyMembers)
                            {
                                // They are, so check for all party members and drop if still eligible!
                                foreach (var partyMember in party)
                                {
                                    if (!lootGenerated.Contains(partyMember))
                                    {
                                        DropItems(partyMember);
                                        lootGenerated.Add(partyMember);
                                    }
                                }
                            }
                            else
                            {
                                // They're not in a party, so drop the item if still eligible!
                                if (!lootGenerated.Contains(player))
                                {
                                    DropItems(player);
                                    lootGenerated.Add(player);
                                }
                            }
                        }
                    }

                    // Clear their loot table and threat table.
                    npc.DamageMap.Clear();
                    npc.LootMap.Clear();
                    npc.LootMapCache = Array.Empty<Guid>();
                }
                else
                {
                    // Drop as normal.
                    DropItems(killer);
                }
            }

            foreach (var instance in MapController.GetSurroundingMapInstances(MapId, MapInstanceId, true))
            {
                instance.ClearEntityTargetsOf(this);
            }

            DoT?.Clear();
            CachedDots = new DoT[0];
            Statuses?.Clear();
            CachedStatuses = new Status[0];
            Stat?.ToList().ForEach(stat => stat?.Reset());

            Dead = true;
        }

        protected virtual bool ShouldDropItem(Entity killer, ItemBase itemDescriptor, Item item, float dropRateModifier, out Guid lootOwner)
        {
            lootOwner = default;

            var dropRate = item.DropChance * 1000 * dropRateModifier;
            var dropResult = Randomization.Next(1, 100001);
            if (dropResult >= dropRate)
            {
                return false;
            }

            // Set the attributes for this item.
            item.Set(new Item(item.ItemId, item.Quantity, null));
            return true;
        }

        protected virtual void OnDropItem(InventorySlot slot, Item drop) { }

        protected virtual void DropItems(Entity killer, bool sendUpdate = true)
        {
            if (this is Player && Options.Instance.MapOpts.DisablePlayerDropsInArenaMaps && Map.ZoneType == MapZone.Arena)
            {
                return;
            }

            // Drop items
            foreach (var slot in Items)
            {
                if (slot == default)
                {
                    continue;
                }

                // Don't mess with the actual object.
                var drop = slot.Clone();

                var itemDescriptor = ItemBase.Get(drop.ItemId);
                if (itemDescriptor == default)
                {
                    continue;
                }

                var playerKiller = killer as Player;
                var dropRateModifier = 1 + (playerKiller?.GetEquipmentBonusEffect(ItemEffect.Luck) / 100f ?? 0);
                if (!ShouldDropItem(killer, itemDescriptor, drop, dropRateModifier, out Guid lootOwner))
                {
                    continue;
                }

                // Spawn the actual item!
                if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
                {
                    instance.SpawnItem(X, Y, drop, drop.Quantity, lootOwner, sendUpdate);
                }

                // Process the drop (for players this would remove it from their inventory)
                OnDropItem(slot, drop);
            }
        }

        public bool IsDead()
        {
            return Dead;
        }

        public virtual void Reset()
        {
            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                RestoreVital((Vital)i);
            }

            Dead = false;
        }

        //Empty virtual functions for players
        public virtual void Warp(Guid newMapId, float newX, float newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, Dir, adminWarp);
        }

        public virtual void Warp(Guid newMapId,
            float newX,
            float newY,
            Direction newDir,
            bool adminWarp = false,
            int zOverride = 0,
            bool mapSave = false,
            bool fromWarpEvent = false,
            MapInstanceType? mapInstanceType = null,
            bool fromLogin = false,
            bool forceInstanceChange = false)
        {
        }

        public virtual EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                throw new Exception("No packet to populate!");
            }

            packet.EntityId = Id;
            packet.MapId = MapId;
            packet.Name = Name;
            packet.Sprite = Sprite;
            packet.Color = Color;
            packet.Face = Face;
            packet.Level = Level;
            packet.X = (byte)X;
            packet.Y = (byte)Y;
            packet.Z = (byte)Z;
            packet.Dir = (byte)Dir;
            packet.Passable = Passable;
            packet.HideName = HideName;
            packet.HideEntity = HideEntity;
            packet.Animations = Animations.ToArray();
            packet.Vital = GetVitals();
            packet.MaxVital = GetMaxVitals();
            packet.Stats = GetStatValues();
            packet.StatusEffects = StatusPackets();
            packet.NameColor = NameColor;
            packet.HeaderLabel = new LabelPacket(HeaderLabel.Text, HeaderLabel.Color);
            packet.FooterLabel = new LabelPacket(FooterLabel.Text, FooterLabel.Color);

            return packet;
        }

        public StatusPacket[] StatusPackets()
        {
            var statuses = CachedStatuses;
            var statusPackets = new StatusPacket[statuses.Length];
            for (var i = 0; i < statuses.Length; i++)
            {
                var status = statuses[i];
                int[] vitalShields = null;
                if (status.Type == SpellEffect.Shield)
                {
                    vitalShields = new int[Enum.GetValues<Vital>().Length];
                    for (var x = 0; x < Enum.GetValues<Vital>().Length; x++)
                    {
                        vitalShields[x] = status.Shield[x];
                    }
                }

                statusPackets[i] = new StatusPacket(
                    status.Spell.Id, status.Type, status.Data, (int)(status.Duration - Timing.Global.Milliseconds),
                    (int)(status.Duration - status.StartTime), vitalShields
                );
            }

            return statusPackets;
        }

        #region Spell Cooldowns

        [JsonIgnore, Column("SpellCooldowns")]
        public string SpellCooldownsJson
        {
            get => JsonConvert.SerializeObject(SpellCooldowns);
            set => SpellCooldowns = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, long>>(value ?? "{}");
        }

        [NotMapped] public ConcurrentDictionary<Guid, long> SpellCooldowns = new ConcurrentDictionary<Guid, long>();

        #endregion

    }

}

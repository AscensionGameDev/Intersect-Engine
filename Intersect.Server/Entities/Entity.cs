using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.UI;
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

namespace Intersect.Server.Entities
{

    public partial class Entity : IDisposable
    {

        //Instance Values
        private Guid _id;

        [JsonProperty("MaxVitals"), NotMapped] private int[] _maxVital = new int[(int) Vitals.VitalCount];

        [NotMapped, JsonIgnore] public Stat[] Stat = new Stat[(int) Stats.StatCount];

        [NotMapped, JsonIgnore] public Entity Target { get; set; } = null;

        public Entity() : this(Guid.NewGuid())
        {
        }

        //Initialization
        public Entity(Guid instanceId)
        {
            if (!(this is EventPageInstance) && !(this is Projectile))
            {
                for (var i = 0; i < (int)Stats.StatCount; i++)
                {
                    Stat[i] = new Stat((Stats)i, this);
                }
            }

            Id = instanceId;
        }

        [Column(Order = 1), JsonProperty(Order = -2)]
        public string Name { get; set; }

        public Guid MapId { get; set; }

        [NotMapped]
        public string MapName => MapInstance.GetName(MapId);

        [JsonIgnore]
        [NotMapped]
        public MapInstance Map => MapInstance.Get(MapId);

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int Dir { get; set; }

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
            get => DatabaseUtils.SaveIntArray(mVitals, (int) Enums.Vitals.VitalCount);
            set => mVitals = DatabaseUtils.LoadIntArray(value, (int) Enums.Vitals.VitalCount);
        }

        [JsonProperty("Vitals"), NotMapped]
        private int[] mVitals { get; set; } = new int[(int) Enums.Vitals.VitalCount];

        [JsonIgnore, NotMapped]
        private int[] mOldVitals { get; set; } = new int[(int)Enums.Vitals.VitalCount];

        [JsonIgnore, NotMapped]
        private int[] mOldMaxVitals { get; set; } = new int[(int)Enums.Vitals.VitalCount];

        //Stats based on npc settings, class settings, etc for quick calculations
        [JsonIgnore, Column(nameof(BaseStats))]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(BaseStats, (int) Enums.Stats.StatCount);
            set => BaseStats = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] BaseStats { get; set; } =
            new int[(int) Enums.Stats
                .StatCount]; // TODO: Why can this be BaseStats while Vitals is _vital and MaxVitals is _maxVital?

        [JsonIgnore, Column(nameof(StatPointAllocations))]
        public string StatPointsJson
        {
            get => DatabaseUtils.SaveIntArray(StatPointAllocations, (int) Enums.Stats.StatCount);
            set => StatPointAllocations = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] StatPointAllocations { get; set; } = new int[(int) Enums.Stats.StatCount];

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

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public bool Blocking { get; set; }

        [NotMapped, JsonIgnore]
        public Entity CastTarget { get; set; }

        [NotMapped, JsonIgnore]
        public Guid CollisionIndex { get; set; }

        [NotMapped, JsonIgnore]
        public long CombatTimer { get; set; }

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
        public long RegenTimer { get; set; } = Globals.Timing.Milliseconds;

        [NotMapped, JsonIgnore]
        public int SpellCastSlot { get; set; } = 0;

        //Status effects
        [NotMapped, JsonIgnore]
        public ConcurrentDictionary<SpellBase, Status> Statuses { get; } = new ConcurrentDictionary<SpellBase, Status>();

        [JsonIgnore, NotMapped]
        public Status[] CachedStatuses = new Status[0];

        [JsonIgnore, NotMapped]
        private Status[] mOldStatuses = new Status[0];

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
                    //Cast timers
                    if (CastTime != 0 && CastTime < timeMs)
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

                    var statsUpdated = false;
                    var statTime = Globals.Timing.Milliseconds;
                    for (var i = 0; i < (int)Stats.StatCount; i++)
                    {
                        statsUpdated |= Stat[i].Update(statTime);
                    }

                    if (statsUpdated)
                    {
                        PacketSender.SendEntityStats(this);
                    }

                    //Regen Timers
                    if (timeMs > CombatTimer && timeMs > RegenTimer)
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

        //Movement
        /// <summary>
        ///     Determines if this entity can move in the direction given.
        ///     Returns -5 if the tile is completely out of bounds.
        ///     Returns -3 if a tile is blocked because of a Z dimension tile
        ///     Returns -2 if a tile is blocked by a map attribute.
        ///     Returns -1 for clear.
        ///     Returns the type of entity that is blocking the way (if one exists)
        /// </summary>
        /// <param name="moveDir"></param>
        /// <returns></returns>
        public virtual int CanMove(int moveDir)
        {
            var xOffset = 0;
            var yOffset = 0;

            // If this is an Npc that has the Static behaviour, it can NEVER move.
            if (this is Npc npc)
            {
                if (npc.Base.Movement == (byte) NpcMovement.Static)
                {
                    return -2;
                }
            }

            var tile = new TileHelper(MapId, X, Y);
            switch (moveDir)
            {
                case 0: //Up
                    yOffset--;

                    break;
                case 1: //Down
                    yOffset++;

                    break;
                case 2: //Left
                    xOffset--;

                    break;
                case 3: //Right
                    xOffset++;

                    break;
                case 4: //NW
                    yOffset--;
                    xOffset--;

                    break;
                case 5: //NE
                    yOffset--;
                    xOffset++;

                    break;
                case 6: //SW
                    yOffset++;
                    xOffset--;

                    break;
                case 7: //SE
                    yOffset++;
                    xOffset++;

                    break;
            }

            MapInstance mapInstance = null;
            int tileX = 0;
            int tileY = 0;

            if (tile.Translate(xOffset, yOffset))
            {
                mapInstance = MapInstance.Get(tile.GetMapId());
                tileX = tile.GetX();
                tileY = tile.GetY();
                var tileAttribute = mapInstance.Attributes[tileX, tileY];
                if (tileAttribute != null)
                {
                    if (tileAttribute.Type == MapAttributes.Blocked || (tileAttribute.Type == MapAttributes.Animation && ((MapAnimationAttribute)tileAttribute).IsBlock))
                    {
                        return -2;
                    }

                    if (tileAttribute.Type == MapAttributes.NpcAvoid && this is Npc)
                    {
                        return -2;
                    }

                    if (tileAttribute.Type == MapAttributes.ZDimension &&
                        ((MapZDimensionAttribute) tileAttribute).BlockedLevel > 0 &&
                        ((MapZDimensionAttribute) tileAttribute).BlockedLevel - 1 == Z)
                    {
                        return -3;
                    }

                    if (tileAttribute.Type == MapAttributes.Slide)
                    {
                        if (this is EventPage)
                        {
                            return -4;
                        }

                        switch (((MapSlideAttribute) tileAttribute).Direction)
                        {
                            case 1:
                                if (moveDir == 1)
                                {
                                    return -4;
                                }

                                break;
                            case 2:
                                if (moveDir == 0)
                                {
                                    return -4;
                                }

                                break;
                            case 3:
                                if (moveDir == 3)
                                {
                                    return -4;
                                }

                                break;
                            case 4:
                                if (moveDir == 2)
                                {
                                    return -4;
                                }

                                break;
                        }
                    }
                }
            }
            else
            {
                return -5; //Out of Bounds
            }

            if (!Passable)
            {
                var targetMap = mapInstance;
                var mapEntities = mapInstance.GetCachedEntities();
                foreach (var en in mapEntities)
                {
                    if (en != null && en.X == tileX && en.Y == tileY && en.Z == Z && !en.Passable)
                    {
                        //Set a target if a projectile
                        CollisionIndex = en.Id;
                        if (en is Player)
                        {
                            if (this is Player)
                            {
                                //Check if this target player is passable....
                                if (!Options.Instance.Passability.Passable[(int)targetMap.ZoneType])
                                {
                                    return (int)EntityTypes.Player;
                                }
                            }
                            else
                            {
                                return (int)EntityTypes.Player;
                            }
                        }
                        else if (en is Npc)
                        {
                            return (int)EntityTypes.Player;
                        }
                        else if (en is Resource resource)
                        {
                            //If determine if we should walk
                            if (!resource.IsPassable())
                            {
                                return (int)EntityTypes.Resource;
                            }
                        }
                    }
                }

                //If this is an npc or other event.. if any global page exists that isn't passable then don't walk here!
                if (!(this is Player))
                {
                    foreach (var evt in mapInstance.GlobalEventInstances)
                    {
                        foreach (var en in evt.Value.GlobalPageInstance)
                        {
                            if (en != null && en.X == tileX && en.Y == tileY && en.Z == Z && !en.Passable)
                            {
                                return (int)EntityTypes.Event;
                            }
                        }
                    }
                }
            }

            return IsTileWalkable(tile.GetMap(), tile.GetX(), tile.GetY(), Z);
        }

        protected virtual int IsTileWalkable(MapInstance map, int x, int y, int z)
        {
            //Out of bounds if no map
            if (map == null)
            {
                return -5;
            }

            //Otherwise fine
            return -1;
        }

        protected virtual bool ProcessMoveRoute(Player forPlayer, long timeMs)
        {
            var moved = false;
            byte lookDir = 0, moveDir = 0;
            if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
            {
                switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                {
                    case MoveRouteEnum.MoveUp:
                        if (CanMove((int) Directions.Up) == -1)
                        {
                            Move((int) Directions.Up, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveDown:
                        if (CanMove((int) Directions.Down) == -1)
                        {
                            Move((int) Directions.Down, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveLeft:
                        if (CanMove((int) Directions.Left) == -1)
                        {
                            Move((int) Directions.Left, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveRight:
                        if (CanMove((int) Directions.Right) == -1)
                        {
                            Move((int) Directions.Right, forPlayer, false, true);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.MoveRandomly:
                        var dir = (byte)Randomization.Next(0, 4);
                        if (CanMove(dir) == -1)
                        {
                            Move(dir, forPlayer);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.StepForward:
                        if (CanMove(Dir) > -1)
                        {
                            Move((byte) Dir, forPlayer);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.StepBack:
                        switch (Dir)
                        {
                            case (int) Directions.Up:
                                moveDir = (int) Directions.Down;

                                break;
                            case (int) Directions.Down:
                                moveDir = (int) Directions.Up;

                                break;
                            case (int) Directions.Left:
                                moveDir = (int) Directions.Right;

                                break;
                            case (int) Directions.Right:
                                moveDir = (int) Directions.Left;

                                break;
                        }

                        if (CanMove(moveDir) > -1)
                        {
                            Move(moveDir, forPlayer);
                            moved = true;
                        }

                        break;
                    case MoveRouteEnum.FaceUp:
                        ChangeDir((int) Directions.Up);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceDown:
                        ChangeDir((int) Directions.Down);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceLeft:
                        ChangeDir((int) Directions.Left);
                        moved = true;

                        break;
                    case MoveRouteEnum.FaceRight:
                        ChangeDir((int) Directions.Right);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn90Clockwise:
                        switch (Dir)
                        {
                            case (int) Directions.Up:
                                lookDir = (int) Directions.Right;

                                break;
                            case (int) Directions.Down:
                                lookDir = (int) Directions.Left;

                                break;
                            case (int) Directions.Left:
                                lookDir = (int) Directions.Up;

                                break;
                            case (int) Directions.Right:
                                lookDir = (int) Directions.Down;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn90CounterClockwise:
                        switch (Dir)
                        {
                            case (int)Directions.Up:
                                lookDir = (int)Directions.Left;

                                break;
                            case (int)Directions.Down:
                                lookDir = (int)Directions.Right;

                                break;
                            case (int)Directions.Left:
                                lookDir = (int)Directions.Down;

                                break;
                            case (int)Directions.Right:
                                lookDir = (int)Directions.Up;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn180:
                        switch (Dir)
                        {
                            case (int) Directions.Up:
                                lookDir = (int) Directions.Down;

                                break;
                            case (int) Directions.Down:
                                lookDir = (int) Directions.Up;

                                break;
                            case (int) Directions.Left:
                                lookDir = (int) Directions.Right;

                                break;
                            case (int) Directions.Right:
                                lookDir = (int) Directions.Left;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.TurnRandomly:
                        ChangeDir((byte)Randomization.Next(0, 4));
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait100:
                        MoveTimer = Globals.Timing.Milliseconds + 100;
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait500:
                        MoveTimer = Globals.Timing.Milliseconds + 500;
                        moved = true;

                        break;
                    case MoveRouteEnum.Wait1000:
                        MoveTimer = Globals.Timing.Milliseconds + 1000;
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

                if (moved && MoveTimer < Globals.Timing.Milliseconds)
                {
                    MoveTimer = Globals.Timing.Milliseconds + (long) GetMovementTime();
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
            var time = 1000f / (float) (1 + Math.Log(Stat[(int) Stats.Speed].Value()));
            if (Blocking)
            {
                time += time * Options.BlockingSlow;
            }

            return Math.Min(1000f, time);
        }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public virtual void Move(int moveDir, Player forPlayer, bool doNotUpdate = false, bool correction = false)
        {
            if (Globals.Timing.Milliseconds < MoveTimer || (!Options.Combat.MovementCancelsCast && CastTime > 0))
            {
                return;
            }

            lock (EntityLock)
            {
                if (this is Player && CastTime > 0 && Options.Combat.MovementCancelsCast)
                {
                    CastTime = 0;
                    CastTarget = null;
                }

                var xOffset = 0;
                var yOffset = 0;
                switch (moveDir)
                {
                    case 0: //Up
                        --yOffset;

                        break;
                    case 1: //Down
                        ++yOffset;

                        break;
                    case 2: //Left
                        --xOffset;

                        break;
                    case 3: //Right
                        ++xOffset;

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

                    var currentMap = MapInstance.Get(tile.GetMapId());
                    if (MapId != tile.GetMapId())
                    {
                        var oldMap = MapInstance.Get(MapId);
                        oldMap?.RemoveEntity(this);
                        currentMap?.AddEntity(this);

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
                            var localMaps = currentMap.GetSurroundingMaps(true);
                            foreach (var map in localMaps)
                            {
                                var projectiles = map.MapProjectilesCached;
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

                        MoveTimer = Globals.Timing.Milliseconds + (long)GetMovementTime();
                    }

                    if (TryToChangeDimension() && doNotUpdate == true)
                    {
                        PacketSender.UpdateEntityZDimension(this, (byte)Z);
                    }

                    //Check for traps
                    if (currentMap != null)
                    {
                        foreach (var trap in currentMap.MapTrapsCached)
                        {
                            trap.CheckEntityHasDetonatedTrap(this);
                        }
                    }

                    // TODO: Why was this scoped to only Event entities?
                    //                if (currentMap != null && this is EventPageInstance)
                    var attribute = currentMap?.Attributes[X, Y];

                    // ReSharper disable once InvertIf
                    //Check for slide tiles
                    if (attribute?.Type == MapAttributes.Slide)
                    {
                        // If sets direction, set it.
                        if (((MapSlideAttribute)attribute).Direction > 0)
                        {
                            //Check for slide tiles
                            if (attribute != null && attribute.Type == MapAttributes.Slide)
                            {
                                if (((MapSlideAttribute)attribute).Direction > 0)
                                {
                                    Dir = (byte)(((MapSlideAttribute)attribute).Direction - 1);
                                }
                            }
                        }

                        var dash = new Dash(this, 1, (byte)Dir);
                    }
                }
            }
        }

        public void ChangeDir(int dir)
        {
            if (dir == -1)
            {
                return;
            }

            if (Dir != dir)
            {
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
        }

        // Change the dimension if the player is on a gateway
        public bool TryToChangeDimension()
        {
            if (X < Options.MapWidth && X >= 0)
            {
                if (Y < Options.MapHeight && Y >= 0)
                {
                    var attribute = MapInstance.Get(MapId).Attributes[X, Y];
                    if (attribute != null && attribute.Type == MapAttributes.ZDimension)
                    {
                        if (((MapZDimensionAttribute) attribute).GatewayTo > 0)
                        {
                            Z = (byte) (((MapZDimensionAttribute) attribute).GatewayTo - 1);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //Misc
        public int GetDirectionTo(Entity target)
        {
            int xDiff = 0, yDiff = 0;

            var map = MapInstance.Get(MapId);
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

                    if (grid.MyGrid[x, y] != Guid.Empty &&
                        grid.MyGrid[x, y] == target.MapId)
                    {
                        xDiff = (x - map.MapGridX) * Options.MapWidth + target.X - X;
                        yDiff = (y - map.MapGridY) * Options.MapHeight + target.Y - Y;
                        if (Math.Abs(xDiff) > Math.Abs(yDiff))
                        {
                            if (xDiff < 0)
                            {
                                return (int) Directions.Left;
                            }

                            if (xDiff > 0)
                            {
                                return (int) Directions.Right;
                            }
                        }
                        else
                        {
                            if (yDiff < 0)
                            {
                                return (int) Directions.Up;
                            }

                            if (yDiff > 0)
                            {
                                return (int) Directions.Down;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        //Combat
        public virtual int CalculateAttackTime()
        {
            return (int) (Options.MaxAttackRate +
                          (float) ((Options.MinAttackRate - Options.MaxAttackRate) *
                                   (((float) Options.MaxStatValue - Stat[(int) Stats.Speed].Value()) /
                                    (float) Options.MaxStatValue)));
        }

        public void TryBlock(bool blocking)
        {
            if (AttackTimer < Globals.Timing.Milliseconds)
            {
                if (blocking && !Blocking && AttackTimer < Globals.Timing.Milliseconds)
                {
                    Blocking = true;
                    PacketSender.SendEntityAttack(this, -1);
                }
                else if (!blocking && Blocking)
                {
                    Blocking = false;
                    AttackTimer = Globals.Timing.Milliseconds + CalculateAttackTime();
                    PacketSender.SendEntityAttack(this, 0);
                }
            }
        }

        public virtual int GetWeaponDamage()
        {
            return 0;
        }

        public virtual bool CanAttack(Entity entity, SpellBase spell)
        {
            return CastTime <= 0;
        }

        public virtual void ProcessRegen()
        {
        }

        public int GetVital(int vital)
        {
            return mVitals[vital];
        }

        public int[] GetVitals()
        {
            var vitals = new int[(int) Vitals.VitalCount];
            Array.Copy(mVitals, 0, vitals, 0, (int) Vitals.VitalCount);

            return vitals;
        }

        public int GetVital(Vitals vital)
        {
            return GetVital((int) vital);
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

        public void SetVital(Vitals vital, int value)
        {
            SetVital((int) vital, value);
        }

        public virtual int GetMaxVital(int vital)
        {
            return _maxVital[vital];
        }

        public virtual int GetMaxVital(Vitals vital)
        {
            return GetMaxVital((int) vital);
        }

        public int[] GetMaxVitals()
        {
            var vitals = new int[(int) Vitals.VitalCount];
            for (var vitalIndex = 0; vitalIndex < vitals.Length; ++vitalIndex)
            {
                vitals[vitalIndex] = GetMaxVital(vitalIndex);
            }

            return vitals;
        }

        public void SetMaxVital(int vital, int value)
        {
            if (value <= 0 && vital == (int) Vitals.Health)
            {
                value = 1; //Must have at least 1 hp
            }

            if (value < 0 && vital == (int) Vitals.Mana)
            {
                value = 0; //Can't have less than 0 mana
            }

            _maxVital[vital] = value;
            if (value < GetVital(vital))
            {
                SetVital(vital, value);
            }
        }

        public void SetMaxVital(Vitals vital, int value)
        {
            SetMaxVital((int) vital, value);
        }

        public bool HasVital(Vitals vital)
        {
            return GetVital(vital) > 0;
        }

        public bool IsFullVital(Vitals vital)
        {
            return GetVital(vital) == GetMaxVital(vital);
        }

        //Vitals
        public void RestoreVital(Vitals vital)
        {
            SetVital(vital, GetMaxVital(vital));
        }

        public void AddVital(Vitals vital, int amount)
        {
            if (vital >= Vitals.VitalCount)
            {
                return;
            }

            var vitalId = (int) vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, int.MaxValue - maxVitalValue);
            SetVital(vital, GetVital(vital) + safeAmount);
        }

        public void SubVital(Vitals vital, int amount)
        {
            if (vital >= Vitals.VitalCount)
            {
                return;
            }

            //Check for any shields.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == StatusTypes.Shield)
                {
                    status.DamageShield(vital, ref amount);
                }
            }

            var vitalId = (int) vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, GetVital(vital));
            SetVital(vital, GetVital(vital) - safeAmount);
        }

        public virtual int[] GetStatValues()
        {
            var stats = new int[(int) Stats.StatCount];
            for (var i = 0; i < (int) Stats.StatCount; i++)
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
        public virtual void TryAttack(
            Entity target,
            ProjectileBase projectile,
            SpellBase parentSpell,
            ItemBase parentItem,
            byte projectileDir
        )
        {
            if (target is Resource && parentSpell != null)
            {
                return;
            }

            //Check for taunt status and trying to attack a target that has not taunted you.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == StatusTypes.Taunt)
                {
                    if (Target != target)
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Combat.Missed);

                        return;
                    }
                }
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

                if (MapInstance.Get(MapId).ZoneType == MapZones.Safe)
                {
                    return;
                }

                if (MapInstance.Get(target.MapId).ZoneType == MapZones.Safe)
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
                    target, parentItem.Damage, 0, (DamageType) parentItem.DamageType, (Stats) parentItem.ScalingStat,
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

            //If there is a knockback, knock them backwards and make sure its linear (diagonal player movement not coded).
            if (projectile.Knockback > 0 && projectileDir < 4)
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
            if (target is Resource)
            {
                return;
            }

            if (spellBase == null)
            {
                return;
            }

            //Check for taunt status and trying to attack a target that has not taunted you.
            if (!trapTrigger) //Traps ignore taunts.
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == StatusTypes.Taunt)
                    {
                        if (Target != target)
                        {
                            PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Combat.Missed);

                            return;
                        }
                    }
                }
            }

            var deadAnimations = new List<KeyValuePair<Guid, sbyte>>();
            var aliveAnimations = new List<KeyValuePair<Guid, sbyte>>();

            //Only count safe zones and friendly fire if its a dangerous spell! (If one has been used)
            if (!spellBase.Combat.Friendly &&
                (spellBase.Combat.TargetType != (int) SpellTargetTypes.Self || onHitTrigger))
            {
                //If about to hit self with an unfriendly spell (maybe aoe?) return
                if (target == this && spellBase.Combat.Effect != StatusTypes.OnHit)
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
                    if (MapInstance.Get(MapId).ZoneType == MapZones.Safe)
                    {
                        return;
                    }

                    if (MapInstance.Get(target.MapId).ZoneType == MapZones.Safe)
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
                (spellBase.Combat.Effect != StatusTypes.OnHit || onHitTrigger))
            {
                deadAnimations.Add(new KeyValuePair<Guid, sbyte>(spellBase.HitAnimationId, (sbyte) Directions.Up));
                aliveAnimations.Add(new KeyValuePair<Guid, sbyte>(spellBase.HitAnimationId, (sbyte) Directions.Up));
            }

            var statBuffTime = -1;
            var expireTime = Globals.Timing.Milliseconds + spellBase.Combat.Duration;
            for (var i = 0; i < (int) Stats.StatCount; i++)
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

            var damageHealth = spellBase.Combat.VitalDiff[(int)Vitals.Health];
            var damageMana = spellBase.Combat.VitalDiff[(int)Vitals.Mana];

            if ((spellBase.Combat.Effect != StatusTypes.OnHit || onHitTrigger) &&
                spellBase.Combat.Effect != StatusTypes.Shield)
            {
                Attack(
                    target, damageHealth, damageMana, (DamageType) spellBase.Combat.DamageType,
                    (Stats) spellBase.Combat.ScalingStat, spellBase.Combat.Scaling, spellBase.Combat.CritChance,
                    spellBase.Combat.CritMultiplier, deadAnimations, aliveAnimations, false
                );
            }

            if (spellBase.Combat.Effect > 0) //Handle status effects
            {
                //Check for onhit effect to avoid the onhit effect recycling.
                if (!(onHitTrigger && spellBase.Combat.Effect == StatusTypes.OnHit))
                {
                    new Status(
                        target, this, spellBase, spellBase.Combat.Effect, spellBase.Combat.Duration,
                        spellBase.Combat.TransformSprite
                    );

                    PacketSender.SendActionMsg(
                        target, Strings.Combat.status[(int) spellBase.Combat.Effect], CustomColors.Combat.Status
                    );

                    //If an onhit or shield status bail out as we don't want to do any damage.
                    if (spellBase.Combat.Effect == StatusTypes.OnHit || spellBase.Combat.Effect == StatusTypes.Shield)
                    {
                        Animate(target, aliveAnimations);

                        return;
                    }
                }
            }
            else
            {
                if (statBuffTime > -1)
                {
                    new Status(target, this, spellBase, spellBase.Combat.Effect, statBuffTime, "");
                }
            }

            //Handle DoT/HoT spells]
            if (spellBase.Combat.HoTDoT)
            {
                var doTFound = false;
                foreach (var dot in target.CachedDots)
                {
                    if (dot.SpellBase.Id == spellBase.Id && dot.Target == this)
                    {
                        doTFound = true;
                    }
                }

                if (doTFound == false) //no duplicate DoT/HoT spells.
                {
                    new DoT(this, spellBase.Id, target);
                }
            }
        }

        private void Animate(Entity target, List<KeyValuePair<Guid, sbyte>> animations)
        {
            foreach (var anim in animations)
            {
                PacketSender.SendAnimationToProximity(anim.Key, 1, target.Id, target.MapId, 0, 0, anim.Value);
            }
        }

        //Attacking with weapon or unarmed.
        public virtual void TryAttack(Entity target)
        {
            //See player and npc override of this virtual void
        }

        //Attack using a weapon or unarmed
        public virtual void TryAttack(
            Entity target,
            int baseDamage,
            DamageType damageType,
            Stats scalingStat,
            int scaling,
            int critChance,
            double critMultiplier,
            List<KeyValuePair<Guid, sbyte>> deadAnimations = null,
            List<KeyValuePair<Guid, sbyte>> aliveAnimations = null,
            ItemBase weapon = null
        )
        {
            if (AttackTimer > Globals.Timing.Milliseconds)
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

                if (MapInstance.Get(MapId)?.ZoneType == MapZones.Safe)
                {
                    return;
                }

                if (MapInstance.Get(target.MapId)?.ZoneType == MapZones.Safe)
                {
                    return;
                }
            }

            //Check for taunt status and trying to attack a target that has not taunted you.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == StatusTypes.Taunt)
                {
                    if (Target != target)
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Combat.Missed);

                        return;
                    }
                }
            }

            AttackTimer = Globals.Timing.Milliseconds + CalculateAttackTime();

            //Check if the attacker is blinded.
            if (IsOneBlockAway(target))
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == StatusTypes.Stun ||
                        status.Type == StatusTypes.Blind ||
                        status.Type == StatusTypes.Sleep)
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
            Stats scalingStat,
            int scaling,
            int critChance,
            double critMultiplier,
            List<KeyValuePair<Guid, sbyte>> deadAnimations = null,
            List<KeyValuePair<Guid, sbyte>> aliveAnimations = null,
            bool isAutoAttack = false
        )
        {
            var originalBaseDamage = baseDamage;
            var damagingAttack = baseDamage > 0;
            if (enemy == null)
            {
                return;
            }

            var invulnerable = enemy.CachedStatuses.Any(status => status.Type == StatusTypes.Invulnerable);

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

            //Calculate Damages
            if (baseDamage != 0)
            {

                if (enemy is Resource)
                {
                    baseDamage = originalBaseDamage;
                }
                else
                {
                    baseDamage = Formulas.CalculateDamage(
                    baseDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy
                );
                }
                
                if (baseDamage < 0 && damagingAttack)
                {
                    baseDamage = 0;
                }

                if (baseDamage > 0 && enemy.HasVital(Vitals.Health) && !invulnerable)
                {
                    if (isCrit)
                    {
                        PacketSender.SendActionMsg(enemy, Strings.Combat.critical, CustomColors.Combat.Critical);
                    }

                    enemy.SubVital(Vitals.Health, (int) baseDamage);
                    switch (damageType)
                    {
                        case DamageType.Physical:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + (int) baseDamage,
                                CustomColors.Combat.PhysicalDamage
                            );

                            break;
                        case DamageType.Magic:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + (int) baseDamage, CustomColors.Combat.MagicDamage
                            );

                            break;
                        case DamageType.True:
                            PacketSender.SendActionMsg(
                                enemy, Strings.Combat.removesymbol + (int) baseDamage, CustomColors.Combat.TrueDamage
                            );

                            break;
                    }

                    var toRemove = new List<Status>();
                    foreach (var status in enemy.CachedStatuses.ToArray())  // ToArray the Array since removing a status will.. you know, change the collection.
                    {
                        //Wake up any sleeping targets targets and take stealthed entities out of stealth
                        if (status.Type == StatusTypes.Sleep || status.Type == StatusTypes.Stealth)
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
                else if (baseDamage < 0 && !enemy.IsFullVital(Vitals.Health))
                {
                    enemy.AddVital(Vitals.Health, (int) -baseDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.addsymbol + (int) Math.Abs(baseDamage), CustomColors.Combat.Heal
                    );
                }
            }

            if (secondaryDamage != 0)
            {
                secondaryDamage = Formulas.CalculateDamage(
                    secondaryDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy
                );

                if (secondaryDamage < 0 && damagingAttack)
                {
                    secondaryDamage = 0;
                }

                if (secondaryDamage > 0 && enemy.HasVital(Vitals.Mana) && !invulnerable)
                {
                    //If we took damage lets reset our combat timer
                    enemy.SubVital(Vitals.Mana, (int) secondaryDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.removesymbol + (int) secondaryDamage, CustomColors.Combat.RemoveMana
                    );

                    //No Matter what, if we attack the entitiy, make them chase us
                    if (enemy is Npc enemyNpc)
                    {
                        enemyNpc.TryFindNewTarget(Timing.Global.Milliseconds, default, false, this);
                    }

                    enemy.NotifySwarm(this);
                }
                else if (secondaryDamage < 0 && !enemy.IsFullVital(Vitals.Mana))
                {
                    enemy.AddVital(Vitals.Mana, (int) -secondaryDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.addsymbol + (int) Math.Abs(secondaryDamage), CustomColors.Combat.AddMana
                    );
                }
            }

            // Set combat timers!
            enemy.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
            CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;

            //Check for lifesteal
            if (GetType() == typeof(Player) && enemy.GetType() != typeof(Resource))
            {
                var lifesteal = ((Player) this).GetLifeSteal() / 100;
                var healthRecovered = lifesteal * baseDamage;
                if (healthRecovered > 0) //Don't send any +0 msg's.
                {
                    AddVital(Vitals.Health, (int) healthRecovered);
                    PacketSender.SendActionMsg(
                        this, Strings.Combat.addsymbol + (int) healthRecovered, CustomColors.Combat.Heal
                    );
                }
            }

            //Dead entity check
            if (enemy.GetVital(Vitals.Health) <= 0)
            {
                if (enemy.GetType() == typeof(Npc) || enemy.GetType() == typeof(Resource))
                {
                    lock (enemy.EntityLock)
                    {
                        enemy.Die(true, this);
                    }
                }
                else
                {
                    //PVP Kill common events
                    if (!enemy.Dead && enemy is Player && this is Player)
                    {
                        ((Player)this).StartCommonEventsWithTrigger(CommonEventTrigger.PVPKill, "", enemy.Name);
                        ((Player)enemy).StartCommonEventsWithTrigger(CommonEventTrigger.PVPDeath, "", this.Name);
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
                            anim.Key, -1, Id, enemy.MapId, (byte) enemy.X, (byte) enemy.Y, anim.Value
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

                //Check for any onhit damage bonus effects!
                CheckForOnhitAttack(enemy, isAutoAttack);
            }

            // Add a timer before able to make the next move.
            if (GetType() == typeof(Npc))
            {
                ((Npc) this).MoveTimer = Globals.Timing.Milliseconds + (long) GetMovementTime();
            }
        }

        void CheckForOnhitAttack(Entity enemy, bool isAutoAttack)
        {
            if (isAutoAttack) //Ignore spell damage.
            {
                foreach (var status in CachedStatuses)
                {
                    if (status.Type == StatusTypes.OnHit)
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

        public bool CanCastSpell(Guid spellId, Entity target)
        {
            return CanCastSpell(SpellBase.Get(spellId), target);
        }

        public virtual bool CanCastSpell(SpellBase spellDescriptor, Entity target)
        {
            if (spellDescriptor == null)
            {
                return false;
            }

            var spellCombat = spellDescriptor.Combat;
            if (spellDescriptor.SpellType != SpellTypes.CombatSpell && spellDescriptor.SpellType != SpellTypes.Event ||
                spellCombat == null)
            {
                return true;
            }

            if (spellCombat.TargetType == SpellTargetTypes.Single)
            {
                return target == null || InRangeOf(target, spellCombat.CastRange);
            }

            return true;
        }

        public virtual void CastSpell(Guid spellId, int spellSlot = -1)
        {
            var spellBase = SpellBase.Get(spellId);
            if (spellBase == null)
            {
                return;
            }

            if (!CanCastSpell(spellBase, CastTarget))
            {
                return;
            }

            if (spellBase.VitalCost[(int)Vitals.Mana] > 0)
            {
                SubVital(Vitals.Mana, spellBase.VitalCost[(int)Vitals.Mana]);
            }
            else
            {
                AddVital(Vitals.Mana, -spellBase.VitalCost[(int)Vitals.Mana]);
            }

            if (spellBase.VitalCost[(int)Vitals.Health] > 0)
            {
                SubVital(Vitals.Health, spellBase.VitalCost[(int)Vitals.Health]);
            }
            else
            {
                AddVital(Vitals.Health, -spellBase.VitalCost[(int)Vitals.Health]);
            }

            switch (spellBase.SpellType)
            {
                case SpellTypes.CombatSpell:
                case SpellTypes.Event:

                    switch (spellBase.Combat.TargetType)
                    {
                        case SpellTargetTypes.Self:
                            if (spellBase.HitAnimationId != Guid.Empty && spellBase.Combat.Effect != StatusTypes.OnHit)
                            {
                                PacketSender.SendAnimationToProximity(
                                    spellBase.HitAnimationId, 1, Id, MapId, 0, 0, (sbyte) Dir
                                ); //Target Type 1 will be global entity
                            }

                            TryAttack(this, spellBase);

                            break;
                        case SpellTargetTypes.Single:
                            if (CastTarget == null)
                            {
                                return;
                            }

                            //If target has stealthed we cannot hit the spell.
                            foreach (var status in CastTarget.CachedStatuses)
                            {
                                if (status.Type == StatusTypes.Stealth)
                                {
                                    return;
                                }
                            }

                            if (spellBase.Combat.HitRadius > 0) //Single target spells with AoE hit radius'
                            {
                                HandleAoESpell(
                                    spellId, spellBase.Combat.HitRadius, CastTarget.MapId, CastTarget.X, CastTarget.Y,
                                    null
                                );
                            }
                            else
                            {
                                TryAttack(CastTarget, spellBase);
                            }

                            break;
                        case SpellTargetTypes.AoE:
                            HandleAoESpell(spellId, spellBase.Combat.HitRadius, MapId, X, Y, null);

                            break;
                        case SpellTargetTypes.Projectile:
                            var projectileBase = spellBase.Combat.Projectile;
                            if (projectileBase != null)
                            {
                                MapInstance.Get(MapId)
                                    .SpawnMapProjectile(
                                        this, projectileBase, spellBase, null, MapId, (byte) X, (byte) Y, (byte) Z,
                                        (byte) Dir, CastTarget
                                    );
                            }

                            break;
                        case SpellTargetTypes.OnHit:
                            if (spellBase.Combat.Effect == StatusTypes.OnHit)
                            {
                                new Status(
                                    this, this, spellBase, StatusTypes.OnHit, spellBase.Combat.OnHitDuration,
                                    spellBase.Combat.TransformSprite
                                );

                                PacketSender.SendActionMsg(
                                    this, Strings.Combat.status[(int) spellBase.Combat.Effect],
                                    CustomColors.Combat.Status
                                );
                            }

                            break;
                        case SpellTargetTypes.Trap:
                            MapInstance.Get(MapId).SpawnTrap(this, spellBase, (byte) X, (byte) Y, (byte) Z);

                            break;
                        default:
                            break;
                    }

                    break;
                case SpellTypes.Warp:
                    if (this is Player)
                    {
                        Warp(
                            spellBase.Warp.MapId, spellBase.Warp.X, spellBase.Warp.Y,
                            spellBase.Warp.Dir - 1 == -1 ? (byte) this.Dir : (byte) (spellBase.Warp.Dir - 1)
                        );
                    }

                    break;
                case SpellTypes.WarpTo:
                    if (CastTarget != null)
                    {
                        HandleAoESpell(spellId, spellBase.Combat.CastRange, MapId, X, Y, CastTarget);
                    }
                    break;
                case SpellTypes.Dash:
                    PacketSender.SendActionMsg(this, Strings.Combat.dash, CustomColors.Combat.Dash);
                    var dash = new Dash(
                        this, spellBase.Combat.CastRange, (byte) Dir, Convert.ToBoolean(spellBase.Dash.IgnoreMapBlocks),
                        Convert.ToBoolean(spellBase.Dash.IgnoreActiveResources),
                        Convert.ToBoolean(spellBase.Dash.IgnoreInactiveResources),
                        Convert.ToBoolean(spellBase.Dash.IgnoreZDimensionAttributes)
                    );

                    break;
                default:
                    break;
            }

            if (spellSlot >= 0 && spellSlot < Options.MaxPlayerSkills)
            {
                // Player cooldown handling is done elsewhere!
                if (this is Player player)
                {
                    player.UpdateCooldown(spellBase);

                    // Trigger the global cooldown, if we're allowed to.
                    if (!spellBase.IgnoreGlobalCooldown)
                    {
                        player.UpdateGlobalCooldown();
                    }
                }
                else
                {
                    SpellCooldowns[Spells[spellSlot].SpellId] =
                    Globals.Timing.MillisecondsUTC + (int)(spellBase.CooldownDuration);
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
                var startMap = MapInstance.Get(startMapId);
                if (startMap != null)
                {
                    var surroundingMaps = startMap.GetSurroundingMaps(true);
                    foreach (var map in surroundingMaps)
                    {
                        foreach (var entity in map.GetCachedEntities())
                        {
                            if (entity != null && (entity is Player || entity is Npc))
                            {
                                if (spellTarget == null || spellTarget == entity)
                                {
                                    if (entity.GetDistanceTo(startMap,startX,startY) <= range)
                                    {
                                        //Check to handle a warp to spell
                                        if (spellBase.SpellType == SpellTypes.WarpTo)
                                        {
                                            if (spellTarget != null)
                                            {
                                                //Spelltarget used to be Target. I don't know if this is correct or not.
                                                int[] position = GetPositionNearTarget(spellTarget.MapId, spellTarget.X, spellTarget.Y);
                                                Warp(spellTarget.MapId, (byte)position[0], (byte)position[1], (byte)Dir);
                                                ChangeDir(DirToEnemy(spellTarget));
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
        }

        private int[] GetPositionNearTarget(Guid mapId, int x, int y)
        {
            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                return new int[] { x, y };
            }

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
                            !map.TileBlocked(newX, newY))
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
                            !map.TileBlocked(newX, newY))
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

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        protected bool IsOneBlockAway(Entity target)
        {
            var myTile = new TileHelper(MapId, X, Y);
            var enemyTile = new TileHelper(target.MapId, target.X, target.Y);
            if (Z == target.Z)
            {
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
            }

            return false;
        }

        //These functions only work when one block away.
        protected bool IsFacingTarget(Entity target)
        {
            if (IsOneBlockAway(target))
            {
                var myTile = new TileHelper(MapId, X, Y);
                var enemyTile = new TileHelper(target.MapId, target.X, target.Y);
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Up)
                {
                    return true;
                }

                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Down)
                {
                    return true;
                }

                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Left)
                {
                    return true;
                }

                myTile.Translate(2, 0);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Right)
                {
                    return true;
                }
            }

            return false;
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

        public int GetDistanceTo(MapInstance targetMap, int targetX, int targetY)
        {
            return GetDistanceBetween(Map, targetMap, X, targetX, Y, targetY);
        }

        public int GetDistanceBetween(MapInstance mapA, MapInstance mapB, int xTileA, int xTileB, int yTileA, int yTileB)
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

        protected byte DirToEnemy(Entity target)
        {
            //Calculate World Tile of Me
            var x1 = X + MapInstance.Get(MapId).MapGridX * Options.MapWidth;
            var y1 = Y + MapInstance.Get(MapId).MapGridY * Options.MapHeight;

            //Calculate world tile of target
            var x2 = target.X + MapInstance.Get(target.MapId).MapGridX * Options.MapWidth;
            var y2 = target.Y + MapInstance.Get(target.MapId).MapGridY * Options.MapHeight;


            if (Math.Abs(x1 - x2) > Math.Abs(y1 - y2))
            {
                //Left or Right
                if (x1 - x2 < 0)
                {
                    return (byte) Directions.Right;
                }

                return (byte) Directions.Left;
            }

            //Left or Right
            if (y1 - y2 < 0)
            {
                return (byte) Directions.Down;
            }

            return (byte) Directions.Up;
        }

        // Outdated : Check if the target is either up, down, left or right of the target on the correct Z dimension.
        // Check for 8 directions
        protected bool IsOneBlockAway(Guid mapId, int x, int y, int z = 0)
        {
            //Calculate World Tile of Me
            var x1 = X + MapInstance.Get(MapId).MapGridX * Options.MapWidth;
            var y1 = Y + MapInstance.Get(MapId).MapGridY * Options.MapHeight;

            //Calculate world tile of target
            var x2 = x + MapInstance.Get(mapId).MapGridX * Options.MapWidth;
            var y2 = y + MapInstance.Get(mapId).MapGridY * Options.MapHeight;
            if (z == Z)
            {
                if (y1 == y2)
                {
                    if (x1 == x2 - 1)
                    {
                        return true;
                    }
                    else if (x1 == x2 + 1)
                    {
                        return true;
                    }
                }

                if (x1 == x2)
                {
                    if (y1 == y2 - 1)
                    {
                        return true;
                    }
                    else if (y1 == y2 + 1)
                    {
                        return true;
                    }
                }
            }

            return false;
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
                            // is this player in a party?
                            if (player.Party.Count > 0 && Options.Instance.LootOpts.IndividualizedLootAutoIncludePartyMembers)
                            {
                                // They are, so check for all party members and drop if still eligible!
                                foreach (var partyMember in player.Party)
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
            
            var currentMap = MapInstance.Get(MapId);
            if (currentMap != null)
            {
                foreach (var map in currentMap.GetSurroundingMaps(true))
                {
                    map.ClearEntityTargetsOf(this);
                }
            }

            DoT?.Clear();
            CachedDots = new DoT[0];
            Statuses?.Clear();
            CachedStatuses = new Status[0];
            Stat?.ToList().ForEach(stat => stat?.Reset());

            Dead = true;
        }

        private void DropItems(Entity killer, bool sendUpdate = true)
        {
            // Drop items
            for (var n = 0; n < Items.Count; n++)
            {
                if (Items[n] == null)
                {
                    continue;
                }

                // Don't mess with the actual object.
                var item = Items[n].Clone();
                
                var itemBase = ItemBase.Get(item.ItemId);
                if (itemBase == null)
                {
                    continue;
                }

                //Don't lose bound items on death for players.
                if (this.GetType() == typeof(Player))
                {
                    if (itemBase.DropChanceOnDeath == 0)
                    {
                        continue;
                    }
                }

                //Calculate the killers luck (If they are a player)
                var playerKiller = killer as Player;
                var luck = 1.0 + (playerKiller != null ? playerKiller.GetLuck() : 0) / 100;

                Guid lootOwner = Guid.Empty;
                if (this is Player)
                {
                    //Player drop rates
                    if (Randomization.Next(1, 101) >= itemBase.DropChanceOnDeath * luck)
                    {
                        continue;
                    }

                    // It's a player, try and set ownership to the player that killed them.. If it was a player.
                    // Otherwise set to self, so they can come and reclaim their items.
                    lootOwner = playerKiller?.Id ?? Id;
                }
                else
                {
                    //Npc drop rates
                    var randomChance = Randomization.Next(1, 100001);
                    if (randomChance >= (item.DropChance * 1000) * luck)
                    {
                        continue;
                    }

                    // Set owner to player that killed this, if there is any.
                    if (playerKiller != null && this is Npc thisNpc)
                    {
                        // Yes, so set the owner to the player that killed it.
                        lootOwner = playerKiller.Id;
                    }

                    // Set the attributes for this item.
                    item.Set(new Item(item.ItemId, item.Quantity, true));
                }

                // Spawn the actual item!
                var map = MapInstance.Get(MapId);
                map?.SpawnItem(X, Y, item, item.Quantity, lootOwner, sendUpdate);

                // Remove the item from inventory if a player.
                var player = this as Player;
                player?.TryTakeItem(Items[n], item.Quantity);
            }


        }

        public virtual bool IsDead()
        {
            return Dead;
        }

        public void Reset()
        {
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                RestoreVital((Vitals) i);
            }

            Dead = false;
        }

        //Empty virtual functions for players
        public virtual void Warp(Guid newMapId, float newX, float newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, (byte) Dir, adminWarp);
        }

        public virtual void Warp(
            Guid newMapId,
            float newX,
            float newY,
            byte newDir,
            bool adminWarp = false,
            byte zOverride = 0,
            bool mapSave = false
        )
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
            packet.X = (byte) X;
            packet.Y = (byte) Y;
            packet.Z = (byte) Z;
            packet.Dir = (byte) Dir;
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
                if (status.Type == StatusTypes.Shield)
                {
                    vitalShields = new int[(int) Vitals.VitalCount];
                    for (var x = 0; x < (int) Vitals.VitalCount; x++)
                    {
                        vitalShields[x] = status.shield[x];
                    }
                }

                statusPackets[i] = new StatusPacket(
                    status.Spell.Id, status.Type, status.Data, (int) (status.Duration - Globals.Timing.Milliseconds),
                    (int) (status.Duration - status.StartTime), vitalShields
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Entities
{

    public partial class Entity : IDisposable
    {

        //Instance Values
        private Guid _id;

        [JsonProperty("MaxVitals"), NotMapped] private int[] _maxVital = new int[(int) Vitals.VitalCount];

        [NotMapped, JsonIgnore] public Stat[] Stat = new Stat[(int) Stats.StatCount];

        [NotMapped, JsonIgnore] public Entity Target = null;

        public Entity() : this(Guid.NewGuid())
        {
        }

        //Initialization
        public Entity(Guid instanceId)
        {
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                Stat[i] = new Stat((Stats) i, this);
            }

            Id = instanceId;
        }

        [Column(Order = 1), JsonProperty(Order = -2)]
        public string Name { get; set; }

        public Guid MapId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public MapInstance Map => MapInstance.Get(MapId);

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int Dir { get; set; }

        public string Sprite { get; set; }

        public string Face { get; set; }

        public int Level { get; set; }

        [JsonIgnore, Column("Vitals")]
        public string VitalsJson
        {
            get => DatabaseUtils.SaveIntArray(_vital, (int) Enums.Vitals.VitalCount);
            set => _vital = DatabaseUtils.LoadIntArray(value, (int) Enums.Vitals.VitalCount);
        }

        [JsonProperty("Vitals"), NotMapped]
        public int[] _vital { get; set; } = new int[(int) Enums.Vitals.VitalCount];

        //Stats based on npc settings, class settings, etc for quick calculations
        [JsonIgnore, Column("BaseStats")]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(BaseStats, (int) Enums.Stats.StatCount);
            set => BaseStats = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] BaseStats { get; set; } =
            new int[(int) Enums.Stats
                .StatCount]; // TODO: Why can this be BaseStats while Vitals is _vital and MaxVitals is _maxVital?

        [JsonIgnore, Column("StatPointAllocations")]
        public string StatPointsJson
        {
            get => DatabaseUtils.SaveIntArray(StatPointAllocations, (int) Enums.Stats.StatCount);
            set => StatPointAllocations = DatabaseUtils.LoadIntArray(value, (int) Enums.Stats.StatCount);
        }

        [NotMapped]
        public int[] StatPointAllocations { get; set; } = new int[(int) Enums.Stats.StatCount];

        //Inventory
        [NotNull, JsonIgnore]
        public virtual List<InventorySlot> Items { get; set; } = new List<InventorySlot>();

        //Spells
        [NotNull, JsonIgnore]
        public virtual List<SpellSlot> Spells { get; set; } = new List<SpellSlot>();

        [JsonIgnore, Column("NameColor")]
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

        [JsonIgnore, Column("HeaderLabel")]
        public string HeaderLabelJson
        {
            get => JsonConvert.SerializeObject(HeaderLabel);
            set => HeaderLabel = value != null ? JsonConvert.DeserializeObject<Label>(value) : new Label();
        }

        [NotMapped]
        public Label FooterLabel { get; set; }

        [JsonIgnore, Column("FooterLabel")]
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
        public List<DoT> DoT { get; set; } = new List<DoT>();

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
        [NotMapped, JsonIgnore, NotNull]
        public Dictionary<SpellBase, Status> Statuses { get; } = new Dictionary<SpellBase, Status>();

        [NotMapped, JsonIgnore]
        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        public virtual void Update(long timeMs)
        {
            //Cast timers
            if (CastTime != 0 && CastTime < timeMs)
            {
                CastTime = 0;
                CastSpell(Spells[SpellCastSlot].SpellId, SpellCastSlot);
                CastTarget = null;
            }

            //DoT/HoT timers
            for (var i = 0; i < DoT.Count; i++)
            {
                DoT[i].Tick();
            }

            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                if (Stat[i].Update())
                {
                    SendStatUpdate(i);
                }
            }

            //Regen Timers
            if (timeMs > CombatTimer && timeMs > RegenTimer)
            {
                ProcessRegen();
                RegenTimer = timeMs + Options.RegenTime;
            }

            //Status timers
            var statusArray = Statuses.ToArray();
            foreach (var status in statusArray)
            {
                status.Value.TryRemoveStatus();
            }

            //If there is a removal of a status, update it client sided.
            if (Statuses.Count != statusArray.Count())
            {
                PacketSender.SendEntityVitals(this);
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

            //if (MoveTimer > Globals.System.GetTimeMs()) return -5;
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

            if (tile.Translate(xOffset, yOffset))
            {
                var tileAttribute = MapInstance.Get(tile.GetMapId()).Attributes[tile.GetX(), tile.GetY()];
                if (tileAttribute != null)
                {
                    if (tileAttribute.Type == MapAttributes.Blocked)
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
                var targetMap = MapInstance.Get(tile.GetMapId());
                var mapEntities = MapInstance.Get(tile.GetMapId()).GetEntities();
                for (var i = 0; i < mapEntities.Count; i++)
                {
                    var en = mapEntities[i];
                    if (en != null && en.X == tile.GetX() && en.Y == tile.GetY() && en.Z == Z && !en.Passable)
                    {
                        //Set a target if a projectile
                        CollisionIndex = en.Id;
                        if (en is Player)
                        {
                            if (this is Player)
                            {
                                //Check if this target player is passable....
                                if (!Options.Instance.Passability.Passable[(int) targetMap.ZoneType])
                                {
                                    return (int) EntityTypes.Player;
                                }
                            }
                            else
                            {
                                return (int) EntityTypes.Player;
                            }
                        }
                        else if (en is Npc)
                        {
                            return (int) EntityTypes.Player;
                        }
                        else if (en is Resource resource)
                        {
                            //If determine if we should walk
                            if (!resource.IsPassable())
                            {
                                return (int) EntityTypes.Resource;
                            }
                        }
                    }
                }

                //If this is an npc or other event.. if any global page exists that isn't passable then don't walk here!
                if (!(this is Player))
                {
                    foreach (var evt in MapInstance.Get(tile.GetMapId()).GlobalEventInstances)
                    {
                        foreach (var en in evt.Value.GlobalPageInstance)
                        {
                            if (en != null && en.X == tile.GetX() && en.Y == tile.GetY() && en.Z == Z && !en.Passable)
                            {
                                return (int) EntityTypes.Event;
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
                                lookDir = (int) Directions.Down;

                                break;
                            case (int) Directions.Right:
                                lookDir = (int) Directions.Up;

                                break;
                        }

                        ChangeDir(lookDir);
                        moved = true;

                        break;
                    case MoveRouteEnum.Turn90CounterClockwise:
                        switch (Dir)
                        {
                            case (int) Directions.Up:
                                lookDir = (int) Directions.Left;

                                break;
                            case (int) Directions.Down:
                                lookDir = (int) Directions.Right;

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
            if (Globals.Timing.Milliseconds <= MoveTimer || CastTime > 0)
            {
                return;
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
                case 4: //NW
                    --yOffset;
                    --xOffset;

                    break;
                case 5: //NE
                    --yOffset;
                    ++xOffset;

                    break;
                case 6: //SW
                    ++yOffset;
                    --xOffset;

                    break;
                case 7: //SE
                    ++yOffset;
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

                }

                MapId = tile.GetMapId();

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
                            var projectiles = map.MapProjectiles.ToArray();
                            foreach (var projectile in projectiles)
                            {
                                var spawns = projectile.Spawns?.ToArray() ?? new ProjectileSpawn[0];
                                foreach (var spawn in spawns)
                                {
                                    // TODO: Filter in Spawns variable, there should be no nulls. See #78 for evidence it is null.
                                    if (spawn == null)
                                    {
                                        continue;
                                    }

                                    if (spawn.IsAtLocation(MapId, X, Y, Z) && spawn.HitEntity(this))
                                    {
                                        projectile.KillSpawn(spawn);
                                    }
                                }
                            }
                        }
                    }

                    MoveTimer = Globals.Timing.Milliseconds + (long) GetMovementTime();
                }

                if (TryToChangeDimension() && doNotUpdate == true)
                {
                    PacketSender.UpdateEntityZDimension(this, (byte) Z);
                }

                //Check for traps
                if (currentMap != null)
                {
                    lock (currentMap.GetMapLock())
                    {
                        foreach (var trap in currentMap.MapTraps)
                        {
                            trap.CheckEntityHasDetonatedTrap(this);
                        }
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
                    if (((MapSlideAttribute) attribute).Direction > 0)
                    {
                        //Check for slide tiles
                        if (attribute != null && attribute.Type == MapAttributes.Slide)
                        {
                            if (((MapSlideAttribute) attribute).Direction > 0)
                            {
                                Dir = (byte) (((MapSlideAttribute) attribute).Direction - 1);
                            }
                        }
                    }

                    var dash = new Dash(this, 1, (byte) Dir);
                }
            }
        }

        public void ChangeDir(int dir)
        {
            if (dir == -1)
            {
                return;
            }

            Dir = dir;
            if (this is EventPageInstance eventPageInstance && eventPageInstance.Player != null)
            {
                if (((EventPageInstance) this).Player != null)
                {
                    PacketSender.SendEntityDirTo(((EventPageInstance) this).Player, this);
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

        public virtual void SendStatUpdate(int index)
        {
            PacketSender.SendEntityStats(this);
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
            return _vital[vital];
        }

        [NotNull]
        public int[] GetVitals()
        {
            var vitals = new int[(int) Vitals.VitalCount];
            Array.Copy(_vital, 0, vitals, 0, (int) Vitals.VitalCount);

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

            _vital[vital] = value;
            PacketSender.SendEntityVitals(this);
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

        [NotNull]
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

            PacketSender.SendEntityVitals(this);
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
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
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

        //Stats
        public virtual int GetStatBuffs(Stats statType)
        {
            return 0;
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

        public virtual bool IsAllyOf([NotNull] Entity otherEntity)
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
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
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

            //Check if the target is blocking facing in the direction against you
            if (target.Blocking)
            {
                var d = Dir;

                if (projectile != null)
                {
                    d = projectileDir;
                }

                if (target.Dir == (int) Directions.Left && d == (int) Directions.Right)
                {
                    PacketSender.SendActionMsg(target, Strings.Combat.blocked, CustomColors.Combat.Blocked);

                    return;
                }
                else if (target.Dir == (int) Directions.Right && d == (int) Directions.Left)
                {
                    PacketSender.SendActionMsg(target, Strings.Combat.blocked, CustomColors.Combat.Blocked);

                    return;
                }
                else if (target.Dir == (int) Directions.Up && d == (int) Directions.Down)
                {
                    PacketSender.SendActionMsg(target, Strings.Combat.blocked, CustomColors.Combat.Blocked);

                    return;
                }
                else if (target.Dir == (int) Directions.Down && d == (int) Directions.Up)
                {
                    PacketSender.SendActionMsg(target, Strings.Combat.blocked, CustomColors.Combat.Blocked);

                    return;
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
                foreach (EventBase evt in EventBase.Lookup.Values)
                {
                    if (evt != null)
                    {
                        targetPlayer.StartCommonEvent(evt, CommonEventTrigger.PlayerInteract, "", this.Name);
                    }
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

            if (targetPlayer == null && !(target is Npc))
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
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
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
                    if (player.InParty(targetPlayer))
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
                        when this is Player player && !player.InParty(targetPlayer) && this != target:
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
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                target.Stat[i]
                    .AddBuff(
                        new Buff(
                            spellBase,
                            spellBase.Combat.StatDiff[i] +
                            (int) ((target.Stat[i].BaseStat + target.StatPointAllocations[i]) *
                                   (spellBase.Combat.PercentageStatDiff[i] / 100f)), spellBase.Combat.Duration
                        )
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

            var damageHealth = spellBase.Combat.VitalDiff[0];
            var damageMana = spellBase.Combat.VitalDiff[1];

            if ((spellBase.Combat.Effect != StatusTypes.OnHit || onHitTrigger) &&
                spellBase.Combat.Effect != StatusTypes.Shield)
            {
                Attack(
                    target, damageHealth, damageMana, (DamageType) spellBase.Combat.DamageType,
                    (Stats) spellBase.Combat.ScalingStat, spellBase.Combat.Scaling, spellBase.Combat.CritChance,
                    spellBase.Combat.CritMultiplier, deadAnimations, aliveAnimations
                );
            }

            if (spellBase.Combat.Effect > 0) //Handle status effects
            {
                //Check for onhit effect to avoid the onhit effect recycling.
                if (!(onHitTrigger && spellBase.Combat.Effect == StatusTypes.OnHit))
                {
                    new Status(
                        target, spellBase, spellBase.Combat.Effect, spellBase.Combat.Duration,
                        spellBase.Combat.TransformSprite
                    );

                    PacketSender.SendActionMsg(
                        target, Strings.Combat.status[(int) spellBase.Combat.Effect], CustomColors.Combat.Status
                    );

                    //Set the enemies target if a taunt spell
                    if (spellBase.Combat.Effect == StatusTypes.Taunt)
                    {
                        target.Target = this;
                        if (target is Player targetPlayer)
                        {
                            PacketSender.SetPlayerTarget((Player) target, Id);
                        }
                    }

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
                    new Status(target, spellBase, spellBase.Combat.Effect, statBuffTime, "");
                }
            }

            //Handle DoT/HoT spells]
            if (spellBase.Combat.HoTDoT)
            {
                var doTFound = false;
                for (var i = 0; i < target.DoT.Count; i++)
                {
                    if (target.DoT[i].SpellBase.Id == spellBase.Id && target.DoT[i].Target == this)
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

        private void Animate([NotNull] Entity target, [NotNull] List<KeyValuePair<Guid, sbyte>> animations)
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
            if (AttackTimer > Globals.Timing.Milliseconds || Blocking)
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
                foreach (EventBase evt in EventBase.Lookup.Values)
                {
                    if (evt != null)
                    {
                        targetPlayer.StartCommonEvent(evt, CommonEventTrigger.PlayerInteract, "", this.Name);
                    }
                }

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
            var statusList = Statuses.Values.ToArray();
            foreach (var status in statusList)
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
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
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

            //If we took damage lets reset our combat timer
            target.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
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
            var damagingAttack = baseDamage > 0;
            if (enemy == null)
            {
                return;
            }

            //Check for enemy statuses
            var statuses = enemy.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                //Invulnerability ignore
                if (status.Type == StatusTypes.Invulnerable)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Combat.invulnerable, CustomColors.Combat.Invulnerable);

                    // Add a timer before able to make the next move.
                    if (this is Npc npc)
                    {
                        npc.MoveTimer = Globals.Timing.Milliseconds + (long) GetMovementTime();
                    }

                    return;
                }
            }

            //Is this a critical hit?
            if (Randomization.Next(1, 101) > critChance)
            {
                critMultiplier = 1;
            }
            else
            {
                PacketSender.SendActionMsg(enemy, Strings.Combat.critical, CustomColors.Combat.Critical);
            }

            //Calculate Damages
            if (baseDamage != 0)
            {
                baseDamage = Formulas.CalculateDamage(
                    baseDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy
                );

                if (baseDamage < 0 && damagingAttack)
                {
                    baseDamage = 0;
                }

                if (baseDamage > 0 && enemy.HasVital(Vitals.Health))
                {
                    enemy.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
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

                    foreach (var status in statuses)
                    {
                        //Wake up any sleeping targets
                        if (status.Type == StatusTypes.Sleep)
                        {
                            status.RemoveStatus();
                        }

                        //Remove stealth from any stealthy targets
                        if (status.Type == StatusTypes.Stealth)
                        {
                            status.RemoveStatus();
                        }
                    }

                    //No Matter what, if we attack the entity, make them chase us
                    if (enemy is Npc enemyNpc)
                    {
                        var dmgMap = enemyNpc.DamageMap;
                        dmgMap.TryGetValue(this, out var damage);
                        dmgMap[this] = damage + baseDamage;

                        if (enemyNpc.Base.FocusHighestDamageDealer)
                        {
                            enemyNpc.AssignTarget(enemyNpc.DamageMapHighest);
                        }
                        else
                        {
                            enemyNpc.AssignTarget(this);
                        }
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

                if (secondaryDamage > 0 && enemy.HasVital(Vitals.Mana))
                {
                    //If we took damage lets reset our combat timer
                    enemy.CombatTimer = Globals.Timing.Milliseconds + Options.CombatTime;
                    enemy.SubVital(Vitals.Mana, (int) secondaryDamage);
                    PacketSender.SendActionMsg(
                        enemy, Strings.Combat.removesymbol + (int) secondaryDamage, CustomColors.Combat.RemoveMana
                    );

                    //No Matter what, if we attack the entitiy, make them chase us
                    if (enemy is Npc enemyNpc)
                    {
                        if (enemyNpc.Base.FocusHighestDamageDealer)
                        {
                            enemyNpc.AssignTarget(enemyNpc.DamageMapHighest);
                        }
                        else
                        {
                            enemyNpc.AssignTarget(this);
                        }
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

                    PacketSender.SendEntityVitals(this);
                }
            }

            //Dead entity check
            if (enemy.GetVital(Vitals.Health) <= 0)
            {
                KilledEntity(enemy);
                if (enemy.GetType() == typeof(Npc) || enemy.GetType() == typeof(Resource))
                {
                    enemy.Die(100, this);
                }
                else
                {
                    enemy.Die(Options.ItemDropChance);

                    //PVP Kill common events
                    if (this.GetType() == typeof(Player))
                    {
                        if (MapInstance.Get(MapId).ZoneType != MapZones.Arena)
                        {
                            foreach (EventBase evt in EventBase.Lookup.Values)
                            {
                                if (evt != null)
                                {
                                    ((Player) this).StartCommonEvent(evt, CommonEventTrigger.PVPKill, "", enemy.Name);
                                    ((Player) enemy).StartCommonEvent(evt, CommonEventTrigger.PVPDeath, "", this.Name);
                                }
                            }
                        }
                    }
                }

                if (deadAnimations != null)
                {
                    foreach (var anim in deadAnimations)
                    {
                        PacketSender.SendAnimationToProximity(
                            anim.Key, -1, Guid.Empty, enemy.MapId, (byte) enemy.X, (byte) enemy.Y, anim.Value
                        );
                    }
                }
            }
            else
            {
                //Hit him, make him mad and send the vital update.
                PacketSender.SendEntityVitals(enemy);
                PacketSender.SendEntityStats(enemy);
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
                foreach (var status in this.Statuses.Values.ToArray())
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
                            foreach (var status in CastTarget.Statuses.Values.ToArray())
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
                                    this, spellBase, StatusTypes.OnHit, spellBase.Combat.OnHitDuration,
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
                            PacketSender.SendAnimationToProximity(
                                spellBase.HitAnimationId, -1, Guid.Empty, MapId, (byte) X, (byte) Y, 0
                            );

                            break;
                        default:
                            break;
                    }

                    break;
                case SpellTypes.Warp:
                    if (this is Player)
                    {
                        Warp(
                            spellBase.Warp.MapId, (byte) spellBase.Warp.X, (byte) spellBase.Warp.Y,
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
                decimal cooldownReduction = 1;

                var thisPlayer = this as Player;

                if (thisPlayer != null) //Only apply cdr for players with equipment
                {
                    cooldownReduction = 1 - thisPlayer.GetCooldownReduction() / 100;
                }

                SpellCooldowns[Spells[spellSlot].SpellId] =
                    Globals.Timing.MillisecondsUTC + (int)(spellBase.CooldownDuration * cooldownReduction);

                if (thisPlayer != null)
                {
                    PacketSender.SendSpellCooldown(thisPlayer, Spells[spellSlot].SpellId);
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
                        foreach (var pair in map.GetEntitiesDictionary())
                        {
                            var entity = pair.Value;
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
                                                Warp(
                                                    spellTarget.MapId, (byte)spellTarget.X, (byte)spellTarget.Y,
                                                    (byte)Dir
                                                ); //Spelltarget used to be Target. I don't know if this is correct or not.
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

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        protected bool IsOneBlockAway(Entity target)
        {
            var myTile = new TileHelper(MapId, X, Y);
            var enemyTile = new TileHelper(target.MapId, target.X, target.Y);
            if (Z == target.Z)
            {
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile))
                {
                    return true;
                }

                myTile.Translate(2, 0);
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

        protected int GetDistanceTo(Entity target)
        {
            if (target != null)
            {
                return GetDistanceTo(target.Map, target.X, target.Y);
            }
            //Something is null.. return a value that is out of range :) 
            return 9999;
        }

        protected int GetDistanceTo(MapInstance targetMap, int targetX, int targetY)
        {
            var myMap = MapInstance.Get(MapId);
            if (myMap != null && targetMap != null && myMap.MapGrid == targetMap.MapGrid
            ) //Make sure both maps exist and that they are in the same dimension
            {
                //Calculate World Tile of Me
                var x1 = X + myMap.MapGridX * Options.MapWidth;
                var y1 = Y + myMap.MapGridY * Options.MapHeight;

                //Calculate world tile of target
                var x2 = targetX + targetMap.MapGridX * Options.MapWidth;
                var y2 = targetY + targetMap.MapGridY * Options.MapHeight;

                return (int) Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
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

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
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
        public virtual void Die(int dropitems = 0, Entity killer = null)
        {
            if (Items == null)
            {
                return;
            }

            if (dropitems > 0)
            {
                // Drop items
                for (var n = 0; n < Items.Count; n++)
                {
                    var item = Items[n];
                    if (item == null)
                    {
                        continue;
                    }

                    var itemBase = ItemBase.Get(item.ItemId);
                    if (itemBase == null)
                    {
                        continue;
                    }

                    //Don't lose bound items on death for players.
                    if (this.GetType() == typeof(Player))
                    {
                        if (itemBase.Bound)
                        {
                            continue;
                        }
                    }

                    //Calculate the killers luck (If they are a player)
                    var playerKiller = killer as Player;
                    var luck = 1.0 + (playerKiller != null ? playerKiller.GetLuck() : 0) / 100;

                    //Player drop rates
                    if (Randomization.Next(1, 101) >= dropitems * luck)
                    {
                        continue;
                    }

                    //Npc drop rates
                    if (Randomization.Next(1, 101) >= item.DropChance * luck)
                    {
                        continue;
                    }

                    // Decide if we want to have a loot ownership timer or not.
                    Guid lootOwner = Guid.Empty;
                    if (this is Npc thisNpc)
                    {
                        // Check if we have someone that tagged this NPC.
                        var taggedBy = thisNpc.DamageMapHighest;
                        if (taggedBy != null && taggedBy is Player)
                        {
                            // Spawn with ownership!
                            lootOwner = taggedBy.Id;
                        }
                    } 
                    else
                    {
                        // There's no tracking of who damaged what player as of now, so going by last hit.. Or set ownership to the player themselves.
                        lootOwner = playerKiller?.Id ?? Id;
                    }

                    // Spawn the actual item!
                    var map = MapInstance.Get(MapId);
                    map?.SpawnItem(X, Y, item, item.Quantity, lootOwner);

                    // Remove the item from inventory if a player.
                    var player = this as Player;
                    player?.TryTakeItem(Items[n], item.Quantity);
                }
            }

            var currentMap = MapInstance.Get(MapId);
            if (currentMap != null)
            {
                currentMap.ClearEntityTargetsOf(this);
                currentMap.GetSurroundingMaps()?.ForEach(map => map?.ClearEntityTargetsOf(this));
            }

            DoT?.Clear();
            Statuses?.Clear();
            Stat?.ToList().ForEach(stat => stat?.Reset());

            PacketSender.SendEntityVitals(this);
            Dead = true;
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
        public virtual void Warp(Guid newMapId, byte newX, byte newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, (byte) Dir, adminWarp);
        }

        public virtual void Warp(
            Guid newMapId,
            byte newX,
            byte newY,
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
                packet = new EntityPacket();
            }

            packet.EntityId = Id;
            packet.MapId = MapId;
            packet.Name = Name;
            packet.Sprite = Sprite;
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
            var statuses = Statuses.Values.ToArray();
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
            set => SpellCooldowns = JsonConvert.DeserializeObject<Dictionary<Guid, long>>(value ?? "{}");
        }

        [NotMapped] public Dictionary<Guid, long> SpellCooldowns = new Dictionary<Guid, long>();

        #endregion

    }

}

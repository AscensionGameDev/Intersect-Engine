using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Server.Classes.Localization;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.General;

using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.Server.Classes.Entities
{
    using LegacyDatabase = Intersect.Server.Classes.Core.LegacyDatabase;

    public class EntityInstance
    {
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

        [Column("Vitals")]
        public string VitalsJson
        {
            get => DatabaseUtils.SaveIntArray(_vital, (int)Enums.Vitals.VitalCount);
            set => _vital = DatabaseUtils.LoadIntArray(value, (int)Enums.Vitals.VitalCount);
        }
        [NotMapped]
        public int[] _vital { get; set; } = new int[(int)Enums.Vitals.VitalCount];


        [Column("MaxVitals")]
        public string MaxVitalsJson
        {
            get => DatabaseUtils.SaveIntArray(_maxVital, (int)Enums.Vitals.VitalCount);
            set => _maxVital = DatabaseUtils.LoadIntArray(value, (int)Enums.Vitals.VitalCount);
        }
        [NotMapped]
        private int[] _maxVital = new int[(int)Vitals.VitalCount];


        [Column("Stats")]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(BaseStat, (int)Enums.Stats.StatCount);
            set => BaseStat = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] BaseStat { get; set; } = new int[(int)Enums.Stats.StatCount];

        //Inventory
        public virtual List<InventorySlot> Items { get; set; } = new List<InventorySlot>();

        //Spells
        public virtual List<SpellSlot> Spells { get; set; } = new List<SpellSlot>();

        [NotMapped]
        public EntityStat[] Stat = new EntityStat[(int)Stats.StatCount];


        //Instance Values
        [NotMapped] public Guid Id;
        [NotMapped] public bool Dead;
        
        //Combat
        [NotMapped] public long CastTime;
        [NotMapped] public long AttackTimer;
        [NotMapped] public bool Blocking;
        [NotMapped] public EntityInstance CastTarget;
        [NotMapped] public Guid CollisionIndex;
        [NotMapped] public long CombatTimer;

        //Visuals
        [NotMapped] public int HideName = 0;
        [NotMapped] public List<Guid> Animations = new List<Guid>();

        //DoT/HoT Spells
        [NotMapped] public List<DoTInstance> DoT = new List<DoTInstance>();
        [NotMapped] public EventMoveRoute MoveRoute = null;
        [NotMapped] public EventPageInstance MoveRouteSetter = null;
        [NotMapped] public long MoveTimer;
        [NotMapped] public int Passable = 0;
        [NotMapped] public long RegenTimer = Globals.System.GetTimeMs();
        [NotMapped] public int SpellCastSlot = 0;





        //Status effects
        [NotMapped] public Dictionary<SpellBase, StatusInstance> Statuses = new Dictionary<SpellBase, StatusInstance>();
        
        [NotMapped] public EntityInstance Target = null;

        [NotMapped] public bool IsDisposed { get; private set; }

        public EntityInstance() : this(Guid.NewGuid())
        {
            
        }

        //Initialization
        public EntityInstance(Guid instanceId)
        {
            for (var I = 0; I < (int) Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(I,this,null);
            }

            Id = instanceId;
        }

        public void Dispose()
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
            if (Globals.System.GetTimeMs() > CombatTimer && Globals.System.GetTimeMs() > RegenTimer)
            {
                ProcessRegen();
                RegenTimer = Globals.System.GetTimeMs() + Options.RegenTime;
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

        public virtual void ProcessRegen()
        {
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
                var tileAttribute = MapInstance.Lookup.Get<MapInstance>(tile.GetMapId())
                    .Attributes[tile.GetX(), tile.GetY()];
                if (tileAttribute != null)
                {
                    if (tileAttribute.Value == (int) MapAttributes.Blocked) return -2;
                    if (tileAttribute.Value == (int) MapAttributes.NpcAvoid && GetType() == typeof(Npc)) return -2;
                    if (tileAttribute.Value == (int) MapAttributes.ZDimension && tileAttribute.Data2 > 0 &&
                        tileAttribute.Data2 - 1 == Z) return -3;
                    if (tileAttribute.Value == (int) MapAttributes.Slide)
                    {
                        if (this.GetType() == typeof(EventPageInstance)) return -4;
                        switch (tileAttribute.Data1)
                        {
                            case 1:
                                if (moveDir == 1) return -4;
                                break;
                            case 2:
                                if (moveDir == 0) return -4;
                                break;
                            case 3:
                                if (moveDir == 3) return -4;
                                break;
                            case 4:
                                if (moveDir == 2) return -4;
                                break;
                        }
                    }
                }
            }
            else
            {
                return -5; //Out of Bounds
            }

            if (Passable == 0)
            {
                var targetMap = MapInstance.Lookup.Get<MapInstance>(tile.GetMapId());
                var mapEntities = MapInstance.Lookup.Get<MapInstance>(tile.GetMapId()).GetEntities();
                for (var i = 0; i < mapEntities.Count; i++)
                {
                    var en = mapEntities[i];
                    if (en != null && en.X == tile.GetX() && en.Y == tile.GetY() && en.Z == Z &&
                        en.Passable == 0)
                    {
                        //Set a target if a projectile
                        CollisionIndex = en.Id;
                        if (en.GetType() == typeof(Player))
                        {
                            if (this.GetType() == typeof(Player))
                            {
                                //Check if this target player is passable....
                                if (!Options.PlayerPassable[(int) targetMap.ZoneType]) return (int) EntityTypes.Player;
                            }
                            else
                            {
                                return (int)EntityTypes.Player;
                            }
                        }
                        else if (en.GetType() == typeof(Npc))
                        {
                            return (int) EntityTypes.Player;
                        }
                        else if (en.GetType() == typeof(Resource))
                        {
                            //If determine if we should walk
                            var res = ((Resource) en);
                            if ((!res.IsDead() && !res.Base.WalkableBefore) ||
                                (res.IsDead() && !res.Base.WalkableAfter))
                            {
                                return (int) EntityTypes.Resource;
                            }
                        }
                    }
                }
                //If this is an npc or other event.. if any global page exists that isn't passable then don't walk here!
                if (this.GetType() != typeof(Player))
                {
                    foreach (var evt in MapInstance.Lookup.Get<MapInstance>(tile.GetMapId()).GlobalEventInstances)
                    {
                        foreach (var en in evt.Value.GlobalPageInstance)
                        {
                            if (en != null && en.X == tile.GetX() && en.Y == tile.GetY() &&
                                en.Z == Z &&
                                en.Passable == 0)
                            {
                                return (int) EntityTypes.Event;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        protected virtual bool ProcessMoveRoute(Client client, long timeMs)
        {
            var moved = false;
            int lookDir = 0, moveDir = 0;
            if (MoveRoute.ActionIndex < MoveRoute.Actions.Count)
            {
                switch (MoveRoute.Actions[MoveRoute.ActionIndex].Type)
                {
                    case MoveRouteEnum.MoveUp:
                        if (CanMove((int) Directions.Up) == -1)
                        {
                            Move((int) Directions.Up, client, false, true);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveDown:
                        if (CanMove((int) Directions.Down) == -1)
                        {
                            Move((int) Directions.Down, client, false, true);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveLeft:
                        if (CanMove((int) Directions.Left) == -1)
                        {
                            Move((int) Directions.Left, client, false, true);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveRight:
                        if (CanMove((int) Directions.Right) == -1)
                        {
                            Move((int) Directions.Right, client, false, true);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.MoveRandomly:
                        var dir = Globals.Rand.Next(0, 4);
                        if (CanMove(dir) == -1)
                        {
                            Move(dir, client);
                            moved = true;
                        }
                        break;
                    case MoveRouteEnum.StepForward:
                        if (CanMove(Dir) > -1)
                        {
                            Move(Dir, client);
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
                            Move(moveDir, client);
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
                        ChangeDir(Globals.Rand.Next(0, 4));
                        moved = true;
                        break;
                    case MoveRouteEnum.Wait100:
                        MoveTimer = Globals.System.GetTimeMs() + 100;
                        moved = true;
                        break;
                    case MoveRouteEnum.Wait500:
                        MoveTimer = Globals.System.GetTimeMs() + 500;
                        moved = true;
                        break;
                    case MoveRouteEnum.Wait1000:
                        MoveTimer = Globals.System.GetTimeMs() + 1000;
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
                        if (MoveRoute.RepeatRoute) MoveRoute.ActionIndex = 0;
                        MoveRoute.Complete = true;
                    }
                }
                if (MoveTimer < Globals.System.GetTimeMs())
                {
                    MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
                }
            }
            return true;
        }

        //Returns the amount of time required to traverse 1 tile
        public virtual float GetMovementTime()
        {
            var time = 1000f / (float) (1 + Math.Log(Stat[(int) Stats.Speed].Value()));
            if (Blocking)
            {
                time += time * (float) Options.BlockingSlow;
            }
            return Math.Min(1000f, time);
        }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public virtual void Move(int moveDir, Client client, bool dontUpdate = false, bool correction = false)
        {
            var xOffset = 0;
            var yOffset = 0;
            Dir = moveDir;
            if (MoveTimer < Globals.System.GetTimeMs() && CastTime <= 0)
            {
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
                    X = tile.GetX();
                    Y = tile.GetY();
                    if (MapId != tile.GetMapId())
                    {
                        var oldMap = MapInstance.Lookup.Get<MapInstance>(MapId);
                        if (oldMap != null) oldMap.RemoveEntity(this);
                        var newMap = MapInstance.Lookup.Get<MapInstance>(tile.GetMapId());
                        if (newMap != null) newMap.AddEntity(this);
                    }
                    MapId = tile.GetMapId();
                    if (dontUpdate == false)
                    {
                        if (GetType() == typeof(EventPageInstance))
                        {
                            if (client != null)
                            {
                                PacketSender.SendEntityMoveTo(client, this, correction);
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
                        var myMap = MapInstance.Lookup.Get<MapInstance>(MapId);
                        if (myMap != null)
                        {
                            var localMaps = myMap.GetSurroundingMaps(true);
                            foreach (var map in localMaps)
                            {
                                var projectiles = map.MapProjectiles;
                                foreach (var projectile in projectiles)
                                {
                                    if (projectile.GetType() == typeof(Projectile))
                                    {
                                        var proj = projectile;
                                        foreach (var spawn in proj.Spawns)
                                        {
                                            if (spawn != null && spawn.MapId == MapId && spawn.X == X &&
                                                spawn.Y == Y && spawn.Z == Z)
                                            {
                                                if (spawn.HitEntity(this))
                                                {
                                                    spawn.Parent.KillSpawn(spawn);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
                    }
                    if (TryToChangeDimension() && dontUpdate == true)
                    {
                        PacketSender.UpdateEntityZDimension(this, Z);
                    }
                    var attribute = MapInstance.Lookup.Get<MapInstance>(MapId).Attributes[X, Y];
                    if (this.GetType() != typeof(EventPageInstance))
                    {
                        //Check for slide tiles
                        if (attribute != null && attribute.Value == (int)MapAttributes.Slide)
                        {
                            if (attribute.Data1 > 0)
                            {
                                Dir = attribute.Data1 - 1;
                            } //If sets direction, set it.
                            var dash = new DashInstance(this, 1, Dir);
                        }
                    }
                }
            }
        }

        public void ChangeDir(int dir)
        {
            Dir = dir;
            if (GetType() == typeof(EventPageInstance))
            {
                if (((EventPageInstance) this).Client != null)
                {
                    PacketSender.SendEntityDirTo(((EventPageInstance) this).Client, Id, (int) EntityTypes.Event, Dir, MapId);
                }
                else
                {
                    PacketSender.SendEntityDir(Id, (int) EntityTypes.Event, Dir, MapId);
                }
            }
            else
            {
                PacketSender.SendEntityDir(Id, (int) EntityTypes.GlobalEntity, Dir, MapId);
            }
        }

        // Change the dimension if the player is on a gateway
        public bool TryToChangeDimension()
        {
            if (X < Options.MapWidth && X >= 0)
            {
                if (Y < Options.MapHeight && Y >= 0)
                {
                    var attribute = MapInstance.Lookup.Get<MapInstance>(MapId)
                        .Attributes[X, Y];
                    if (attribute != null && attribute.Value == (int) MapAttributes.ZDimension)
                    {
                        if (attribute.Data1 > 0)
                        {
                            Z = attribute.Data1 - 1;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Misc
        public int GetDirectionTo(EntityInstance target)
        {
            int xDiff = 0, yDiff = 0;
            var myGrid = MapInstance.Lookup.Get<MapInstance>(MapId).MapGrid;
            //Loop through surrouding maps to generate a array of open and blocked points.
            for (var x = MapInstance.Lookup.Get<MapInstance>(MapId).MapGridX - 1;
                x <= MapInstance.Lookup.Get<MapInstance>(MapId).MapGridX + 1;
                x++)
            {
                if (x == -1 || x >= LegacyDatabase.MapGrids[myGrid].Width) continue;
                for (var y = MapInstance.Lookup.Get<MapInstance>(MapId).MapGridY - 1;
                    y <= MapInstance.Lookup.Get<MapInstance>(MapId).MapGridY + 1;
                    y++)
                {
                    if (y == -1 || y >= LegacyDatabase.MapGrids[myGrid].Height) continue;
                    if (LegacyDatabase.MapGrids[myGrid].MyGrid[x, y] != Guid.Empty &&
                        LegacyDatabase.MapGrids[myGrid].MyGrid[x, y] == target.MapId)
                    {
                        xDiff = (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridX - x) * Options.MapWidth +
                                target.X -
                                X;
                        yDiff = (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridY - y) * Options.MapHeight +
                                target.Y -
                                Y;
                        if (Math.Abs(xDiff) > Math.Abs(yDiff))
                        {
                            if (xDiff < 0) return (int) Directions.Left;
                            if (xDiff > 0) return (int) Directions.Right;
                        }
                        else
                        {
                            if (yDiff < 0) return (int) Directions.Up;
                            if (yDiff > 0) return (int) Directions.Down;
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
        public int CalculateAttackTime()
        {
            return
                (int)
                (Options.MaxAttackRate +
                 (float)
                 ((Options.MinAttackRate - Options.MaxAttackRate) *
                  (((float) Options.MaxStatValue - Stat[(int) Stats.Speed].Value()) / (float) Options.MaxStatValue)));
        }

        public void TryBlock(int blocking)
        {
            if (AttackTimer < Globals.System.GetTimeMs())
            {
                if (blocking == 1 && !Blocking && AttackTimer < Globals.System.GetTimeMs())
                {
                    Blocking = true;
                    PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, MapId, -1);
                }
                else if (blocking == 0 && Blocking)
                {
                    Blocking = false;
                    AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
                    PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, MapId, 0);
                }
            }
        }

        public virtual int GetWeaponDamage()
        {
            return 0;
        }

        public virtual bool CanAttack(EntityInstance en, SpellBase spell)
        {
            if (CastTime > 0) return false;
            return true;
        }

        public int GetVital(int vital)
        {
            return _vital[vital];
        }
        public int GetVital(Vitals vital)
        {
            return GetVital((int) vital);
        }
        public void SetVital(int vital, int value)
        {
            if (value < 0) value = 0;
            if (GetMaxVital(vital) < value)
                value = GetMaxVital(vital);
            _vital[vital] = value;
            PacketSender.SendEntityVitals(this);
        }
        public void SetVital(Vitals vital, int value)
        {
            SetVital((int) vital, value);
        }
        public int GetMaxVital(int vital)
        {
            return _maxVital[vital];
        }
        public int GetMaxVital(Vitals vital)
        {
            return GetMaxVital((int)vital);
        }
        public void SetMaxVital(int vital, int value)
        {
            if (value <= 0 && vital == (int)Vitals.Health) value = 1; //Must have at least 1 hp
            if (value < 0 && vital == (int)Vitals.Mana) value = 0; //Can't have less than 0 mana
            _maxVital[vital] = value;
            if (value < GetVital(vital))
                SetVital(vital,value);
            PacketSender.SendEntityVitals(this);
        }
        public void SetMaxVital(Vitals vital, int value)
        {
            SetMaxVital((int)vital, value);
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
            if (vital >= Vitals.VitalCount) return;

            var vitalId = (int)vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, int.MaxValue - maxVitalValue);
            SetVital(vital,GetVital(vital) + safeAmount);
        }
        public void SubVital(Vitals vital, int amount)
        {
            if (vital >= Vitals.VitalCount) return;

            var vitalId = (int)vital;
            var maxVitalValue = GetMaxVital(vitalId);
            var safeAmount = Math.Min(amount, GetVital(vital));
            SetVital(vital, GetVital(vital) - safeAmount);
        }

        //Attacking with projectile
        public virtual void TryAttack(EntityInstance enemy, ProjectileBase projectile, SpellBase parentSpell,
            ItemBase parentItem, int projectileDir)
        {
            if (enemy.GetType() == typeof(Resource) && parentSpell != null) return;

            //Check if the target is blocking facing in the direction against you
            if (enemy.Blocking)
            {
                var d = Dir;

                if (projectile != null)
                {
                    d = projectileDir;
                }

                if (enemy.Dir == (int) Directions.Left && d == (int) Directions.Right)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Combat.blocked, CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Right && d == (int) Directions.Left)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Combat.blocked, CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Up && d == (int) Directions.Down)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Combat.blocked, CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Down && d == (int) Directions.Up)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Combat.blocked, CustomColors.Blocked);
                    return;
                }
            }

            if (parentSpell != null)
            {
                TryAttack(enemy, parentSpell);
            }
            else
            {
                if (GetType() == typeof(Player) && enemy.GetType() == typeof(Player))
                {
                    if (MapInstance.Lookup.Get<MapInstance>(MapId).ZoneType == MapZones.Safe)
                    {
                        return;
                    }
                    if (MapInstance.Lookup.Get<MapInstance>(enemy.MapId).ZoneType == MapZones.Safe)
                    {
                        return;
                    }
                    if (((Player) this).InParty((Player) enemy) == true) return;
                }
                Attack(enemy, parentItem.Damage, 0, (DamageType) parentItem.DamageType, (Stats) parentItem.ScalingStat,
                    parentItem.Scaling, parentItem.CritChance, Options.CritMultiplier);
            }

            //If projectile, check if a splash spell is applied
            if (projectile != null)
            {
                if (projectile.SpellId != Guid.Empty)
                {
                    var s = projectile.Spell;
                    if (s != null)
                        HandleAoESpell(projectile.SpellId, s.HitRadius, enemy.MapId, enemy.X, enemy.Y,
                            null);

                    //Check that the npc has not been destroyed by the splash spell
                    //TODO: Actually implement this, since null check is wrong.
                    if (enemy == null)
                    {
                        return;
                    }
                }
                if (enemy.GetType() == typeof(Player) || enemy.GetType() == typeof(Npc))
                {
                    if (projectile.Knockback > 0 && projectileDir < 4)
                        //If there is a knockback, knock them backwards and make sure its linear (diagonal player movement not coded).
                    {
                        var dash = new DashInstance(enemy, projectile.Knockback, projectileDir, false, false, false,
                            false);
                    }
                }
            }
        }

        //Attacking with spell
        public virtual void TryAttack(EntityInstance enemy, SpellBase spellBase)
        {
            if (enemy?.GetType() == typeof(Resource)) return;
            if (spellBase == null) return;

            var deadAnimations = new List<KeyValuePair<Guid, int>>();
            var aliveAnimations = new List<KeyValuePair<Guid, int>>();

            //Only count safe zones and friendly fire if its a dangerous spell! (If one has been used)
            if (spellBase.Friendly == 0 && spellBase.TargetType != (int) SpellTargetTypes.Self)
            {
                //If about to hit self with an unfriendly spell (maybe aoe?) return
                if (enemy == this) return;

                //Check for parties and safe zones, friendly fire off (unless its healing)
                if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
                {
                    if (((Player) this).InParty((Player) enemy) == true) return;
                }

                if (enemy.GetType() == typeof(Npc) && GetType() == typeof(Npc))
                {
                    if (!((Npc) this).CanNpcCombat(enemy, spellBase.Friendly == 1))
                    {
                        return;
                    }
                }

                //Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
                if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
                {
                    if (MapInstance.Lookup.Get<MapInstance>(MapId).ZoneType == MapZones.Safe)
                    {
                        return;
                    }
                    if (MapInstance.Lookup.Get<MapInstance>(enemy.MapId).ZoneType == MapZones.Safe)
                    {
                        return;
                    }
                }
            }
            else
            {
                //Friendly Spell! Do not attack other players/npcs around us.
                if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
                {
                    if (!((Player) this).InParty((Player) enemy) && this != enemy) return;
                }
                if (enemy.GetType() == typeof(Npc) && GetType() == typeof(Npc))
                {
                    if (!((Npc)this).CanNpcCombat(enemy, spellBase.Friendly == 1))
                    {
                        return;
                    }
                }
                if (enemy.GetType() != GetType()) return; //Don't let players aoe heal npcs. Don't let npcs aoe heal players.
            }

            if (spellBase.HitAnimationId != Guid.Empty)
            {
                deadAnimations.Add(new KeyValuePair<Guid, int>(spellBase.HitAnimationId, (int) Directions.Up));
                aliveAnimations.Add(new KeyValuePair<Guid, int>(spellBase.HitAnimationId, (int) Directions.Up));
            }

            var damageHealth = spellBase.VitalDiff[0];
            var damageMana = spellBase.VitalDiff[1];

            Attack(enemy, damageHealth, damageMana, (DamageType) spellBase.DamageType,
                (Stats) spellBase.ScalingStat,
                spellBase.Scaling, spellBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);

            var statBuffTime = -1;
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                enemy.Stat[i].AddBuff(new EntityBuff(spellBase, spellBase.StatDiff[i], spellBase.Data2));
                if (spellBase.StatDiff[i] != 0)
                    statBuffTime = spellBase.Data2;
            }

                //Handle other status effects
                if (spellBase.Data3 > 0)
                {
                    new StatusInstance(enemy, spellBase, spellBase.Data3, spellBase.Data2, spellBase.Data5);
                    PacketSender.SendActionMsg(enemy, Strings.Combat.status[spellBase.Data3], CustomColors.Status);
                }
                else
                {
                    if (statBuffTime > -1) new StatusInstance(enemy, spellBase, -1, statBuffTime, "");
                }

            //Handle DoT/HoT spells]
            if (spellBase.Data1 > 0)
            {
                var doTFound = false;
                for (var i = 0; i < enemy.DoT.Count; i++)
                {
                    if (enemy.DoT[i].SpellBase.Id == spellBase.Id ||
                        enemy.DoT[i].Target == this)
                    {
                        doTFound = true;
                    }
                }
                if (doTFound == false) //no duplicate DoT/HoT spells.
                {
                    new DoTInstance(this, spellBase.Id, enemy);
                }
            }
        }

        //Attacking with weapon or unarmed.
        public virtual void TryAttack(EntityInstance enemy)
        {
            //See player and npc override of this virtual void
        }

        //Attack using a weapon or unarmed
        public virtual void TryAttack(EntityInstance enemy, int baseDamage, DamageType damageType, Stats scalingStat, int scaling, int critChance, double critMultiplier, List<KeyValuePair<Guid, int>> deadAnimations = null, List<KeyValuePair<Guid, int>> aliveAnimations = null, ItemBase weapon = null)
        {
            if ((AttackTimer > Globals.System.GetTimeMs() || Blocking)) return;

            //Check for parties and safe zones, friendly fire off (unless its healing)
            if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
            {
                if (((Player) this).InParty((Player) enemy) == true) return;
            }

            //Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
            if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
            {
                if (MapInstance.Lookup.Get<MapInstance>(MapId).ZoneType == MapZones.Safe)
                {
                    return;
                }
                if (MapInstance.Lookup.Get<MapInstance>(enemy.MapId).ZoneType == MapZones.Safe)
                {
                    return;
                }
            }

            AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
            //Check if the attacker is blinded.
            if (IsOneBlockAway(enemy))
            {
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int) StatusTypes.Stun || status.Type == (int) StatusTypes.Blind)
                    {
                        PacketSender.SendActionMsg(this, Strings.Combat.miss, CustomColors.Missed);
                        PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, MapId,
                            CalculateAttackTime());
                        return;
                    }
                }
            }

            Attack(enemy, baseDamage, 0, damageType, scalingStat, scaling, critChance, critMultiplier, deadAnimations,
                aliveAnimations, weapon);

            //If we took damage lets reset our combat timer
            enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
        }

        public void Attack(EntityInstance enemy, int baseDamage, int secondaryDamage, DamageType damageType, Stats scalingStat,
            int scaling, int critChance, double critMultiplier, List<KeyValuePair<Guid, int>> deadAnimations = null,
            List<KeyValuePair<Guid, int>> aliveAnimations = null, ItemBase weapon = null)
        {
            if (enemy == null) return;

            //No Matter what, if we attack the entitiy, make them chase us
            if (enemy.GetType() == typeof(Npc))
            {
                if (((Npc) enemy).Base.Behavior == (int) NpcBehavior.Friendly) return;
                ((Npc) enemy).AssignTarget(this);

                //Check if there are any guards nearby
                //TODO Loop through CurrentMap - SurroundingMaps Entity List instead of global entity list.
                var mapEntities = MapInstance.Lookup.Get<MapInstance>(MapId).GetEntities();
                for (var n = 0; n < mapEntities.Count; n++)
                {
                    if (mapEntities[n].GetType() == typeof(Npc))
                    {
                        if (((Npc) mapEntities[n]).Behaviour == 3) // Type guard
                        {
                            var x = mapEntities[n].X - ((Npc) mapEntities[n]).Range;
                            var y = mapEntities[n].Y - ((Npc) mapEntities[n]).Range;
                            var xMax = mapEntities[n].X + ((Npc) mapEntities[n]).Range;
                            var yMax = mapEntities[n].Y + ((Npc) mapEntities[n]).Range;

                            //Check that not going out of the map boundaries
                            if (x < 0) x = 0;
                            if (y < 0) y = 0;
                            if (xMax >= Options.MapWidth) xMax = Options.MapWidth;
                            if (yMax >= Options.MapHeight) yMax = Options.MapHeight;

                            if (x < X && xMax > X)
                            {
                                if (y < Y && yMax > Y)
                                {
                                    // In range, so make a target
                                    ((Npc) mapEntities[n]).AssignTarget(this);
                                }
                            }
                        }
                    }
                }
            }

            //Is this a critical hit?
            if (Globals.Rand.Next(1, 101) > critChance)
            {
                critMultiplier = 1;
            }
            else
            {
                PacketSender.SendActionMsg(enemy, Strings.Combat.critical, CustomColors.Critical);
            }

            //Calculate Damages
            if (baseDamage != 0)
            {
                baseDamage = Formulas.CalculateDamage(baseDamage, damageType, scalingStat, scaling, critMultiplier, this, enemy);
                if (baseDamage > 0 && enemy.HasVital(Vitals.Health))
                {
                    enemy.SubVital(Vitals.Health, (int) baseDamage);
                    switch (damageType)
                    {
                        case DamageType.Physical:
                            PacketSender.SendActionMsg(enemy, Strings.Combat.removesymbol + (int) baseDamage,
                                CustomColors.PhysicalDamage);
                            break;
                        case DamageType.Magic:
                            PacketSender.SendActionMsg(enemy, Strings.Combat.removesymbol + (int) baseDamage,
                                CustomColors.MagicDamage);
                            break;
                        case DamageType.True:
                            PacketSender.SendActionMsg(enemy, Strings.Combat.removesymbol + (int) baseDamage,
                                CustomColors.TrueDamage);
                            break;
                    }
                    enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
                }
                else if (baseDamage < 0 && !enemy.IsFullVital(Vitals.Health))
                {
                    enemy.SubVital(Vitals.Health, (int)baseDamage);
                    PacketSender.SendActionMsg(enemy, Strings.Combat.addsymbol + (int) Math.Abs(baseDamage),CustomColors.Heal);
                }
            }
            if (secondaryDamage != 0)
            {
                secondaryDamage = Formulas.CalculateDamage(secondaryDamage, damageType, scalingStat, scaling,
                    critMultiplier, this, enemy);
                if (secondaryDamage > 0 && enemy.HasVital(Vitals.Mana))
                {
                    //If we took damage lets reset our combat timer
                    enemy.SubVital(Vitals.Mana, (int) secondaryDamage);
                    enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
                    PacketSender.SendActionMsg(enemy, Strings.Combat.removesymbol + (int)secondaryDamage,
                        CustomColors.RemoveMana);
                }
                else if (secondaryDamage < 0 && !enemy.IsFullVital(Vitals.Mana))
                {
                    enemy.SubVital(Vitals.Mana, (int)secondaryDamage);
                    PacketSender.SendActionMsg(enemy, Strings.Combat.addsymbol + (int) Math.Abs(secondaryDamage),CustomColors.AddMana);
                }
            }
			
			//Check for lifesteal
			if (GetType() == typeof(Player))
			{
				decimal lifesteal = ((Player)this).GetLifeSteal() / 100;
				decimal healthRecovered = lifesteal * baseDamage;
				if (healthRecovered > 0) //Don't send any +0 msg's.
				{
					AddVital(Vitals.Health, (int)healthRecovered);
					PacketSender.SendActionMsg(this, Strings.Combat.addsymbol + (int)healthRecovered, CustomColors.Heal);
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
                }
                if (deadAnimations != null)
                {
                    foreach (var anim in deadAnimations)
                    {
                        PacketSender.SendAnimationToProximity(anim.Key, -1, Guid.Empty, enemy.MapId, enemy.X,
                            enemy.Y, anim.Value);
                    }
                }
            }
            else
            {
                //Hit him, make him mad and send the vital update.
                PacketSender.SendEntityVitals(enemy);
                PacketSender.SendEntityStats(enemy);
                if (aliveAnimations != null)
                {
                    foreach (var anim in aliveAnimations)
                    {
                        PacketSender.SendAnimationToProximity(anim.Key, 1, enemy.Id, enemy.MapId, -1, -1,
                            anim.Value);
                    }
                }
            }
            // Add a timer before able to make the next move.
            if (GetType() == typeof(Npc))
            {
                ((Npc)this).MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
            }
        }

        public virtual void KilledEntity(EntityInstance en)
        {
        }

        public virtual void CastSpell(Guid spellId, int spellSlot = -1)
        {
            var spellBase = SpellBase.Lookup.Get<SpellBase>(spellId);
            if (spellBase != null)
            {
                switch (spellBase.SpellType)
                {
                    case (int) SpellTypes.CombatSpell:

                        switch (spellBase.TargetType)
                        {
                            case (int) SpellTargetTypes.Self:
                                if (spellBase.HitAnimationId != Guid.Empty)
                                {
                                    PacketSender.SendAnimationToProximity(spellBase.HitAnimationId, 1,
                                        Id, MapId, 0, 0, Dir); //Target Type 1 will be global entity
                                }
                                TryAttack(this, spellBase);
                                break;
                            case (int) SpellTargetTypes.Single:
                                if (CastTarget == null) return;
                                if (spellBase.HitRadius > 0) //Single target spells with AoE hit radius'
                                {
                                    HandleAoESpell(spellId, spellBase.HitRadius, CastTarget.MapId,
                                        CastTarget.X, CastTarget.Y, null);
                                }
                                else
                                {
                                    TryAttack(CastTarget, spellBase);
                                }
                                break;
                            case (int) SpellTargetTypes.AoE:
                                HandleAoESpell(spellId, spellBase.HitRadius, MapId, X, Y, null);
                                break;
                            case (int) SpellTargetTypes.Projectile:
                                var projectileBase = spellBase.Projectile;
                                if (projectileBase != null)
                                {
                                    MapInstance.Lookup.Get<MapInstance>(MapId).SpawnMapProjectile(this,
                                        projectileBase, spellBase, null, MapId, X, Y, Z,
                                        Dir, CastTarget);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case (int) SpellTypes.Warp:
                        if (GetType() == typeof(Player))
                        {
                            Warp(spellBase.Guid1, spellBase.Data2,
                                spellBase.Data3, spellBase.Data4);
                        }
                        break;
                    case (int) SpellTypes.WarpTo:
                        HandleAoESpell(spellId, spellBase.CastRange, MapId, X, Y, CastTarget);
                        break;
                    case (int) SpellTypes.Dash:
                        PacketSender.SendActionMsg(this, Strings.Combat.dash, CustomColors.Dash);
                        var dash = new DashInstance(this, spellBase.CastRange, Dir, Convert.ToBoolean(spellBase.Data1),
                            Convert.ToBoolean(spellBase.Data2), Convert.ToBoolean(spellBase.Data3),
                            Convert.ToBoolean(spellBase.Data4));
                        break;
                    case (int) SpellTypes.Event:
                        //Handled at the player level
                        break;
                    default:
                        break;
                }
                if (spellSlot >= 0 && spellSlot < Options.MaxPlayerSkills)
                {
					decimal cooldownReduction = 1;

					if (GetType() == typeof(Player)) //Only apply cdr for players with equipment
					{
						cooldownReduction = (1 - ((decimal)((Player)this).GetCooldownReduction() / 100));
					}

					Spells[spellSlot].SpellCd = Globals.System.GetTimeMs() + (int)(spellBase.CooldownDuration * cooldownReduction);
                    if (GetType() == typeof(Player))
                    {
                        PacketSender.SendSpellCooldown(((Player) this).MyClient, spellSlot);
                    }
                }
            }
        }

        private void HandleAoESpell(Guid spellId, int range, Guid startMapId, int startX, int startY, EntityInstance spellTarget)
        {
            var spellBase = SpellBase.Lookup.Get<SpellBase>(spellId);
            var targetsHit = new List<EntityInstance>();
            if (spellBase != null)
            {
                for (var x = startX - range; x <= startX + range; x++)
                {
                    for (var y = startY - range; y <= startY + range; y++)
                    {
                        var tempMap = MapInstance.Lookup.Get<MapInstance>(startMapId);
                        var x2 = x;
                        var y2 = y;

                        if (y < 0 && tempMap.Up != Guid.Empty)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Up);
                            y2 = Options.MapHeight + y;
                        }
                        else if (y > Options.MapHeight - 1 && tempMap.Down != Guid.Empty)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Down);
                            y2 = Options.MapHeight - y;
                        }

                        if (x < 0 && tempMap.Left != Guid.Empty)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Left);
                            x2 = Options.MapWidth + x;
                        }
                        else if (x > Options.MapWidth - 1 && tempMap.Right != Guid.Empty)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Right);
                            x2 = Options.MapWidth - x;
                        }

                        var mapEntities = tempMap.GetEntities();
                        for (var i = 0; i < mapEntities.Count; i++)
                        {
                            var t = mapEntities[i];
                            if (t == null || targetsHit.Contains(t)) continue;
                            if (t.GetType() == typeof(Player) || t.GetType() == typeof(Npc))
                            {
                                if (t.MapId == tempMap.Id && t.X == x2 && t.Y == y2)
                                {
                                    if (spellTarget == null || spellTarget == t)
                                    {
                                        targetsHit.Add(t);
                                        //Warp or attack.
                                        if (spellBase.SpellType == (int) SpellTypes.CombatSpell)
                                        {
                                            TryAttack(t, spellBase);
                                            if (spellTarget != null) return;
                                        }
                                        else
                                        {
                                            if (spellTarget != null)
                                            {
                                                Warp(spellTarget.MapId, spellTarget.X, spellTarget.Y,
                                                    Dir); //Spelltarget used to be Target. I don't know if this is correct or not.
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        protected bool IsOneBlockAway(EntityInstance target)
        {
            var myTile = new TileHelper(MapId, X, Y);
            var enemyTile = new TileHelper(target.MapId, target.X, target.Y);
            if (Z == target.Z)
            {
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(2, 0);
                if (myTile.Matches(enemyTile)) return true;
            }
            return false;
        }

        //These functions only work when one block away.
        protected bool IsFacingTarget(EntityInstance target)
        {
            if (IsOneBlockAway(target))
            {
                var myTile = new TileHelper(MapId, X, Y);
                var enemyTile = new TileHelper(target.MapId, target.X, target.Y);
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Up) return true;
                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Down) return true;
                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Left) return true;
                myTile.Translate(2, 0);
                if (myTile.Matches(enemyTile) && Dir == (int) Directions.Right) return true;
            }
            return false;
        }

        protected int GetDistanceTo(EntityInstance target)
        {
            if (target != null)
            {
                var myMap = MapInstance.Lookup.Get<MapInstance>(MapId);
                var targetMap = MapInstance.Lookup.Get<MapInstance>(target.MapId);
                if (myMap != null && targetMap != null)
                {
                    //Calculate World Tile of Me
                    var x1 = X + (myMap.MapGridX * Options.MapWidth);
                    var y1 = Y + (myMap.MapGridY * Options.MapHeight);
                    //Calculate world tile of target
                    var x2 = target.X + (targetMap.MapGridX * Options.MapWidth);
                    var y2 = target.Y + (targetMap.MapGridY * Options.MapHeight);
                    return (int) Math.Sqrt(Math.Pow(x1 - x2, 2) + (Math.Pow(y1 - y2, 2)));
                }
            }
            //Something is null.. return a value that is out of range :) 
            return 9999;
        }

        protected bool InRangeOf(EntityInstance target, int range)
        {
            var dist = GetDistanceTo(target);
            if (dist <= range) return true;
            return false;
        }

        protected int DirToEnemy(EntityInstance target)
        {
            //Calculate World Tile of Me
            var x1 = X + (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridX * Options.MapWidth);
            var y1 = Y + (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridY * Options.MapHeight);
            //Calculate world tile of target
            var x2 = target.X + (MapInstance.Lookup.Get<MapInstance>(target.MapId).MapGridX * Options.MapWidth);
            var y2 = target.Y + (MapInstance.Lookup.Get<MapInstance>(target.MapId).MapGridY * Options.MapHeight);
            if (Math.Abs(x1 - x2) > Math.Abs(y1 - y2))
            {
                //Left or Right
                if (x1 - x2 < 0)
                {
                    return (int) Directions.Right;
                }
                else
                {
                    return (int) Directions.Left;
                }
            }
            else
            {
                //Left or Right
                if (y1 - y2 < 0)
                {
                    return (int) Directions.Down;
                }
                else
                {
                    return (int) Directions.Up;
                }
            }
        }

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        protected bool IsOneBlockAway(Guid mapId, int x, int y, int z = 0)
        {
            //Calculate World Tile of Me
            var x1 = X + (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridX * Options.MapWidth);
            var y1 = Y + (MapInstance.Lookup.Get<MapInstance>(MapId).MapGridY * Options.MapHeight);
            //Calculate world tile of target
            var x2 = x + (MapInstance.Lookup.Get<MapInstance>(mapId).MapGridX * Options.MapWidth);
            var y2 = y + (MapInstance.Lookup.Get<MapInstance>(mapId).MapGridY * Options.MapHeight);
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
        public virtual void Die(int dropitems = 0, EntityInstance killer = null)
        {
            if (Items == null) return;

            if (dropitems > 0)
            {
                // Drop items
                for (var n = 0; n < Items.Count; n++)
                {
                    var item = Items[n];
                    if (item == null) continue;

                    var itemBase = ItemBase.Lookup.Get<ItemBase>(item.Id);
                    if (itemBase == null) continue;

					//Don't lose bound items on death for players.
					if (this.GetType() == typeof(Player))
					{
						if (itemBase.Bound)
						{
							continue;
						}
					}

                    if (Globals.Rand.Next(1, 101) >= dropitems) continue;

                    var map = MapInstance.Lookup.Get<MapInstance>(MapId);
                    map?.SpawnItem(X, Y, item, item.Quantity);

                    var player = this as Player;
                    player?.TakeItemsBySlot(n, item.Quantity);
                }
            }

            var currentMap = MapInstance.Lookup.Get<MapInstance>(MapId);
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
                RestoreVital((Vitals)i);
            }
            Dead = false;
        }

        //Empty virtual functions for players
        public virtual void Warp(Guid newMapId, int newX, int newY, bool adminWarp = false)
        {
            Warp(newMapId, newX, newY, Dir, adminWarp);
        }

        public virtual void Warp(Guid newMapId, int newX, int newY, int newDir, bool adminWarp = false, int zOverride = 0, bool mapSave = false)
        {
        }

        //Serializing Data
        public virtual byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(MapId);
            bf.WriteString(Name);
            bf.WriteString(Sprite);
            bf.WriteString(Face);
            bf.WriteInteger(Level);
            bf.WriteInteger(X);
            bf.WriteInteger(Y);
            bf.WriteInteger(Z);
            bf.WriteInteger(Dir);
            bf.WriteInteger(Passable);
            bf.WriteInteger(HideName);
            bf.WriteInteger(Animations.Count);
            for (var i = 0; i < Animations.Count; i++)
            {
                bf.WriteGuid(Animations[i]);
            }
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                bf.WriteInteger(GetMaxVital(i));
                bf.WriteInteger(GetVital(i));
            }
            var statuses = Statuses.ToArray();
            bf.WriteInteger(statuses.Count());
            foreach (var status in statuses)
            {
                bf.WriteGuid(status.Value.Spell.Id);
                bf.WriteInteger(status.Value.Type);
                bf.WriteString(status.Value.Data);
                bf.WriteInteger((int) (status.Value.Duration - Globals.System.GetTimeMs()));
                bf.WriteInteger((int) (status.Value.Duration - status.Value.StartTime));
            }
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                bf.WriteInteger(Stat[i].Value());
            }
            if (GetType() == typeof(Player)) //helps the client identify admins if entity is a player.
            {
                bf.WriteInteger(((Player) this).MyClient.Access);
            }
            else if (GetType() == typeof(Npc)) //Helps the client identify NPC Behavior
            {
                if (((Npc) this).MyTarget != null)
                {
                    bf.WriteInteger(-1); //Used for coloring the npc's name red when aggression is shown.
                }
                else
                {
                    bf.WriteInteger(((Npc) this).Base.Behavior);
                }
            }
            else
            {
                bf.WriteInteger(0);
            }
            return bf.ToArray();
        }
    }

    public class EntityStat
    {
        private EntityInstance mOwner;
        private Player mPlayer;
        private int mStatType;
        private Dictionary<SpellBase, EntityBuff> mBuff = new Dictionary<SpellBase, EntityBuff>();
        private bool mChanged;

        public int Stat
        {
            get => mOwner.BaseStat[mStatType];
            set => mOwner.BaseStat[mStatType] = value;
        }

        public EntityStat(int statType, EntityInstance owner, Player player)
        {
            mOwner = owner;
            mPlayer = player;
            mStatType = statType;
        }

        public int Value()
        {
            var s = Stat;

            var buffs = mBuff.Values.ToArray();
            foreach (var buff in buffs)
            {
                s += buff.Buff;
            }

            if (mPlayer != null)
            {
                //Add up player equipment values
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (mPlayer.Equipment[i] >= 0 && mPlayer.Equipment[i] < Options.MaxInvItems)
                    {
                        if (mPlayer.Items[mPlayer.Equipment[i]].Id != Guid.Empty)
                        {
                            var item = ItemBase.Lookup.Get<ItemBase>(mPlayer.Items[mPlayer.Equipment[i]].Id);
                            if (item != null)
                            {
                                s += mPlayer.Items[mPlayer.Equipment[i]].StatBoost[mStatType] +
                                     item.StatsGiven[mStatType];
                            }
                        }
                    }
                }
            }

            if (s <= 0)
                s = 1; //No 0 or negative stats, will give errors elsewhere in the code (especially divide by 0 errors).
            return s;
        }

        public bool Update()
        {
            var changed = false;
            var buffs = mBuff.ToArray();
            foreach (var buff in buffs)
            {
                if (buff.Value.Duration <= Globals.System.GetTimeMs())
                {
                    mBuff.Remove(buff.Key);
                    changed = true;
                }
            }

            changed |= mChanged;
            mChanged = false;

            return changed;
        }

        public void AddBuff(EntityBuff buff)
        {
            if (mBuff.ContainsKey(buff.Spell))
            {
                mBuff[buff.Spell].Duration = buff.Duration;
            }
            else
            {
                mBuff.Add(buff.Spell, buff);
            }
            mChanged = true;
        }

        public void Reset()
        {
            mBuff.Clear();
        }
    }

    public class EntityBuff
    {
        public int Buff;
        public long Duration;
        public SpellBase Spell;

        public EntityBuff(SpellBase spell, int buff, int duration)
        {
            Spell = spell;
            Buff = buff;
            Duration = Globals.System.GetTimeMs() + duration;
        }
    }

    public class DoTInstance
    {
        private long mInterval;

        public EntityInstance Attacker;

        public int Count;
        public SpellBase SpellBase;
        public EntityInstance Target { get; }

        public DoTInstance(EntityInstance attacker, Guid spellId, EntityInstance target)
        {
            SpellBase = SpellBase.Lookup.Get<SpellBase>(spellId);

            Attacker = attacker;
            Target = target;

            if (SpellBase == null || SpellBase.Data4 < 1)
            {
                return;
            }

            mInterval = Globals.System.GetTimeMs() + SpellBase.Data4;
            Count = SpellBase.Data2 / SpellBase.Data4 - 1;
            target.DoT.Add(this);
            //Subtract 1 since the first tick always occurs when the spell is cast.
        }

        public bool CheckExpired()
        {
            if (SpellBase == null || Count > 0) return false;
            Target?.DoT?.Remove(this);
            return true;
        }

        public void Tick()
        {
            if (CheckExpired()) return;

            if (mInterval > Globals.System.GetTimeMs()) return;
            var deadAnimations = new List<KeyValuePair<Guid, int>>();
            var aliveAnimations = new List<KeyValuePair<Guid, int>>();
            if (SpellBase.HitAnimationId != Guid.Empty)
            {
                deadAnimations.Add(new KeyValuePair<Guid, int>(SpellBase.HitAnimationId, (int) Directions.Up));
                aliveAnimations.Add(new KeyValuePair<Guid, int>(SpellBase.HitAnimationId, (int) Directions.Up));
            }

            Attacker?.Attack(Target, SpellBase.VitalDiff[0], SpellBase.VitalDiff[1],
                (DamageType) SpellBase.DamageType, (Stats) SpellBase.ScalingStat, SpellBase.Scaling,
                SpellBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
            mInterval = Globals.System.GetTimeMs() + SpellBase.Data4;
            Count--;
        }
    }

    public class StatusInstance
    {
        public SpellBase Spell;
        public string Data = "";
        public long Duration;
        private EntityInstance mEntity;
        public long StartTime;
        public int Type;

        public StatusInstance(EntityInstance en, SpellBase spell, int type, int duration, string data)
        {
            mEntity = en;
            Spell = spell;
            Type = type;
            Duration = Globals.System.GetTimeMs() + duration;
            StartTime = Globals.System.GetTimeMs();
            Data = data;
            if (en.Statuses.ContainsKey(spell))
            {
                en.Statuses[spell].Duration = Duration;
                en.Statuses[spell].StartTime = StartTime;
            }
            else
            {
                en.Statuses.Add(Spell, this);
            }
            PacketSender.SendEntityVitals(mEntity);
        }

        public void TryRemoveStatus()
        {
            if (Duration <= Globals.System.GetTimeMs()) //Check the timer
            {
                RemoveStatus();
            }
        }

        public void RemoveStatus()
        {
            mEntity.Statuses.Remove(Spell);
            PacketSender.SendEntityVitals(mEntity);
        }
    }

    public class DashInstance
    {
        public int Direction;
        public int DistanceTraveled;
        public int Facing;
        public int Range;
        public long TransmittionTimer;

        public DashInstance(EntityInstance en, int range, int direction, bool blockPass = false,
            bool activeResourcePass = false,
            bool deadResourcePass = false, bool zdimensionPass = false)
        {
            DistanceTraveled = 0;
            Direction = direction;
            Facing = en.Dir;

            CalculateRange(en, range, blockPass,activeResourcePass,deadResourcePass,zdimensionPass);
            if (Range <= 0)
            {
                return;
            } //Remove dash instance if no where to dash
            TransmittionTimer = Globals.System.GetTimeMs() + (long) ((float) Options.MaxDashSpeed / (float) Range);
            PacketSender.SendEntityDash(en, en.MapId, en.X, en.Y,
                (int) (Options.MaxDashSpeed * (Range / 10f)), Direction == Facing ? Direction : -Facing);
            en.MoveTimer = Globals.System.GetTimeMs() + Options.MaxDashSpeed;
        }

        public void CalculateRange(EntityInstance en, int range, bool blockPass = false, bool activeResourcePass = false, bool deadResourcePass = false, bool zdimensionPass = false)
        {
            var n = 0;
            en.MoveTimer = 0;
            Range = 0;
            for (var i = 1; i <= range; i++)
            {
                n = en.CanMove(Direction);
                if (n == -5) //Check for out of bounds
				{
                    return;
				} //Check for blocks
				if (n == -2 && blockPass == false)
                {
                    return;
				} //Check for ZDimensionTiles
				if (n == -3 && zdimensionPass == false)
                {
                    return;
				} //Check for active resources
				if (n == (int) EntityTypes.Resource && activeResourcePass == false)
                {
                    return;
				} //Check for dead resources
				if (n == (int) EntityTypes.Resource && deadResourcePass == false)
                {
                    return;
                } //Check for players and solid events
                if (n == (int) EntityTypes.Player || n == (int)EntityTypes.Event) return;

                en.Move(Direction, null, true);
                en.Dir = Facing;

                Range = i;
            }
        }
    }
}
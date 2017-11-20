using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Items;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;
using Attribute = Intersect.GameObjects.Maps.Attribute;

namespace Intersect.Server.Classes.Entities
{
    using Database = Intersect.Server.Classes.Core.Database;

    public class Entity
    {
        private int _dir;

        //Active Animations -- for events mainly
        public List<int> Animations = new List<int>();

        public long AttackTimer;
        public bool Blocking;
        public Entity CastTarget;

        //Combat Status
        public long CastTime;

        public long CollisionIndex;
        public long CombatTimer;
        public int CurrentMap = -1;

        //Location Info
        public int CurrentX;

        public int CurrentY;
        public int CurrentZ;
        public bool Dead;

        //DoT/HoT Spells
        public List<DoTInstance> DoT = new List<DoTInstance>();

        //Extras
        public string Face = "";

        public int HideName = 0;

        //Inventory
        public List<ItemInstance> Inventory = new List<ItemInstance>();

        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        public EventMoveRoute MoveRoute = null;
        public EventPageInstance MoveRouteSetter = null;

        public long MoveTimer;

        //Core Values
        public int MyIndex;

        public string MyName = "";
        public string MySprite = "";
        public int Passable = 0;
        public long RegenTimer = Globals.System.GetTimeMs();
        public long SpawnTime;
        public int SpellCastSlot = 0;

        //Spells
        public List<SpellInstance> Spells = new List<SpellInstance>();

        public EntityStat[] Stat = new EntityStat[(int) Stats.StatCount];

        //Status effects
        public Dictionary<SpellBase, StatusInstance> Statuses = new Dictionary<SpellBase, StatusInstance>();

        public Entity Target = null;
        public int[] Vital = new int[(int) Vitals.VitalCount];

        //Initialization
        public Entity(int index)
        {
            for (int I = 0; I < (int) Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(0, I);
            }

            MyIndex = index;
            //HP
            MaxVital[(int) Vitals.Health] = 100;
            Vital[(int) Vitals.Health] = 100;
            //MP
            MaxVital[(int) Vitals.Health] = 100;
            Vital[(int) Vitals.Health] = 100;
            //ATK
            Stat[(int) Stats.Attack].Stat = 23;
            //Ability
            Stat[(int) Stats.AbilityPower].Stat = 16;
            //Def
            Stat[(int) Stats.Defense].Stat = 23;
            //MR
            Stat[(int) Stats.MagicResist].Stat = 16;
            //SPD
            Stat[(int) Stats.Speed].Stat = 20;

            SpawnTime = Globals.System.GetTimeMs();
        }

        public int Dir
        {
            get => _dir;
            set => _dir = (value + 4) % 4;
        }

        public bool IsDisposed { get; private set; }

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
                CastSpell(Spells[SpellCastSlot].SpellNum, SpellCastSlot);
                CastTarget = null;
            }
            //DoT/HoT timers
            for (int i = 0; i < DoT.Count; i++)
            {
                DoT[i].Tick();
            }
            for (int i = 0; i < (int) Stats.StatCount; i++)
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
            var tile = new TileHelper(CurrentMap, CurrentX, CurrentY);
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
                Attribute tileAttribute = MapInstance.Lookup.Get<MapInstance>(tile.GetMap())
                    .Attributes[tile.GetX(), tile.GetY()];
                if (tileAttribute != null)
                {
                    if (tileAttribute.value == (int) MapAttributes.Blocked) return -2;
                    if (tileAttribute.value == (int) MapAttributes.NPCAvoid && GetType() == typeof(Npc)) return -2;
                    if (tileAttribute.value == (int) MapAttributes.ZDimension && tileAttribute.data2 > 0 &&
                        tileAttribute.data2 - 1 == CurrentZ) return -3;
                    if (tileAttribute.value == (int) MapAttributes.Slide) return -4;
                }
            }
            else
            {
                return -5; //Out of Bounds
            }

            if (Passable == 0)
            {
                var targetMap = MapInstance.Lookup.Get<MapInstance>(tile.GetMap());
                var mapEntities = MapInstance.Lookup.Get<MapInstance>(tile.GetMap()).GetEntities();
                for (int i = 0; i < mapEntities.Count; i++)
                {
                    Entity en = mapEntities[i];
                    if (en != null && en.CurrentX == tile.GetX() && en.CurrentY == tile.GetY() && en.CurrentZ == CurrentZ &&
                        en.Passable == 0)
                    {
                        //Set a target if a projectile
                        CollisionIndex = en.MyIndex;
                        if (en.GetType() == typeof(Player))
                        {
                            //Check if this target player is passable....
                            if (!Options.PlayerPassable[(int) targetMap.ZoneType]) return (int) EntityTypes.Player;
                        }
                        else if (en.GetType() == typeof(Npc))
                        {
                            return (int) EntityTypes.Player;
                        }
                        else if (en.GetType() == typeof(Resource))
                        {
                            //If determine if we should walk
                            var res = ((Resource) en);
                            if ((!res.IsDead() && !res.MyBase.WalkableBefore) ||
                                (res.IsDead() && !res.MyBase.WalkableAfter))
                            {
                                return (int) EntityTypes.Resource;
                            }
                        }
                    }
                }
                //If this is an npc or other event.. if any global page exists that isn't passable then don't walk here!
                if (this.GetType() != typeof(Player))
                {
                    foreach (var evt in MapInstance.Lookup.Get<MapInstance>(tile.GetMap()).GlobalEventInstances)
                    {
                        foreach (var en in evt.Value.GlobalPageInstance)
                        {
                            if (en != null && en.CurrentX == tile.GetX() && en.CurrentY == tile.GetY() &&
                                en.CurrentZ == CurrentZ &&
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

        public virtual void Move(int moveDir, Client client, bool DontUpdate = false, bool correction = false)
        {
            var xOffset = 0;
            var yOffset = 0;
            Dir = moveDir;
            if (MoveTimer < Globals.System.GetTimeMs())
            {
                var tile = new TileHelper(CurrentMap, CurrentX, CurrentY);
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
                    CurrentX = tile.GetX();
                    CurrentY = tile.GetY();
                    if (CurrentMap != tile.GetMap())
                    {
                        var oldMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                        if (oldMap != null) oldMap.RemoveEntity(this);
                        var newMap = MapInstance.Lookup.Get<MapInstance>(tile.GetMap());
                        if (newMap != null) newMap.AddEntity(this);
                    }
                    CurrentMap = tile.GetMap();
                    if (DontUpdate == false)
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
                        var myMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
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
                                            if (spawn != null && spawn.Map == CurrentMap && spawn.X == CurrentX &&
                                                spawn.Y == CurrentY && spawn.Z == CurrentZ)
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
                    if (TryToChangeDimension() && DontUpdate == true)
                    {
                        PacketSender.UpdateEntityZDimension(MyIndex, CurrentZ);
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
                    PacketSender.SendEntityDirTo(((EventPageInstance) this).Client, MyIndex, (int) EntityTypes.Event,
                        Dir, CurrentMap);
                }
                else
                {
                    PacketSender.SendEntityDir(MyIndex, (int) EntityTypes.Event, Dir, CurrentMap);
                }
            }
            else
            {
                PacketSender.SendEntityDir(MyIndex, (int) EntityTypes.GlobalEntity, Dir, CurrentMap);
            }
        }

        // Change the dimension if the player is on a gateway
        public bool TryToChangeDimension()
        {
            if (CurrentX < Options.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Options.MapHeight && CurrentY >= 0)
                {
                    Attribute attribute = MapInstance.Lookup.Get<MapInstance>(CurrentMap)
                        .Attributes[CurrentX, CurrentY];
                    if (attribute != null && attribute.value == (int) MapAttributes.ZDimension)
                    {
                        if (attribute.data1 > 0)
                        {
                            CurrentZ = attribute.data1 - 1;
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
            int myGrid = MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGrid;
            //Loop through surrouding maps to generate a array of open and blocked points.
            for (var x = MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridX - 1;
                x <= MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridX + 1;
                x++)
            {
                if (x == -1 || x >= Database.MapGrids[myGrid].Width) continue;
                for (var y = MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridY - 1;
                    y <= MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridY + 1;
                    y++)
                {
                    if (y == -1 || y >= Database.MapGrids[myGrid].Height) continue;
                    if (Database.MapGrids[myGrid].MyGrid[x, y] > -1 &&
                        Database.MapGrids[myGrid].MyGrid[x, y] == target.CurrentMap)
                    {
                        xDiff = (MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridX - x) * Options.MapWidth +
                                target.CurrentX -
                                CurrentX;
                        yDiff = (MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridY - y) * Options.MapHeight +
                                target.CurrentY -
                                CurrentY;
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
                    PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, CurrentMap, -1);
                }
                else if (blocking == 0 && Blocking)
                {
                    Blocking = false;
                    AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
                    PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, CurrentMap, 0);
                }
            }
        }

        public virtual int GetWeaponDamage()
        {
            return 0;
        }

        public virtual bool CanAttack(Entity en, SpellBase spell)
        {
            return true;
        }

        //Vitals
        public void RestoreVital(Vitals vital)
        {
            Vital[(int)vital] = MaxVital[(int)vital];
            PacketSender.SendEntityVitals(this);
        }

        public void AddVital(Vitals vital, int amount)
        {
            Vital[(int)vital] += amount;
            if (Vital[(int)vital] < 0) Vital[(int)vital] = 0;
            if (Vital[(int)vital] > MaxVital[(int)vital]) Vital[(int)vital] = MaxVital[(int)vital];
            PacketSender.SendEntityVitals(this);
        }

        //Attacking with projectile
        public virtual void TryAttack(Entity enemy, ProjectileBase projectile, SpellBase parentSpell,
            ItemBase parentItem, int projectileDir)
        {
            if (enemy.GetType() == typeof(Resource) && parentSpell != null) return;

            //Check if the target is blocking facing in the direction against you
            if (enemy.Blocking)
            {
                int d = Dir;

                if (projectile != null)
                {
                    d = projectileDir;
                }

                if (enemy.Dir == (int) Directions.Left && d == (int) Directions.Right)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "blocked"), CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Right && d == (int) Directions.Left)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "blocked"), CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Up && d == (int) Directions.Down)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "blocked"), CustomColors.Blocked);
                    return;
                }
                else if (enemy.Dir == (int) Directions.Down && d == (int) Directions.Up)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "blocked"), CustomColors.Blocked);
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
                    if (MapInstance.Lookup.Get<MapInstance>(CurrentMap).ZoneType == MapZones.Safe)
                    {
                        return;
                    }
                    if (MapInstance.Lookup.Get<MapInstance>(enemy.CurrentMap).ZoneType == MapZones.Safe)
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
                if (projectile.Spell > -1)
                {
                    var s = SpellBase.Lookup.Get<SpellBase>(projectile.Spell);
                    if (s != null)
                        HandleAoESpell(projectile.Spell, s.HitRadius, enemy.CurrentMap, enemy.CurrentX, enemy.CurrentY,
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
        public virtual void TryAttack(Entity enemy, SpellBase spellBase)
        {
            if (enemy.GetType() == typeof(Resource)) return;
            if (spellBase != null)
            {
                List<KeyValuePair<int, int>> deadAnimations = new List<KeyValuePair<int, int>>();
                List<KeyValuePair<int, int>> aliveAnimations = new List<KeyValuePair<int, int>>();

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

                    //Check if either the attacker or the defender is in a "safe zone" (Only apply if combat is PVP)
                    if (enemy.GetType() == typeof(Player) && GetType() == typeof(Player))
                    {
                        if (MapInstance.Lookup.Get<MapInstance>(CurrentMap).ZoneType == MapZones.Safe)
                        {
                            return;
                        }
                        if (MapInstance.Lookup.Get<MapInstance>(enemy.CurrentMap).ZoneType == MapZones.Safe)
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
                    if (enemy.GetType() != GetType()) return; //Don't let players aoe heal npcs. Don't let npcs aoe heal players.
                }

                if (spellBase.HitAnimation > -1)
                {
                    deadAnimations.Add(new KeyValuePair<int, int>(spellBase.HitAnimation, (int) Directions.Up));
                    aliveAnimations.Add(new KeyValuePair<int, int>(spellBase.HitAnimation, (int) Directions.Up));
                }

                var damageHealth = spellBase.VitalDiff[0];
                var damageMana = spellBase.VitalDiff[1];

                Attack(enemy, damageHealth, damageMana, (DamageType) spellBase.DamageType,
                    (Stats) spellBase.ScalingStat,
                    spellBase.Scaling, spellBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);

                var statBuffTime = -1;
                for (int i = 0; i < (int) Stats.StatCount; i++)
                {
                    enemy.Stat[i].AddBuff(new EntityBuff(spellBase, spellBase.StatDiff[i], (spellBase.Data2 * 100)));
                    if (spellBase.StatDiff[i] != 0 && spellBase.Data2 * 100 != null)
                        statBuffTime = spellBase.Data2 * 100;
                }

                //Handle other status effects
                if (spellBase.Data3 > 0)
                {
                    new StatusInstance(enemy, spellBase, spellBase.Data3, (spellBase.Data2 * 100), spellBase.Data5);
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "status" + spellBase.Data3),
                        CustomColors.Status);
                }
                else
                {
                    if (statBuffTime > -1) new StatusInstance(enemy, spellBase, -1, statBuffTime, "");
                }

                //Handle DoT/HoT spells]
                if (spellBase.Data1 > 0)
                {
                    bool DoTFound = false;
                    for (int i = 0; i < enemy.DoT.Count; i++)
                    {
                        if (enemy.DoT[i].SpellBase.Index == spellBase.Index ||
                            enemy.DoT[i].OwnerID == MyIndex)
                        {
                            DoTFound = true;
                        }
                    }
                    if (DoTFound == false) //no duplicate DoT/HoT spells.
                    {
                        enemy.DoT.Add(new DoTInstance(MyIndex, spellBase.Index, enemy));
                    }
                }
            }
        }

        //Attacking with weapon or unarmed.
        public virtual void TryAttack(Entity enemy)
        {
            //See player and npc override of this virtual void
        }

        //Attack using a weapon or unarmed
        public virtual void TryAttack(Entity enemy, int baseDamage, DamageType damageType, Stats scalingStat,
            int scaling, int critChance, double critMultiplier, List<KeyValuePair<int, int>> deadAnimations = null,
            List<KeyValuePair<int, int>> aliveAnimations = null)
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
                if (MapInstance.Lookup.Get<MapInstance>(CurrentMap).ZoneType == MapZones.Safe)
                {
                    return;
                }
                if (MapInstance.Lookup.Get<MapInstance>(enemy.CurrentMap).ZoneType == MapZones.Safe)
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
                        PacketSender.SendActionMsg(this, Strings.Get("combat", "miss"), CustomColors.Missed);
                        PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, CurrentMap,
                            CalculateAttackTime());
                        return;
                    }
                }
            }

            Attack(enemy, baseDamage, 0, damageType, scalingStat, scaling, critChance, critMultiplier, deadAnimations,
                aliveAnimations);

            //If we took damage lets reset our combat timer
            enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
        }

        public void Attack(Entity enemy, int baseDamage, int secondaryDamage, DamageType damageType, Stats scalingStat,
            int scaling, int critChance, double critMultiplier, List<KeyValuePair<int, int>> deadAnimations = null,
            List<KeyValuePair<int, int>> aliveAnimations = null)
        {
            if (enemy == null) return;

            //No Matter what, if we attack the entitiy, make them chase us
            if (enemy.GetType() == typeof(Npc))
            {
                if (((Npc) enemy).MyBase.Behavior == (int) NpcBehavior.Friendly) return;
                ((Npc) enemy).AssignTarget(this);

                //Check if there are any guards nearby
                //TODO Loop through CurrentMap - SurroundingMaps Entity List instead of global entity list.
                var mapEntities = MapInstance.Lookup.Get<MapInstance>(CurrentMap).GetEntities();
                for (int n = 0; n < mapEntities.Count; n++)
                {
                    if (mapEntities[n].GetType() == typeof(Npc))
                    {
                        if (((Npc) mapEntities[n]).Behaviour == 3) // Type guard
                        {
                            int x = mapEntities[n].CurrentX - ((Npc) mapEntities[n]).Range;
                            int y = mapEntities[n].CurrentY - ((Npc) mapEntities[n]).Range;
                            int xMax = mapEntities[n].CurrentX + ((Npc) mapEntities[n]).Range;
                            int yMax = mapEntities[n].CurrentY + ((Npc) mapEntities[n]).Range;

                            //Check that not going out of the map boundaries
                            if (x < 0) x = 0;
                            if (y < 0) y = 0;
                            if (xMax >= Options.MapWidth) xMax = Options.MapWidth;
                            if (yMax >= Options.MapHeight) yMax = Options.MapHeight;

                            if (x < Globals.Entities[MyIndex].CurrentX && xMax > Globals.Entities[MyIndex].CurrentX)
                            {
                                if (y < Globals.Entities[MyIndex].CurrentY && yMax > Globals.Entities[MyIndex].CurrentY)
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
                PacketSender.SendActionMsg(enemy, Strings.Get("combat", "critical"), CustomColors.Critical);
            }

            //Calculate Damages
            if (baseDamage != 0)
            {
                baseDamage = Formulas.CalculateDamage(baseDamage, damageType, scalingStat, scaling, critMultiplier,
                    this,
                    enemy);
                enemy.Vital[(int) Vitals.Health] -= (int) baseDamage;
                if (baseDamage > 0)
                {
                    switch (damageType)
                    {
                        case DamageType.Physical:
                            PacketSender.SendActionMsg(enemy, Strings.Get("combat", "removesymbol") + (int) baseDamage,
                                CustomColors.PhysicalDamage);
                            break;
                        case DamageType.Magic:
                            PacketSender.SendActionMsg(enemy, Strings.Get("combat", "removesymbol") + (int) baseDamage,
                                CustomColors.MagicDamage);
                            break;
                        case DamageType.True:
                            PacketSender.SendActionMsg(enemy, Strings.Get("combat", "removesymbol") + (int) baseDamage,
                                CustomColors.TrueDamage);
                            break;
                    }
                    enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
                }
                else if (baseDamage < 0)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "addsymbol") + (int) Math.Abs(baseDamage),
                        CustomColors.Heal);
                }
            }
            if (secondaryDamage != 0)
            {
                secondaryDamage = Formulas.CalculateDamage(secondaryDamage, damageType, scalingStat, scaling,
                    critMultiplier, this, enemy);
                enemy.Vital[(int) Vitals.Mana] -= (int) baseDamage;
                if (secondaryDamage > 0)
                {
                    //If we took damage lets reset our combat timer
                    enemy.CombatTimer = Globals.System.GetTimeMs() + 5000;
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "removesymbol") + (int)secondaryDamage,
                        CustomColors.RemoveMana);
                }
                else if (secondaryDamage < 0)
                {
                    PacketSender.SendActionMsg(enemy, Strings.Get("combat", "addsymbol") + (int) Math.Abs(secondaryDamage),
                        CustomColors.AddMana);
                }
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int) Vitals.Health] >=
                enemy.MaxVital[(int) Vitals.Health])
            {
                enemy.Vital[(int) Vitals.Health] =
                    enemy.MaxVital[(int) Vitals.Health];
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int) Vitals.Mana] >=
                enemy.MaxVital[(int) Vitals.Mana])
            {
                enemy.Vital[(int) Vitals.Mana] =
                    enemy.MaxVital[(int) Vitals.Mana];
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int) Vitals.Mana] <= 0)
            {
                enemy.Vital[(int) Vitals.Mana] = 0;
            }

            //Dead entity check
            if (enemy.Vital[(int) Vitals.Health] <= 0)
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
                        PacketSender.SendAnimationToProximity(anim.Key, -1, -1, enemy.CurrentMap, enemy.CurrentX,
                            enemy.CurrentY, anim.Value);
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
                        PacketSender.SendAnimationToProximity(anim.Key, 1, enemy.MyIndex, enemy.CurrentMap, -1, -1,
                            anim.Value);
                    }
                }
            }
            // Add a timer before able to make the next move.
            if (Globals.Entities[MyIndex] != null && Globals.Entities[MyIndex].GetType() == typeof(Npc))
            {
                ((Npc) Globals.Entities[MyIndex]).MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
            }
        }

        public virtual void KilledEntity(Entity en)
        {
        }

        public virtual void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            var spellBase = SpellBase.Lookup.Get<SpellBase>(SpellNum);
            if (spellBase != null)
            {
                switch (spellBase.SpellType)
                {
                    case (int) SpellTypes.CombatSpell:

                        switch (spellBase.TargetType)
                        {
                            case (int) SpellTargetTypes.Self:
                                if (spellBase.HitAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spellBase.HitAnimation, 1,
                                        MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
                                }
                                TryAttack(this, spellBase);
                                break;
                            case (int) SpellTargetTypes.Single:
                                if (CastTarget == null) return;
                                if (spellBase.HitRadius > 0) //Single target spells with AoE hit radius'
                                {
                                    HandleAoESpell(SpellNum, spellBase.HitRadius, CastTarget.CurrentMap,
                                        CastTarget.CurrentX, CastTarget.CurrentY, null);
                                }
                                else
                                {
                                    TryAttack(CastTarget, spellBase);
                                }
                                break;
                            case (int) SpellTargetTypes.AoE:
                                HandleAoESpell(SpellNum, spellBase.HitRadius, CurrentMap, CurrentX, CurrentY, null);
                                break;
                            case (int) SpellTargetTypes.Projectile:
                                var projectileBase = ProjectileBase.Lookup.Get<ProjectileBase>(spellBase.Projectile);
                                if (projectileBase != null)
                                {
                                    MapInstance.Lookup.Get<MapInstance>(CurrentMap).SpawnMapProjectile(this,
                                        projectileBase, spellBase, null, CurrentMap, CurrentX, CurrentY, CurrentZ,
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
                            Warp(spellBase.Data1, spellBase.Data2,
                                spellBase.Data3, spellBase.Data4);
                        }
                        break;
                    case (int) SpellTypes.WarpTo:
                        if (GetType() == typeof(Player))
                        {
                            HandleAoESpell(SpellNum, spellBase.CastRange, CurrentMap, CurrentX, CurrentY, CastTarget);
                        }
                        break;
                    case (int) SpellTypes.Dash:
                        PacketSender.SendActionMsg(this, Strings.Get("combat", "dash"), CustomColors.Dash);
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
                if (SpellSlot >= 0 && SpellSlot < Options.MaxPlayerSkills)
                {
                    Spells[SpellSlot].SpellCD = Globals.System.GetTimeMs() +
                                                (spellBase.CooldownDuration * 100);
                    if (GetType() == typeof(Player))
                    {
                        PacketSender.SendSpellCooldown(((Player) this).MyClient, SpellSlot);
                    }
                }
            }
        }

        private void HandleAoESpell(int SpellNum, int Range, int StartMap, int StartX, int StartY, Entity spellTarget)
        {
            var spellBase = SpellBase.Lookup.Get<SpellBase>(SpellNum);
            var targetsHit = new List<Entity>();
            if (spellBase != null)
            {
                for (int x = StartX - Range; x <= StartX + Range; x++)
                {
                    for (int y = StartY - Range; y <= StartY + Range; y++)
                    {
                        var tempMap = MapInstance.Lookup.Get<MapInstance>(StartMap);
                        int x2 = x;
                        int y2 = y;

                        if (y < 0 && tempMap.Up > -1)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Up);
                            y2 = Options.MapHeight + y;
                        }
                        else if (y > Options.MapHeight - 1 && tempMap.Down > -1)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Down);
                            y2 = Options.MapHeight - y;
                        }

                        if (x < 0 && tempMap.Left > -1)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Left);
                            x2 = Options.MapWidth + x;
                        }
                        else if (x > Options.MapWidth - 1 && tempMap.Right > -1)
                        {
                            tempMap = MapInstance.Lookup.Get<MapInstance>(tempMap.Right);
                            x2 = Options.MapWidth - x;
                        }

                        var mapEntities = tempMap.GetEntities();
                        for (int i = 0; i < mapEntities.Count; i++)
                        {
                            Entity t = mapEntities[i];
                            if (t == null || targetsHit.Contains(t)) continue;
                            if (t.GetType() == typeof(Player) || t.GetType() == typeof(Npc))
                            {
                                if (t.CurrentMap == tempMap.Index && t.CurrentX == x2 && t.CurrentY == y2)
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
                                                Warp(spellTarget.CurrentMap, spellTarget.CurrentX, spellTarget.CurrentY,
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
        protected bool IsOneBlockAway(Entity target)
        {
            TileHelper myTile = new TileHelper(CurrentMap, CurrentX, CurrentY);
            TileHelper enemyTile = new TileHelper(target.CurrentMap, target.CurrentX, target.CurrentY);
            if (CurrentZ == target.CurrentZ)
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
        protected bool isFacingTarget(Entity target)
        {
            if (IsOneBlockAway(target))
            {
                TileHelper myTile = new TileHelper(CurrentMap, CurrentX, CurrentY);
                TileHelper enemyTile = new TileHelper(target.CurrentMap, target.CurrentX, target.CurrentY);
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

        protected int GetDistanceTo(Entity target)
        {
            if (target != null)
            {
                var myMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
                var targetMap = MapInstance.Lookup.Get<MapInstance>(target.CurrentMap);
                if (myMap != null && targetMap != null)
                {
                    //Calculate World Tile of Me
                    var x1 = CurrentX + (myMap.MapGridX * Options.MapWidth);
                    var y1 = CurrentY + (myMap.MapGridY * Options.MapHeight);
                    //Calculate world tile of target
                    var x2 = target.CurrentX + (targetMap.MapGridX * Options.MapWidth);
                    var y2 = target.CurrentY + (targetMap.MapGridY * Options.MapHeight);
                    return (int) Math.Sqrt(Math.Pow(x1 - x2, 2) + (Math.Pow(y1 - y2, 2)));
                }
            }
            //Something is null.. return a value that is out of range :) 
            return 9999;
        }

        protected bool InRangeOf(Entity target, int Range)
        {
            var dist = GetDistanceTo(target);
            if (dist <= Range) return true;
            return false;
        }

        protected int DirToEnemy(Entity target)
        {
            //Calculate World Tile of Me
            var x1 = CurrentX + (MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridX * Options.MapWidth);
            var y1 = CurrentY + (MapInstance.Lookup.Get<MapInstance>(CurrentMap).MapGridY * Options.MapHeight);
            //Calculate world tile of target
            var x2 = target.CurrentX + (MapInstance.Lookup.Get<MapInstance>(target.CurrentMap).MapGridX * Options.MapWidth);
            var y2 = target.CurrentY + (MapInstance.Lookup.Get<MapInstance>(target.CurrentMap).MapGridY * Options.MapHeight);
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
        protected bool IsOneBlockAway(int map, int x, int y, int z = 0)
        {
            if (z == CurrentZ)
            {
                if (y == CurrentY)
                {
                    if (x == CurrentX - 1)
                    {
                        return true;
                    }
                    else if (x == CurrentX + 1)
                    {
                        return true;
                    }
                }
                if (x == CurrentX)
                {
                    if (y == CurrentY - 1)
                    {
                        return true;
                    }
                    else if (y == CurrentY + 1)
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
            if (dropitems > 0)
            {
                // Drop items
                for (int n = 0; n < Inventory.Count; n++)
                {
                    ItemInstance item = Inventory[n];
                    if (ItemBase.Lookup.Get<ItemBase>(item.ItemNum) != null && Globals.Rand.Next(1, 101) < dropitems)
                    {
                        MapInstance.Lookup.Get<MapInstance>(CurrentMap)
                            .SpawnItem(CurrentX, CurrentY, item, item.ItemVal);
                        if (GetType() == typeof(Player))
                        {
                            ((Player) this).TakeItemsBySlot(n, item.ItemVal);
                        }
                    }
                }
            }
            var currentMap = MapInstance.Lookup.Get<MapInstance>(CurrentMap);
            if (currentMap != null)
            {
                currentMap.ClearEntityTargetsOf(this);
                foreach (var mapNum in currentMap.SurroundingMaps)
                {
                    var surroundingMap = MapInstance.Lookup.Get<MapInstance>(mapNum);
                    if (surroundingMap != null)
                    {
                        surroundingMap.ClearEntityTargetsOf(this);
                    }
                }
            }
            DoT.Clear();
            Statuses.Clear();
            for (int i = 0; i < (int) Stats.StatCount; i++)
            {
                Stat[i].Reset();
            }
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
                Vital[i] = MaxVital[i];
            }
            Dead = false;
        }

        //Empty virtual functions for players
        public virtual void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, Dir);
        }

        public virtual void Warp(int newMap, int newX, int newY, int newDir)
        {
        }

        //Serializing Data
        public virtual byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(CurrentMap);
            bf.WriteString(MyName);
            bf.WriteString(MySprite);
            bf.WriteString(Face);
            bf.WriteInteger(Level);
            bf.WriteInteger(CurrentX);
            bf.WriteInteger(CurrentY);
            bf.WriteInteger(CurrentZ);
            bf.WriteInteger(Dir);
            bf.WriteInteger(Passable);
            bf.WriteInteger(HideName);
            bf.WriteInteger(Animations.Count);
            for (int i = 0; i < Animations.Count; i++)
            {
                bf.WriteInteger(Animations[i]);
            }
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                bf.WriteInteger(MaxVital[i]);
                bf.WriteInteger(Vital[i]);
            }
            var statuses = Statuses.ToArray();
            bf.WriteInteger(statuses.Count());
            foreach (var status in statuses)
            {
                bf.WriteInteger(status.Value._spell.Index);
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
                bf.WriteInteger(((Player) this).MyClient.Power);
            }
            else if (GetType() == typeof(Npc)) //Helps the client identify NPC Behavior
            {
                if (((Npc) this).MyTarget != null)
                {
                    bf.WriteInteger(-1); //Used for coloring the npc's name red when aggression is shown.
                }
                else
                {
                    bf.WriteInteger(((Npc) this).MyBase.Behavior);
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
        private Player _player;
        private int _statType;
        private Dictionary<SpellBase, EntityBuff> mBuff = new Dictionary<SpellBase, EntityBuff>();
        private bool mChanged;
        public int Stat;

        public EntityStat(int stat, int statType, Player owner = null)
        {
            Stat = stat;
            _player = owner;
            _statType = statType;
        }

        public int Value()
        {
            int s = Stat;

            var buffs = mBuff.Values.ToArray();
            foreach (var buff in buffs)
            {
                s += buff.Buff;
            }

            if (_player != null)
            {
                //Add up player equipment values
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (_player.Equipment[i] >= 0 && _player.Equipment[i] < Options.MaxInvItems)
                    {
                        if (_player.Inventory[_player.Equipment[i]].ItemNum > -1)
                        {
                            var item = ItemBase.Lookup.Get<ItemBase>(_player.Inventory[_player.Equipment[i]].ItemNum);
                            if (item != null)
                            {
                                s += _player.Inventory[_player.Equipment[i]].StatBoost[_statType] +
                                     item.StatsGiven[_statType];
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
        public int Count;
        private long Interval;
        public int OwnerID = -1;
        public SpellBase SpellBase;
        public Entity Target;

        public DoTInstance(int ownerID, int spellNum, Entity target)
        {
            SpellBase = SpellBase.Lookup.Get<SpellBase>(spellNum);
            if (SpellBase != null && SpellBase.Data4 > 0)
            { 
                OwnerID = ownerID;
                Target = target;
                Interval = Globals.System.GetTimeMs() + (SpellBase.Data4 * 100);
                Count = (SpellBase.Data2 / SpellBase.Data4) - 1;
                //Subtract 1 since the first tick always occurs when the spell is cast.
            }
        }

        public void Tick()
        {
            if (Interval <= Globals.System.GetTimeMs())
            {
                var deadAnimations = new List<KeyValuePair<int, int>>();
                var aliveAnimations = new List<KeyValuePair<int, int>>();
                if (SpellBase.HitAnimation > -1)
                {
                    deadAnimations.Add(new KeyValuePair<int, int>(SpellBase.HitAnimation, (int) Directions.Up));
                    aliveAnimations.Add(new KeyValuePair<int, int>(SpellBase.HitAnimation, (int) Directions.Up));
                }
                if (Globals.Entities[OwnerID] != null)
                    Globals.Entities[OwnerID].Attack(Target, SpellBase.VitalDiff[0], SpellBase.VitalDiff[1],
                        (DamageType) SpellBase.DamageType, (Stats) SpellBase.ScalingStat, SpellBase.Scaling,
                        SpellBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
                Interval = Globals.System.GetTimeMs() + (SpellBase.Data4 * 100);
                Count--;

                if (Count <= 0 && Target != null)
                {
                    Target.DoT.Remove(this);
                }
            }
        }
    }

    public class StatusInstance
    {
        public SpellBase _spell;
        public string Data = "";
        public long Duration;
        private Entity entity;
        public long StartTime;
        public int Type;

        public StatusInstance(Entity en, SpellBase spell, int type, int duration, string data)
        {
            entity = en;
            _spell = spell;
            Type = type;
            Duration = Globals.System.GetTimeMs() + duration;
            StartTime = Globals.System.GetTimeMs();
            Data = data;
            if (en.Statuses.ContainsKey(spell))
            {
                en.Statuses[spell].Duration = Duration;
            }
            else
            {
                en.Statuses.Add(_spell, this);
            }
            PacketSender.SendEntityVitals(entity);
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
            entity.Statuses.Remove(_spell);
            PacketSender.SendEntityVitals(entity);
        }
    }

    public class DashInstance
    {
        public bool ActiveResourcePass;

        public bool BlockPass;
        public bool DeadResourcePass;
        public int Direction;
        public int DistanceTraveled;
        public int Facing;
        public int Range;
        public long TransmittionTimer;
        public bool ZDimensionPass;

        public DashInstance(Entity en, int range, int direction, bool blockPass = false,
            bool activeResourcePass = false,
            bool deadResourcePass = false, bool zdimensionPass = false)
        {
            DistanceTraveled = 0;
            Direction = direction;
            Facing = en.Dir;

            BlockPass = blockPass;
            ActiveResourcePass = activeResourcePass;
            DeadResourcePass = deadResourcePass;
            ZDimensionPass = zdimensionPass;

            CalculateRange(en, range);
            if (Range <= 0)
            {
                return;
            } //Remove dash instance if no where to dash
            TransmittionTimer = Globals.System.GetTimeMs() + (long) ((float) Options.MaxDashSpeed / (float) Range);
            PacketSender.SendEntityDash(en, en.CurrentMap, en.CurrentX, en.CurrentY,
                (int) (Options.MaxDashSpeed * (Range / 10f)), Direction == Facing ? Direction : -Facing);
            en.MoveTimer = Globals.System.GetTimeMs() + Options.MaxDashSpeed;
        }

        public void CalculateRange(Entity en, int range)
        {
            int n = 0;
            en.MoveTimer = 0;
            Range = 0;
            for (int i = 1; i <= range; i++)
            {
                n = en.CanMove(Direction);
                if (n == -5)
                {
                    return;
                } //Check for out of bounds
                if (n == -2 && BlockPass == false)
                {
                    return;
                } //Check for blocks
                if (n == -3 && ZDimensionPass == false)
                {
                    return;
                } //Check for ZDimensionTiles
                if (n == (int) EntityTypes.Resource && ActiveResourcePass == false)
                {
                    return;
                } //Check for active resources
                if (n == (int) EntityTypes.Resource && DeadResourcePass == false)
                {
                    return;
                } //Check for dead resources
                if (n == (int) EntityTypes.Player) return;

                en.Move(Direction, null, true);
                en.Dir = Facing;

                Range = i;
                if (n == -4) return;
            }
        }
    }
}
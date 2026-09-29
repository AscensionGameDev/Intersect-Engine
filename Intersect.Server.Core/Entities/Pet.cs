using Intersect;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Pets;
using Intersect.GameObjects;
using Intersect.Server.Framework.Items;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities.Pathfinding;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities;

/// <summary>
/// A companion that follows its owner around and fetches loot for them.
/// Pets are passable, cannot be attacked and never fight: their only job is to walk to
/// items lying on the ground (that their owner is allowed to pick up) and put them in the
/// owner's inventory, then come back next to the owner.
/// </summary>
public partial class Pet : Entity
{
    /// <summary>
    /// How far (in tiles) the pet may drift from its owner before being teleported back.
    /// </summary>
    private const int MaxDistanceFromOwner = 12;

    /// <summary>
    /// How often the pet re-evaluates what it should be doing.
    /// </summary>
    private const long ThinkIntervalMs = 200;

    private readonly Pathfinder _pathfinder;

    private long _nextThinkTime;

    private Guid _targetItemId = Guid.Empty;

    private Guid _targetItemMapId = Guid.Empty;

    public Pet(PetDescriptor descriptor, Player owner) : base(Guid.NewGuid(), owner.MapInstanceId)
    {
        Descriptor = descriptor;
        Owner = owner;

        Name = descriptor.Name;
        Sprite = descriptor.Sprite;
        Level = owner.Level;
        Passable = true;

        MapId = owner.MapId;
        X = owner.X;
        Y = owner.Y;
        Z = owner.Z;
        Dir = owner.Dir;

        Stat[(int)Enums.Stat.Speed].BaseStat = Math.Max(1, descriptor.Speed);

        // Pets do not fight, keep vitals trivially "full" so no health bar shows up client-side.
        for (var vital = 0; vital < Enum.GetValues<Vital>().Length; vital++)
        {
            SetMaxVital(vital, 1);
            SetVital(vital, 1);
        }

        _pathfinder = new Pathfinder(this);
    }

    public PetDescriptor Descriptor { get; }

    public Player Owner { get; }

    public override EntityType GetEntityType() => EntityType.GlobalEntity;

    public override bool CanAttack(Entity entity, SpellDescriptor spell) => false;

    // Pets never drop or spawn items themselves.
    protected override EntityItemSource? AsItemSource() => null;

    /// <summary>
    /// Pets are passable, so other entities don't block them either: loot very often lies under the
    /// owner's (or a monster's) feet and the pet must be able to step onto that tile to grab it.
    /// </summary>
    public override bool CanMoveInDirection(
        Direction direction,
        out MovementBlockerType blockerType,
        out EntityType entityType,
        out Entity? blockingEntity
    )
    {
        var canMove = base.CanMoveInDirection(direction, out blockerType, out entityType, out blockingEntity);
        if (!canMove && blockerType == MovementBlockerType.Entity)
        {
            blockerType = MovementBlockerType.NotBlocked;
            blockingEntity = null;

            return true;
        }

        return canMove;
    }

    public override void Update(long timeMs)
    {
        if (IsDisposed)
        {
            return;
        }

        if (Owner.IsDisposed || Owner.ActivePet != this)
        {
            Despawn();

            return;
        }

        base.Update(timeMs);

        if (timeMs < _nextThinkTime)
        {
            return;
        }

        _nextThinkTime = timeMs + ThinkIntervalMs;

        // Map/instance transitions are handled deterministically by Player.SpawnPet()/DespawnPet()
        // (called from MapInstance.PlayerEnteredMap/RemoveEntity when the owner actually enters/leaves
        // a map instance). Reacting to a mismatch here too raced with that system: both could try to
        // relocate/dispose the pet at the same time, leaving a stale duplicate stuck on the old map
        // while a fresh one followed the owner. Just wait for the deterministic path once mismatched.
        if (Owner.MapInstanceId != MapInstanceId || Owner.MapId == Guid.Empty)
        {
            return;
        }

        // Still on the same map/instance as the owner but lagging behind: catch up instantly.
        if (GetDistanceTo(Owner) > MaxDistanceFromOwner)
        {
            TeleportToOwner();

            return;
        }

        if (Timing.Global.Milliseconds < MoveTimer)
        {
            return;
        }

        var targetItem = FindTargetItem(out var targetMap);
        if (targetItem != null && targetMap != null)
        {
            // MapInstance.Id is the instance's own id; the map id lives on its controller.
            var targetMapId = targetMap.GetController().Id;
            if (MapId == targetMapId && X == targetItem.X && Y == targetItem.Y)
            {
                PickUp(targetItem, targetMap);
                ClearItemTarget();

                return;
            }

            MoveTowards(targetMapId, targetItem.X, targetItem.Y, timeMs, stopWhenAdjacent: false);

            return;
        }

        ClearItemTarget();

        // Nothing to fetch: stay next to the owner.
        if (!IsOneBlockAway(Owner.MapId, Owner.X, Owner.Y, Owner.Z) && GetDistanceTo(Owner) > 0)
        {
            MoveTowards(Owner.MapId, Owner.X, Owner.Y, timeMs, stopWhenAdjacent: true);
        }
        else if (Dir != Owner.Dir && GetDistanceTo(Owner) > 0)
        {
            ChangeDir(Owner.Dir);
        }
    }

    /// <summary>
    /// Finds the closest item the pet is allowed to fetch, within the descriptor's pickup range of the owner.
    /// Keeps the current target as long as it is still valid so the pet doesn't dither between items.
    /// </summary>
    private MapItem? FindTargetItem(out MapInstance? targetMap)
    {
        targetMap = null;

        // Keep going for the item we already picked if it's still there.
        if (_targetItemId != Guid.Empty &&
            MapController.TryGetInstanceFromMap(_targetItemMapId, MapInstanceId, out var currentTargetMap))
        {
            var currentTarget = currentTargetMap.FindItem(_targetItemId);
            if (currentTarget != null && CanFetch(currentTarget))
            {
                targetMap = currentTargetMap;

                return currentTarget;
            }
        }

        ClearItemTarget();

        var ownerMap = Owner.Map;
        if (ownerMap == null)
        {
            return null;
        }

        MapItem? best = null;
        var bestDistance = int.MaxValue;
        var range = Math.Max(0, Descriptor.PickupRange);

        foreach (var (mapController, tileIndices) in ownerMap.FindSurroundingTiles(new Point(Owner.X, Owner.Y), range))
        {
            if (!mapController.TryGetInstance(MapInstanceId, out var mapInstance))
            {
                continue;
            }

            foreach (var tileIndex in tileIndices)
            {
                foreach (var mapItem in mapInstance.FindItemsAt(tileIndex))
                {
                    if (!CanFetch(mapItem))
                    {
                        continue;
                    }

                    var distance = GetDistanceTo(mapController, mapItem.X, mapItem.Y);
                    if (distance >= bestDistance)
                    {
                        continue;
                    }

                    best = mapItem;
                    bestDistance = distance;
                    targetMap = mapInstance;
                }
            }
        }

        if (best != null && targetMap != null)
        {
            _targetItemId = best.UniqueId;
            _targetItemMapId = targetMap.GetController().Id;
        }

        return best;
    }

    /// <summary>
    /// Mirrors the ownership rules of the regular pickup packet, plus the pet's own options.
    /// </summary>
    private bool CanFetch(MapItem mapItem)
    {
        if (mapItem == null)
        {
            return false;
        }

        if (mapItem.DroppedByPlayer && mapItem.Owner == Owner.Id && !Descriptor.PickupOwnerDrops)
        {
            return false;
        }

        if (mapItem.Owner == Owner.Id)
        {
            return true;
        }

        // Still reserved for someone else.
        if (mapItem.Owner != Guid.Empty && Timing.Global.Milliseconds < mapItem.OwnershipTime)
        {
            return false;
        }

        return Descriptor.PickupUnownedItems;
    }

    private void PickUp(MapItem mapItem, MapInstance mapInstance)
    {
        // Make sure it is still there (another player may have grabbed it in the meantime).
        if (mapInstance.FindItem(mapItem.UniqueId) == null)
        {
            return;
        }

        // Don't take it off the ground if the owner has no room for it.
        if (!Owner.CanGiveItem(mapItem, out _))
        {
            return;
        }

        mapInstance.RemoveItem(mapItem);

        if (!Owner.TryGiveItem(mapItem, ItemHandling.Overflow, false, -1, true, mapItem.X, mapItem.Y))
        {
            return;
        }

        if (ItemDescriptor.TryGet(mapItem.ItemId, out var item))
        {
            PacketSender.SendActionMsg(this, item.Name, CustomColors.Items.Rarities[item.Rarity]);
        }
    }

    private void MoveTowards(Guid mapId, int x, int y, long timeMs, bool stopWhenAdjacent)
    {
        var currentTarget = _pathfinder.GetTarget();
        if (currentTarget == null || currentTarget.TargetMapId != mapId || currentTarget.TargetX != x || currentTarget.TargetY != y)
        {
            _pathfinder.SetTarget(new PathfinderTarget(mapId, x, y, Z));
        }

        if (stopWhenAdjacent && IsOneBlockAway(mapId, x, y, Z))
        {
            return;
        }

        var direction = Direction.None;
        var result = _pathfinder.Update(timeMs);
        if (result.Type == PathfinderResultType.Success)
        {
            direction = _pathfinder.GetMove();
        }

        // The pathfinder can't help (blocked / out of range / cooling down): fall back to walking straight at the goal.
        if (direction == Direction.None)
        {
            direction = DirectionTo(mapId, x, y);
        }

        if (direction == Direction.None)
        {
            return;
        }

        if (CanMoveInDirection(direction, out var blockerType, out _, out _) || blockerType == MovementBlockerType.Slide)
        {
            Move(direction, null);
        }
        else
        {
            _pathfinder.PathFailed(timeMs);
        }
    }

    /// <summary>
    /// Straight-line direction towards a tile (possibly on a neighbouring map).
    /// </summary>
    private Direction DirectionTo(Guid mapId, int x, int y)
    {
        var map = MapController.Get(mapId);
        var myMap = Map;
        if (map == null || myMap == null)
        {
            return Direction.None;
        }

        var deltaX = x - X;
        var deltaY = y - Y;
        if (map.Id != myMap.Id)
        {
            deltaX += (map.MapGridX - myMap.MapGridX) * Options.Instance.Map.MapWidth;
            deltaY += (map.MapGridY - myMap.MapGridY) * Options.Instance.Map.MapHeight;
        }

        if (deltaX == 0 && deltaY == 0)
        {
            return Direction.None;
        }

        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            return deltaX > 0 ? Direction.Right : Direction.Left;
        }

        return deltaY > 0 ? Direction.Down : Direction.Up;
    }

    private void ClearItemTarget()
    {
        _targetItemId = Guid.Empty;
        _targetItemMapId = Guid.Empty;
    }

    /// <summary>
    /// Puts the pet right on top of its owner (used on warps and when it lags too far behind).
    /// </summary>
    public void TeleportToOwner()
    {
        if (!MapController.TryGetInstanceFromMap(Owner.MapId, Owner.MapInstanceId, out var newInstance))
        {
            return;
        }

        var mapChanged = MapId != Owner.MapId || MapInstanceId != Owner.MapInstanceId;
        if (mapChanged && MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var oldInstance))
        {
            oldInstance.RemoveEntity(this);
            PacketSender.SendEntityLeave(this);
        }

        _pathfinder.SetTarget(null);
        ClearItemTarget();

        MapInstanceId = Owner.MapInstanceId;
        MapId = Owner.MapId;
        X = Owner.X;
        Y = Owner.Y;
        Z = Owner.Z;
        Dir = Owner.Dir;

        if (mapChanged)
        {
            newInstance.AddEntity(this);
            PacketSender.SendEntityDataToProximity(this);
        }

        PacketSender.SendEntityPositionToAll(this);
    }

    /// <summary>
    /// Removes the pet from the world.
    /// </summary>
    public void Despawn()
    {
        if (IsDisposed)
        {
            return;
        }

        if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var mapInstance))
        {
            mapInstance.RemoveEntity(this);
        }

        PacketSender.SendEntityLeave(this);

        // The proximity broadcast above can miss the owner: by the time a pet despawns because its
        // owner is leaving a map, the owner has usually already been removed from that map's player
        // list, so they never receive the notification. Their client then keeps the pet cached as
        // still present, and it resurfaces as a frozen ghost if they revisit that map later.
        if (Owner.Client != null)
        {
            PacketSender.SendEntityLeaveTo(Owner, this);
        }

        Dispose();
    }

    public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
    {
        packet ??= new PetEntityPacket();

        packet = base.EntityPacket(packet, forPlayer);

        var pkt = (PetEntityPacket)packet;
        pkt.OwnerId = Owner.Id;
        pkt.PetDescriptorId = Descriptor.Id;

        return pkt;
    }
}

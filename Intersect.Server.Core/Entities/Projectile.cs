using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Framework.Items;
using Intersect.Server.Maps;
using Intersect.Utilities;

namespace Intersect.Server.Entities;


public partial class Projectile : Entity
{

    public ProjectileBase Base;

    public bool HasGrappled;

    public ItemBase Item;

    private int _quantity;

    private int _spawnCount;

    private int _spawnedAmount;

    private long _spawnTime;

    private int _totalSpawns;

    public Entity Owner;

    // Individual Spawns
    public ProjectileSpawn[] Spawns;

    public SpellBase Spell;

    public new Entity Target;

    private int _lastTargetX = -1;

    private int _lastTargetY = -1;

    private Guid _lastTargetMapId = Guid.Empty;

    public Projectile(
        Entity owner,
        SpellBase parentSpell,
        ItemBase parentItem,
        ProjectileBase projectile,
        Guid mapId,
        byte X,
        byte Y,
        byte z,
        Direction direction,
        Entity target
    ) : base()
    {
        Base = projectile;
        Name = Base.Name;
        Owner = owner;
        Target = owner.Target;
        Stat = owner.Stat;
        MapId = mapId;
        base.X = X;
        base.Y = Y;
        Z = z;
        SetMaxVital(Vital.Health, 1);
        SetVital(Vital.Health, 1);
        Dir = direction;
        Spell = parentSpell;
        Item = parentItem;

        Passable = true;
        HideName = true;
        for (var x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
        {
            for (var y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
            {
                for (var d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                {
                    if (Base.SpawnLocations[x, y].Directions[d] == true)
                    {
                        _totalSpawns++;
                    }
                }
            }
        }

        _totalSpawns *= Base.Quantity;
        Spawns = new ProjectileSpawn[_totalSpawns];
    }

    /// <summary>
    /// Adds projectile spawns based on predefined spawn locations and directions.
    /// </summary>
    private void AddProjectileSpawns()
    {
        // Iterate over all possible spawn locations within the defined width and height
        for (byte x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
        {
            for (byte y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
            {
                // Iterate over all possible directions a projectile can be spawned in
                for (byte d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                {
                    // Check if the current direction is enabled for spawning at this location
                    // and if the maximum number of spawned projectiles has not been reached
                    if (Base.SpawnLocations[x, y].Directions[d] && _spawnedAmount < Spawns.Length)
                    {
                        // Calculate the spawn position and direction for the new projectile
                        var s = new ProjectileSpawn(
                            FindProjectileRotationDir(Dir, (Direction)d),
                            (byte)(X + FindProjectileRotation(Dir, x - 2, y - 2, true)),
                            (byte)(Y + FindProjectileRotation(Dir, x - 2, y - 2, false)),
                            (byte)Z, MapId, MapInstanceId, Base, this
                        );

                        // Add the new spawn to the array and increment counters
                        Spawns[_spawnedAmount] = s;
                        _spawnedAmount++;
                        _spawnCount++;

                        if (CheckForCollision(s))
                        {
                            s.Dead = true;
                        }
                    }
                }
            }
        }

        // Increment the quantity of projectiles spawned and update the spawn time based on the delay
        _quantity++;
        _spawnTime = Timing.Global.Milliseconds + Base.Delay;
    }

    /// <summary>
    /// Finds the projectile rotation value based on the direction, position and the axis.
    /// </summary>
    /// <param name="direction">The direction of the projectile.</param>
    /// <param name="x">The x-coordinate value.</param>
    /// <param name="y">The y-coordinate value.</param>
    /// <param name="isXAxis">True for horizontal (X-axis), false for vertical (Y-axis) calculation.</param>
    /// <returns>The rotation value for the specified axis based on the direction.</returns>
    private static int FindProjectileRotation(Direction direction, int x, int y, bool isXAxis)
    {
        if (isXAxis)
        {
            return direction switch
            {
                Direction.Up => x,
                Direction.Down => -x,
                Direction.Left or Direction.UpLeft or Direction.DownLeft => y,
                Direction.Right or Direction.UpRight or Direction.DownRight => -y,
                _ => x,
            };
        }
        else
        {
            return direction switch
            {
                Direction.Up => y,
                Direction.Down => -y,
                Direction.Left or Direction.UpLeft or Direction.DownLeft => -x,
                Direction.Right or Direction.UpRight or Direction.DownRight => x,
                _ => y,
            };
        }
    }

    private static Direction FindProjectileRotationDir(Direction entityDir, Direction projectionDir) =>
        (Direction)ProjectileBase.ProjectileRotationDir[(int)entityDir * ProjectileBase.MAX_PROJECTILE_DIRECTIONS + (int)projectionDir];

    public void Update(List<Guid> projDeaths, List<KeyValuePair<Guid, int>> spawnDeaths)
    {
        if (_quantity < Base.Quantity && Timing.Global.Milliseconds > _spawnTime)
        {
            AddProjectileSpawns();
        }

        ProcessFragments(projDeaths, spawnDeaths);
    }

    /// <summary>
    /// Calculates the projectile range based on the given direction, range, and axis.
    /// </summary>
    /// <param name="direction">The direction of the projectile.</param>
    /// <param name="range">The range of the projectile.</param>
    /// <param name="isXAxis">True for horizontal (X-axis), false for vertical (Y-axis) calculation.</param>
    /// <returns>The calculated range value.</returns>
    private static float GetRange(Direction direction, float range, bool isXAxis)
    {
        if (isXAxis)
        {
            return direction switch
            {
                Direction.Left or Direction.UpLeft or Direction.DownLeft => -range,
                Direction.Right or Direction.UpRight or Direction.DownRight => range,
                _ => 0,
            };
        }
        else
        {
            return direction switch
            {
                Direction.Up or Direction.UpLeft or Direction.UpRight => -range,
                Direction.Down or Direction.DownLeft or Direction.DownRight => range,
                _ => 0,
            };
        }
    }

    public void ProcessFragments(List<Guid> projDeaths, List<KeyValuePair<Guid, int>> spawnDeaths)
    {
        if (Base == null)
        {
            return;
        }

        if (_spawnCount != 0 || _quantity < Base.Quantity)
        {
            for (var i = 0; i < _spawnedAmount; i++)
            {
                var spawn = Spawns[i];
                if (spawn != null)
                {
                    while (Timing.Global.Milliseconds > spawn.TransmissionTimer && Spawns[i] != null)
                    {
                        var x = spawn.X;
                        var y = spawn.Y;
                        var map = spawn.MapId;
                        var killSpawn = false;
                        if (!spawn.Dead)
                        {
                            killSpawn = MoveFragment(spawn);
                            if (!killSpawn && (x != spawn.X || y != spawn.Y || map != spawn.MapId))
                            {
                                killSpawn = CheckForCollision(spawn);
                            }
                        }

                        if (killSpawn || spawn.Dead || CheckForCollision(spawn))
                        {
                            spawnDeaths.Add(new KeyValuePair<Guid, int>(Id, i));
                            Spawns[i] = null;
                            _spawnCount--;
                        }
                    }
                }
            }
        }
        else
        {
            lock (EntityLock)
            {
                projDeaths.Add(Id);
                Die(false);
            }
        }
    }

    /// <summary>
    /// Checks for collision between the projectile with other entities and attributes on the map.
    /// </summary>
    /// <param name="spawn">The projectile spawn information.</param>
    /// <returns>True if the projectile collides with an entity or attributes, false otherwise.</returns>
    public bool CheckForCollision(ProjectileSpawn spawn)
    {
        if (spawn == null)
        {
            return false;
        }

        var killSpawn = MoveFragment(spawn, false);

        //Check Map Entities For Hits
        var map = MapController.Get(spawn.MapId);
        if (map == null)
        {
            return false;
        }

        var roundedX = MathF.Round(spawn.X);
        var roundedY = MathF.Round(spawn.Y);

        //Checking if the coordinates are within the map boundaries
        if (roundedX < 0 || roundedX >= Options.Instance.Map.MapWidth ||
            roundedY < 0 || roundedY >= Options.Instance.Map.MapHeight)
        {
            return false;
        }

        GameObjects.Maps.MapAttribute attribute;

        // Before accessing map attributes, check if the coordinates are within the bounds of the map's attribute array.
        if (roundedX >= 0 && roundedX < map.Attributes.GetLength(0) &&
            roundedY >= 0 && roundedY < map.Attributes.GetLength(1))
        {
            attribute = map.Attributes[(int)roundedX, (int)roundedY];
        }
        else
        {
            attribute = null;
        }

        if (!killSpawn && attribute != null)
        {
            // Check for Z-Dimension
            if (!spawn.ProjectileBase.IgnoreZDimension && attribute is MapZDimensionAttribute zDimAttr && zDimAttr != null)
            {
                // If the Z dimension attribute specifies a blocked level that matches the projectile's current Z level,
                // mark the projectile for destruction.
                if (zDimAttr.BlockedLevel > 0 && spawn.Z == zDimAttr.BlockedLevel - 1)
                {
                    killSpawn = true;
                }

                // If the Z dimension attribute specifies a gateway to another level,
                // adjust the projectile's Z level to the specified gateway level.
                if (zDimAttr.GatewayTo > 0)
                {
                    spawn.Z = (byte)(zDimAttr.GatewayTo - 1);
                }
            }

            //Check for grapplehooks.
            if (attribute.Type == MapAttributeType.GrappleStone &&
                Base.GrappleHookOptions.Contains(GrappleOption.MapAttribute) &&
                !spawn.Parent.HasGrappled &&
                (spawn.X != Owner.X || spawn.Y != Owner.Y))
            {
                // Grapple hooks are only allowed in the default projectile behavior
                if (!spawn.ProjectileBase.HomingBehavior && !spawn.ProjectileBase.DirectShotBehavior &&
                    (spawn.Dir <= Direction.Right || (spawn.Dir != Direction.None && Options.Instance.Map.EnableDiagonalMovement)))
                {
                    spawn.Parent.HasGrappled = true;

                    //Only grapple if the player hasnt left the firing position.. if they have then we assume they dont wanna grapple
                    if (Owner.X == X && Owner.Y == Y && Owner.MapId == MapId)
                    {
                        Owner.Dir = spawn.Dir;
                        new Dash(
                            Owner, spawn.Distance, Owner.Dir, Base.IgnoreMapBlocks,
                            Base.IgnoreActiveResources, Base.IgnoreExhaustedResources, Base.IgnoreZDimension
                        );
                    }

                    killSpawn = true;
                }
            }

            if (!spawn.ProjectileBase.IgnoreMapBlocks &&
                ((attribute.Type == MapAttributeType.Blocked) || (attribute.Type == MapAttributeType.Animation && ((MapAnimationAttribute)attribute).IsBlock)))
            {
                killSpawn = true;
            }
        }

        if (!killSpawn && MapController.TryGetInstanceFromMap(map.Id, MapInstanceId, out var mapInstance))
        {
            var entities = mapInstance.GetEntities();
            for (var z = 0; z < entities.Count; z++)
            {
                var entity = entities[z];
                if (entity != null && entity != spawn.Parent.Owner && entity.Z == spawn.Z &&
                    entity.X == roundedX && entity.Y == roundedY &&
                    (spawn.X != Owner.X || spawn.Y != Owner.Y))
                {
                    killSpawn = spawn.HitEntity(entity);
                    if (killSpawn && !spawn.ProjectileBase.PierceTarget)
                    {
                        return killSpawn;
                    }
                }
                else if (z == entities.Count - 1 && spawn.Distance >= Base.Range)
                {
                    killSpawn = true;
                }
            }
        }

        return killSpawn || spawn?.Distance >= Base.Range;
    }

    /// <summary>
    /// Calculates the offset for the projectile spawn position based on the target map and spawn location.
    /// </summary>
    /// <param name="spawn">The projectile spawn information.</param>
    /// <param name="isXAxis">True for horizontal (X-axis), false for vertical (Y-axis) calculation.</param>
    /// <returns>The calculated offset value.</returns>
    private float GetProjectileOffset(ProjectileSpawn spawn, bool isXAxis)
    {
        if (_lastTargetMapId == Guid.Empty || _lastTargetMapId == spawn.MapId || !MapController.TryGet(spawn.MapId, out var map))
        {
            return isXAxis ? _lastTargetX - spawn.X : _lastTargetY - spawn.Y;
        }

        var grid = DbInterface.GetGrid(map.MapGrid);

        for (var y = map.MapGridY - 1; y <= map.MapGridY + 1; y++)
        {
            for (var x = map.MapGridX - 1; x <= map.MapGridX + 1; x++)
            {
                if (x < 0 || x >= grid.MapIdGrid.GetLength(0) || y < 0 || y >= grid.MapIdGrid.GetLength(1))
                {
                    continue;
                }

                if (grid.MapIdGrid[x, y] != _lastTargetMapId || grid.MapIdGrid[x, y] == Guid.Empty)
                {
                    continue;
                }

                if (isXAxis)
                {
                    var leftSide = x == map.MapGridX - 1;
                    var rightSide = x == map.MapGridX + 1;

                    if (leftSide)
                    {
                        return _lastTargetX - Options.Instance.Map.MapWidth - spawn.X;
                    }

                    if (rightSide)
                    {
                        return _lastTargetX + Options.Instance.Map.MapWidth - spawn.X;
                    }
                }
                else
                {
                    var topSide = y == map.MapGridY - 1;
                    var bottomSide = y == map.MapGridY + 1;

                    if (topSide)
                    {
                        return _lastTargetY - Options.Instance.Map.MapHeight - spawn.Y;
                    }

                    if (bottomSide)
                    {
                        return _lastTargetY + Options.Instance.Map.MapHeight - spawn.Y;
                    }
                }
            }
        }

        return isXAxis ? _lastTargetX - spawn.X : _lastTargetY - spawn.Y;
    }

    /// <summary>
    /// Moves the projectile fragment based on the specified spawn information. This method is responsible for
    /// updating the position of the projectile, handling homing and direct shot behaviors, and checking for out-of-bounds conditions.
    /// </summary>
    /// <param name="spawn">The projectile spawn information, including current position, direction, and other relevant details.</param>
    /// <param name="move">Indicates whether the fragment should be moved. This can be used to pause movement if needed. Default is true.</param>
    /// <returns>True if the projectile goes out of bounds after moving, false otherwise.</returns>
    public bool MoveFragment(ProjectileSpawn spawn, bool move = true)
    {
        if (move)
        {
            // Increase the distance traveled by the projectile and update its transmission timer.
            spawn.Distance++;
            spawn.TransmissionTimer += (long)(Base.Speed / (float)Base.Range);

            if (Target != default && Target.Id != Owner.Id && (Base.HomingBehavior || Base.DirectShotBehavior))
            {
                // Homing or direct shot logic: Adjusts the projectile's trajectory towards the target.
                _lastTargetX = Target.X;
                _lastTargetY = Target.Y;
                _lastTargetMapId = Target.MapId;
                var (directionX, directionY) = (GetProjectileOffset(spawn, true), GetProjectileOffset(spawn, false));
                var distance = MathF.Sqrt(directionX * directionX + directionY * directionY);

                // Normalize the direction and update the projectile's position.
                spawn.X += directionX / distance;
                spawn.Y += directionY / distance;

                // For direct shots, reset the target after moving towards it once.
                if (Base.DirectShotBehavior)
                {
                    Target = default;
                }
            }
            else if (_lastTargetX != -1 && _lastTargetY != -1)
            {
                // Last known target location logic: Moves the projectile towards the last known position of the target.
                var (directionX, directionY) = (GetProjectileOffset(spawn, true), GetProjectileOffset(spawn, false));
                var distance = MathF.Sqrt(directionX * directionX + directionY * directionY);

                // Normalize the direction and update the projectile's position.
                spawn.X += directionX / distance;
                spawn.Y += directionY / distance;
            }
            else
            {
                // Default movement logic: Moves the projectile in its current direction.
                spawn.X += GetRange(spawn.Dir, 1, true);
                spawn.Y += GetRange(spawn.Dir, 1, false);
            }
        }

        // Adjust the projectile's position if it crosses map boundaries and potentially transitions it to a neighboring map.
        AdjustPositionOnMapBoundaries(ref spawn.X, ref spawn.Y, ref spawn);

        // Check for map boundaries and remove the spawn if the projectile has gone out of bounds after moving.
        return spawn.X < 0 || spawn.X >= Options.Instance.Map.MapWidth || spawn.Y < 0 || spawn.Y >= Options.Instance.Map.MapHeight;
    }

    /// <summary>
    /// Adjusts the position of a projectile when it crosses the boundaries of the current map.
    /// This method checks if the projectile crosses the map boundaries and, if so, attempts to
    /// transition the projectile to the adjacent map in the direction it is moving. The new position
    /// on the adjacent map is calculated to maintain the continuity of the projectile's path.
    /// </summary>
    /// <param name="newx">The new x-coordinate of the projectile, which may be adjusted if crossing map boundaries.</param>
    /// <param name="newy">The new y-coordinate of the projectile, which may be adjusted if crossing map boundaries.</param>
    /// <param name="spawn">The projectile spawn information, including the current map ID, which may be updated to a new map.</param>
    private static void AdjustPositionOnMapBoundaries(ref float newx, ref float newy, ref ProjectileSpawn spawn)
    {
        // Retrieve the current map based on the projectile's spawn information.
        var map = MapController.Get(spawn.MapId);
        int MapWidth = Options.Instance.Map.MapWidth;
        int MapHeight = Options.Instance.Map.MapHeight;

        // Determine if the projectile crosses any of the map's boundaries.
        bool crossesLeftBoundary = MathF.Floor(newx) < 0;
        bool crossesRightBoundary = MathF.Ceiling(newx) > MapWidth - 1;
        bool crossesTopBoundary = MathF.Floor(newy) < 0;
        bool crossesBottomBoundary = MathF.Ceiling(newy) > MapHeight - 1;

        // Check for corner cases where the projectile crosses two boundaries.
        if (crossesLeftBoundary && crossesTopBoundary)
        {
            // Projectile crosses the top-left corner of the map.
            if (MapController.TryGet(map.Up, out var upMap) &&
                MapController.TryGet(upMap.Left, out var upLeftMap))
            {
                // Transition to the map diagonally up-left and adjust position to bottom-right corner.
                spawn.MapId = upLeftMap.Id;
                newx = MapWidth - 1;
                newy = MapHeight - 1;
            }
        }
        else if (crossesRightBoundary && crossesTopBoundary)
        {
            // Projectile crosses the top-right corner of the map.
            if (MapController.TryGet(map.Up, out var upMap) &&
                MapController.TryGet(upMap.Right, out var upRightMap))
            {
                // Transition to the map diagonally up-right and adjust position to bottom-left corner.
                spawn.MapId = upRightMap.Id;
                newx = 0;
                newy = MapHeight - 1;
            }
        }
        else if (crossesLeftBoundary && crossesBottomBoundary)
        {
            // Projectile crosses the bottom-left corner of the map.
            if (MapController.TryGet(map.Down, out var downMap) &&
                MapController.TryGet(downMap.Left, out var downLeftMap))
            {
                // Transition to the map diagonally down-left and adjust position to top-right corner.
                spawn.MapId = downLeftMap.Id;
                newx = MapWidth - 1;
                newy = 0;
            }
        }
        else if (crossesRightBoundary && crossesBottomBoundary)
        {
            // Projectile crosses the bottom-right corner of the map.
            if (MapController.TryGet(map.Down, out var downMap) &&
                MapController.TryGet(downMap.Right, out var downRightMap))
            {
                // Transition to the map diagonally down-right and adjust position to top-left corner.
                spawn.MapId = downRightMap.Id;
                newx = 0;
                newy = 0;
            }
        }
        // Check for single boundary crossings and adjust position accordingly.
        else if (crossesLeftBoundary)
        {
            // Projectile crosses the left boundary of the map.
            if (MapController.TryGet(map.Left, out var leftMap))
            {
                // Transition to the map on the left and adjust position to the right edge.
                spawn.MapId = leftMap.Id;
                newx = MapWidth - 1;
            }
        }
        else if (crossesRightBoundary)
        {
            // Projectile crosses the right boundary of the map.
            if (MapController.TryGet(map.Right, out var rightMap))
            {
                // Transition to the map on the right and adjust position to the left edge.
                spawn.MapId = rightMap.Id;
                newx = 0;
            }
        }
        else if (crossesTopBoundary)
        {
            // Projectile crosses the top boundary of the map.
            if (MapController.TryGet(map.Up, out var upMap))
            {
                // Transition to the map above and adjust position to the bottom edge.
                spawn.MapId = upMap.Id;
                newy = MapHeight - 1;
            }
        }
        else if (crossesBottomBoundary)
        {
            // Projectile crosses the bottom boundary of the map.
            if (MapController.TryGet(map.Down, out var downMap))
            {
                // Transition to the map below and adjust position to the top edge.
                spawn.MapId = downMap.Id;
                newy = 0;
            }
        }
    }

    public override void Die(bool dropItems = true, Entity killer = null)
    {
        for (var i = 0; i < Spawns.Length; i++)
        {
            Spawns[i] = null;
        }

        if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var mapInstance))
        {
            mapInstance.RemoveProjectile(this);
        }
    }

    public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
    {
        packet ??= new ProjectileEntityPacket();
        packet = base.EntityPacket(packet, forPlayer);

        var pkt = (ProjectileEntityPacket)packet;
        pkt.ProjectileId = Base.Id;
        pkt.ProjectileDirection = (byte)Dir;
        pkt.TargetId = Owner.Target != default && Owner.Target.Id != Guid.Empty ? Owner.Target.Id : Guid.Empty;
        pkt.OwnerId = Owner?.Id ?? Guid.Empty;

        return pkt;
    }

    public override EntityType GetEntityType()
    {
        return EntityType.Projectile;
    }
    
    protected override EntityItemSource? AsItemSource()
    {
        return null;
    }

}

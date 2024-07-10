using Intersect.Client.Framework.Entities;
using Intersect.Client.General;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using MapAttribute = Intersect.Enums.MapAttribute;

namespace Intersect.Client.Entities.Projectiles;

public partial class Projectile : Entity
{
    private bool _isDisposing;

    private bool _isLoaded;

    private readonly object _lock = new();

    private ProjectileBase? _myBase;

    private Guid _owner;

    private int _quantity;

    private int _spawnCount;

    private int _spawnedAmount;

    private long _spawnTime;

    private int _totalSpawns;

    private Guid _projectileId;

    // Individual Spawns
    private ProjectileSpawns?[]? _spawns;

    private Guid _targetId;

    private int _lastTargetX = -1;

    private int _lastTargetY = -1;

    private Guid _lastTargetMapId = Guid.Empty;

    /// <summary>
    /// The constructor for the inherated projectile class
    /// </summary>
    public Projectile(Guid id, ProjectileEntityPacket packet) : base(id, packet, EntityType.Projectile)
    {
        Vital[(int)Enums.Vital.Health] = 1;
        MaxVital[(int)Enums.Vital.Health] = 1;
        HideName = true;
        Passable = true;
        IsMoving = true;
    }

    public override void Load(EntityPacket? packet)
    {
        if (_isLoaded)
        {
            return;
        }

        base.Load(packet);

        if (packet == null)
        {
            return;
        }

        var pkt = (ProjectileEntityPacket)packet;
        _projectileId = pkt.ProjectileId;
        Dir = (Direction)pkt.ProjectileDirection;
        _targetId = pkt.TargetId;
        _owner = pkt.OwnerId;
        _myBase = ProjectileBase.Get(_projectileId);
        if (_myBase != null)
        {
            for (var x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (var y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    for (var d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                    {
                        if (_myBase.SpawnLocations[x, y].Directions[d] == true)
                        {
                            _totalSpawns++;
                        }
                    }
                }
            }

            _totalSpawns *= _myBase.Quantity;
        }

        _spawns = new ProjectileSpawns[_totalSpawns];
        _isLoaded = true;
    }

    /// <summary>
    /// Disposes of the resources used by this instance, preventing any further use.
    /// </summary>
    public override void Dispose()
    {
        if (!_isDisposing)
        {
            lock (_lock)
            {
                _isDisposing = true;

                // Perform a final update if no projectiles have been spawned.
                if (_spawnedAmount == 0)
                {
                    _ = Update();
                }

                if (_spawns != null)
                {
                    foreach (var s in _spawns)
                    {
                        s?.Anim?.DisposeNextDraw();
                    }
                }

                GC.SuppressFinalize(this);
            }
        }
    }

    /// <inheritdoc />
    public override bool CanBeAttacked
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Determines the appropriate animation data index for the current spawn wave of the projectile.
    /// This method iterates through the available animations defined in the projectile's base configuration,
    /// selecting the one whose spawn range encompasses the current quantity of spawns.
    /// </summary>
    /// <returns>The index of the animation data to use for the current spawn wave.</returns>
    private int FindSpawnAnimationData()
    {
        if (_myBase == default)
        {
            return 0;
        }

        var start = 0;
        for (var i = 0; i < _myBase.Animations.Count; i++)
        {
            var end = _myBase.Animations[i].SpawnRange;
            if (_quantity >= start && _quantity < end)
            {
                return i;
            }

            start = end;
        }

        // If no suitable animation is found (e.g., developer(s) fucked up the animation ranges), default to the last animation.
        // This serves as a fallback to prevent crashes or undefined behavior in case of misconfiguration.
        return _myBase.Animations.Count - 1;
    }

    /// <summary>
    /// Adds projectile spawns based on predefined spawn locations and directions.
    /// </summary>
    private void AddProjectileSpawns()
    {
        if (_myBase == default || _spawns == default)
        {
            return;
        }

        var spawn = FindSpawnAnimationData();
        var animBase = AnimationBase.Get(_myBase.Animations[spawn].AnimationId);

        for (var x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
        {
            for (var y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
            {
                for (var d = 0; d < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; d++)
                {
                    // Check if the current direction is enabled for spawning at this location.
                    if (_myBase.SpawnLocations[x, y].Directions[d] == true)
                    {
                        // Calculate the spawn position and direction for the new projectile
                        var s = new ProjectileSpawns(
                            FindProjectileRotationDir(Dir, (Direction)d),
                            (byte)(X + FindProjectileRotation(Dir, x - 2, y - 2, true)),
                            (byte)(Y + FindProjectileRotation(Dir, x - 2, y - 2, false)), Z, MapId, animBase,
                            _myBase.Animations[spawn].AutoRotate, _myBase, this
                        );

                        _spawns[_spawnedAmount] = s;
                        if (Collided(_spawnedAmount))
                        {
                            TryRemoveSpawn(_spawnedAmount);
                            _spawnCount--;
                        }

                        // Add the new spawn to the array and increment counters
                        _spawnedAmount++;
                        _spawnCount++;
                    }
                }
            }
        }

        // Increment the quantity of projectiles spawned and update the spawn time based on the delay
        _quantity++;
        _spawnTime = Timing.Global.Milliseconds + _myBase.Delay;
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

    /// <summary>
    /// Gets the displacement of the projectile during projection
    /// </summary>
    /// <returns>The displacement from the co-ordinates if placed on a Options.TileHeight grid.</returns>
    private float GetDisplacement(long spawnTime)
    {
        if (_myBase == default)
        {
            return 0f;
        }

        var elapsedTime = Timing.Global.Milliseconds - spawnTime;
        var displacementPercent = elapsedTime / (float)_myBase.Speed;
        var calculatedDisplacement = displacementPercent * Options.TileHeight * _myBase.Range;

        // Ensure displacement does not exceed the maximum range of the projectile
        var maxDisplacement = Options.TileHeight * _myBase.Range;
        return Math.Min(calculatedDisplacement, maxDisplacement);
    }

    /// <summary>
    /// Overwrite updating the offsets for projectile movement.
    /// </summary>
    public override bool Update()
    {
        if (_myBase == null || _spawns == default)
        {
            return false;
        }

        lock (_lock)
        {
            var map = MapId;
            var y = Y;

            if (!_isDisposing && _quantity < _myBase.Quantity && _spawnTime < Timing.Global.Milliseconds)
            {
                AddProjectileSpawns();
            }

            if (IsMoving)
            {
                for (var s = 0; s < _spawnedAmount; s++)
                {
                    var spawn = _spawns[s];
                    
                    if (spawn != null && Maps.MapInstance.Get(spawn.SpawnMapId) != null)
                    {
                        if (_targetId != Guid.Empty && _targetId != _owner &&
                            Globals.Entities.TryGetValue(_targetId, out var target) &&
                            (_myBase.HomingBehavior || _myBase.DirectShotBehavior)
                        )
                        {
                            _lastTargetX = target.X;
                            _lastTargetY = target.Y;
                            _lastTargetMapId = target.MapId;

                            spawn.OffsetX = GetProjectileLerping(spawn, true);
                            spawn.OffsetY = GetProjectileLerping(spawn, false);
                            SetProjectileRotation(spawn);

                            if (_myBase.DirectShotBehavior)
                            {
                                _targetId = Guid.Empty;
                            }
                        }
                        else if (_lastTargetX != -1 && _lastTargetY != -1)
                        {
                            spawn.OffsetX = GetProjectileLerping(spawn, true);
                            spawn.OffsetY = GetProjectileLerping(spawn, false);
                            SetProjectileRotation(spawn);
                        }
                        else
                        {
                            spawn.OffsetX = GetRange(spawn.Dir, GetDisplacement(spawn.SpawnTime), true);
                            spawn.OffsetY = GetRange(spawn.Dir, GetDisplacement(spawn.SpawnTime), false);
                            spawn.Anim.SetRotation(false);
                        }

                        var spawnMapId = Maps.MapInstance.Get(spawn.SpawnMapId);
                        var spawnX = spawnMapId.GetX() + spawn.SpawnX * Options.TileWidth + spawn.OffsetX + Options.TileWidth / 2;
                        var spawnY = spawnMapId.GetY() + spawn.SpawnY * Options.TileHeight + spawn.OffsetY + Options.TileHeight / 2;
                        var spawnDirection = spawn.AutoRotate ? spawn.Dir : Direction.Up;

                        spawn.Anim.SetPosition(spawnX, spawnY, X, Y, MapId, spawnDirection, spawn.Z);
                        spawn.Anim.Update();
                    }
                }
            }

            CheckForCollision();
        }

        return true;
    }

    /// <summary>
    /// Calculates the offset for the projectile spawn position based on the target map and spawn location.
    /// </summary>
    /// <param name="spawn">The projectile spawn information.</param>
    /// <param name="isXAxis">True for horizontal (X-axis), false for vertical (Y-axis) calculation.</param>
    /// <returns>The calculated offset value.</returns>
    private float GetProjectileOffset(ProjectileSpawns spawn, bool isXAxis)
    {
        if (_lastTargetMapId == Guid.Empty ||
            _lastTargetMapId == spawn.SpawnMapId ||
            !Maps.MapInstance.TryGet(spawn.SpawnMapId, out var map) ||
            Globals.MapGrid == default
        )
        {
            return isXAxis ? _lastTargetX - spawn.SpawnX : _lastTargetY - spawn.SpawnY;
        }

        for (var y = map.GridY - 1; y <= map.GridY + 1; y++)
        {
            for (var x = map.GridX - 1; x <= map.GridX + 1; x++)
            {
                if (x < 0 || x >= Globals.MapGrid.GetLength(0) || y < 0 || y >= Globals.MapGrid.GetLength(1))
                {
                    continue;
                }

                if (Globals.MapGrid[x, y] != _lastTargetMapId || Globals.MapGrid[x, y] == Guid.Empty)
                {
                    continue;
                }

                if (isXAxis) // Horizontal (X) calculation
                {
                    var leftSide = x == map.GridX - 1;
                    var rightSide = x == map.GridX + 1;

                    if (leftSide)
                    {
                        return _lastTargetX - Options.MapWidth - spawn.SpawnX;
                    }

                    if (rightSide)
                    {
                        return _lastTargetX + Options.MapWidth - spawn.SpawnX;
                    }
                }
                else // Vertical (Y) calculation
                {
                    var topSide = y == map.GridY + 1;
                    var bottomSide = y == map.GridY - 1;

                    if (topSide)
                    {
                        return _lastTargetY + Options.MapHeight - spawn.SpawnY;
                    }

                    if (bottomSide)
                    {
                        return _lastTargetY - Options.MapHeight - spawn.SpawnY;
                    }
                }
            }
        }

        return isXAxis ? _lastTargetX - spawn.SpawnX : _lastTargetY - spawn.SpawnY;
    }

    /// <summary>
    /// Calculates the interpolated (lerped) position value for a projectile along the X or Y axis.
    /// </summary>
    /// <param name="spawn">The spawn information of the projectile, including its initial position and spawn time.</param>
    /// <param name="isXAxis">True for horizontal (X-axis), false for vertical (Y-axis) calculation.</param>
    /// <returns>The interpolated position value for the projectile on the specified axis, taking into account the projectile's travel direction, speed, and elapsed time since spawning.</returns>
    /// <remarks>
    /// This method determines the projectile's current position by interpolating between its initial offset and its desired position at the current time.
    /// It calculates the direction and magnitude of the projectile's movement, normalizes this to obtain a unit vector in the direction of movement,
    /// and then applies linear interpolation based on the elapsed time since the projectile was spawned.
    /// The interpolation factor is clamped to ensure that the projectile does not overshoot its target position.
    /// </remarks>
    private float GetProjectileLerping(ProjectileSpawns spawn, bool isXAxis)
    {
        if (_myBase == default)
        {
            return 0f;
        }

        var (directionX, directionY) = (GetProjectileOffset(spawn, true), GetProjectileOffset(spawn, false));
        var distance = MathF.Sqrt(directionX * directionX + directionY * directionY);
        if (distance == 0) return 0;

        var valueToLerp = (isXAxis ? directionX : directionY) / distance;
        var offset = isXAxis ? spawn.OffsetX : spawn.OffsetY;
        var desiredValue = GetDisplacement(spawn.SpawnTime + Options.Instance.Processing.ProjectileUpdateInterval) * valueToLerp;
        var totalDuration = (float)_myBase.Range * (_myBase.Speed / Options.TileHeight);
        var elapsedTime = Timing.Global.Milliseconds - spawn.SpawnTime;
        var lerpFactor = Utilities.MathHelper.Clamp(elapsedTime / totalDuration, 0f, 1f);

        // Dynamically calculated lerp factor
        return (1 - lerpFactor) * offset + lerpFactor * desiredValue;
    }

    /// <summary>
    /// Sets the rotation of a projectile based on its spawn information.
    /// </summary>
    /// <param name="spawn">The spawn information of the projectile, including its current animation state and position.</param>
    /// <remarks>
    /// This method calculates the angle between the projectile's current position and its target,
    /// converting the angle from radians to degrees and adjusting it by 90 degrees to align with the game's coordinate system.
    /// The calculated angle is then applied to the projectile's animation state to visually orient the projectile towards its target.
    /// </remarks>
    private void SetProjectileRotation(ProjectileSpawns spawn)
    {
        var (directionX, directionY) = (GetProjectileOffset(spawn, true), GetProjectileOffset(spawn, false));
        var angle = (float)(Math.Atan2(directionY, directionX) * (180.0 / Math.PI) + 90);
        spawn.Anim.SetRotation(angle);
    }

    /// <summary>
    /// Checks for collision of the projectile with other entities or map boundaries.
    /// </summary>
    public void CheckForCollision()
    {
        if (_myBase == default || _spawns == default || _spawnCount == 0 && _quantity > _myBase.Quantity)
        {
            Globals.Entities[Id].Dispose();
            return;
        }

        for (var i = 0; i < _spawnedAmount && i < _spawns.Length; i++)
        {
            var projectileSpawn = _spawns[i];
            if (projectileSpawn == null || Timing.Global.Milliseconds <= projectileSpawn.TransmissionTimer)
            {
                continue;
            }

            if (!Maps.MapInstance.TryGet(projectileSpawn.MapId, out var spawnMap))
            {
                continue;
            }

            float newx = projectileSpawn.X;
            float newy = projectileSpawn.Y;

            if (_myBase.HomingBehavior || _myBase.DirectShotBehavior)
            {
                if (_targetId != Guid.Empty && _targetId != _owner || _lastTargetX != -1 && _lastTargetY != -1 && _lastTargetMapId != Guid.Empty)
                {
                    newx += GetProjectileOffset(projectileSpawn, true);
                    newy += GetProjectileOffset(projectileSpawn, false);
                }
            }
            else
            {
                newx += GetRange(projectileSpawn.Dir, 1, true);
                newy += GetRange(projectileSpawn.Dir, 1, false);
            }

            AdjustPositionOnMapBoundaries(ref newx, ref newy, ref spawnMap);

            projectileSpawn.X = (int)newx;
            projectileSpawn.Y = (int)newy;
            projectileSpawn.MapId = spawnMap.Id;
            projectileSpawn.Distance++;
            projectileSpawn.TransmissionTimer = Timing.Global.MillisecondsOffset + (long)(_myBase.Speed / (float)_myBase.Range);

            var killSpawn = Collided(i) || projectileSpawn.Distance >= _myBase.Range ||
                            newx < 0 || newx >= Options.MapWidth || newy < 0 ||
                            newy >= Options.MapHeight;

            // Check for map boundaries and remove the spawn if it goes out of bounds.
            if (killSpawn)
            {
                TryRemoveSpawn(i);
                _spawnCount--;

                continue;
            }

            // Check for Z-Dimension
            if (!killSpawn && spawnMap.Attributes != null &&
                projectileSpawn.X >= 0 && projectileSpawn.Y >= 0 && projectileSpawn.X < spawnMap.Attributes.GetLength(0) &&
                projectileSpawn.Y < spawnMap.Attributes.GetLength(1))
            {
                var attribute = spawnMap.Attributes[projectileSpawn.X, projectileSpawn.Y];

                if (attribute != null && attribute.Type == MapAttribute.ZDimension)
                {
                    var zDimensionAttribute = (MapZDimensionAttribute)attribute;

                    // If the Z dimension attribute specifies a blocked level that matches the projectile's current Z level,
                    // mark the projectile for destruction.
                    if (zDimensionAttribute.BlockedLevel > 0 && projectileSpawn.Z == zDimensionAttribute.BlockedLevel - 1)
                    {
                        killSpawn = true;
                    }

                    // If the Z dimension attribute specifies a gateway to another level,
                    // adjust the projectile's Z level to the specified gateway level.
                    if (zDimensionAttribute.GatewayTo > 0)
                    {
                        projectileSpawn.Z = zDimensionAttribute.GatewayTo - 1;
                    }
                }
            }

            if (killSpawn)
            {
                TryRemoveSpawn(i);
                _spawnCount--;
            }
        }
    }

    /// <summary>
    /// Adjusts the position of a projectile on the map boundaries, potentially changing its map instance.
    /// </summary>
    /// <param name="newx">The new x-coordinate of the projectile.</param>
    /// <param name="newy">The new y-coordinate of the projectile.</param>
    /// <param name="spawnMap">The map instance where the projectile is spawned. This may be updated if the projectile crosses map boundaries.</param>
    /// <remarks>
    /// This method checks if the projectile crosses the boundaries of its current map. If it does, it attempts to move the projectile
    /// to the adjacent map in the direction it crossed the boundary. This includes handling corner cases where the projectile crosses
    /// at the corners of the map, potentially moving it diagonally to a new map. The new position of the projectile is adjusted to
    /// visually reflect its entry point into the adjacent map.
    /// </remarks>
    private static void AdjustPositionOnMapBoundaries(ref float newx, ref float newy, ref Maps.MapInstance spawnMap)
    {
        int MapWidth = Options.MapWidth;
        int MapHeight = Options.MapHeight;

        // Determine if the projectile crosses any of the map boundaries.
        bool crossesLeftBoundary = MathF.Floor(newx) < 0;
        bool crossesRightBoundary = MathF.Ceiling(newx) > MapWidth - 1;
        bool crossesTopBoundary = MathF.Floor(newy) < 0;
        bool crossesBottomBoundary = MathF.Ceiling(newy) > MapHeight - 1;

        // Handle corner cases: crossing boundaries at the corners of the map.
        if (crossesLeftBoundary && crossesTopBoundary)
        {
            // Move to the map diagonally up-left if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Up, out var upMap) &&
                Maps.MapInstance.TryGet(upMap.Left, out var upLeftMap))
            {
                spawnMap = upLeftMap;
                newx = MapWidth - 1;
                newy = MapHeight - 1;
            }
        }
        else if (crossesRightBoundary && crossesTopBoundary)
        {
            // Move to the map diagonally up-right if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Up, out var upMap) &&
                Maps.MapInstance.TryGet(upMap.Right, out var upRightMap))
            {
                spawnMap = upRightMap;
                newx = 0;
                newy = MapHeight - 1;
            }
        }
        else if (crossesLeftBoundary && crossesBottomBoundary)
        {
            // Move to the map diagonally down-left if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Down, out var downMap) &&
                Maps.MapInstance.TryGet(downMap.Left, out var downLeftMap))
            {
                spawnMap = downLeftMap;
                newx = MapWidth - 1;
                newy = 0;
            }
        }
        else if (crossesRightBoundary && crossesBottomBoundary)
        {
            // Move to the map diagonally down-right if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Down, out var downMap) &&
                Maps.MapInstance.TryGet(downMap.Right, out var downRightMap))
            {
                spawnMap = downRightMap;
                newx = 0;
                newy = 0;
            }
        }
        // Handle cases where the projectile crosses one boundary.
        else if (crossesLeftBoundary)
        {
            // Move to the map on the left if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Left, out var leftMap))
            {
                spawnMap = leftMap;
                newx = MapWidth - 1;
            }
        }
        else if (crossesRightBoundary)
        {
            // Move to the map on the right if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Right, out var rightMap))
            {
                spawnMap = rightMap;
                newx = 0;
            }
        }
        else if (crossesTopBoundary)
        {
            // Move to the map above if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Up, out var upMap))
            {
                spawnMap = upMap;
                newy = MapHeight - 1;
            }
        }
        else if (crossesBottomBoundary)
        {
            // Move to the map below if possible.
            if (Maps.MapInstance.TryGet(spawnMap.Down, out var downMap))
            {
                spawnMap = downMap;
                newy = 0;
            }
        }
    }

    /// <summary>
    /// Determines if a projectile spawn has collided with an entity, resource, or map block.
    /// </summary>
    /// <param name="i">The index of the projectile spawn in the _spawns array.</param>
    /// <returns>True if the projectile spawn has collided and should be destroyed; otherwise, false.</returns>
    private bool Collided(int i)
    {
        if (Globals.Me == default || _spawns == default)
        {
            return true;
        }

        var killSpawn = false;
        IEntity? blockedBy = default;
        var spawn = _spawns[i];

        if (spawn == null)
        {
            return true;
        }

        // Check if the tile at the projectile's location is blocked.
        var tileBlocked = Globals.Me.IsTileBlocked(
            delta: new Point(spawn.X, spawn.Y),
            z: Z,
            mapId: spawn.MapId,
            blockedBy: ref blockedBy,
            ignoreAliveResources: spawn.ProjectileBase.IgnoreActiveResources,
            ignoreDeadResources: spawn.ProjectileBase.IgnoreExhaustedResources,
            ignoreNpcAvoids: true,
            projectileTrigger: true
        );

        switch (tileBlocked)
        {
            case -1: // No collision detected
                return killSpawn;
            case -6: // Collision with an entity other than the owner
                if (blockedBy != default && blockedBy.Id != _owner && Globals.Entities.ContainsKey(blockedBy.Id))
                {
                    if (blockedBy is Resource)
                    {
                        killSpawn = true;
                    }
                }

                break;
            case -2: // Collision with a map block
                if (!spawn.ProjectileBase.IgnoreMapBlocks)
                {
                    killSpawn = true;
                }

                break;
            case -3: // Collision with a Z-dimension block
                if (!spawn.ProjectileBase.IgnoreZDimension)
                {
                    killSpawn = true;
                }

                break;
            case -5: // Collision with an unspecified block type or out of map bounds
                killSpawn = true;
                break;
        }

        return killSpawn;
    }

    /// <summary>
    /// Rendering all of the individual projectiles from a singular spawn to a map.
    /// </summary>
    public override void Draw()
    {
        if (Maps.MapInstance.Get(MapId) == null || !Globals.GridMaps.Contains(MapId))
        {
            return;
        }
    }

    public void SpawnDead(int spawnIndex)
    {
        if (spawnIndex < _spawnedAmount && _spawns?[spawnIndex] != null)
        {
            TryRemoveSpawn(spawnIndex);
        }
    }

    private void TryRemoveSpawn(int spawnIndex)
    {
        if (spawnIndex < _spawnedAmount && _spawns?[spawnIndex] != null)
        {
            _spawns[spawnIndex]!.Dispose();
            _spawns[spawnIndex] = null;
        }
    }
}

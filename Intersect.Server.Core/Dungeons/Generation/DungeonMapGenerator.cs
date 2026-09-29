using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.Framework.Core.GameObjects.Maps.Attributes;
using Intersect.Server.Database;
using Intersect.Server.Entities;
using Intersect.Server.Maps;

namespace Intersect.Server.Dungeons.Generation;

/// <summary>
/// Builds a fresh room-and-corridor layout for a <see cref="DungeonDescriptor"/> with
/// <see cref="DungeonDescriptor.ProceduralGeneration"/> enabled. The resulting <see cref="MapController"/> is
/// registered in <see cref="MapController.Lookup"/> only (never persisted to the database) so it behaves like any
/// other map to the server/clients while it's in use, and can be forgotten once the run ends.
/// </summary>
public static class DungeonMapGenerator
{
    private readonly record struct Room(int X, int Y, int Width, int Height)
    {
        public int CenterX => X + Width / 2;

        public int CenterY => Y + Height / 2;
    }

    public static MapController Generate(DungeonDescriptor descriptor, Player starter, Guid mapInstanceId, out int entryX, out int entryY)
    {
        var width = Options.Instance.Map.MapWidth;
        var height = Options.Instance.Map.MapHeight;
        var layerName = Options.Instance.Map.Layers.LowerLayers[0];

        var floorTile = new Tile
        {
            TilesetId = descriptor.FloorTilesetId,
            X = descriptor.FloorTileX,
            Y = descriptor.FloorTileY,
            Autotile = descriptor.FloorTileIsAutotile ? MapAutotiles.AUTOTILE_NORMAL : MapAutotiles.AUTOTILE_NONE,
        };
        var wallTile = new Tile
        {
            TilesetId = descriptor.WallTilesetId,
            X = descriptor.WallTileX,
            Y = descriptor.WallTileY,
            Autotile = descriptor.WallTileIsAutotile ? MapAutotiles.AUTOTILE_NORMAL : MapAutotiles.AUTOTILE_NONE,
        };

        var ground = new Tile[width, height];
        var attributes = new MapAttribute[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                ground[x, y] = wallTile;
                attributes[x, y] = new MapBlockedAttribute();
            }
        }

        var random = new Random();
        var minRooms = Math.Max(1, Math.Min(descriptor.MinRooms, descriptor.MaxRooms));
        var maxRooms = Math.Max(minRooms, descriptor.MaxRooms);
        var minSize = Math.Max(3, Math.Min(descriptor.MinRoomSize, descriptor.MaxRoomSize));
        var maxSize = Math.Max(minSize, descriptor.MaxRoomSize);
        var roomCount = random.Next(minRooms, maxRooms + 1);
        var rooms = new List<Room>();

        var attempts = 0;
        while (rooms.Count < roomCount && attempts < roomCount * 25)
        {
            attempts++;

            var roomWidth = random.Next(minSize, maxSize + 1);
            var roomHeight = random.Next(minSize, maxSize + 1);
            if (roomWidth + 2 >= width || roomHeight + 2 >= height)
            {
                continue;
            }

            var roomX = random.Next(1, width - roomWidth - 1);
            var roomY = random.Next(1, height - roomHeight - 1);
            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            // Leave a 1-tile gap between rooms so walls stay solid between neighbours.
            var overlaps = rooms.Any(existing =>
                room.X - 1 < existing.X + existing.Width + 1 &&
                room.X + room.Width + 1 > existing.X - 1 &&
                room.Y - 1 < existing.Y + existing.Height + 1 &&
                room.Y + room.Height + 1 > existing.Y - 1
            );

            if (overlaps)
            {
                continue;
            }

            rooms.Add(room);
            CarveRoom(ground, attributes, room, floorTile);
        }

        if (rooms.Count == 0)
        {
            // Every placement attempt failed (tiny map / huge room sizes) - fall back to a single centered room
            // so the dungeon is never fully solid.
            var fallbackWidth = Math.Min(minSize, width - 2);
            var fallbackHeight = Math.Min(minSize, height - 2);
            var fallback = new Room((width - fallbackWidth) / 2, (height - fallbackHeight) / 2, fallbackWidth, fallbackHeight);
            rooms.Add(fallback);
            CarveRoom(ground, attributes, fallback, floorTile);
        }

        for (var i = 1; i < rooms.Count; i++)
        {
            CarveCorridor(ground, attributes, rooms[i - 1], rooms[i], floorTile, random);
        }

        var firstRoom = rooms[0];
        var lastRoom = rooms[^1];
        entryX = firstRoom.CenterX;
        entryY = firstRoom.CenterY;

        // The exit warp sends participants back to wherever the dungeon is configured to return them to
        // (EntryMapId/X/Y/Direction take on that meaning once procedural generation is enabled).
        attributes[lastRoom.CenterX, lastRoom.CenterY] = new MapWarpAttribute
        {
            MapId = descriptor.EntryMapId,
            X = (byte)Math.Clamp(descriptor.EntryX, 0, byte.MaxValue),
            Y = (byte)Math.Clamp(descriptor.EntryY, 0, byte.MaxValue),
            Direction = WarpDirection.Retain,
            ChangeInstance = true,
            InstanceType = MapInstanceType.Overworld,
        };

        var map = new MapController(newMap: true)
        {
            Name = $"{descriptor.Name} (Generated)",
            // Dungeons are indoors: full, constant brightness regardless of the day/night cycle. Without this the
            // map is treated as outdoor and gets tinted by the current time of day, which can render pitch black.
            IsIndoors = true,
            Brightness = 100,
        };
        map.Layers = new Dictionary<string, Tile[,]> { [layerName] = ground };
        map.CommitGeneratedLayers();
        map.Attributes = attributes;

        MapController.Lookup.Add(map);

        // Generated maps have no Up/Down/Left/Right neighbours, so they're not part of any map grid by default
        // (MapGrid stays 0, an unrelated real grid). Without a grid of their own, the normal "send the maps around
        // the player" sync in Player.Warp() never reaches them and the client never actually loads the map -
        // giving a black screen with no error. This registers a standalone 1x1 grid containing just this map.
        DbInterface.AddStandaloneMapGrid(map.Id);

        // Create the run's shared instance right away (instead of waiting for the first player's Warp() call) so
        // we can populate it with monsters before anyone actually arrives.
        if (map.TryCreateInstance(mapInstanceId, out var mapInstance, starter))
        {
            SpawnNpcs(mapInstance, descriptor, rooms, random);
        }

        return map;
    }

    /// <summary>
    /// Drops a random handful of <see cref="DungeonDescriptor.MonsterPoolNpcIds"/> into every room except the
    /// entry (first) and boss (last) room, then places <see cref="DungeonDescriptor.BossNpcId"/> (if set) in the
    /// last room.
    /// </summary>
    private static void SpawnNpcs(MapInstance mapInstance, DungeonDescriptor descriptor, List<Room> rooms, Random random)
    {
        if (rooms.Count == 0)
        {
            return;
        }

        if (descriptor.MonsterPoolNpcIds.Count > 0)
        {
            var minPerRoom = Math.Max(0, Math.Min(descriptor.MinNpcsPerRoom, descriptor.MaxNpcsPerRoom));
            var maxPerRoom = Math.Max(minPerRoom, descriptor.MaxNpcsPerRoom);

            for (var i = 1; i < rooms.Count - 1; i++)
            {
                var room = rooms[i];
                var count = random.Next(minPerRoom, maxPerRoom + 1);
                for (var n = 0; n < count; n++)
                {
                    var npcId = descriptor.MonsterPoolNpcIds[random.Next(descriptor.MonsterPoolNpcIds.Count)];
                    var (x, y) = RandomPointInRoom(room, random);
                    mapInstance.SpawnNpc((byte)x, (byte)y, Direction.Down, npcId, despawnable: true);
                }
            }
        }

        if (descriptor.BossNpcId != Guid.Empty)
        {
            var bossRoom = rooms[^1];
            mapInstance.SpawnNpc((byte)bossRoom.CenterX, (byte)bossRoom.CenterY, Direction.Down, descriptor.BossNpcId, despawnable: true);
        }
    }

    private static (int X, int Y) RandomPointInRoom(Room room, Random random)
    {
        // Stay a tile in from the walls.
        var x = room.Width > 2 ? random.Next(room.X + 1, room.X + room.Width - 1) : room.CenterX;
        var y = room.Height > 2 ? random.Next(room.Y + 1, room.Y + room.Height - 1) : room.CenterY;
        return (x, y);
    }

    /// <summary>
    /// Removes a map generated by <see cref="Generate"/> from <see cref="MapController.Lookup"/>. Safe to call
    /// even if the map was never registered.
    /// </summary>
    public static void Discard(MapController? map)
    {
        if (map == null)
        {
            return;
        }

        MapController.Lookup.Delete(map);
    }

    private static void CarveRoom(Tile[,] ground, MapAttribute[,] attributes, Room room, Tile floorTile)
    {
        for (var x = room.X; x < room.X + room.Width; x++)
        {
            for (var y = room.Y; y < room.Y + room.Height; y++)
            {
                ground[x, y] = floorTile;
                attributes[x, y] = null;
            }
        }
    }

    /// <summary>
    /// Carves an L-shaped corridor (one horizontal leg, one vertical leg, order randomized) between the centers of
    /// two rooms.
    /// </summary>
    private static void CarveCorridor(Tile[,] ground, MapAttribute[,] attributes, Room from, Room to, Tile floorTile, Random random)
    {
        var horizontalFirst = random.Next(2) == 0;
        var (startX, startY) = (from.CenterX, from.CenterY);
        var (endX, endY) = (to.CenterX, to.CenterY);

        if (horizontalFirst)
        {
            CarveHorizontal(ground, attributes, startX, endX, startY, floorTile);
            CarveVertical(ground, attributes, startY, endY, endX, floorTile);
        }
        else
        {
            CarveVertical(ground, attributes, startY, endY, startX, floorTile);
            CarveHorizontal(ground, attributes, startX, endX, endY, floorTile);
        }
    }

    /// <summary>How many tiles wide a carved corridor is, so they read as proper passages instead of a 1-tile crack.</summary>
    private const int CorridorWidth = 2;

    private static void CarveHorizontal(Tile[,] ground, MapAttribute[,] attributes, int fromX, int toX, int y, Tile floorTile)
    {
        var (minX, maxX) = fromX <= toX ? (fromX, toX) : (toX, fromX);
        for (var x = minX; x <= maxX; x++)
        {
            for (var offset = 0; offset < CorridorWidth; offset++)
            {
                SetFloor(ground, attributes, x, y + offset - CorridorWidth / 2, floorTile);
            }
        }
    }

    private static void CarveVertical(Tile[,] ground, MapAttribute[,] attributes, int fromY, int toY, int x, Tile floorTile)
    {
        var (minY, maxY) = fromY <= toY ? (fromY, toY) : (toY, fromY);
        for (var y = minY; y <= maxY; y++)
        {
            for (var offset = 0; offset < CorridorWidth; offset++)
            {
                SetFloor(ground, attributes, x + offset - CorridorWidth / 2, y, floorTile);
            }
        }
    }

    private static void SetFloor(Tile[,] ground, MapAttribute[,] attributes, int x, int y, Tile floorTile)
    {
        if (x < 0 || x >= ground.GetLength(0) || y < 0 || y >= ground.GetLength(1))
        {
            return;
        }

        ground[x, y] = floorTile;
        attributes[x, y] = null;
    }
}

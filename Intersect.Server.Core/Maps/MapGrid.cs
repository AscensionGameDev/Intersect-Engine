using System.Diagnostics.Contracts;
using Intersect.Server.Database;

using Newtonsoft.Json.Linq;

namespace Intersect.Server.Maps;

public partial class MapGrid
{
    private Point _bottomRight = new(0, 0);

    private Point _topLeft = new(0, 0);

    public long Height { get; }
    public int Index { get; }
    public Guid[,] MapIdGrid { get; }
    public List<Guid> MapIds { get; }
    public long Width { get; }
    public long XMax { get; }
    public long XMin { get; }
    public long YMax { get; }
    public long YMin { get; }

    public MapGrid(Guid startMapId, int index)
    {
        if (!MapController.TryGet(startMapId, out var startMap))
        {
            throw new Exception($"Map {startMapId} does not exist, cannot get the grid for this map.");
        }

        Index = index;
        startMap.MapGrid = index;
        startMap.MapGridX = 0;
        startMap.MapGridY = 0;
        MapIds = CalculateBounds(startMap, 0, 0);

        Width = _bottomRight.X - _topLeft.X + 1;
        Height = _bottomRight.Y - _topLeft.Y + 1;

        var xOffset = _topLeft.X;
        var yOffset = _topLeft.Y;
        XMin = _topLeft.X - xOffset;
        YMin = _topLeft.Y - yOffset;
        XMax = _bottomRight.X - xOffset + 1;
        YMax = _bottomRight.Y - yOffset + 1;

        MapIdGrid = new Guid[Width, Height];
        List<Guid> mapIds = new(MapIds.ToArray());
        foreach (var mapId in mapIds.ToArray())
        {
            if (!MapController.TryGet(mapId, out var map))
            {
                continue;
            }

            bool foundMap = false;

            for (var x = XMin; x < XMax; x++)
            {
                if (foundMap)
                {
                    break;
                }

                for (var y = YMin; y < YMax; y++)
                {
                    if (map.MapGridX + Math.Abs(_topLeft.X) != x || map.MapGridY + Math.Abs(_topLeft.Y) != y)
                    {
                        continue;
                    }

                    foundMap = true;
                    MapIdGrid[x, y] = mapId;
                    map.MapGrid = index;
                    map.MapGridX = (int)x;
                    map.MapGridY = (int)y;
                    map.CachedMapClientPacket = default;
                    mapIds.Remove(mapId);
                    break;
                }
            }
        }

        MapIds.RemoveAll(mapIds.Contains);
    }

    [Pure]
    private List<Guid> CalculateBounds(MapController map, int x, int y)
    {
        List<Guid> mapIds = new(9);
        var maps = new Stack<Tuple<MapController, int, int>>();
        maps.Push(new Tuple<MapController, int, int>(map, x, y));
        while (maps.Count > 0)
        {
            var curMap = maps.Pop();
            map = curMap.Item1;
            x = curMap.Item2;
            y = curMap.Item3;

            if (mapIds.Contains(map.Id) || DbInterface.GridsContain(map.Id))
            {
                continue;
            }

            mapIds.Add(map.Id);
            map.MapGridX = x;
            map.MapGridY = y;
            if (x < _topLeft.X)
            {
                _topLeft.X = x;
            }

            if (y < _topLeft.Y)
            {
                _topLeft.Y = y;
            }

            if (x > _bottomRight.X)
            {
                _bottomRight.X = x;
            }

            if (y > _bottomRight.Y)
            {
                _bottomRight.Y = y;
            }

            if (MapController.TryGet(map.Up, out var mapUp) && mapUp.Down == map.Id)
            {
                maps.Push(new Tuple<MapController, int, int>(MapController.Get(map.Up), x, y - 1));
            }

            if (MapController.TryGet(map.Down, out var mapDown) && mapDown.Up == map.Id)
            {
                maps.Push(new Tuple<MapController, int, int>(MapController.Get(map.Down), x, y + 1));
            }

            if (MapController.TryGet(map.Left, out var mapLeft) && mapLeft.Right == map.Id)
            {
                maps.Push(new Tuple<MapController, int, int>(MapController.Get(map.Left), x - 1, y));
            }

            if (MapController.TryGet(map.Right, out var mapRight) && mapRight.Left == map.Id)
            {
                maps.Push(new Tuple<MapController, int, int>(MapController.Get(map.Right), x + 1, y));
            }
        }

        return mapIds;
    }

    public bool Contains(Guid mapId) => mapId != default && MapIds.Contains(mapId);

    public string[,] GetEditorData()
    {
        var data = new string[Width, Height];
        for (var x = XMin; x < XMax; x++)
        {
            for (var y = YMin; y < YMax; y++)
            {
                if (!MapController.TryGet(MapIdGrid[x, y], out var map))
                {
                    continue;
                }

                var obj = new JObject
                {
                    { "Guid", map.Id },
                    { "Name", map.Name },
                    { "Revision", map.Revision },
                };
                data[x, y] = obj.ToString();
            }
        }

        return data;
    }

    public Guid[,] GetClientData() => MapIdGrid;
}
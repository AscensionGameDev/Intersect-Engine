using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;

namespace Intersect.Client.Interface.Game.Map
{
    public sealed class MinimapWindow : Window
    {
        private GameRenderTexture _renderTexture;
        private GameTexture _whiteTexture;
        private bool _redrawMaps;
        private bool _redrawEntities;
        private int _zoomLevel;
        private Dictionary<MapPosition, MapBase> _mapGrid = new();
        private Dictionary<MapPosition, Dictionary<Point, EntityInfo>> _entityInfoCache = new();

        private readonly Point _minimapTileSize;
        private readonly Dictionary<MapPosition, GameRenderTexture> _minimapCache = new();
        private readonly Dictionary<MapPosition, GameRenderTexture> _entityCache = new();
        private readonly Dictionary<Guid, MapPosition> _mapPosition = new();
        private readonly ImagePanel _minimap;
        private readonly Button _zoomInButton;
        private readonly Button _zoomOutButton;

        private static readonly GameContentManager ContentManager = Globals.ContentManager;

        private volatile bool _initialized;

        // Constructors
        public MinimapWindow(Base parent) : base(parent, Strings.Minimap.Title, false, "MinimapWindow")
        {
            DisableResizing();
            _zoomLevel = Options.Instance.MinimapOpts.DefaultZoom;
            _minimapTileSize = Options.Instance.MinimapOpts.TileSize;

            _minimap = new ImagePanel(this, "MinimapContainer");
            _zoomInButton = new Button(_minimap, "ZoomInButton");
            _zoomOutButton = new Button(_minimap, "ZoomOutButton");
            _zoomInButton.Clicked += MZoomInButton_Clicked;
            _zoomInButton.SetToolTipText(Strings.Minimap.ZoomIn);
            _zoomOutButton.Clicked += MZoomOutButton_Clicked;
            _zoomOutButton.SetToolTipText(Strings.Minimap.ZoomOut);

            _whiteTexture = Graphics.Renderer.GetWhiteTexture();
            _renderTexture = GenerateBaseRenderTexture();
        }

        protected override void EnsureInitialized()
        {
            LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            _initialized = true;
        }

        // Public Methods
        public void Update()
        {
            if (!IsVisible() || !_initialized)
            {
                return;
            }

            DrawMinimap();
            UpdateMinimap(Globals.Me, Globals.Entities);
        }

        public void Show()
        {
            IsHidden = false;
        }

        public bool IsVisible()
        {
            return !IsHidden;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        // Private Methods
        private void UpdateMinimap(Player player, Dictionary<Guid, Entity> allEntities)
        {
            if (player == null)
            {
                Console.WriteLine("Player is null in UpdateMinimap.");
                return;
            }

            if (allEntities == null)
            {
                Console.WriteLine("allEntities is null in UpdateMinimap.");
                return;
            }

            if (player.MapInstance == null)
            {
                Console.WriteLine("player.MapInstance is null in UpdateMinimap.");
                return;
            }

            if (player.MapInstance.Id == Guid.Empty)
            {
                Console.WriteLine("player.MapInstance.Id is empty in UpdateMinimap.");
                return;
            }

            if (!MapInstance.TryGet(player.MapInstance.Id, out var mapBase))
            {
                Console.WriteLine("MapInstance.TryGet failed in UpdateMinimap.");
                return;
            }

            if (_renderTexture == null)
            {
                Console.WriteLine("_renderTexture is null in UpdateMinimap.");
                return;
            }

            if (_minimapTileSize == null)
            {
                Console.WriteLine("_minimapTileSize is null in UpdateMinimap.");
                return;
            }

            var newGrid = CreateMapGridFromMap(mapBase);

            if (!newGrid.SequenceEqual(_mapGrid))
            {
                _mapGrid = newGrid;
                _mapPosition.Clear();
                foreach (var map in _mapGrid)
                {
                    if (map.Value != null)
                    {
                        _mapPosition.Add(map.Value.Id, map.Key);
                    }
                }

                _redrawMaps = true;
            }

            var newLocations = GenerateEntityInfo(allEntities, player);
            if (!newLocations.Equals(_entityInfoCache))
            {
                _entityInfoCache = newLocations;
                _redrawEntities = true;
            }

            // Update our minimap display area
            var centerX = (_renderTexture.Width / 3) + (player.X * _minimapTileSize.X);
            var centerY = (_renderTexture.Height / 3) + (player.Y * _minimapTileSize.Y);
            var displayWidth = (int)(_renderTexture.Width * (_zoomLevel / 100f));
            var displayHeight = (int)(_renderTexture.Height * (_zoomLevel / 100f));

            var x = centerX - (displayWidth / 2);
            if (x + displayWidth > _renderTexture.Width)
            {
                x = _renderTexture.Width - displayWidth;
            }

            if (x < 0)
            {
                x = 0;
            }

            var y = centerY - (displayHeight / 2);
            if (y + displayHeight > _renderTexture.Height)
            {
                y = _renderTexture.Height - displayHeight;
            }

            if (y < 0)
            {
                y = 0;
            }

            _minimap.SetTextureRect(x, y, displayWidth, displayHeight);
        }

        private void DrawMinimap()
        {
            if (!_redrawEntities && !_redrawMaps)
            {
                return;
            }

            _renderTexture.Clear(Color.Transparent);
            _minimap.Texture = _renderTexture;
            _minimap.SetTextureRect(0, 0, _renderTexture.Width, _renderTexture.Height);

            foreach (var pos in _mapGrid.Keys)
            {
                if (_redrawMaps)
                {
                    GenerateMinimapCacheFor(pos);
                }

                if (_redrawEntities)
                {
                    GenerateEntityCacheFor(pos);
                }

                DrawMinimapCacheToTexture(pos);
            }

            if (_redrawMaps)
            {
                _redrawMaps = false;
            }

            if (_redrawEntities)
            {
                _redrawEntities = false;
            }
        }

        private void GenerateMinimapCacheFor(MapPosition position)
        {
            if (!_minimapCache.TryGetValue(position, out var cachedMinimap))
            {
                // If the position is not in the cache, generate a new texture.
                cachedMinimap = GenerateMapRenderTexture();
                // Add the newly generated texture to the cache for future use.
                _minimapCache[position] = cachedMinimap;
            }

            // Clear the texture, whether it's newly generated or already existed in the cache
            cachedMinimap.Clear(Color.Transparent);

            if (!_mapGrid.TryGetValue(position, out var cachedMapGrid) || cachedMapGrid == null)
            {
                return;
            }

            foreach (var layer in Options.Instance.MinimapOpts.RenderLayers)
            {
                for (var x = 0; x < Options.Instance.MapOpts.MapWidth; x++)
                {
                    for (var y = 0; y < Options.Instance.MapOpts.MapHeight; y++)
                    {
                        var curTile = cachedMapGrid.Layers[layer][x, y];

                        if (curTile.TilesetId == Guid.Empty ||
                            !TilesetBase.TryGet(curTile.TilesetId, out var tileSet))
                        {
                            continue;
                        }

                        var texture = ContentManager.GetTexture(TextureType.Tileset, tileSet.Name);

                        if (texture == null)
                        {
                            continue;
                        }

                        Graphics.Renderer.DrawTexture(
                            texture,
                            curTile.X * Options.Instance.MapOpts.TileWidth + (Options.Instance.MapOpts.TileWidth / 2),
                            curTile.Y * Options.Instance.MapOpts.TileHeight + (Options.Instance.MapOpts.TileHeight / 2),
                            1,
                            1,
                            x * _minimapTileSize.X,
                            y * _minimapTileSize.Y,
                            _minimapTileSize.X,
                            _minimapTileSize.Y,
                            Color.White,
                            cachedMinimap);
                    }
                }
            }
        }

        private void GenerateEntityCacheFor(MapPosition position)
        {
            if (!_entityCache.TryGetValue(position, out var cachedEntity))
            {
                // If the position is not in the cache, generate a new texture.
                cachedEntity = GenerateMapRenderTexture();
                // Add the newly generated texture to the cache for future use.
                _entityCache[position] = cachedEntity;
            }

            // Clear the texture, whether it's newly generated or already existed in the cache
            cachedEntity.Clear(Color.Transparent);

            if (!_entityInfoCache.TryGetValue(position, out var cachedEntityInfo) || cachedEntityInfo == null)
            {
                return;
            }

            foreach (var entity in cachedEntityInfo)
            {
                var texture = _whiteTexture;
                var color = entity.Value.Color;

                if (!string.IsNullOrWhiteSpace(entity.Value.Texture))
                {
                    var found = ContentManager.Find<GameTexture>(ContentTypes.Miscellaneous, entity.Value.Texture);
                    if (found != null)
                    {
                        texture = found;
                        color = Color.White;
                    }
                }

                Graphics.Renderer.DrawTexture(
                    texture,
                    0,
                    0,
                    texture.Width,
                    texture.Height,
                    entity.Key.X * _minimapTileSize.X,
                    entity.Key.Y * _minimapTileSize.Y,
                    _minimapTileSize.X,
                    _minimapTileSize.Y,
                    color,
                    cachedEntity,
                    GameBlendModes.Add);
            }
        }

        private void DrawMinimapCacheToTexture(MapPosition position)
        {
            if (!_minimapCache.TryGetValue(position, out var value))
            {
                return;
            }

            var x = 0;
            var y = 0;

            switch (position)
            {
                case MapPosition.TopLeft:
                    break;

                case MapPosition.TopMiddle:
                    x = value.Width;
                    break;

                case MapPosition.TopRight:
                    x = value.Width * 2;
                    break;

                case MapPosition.MiddleLeft:
                    y = value.Height;
                    break;

                case MapPosition.Middle:
                    x = value.Width;
                    y = value.Height;
                    break;

                case MapPosition.MiddleRight:
                    x = value.Width * 2;
                    y = value.Height;
                    break;

                case MapPosition.BottomLeft:
                    y = value.Height * 2;
                    break;

                case MapPosition.BottomMiddle:
                    x = value.Width;
                    y = value.Height * 2;
                    break;

                case MapPosition.BottomRight:
                    x = value.Width * 2;
                    y = value.Height * 2;
                    break;
            }

            if (_minimapCache.TryGetValue(position, out var cachedMinimap))
            {
                Graphics.Renderer.DrawTexture(cachedMinimap, 0, 0, cachedMinimap.Width,
                    cachedMinimap.Height, x, y, cachedMinimap.Width, cachedMinimap.Height,
                    Color.White, _renderTexture);
            }

            if (_entityCache.TryGetValue(position, out var cachedEntity))
            {
                Graphics.Renderer.DrawTexture(cachedEntity, 0, 0, cachedEntity.Width,
                    cachedEntity.Height, x, y, cachedEntity.Width, cachedEntity.Height,
                    Color.White, _renderTexture);
            }
        }

        private static Dictionary<MapPosition, MapBase> CreateMapGridFromMap(MapInstance map)
        {
            var grid = new Dictionary<MapPosition, MapBase>();
            for (var x = map.GridX - 1; x <= map.GridX + 1; x++)
            {
                for (var y = map.GridY - 1; y <= map.GridY + 1; y++)
                {
                    if (x < 0 || x >= Globals.MapGridWidth ||
                        y < 0 || y >= Globals.MapGridHeight)
                    {
                        continue;
                    }

                    var currentGridValue = Globals.MapGrid[x, y];

                    if (currentGridValue == Guid.Empty)
                    {
                        continue;
                    }

                    int minimapX = x - (map.GridX - 1);
                    int minimapY = y - (map.GridY - 1);

                    var mapBase = MapBase.Get(currentGridValue);
                    if (mapBase != null)
                    {
                        grid.Add((MapPosition)(minimapX + (minimapY * 3)), mapBase);
                    }
                }
            }

            return grid;
        }

        private Dictionary<MapPosition, Dictionary<Point, EntityInfo>> GenerateEntityInfo(Dictionary<Guid, Entity> entities, Player player)
        {
            var entityInfo = new Dictionary<MapPosition, Dictionary<Point, EntityInfo>>();
            var minimapOptions = Options.Instance.MinimapOpts;
            var minimapColorOptions = minimapOptions.MinimapColors;
            var minimapImageOptions = minimapOptions.MinimapImages;

            foreach (var entity in entities.Values)
            {
                if (!_mapPosition.TryGetValue(entity.MapInstance.Id, out var map))
                {
                    continue;
                }

                if (entity.IsHidden)
                {
                    continue;
                }

                var color = Color.Transparent;
                var texture = string.Empty;

                switch (entity.Type)
                {
                    case EntityType.Player:
                        if (entity.IsStealthed)
                        {
                            color = Color.Transparent;
                            texture = string.Empty;
                        }
                        else
                        {
                            if (entity.Id == player.Id)
                            {
                                color = minimapColorOptions.MyEntity;
                                texture = minimapImageOptions.MyEntity;
                            }
                            else if (player.IsInMyParty(entity.Id))
                            {
                                color = minimapColorOptions.PartyMember;
                                texture = minimapImageOptions.PartyMember;
                            }
                            else
                            {
                                color = minimapColorOptions.Player;
                                texture = minimapImageOptions.Player;
                            }
                        }
                        break;

                    case EntityType.Event:
                        color = Color.Transparent;
                        texture = minimapImageOptions.Event;

                        break;

                    case EntityType.GlobalEntity:
                        if (entity.IsStealthed)
                        {
                            color = Color.Transparent;
                            texture = string.Empty;
                        }
                        else
                        {
                            color = minimapColorOptions.Npc;
                            texture = minimapImageOptions.Npc;
                        }
                        break;

                    case EntityType.Resource:
                        // This relies on users configuring it PROPERLY.
                        var tool = ((Resource)entity).BaseResource.Tool;
                        var texSet = false;
                        var colSet = false;

                        // Is the tool a valid one to get the string version for?
                        if (tool >= 0 && tool < Options.Instance.EquipmentOpts.ToolTypes.Count)
                        {
                            // Get the actual tool type from the server configuration.
                            var toolType = Options.Instance.EquipmentOpts.ToolTypes[tool];

                            // Attempt to get our color from the plugin configuration.
                            if (minimapColorOptions.Resource.TryGetValue(toolType, out color))
                            {
                                colSet = true;
                            }

                            // Attempt to get our texture from the plugin configuration.
                            if (minimapImageOptions.Resource.TryGetValue(toolType, out texture))
                            {
                                texSet = true;
                            }
                        }
                        // Is it a None tool?
                        else if (tool == -1)
                        {
                            color = minimapColorOptions.Resource["None"];
                            colSet = true;
                            texture = minimapImageOptions.Resource["None"];
                            texSet = true;
                        }

                        // Have we managed to set our color? If not, set to default.
                        if (!colSet)
                        {
                            color = minimapColorOptions.Default;
                        }

                        // Have we managed to set our texture? If not, set to blank.
                        if (!texSet)
                        {
                            texture = minimapImageOptions.Default;
                        }

                        break;

                    case EntityType.Projectile:
                        continue;

                    default:
                        color = minimapColorOptions.Default;
                        texture = minimapImageOptions.Default;
                        break;
                }


                // Add this to our location dictionary!
                if (!entityInfo.TryGetValue(map, out var locationDictionary))
                {
                    locationDictionary = new Dictionary<Point, EntityInfo>();
                    entityInfo.Add(map, locationDictionary);
                }

                if (color == null || texture == null)
                {
                    continue;
                }

                locationDictionary.TryAdd(new Point(entity.X, entity.Y),
                    new EntityInfo { Color = color, Texture = texture });
            }

            return entityInfo;
        }

        private GameRenderTexture GenerateRenderTexture(int multiplier)
        {
            var sizeX = _minimapTileSize.X * Options.Instance.MapOpts.MapWidth * multiplier;
            var sizeY = _minimapTileSize.Y * Options.Instance.MapOpts.MapHeight * multiplier;

            return Graphics.Renderer.CreateRenderTexture(sizeX, sizeY);
        }

        private GameRenderTexture GenerateBaseRenderTexture()
        {
            return GenerateRenderTexture(3);
        }

        private GameRenderTexture GenerateMapRenderTexture()
        {
            return GenerateRenderTexture(1);
        }

        private void MZoomOutButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _zoomLevel = Math.Min(_zoomLevel + Options.Instance.MinimapOpts.ZoomStep,
                Options.Instance.MinimapOpts.MaximumZoom);
        }

        private void MZoomInButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _zoomLevel = Math.Max(_zoomLevel - Options.Instance.MinimapOpts.ZoomStep,
                Options.Instance.MinimapOpts.MinimumZoom);
        }

        private enum MapPosition
        {
            TopLeft,
            TopMiddle,
            TopRight,
            MiddleLeft,
            Middle,
            MiddleRight,
            BottomLeft,
            BottomMiddle,
            BottomRight,
        }

        private sealed class EntityInfo
        {
            public Color Color { get; set; }

            public string Texture { get; set; }
        }
    }
}
using System.Diagnostics;
using System.Collections.Concurrent;
using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Items;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Compression;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.Serialization;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Intersect.Client.Maps;


public partial class MapInstance : MapBase, IGameObject<Guid, MapInstance>, IMapInstance
{
    public const int MapAnimationFrames = 3;
    private const int XpAnimationFrameTileWidth = 3;
    private const int VxAnimationFrameTileWidth = 2;

    //Client Only Values
    public delegate void MapLoadedDelegate(MapInstance map);

    //Map State Variables
    public static Dictionary<Guid, long> MapRequests { get; set; } = new Dictionary<Guid, long>();

    public static event MapLoadedDelegate? MapLoaded;

    public void MarkLoadFinished()
    {
        ApplicationContext.CurrentContext.Logger.LogDebug(
            "Done loading map {Id} ({Name}) @ ({GridX}, {GridY})",
            Id,
            Name,
            GridX,
            GridY
        );
        MapLoaded?.Invoke(this);
    }

    private static MapControllers sLookup;

    public List<IWeatherParticle> _removeParticles { get; set; } = new List<IWeatherParticle>();

    //Weather
    public List<IWeatherParticle> _weatherParticles { get; set; } = new List<IWeatherParticle>();

    private long _weatherParticleSpawnTime;

    //Action Msg's
    public List<IActionMessage> ActionMessages { get; set; } = new List<IActionMessage>();

    IReadOnlyList<IActionMessage> IMapInstance.ActionMessages => ActionMessages;

    //Attribute Sounds
    public List<IMapSound> AttributeSounds { get; set; } = new List<IMapSound>();

    IReadOnlyList<IMapSound> IMapInstance.AttributeSounds => AttributeSounds;

    //Map Animations
    public ConcurrentDictionary<Guid, MapAnimation> LocalAnimations { get; set; } = new ConcurrentDictionary<Guid, MapAnimation>();

    IReadOnlyList<IMapAnimation> IMapInstance.Animations => LocalAnimations.Values.ToList();

    public Dictionary<Guid, Entity> LocalEntities { get; } = [];

    IReadOnlyList<IEntity> IMapInstance.Entities => LocalEntities.Values.ToList();

    //Map Critters
    public Dictionary<Guid, Critter> LocalCritters { get; set; } = new Dictionary<Guid, Critter>();

    IReadOnlyList<IEntity> IMapInstance.Critters => LocalCritters.Values.ToList();

    //Map Players/Events/Npcs
    public List<Guid> LocalEntitiesToDispose { get; set; } = new List<Guid>();

    //Map Items
    public Dictionary<int, List<IMapItemInstance>> MapItems { get; set; } = new Dictionary<int, List<IMapItemInstance>>();

    IReadOnlyList<IMapItemInstance> IMapInstance.Items => MapItems.Values.SelectMany(x => x).ToList();

    //Map Attributes
    private Dictionary<GameObjects.Maps.MapAttribute, Animation> mAttributeAnimInstances = new Dictionary<GameObjects.Maps.MapAttribute, Animation>();
    private Dictionary<GameObjects.Maps.MapAttribute, Entity> mAttributeCritterInstances = new Dictionary<GameObjects.Maps.MapAttribute, Entity>();

    protected float mCurFogIntensity;

    private List<Event> mEvents = new List<Event>();

    protected float mFogCurrentX;

    protected float mFogCurrentY;

    //Fog Variables
    protected long mFogUpdateTime = -1;

    //Update Timer
    private long _lastUpdateTime;

    protected float mOverlayIntensity;

    //Overlay Image Variables
    protected long mOverlayUpdateTime = -1;

    protected float mPanoramaIntensity;

    //Panorama Variables
    protected long mPanoramaUpdateTime = -1;

    private bool mTexturesFound = false;

    private readonly Dictionary<string, Dictionary<object, GameTileBuffer[]>> _tileBuffersPerTexturePerLayer = []; // [Layer][Platform Texture][Buffer Index]

    private readonly Dictionary<string, GameTileBuffer[][]> _tileBuffersPerLayer = []; // [Layer][Autotile Frame][Buffer Index]

    //Initialization
    public MapInstance(Guid id) : base(id)
    {
    }

    public bool IsDisposed { get; private set; }

    public bool IsLoaded { get; private set; }

    //Camera Locking Variables
    public bool[] CameraHolds { get; set; } = new bool[4];

    //World Position
    public int GridX
    {
        get => _gridX;
        set
        {
            if (value == _gridX)
            {
                return;
            }

            _gridX = value;
            X = _gridX * _width * _tileWidth;
        }
    }

    public int GridY
    {
        get => _gridY;
        set
        {
            if (value == _gridY)
            {
                return;
            }

            _gridY = value;
            Y = _gridY * _height * _tileHeight;
        }
    }

    //Map Sounds
    public IMapSound? BackgroundSound { get; set; }

    public new static MapControllers Lookup => sLookup ?? (sLookup = new MapControllers(MapBase.Lookup));

    //Load
    public void Load(string json)
    {
        LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
        JsonConvert.PopulateObject(
            json,
            this,
            new JsonSerializerSettings
            {
                SerializationBinder = new IntersectTypeSerializationBinder(),
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            }
        );

        IsLoaded = true;
        Autotiles = new MapAutotiles(this);
        MapLoaded -= HandleMapLoaded;
        MapLoaded += HandleMapLoaded;
        MapRequests.Remove(Id);
    }

    public void LoadTileData(byte[] packet)
    {
        Layers = JsonConvert.DeserializeObject<Dictionary<string, Tile[,]>>(LZ4.UnPickleString(packet), mJsonSerializerSettings);
        foreach (var layer in Options.Instance.Map.Layers.All)
        {
            if (!Layers.ContainsKey(layer))
            {
                Layers.Add(layer, new Tile[_width, _height]);
            }
        }
    }

    private void CacheTextures()
    {
        if (mTexturesFound || !GameContentManager.Current.TilesetsLoaded)
        {
            return;
        }

        foreach (var layer in Options.Instance.Map.Layers.All)
        {
            if (!Layers.TryGetValue(layer, out var layerTiles))
            {
                continue;
            }

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var layerTile = layerTiles[x, y];
                    if (layerTile.TilesetId == default)
                    {
                        continue;
                    }

                    if (!TilesetBase.TryGet(layerTile.TilesetId, out var tileset))
                    {
                        continue;
                    }

                    var tilesetTexture = Globals.ContentManager.GetTexture(
                        Framework.Content.TextureType.Tileset,
                        tileset.Name
                    );

                    // Tile is a struct, this has to be an array index
                    layerTiles[x, y].TilesetTexture = tilesetTexture;
                }
            }
        }

        mTexturesFound = true;
    }

    //Updating
    public void Update(bool isLocal)
    {
        var nowMilliseconds = Timing.Global.Milliseconds;
        if (!isLocal)
        {
            if (nowMilliseconds > _lastUpdateTime)
            {
                Dispose();
            }

            HideActiveAnimations();
            return;
        }

        _lastUpdateTime = nowMilliseconds + 10000;
        UpdateMapAttributes();
        if (BackgroundSound == null && !string.IsNullOrWhiteSpace(Sound))
        {
            BackgroundSound = Audio.AddMapSound(
                Sound,
                -1,
                -1,
                Id,
                true,
                0,
                10
            );
        }

        foreach (var (animationId, animation) in LocalAnimations)
        {
            if (animation.IsDisposed)
            {
                LocalAnimations.TryRemove(animationId, out _);
            }
            else
            {
                animation.Update();
            }
        }

        foreach (var (_, entity) in LocalEntities)
        {
            entity.Update();
        }

        foreach (var (_, critter) in mAttributeCritterInstances)
        {
            critter.Update();
        }

        foreach (var localEntityToDispose in LocalEntitiesToDispose)
        {
            LocalEntities.Remove(localEntityToDispose);
        }

        LocalEntitiesToDispose.Clear();
    }

    public bool InView()
    {
        var myMap = Globals.Me.MapInstance;
        if (Globals.MapGridWidth == 0 || Globals.MapGridHeight == 0 || myMap == null)
        {
            return true;
        }

        var gridX = myMap.GridX;
        var gridY = myMap.GridY;
        for (var x = gridX - 1; x <= gridX + 1; x++)
        {
            for (var y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight)
                {
                    if (Globals.MapGrid[x, y] == Id)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleMapLoaded(MapInstance map)
    {
        //See if this new map is on the same grid as us
        var updatedBuffers = new HashSet<GameTileBuffer>();
        if (map != this && Globals.GridMaps.ContainsKey(map.Id) && Globals.GridMaps.ContainsKey(Id) && IsLoaded)
        {
            var surroundingMaps = GenerateAutotileGrid();
            if (map.GridX == GridX - 1)
            {
                if (map.GridY == GridY - 1)
                {
                    //Check Northwest
                    updatedBuffers.UnionWith(CheckAutotile(0, 0, surroundingMaps));
                }
                else if (map.GridY == GridY)
                {
                    //Check West
                    for (var y = 0; y < _height; y++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(0, y, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check Southwest
                    updatedBuffers.UnionWith(CheckAutotile(0, _height - 1, surroundingMaps));
                }
            }
            else if (map.GridX == GridX)
            {
                if (map.GridY == GridY - 1)
                {
                    //Check North
                    for (var x = 0; x < _width; x++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(x, 0, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check South
                    for (var x = 0; x < _width; x++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(x, _height - 1, surroundingMaps));
                    }
                }
            }
            else if (map.GridX == GridX + 1)
            {
                if (map.GridY == GridY - 1)
                {
                    //Check Northeast
                    updatedBuffers.UnionWith(
                        CheckAutotile(_width - 1, _height, surroundingMaps)
                    );
                }
                else if (map.GridY == GridY)
                {
                    //Check East
                    for (var y = 0; y < _height; y++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(_width - 1, y, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check Southeast
                    updatedBuffers.UnionWith(
                        CheckAutotile(_width - 1, _height - 1, surroundingMaps)
                    );
                }
            }

            //Along with edges we need to recalculate ALL cliffs :(
            foreach (var layer in Options.Instance.Map.Layers.All)
            {
                for (var x = 0; x < _width; x++)
                {
                    for (var y = 0; y < _height; y++)
                    {
                        if (Layers[layer][x, y].Autotile == MapAutotiles.AUTOTILE_CLIFF)
                        {
                            updatedBuffers.UnionWith(CheckAutotile(x, y, surroundingMaps));
                        }
                    }
                }
            }
        }

        foreach (var buffer in updatedBuffers)
        {
            buffer.SetData();
        }
    }

    private GameTileBuffer[] CheckAutotile(int x, int y, MapBase[,] surroundingMaps)
    {
        var mapXOffset = X;
        var mapYOffset = Y;

        var updated = new List<GameTileBuffer>();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var layer in Options.Instance.Map.Layers.All)
        {
            if (!Autotiles.UpdateAutoTile(
                    x,
                    y,
                    layer,
                    surroundingMaps
                ))
            {
                continue;
            }

            // Find the VBO, update it.
            if (!_tileBuffersPerTexturePerLayer.TryGetValue(layer, out var tileBuffer))
            {
                continue;
            }

            if (!Layers.TryGetValue(layer, out var layerTiles))
            {
                continue;
            }

            var tile = layerTiles[x, y];
            if (tile.TilesetTexture == default)
            {
                continue;
            }

            var tilesetTexture = (IGameTexture)tile.TilesetTexture;
            if (tile.X < 0 || tile.Y < 0)
            {
                continue;
            }

            if (tile.X * _tileWidth >= tilesetTexture.Width ||
                tile.Y * _tileHeight >= tilesetTexture.Height)
            {
                continue;
            }

            var tilesetPlatformTexture = tilesetTexture.GetTexture();
            if (!tileBuffer.TryGetValue(tilesetPlatformTexture, out var tileBuffersForTexture))
            {
                continue;
            }

            var tileX = x * _tileWidth + mapXOffset;
            var tileY = y * _tileHeight + mapYOffset;

            for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
            {
                var buffer = tileBuffersForTexture[animationFrameIndex];

                DrawAutoTile(
                    layer,
                    tileX,
                    tileY,
                    1,
                    x,
                    y,
                    animationFrameIndex,
                    tilesetTexture,
                    buffer,
                    true
                );

                DrawAutoTile(
                    layer,
                    tileX + _tileHalfWidth,
                    tileY,
                    2,
                    x,
                    y,
                    animationFrameIndex,
                    tilesetTexture,
                    buffer,
                    true
                );

                DrawAutoTile(
                    layer,
                    tileX,
                    tileY + _tileHalfHeight,
                    3,
                    x,
                    y,
                    animationFrameIndex,
                    tilesetTexture,
                    buffer,
                    true
                );

                DrawAutoTile(
                    layer,
                    tileX + _tileHalfWidth,
                    tileY + _tileHalfHeight,
                    4,
                    x,
                    y,
                    animationFrameIndex,
                    tilesetTexture,
                    buffer,
                    true
                );

                if (!updated.Contains(buffer))
                {
                    updated.Add(buffer);
                }
            }
        }

        return updated.ToArray();
    }

    //Helper Functions
    public MapBase[,] GenerateAutotileGrid()
    {
        var mapBase = new MapBase[3, 3];
        if (Globals.MapGrid != null && Globals.GridMaps.ContainsKey(Id))
        {
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    var x1 = GridX + x;
                    var y1 = GridY + y;
                    if (x1 >= 0 && y1 >= 0 && x1 < Globals.MapGridWidth && y1 < Globals.MapGridHeight)
                    {
                        if (x == 0 && y == 0)
                        {
                            mapBase[x + 1, y + 1] = this;
                        }
                        else
                        {
                            mapBase[x + 1, y + 1] = Lookup.Get<MapInstance>(Globals.MapGrid[x1, y1]);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.WriteLine("Returning null mapgrid for map " + Name);
        }

        return mapBase;
    }

    /// <summary>
    /// X coordinate of the map in world space
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Y coordinate of the map in world space
    /// </summary>
    public int Y { get; private set; }

    // Attribute References
    private void UpdateMapAttributes()
    {
        var mapAttributes = Attributes;
        if (mapAttributes == default)
        {
            return;
        }

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var mapAttribute = mapAttributes[x, y];
                if (mapAttribute == default)
                {
                    continue;
                }

                switch (mapAttribute.Type)
                {
                    case MapAttributeType.Animation:
                    {
                        var anim = AnimationBase.Get(((MapAnimationAttribute)mapAttribute).AnimationId);
                        if (anim == null)
                        {
                            continue;
                        }

                        if (!mAttributeAnimInstances.ContainsKey(mapAttribute))
                        {
                            var animInstance = new Animation(anim, true);
                            animInstance.SetPosition(
                                X + x * _tileWidth + _tileHalfWidth,
                                Y + y * _tileHeight + _tileHalfHeight, x, y, Id, 0
                            );

                            mAttributeAnimInstances.Add(mapAttribute, animInstance);
                        }

                        mAttributeAnimInstances[mapAttribute].Update();
                        break;
                    }
                    case MapAttributeType.Critter:
                    {
                        var critterAttribute = ((MapCritterAttribute)mapAttribute);
                        var sprite = critterAttribute.Sprite;
                        var anim = AnimationBase.Get(critterAttribute.AnimationId);
                        if (anim == null && TextUtils.IsNone(sprite))
                        {
                            continue;
                        }

                        if (!mAttributeCritterInstances.ContainsKey(mapAttribute))
                        {
                            var critter = new Critter(this, (byte)x, (byte)y, critterAttribute);
                            LocalCritters.Add(critter.Id, critter);
                            mAttributeCritterInstances.Add(mapAttribute, critter);
                        }

                        mAttributeCritterInstances[mapAttribute].Update();
                        break;
                    }
                }
            }
        }
    }

    private void ClearMapAttributes()
    {
        foreach (var attributeInstance in mAttributeAnimInstances)
        {
            attributeInstance.Value.Dispose();
        }

        foreach (var critter in mAttributeCritterInstances)
        {
            critter.Value.Dispose();
        }

        LocalCritters.Clear();
        mAttributeCritterInstances.Clear();
        mAttributeAnimInstances.Clear();
    }

    //Sound Functions
    public void CreateMapSounds()
    {
        ClearAttributeSounds();
        for (var x = 0; x < _width; ++x)
        {
            for (var y = 0; y < _height; ++y)
            {
                var attribute = Attributes?[x, y];
                if (attribute?.Type != MapAttributeType.Sound)
                {
                    continue;
                }

                if (TextUtils.IsNone(((MapSoundAttribute)attribute).File))
                {
                    continue;
                }

                var sound = Audio.AddMapSound(
                    ((MapSoundAttribute)attribute).File, x, y, Id, true, ((MapSoundAttribute)attribute).LoopInterval, ((MapSoundAttribute)attribute).Distance
                );

                AttributeSounds?.Add(sound);
            }
        }
    }

    private void ClearAttributeSounds()
    {
        AttributeSounds?.ForEach(Audio.StopSound);
        AttributeSounds?.Clear();
    }

    //Animations
    public void AddTileAnimation(
        Guid animId,
        int tileX,
        int tileY,
        Direction dir = Direction.None,
        IEntity? owner = null,
        AnimationSource source = default
    )
    {
        var animBase = AnimationBase.Get(animId);
        if (animBase == null)
        {
            return;
        }

        var anim = new MapAnimation(
            animBase,
            tileX,
            tileY,
            dir,
            owner as Entity,
            source: source
        );
        LocalAnimations.TryAdd(anim.Id, anim);
        anim.SetPosition(
            X + tileX * _tileWidth + _tileHalfWidth,
            Y + tileY * _tileHeight + _tileHalfHeight,
            tileX,
            tileY,
            Id,
            dir
        );
    }

    private void HideActiveAnimations()
    {
        foreach (var entity in LocalEntities.Values.ToList())
        {
            entity.ClearAnimations();
        }
        foreach (var anim in LocalAnimations)
        {
            anim.Value?.Dispose();
        }
        LocalAnimations?.Clear();
        ClearMapAttributes();
    }

    private Task? _vboCompute;

    public void BuildVBOs()
    {
        lock (_tileBuffersPerLayer)
        {
            if (_vboCompute != default)
            {
                return;
            }

            // _vboCompute = Task.Run(
            //     () =>
            //     {
                    var startVbo = DateTime.UtcNow;
                    Dictionary<string, GameTileBuffer[][]> buffers = [];
                    foreach (var layer in Options.Instance.Map.Layers.All)
                    {
                        var layerBuffers = DrawMapLayer(layer, X, Y);
                        if (layerBuffers == default)
                        {
                            continue;
                        }

                        buffers[layer] = layerBuffers;
                        for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
                        {
                            var layerBuffersForFrame = layerBuffers[animationFrameIndex];
                            foreach (var tileBuffer in layerBuffersForFrame)
                            {
                                tileBuffer.SetData();
                            }
                        }
                    }

                    var endVbo = DateTime.UtcNow;
                    var elapsedVbo = endVbo - startVbo;
                    ApplicationContext.Context.Value?.Logger.LogInformation($"Built VBO for map {Id} '{Name}' in {elapsedVbo.TotalMilliseconds}ms");

                    // lock (mTileBuffers)
                    // {
                        foreach (var (layer, layerBuffers) in buffers)
                        {
                            _tileBuffersPerLayer[layer] = layerBuffers;
                        }
                    // }

            //         _vboCompute = default;
            //     }
            // );
        }
    }

    public void DestroyVBOs()
    {
        foreach (var layer in Options.Instance.Map.Layers.All)
        {
            if (_tileBuffersPerTexturePerLayer.Remove(layer, out var tileBuffersPerTextureForLayer))
            {
                tileBuffersPerTextureForLayer.Clear();
            }

            if (!_tileBuffersPerLayer.TryGetValue(layer, out var layerBuffers))
            {
                continue;
            }

            foreach (var layerBuffersForFrame in layerBuffers)
            {
                foreach (var tileBuffer in layerBuffersForFrame)
                {
                    tileBuffer.Dispose();
                }
            }
        }

        _tileBuffersPerTexturePerLayer.Clear();
        _tileBuffersPerLayer.Clear();
    }

    //Rendering/Drawing Code
    public void Draw(int layer) //Lower, Middle, Upper
    {
        if (!IsLoaded)
        {
            return;
        }

        CacheTextures();
        if (!mTexturesFound)
        {
            return;
        }

        if (_tileBuffersPerLayer.Count < 1)
        {
            BuildVBOs();
        }

        var drawLayers = layer switch
        {
            1 => Options.Instance.Map.Layers.MiddleLayers,
            > 1 => Options.Instance.Map.Layers.UpperLayers,
            < 1 => Options.Instance.Map.Layers.LowerLayers,
        };

        foreach (var drawLayer in drawLayers)
        {
            if (!_tileBuffersPerLayer.TryGetValue(drawLayer, out var layerBuffers))
            {
                continue;
            }

            if (layerBuffers[Globals.AnimationFrame] == null)
            {
                continue;
            }

            var buffersForFrame = layerBuffers[Globals.AnimationFrame];
            foreach (var buffer in buffersForFrame)
            {
                Graphics.Renderer?.DrawTileBuffer(buffer);
            }
        }
    }

    public void DrawItemsAndLights()
    {
        // Calculate tile and map item dimensions.
        var mapItemWidth = Options.Instance.Map.MapItemWidth;
        var mapItemHeight = Options.Instance.Map.MapItemHeight;

        // Draw map items.
        foreach (var (key, tileItems) in MapItems)
        {
            // Calculate tile coordinates.
            var tileX = key % _width;
            var tileY = (int)Math.Floor(key / (float)_width);

            // Loop through this in reverse to match client/server display and pick-up order.
            for (var index = tileItems.Count - 1; index >= 0; index--)
            {
                // Set up all information we need to draw this name.
                var itemBase = ItemBase.Get(tileItems[index].ItemId);
                var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, itemBase.Icon);

                if (itemTex == null)
                {
                    continue;
                }

                var x = X + tileX * _tileWidth;
                var y = Y + tileY * _tileHeight;
                var centerX = x + (_tileWidth / 2);
                var centerY = y + (_tileHeight / 2);
                var textureXPosition = centerX - (mapItemWidth / 2);
                var textureYPosition = centerY - (mapItemHeight / 2);

                // Draw the item texture.
                Graphics.DrawGameTexture(
                    itemTex,
                    new FloatRect(0, 0, itemTex.Width, itemTex.Height),
                    new FloatRect(textureXPosition, textureYPosition, mapItemWidth, mapItemHeight),
                    itemBase.Color
                );
            }
        }

        //Add lights to our darkness texture
        foreach (var light in Lights)
        {
            double w = light.Size;
            var x = X + (light.TileX * _tileWidth + light.OffsetX) + _tileWidth / 2f;
            var y = Y + (light.TileY * _tileHeight + light.OffsetY) + _tileHeight / 2f;
            Graphics.AddLight((int)x, (int)y, (int)w, light.Intensity, light.Expand, light.Color);
        }
    }

    /// <summary>
    /// Draws all names of the items on the tile the user is hovering over.
    /// </summary>
    public void DrawItemNames()
    {
        if (Interface.Interface.DoesMouseHitInterface())
        {
            return;
        }
        // Get where our mouse is located and convert it to a tile based location.
        var mousePos = Graphics.ConvertToWorldPoint(
                Globals.InputManager.GetMousePosition()
        );
        var x = (int)(mousePos.X - (int)X) / _tileWidth;
        var y = (int)(mousePos.Y - (int)Y) / _tileHeight;
        var mapId = Id;

        // Is this an actual location on this map?
        if (Globals.Me.TryGetRealLocation(ref x, ref y, ref mapId) && mapId == Id)
        {
            // Apparently it is! Do we have any items to render here?
            var tileItems = new List<IMapItemInstance>();
            if (MapItems.TryGetValue(y * _width + x, out tileItems))
            {
                var baseOffset = 0;
                // Loop through this in reverse to match client/server display and pick-up order.
                for (var index = tileItems.Count - 1; index >= 0; index--)
                {
                    // Set up all information we need to draw this name.
                    var itemBase = ItemBase.Get(tileItems[index].ItemId);
                    var name = tileItems[index].Base.Name;
                    var quantity = tileItems[index].Quantity;
                    var rarity = itemBase.Rarity;
                    if (tileItems[index].Quantity > 1)
                    {
                        name = Strings.General.MapItemStackable.ToString(name, Strings.FormatQuantityAbbreviated(quantity));
                    }
                    var color = CustomColors.Items.MapRarities.ContainsKey(rarity)
                        ? CustomColors.Items.MapRarities[rarity]
                        : new LabelColor(Color.White, Color.Black, new Color(100, 0, 0, 0));
                    var textSize = Graphics.Renderer.MeasureText(name, Graphics.EntityNameFont, 1);
                    var offsetY = (baseOffset * textSize.Y);
                    var destX = X + (int)Math.Ceiling(((x * _tileWidth) + (_tileHalfWidth)) - (textSize.X / 2));
                    var destY = Y + (int)Math.Ceiling(((y * _tileHeight) - ((_tileHeight / 3) + textSize.Y))) - offsetY;

                    // Do we need to draw a background?
                    if (color.Background != Color.Transparent)
                    {
                        Graphics.DrawGameTexture(
                            Graphics.Renderer.WhitePixel, new FloatRect(0, 0, 1, 1),
                            new FloatRect(destX - 4, destY, textSize.X + 8, textSize.Y), color.Background
                        );
                    }

                    // Finaly, draw the actual name!
                    Graphics.Renderer.DrawString(name, Graphics.EntityNameFont, destX, destY, 1, color.Name, true, null, color.Outline);

                    baseOffset++;
                }
            }
        }
    }

    private readonly int _width = Options.Instance.Map.MapWidth;
    private readonly int _height = Options.Instance.Map.MapHeight;
    private readonly int _tileWidth = Options.Instance.Map.TileWidth;
    private readonly int _tileHeight = Options.Instance.Map.TileHeight;
    private readonly int _tileHalfWidth = Options.Instance.Map.TileWidth / 2;
    private readonly int _tileHalfHeight = Options.Instance.Map.TileHeight / 2;
    private int _gridY;
    private int _gridX;

    private void DrawAutoTile(
        string layerName,
        int destX,
        int destY,
        int quarterNum,
        int x,
        int y,
        int forceFrame,
        IGameTexture tileset,
        GameTileBuffer buffer,
        bool update = false,
        Tile? layerTile = default,
        QuarterTileCls? layerAutoTile = default
    )
    {
        if (layerTile == null)
        {
            if (!Layers.TryGetValue(layerName, out var layerTiles))
            {
                return;
            }

            layerTile = layerTiles[x, y];
        }

        if (layerAutoTile == null)
        {
            if (!Autotiles.Layers.TryGetValue(layerName, out var layerAutoTiles))
            {
                return;
            }

            layerAutoTile = layerAutoTiles[x, y];
        }

        var quarterTile = layerAutoTile.QuarterTile[quarterNum];

        int yOffset = 0, xOffset = 0;

        // calculate the offset

        switch (layerTile.Value.Autotile)
        {
            case MapAutotiles.AUTOTILE_WATERFALL:
                yOffset = (forceFrame - 1) * _tileHeight;
                break;

            case MapAutotiles.AUTOTILE_ANIM:
                xOffset = forceFrame * _tileWidth * VxAnimationFrameTileWidth;
                break;

            case MapAutotiles.AUTOTILE_ANIM_XP:
                xOffset = forceFrame * _tileWidth * XpAnimationFrameTileWidth;
                break;

            case MapAutotiles.AUTOTILE_CLIFF:
                yOffset = -_tileHeight;
                break;
        }

        if (update)
        {
            if (!buffer.TryUpdateTile(
                    tileset,
                    destX,
                    destY,
                    quarterTile.X + xOffset,
                    quarterTile.Y + yOffset,
                    _tileHalfWidth,
                    _tileHalfHeight
                ))
            {
                throw new Exception("Failed to update tile to VBO!");
            }
        }
        else
        {
            if (!buffer.TryAddTile(
                    tileset,
                    destX,
                    destY,
                    quarterTile.X + xOffset,
                    quarterTile.Y + yOffset,
                    _tileHalfWidth,
                    _tileHalfHeight
                ))
            {
                throw new Exception("Failed to add tile to VBO!");
            }
        }
    }

    private GameTileBuffer[][]? DrawMapLayer(string layerName, int xOffset = 0, int yOffset = 0)
    {
        if (!Layers.TryGetValue(layerName, out var layerTiles))
        {
            return null;
        }

        if (!Autotiles.Layers.TryGetValue(layerName, out var layerAutoTiles))
        {
            return null;
        }

        var tileBuffersPerTexture = new Dictionary<object, GameTileBuffer[]>();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var layerTile = layerTiles[x, y];
                if (layerTile.TilesetTexture is not IGameTexture tilesetTexture)
                {
                    continue;
                }

                if (layerTile.X < 0 || layerTile.Y < 0)
                {
                    continue;
                }

                if (layerTile.X * _tileWidth >= tilesetTexture.Width ||
                    layerTile.Y * _tileHeight >= tilesetTexture.Height)
                {
                    continue;
                }

                var platformTexture = tilesetTexture.GetTexture();
                if (platformTexture == default)
                {
                    continue;
                }

                GameTileBuffer[] animationFrameBuffers;
                if (tileBuffersPerTexture.TryGetValue(platformTexture, out var tileBuffers))
                {
                    animationFrameBuffers = tileBuffers;
                }
                else
                {
                    animationFrameBuffers = new GameTileBuffer[MapAnimationFrames];
                    for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
                    {
                        animationFrameBuffers[animationFrameIndex] = Graphics.Renderer!.CreateTileBuffer();
                    }

                    tileBuffersPerTexture.Add(platformTexture, animationFrameBuffers);
                }

                var tileXOffset = x * _tileWidth + xOffset;
                var tileYOffset = y * _tileHeight + yOffset;

                var layerAutoTile = layerAutoTiles[x, y];
                switch (layerAutoTile.RenderState)
                {
                    case MapAutotiles.RENDER_STATE_NORMAL:
                        for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
                        {
                            var animationFrameBuffer = animationFrameBuffers[animationFrameIndex];
                            if (!animationFrameBuffer.TryAddTile(
                                    tilesetTexture,
                                    tileXOffset,
                                    tileYOffset,
                                    layerTile.X * _tileWidth,
                                    layerTile.Y * _tileHeight,
                                    _tileWidth,
                                    _tileHeight
                                ))
                            {
                                throw new Exception("Failed to add VBO!");
                            }
                        }
                        break;

                    case MapAutotiles.RENDER_STATE_AUTOTILE:
                        for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
                        {
                            var animationFrameBuffer = animationFrameBuffers[animationFrameIndex];
                            DrawAutoTile(
                                layerName,
                                tileXOffset,
                                tileYOffset,
                                1,
                                x,
                                y,
                                animationFrameIndex,
                                tilesetTexture,
                                animationFrameBuffer,
                                layerTile: layerTile,
                                layerAutoTile: layerAutoTile
                            );

                            DrawAutoTile(
                                layerName,
                                tileXOffset + _tileHalfWidth,
                                tileYOffset,
                                2,
                                x,
                                y,
                                animationFrameIndex,
                                tilesetTexture,
                                animationFrameBuffer,
                                layerTile: layerTile,
                                layerAutoTile: layerAutoTile
                            );

                            DrawAutoTile(
                                layerName,
                                tileXOffset,
                                tileYOffset + _tileHalfHeight,
                                3,
                                x,
                                y,
                                animationFrameIndex,
                                tilesetTexture,
                                animationFrameBuffer,
                                layerTile: layerTile,
                                layerAutoTile: layerAutoTile
                            );

                            DrawAutoTile(
                                layerName,
                                tileXOffset + _tileHalfWidth,
                                tileYOffset + _tileHalfHeight,
                                4,
                                x,
                                y,
                                animationFrameIndex,
                                tilesetTexture,
                                animationFrameBuffer,
                                layerTile: layerTile,
                                layerAutoTile: layerAutoTile
                            );
                        }
                        break;
                }
            }
        }

        var outputBuffers = new GameTileBuffer[MapAnimationFrames][];
        for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
        {
            outputBuffers[animationFrameIndex] = new GameTileBuffer[tileBuffersPerTexture.Count];
        }

        var valueArrays = tileBuffersPerTexture.Values.ToArray();
        for (var bufferIndex = 0; bufferIndex < valueArrays.Length; bufferIndex++)
        {
            var bufferGroup = valueArrays[bufferIndex];
            for (var animationFrameIndex = 0; animationFrameIndex < MapAnimationFrames; animationFrameIndex++)
            {
                var bufferForFrame = bufferGroup[animationFrameIndex];
                // if (bufferForFrame is MonoTileBuffer monoTileBuffer)
                // {
                //     ApplicationContext.Context.Value?.Logger.LogInformation($"[{Name}][{layerName}] Buffer for {monoTileBuffer._texture?.Name} frame {i} has {monoTileBuffer._addedTileCount.Count} unique tiles");
                //     foreach (var (key, value) in monoTileBuffer._addedTileCount.OrderByDescending(kvp => kvp.Value))
                //     {
                //         ApplicationContext.Context.Value?.Logger.LogInformation($"[{Name}][{layerName}] {key} has {value} occurrences");
                //     }
                // }
                outputBuffers[animationFrameIndex][bufferIndex] = bufferForFrame;
            }
        }

        _tileBuffersPerTexturePerLayer.Add(layerName, tileBuffersPerTexture);

        return outputBuffers;
    }

    /// <summary>
    /// Draws the fog over the game view.
    /// </summary>
    public void DrawFog()
    {
        // Exit early if the player or map data is not available, or if there is no fog texture.
        if (Globals.Me == null || Lookup.Get(Globals.Me.MapId) == null || string.IsNullOrWhiteSpace(Fog))
        {
            return;
        }

        // Get fog texture and exit early if it is not available.
        var fogTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Fog, Fog);
        if (fogTex == null)
        {
            return;
        }

        // Calculate elapsed time since the last update and set maximum value for elapsedTime to
        // prevent large jumps in fog intensity (1 second maximum).
        float elapsedTime = Math.Min(Timing.Global.MillisecondsUtc - mFogUpdateTime, 1000);
        mFogUpdateTime = Timing.Global.MillisecondsUtc;

        // Update fog intensity based on whether the player is on the current map or not.
        mCurFogIntensity = Id == Globals.Me.MapId
            ? Math.Min(1, mCurFogIntensity + elapsedTime / 2000f)
            : Math.Max(0, mCurFogIntensity - elapsedTime / 2000f);

        // Calculate the number of times the fog texture needs to be drawn to cover the map area.
        var xCount = _width * _tileWidth * 3 / fogTex.Width;
        var yCount = _height * _tileHeight * 3 / fogTex.Height;

        // Update the fog texture's position based on its speed and elapsed time.
        mFogCurrentX += elapsedTime / 1000f * FogXSpeed * 2;
        mFogCurrentY += elapsedTime / 1000f * FogYSpeed * 2;

        // Handle cases where the fog texture's position goes out of bounds.
        mFogCurrentX %= fogTex.Width;
        mFogCurrentY %= fogTex.Height;

        // Round the fog texture's position to the nearest integer value.
        var drawX = (float)Math.Round(mFogCurrentX);
        var drawY = (float)Math.Round(mFogCurrentY);

        for (var x = -1; x <= xCount; x++)
        {
            for (var y = -1; y <= yCount; y++)
            {
                Graphics.DrawGameTexture(
                    fogTex, new FloatRect(0, 0, fogTex.Width, fogTex.Height),
                    new FloatRect(
                        X - _width * _tileWidth * 1f + x * fogTex.Width + drawX,
                        Y - _height * _tileHeight * 1f + y * fogTex.Height + drawY,
                        fogTex.Width, fogTex.Height
                    ), new Color((byte)(FogTransparency * mCurFogIntensity), 255, 255, 255)
                );
            }
        }
    }

    //Weather
    public void DrawWeather()
    {
        if (Globals.Me == null || Lookup.Get(Globals.Me.MapId) == null)
        {
            return;
        }

        var anim = AnimationBase.Get(WeatherAnimationId);

        if (anim == null || WeatherIntensity == 0)
        {
            return;
        }

        _removeParticles.Clear();

        if ((WeatherXSpeed != 0 || WeatherYSpeed != 0) && Globals.Me.MapInstance == this)
        {
            if (Timing.Global.MillisecondsUtc > _weatherParticleSpawnTime)
            {
                _weatherParticles.Add(new WeatherParticle(_removeParticles, WeatherXSpeed, WeatherYSpeed, anim));
                var spawnTime = 25 + (int)(475 * (1f - WeatherIntensity / 100f));
                spawnTime = (int)(spawnTime *
                                   (480000f /
                                    (Graphics.Renderer.ScreenWidth * Graphics.Renderer.ScreenHeight)));

                _weatherParticleSpawnTime = Timing.Global.MillisecondsUtc + spawnTime;
            }
        }

        //Process and draw each weather particle
        foreach (var w in _weatherParticles)
        {
            w.Update();
        }

        //Remove all old particles from the weather particles list from the removeparticles list.
        foreach (var r in _removeParticles)
        {
            r.Dispose();
            _weatherParticles.Remove(r);
        }
    }

    private void ClearWeather()
    {
        foreach (var r in _weatherParticles)
        {
            r.Dispose();
        }

        _weatherParticles.Clear();
    }

    public void GridSwitched()
    {
        mPanoramaIntensity = 1f;
        mCurFogIntensity = 1f;
    }

    public void DrawPanorama()
    {
        float ecTime = Timing.Global.MillisecondsUtc - mPanoramaUpdateTime;
        mPanoramaUpdateTime = Timing.Global.MillisecondsUtc;
        if (Id == Globals.Me.MapId)
        {
            if (mPanoramaIntensity != 1)
            {
                mPanoramaIntensity += ecTime / 2000f;
                if (mPanoramaIntensity > 1)
                {
                    mPanoramaIntensity = 1;
                }
            }
        }
        else
        {
            if (mPanoramaIntensity != 0)
            {
                mPanoramaIntensity -= ecTime / 2000f;
                if (mPanoramaIntensity < 0)
                {
                    mPanoramaIntensity = 0;
                }
            }
        }

        var imageTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Image, Panorama);
        if (imageTex != null)
        {
            Graphics.DrawFullScreenTexture(imageTex, mPanoramaIntensity);
        }
    }

    public void DrawOverlayGraphic()
    {
        float ecTime = Timing.Global.MillisecondsUtc - mOverlayUpdateTime;
        mOverlayUpdateTime = Timing.Global.MillisecondsUtc;
        if (Id == Globals.Me.MapId)
        {
            if (mOverlayIntensity != 1)
            {
                mOverlayIntensity += ecTime / 2000f;
                if (mOverlayIntensity > 1)
                {
                    mOverlayIntensity = 1;
                }
            }
        }
        else
        {
            if (mOverlayIntensity != 0)
            {
                mOverlayIntensity -= ecTime / 2000f;
                if (mOverlayIntensity < 0)
                {
                    mOverlayIntensity = 0;
                }
            }
        }

        var imageTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Image, OverlayGraphic);
        if (imageTex != null)
        {
            Graphics.DrawFullScreenTexture(imageTex, mOverlayIntensity);
        }
    }

    public void CompareEffects(IMapInstance oldMap)
    {
        // Return if the old map is not a MapInstance.
        if (!(oldMap is MapInstance tempMap))
        {
            return;
        }

        // Check if fog is the same.
        if (tempMap.Fog == Fog)
        {
            // Get fog texture.
            var fogTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Fog, Fog);
            if (fogTex == null)
            {
                return;
            }

            // Copy over fog values.
            mFogUpdateTime = tempMap.mFogUpdateTime;
            var ratio = (float)tempMap.FogTransparency / FogTransparency;
            mCurFogIntensity = ratio * tempMap.mCurFogIntensity;
            mFogCurrentX = tempMap.mFogCurrentX;
            mFogCurrentY = tempMap.mFogCurrentY;

            // Calculate displacement of current map compared to old map.
            float dx = X - oldMap.X;
            float dy = Y - oldMap.Y;

            // Update fog position based on displacement.
            mFogCurrentX += (_tileWidth * _width % fogTex.Width) * -Math.Sign(dx);
            mFogCurrentY += (_tileHeight * _height % fogTex.Height) * -Math.Sign(dy);

            // Reset fog intensity of old map.
            tempMap.mCurFogIntensity = 0;
        }

        // Check if panorama is the same.
        if (tempMap.Panorama == Panorama)
        {
            // Copy over panorama values.
            mPanoramaIntensity = tempMap.mPanoramaIntensity;
            mPanoramaUpdateTime = tempMap.mPanoramaUpdateTime;
            // Reset panorama intensity of old map.
            tempMap.mPanoramaIntensity = 0;
        }

        // Check if overlay graphic is the same.
        if (tempMap.OverlayGraphic == OverlayGraphic)
        {
            // Copy over overlay graphic values.
            mOverlayIntensity = tempMap.mOverlayIntensity;
            mOverlayUpdateTime = tempMap.mOverlayUpdateTime;
            // Reset overlay graphic intensity of old map.
            tempMap.mOverlayIntensity = 0;
        }
    }

    public void DrawActionMsgs()
    {
        for (var n = ActionMessages.Count - 1; n > -1; n--)
        {
            var actionMessage = ActionMessages[n];
            var x = (X + actionMessage.X * _tileWidth + actionMessage.XOffset);
            var y = Y + actionMessage.Y * _tileHeight - _tileHeight * 2 *
                (1000 - (int)(actionMessage.TransmissionTimer - Timing.Global.MillisecondsUtc)) / 1000;
            var textWidth = Graphics.Renderer.MeasureText(actionMessage.Text, Graphics.ActionMsgFont, 1).X;

            Graphics.Renderer.DrawString(
                actionMessage.Text,
                Graphics.ActionMsgFont,
                x - textWidth / 2f,
                y,
                1,
                actionMessage.Color,
                true,
                null,
                new Color(40, 40, 40)
            );

            //Try to remove
            actionMessage.TryRemove();
        }
    }

    //Events
    public void AddEvent(Guid evtId, EventEntityPacket packet)
    {
        if (IsLoaded)
        {
            if (LocalEntities.ContainsKey(evtId))
            {
                LocalEntities[evtId].Load(packet);
            }
            else
            {
                var evt = new Event(evtId, packet);
                LocalEntities.Add(evtId, evt);
                mEvents.Add(evt);
            }
        }
    }

    public static new MapInstance? Get(Guid id)
    {
        return Lookup.Get<MapInstance>(id);
    }

    public static bool TryGet(Guid id, out MapInstance instance)
    {
        instance = MapInstance.Lookup.Get<MapInstance>(id);
        if (instance == null)
        {
            return false;
        }

        return true;
    }

    public override void Delete()
    {
        if (Lookup != null)
        {
            Lookup.Delete(this);
        }
    }

    //Dispose
    public void Dispose(bool prep = true, bool killentities = true)
    {
        IsDisposed = true;

        ApplicationContext.CurrentContext.Logger.LogDebug(
            "Disposing map {Id} ({Name}) @ ({GridX}, {GridY})",
            Id,
            Name,
            GridX,
            GridY
        );

        IsLoaded = false;
        MapLoaded -= HandleMapLoaded;

        foreach (var evt in mEvents)
        {
            evt.Dispose();
        }

        mEvents.Clear();

        if (killentities)
        {
            foreach (var en in Globals.Entities)
            {
                if (en.Value.MapId == Id)
                {
                    Globals.EntitiesToDispose.Add(en.Key);
                }
            }

            foreach (var en in LocalEntities)
            {
                en.Value.Dispose();
            }
        }

        HideActiveAnimations();
        ClearWeather();
        ClearMapAttributes();
        ClearAttributeSounds();
        DestroyVBOs();
        Delete();
    }

    public static bool MapNotRequested(Guid mapId)
    {
        if (!MapRequests.TryGetValue(mapId, out var nextRequest))
        {
            return true;
        }

        return nextRequest < Timing.Global.Milliseconds;
    }

    public static void UpdateMapRequestTime(params Guid[] mapIds)
    {
        foreach (var mapId in mapIds)
        {
            if (mapId == default)
            {
                continue;
            }

            MapRequests[mapId] = Timing.Global.Milliseconds + 2000;
        }
    }
}

﻿using System.Diagnostics;
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
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

using Newtonsoft.Json;
using MapAttribute = Intersect.Enums.MapAttribute;

namespace Intersect.Client.Maps;


public partial class MapInstance : MapBase, IGameObject<Guid, MapInstance>, IMapInstance
{

    //Client Only Values
    public delegate void MapLoadedDelegate(MapInstance map);

    //Map State Variables
    public static Dictionary<Guid, long> MapRequests { get; set; } = new Dictionary<Guid, long>();

    public static MapLoadedDelegate OnMapLoaded { get; set; }

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

    public Dictionary<Guid, Entity> LocalEntities { get; set; } = new Dictionary<Guid, Entity>();

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
    private long mLastUpdateTime;

    protected float mOverlayIntensity;

    //Overlay Image Variables
    protected long mOverlayUpdateTime = -1;

    protected float mPanoramaIntensity;

    //Panorama Variables
    protected long mPanoramaUpdateTime = -1;

    private bool mTexturesFound = false;

    private Dictionary<string, Dictionary<object, GameTileBuffer[]>> mTileBufferDict = new Dictionary<string, Dictionary<object, GameTileBuffer[]>>(); //[Layer][?][?]

    private Dictionary<string, GameTileBuffer[][]> mTileBuffers = new Dictionary<string, GameTileBuffer[][]>(); //[Layer][Autotile Frame][Buffer Index]

    //Initialization
    public MapInstance(Guid id) : base(id)
    {
        mTileBuffers.Clear();
        foreach (var layer in Options.Instance.MapOpts.Layers.All)
        {
            mTileBufferDict.Add(layer, new Dictionary<object, GameTileBuffer[]>());
            mTileBuffers.Add(layer, new GameTileBuffer[3][]); //3 autotile frames per layer
        }
    }

    public bool IsLoaded { get; private set; }

    //Camera Locking Variables
    public bool[] CameraHolds { get; set; } = new bool[4];

    //World Position
    public int GridX { get; set; }

    public int GridY { get; set; }

    //Map Sounds
    public IMapSound BackgroundSound { get; set; }

    public new static MapControllers Lookup => sLookup ?? (sLookup = new MapControllers(MapBase.Lookup));

    //Load
    public void Load(string json)
    {
        LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
        JsonConvert.PopulateObject(
            json, this, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }
        );

        IsLoaded = true;
        Autotiles = new MapAutotiles(this);
        OnMapLoaded -= HandleMapLoaded;
        OnMapLoaded += HandleMapLoaded;
        MapRequests.Remove(Id);
    }

    public void LoadTileData(byte[] packet)
    {
        Layers = JsonConvert.DeserializeObject<Dictionary<string, Tile[,]>>(LZ4.UnPickleString(packet), mJsonSerializerSettings);
        foreach (var layer in Options.Instance.MapOpts.Layers.All)
        {
            if (!Layers.ContainsKey(layer))
            {
                Layers.Add(layer, new Tile[Options.MapWidth, Options.MapHeight]);
            }
        }
    }

    private void CacheTextures()
    {
        if (mTexturesFound == false && GameContentManager.Current.TilesetsLoaded)
        {
            foreach (var layer in Options.Instance.MapOpts.Layers.All)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        var tileset = TilesetBase.Get(Layers[layer][x, y].TilesetId);
                        if (tileset != null)
                        {
                            var tilesetTex = Globals.ContentManager.GetTexture(
                                Framework.Content.TextureType.Tileset, tileset.Name
                            );

                            Layers[layer][x, y].TilesetTex = tilesetTex;
                        }
                    }
                }
            }

            mTexturesFound = true;
        }
    }

    //Updating
    public void Update(bool isLocal)
    {
        if (isLocal)
        {
            mLastUpdateTime = Timing.Global.Milliseconds + 10000;
            UpdateMapAttributes();
            if (BackgroundSound == null && !TextUtils.IsNone(Sound))
            {
                BackgroundSound = Audio.AddMapSound(Sound, -1, -1, Id, true, 0, 10);
            }

            foreach (var anim in LocalAnimations)
            {
                if (anim.Value.Disposed())
                {
                    LocalAnimations.TryRemove(anim.Key, out MapAnimation removed);
                }
                else
                {
                    anim.Value.Update();
                }
            }

            foreach (var en in LocalEntities)
            {
                if (en.Value == null)
                {
                    continue;
                }

                en.Value.Update();
            }

            foreach (var critter in mAttributeCritterInstances)
            {
                if (critter.Value == null)
                {
                    continue;
                }

                critter.Value.Update();
            }

            for (var i = 0; i < LocalEntitiesToDispose.Count; i++)
            {
                LocalEntities.Remove(LocalEntitiesToDispose[i]);
            }

            LocalEntitiesToDispose.Clear();
        }
        else
        {
            if (Timing.Global.Milliseconds > mLastUpdateTime)
            {
                Dispose();
            }

            HideActiveAnimations();
        }
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
        if (map != this && Globals.GridMaps.Contains(map.Id) && Globals.GridMaps.Contains(Id) && IsLoaded)
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
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(0, y, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check Southwest
                    updatedBuffers.UnionWith(CheckAutotile(0, Options.MapHeight - 1, surroundingMaps));
                }
            }
            else if (map.GridX == GridX)
            {
                if (map.GridY == GridY - 1)
                {
                    //Check North
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(x, 0, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check South
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(x, Options.MapHeight - 1, surroundingMaps));
                    }
                }
            }
            else if (map.GridX == GridX + 1)
            {
                if (map.GridY == GridY - 1)
                {
                    //Check Northeast
                    updatedBuffers.UnionWith(
                        CheckAutotile(Options.MapWidth - 1, Options.MapHeight, surroundingMaps)
                    );
                }
                else if (map.GridY == GridY)
                {
                    //Check East
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        updatedBuffers.UnionWith(CheckAutotile(Options.MapWidth - 1, y, surroundingMaps));
                    }
                }
                else if (map.GridY == GridY + 1)
                {
                    //Check Southeast
                    updatedBuffers.UnionWith(
                        CheckAutotile(Options.MapWidth - 1, Options.MapHeight - 1, surroundingMaps)
                    );
                }
            }

            //Along with edges we need to recalculate ALL cliffs :(
            foreach (var layer in Options.Instance.MapOpts.Layers.All)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
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
        var updated = new List<GameTileBuffer>();
        foreach (var layer in Options.Instance.MapOpts.Layers.All)
        {
            if (Autotiles.UpdateAutoTile(x, y, layer, surroundingMaps))
            {
                //Find the VBO, update it.
                var tileBuffer = mTileBufferDict[layer];
                var tile = Layers[layer][x, y];
                if (tile.TilesetTex == null)
                {
                    continue;
                }

                var tilesetTex = (GameTexture)tile.TilesetTex;
                if (tile.X < 0 || tile.Y < 0)
                {
                    continue;
                }

                if (tile.X * Options.TileWidth >= tilesetTex.GetWidth() ||
                    tile.Y * Options.TileHeight >= tilesetTex.GetHeight())
                {
                    continue;
                }

                var platformTex = tilesetTex.GetTexture();
                if (tileBuffer.ContainsKey(platformTex))
                {
                    for (var autotileFrame = 0; autotileFrame < 3; autotileFrame++)
                    {
                        var buffer = tileBuffer[platformTex][autotileFrame];
                        var xoffset = GetX();
                        var yoffset = GetY();
                        DrawAutoTile(
                            layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y,
                            autotileFrame, tilesetTex, buffer, true
                        ); //Top Left

                        DrawAutoTile(
                            layer, x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                            y * Options.TileHeight + yoffset, 2, x, y, autotileFrame, tilesetTex, buffer, true
                        );

                        DrawAutoTile(
                            layer, x * Options.TileWidth + xoffset,
                            y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 3, x, y, autotileFrame,
                            tilesetTex, buffer, true
                        );

                        DrawAutoTile(
                            layer, +x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                            y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 4, x, y, autotileFrame,
                            tilesetTex, buffer, true
                        );

                        if (!updated.Contains(buffer))
                        {
                            updated.Add(buffer);
                        }
                    }
                }
            }
        }

        return updated.ToArray();
    }

    //Helper Functions
    public MapBase[,] GenerateAutotileGrid()
    {
        var mapBase = new MapBase[3, 3];
        if (Globals.MapGrid != null && Globals.GridMaps.Contains(Id))
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

    public float X => GetX();

    //Retreives the X Position of the Left side of the map in world space.
    public float GetX()
    {
        return GridX * Options.MapWidth * Options.TileWidth;
    }

    public float Y => GetY();

    //Retreives the Y Position of the Top side of the map in world space.
    public float GetY()
    {
        return GridY * Options.MapHeight * Options.TileHeight;
    }

    //Attribute References
    private void UpdateMapAttributes()
    {
        var width = Options.MapWidth;
        var height = Options.MapHeight;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var att = Attributes[x, y];
                if (att == null)
                {
                    continue;
                }

                if (att.Type == MapAttribute.Animation)
                {
                    var anim = AnimationBase.Get(((MapAnimationAttribute)att).AnimationId);
                    if (anim == null)
                    {
                        continue;
                    }

                    if (!mAttributeAnimInstances.ContainsKey(att))
                    {
                        var animInstance = new Animation(anim, true);
                        animInstance.SetPosition(
                            GetX() + x * Options.TileWidth + Options.TileWidth / 2,
                            GetY() + y * Options.TileHeight + Options.TileHeight / 2, x, y, Id, 0
                        );

                        mAttributeAnimInstances.Add(att, animInstance);
                    }

                    mAttributeAnimInstances[att].Update();
                }


                if (att.Type == MapAttribute.Critter)
                {
                    var critterAttribute = ((MapCritterAttribute)att);
                    var sprite = critterAttribute.Sprite;
                    var anim = AnimationBase.Get(critterAttribute.AnimationId);
                    if (anim == null && TextUtils.IsNone(sprite))
                    {
                        continue;
                    }

                    if (!mAttributeCritterInstances.ContainsKey(att))
                    {
                        var critter = new Critter(this, (byte)x, (byte)y, critterAttribute);
                        LocalCritters.Add(critter.Id, critter);
                        mAttributeCritterInstances.Add(att, critter);
                    }

                    mAttributeCritterInstances[att].Update();
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
        for (var x = 0; x < Options.MapWidth; ++x)
        {
            for (var y = 0; y < Options.MapHeight; ++y)
            {
                var attribute = Attributes?[x, y];
                if (attribute?.Type != MapAttribute.Sound)
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
    public void AddTileAnimation(Guid animId, int tileX, int tileY, Direction dir = Direction.None, IEntity owner = null)
    {
        var animBase = AnimationBase.Get(animId);
        if (animBase == null)
        {
            return;
        }

        var anim = new MapAnimation(animBase, tileX, tileY, dir, owner as Entity);
        LocalAnimations.TryAdd(anim.Id, anim);
        anim.SetPosition(
            GetX() + tileX * Options.TileWidth + Options.TileWidth / 2,
            GetY() + tileY * Options.TileHeight + Options.TileHeight / 2, tileX, tileY, Id, dir
        );
    }

    private void HideActiveAnimations()
    {
        LocalEntities?.Values.ToList().ForEach(entity => entity?.ClearAnimations(null));
        foreach (var anim in LocalAnimations)
        {
            anim.Value?.Dispose();
        }
        LocalAnimations?.Clear();
        ClearMapAttributes();
    }

    public void BuildVBOs()
    {
        foreach (var layer in Options.Instance.MapOpts.Layers.All)
        {
            mTileBuffers[layer] = DrawMapLayer(layer, GetX(), GetY());
            for (var y = 0; y < 3; y++)
            {
                for (var z = 0; z < mTileBuffers[layer][y].Length; z++)
                {
                    mTileBuffers[layer][y][z].SetData();
                }
            }
        }
    }

    public void DestroyVBOs()
    {
        foreach (var layer in Options.Instance.MapOpts.Layers.All)
        {
            for (var y = 0; y < 3; y++)
            {
                if (mTileBuffers[layer][y] != null)
                {
                    for (var z = 0; z < mTileBuffers[layer][y].Length; z++)
                    {
                        mTileBuffers[layer][y][z].Dispose();
                    }
                }
            }
            mTileBufferDict[layer]?.Clear();
        }
        mTileBuffers = null;
    }

    //Rendering/Drawing Code
    public void Draw(int layer = 0) //Lower, Middle, Upper
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

        if (mTileBuffers[Options.Instance.MapOpts.Layers.All[0]][0] == null)
        {
            BuildVBOs();
        }

        var drawLayers = Options.Instance.MapOpts.Layers.LowerLayers;

        if (layer == 1)
        {
            drawLayers = Options.Instance.MapOpts.Layers.MiddleLayers;
        }

        if (layer == 2)
        {
            drawLayers = Options.Instance.MapOpts.Layers.UpperLayers;
        }

        foreach (var drawLayer in drawLayers)
        {
            if (mTileBuffers[drawLayer][Globals.AnimFrame] != null)
            {
                for (var i = 0; i < mTileBuffers[drawLayer][Globals.AnimFrame].Length; i++)
                {
                    Graphics.Renderer.DrawTileBuffer(mTileBuffers[drawLayer][Globals.AnimFrame][i]);
                }
            }
        }
    }

    public void DrawItemsAndLights()
    {
        // Calculate tile and map item dimensions.
        var tileWidth = Options.TileWidth;
        var tileHeight = Options.TileHeight;
        var mapItemWidth = Options.Instance.MapOpts.MapItemWidth;
        var mapItemHeight = Options.Instance.MapOpts.MapItemHeight;

        // Draw map items.
        foreach (var (key, tileItems) in MapItems)
        {
            // Calculate tile coordinates.
            var tileX = key % Options.MapWidth;
            var tileY = (int)Math.Floor(key / (float)Options.MapWidth);

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

                var x = X + tileX * tileWidth;
                var y = Y + tileY * tileHeight;
                var centerX = x + (tileWidth / 2);
                var centerY = y + (tileHeight / 2);
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
            var x = GetX() + (light.TileX * tileWidth + light.OffsetX) + tileWidth / 2f;
            var y = GetY() + (light.TileY * tileHeight + light.OffsetY) + tileHeight / 2f;
            Graphics.AddLight((int)x, (int)y, (int)w, light.Intensity, light.Expand, light.Color);
        }
    }

    /// <summary>
    /// Draws all names of the items on the tile the user is hovering over.
    /// </summary>
    public void DrawItemNames()
    {
        if (Interface.Interface.MouseHitGui())
        {
            return;
        }
        // Get where our mouse is located and convert it to a tile based location.
        var mousePos = Graphics.ConvertToWorldPoint(
                Globals.InputManager.GetMousePosition()
        );
        var x = (int)(mousePos.X - (int)GetX()) / Options.TileWidth;
        var y = (int)(mousePos.Y - (int)GetY()) / Options.TileHeight;
        var mapId = Id;

        // Is this an actual location on this map?
        if (Globals.Me.TryGetRealLocation(ref x, ref y, ref mapId) && mapId == Id)
        {
            // Apparently it is! Do we have any items to render here?
            var tileItems = new List<IMapItemInstance>();
            if (MapItems.TryGetValue(y * Options.MapWidth + x, out tileItems))
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
                        name = Localization.Strings.General.MapItemStackable.ToString(name, Strings.FormatQuantityAbbreviated(quantity));
                    }
                    var color = CustomColors.Items.MapRarities.ContainsKey(rarity)
                        ? CustomColors.Items.MapRarities[rarity]
                        : new LabelColor(Color.White, Color.Black, new Color(100, 0, 0, 0));
                    var textSize = Graphics.Renderer.MeasureText(name, Graphics.EntityNameFont, 1);
                    var offsetY = (baseOffset * textSize.Y);
                    var destX = GetX() + (int)Math.Ceiling(((x * Options.TileWidth) + (Options.TileWidth / 2)) - (textSize.X / 2));
                    var destY = GetY() + (int)Math.Ceiling(((y * Options.TileHeight) - ((Options.TileHeight / 3) + textSize.Y))) - offsetY;

                    // Do we need to draw a background?
                    if (color.Background != Color.Transparent)
                    {
                        Graphics.DrawGameTexture(
                            Graphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
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

    private void DrawAutoTile(
        string layerName,
        float destX,
        float destY,
        int quarterNum,
        int x,
        int y,
        int forceFrame,
        GameTexture tileset,
        GameTileBuffer buffer,
        bool update = false
    )
    {
        int yOffset = 0, xOffset = 0;

        // calculate the offset
        switch (Layers[layerName][x, y].Autotile)
        {
            case MapAutotiles.AUTOTILE_WATERFALL:
                yOffset = (forceFrame - 1) * Options.TileHeight;

                break;

            case MapAutotiles.AUTOTILE_ANIM:
                xOffset = forceFrame * Options.TileWidth * 2;

                break;

            case MapAutotiles.AUTOTILE_ANIM_XP:
                xOffset = forceFrame * Options.TileWidth * 3;

                break;

            case MapAutotiles.AUTOTILE_CLIFF:
                yOffset = -Options.TileHeight;

                break;
        }

        if (update)
        {
            if (!buffer.UpdateTile(
                tileset, destX, destY,
                Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].X + xOffset,
                Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].Y + yOffset,
                Options.TileWidth / 2, Options.TileHeight / 2
            ))
            {
                throw new Exception("Failed to update tile to VBO!");
            }
        }
        else
        {
            if (!buffer.AddTile(
                tileset, destX, destY,
                Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].X + xOffset,
                Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].Y + yOffset,
                Options.TileWidth / 2, Options.TileHeight / 2
            ))
            {
                throw new Exception("Failed to add tile to VBO!");
            }
        }
    }

    private GameTileBuffer[][] DrawMapLayer(string layerName, float xoffset = 0, float yoffset = 0)
    {
        var tileBuffers = new Dictionary<object, GameTileBuffer[]>();

        if (!Layers.ContainsKey(layerName))
        {
            return null;
        }

        for (var x = 0; x < Options.MapWidth; x++)
        {
            for (var y = 0; y < Options.MapHeight; y++)
            {
                var tile = Layers[layerName][x, y];
                if (tile.TilesetTex == null)
                {
                    continue;
                }

                var tilesetTex = (GameTexture)tile.TilesetTex;

                if (tile.X < 0 || tile.Y < 0)
                {
                    continue;
                }

                if (tile.X * Options.TileWidth >= tilesetTex.GetWidth() ||
                    tile.Y * Options.TileHeight >= tilesetTex.GetHeight())
                {
                    continue;
                }

                var platformTex = tilesetTex.GetTexture();

                GameTileBuffer[] buffers = null;
                if (tileBuffers.ContainsKey(platformTex))
                {
                    buffers = tileBuffers[platformTex];
                }
                else
                {
                    buffers = new GameTileBuffer[3];
                    for (var i = 0; i < 3; i++)
                    {
                        buffers[i] = Graphics.Renderer.CreateTileBuffer();
                    }

                    tileBuffers.Add(platformTex, buffers);
                }

                switch (Autotiles.Layers[layerName][x, y].RenderState)
                {
                    case MapAutotiles.RENDER_STATE_NORMAL:
                        for (var i = 0; i < 3; i++)
                        {
                            var buffer = buffers[i];
                            if (!buffer.AddTile(
                                tilesetTex, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset,
                                Layers[layerName][x, y].X * Options.TileWidth,
                                Layers[layerName][x, y].Y * Options.TileHeight, Options.TileWidth,
                                Options.TileHeight
                            ))
                            {
                                throw new Exception("Failed to add VBO!");
                            }
                        }

                        break;
                    case MapAutotiles.RENDER_STATE_AUTOTILE:
                        for (var i = 0; i < 3; i++)
                        {
                            DrawAutoTile(
                                layerName, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y,
                                i, tilesetTex, buffers[i]
                            );

                            DrawAutoTile(
                                layerName, x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                                y * Options.TileHeight + yoffset, 2, x, y, i, tilesetTex, buffers[i]
                            );

                            DrawAutoTile(
                                layerName, x * Options.TileWidth + xoffset,
                                y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 3, x, y, i, tilesetTex,
                                buffers[i]
                            );

                            DrawAutoTile(
                                layerName, +x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                                y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 4, x, y, i, tilesetTex,
                                buffers[i]
                            );
                        }

                        break;
                }
            }
        }

        var outputBuffers = new GameTileBuffer[3][];
        for (var i = 0; i < 3; i++)
        {
            outputBuffers[i] = new GameTileBuffer[tileBuffers.Count];
        }

        var valueArrays = tileBuffers.Values.ToArray();
        for (var x = 0; x < valueArrays.Length; x++)
        {
            for (var i = 0; i < 3; i++)
            {
                outputBuffers[i][x] = valueArrays[x][i];
            }
        }

        mTileBufferDict[layerName] = tileBuffers;

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
        var xCount = Options.MapWidth * Options.TileWidth * 3 / fogTex.Width;
        var yCount = Options.MapHeight * Options.TileHeight * 3 / fogTex.Height;

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
                        X - Options.MapWidth * Options.TileWidth * 1f + x * fogTex.Width + drawX,
                        Y - Options.MapHeight * Options.TileHeight * 1f + y * fogTex.Height + drawY,
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
                                    (Graphics.Renderer.GetScreenWidth() * Graphics.Renderer.GetScreenHeight())));

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
            mFogCurrentX += (Options.TileWidth * Options.MapWidth % fogTex.Width) * -Math.Sign(dx);
            mFogCurrentY += (Options.TileHeight * Options.MapHeight % fogTex.Height) * -Math.Sign(dy);

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
            var y = (int)Math.Ceiling(
                GetY() +
                ActionMessages[n].Y * Options.TileHeight -
                Options.TileHeight *
                2 *
                (1000 - (ActionMessages[n].TransmissionTimer - Timing.Global.MillisecondsUtc)) /
                1000
            );

            var x = (int)Math.Ceiling(GetX() + ActionMessages[n].X * Options.TileWidth + ActionMessages[n].XOffset);
            var textWidth = Graphics.Renderer.MeasureText(ActionMessages[n].Msg, Graphics.ActionMsgFont, 1).X;
            Graphics.Renderer.DrawString(
                ActionMessages[n].Msg, Graphics.ActionMsgFont, x - textWidth / 2f, y, 1, ActionMessages[n].Color,
                true, null, new Color(40, 40, 40)
            );

            //Try to remove
            ActionMessages[n].TryRemove();
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

    public new static MapInstance Get(Guid id)
    {
        return MapInstance.Lookup.Get<MapInstance>(id);
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
        IsLoaded = false;
        OnMapLoaded -= HandleMapLoaded;

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

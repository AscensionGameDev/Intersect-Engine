﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Core.Sounds;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Client.Maps
{

    public class MapInstance : MapBase, IGameObject<Guid, MapInstance>
    {

        //Client Only Values
        public delegate void MapLoadedDelegate(MapInstance map);

        //Map State Variables
        public static Dictionary<Guid, long> MapRequests = new Dictionary<Guid, long>();

        public static MapLoadedDelegate OnMapLoaded;

        private static MapInstances sLookup;

        public List<WeatherParticle> _removeParticles = new List<WeatherParticle>();

        //Weather
        public List<WeatherParticle> _weatherParticles = new List<WeatherParticle>();

        private long _weatherParticleSpawnTime;

        //Action Msg's
        public List<ActionMessage> ActionMsgs = new List<ActionMessage>();

        public List<MapSound> AttributeSounds = new List<MapSound>();

        //Map Animations
        public List<MapAnimation> LocalAnimations = new List<MapAnimation>();

        public Dictionary<Guid, Entity> LocalEntities = new Dictionary<Guid, Entity>();

        //Map Players/Events/Npcs
        public List<Guid> LocalEntitiesToDispose = new List<Guid>();

        //Map Items
        public Dictionary<int, MapItemInstance> MapItems = new Dictionary<int, MapItemInstance>();

        //Map Attributes
        private Dictionary<Intersect.GameObjects.Maps.MapAttribute, Animation> mAttributeAnimInstances =
            new Dictionary<Intersect.GameObjects.Maps.MapAttribute, Animation>();

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

        private Dictionary<object, GameTileBuffer[]>[] mTileBufferDict =
            new Dictionary<object, GameTileBuffer[]>[Options.LayerCount];

        private GameTileBuffer[][][] mTileBuffers; //Array is layer, autotile frame, buffer index

        //Initialization
        public MapInstance(Guid id) : base(id)
        {
            mTileBuffers = new GameTileBuffer[Options.LayerCount][][];
            for (var i = 0; i < Options.LayerCount; i++)
            {
                mTileBuffers[i] = new GameTileBuffer[3][]; //3 autotile frames
            }
        }

        public bool MapLoaded { get; private set; }

        //Camera Locking Variables
        public bool[] CameraHolds { get; set; } = new bool[4];

        //World Position
        public int MapGridX { get; set; }

        public int MapGridY { get; set; }

        //Map Sounds
        public MapSound BackgroundSound { get; set; }

        [NotNull]
        public new static MapInstances Lookup => sLookup ?? (sLookup = new MapInstances(MapBase.Lookup));

        //Load
        public void Load(string json)
        {
            LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
            JsonConvert.PopulateObject(
                json, this, new JsonSerializerSettings {ObjectCreationHandling = ObjectCreationHandling.Replace}
            );

            MapLoaded = true;
            Autotiles = new MapAutotiles(this);
            OnMapLoaded -= HandleMapLoaded;
            OnMapLoaded += HandleMapLoaded;
            if (MapRequests.ContainsKey(Id))
            {
                MapRequests.Remove(Id);
            }
        }

        public void LoadTileData(byte[] packet)
        {
            Layers = mCeras.Decompress<TileArray[]>(packet);
        }

        private void CacheTextures()
        {
            if (mTexturesFound == false && GameContentManager.Current.TilesetsLoaded)
            {
                for (var i = 0; i < Options.LayerCount; i++)
                {
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            var tileset = TilesetBase.Get(Layers[i].Tiles[x, y].TilesetId);
                            if (tileset != null)
                            {
                                var tilesetTex = Globals.ContentManager.GetTexture(
                                    GameContentManager.TextureType.Tileset, tileset.Name
                                );

                                Layers[i].Tiles[x, y].TilesetTex = tilesetTex;
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
                mLastUpdateTime = Globals.System.GetTimeMs() + 10000;
                UpdateMapAttributes();
                if (BackgroundSound == null && !TextUtils.IsNone(Sound))
                {
                    BackgroundSound = Audio.AddMapSound(Sound, -1, -1, Id, true, 10);
                }

                foreach (var anim in LocalAnimations)
                {
                    anim.Update();
                }

                foreach (var en in LocalEntities)
                {
                    if (en.Value == null)
                    {
                        continue;
                    }

                    en.Value.Update();
                }

                for (var i = 0; i < LocalEntitiesToDispose.Count; i++)
                {
                    LocalEntities.Remove(LocalEntitiesToDispose[i]);
                }

                LocalEntitiesToDispose.Clear();
            }
            else
            {
                if (Globals.System.GetTimeMs() > mLastUpdateTime)
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

            var gridX = myMap.MapGridX;
            var gridY = myMap.MapGridY;
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
            if (map != this && Globals.GridMaps.Contains(map.Id) && Globals.GridMaps.Contains(Id) && MapLoaded)
            {
                var surroundingMaps = GenerateAutotileGrid();
                if (map.MapGridX == MapGridX - 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northwest
                        updatedBuffers.UnionWith(CheckAutotile(0, 0, surroundingMaps));
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check West
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            updatedBuffers.UnionWith(CheckAutotile(0, y, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southwest
                        updatedBuffers.UnionWith(CheckAutotile(0, Options.MapHeight - 1, surroundingMaps));
                    }
                }
                else if (map.MapGridX == MapGridX)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check North
                        for (var x = 0; x < Options.MapWidth; x++)
                        {
                            updatedBuffers.UnionWith(CheckAutotile(x, 0, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check South
                        for (var x = 0; x < Options.MapWidth; x++)
                        {
                            updatedBuffers.UnionWith(CheckAutotile(x, Options.MapHeight - 1, surroundingMaps));
                        }
                    }
                }
                else if (map.MapGridX == MapGridX + 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northeast
                        updatedBuffers.UnionWith(
                            CheckAutotile(Options.MapWidth - 1, Options.MapHeight, surroundingMaps)
                        );
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check East
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            updatedBuffers.UnionWith(CheckAutotile(Options.MapWidth - 1, y, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southeast
                        updatedBuffers.UnionWith(
                            CheckAutotile(Options.MapWidth - 1, Options.MapHeight - 1, surroundingMaps)
                        );
                    }
                }

                //Along with edges we need to recalculate ALL cliffs :(
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        for (var i = 0; i < Options.LayerCount; i++)
                        {
                            if (Layers[i].Tiles[x, y].Autotile == MapAutotiles.AUTOTILE_CLIFF)
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
            for (var layer = 0; layer < 5; layer++)
            {
                if (Autotiles.UpdateAutoTile(x, y, layer, surroundingMaps))
                {
                    //Find the VBO, update it.
                    var tileBuffer = mTileBufferDict[layer];
                    var tile = Layers[layer].Tiles[x, y];
                    if (tile.TilesetTex == null)
                    {
                        continue;
                    }

                    var tilesetTex = (GameTexture) tile.TilesetTex;
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
                        var x1 = MapGridX + x;
                        var y1 = MapGridY + y;
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

        //Retreives the X Position of the Left side of the map in world space.
        public float GetX()
        {
            return MapGridX * Options.MapWidth * Options.TileWidth;
        }

        //Retreives the Y Position of the Top side of the map in world space.
        public float GetY()
        {
            return MapGridY * Options.MapHeight * Options.TileHeight;
        }

        //Attribute References
        private void UpdateMapAttributes()
        {
            var width = Options.MapWidth;
            var height = Options.MapHeight;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var att = Attributes[x, y];
                    if (att == null)
                    {
                        continue;
                    }

                    if (att.Type != MapAttributes.Animation)
                    {
                        continue;
                    }

                    var anim = AnimationBase.Get(((MapAnimationAttribute) att).AnimationId);
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
            }
        }

        private void ClearMapAttributes()
        {
            foreach (var attributeInstance in mAttributeAnimInstances)
            {
                attributeInstance.Value.Dispose();
            }

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
                    if (attribute?.Type != MapAttributes.Sound)
                    {
                        continue;
                    }

                    if (TextUtils.IsNone(((MapSoundAttribute) attribute).File))
                    {
                        continue;
                    }

                    var sound = Audio.AddMapSound(
                        ((MapSoundAttribute) attribute).File, x, y, Id, true, ((MapSoundAttribute) attribute).Distance
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
        public void AddTileAnimation(Guid animId, int tileX, int tileY, int dir = -1)
        {
            var animBase = AnimationBase.Get(animId);
            if (animBase == null)
            {
                return;
            }

            var anim = new MapAnimation(animBase, tileX, tileY, dir);
            LocalAnimations.Add(anim);
            anim.SetPosition(
                GetX() + tileX * Options.TileWidth + Options.TileWidth / 2,
                GetY() + tileY * Options.TileHeight + Options.TileHeight / 2, tileX, tileY, Id, dir
            );
        }

        private void HideActiveAnimations()
        {
            LocalEntities?.Values.ToList().ForEach(entity => entity?.ClearAnimations(null));
            LocalAnimations?.ForEach(animation => animation?.Dispose());
            LocalAnimations?.Clear();
            ClearMapAttributes();
        }

        public void BuildVBOs()
        {
            for (var i = 0; i < Options.LayerCount; i++)
            {
                mTileBuffers[i] = DrawMapLayer(i, GetX(), GetY());
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < mTileBuffers[i][y].Length; z++)
                    {
                        mTileBuffers[i][y][z].SetData();
                    }
                }
            }
        }

        public void DestroyVBOs()
        {
            for (var i = 0; i < Options.LayerCount; i++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (mTileBuffers[i][y] != null)
                    {
                        for (var z = 0; z < mTileBuffers[i][y].Length; z++)
                        {
                            mTileBuffers[i][y][z].Dispose();
                        }
                    }
                }
                mTileBufferDict[i]?.Clear();
            }

            mTileBuffers = null;
        }

        //Rendering/Drawing Code
        public void Draw(int layer = 0) //Lower, Middle, Upper
        {
            if (!MapLoaded)
            {
                return;
            }

            CacheTextures();
            if (!mTexturesFound)
            {
                return;
            }

            if (mTileBuffers[0][0] == null)
            {
                BuildVBOs();
            }

            var drawLayerStart = 0;
            var drawLayerEnd = 2;

            if (layer == 1)
            {
                drawLayerStart = 3;
                drawLayerEnd = 3;
            }

            if (layer == 2)
            {
                drawLayerStart = 4;
                drawLayerEnd = 4;
            }

            for (var x = drawLayerStart; x <= drawLayerEnd; x++)
            {
                if (mTileBuffers[x][Globals.AnimFrame] != null)
                {
                    for (var i = 0; i < mTileBuffers[x][Globals.AnimFrame].Length; i++)
                    {
                        Graphics.Renderer.DrawTileBuffer(mTileBuffers[x][Globals.AnimFrame][i]);
                    }
                }
            }
        }

        public void DrawItemsAndLights()
        {
            //Draw Map Items
            foreach (var item in MapItems)
            {
                var itemBase = ItemBase.Get(item.Value.ItemId);
                if (itemBase != null)
                {
                    var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, itemBase.Icon);
                    if (itemTex != null)
                    {
                        if (item.Value.hasFallen > 0)
                        {
                            Graphics.DrawGameTexture(
                                itemTex, new FloatRect(0, 0, itemTex.GetWidth(), itemTex.GetHeight()),
                                new FloatRect(
                                    GetX() + item.Value.X * Options.TileWidth, (GetY() + item.Value.Y * Options.TileHeight) - (item.Value.hasFallen * Options.TileHeight),
                                    Options.TileWidth, Options.TileHeight
                                ), Color.White
                            );
                            item.Value.hasFallen -= 0.2f;
                        }
                        else
                        {
                            Graphics.DrawGameTexture(
                                itemTex, new FloatRect(0, 0, itemTex.GetWidth(), itemTex.GetHeight()),
                                new FloatRect(
                                    GetX() + item.Value.X * Options.TileWidth, GetY() + item.Value.Y * (Options.TileHeight),
                                    Options.TileWidth, Options.TileHeight
                                ), Color.White
                            );
                        }
                    }
                }
            }
            //Add lights to our darkness texture
            foreach (var light in Lights)
            {
                double w = light.Size;
                var x = GetX() + (light.TileX * Options.TileWidth + light.OffsetX) + Options.TileWidth / 2f;
                var y = GetY() + (light.TileY * Options.TileHeight + light.OffsetY) + Options.TileHeight / 2f;
                Graphics.AddLight((int) x, (int) y, (int) w, light.Intensity, light.Expand, light.Color);
            }
        }

        private void DrawAutoTile(
            int layerNum,
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
            switch (Layers[layerNum].Tiles[x, y].Autotile)
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
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
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
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                    Options.TileWidth / 2, Options.TileHeight / 2
                ))
                {
                    throw new Exception("Failed to add tile to VBO!");
                }
            }
        }

        private GameTileBuffer[][] DrawMapLayer(int layer, float xoffset = 0, float yoffset = 0)
        {
            var tileBuffers = new Dictionary<object, GameTileBuffer[]>();

            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    var tile = Layers[layer].Tiles[x, y];
                    if (tile.TilesetTex == null)
                    {
                        continue;
                    }

                    var tilesetTex = (GameTexture) tile.TilesetTex;

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

                    switch (Autotiles.Autotile[x, y].Layer[layer].RenderState)
                    {
                        case MapAutotiles.RENDER_STATE_NORMAL:
                            for (var i = 0; i < 3; i++)
                            {
                                var buffer = buffers[i];
                                if (!buffer.AddTile(
                                    tilesetTex, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset,
                                    Layers[layer].Tiles[x, y].X * Options.TileWidth,
                                    Layers[layer].Tiles[x, y].Y * Options.TileHeight, Options.TileWidth,
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
                                    layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y,
                                    i, tilesetTex, buffers[i]
                                );

                                DrawAutoTile(
                                    layer, x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                                    y * Options.TileHeight + yoffset, 2, x, y, i, tilesetTex, buffers[i]
                                );

                                DrawAutoTile(
                                    layer, x * Options.TileWidth + xoffset,
                                    y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 3, x, y, i, tilesetTex,
                                    buffers[i]
                                );

                                DrawAutoTile(
                                    layer, +x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
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

            mTileBufferDict[layer] = tileBuffers;

            return outputBuffers;
        }

        //Fogs/Panorama/Overlay
        public void DrawFog()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null)
            {
                return;
            }

            float ecTime = Globals.System.GetTimeMs() - mFogUpdateTime;
            mFogUpdateTime = Globals.System.GetTimeMs();
            if (Id == Globals.Me.CurrentMap)
            {
                if (mCurFogIntensity != 1)
                {
                    if (mCurFogIntensity < 1)
                    {
                        mCurFogIntensity += ecTime / 2000f;
                        if (mCurFogIntensity > 1)
                        {
                            mCurFogIntensity = 1;
                        }
                    }
                    else
                    {
                        mCurFogIntensity -= ecTime / 2000f;
                        if (mCurFogIntensity < 1)
                        {
                            mCurFogIntensity = 1;
                        }
                    }
                }
            }
            else
            {
                if (mCurFogIntensity != 0)
                {
                    mCurFogIntensity -= ecTime / 2000f;
                    if (mCurFogIntensity < 0)
                    {
                        mCurFogIntensity = 0;
                    }
                }
            }

            if (Fog != null && Fog.Length > 0)
            {
                var fogTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Fog, Fog);
                if (fogTex != null)
                {
                    var xCount = (int) (Options.MapWidth * Options.TileWidth * 3 / fogTex.GetWidth());
                    var yCount = (int) (Options.MapHeight * Options.TileHeight * 3 / fogTex.GetHeight());

                    mFogCurrentX -= ecTime / 1000f * FogXSpeed * -6;
                    mFogCurrentY += ecTime / 1000f * FogYSpeed * 2;
                    float deltaX = 0;
                    mFogCurrentX -= deltaX;
                    float deltaY = 0;
                    mFogCurrentY -= deltaY;

                    if (mFogCurrentX < fogTex.GetWidth())
                    {
                        mFogCurrentX += fogTex.GetWidth();
                    }

                    if (mFogCurrentX > fogTex.GetWidth())
                    {
                        mFogCurrentX -= fogTex.GetWidth();
                    }

                    if (mFogCurrentY < fogTex.GetHeight())
                    {
                        mFogCurrentY += fogTex.GetHeight();
                    }

                    if (mFogCurrentY > fogTex.GetHeight())
                    {
                        mFogCurrentY -= fogTex.GetHeight();
                    }

                    var drawX = (float) Math.Round(mFogCurrentX);
                    var drawY = (float) Math.Round(mFogCurrentY);

                    for (var x = -1; x < xCount; x++)
                    {
                        for (var y = -1; y < yCount; y++)
                        {
                            var fogW = fogTex.GetWidth();
                            var fogH = fogTex.GetHeight();
                            Graphics.DrawGameTexture(
                                fogTex, new FloatRect(0, 0, fogW, fogH),
                                new FloatRect(
                                    GetX() - Options.MapWidth * Options.TileWidth * 1f + x * fogW + drawX,
                                    GetY() - Options.MapHeight * Options.TileHeight * 1f + y * fogH + drawY, fogW, fogH
                                ), new Intersect.Color((byte) (FogTransparency * mCurFogIntensity), 255, 255, 255),
                                null, GameBlendModes.None
                            );
                        }
                    }
                }
            }
        }

        //Weather
        public void DrawWeather()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null)
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
                if (Globals.System.GetTimeMs() > _weatherParticleSpawnTime)
                {
                    _weatherParticles.Add(new WeatherParticle(_removeParticles, WeatherXSpeed, WeatherYSpeed, anim));
                    var spawnTime = 25 + (int) (475 * (float) (1f - (float) (WeatherIntensity / 100f)));
                    spawnTime = (int) (spawnTime *
                                       (480000f /
                                        (Graphics.Renderer.GetScreenWidth() * Graphics.Renderer.GetScreenHeight())));

                    _weatherParticleSpawnTime = Globals.System.GetTimeMs() + spawnTime;
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
            float ecTime = Globals.System.GetTimeMs() - mPanoramaUpdateTime;
            mPanoramaUpdateTime = Globals.System.GetTimeMs();
            if (Id == Globals.Me.CurrentMap)
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

            var imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, Panorama);
            if (imageTex != null)
            {
                Graphics.DrawFullScreenTexture(imageTex, mPanoramaIntensity);
            }
        }

        public void DrawOverlayGraphic()
        {
            float ecTime = Globals.System.GetTimeMs() - mOverlayUpdateTime;
            mOverlayUpdateTime = Globals.System.GetTimeMs();
            if (Id == Globals.Me.CurrentMap)
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

            var imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, OverlayGraphic);
            if (imageTex != null)
            {
                Graphics.DrawFullScreenTexture(imageTex, mOverlayIntensity);
            }
        }

        public void CompareEffects(MapInstance oldMap)
        {
            //Check if fogs the same
            if (oldMap.Fog == Fog)
            {
                var fogTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Fog, Fog);
                if (fogTex != null)
                {
                    //Copy over fog values
                    mFogUpdateTime = oldMap.mFogUpdateTime;
                    var ratio = (float) oldMap.FogTransparency / FogTransparency;
                    mCurFogIntensity = ratio * oldMap.mCurFogIntensity;
                    mFogCurrentX = oldMap.mFogCurrentX;
                    mFogCurrentY = oldMap.mFogCurrentY;
                    if (GetX() > oldMap.GetX())
                    {
                        mFogCurrentX -= Options.TileWidth * Options.MapWidth % fogTex.GetWidth();
                    }
                    else if (GetX() < oldMap.GetX())
                    {
                        mFogCurrentX += Options.TileWidth * Options.MapWidth % fogTex.GetWidth();
                    }

                    if (GetY() > oldMap.GetY())
                    {
                        mFogCurrentY -= Options.TileHeight * Options.MapHeight % fogTex.GetHeight();
                    }
                    else if (GetY() < oldMap.GetY())
                    {
                        mFogCurrentY += Options.TileHeight * Options.MapHeight % fogTex.GetHeight();
                    }

                    oldMap.mCurFogIntensity = 0;
                }
            }

            if (oldMap.Panorama == Panorama)
            {
                mPanoramaIntensity = oldMap.mPanoramaIntensity;
                mPanoramaUpdateTime = oldMap.mPanoramaUpdateTime;
                oldMap.mPanoramaIntensity = 0;
            }

            if (oldMap.OverlayGraphic == OverlayGraphic)
            {
                mOverlayIntensity = oldMap.mOverlayIntensity;
                mOverlayUpdateTime = oldMap.mOverlayUpdateTime;
                oldMap.mOverlayIntensity = 0;
            }
        }

        public void DrawActionMsgs()
        {
            for (var n = ActionMsgs.Count - 1; n > -1; n--)
            {
                var y = (int) Math.Ceiling(
                    GetY() +
                    ActionMsgs[n].Y * Options.TileHeight -
                    Options.TileHeight *
                    2 *
                    (1000 - (ActionMsgs[n].TransmittionTimer - Globals.System.GetTimeMs())) /
                    1000
                );

                var x = (int) Math.Ceiling(GetX() + ActionMsgs[n].X * Options.TileWidth + ActionMsgs[n].XOffset);
                var textWidth = Graphics.Renderer.MeasureText(ActionMsgs[n].Msg, Graphics.ActionMsgFont, 1).X;
                Graphics.Renderer.DrawString(
                    ActionMsgs[n].Msg, Graphics.ActionMsgFont, (int) x - textWidth / 2f, (int) y, 1, ActionMsgs[n].Clr,
                    true, null, new Color(40, 40, 40)
                );

                //Try to remove
                ActionMsgs[n].TryRemove();
            }
        }

        //Events
        public void AddEvent(Guid evtId, EventEntityPacket packet)
        {
            if (MapLoaded)
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
            MapLoaded = false;
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
                    if (en.Value.CurrentMap == Id)
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

    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.UI.Game.Chat;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Client.Maps
{
    public class MapInstance : MapBase, IGameObject<Guid, MapInstance>
    {
        //Client Only Values
        public delegate void MapLoadedDelegate(MapInstance map);

        private static MapInstances sLookup;
        public static MapLoadedDelegate OnMapLoaded;

        //Map State Variables
        public static Dictionary<Guid, long> MapRequests = new Dictionary<Guid, long>();

        //Map Attributes
        private Dictionary<Intersect.GameObjects.Maps.MapAttribute, AnimationInstance> mAttributeAnimInstances =
            new Dictionary<Intersect.GameObjects.Maps.MapAttribute, AnimationInstance>();

        protected float mCurFogIntensity;
        protected float mFogCurrentX;
        protected float mFogCurrentY;

        //Fog Variables
        protected long mFogUpdateTime = -1;

        //Weather
        public List<WeatherParticle> _weatherParticles = new List<WeatherParticle>();
        public List<WeatherParticle> _removeParticles = new List<WeatherParticle>();
        private long _weatherParticleSpawnTime;

        //Update Timer
        private long mLastUpdateTime;

        protected float mOverlayIntensity;

        //Overlay Image Variables
        protected long mOverlayUpdateTime = -1;

        protected float mPanoramaIntensity;

        //Panorama Variables
        protected long mPanoramaUpdateTime = -1;

        //Action Msg's
        public List<ActionMsgInstance> ActionMsgs = new List<ActionMsgInstance>();

        public List<MapSound> AttributeSounds = new List<MapSound>();

        //Map Animations
        public List<MapAnimationInstance> LocalAnimations = new List<MapAnimationInstance>();

        public Dictionary<Guid, Entity> LocalEntities = new Dictionary<Guid, Entity>();

        //Map Players/Events/Npcs
        public List<Guid> LocalEntitiesToDispose = new List<Guid>();

        //Map Items
        public Dictionary<int, MapItemInstance> MapItems = new Dictionary<int, MapItemInstance>();

        private List<Event> mEvents = new List<Event>();

        public bool MapLoaded { get; private set; }

        //Camera Locking Variables
        public int HoldUp { get; set; }
        public int HoldDown { get; set; }
        public int HoldLeft { get; set; }
        public int HoldRight { get; set; }

        //World Position
        public int MapGridX { get; set; }
        public int MapGridY { get; set; }

        //Map Sounds
        public MapSound BackgroundSound { get; set; }

        private bool mTexturesFound = false;
        private GameTileBuffer[][][] mTileBuffers; //Array is layer, autotile frame, buffer index
        private Dictionary<object,GameTileBuffer[]>[] mTileBufferDict = new Dictionary<object, GameTileBuffer[]>[Options.LayerCount];


        //Initialization
        public MapInstance(Guid id) : base(id)
        {
            mTileBuffers = new GameTileBuffer[Options.LayerCount][][];
            for (int i = 0; i < Options.LayerCount; i++)
                mTileBuffers[i] = new GameTileBuffer[3][];  //3 autotile frames
        }

        [NotNull]
        public new static MapInstances Lookup => sLookup ?? (sLookup = new MapInstances(MapBase.Lookup));

        //Load
        public void Load(string json)
        {
            LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
            JsonConvert.PopulateObject(json, this, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            MapLoaded = true;
            Autotiles = new MapAutotiles(this);
            OnMapLoaded += HandleMapLoaded;
            if (MapRequests.ContainsKey(Id)) MapRequests.Remove(Id);
        }

        public void LoadTileData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(Compression.DecompressPacket(packet));
            Layers = new TileArray[Options.LayerCount];
            for (var i = 0; i < Options.LayerCount; i++)
            {
                Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetId = bf.ReadGuid();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            bf.Dispose();
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
                                GameTexture tilesetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset, tileset.Name);
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
                    BackgroundSound = GameAudio.AddMapSound(Sound, -1, -1, Id, true, 10);
                }
                foreach (var anim in LocalAnimations)
                {
                    anim.Update();
                }
                foreach (var en in LocalEntities)
                {
                    if (en.Value == null) continue;
                    en.Value.Update();
                }
                for (int i = 0; i < LocalEntitiesToDispose.Count; i++)
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
                return true;
            var gridX = myMap.MapGridX;
            var gridY = myMap.MapGridY;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight)
                    {
                        if (Globals.MapGrid[x, y] == Id) return true;
                    }
                }
            }
            return false;
        }

        private void HandleMapLoaded(MapInstance map)
        {
            //See if this new map is on the same grid as us
            List<GameTileBuffer> updatedBuffers = new List<GameTileBuffer>();
            if (map != this && Globals.GridMaps.Contains(map.Id) && Globals.GridMaps.Contains(Id) && MapLoaded)
            {
                var surroundingMaps = GenerateAutotileGrid();
                if (map.MapGridX == MapGridX - 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northwest
                        updatedBuffers.AddRange(CheckAutotile(0, 0, surroundingMaps));
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check West
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            updatedBuffers.AddRange(CheckAutotile(0, y, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southwest
                        updatedBuffers.AddRange(CheckAutotile(0, Options.MapHeight - 1, surroundingMaps));
                    }
                }
                else if (map.MapGridX == MapGridX)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check North
                        for (int x = 0; x < Options.MapWidth; x++)
                        {
                            updatedBuffers.AddRange(CheckAutotile(x, 0, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check South
                        for (int x = 0; x < Options.MapWidth; x++)
                        {
                            updatedBuffers.AddRange(CheckAutotile(x, Options.MapHeight - 1, surroundingMaps));
                        }
                    }
                }
                else if (map.MapGridX == MapGridX + 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northeast
                        updatedBuffers.AddRange(CheckAutotile(Options.MapWidth - 1, Options.MapHeight, surroundingMaps));
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check East
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            updatedBuffers.AddRange(CheckAutotile(Options.MapWidth - 1, y, surroundingMaps));
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southeast
                        updatedBuffers.AddRange(CheckAutotile(Options.MapWidth - 1, Options.MapHeight - 1, surroundingMaps));
                    }
                }
            }
            foreach (var buffer in updatedBuffers)
                buffer.SetData();
        }

        private GameTileBuffer[] CheckAutotile(int x, int y, MapBase[,] surroundingMaps)
        {
            var updated = new List<GameTileBuffer>();
            for (int layer = 0; layer < 5; layer++)
            {
                if (Autotiles.UpdateAutoTile(x, y, layer, surroundingMaps))
                {
                    //Find the VBO, update it.
                    var tileBuffer = mTileBufferDict[layer];
                    var tile = Layers[layer].Tiles[x, y];
                    if (tile.TilesetTex == null) continue;
                    GameTexture tilesetTex = (GameTexture)tile.TilesetTex;
                    var platformTex = tilesetTex.GetTexture();
                    if (tileBuffer.ContainsKey(platformTex))
                    {
                        for (var autotileFrame = 0; autotileFrame < 3; autotileFrame++)
                        {
                            var buffer = tileBuffer[platformTex][autotileFrame];
                            var xoffset = GetX();
                            var yoffset = GetY();
                            DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y, autotileFrame, tilesetTex, buffer,true);
                            DrawAutoTile(layer, x * Options.TileWidth + (Options.TileWidth / 2) + xoffset, y * Options.TileHeight + yoffset, 2, x, y, autotileFrame, tilesetTex, buffer,true);
                            DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 3, x, y, autotileFrame, tilesetTex, buffer,true);
                            DrawAutoTile(layer, +x * Options.TileWidth + (Options.TileWidth / 2) + xoffset, y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 4, x, y, autotileFrame, tilesetTex, buffer,true);
                            if (!updated.Contains(buffer)) updated.Add(buffer);
                        }
                    }
                }
            }
            return updated.ToArray();
        }

        //Helper Functions
        public MapBase[,] GenerateAutotileGrid()
        {
            MapBase[,] mapBase = new MapBase[3, 3];
            if (Globals.MapGrid != null && Globals.GridMaps.Contains(Id))
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
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
                    if (att == null) continue;
                    if (att.Type != MapAttributes.Animation) continue;
                    var anim = AnimationBase.Get(((MapAnimationAttribute)att).AnimationId);
                    if (anim == null) continue;
                    if (!mAttributeAnimInstances.ContainsKey(att))
                    {
                        var animInstance = new AnimationInstance(anim, true);
                        animInstance.SetPosition(GetX() + x * Options.TileWidth + Options.TileWidth / 2,
                            GetY() + y * Options.TileHeight + Options.TileHeight / 2, x, y, Id, 0);
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
                    if (attribute?.Type != MapAttributes.Sound) continue;
                    if (TextUtils.IsNone(((MapSoundAttribute)attribute).File)) continue;
                    var sound = GameAudio.AddMapSound(((MapSoundAttribute)attribute).File, x, y, Id, true, ((MapSoundAttribute)attribute).Distance);
                    AttributeSounds?.Add(sound);
                }
            }
        }

        private void ClearAttributeSounds()
        {
            AttributeSounds?.ForEach(GameAudio.StopSound);
            AttributeSounds?.Clear();
        }

        //Animations
        public void AddTileAnimation(Guid animId, int tileX, int tileY, int dir = -1)
        {
            var animBase = AnimationBase.Get(animId);
            if (animBase == null) return;
            var anim = new MapAnimationInstance(animBase, tileX, tileY, dir);
            LocalAnimations.Add(anim);
            anim.SetPosition(GetX() + tileX * Options.TileWidth + Options.TileWidth / 2,
                GetY() + tileY * Options.TileHeight + Options.TileHeight / 2, tileX, tileY,
                Id, dir);
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
            for (int i = 0; i < Options.LayerCount; i++)
            {
                mTileBuffers[i] = DrawMapLayer(i, GetX(), GetY());
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < mTileBuffers[i][y].Length; z++)
                        mTileBuffers[i][y][z].SetData();
                }
            }
        }

        public void DestroyVBOs()
        {
            for (int i = 0; i < Options.LayerCount; i++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (mTileBuffers[i][y] != null)
                    {
                        for (int z = 0; z < mTileBuffers[i][y].Length; z++)
                        {
                            mTileBuffers[i][y][z].Dispose();
                        }
                    }
                }
            }
        }

        //Rendering/Drawing Code
        public void Draw(int layer = 0) //Lower, Middle, Upper
        {
            if (!MapLoaded) return;
            CacheTextures();
            if (!mTexturesFound) return;
            if (mTileBuffers[0][0] == null) BuildVBOs();

            int drawLayerStart = 0;
            int drawLayerEnd = 2;

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

            for (int x = drawLayerStart; x <= drawLayerEnd; x++)
            {
                if (mTileBuffers[x][Globals.AnimFrame] != null)
                {
                    for (int i = 0; i < mTileBuffers[x][Globals.AnimFrame].Length; i++)
                    {
                        GameGraphics.Renderer.DrawTileBuffer(mTileBuffers[x][Globals.AnimFrame][i]);
                    }
                }
            }

            if (layer == 0)
            {
                //Draw Map Items
                foreach (var item in MapItems)
                {
                    var itemBase = ItemBase.Get(item.Value.ItemId);
                    if (itemBase != null)
                    {
                        GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                            itemBase.Icon);
                        if (itemTex != null)
                        {
                            GameGraphics.DrawGameTexture(itemTex, GetX() + item.Value.X * Options.TileWidth,
                                GetY() + item.Value.Y * Options.TileHeight);
                        }
                    }
                }

                //Add lights to our darkness texture
                foreach (var light in Lights)
                {
                    double w = light.Size;
                    var x = GetX() + (light.TileX * Options.TileWidth + light.OffsetX) + Options.TileWidth / 2f;
                    var y = GetY() + (light.TileY * Options.TileHeight + light.OffsetY) + Options.TileHeight / 2f;
                    GameGraphics.AddLight((int)x, (int)y, (int)w, light.Intensity, light.Expand, light.Color);
                }
            }
        }

        private void DrawAutoTile(int layerNum, float destX, float destY, int quarterNum, int x, int y, int forceFrame, GameTexture tileset, GameTileBuffer buffer, bool update = false)
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
                if (!buffer.UpdateTile(destX, destY,
                    (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                    (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                    Options.TileWidth / 2, Options.TileHeight / 2))
                {
                    throw new Exception("Failed to update tile to VBO!");
                }
            }
            else
            {
                if (!buffer.AddTile(tileset, destX, destY,
                    (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                    (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                    Options.TileWidth / 2, Options.TileHeight / 2))
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
                    if (tile.TilesetTex == null) continue;
                    GameTexture tilesetTex = (GameTexture)tile.TilesetTex;
                    var platformTex = tilesetTex.GetTexture();

                    GameTileBuffer[] buffers = null;
                    if (tileBuffers.ContainsKey(platformTex))
                    {
                        buffers = tileBuffers[platformTex];
                    }
                    else
                    {
                        buffers = new GameTileBuffer[3];
                        for (int i = 0; i < 3; i++)
                            buffers[i] = GameGraphics.Renderer.CreateTileBuffer();
                        tileBuffers.Add(platformTex, buffers);
                    }

                    switch (Autotiles.Autotile[x, y].Layer[layer].RenderState)
                    {
                        case MapAutotiles.RENDER_STATE_NORMAL:
                            for (int i = 0; i < 3; i++)
                            {
                                var buffer = buffers[i];
                                if (!buffer.AddTile(tilesetTex, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, Layers[layer].Tiles[x, y].X * Options.TileWidth, Layers[layer].Tiles[x, y].Y * Options.TileHeight, Options.TileWidth, Options.TileHeight))
                                {
                                    throw new Exception("Failed to add VBO!");
                                }
                            }

                            break;
                        case MapAutotiles.RENDER_STATE_AUTOTILE:
                            for (int i = 0; i < 3; i++)
                            {
                                DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y, i, tilesetTex, buffers[i]);
                                DrawAutoTile(layer, x * Options.TileWidth + (Options.TileWidth / 2) + xoffset, y * Options.TileHeight + yoffset, 2, x, y, i, tilesetTex, buffers[i]);
                                DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 3, x, y, i, tilesetTex, buffers[i]);
                                DrawAutoTile(layer, +x * Options.TileWidth + (Options.TileWidth / 2) + xoffset, y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 4, x, y, i, tilesetTex, buffers[i]);
                            }

                            break;
                    }
                }
            }

            GameTileBuffer[][] outputBuffers = new GameTileBuffer[3][];
            for (int i = 0; i < 3; i++)
                outputBuffers[i] = new GameTileBuffer[tileBuffers.Count];

            var valueArrays = tileBuffers.Values.ToArray();
            for (int x = 0; x < valueArrays.Length; x++)
            {
                for (int i = 0; i < 3; i++)
                    outputBuffers[i][x] = valueArrays[x][i];
            }

            mTileBufferDict[layer] = tileBuffers;
            return outputBuffers;
        }

        //Fogs/Panorama/Overlay
        public void DrawFog()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null) return;
            float ecTime = Globals.System.GetTimeMs() - mFogUpdateTime;
            mFogUpdateTime = Globals.System.GetTimeMs();
            if (Id == Globals.Me.CurrentMap)
            {
                if (mCurFogIntensity != 1)
                {
                    if (mCurFogIntensity < 1)
                    {
                        mCurFogIntensity += (ecTime / 2000f);
                        if (mCurFogIntensity > 1)
                        {
                            mCurFogIntensity = 1;
                        }
                    }
                    else
                    {
                        mCurFogIntensity -= (ecTime / 2000f);
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
                    mCurFogIntensity -= (ecTime / 2000f);
                    if (mCurFogIntensity < 0)
                    {
                        mCurFogIntensity = 0;
                    }
                }
            }
            if (Fog != null && Fog.Length > 0)
            {
                GameTexture fogTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Fog, Fog);
                if (fogTex != null)
                {
                    int xCount = (int)(Options.MapWidth * Options.TileWidth * 3 / fogTex.GetWidth());
                    int yCount = (int)(Options.MapHeight * Options.TileHeight * 3 / fogTex.GetHeight());

                    mFogCurrentX -= (ecTime / 1000f) * FogXSpeed * -6;
                    mFogCurrentY += (ecTime / 1000f) * FogYSpeed * 2;
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

                    var drawX = (float)Math.Round(mFogCurrentX);
                    var drawY = (float)Math.Round(mFogCurrentY);

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            int fogW = fogTex.GetWidth();
                            int fogH = fogTex.GetHeight();
                            GameGraphics.DrawGameTexture(fogTex, new FloatRect(0, 0, fogW, fogH),
                                new FloatRect(GetX() - (Options.MapWidth * Options.TileWidth * 1f) + x * fogW + drawX,
                                    GetY() - (Options.MapHeight * Options.TileHeight * 1f) + y * fogH + drawY, fogW,
                                    fogH),
                                new Intersect.Color((byte)(FogTransparency * mCurFogIntensity), 255, 255, 255), null,
                                GameBlendModes.None);
                        }
                    }
                }
            }
        }

        //Weather
        public void DrawWeather()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null) return;
            var anim = AnimationBase.Get(WeatherAnimationId);

            if (anim == null || WeatherIntensity == 0) { return; }

            _removeParticles.Clear();

            if ((WeatherXSpeed != 0 || WeatherYSpeed != 0) && Globals.Me.MapInstance == this)
            {
                if (Globals.System.GetTimeMs() > _weatherParticleSpawnTime)
                {
                    _weatherParticles.Add(new WeatherParticle(_removeParticles, WeatherXSpeed, WeatherYSpeed, anim));
                    var spawnTime = 25 + (int)(475 * (float)(1f - (float)(WeatherIntensity / 100f)));
                    spawnTime = (int)(spawnTime * (480000f / (GameGraphics.Renderer.GetScreenWidth() * GameGraphics.Renderer.GetScreenHeight())));
                    _weatherParticleSpawnTime = Globals.System.GetTimeMs() + spawnTime;
                }
            }

            //Process and draw each weather particle
            foreach (WeatherParticle w in _weatherParticles)
            {
                w.Update();
            }

            //Remove all old particles from the weather particles list from the removeparticles list.
            foreach (WeatherParticle r in _removeParticles)
            {
                r.Dispose();
                _weatherParticles.Remove(r);
            }
        }

        private void ClearWeather()
        {
            foreach (WeatherParticle r in _weatherParticles)
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
                    mPanoramaIntensity += (ecTime / 2000f);
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
                    mPanoramaIntensity -= (ecTime / 2000f);
                    if (mPanoramaIntensity < 0)
                    {
                        mPanoramaIntensity = 0;
                    }
                }
            }
            GameTexture imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, Panorama);
            if (imageTex != null)
            {
                GameGraphics.DrawFullScreenTexture(imageTex, mPanoramaIntensity);
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
                    mOverlayIntensity += (ecTime / 2000f);
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
                    mOverlayIntensity -= (ecTime / 2000f);
                    if (mOverlayIntensity < 0)
                    {
                        mOverlayIntensity = 0;
                    }
                }
            }
            GameTexture imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image,
                OverlayGraphic);
            if (imageTex != null)
            {
                GameGraphics.DrawFullScreenTexture(imageTex, mOverlayIntensity);
            }
        }

        public void CompareEffects(MapInstance oldMap)
        {
            //Check if fogs the same
            if (oldMap.Fog == Fog)
            {
                GameTexture fogTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Fog, Fog);
                if (fogTex != null)
                {
                    //Copy over fog values
                    mFogUpdateTime = oldMap.mFogUpdateTime;
                    var ratio = ((float)oldMap.FogTransparency / FogTransparency);
                    mCurFogIntensity = ratio * oldMap.mCurFogIntensity;
                    mFogCurrentX = oldMap.mFogCurrentX;
                    mFogCurrentY = oldMap.mFogCurrentY;
                    if (GetX() > oldMap.GetX())
                    {
                        mFogCurrentX -= (Options.TileWidth * Options.MapWidth) % fogTex.GetWidth();
                    }
                    else if (GetX() < oldMap.GetX())
                    {
                        mFogCurrentX += (Options.TileWidth * Options.MapWidth) % fogTex.GetWidth();
                    }
                    if (GetY() > oldMap.GetY())
                    {
                        mFogCurrentY -= (Options.TileHeight * Options.MapHeight) % fogTex.GetHeight();
                    }
                    else if (GetY() < oldMap.GetY())
                    {
                        mFogCurrentY += (Options.TileHeight * Options.MapHeight) % fogTex.GetHeight();
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
            for (int n = ActionMsgs.Count - 1; n > -1; n--)
            {
                var y =
                    (int)
                    Math.Ceiling((GetY() + ActionMsgs[n].Y * Options.TileHeight) -
                                 ((Options.TileHeight * 2) *
                                  (1000 - (ActionMsgs[n].TransmittionTimer - Globals.System.GetTimeMs())) / 1000));
                var x = (int)Math.Ceiling(GetX() + ActionMsgs[n].X * Options.TileWidth + ActionMsgs[n].XOffset);
                float textWidth = GameGraphics.Renderer.MeasureText(ActionMsgs[n].Msg, GameGraphics.GameFont, 1).X;
                GameGraphics.Renderer.DrawString(ActionMsgs[n].Msg, GameGraphics.GameFont, (int)(x) - textWidth / 2f,
                    (int)(y), 1, ActionMsgs[n].Clr, true, null, new Framework.GenericClasses.Color(40, 40, 40));

                //Try to remove
                ActionMsgs[n].TryRemove();
            }
        }

        //Events
        public void AddEvent(Guid evtId, ByteBuffer bf)
        {
            if (MapLoaded)
            {
                if (LocalEntities.ContainsKey(evtId))
                {
                    LocalEntities[evtId].Load(bf);
                }
                else
                {
                    var evt = new Event(evtId, bf);
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
            if (Lookup != null) Lookup.Delete(this);
        }

        //Dispose
        public void Dispose(bool prep = true, bool killentities = true)
        {
            MapLoaded = false;

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
            DestroyVBOs();
            Delete();
        }
    }

    public class ActionMsgInstance
    {
        public Framework.GenericClasses.Color Clr = new Framework.GenericClasses.Color();
        public MapInstance Map;
        public string Msg = "";
        public long TransmittionTimer;
        public int X;
        public long XOffset;
        public int Y;

        public ActionMsgInstance(MapInstance map, int x, int y, string message, Framework.GenericClasses.Color color)
        {
            Random rnd = new Random();

            Map = map;
            X = x;
            Y = y;
            Msg = message;
            Clr = color;
            XOffset = rnd.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
            TransmittionTimer = Globals.System.GetTimeMs() + 1000;
        }

        public void TryRemove()
        {
            if (TransmittionTimer <= Globals.System.GetTimeMs())
            {
                Map.ActionMsgs.Remove(this);
            }
        }
    }
}
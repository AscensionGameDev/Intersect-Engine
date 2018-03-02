using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.Maps;
using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Utilities;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Maps
{
    public class MapInstance : MapBase, IGameObject<int, MapInstance>
    {
        //Client Only Values
        public delegate void MapLoadedDelegate(MapInstance map);

        private static MapInstances sLookup;
        public static MapLoadedDelegate OnMapLoaded;

        //Map State Variables
        public static Dictionary<int, long> MapRequests = new Dictionary<int, long>();

        //Map Attributes
        private Dictionary<Intersect.GameObjects.Maps.Attribute, AnimationInstance> mAttributeAnimInstances =
            new Dictionary<Intersect.GameObjects.Maps.Attribute, AnimationInstance>();

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

        private int mPreRenderLayer;

        //Map Texture Caching Options
        private int mPreRenderStage;

        private bool mReRenderMap;

        //Action Msg's
        public List<ActionMsgInstance> ActionMsgs = new List<ActionMsgInstance>();

        public List<MapSound> AttributeSounds = new List<MapSound>();

        //Map Animations
        public List<MapAnimationInstance> LocalAnimations = new List<MapAnimationInstance>();

        public Dictionary<int, Entity> LocalEntities = new Dictionary<int, Entity>();

        //Map Players/Events/Npcs
        public List<int> LocalEntitiesToDispose = new List<int>();

        //Autotile Redraws
        public List<Intersect.Point> LowerAutotileRedraws = new List<Intersect.Point>();

        public GameRenderTexture[] LowerTextures = new GameRenderTexture[3];

        //Map Items
        public Dictionary<int, MapItemInstance> MapItems = new Dictionary<int, MapItemInstance>();

        private List<Event> mEvents = new List<Event>();
        public List<Intersect.Point> PeakAutotileRedraws = new List<Intersect.Point>();

        //Map Textures (If RenderCache is on)
        public GameRenderTexture[] PeakTextures = new GameRenderTexture[3];

        public List<Intersect.Point> UpperAutotileRedraws = new List<Intersect.Point>();
        public GameRenderTexture[] UpperTextures = new GameRenderTexture[3];

        //Initialization
        public MapInstance(int mapNum) : base(mapNum, true)
        {
        }

        [NotNull]
        public new static MapInstances Lookup => sLookup ?? (sLookup = new MapInstances(MapBase.Lookup));

        public bool MapLoaded { get; private set; }
        public bool MapRendered { get; set; }

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
        
        //Load
        public void Load(string json)
        {
            LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
            JsonConvert.PopulateObject(json, this, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            MapLoaded = true;
            Autotiles = new MapAutotiles(this);
            CreateMapSounds();
            MapRendered = false;
            OnMapLoaded += HandleMapLoaded;
            if (MapRequests.ContainsKey(Index)) MapRequests.Remove(Index);
        }

        public void LoadTileData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Layers = new TileArray[Options.LayerCount];
            for (var i = 0; i < Options.LayerCount; i++)
            {
                Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            bf.Dispose();
        }

        //Updating
        public void Update(bool isLocal)
        {
            if (isLocal)
            {
                mLastUpdateTime = Globals.System.GetTimeMs() + 10000;
                if (BackgroundSound == null && !TextUtils.IsNone(Sound))
                {
                    BackgroundSound = GameAudio.AddMapSound(Sound, -1, -1, Index, true, 10);
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
                if (Globals.System.GetTimeMs() > mLastUpdateTime || GameGraphics.FreeMapTextures.Count < 27)
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
                        if (Globals.MapGrid[x, y] == Index) return true;
                    }
                }
            }
            return false;
        }

        //Helper Functions
        public MapBase[,] GenerateAutotileGrid()
        {
            MapBase[,] mapBase = new MapBase[3, 3];
            if (Globals.MapGrid != null && Globals.GridMaps.Contains(Index))
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
            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] == null) continue;
                    if (Attributes[x, y].Value != (int) MapAttributes.Animation) continue;
                    var anim = AnimationBase.Lookup.Get<AnimationBase>(Attributes[x, y].Data1);
                    if (anim == null) continue;
                    if (!mAttributeAnimInstances.ContainsKey(Attributes[x, y]))
                    {
                        var animInstance = new AnimationInstance(anim, true);
                        animInstance.SetPosition(GetX() + x * Options.TileWidth + Options.TileWidth / 2,
                            GetY() + y * Options.TileHeight + Options.TileHeight / 2, x, y,
                            Index, 0);
                        mAttributeAnimInstances.Add(Attributes[x, y], animInstance);
                    }
                    mAttributeAnimInstances[Attributes[x, y]].Update();
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
        private void CreateMapSounds()
        {
            ClearAttributeSounds();
            for (var x = 0; x < Options.MapWidth; ++x)
            {
                for (var y = 0; y < Options.MapHeight; ++y)
                {
                    var attribute = Attributes?[x, y];
                    if (attribute?.Value != (int) MapAttributes.Sound) continue;
                    if (TextUtils.IsNone(attribute.Data4)) continue;
                    var sound = GameAudio.AddMapSound(attribute.Data4, x, y, Index, true, attribute.Data1);
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
        public void AddTileAnimation(int animNum, int tileX, int tileY, int dir = -1)
        {
            var animBase = AnimationBase.Lookup.Get<AnimationBase>(animNum);
            if (animBase == null) return;
            var anim = new MapAnimationInstance(animBase, tileX, tileY, dir);
            LocalAnimations.Add(anim);
            anim.SetPosition(GetX() + tileX * Options.TileWidth + Options.TileWidth / 2,
                GetY() + tileY * Options.TileHeight + Options.TileHeight / 2, tileX, tileY,
                Index, dir);
        }

        private void HideActiveAnimations()
        {
            LocalEntities?.Values.ToList().ForEach(entity => entity?.ClearAnimations(null));
            LocalAnimations?.ForEach(animation => animation?.Dispose());
            LocalAnimations?.Clear();
            ClearMapAttributes();
        }

        public void FixAutotiles()
        {
            //If we have autotiles that recalculated then let's redraw them without redrawing the whole map....
            var autotileUpdates = LowerAutotileRedraws?.ToArray();
            LowerAutotileRedraws?.Clear();
            RedrawAutotiles(autotileUpdates, LowerTextures, 0);

            autotileUpdates = UpperAutotileRedraws?.ToArray();
            UpperAutotileRedraws?.Clear();
            RedrawAutotiles(autotileUpdates, UpperTextures, 1);

            autotileUpdates = PeakAutotileRedraws?.ToArray();
            PeakAutotileRedraws?.Clear();
            RedrawAutotiles(autotileUpdates, PeakTextures, 2);
        }

        //Rendering/Drawing Code
        public void Draw(int layer = 0)
        {
            if (!MapLoaded) return;
            if (mReRenderMap)
            {
                MapRendered = false;
                ReleaseRenderTextures();
                mPreRenderLayer = 0;
                mPreRenderStage = 0;
                mReRenderMap = false;
            }
            if (!MapRendered)
            {
                if (ClientOptions.RenderCache) return;
                switch (layer)
                {
                    case 0:
                        for (int i = 0; i < 3; i++)
                        {
                            DrawMapLayer(null, i, Globals.AnimFrame, GetX(), GetY());
                        }
                        break;

                    case 1:
                        DrawMapLayer(null, 3, Globals.AnimFrame, GetX(), GetY());
                        break;

                    case 2:
                        DrawMapLayer(null, 4, Globals.AnimFrame, GetX(), GetY());
                        break;
                }
            }
            else
            {
                if (layer == 0)
                {
                    GameGraphics.DrawGameTexture(LowerTextures[Globals.AnimFrame], GetX(), GetY(), null, GameBlendModes.None);
                }
                else if (layer == 1)
                {
                    GameGraphics.DrawGameTexture(UpperTextures[Globals.AnimFrame], GetX(), GetY(), null, GameBlendModes.None);
                }
                else
                {
                    GameGraphics.DrawGameTexture(PeakTextures[Globals.AnimFrame], GetX(), GetY(), null, GameBlendModes.None);
                }
            }
            if (layer == 0)
            {
                //Draw Map Items
                foreach (var item in MapItems)
                {
                    var itemBase = ItemBase.Lookup.Get<ItemBase>(item.Value.ItemNum);
                    if (itemBase != null)
                    {
                        GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                            itemBase.Pic);
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
                    GameGraphics.AddLight((int) x, (int) y, (int) w, light.Intensity, light.Expand, light.Color);
                }
                UpdateMapAttributes();
            }
        }

        private void HandleMapLoaded(MapInstance map)
        {
            //See if this new map is on the same grid as us
            if (map != this && Globals.GridMaps.Contains(map.Index) && Globals.GridMaps.Contains(Index))
            {
                var surroundingMaps = GenerateAutotileGrid();
                if (map.MapGridX == MapGridX - 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northwest
                        CheckAutotile(0, 0, surroundingMaps);
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check West
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            CheckAutotile(0, y, surroundingMaps);
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southwest
                        CheckAutotile(0, Options.MapHeight - 1, surroundingMaps);
                    }
                }
                else if (map.MapGridX == MapGridX)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check North
                        for (int x = 0; x < Options.MapWidth; x++)
                        {
                            CheckAutotile(x, 0, surroundingMaps);
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check South
                        for (int x = 0; x < Options.MapWidth; x++)
                        {
                            CheckAutotile(x, Options.MapHeight - 1, surroundingMaps);
                        }
                    }
                }
                else if (map.MapGridX == MapGridX + 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northeast
                        CheckAutotile(Options.MapWidth - 1, Options.MapHeight, surroundingMaps);
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check East
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            CheckAutotile(Options.MapWidth - 1, y, surroundingMaps);
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southeast
                        CheckAutotile(Options.MapWidth - 1, Options.MapHeight - 1, surroundingMaps);
                    }
                }
            }
        }

        private void CheckAutotile(int x, int y, MapBase[,] surroundingMaps)
        {
            var addedLower = false;
            for (int layer = 0; layer < 5; layer++)
            {
                if (Autotiles.UpdateAutoTile(x, y, layer, surroundingMaps) && ClientOptions.RenderCache)
                {
                    Intersect.Point pnt;
                    pnt.X = x;
                    pnt.Y = y;
                    if (layer == 3)
                    {
                        if (!UpperAutotileRedraws.Contains(pnt)) UpperAutotileRedraws.Add(pnt);
                    }
                    else if (layer == 4)
                    {
                        if (!PeakAutotileRedraws.Contains(pnt)) PeakAutotileRedraws.Add(pnt);
                    }
                    else
                    {
                        if (!LowerAutotileRedraws.Contains(pnt)) LowerAutotileRedraws.Add(pnt);
                    }
                }
            }
        }

        private void RedrawAutotiles(Intersect.Point[] points, GameRenderTexture[] textures, int layer)
        {
            int layerMin = 0;
            int layerMax = 0;
            switch (layer)
            {
                case 0: //Lower 3 layers
                    layerMax = 2;
                    break;
                case 1: //Upper Layers
                    layerMin = 3;
                    layerMax = 3;
                    break;
                case 2: //Peak Layers
                    layerMin = 4;
                    layerMax = 4;
                    break;
            }
            if (points == null || points.Length == 0) return;
            //We want to run this in such a way that we are switching textures as little as possible
            for (int animFrame = 0; animFrame < 3; animFrame++)
            {
                //Clear all of the layers
                for (layer = layerMin; layer <= layerMax; layer++)
                {
                    foreach (var pnt in points)
                    {
                        GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1),
                            new FloatRect(pnt.X * Options.TileWidth, pnt.Y * Options.TileHeight, Options.TileWidth,
                                Options.TileHeight), Intersect.Color.White, textures[animFrame], GameBlendModes.Cutout,
                            null, 0f);
                    }
                }
                //Then draw all of the layers
                for (layer = layerMin; layer <= layerMax; layer++)
                {
                    foreach (var pnt in points)
                    {
                        DrawTile(pnt.X, pnt.Y, layer, animFrame, 0, 0, textures[animFrame]);
                    }
                }
            }
        }

        private void DrawAutoTile(int layerNum, float destX, float destY, int quarterNum, int x, int y, int forceFrame,
            GameRenderTexture tex, GameTexture tileset)
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

                case MapAutotiles.AUTOTILE_CLIFF:
                    yOffset = -Options.TileHeight;
                    break;
            }
            GameGraphics.DrawGameTexture(tileset, destX, destY,
                (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                Options.TileWidth / 2, Options.TileHeight / 2, tex);
        }

        private void DrawMapLayer(GameRenderTexture tex, int layer, int animFrame, float xoffset = 0, float yoffset = 0)
        {
            int xmin = 0;
            int xmax = Options.MapWidth;
            int ymin = 0;
            int ymax = Options.MapHeight;
            if (!ClientOptions.RenderCache)
            {
                if (GetX() < GameGraphics.CurrentView.Left)
                {
                    xmin += (int) (Math.Floor((GameGraphics.CurrentView.Left - GetX()) / 32));
                }
                if (GetX() + (xmax * 32) > GameGraphics.CurrentView.Left + GameGraphics.CurrentView.Width)
                {
                    xmax -=
                        (int)
                        (Math.Floor(((GetX() + (xmax * 32)) -
                                     (GameGraphics.CurrentView.Left + GameGraphics.CurrentView.Width)) / 32));
                }
                if (GetY() < GameGraphics.CurrentView.Top)
                {
                    ymin += (int) (Math.Floor((GameGraphics.CurrentView.Top - GetY()) / 32));
                }
                if (GetY() + (ymax * 32) > GameGraphics.CurrentView.Top + GameGraphics.CurrentView.Height)
                {
                    ymax -=
                        (int)
                        (Math.Floor(((GetY() + (ymax * 32)) -
                                     (GameGraphics.CurrentView.Top + GameGraphics.CurrentView.Height)) / 32));
                }
                xmin -= 2;
                if (xmin < 0) xmin = 0;
                xmax += 2;
                if (xmax > Options.MapWidth) xmax = Options.MapWidth;
                ymin -= 2;
                if (ymin < 0) ymin = 0;
                ymax += 2;
                if (ymax > Options.MapHeight) ymax = Options.MapHeight;
            }
            for (var x = xmin; x < xmax; x++)
            {
                for (var y = ymin; y < ymax; y++)
                {
                    DrawTile(x, y, layer, animFrame, xoffset, yoffset, tex);
                }
            }
        }

        private void DrawTile(int x, int y, int layer, int animFrame, float xoffset, float yoffset,
            GameRenderTexture tex)
        {
            var tileset = TilesetBase.Lookup.Get<TilesetBase>(Layers[layer].Tiles[x, y].TilesetIndex);
            if (tileset == null) return;
            GameTexture tilesetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                tileset.Name);
            if (tilesetTex == null) return;
            switch (Autotiles.Autotile[x, y].Layer[layer].RenderState)
            {
                case MapAutotiles.RENDER_STATE_NORMAL:
                    GameGraphics.DrawGameTexture(tilesetTex, x * Options.TileWidth + xoffset,
                        y * Options.TileHeight + yoffset, Layers[layer].Tiles[x, y].X * Options.TileWidth,
                        Layers[layer].Tiles[x, y].Y * Options.TileHeight, Options.TileWidth, Options.TileHeight, tex);
                    break;
                case MapAutotiles.RENDER_STATE_AUTOTILE:
                    DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y,
                        animFrame, tex, tilesetTex);
                    DrawAutoTile(layer, x * Options.TileWidth + (Options.TileWidth / 2) + xoffset,
                        y * Options.TileHeight + yoffset, 2, x, y, animFrame, tex, tilesetTex);
                    DrawAutoTile(layer, x * Options.TileWidth + xoffset,
                        y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 3, x, y, animFrame, tex,
                        tilesetTex);
                    DrawAutoTile(layer, +x * Options.TileWidth + (Options.TileWidth / 2) + xoffset,
                        y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 4, x, y, animFrame, tex,
                        tilesetTex);
                    break;
            }
        }

        //Map Caching (Only used if Options.RenderCaching is true)
        public bool PreRenderMap()
        {
            if (!Globals.HasGameData) return false;
            GameGraphics.PreRenderedMapLayer = true;
            if (mPreRenderStage < 3)
            {
                int i = mPreRenderStage;
                if (LowerTextures[i] == null)
                {
                    if (!GameGraphics.GetMapTexture(ref LowerTextures[i])) return false;
                }
                if (UpperTextures[i] == null)
                {
                    if (!GameGraphics.GetMapTexture(ref UpperTextures[i])) return false;
                }
                if (PeakTextures[i] == null)
                {
                    if (!GameGraphics.GetMapTexture(ref PeakTextures[i])) return false;
                }
                LowerTextures[i].Clear(Color.Transparent);
                UpperTextures[i].Clear(Color.Transparent);
                PeakTextures[i].Clear(Color.Transparent);

                mPreRenderStage++;
                return true;
            }
            else if (mPreRenderStage >= 3 && mPreRenderStage < 6)
            {
                int i = mPreRenderStage - 3;
                int l = mPreRenderLayer;
                mPreRenderLayer++;
                if (l < 3)
                {
                    LowerTextures[i].Begin();
                    DrawMapLayer(LowerTextures[i], l, i);
                    LowerTextures[i].End();
                    return true;
                }
                else if (l == 3)
                {
                    UpperTextures[i].Begin();
                    DrawMapLayer(UpperTextures[i], l, i);
                    UpperTextures[i].End();
                    return true;
                }
                else
                {
                    PeakTextures[i].Begin();
                    DrawMapLayer(PeakTextures[i], l, i);
                    PeakTextures[i].End();
                    mPreRenderStage++;
                    mPreRenderLayer = 0;
                    return true;
                }
            }

            MapRendered = true;
            return true;
        }

        //Fogs/Panorama/Overlay
        public void DrawFog()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null) return;
            float ecTime = Globals.System.GetTimeMs() - mFogUpdateTime;
            mFogUpdateTime = Globals.System.GetTimeMs();
            if (Index == Globals.Me.CurrentMap)
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
                    int xCount = (int) (Options.MapWidth * Options.TileWidth * 3 / fogTex.GetWidth());
                    int yCount = (int) (Options.MapHeight * Options.TileHeight * 3 / fogTex.GetHeight());

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

                    var drawX = (float) Math.Round(mFogCurrentX);
                    var drawY = (float) Math.Round(mFogCurrentY);

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
                                new Intersect.Color((byte) (FogTransparency * mCurFogIntensity), 255, 255, 255), null,
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
            var anim = AnimationBase.Lookup.Get<AnimationBase>(Weather);

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
            if (Index == Globals.Me.CurrentMap)
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
            if (Index == Globals.Me.CurrentMap)
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
                    var ratio = ((float) oldMap.FogTransparency / FogTransparency);
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
                var x = (int) Math.Ceiling(GetX() + ActionMsgs[n].X * Options.TileWidth + ActionMsgs[n].XOffset);
                float textWidth = GameGraphics.Renderer.MeasureText(ActionMsgs[n].Msg, GameGraphics.GameFont, 1).X;
                GameGraphics.Renderer.DrawString(ActionMsgs[n].Msg, GameGraphics.GameFont, (int) (x) - textWidth / 2f,
                    (int) (y), 1, ActionMsgs[n].Clr, true, null, new Color(40, 40, 40));

                //Try to remove
                ActionMsgs[n].TryRemove();
            }
        }

        //Events
        public void AddEvent(Event evt)
        {
            if (MapLoaded && (!LocalEntities.ContainsKey(evt.MyIndex) || LocalEntities[evt.MyIndex].SpawnTime != evt.SpawnTime))
            {
                mEvents.Add(evt);
                if (LocalEntities.ContainsKey(evt.MyIndex))
                {
                    LocalEntities[evt.MyIndex].Dispose();
                    LocalEntities[evt.MyIndex] = evt;
                }
                else
                {
                    LocalEntities.Add(evt.MyIndex, evt);
                }
            }
            else
            {
                evt.Dispose();
            }
        }

        public void RemoveEvent(Event evt)
        {
            mEvents.Remove(evt);
            if (LocalEntities.ContainsKey(evt.MyIndex))
            {
                LocalEntities.Remove(evt.MyIndex);
                evt.Dispose();
            }
        }

        public override void Delete()
        {
            if (Lookup != null) Lookup.Delete(this);
        }

        //Dispose
        public void Dispose(bool prep = true, bool killentities = true)
        {
            if (ClientOptions.RenderCache)
            {
                ReleaseRenderTextures();
            }

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
                    if (en.Value.CurrentMap == Index)
                    {
                        Globals.EntitiesToDispose.Add(en.Key);
                    }
                }
                foreach (var en in LocalEntities)
                {
                    en.Value.Dispose();
                }
            }
            MapRendered = false;
            HideActiveAnimations();
            ClearWeather();
            ClearMapAttributes();
            Delete();
        }

        private void ReleaseRenderTextures()
        {
            //Clean up all map stuff.
            for (int i = 0; i < 3; i++)
            {
                if (LowerTextures[i] != null)
                {
                    GameGraphics.ReleaseMapTexture(LowerTextures[i]);
                    LowerTextures[i] = null;
                }
                if (UpperTextures[i] != null)
                {
                    GameGraphics.ReleaseMapTexture(UpperTextures[i]);
                    UpperTextures[i] = null;
                }
                if (PeakTextures[i] != null)
                {
                    GameGraphics.ReleaseMapTexture(PeakTextures[i]);
                    PeakTextures[i] = null;
                }
            }
        }
    }

    public class ActionMsgInstance
    {
        public Color Clr = new Color();
        public int MapNum;
        public string Msg = "";
        public long TransmittionTimer;
        public int X;
        public long XOffset;
        public int Y;

        public ActionMsgInstance(int mapNum, int x, int y, string message, Color color)
        {
            Random rnd = new Random();

            MapNum = mapNum;
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
                MapInstance.Lookup.Get<MapInstance>(MapNum).ActionMsgs.Remove(this);
            }
        }
    }
}
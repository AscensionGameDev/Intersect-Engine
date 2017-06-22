using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Localization;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intersect.Enums;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Maps
{
    public class MapInstance : MapBase, IGameObject<int, MapInstance>
    {
        private static MapInstances sLookup;

        public new static MapInstances Lookup
        {
            get
            {
                if (sLookup == null)
                {
                    sLookup = new MapInstances(MapBase.Lookup);
                }

                return sLookup;
            }
        }

        //Client Only Values
        public delegate void MapLoadedDelegate(MapInstance map);
        public static MapLoadedDelegate OnMapLoaded;

        //Map State Variables
        public static Dictionary<int, long> MapRequests = new Dictionary<int, long>();

        //Map Attributes
        private Dictionary<Intersect.GameObjects.Maps.Attribute, AnimationInstance> _attributeAnimInstances =
            new Dictionary<Intersect.GameObjects.Maps.Attribute, AnimationInstance>();

        protected float _curFogIntensity;
        protected float _fogCurrentX;
        protected float _fogCurrentY;

        //Fog Variables
        protected long _fogUpdateTime = -1;

        //Update Timer
        private long _lastUpdateTime;
        protected float _overlayIntensity;

        //Overlay Image Variables
        protected long _overlayUpdateTime = -1;
        protected float _panoramaIntensity;

        //Panorama Variables
        protected long _panoramaUpdateTime = -1;
        private int _preRenderLayer;

        //Map Texture Caching Options
        private int _preRenderStage;
        private bool _reRenderMap;

        //Action Msg's
        public List<ActionMsgInstance> ActionMsgs = new List<ActionMsgInstance>();
        public List<MapSound> AttributeSounds = new List<MapSound>();

        //Map Animations
        public List<MapAnimationInstance> LocalAnimations = new List<MapAnimationInstance>();
        public Dictionary<int, Entity> LocalEntities = new Dictionary<int, Entity>();

        //Map Items
        public Dictionary<int, MapItemInstance> MapItems = new Dictionary<int, MapItemInstance>();

        //Map Players/Events/Npcs
        public List<int> LocalEntitiesToDispose = new List<int>();
        private List<Event> mEvents = new List<Event>();

        //Map Textures (If RenderCache is on)
        public GameRenderTexture[] PeakTextures = new GameRenderTexture[3];
        public GameRenderTexture[] UpperTextures = new GameRenderTexture[3];
        public GameRenderTexture[] LowerTextures = new GameRenderTexture[3];

        //Autotile Redraws
        public List<Intersect.Point> LowerAutotileRedraws = new List<Intersect.Point>();
        public List<Intersect.Point> UpperAutotileRedraws = new List<Intersect.Point>();
        public List<Intersect.Point> PeakAutotileRedraws = new List<Intersect.Point>();

        //Initialization
        public MapInstance(int mapNum) : base(mapNum, true)
        {
        }

        public bool MapLoaded { get; private set; }
        public bool MapRendered { get; private set; }

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

        //LEGACY -- REMOVE WHEN SERIALIZATION IS WORKING!
        //Load
        public void Load(byte[] packet)
        {
            LocalEntitiesToDispose.AddRange(LocalEntities.Keys.ToArray());
            base.Load(packet);
            MapLoaded = true;
            Autotiles = new MapAutotiles(this);
            CreateMapSounds();
            MapRendered = false;
            OnMapLoaded += HandleMapLoaded;
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
                _lastUpdateTime = Globals.System.GetTimeMS() + 10000;
                if (BackgroundSound == null && Sound != Strings.Get("general", "none") && Sound != "")
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
                if (Globals.System.GetTimeMS() > _lastUpdateTime || GameGraphics.FreeMapTextures.Count < 27)
                {
                    Dispose();
                }
                HideActiveAnimations();
            }
        }

        public bool InView()
        {
            if (Globals.MapGridWidth == 0 || Globals.MapGridHeight == 0 || Lookup.Get(Globals.Me.CurrentMap) == null)
                return true;
            var gridX = Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridX;
            var gridY = Lookup.Get<MapInstance>(Globals.Me.CurrentMap).MapGridY;
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
            for (int x = 0; x < Options.MapWidth; x++)
            {
                for (int y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
                    {
                        if (Attributes[x, y].value == (int) MapAttributes.Animation)
                        {
                            var anim = AnimationBase.Lookup.Get<AnimationBase>(Attributes[x, y].data1);
                            if (anim != null)
                            {
                                if (!_attributeAnimInstances.ContainsKey(Attributes[x, y]))
                                {
                                    var animInstance = new AnimationInstance(anim, true);
                                    animInstance.SetPosition(GetX() + x * Options.TileWidth + Options.TileWidth / 2,
                                        GetY() + y * Options.TileHeight + Options.TileHeight / 2, x, y,
                                        this.Index, 0);
                                    _attributeAnimInstances.Add(Attributes[x, y], animInstance);
                                }
                                _attributeAnimInstances[Attributes[x, y]].Update();
                            }
                        }
                    }
                }
            }
        }

        private void ClearMapAttributes()
        {
            foreach (var attributeInstance in _attributeAnimInstances)
            {
                attributeInstance.Value.Dispose();
            }
            _attributeAnimInstances.Clear();
        }

        //Sound Functions
        private void CreateMapSounds()
        {
            ClearAttributeSounds();
            for (int x = 0; x < Options.MapWidth; x++)
            {
                for (int y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
                    {
                        if (Attributes[x, y].value == (int) MapAttributes.Sound)
                        {
                            if (Attributes[x, y].data4 != Strings.Get("general", "none") && Attributes[x, y].data4 != "")
                            {
                                AttributeSounds.Add(GameAudio.AddMapSound(Attributes[x, y].data4, x, y, Index, true,
                                    Attributes[x, y].data1));
                            }
                        }
                    }
                }
            }
        }

        private void ClearAttributeSounds()
        {
            for (int i = 0; i < AttributeSounds.Count; i++)
            {
                GameAudio.StopSound(AttributeSounds[i]);
            }
            AttributeSounds.Clear();
        }

        //Animations
        public void AddTileAnimation(int animNum, int tileX, int tileY, int dir = -1)
        {
            var animBase = AnimationBase.Lookup.Get<AnimationBase>(animNum);
            if (animBase != null)
            {
                var anim = new MapAnimationInstance(animBase, tileX, tileY, dir);
                LocalAnimations.Add(anim);
                anim.SetPosition(GetX() + tileX * Options.TileWidth + Options.TileWidth / 2,
                    GetY() + tileY * Options.TileHeight + Options.TileHeight / 2, tileX, tileY,
                    this.Index, dir);
            }
        }

        private void HideActiveAnimations()
        {
            foreach (Entity en in LocalEntities.Values)
            {
                en.ClearAnimations();
            }
            foreach (MapAnimationInstance anim in LocalAnimations)
            {
                anim.Dispose();
            }
            LocalAnimations.Clear();
            ClearMapAttributes();
        }

        public void FixAutotiles()
        {
            //If we have autotiles that recalculated then let's redraw them without redrawing the whole map....
            var autotileUpdates = LowerAutotileRedraws.ToArray();
            LowerAutotileRedraws.Clear();
            RedrawAutotiles(autotileUpdates, LowerTextures, 0);
            autotileUpdates = UpperAutotileRedraws.ToArray();
            UpperAutotileRedraws.Clear();
            RedrawAutotiles(autotileUpdates, UpperTextures, 1);
            autotileUpdates = PeakAutotileRedraws.ToArray();
            PeakAutotileRedraws.Clear();
            RedrawAutotiles(autotileUpdates, PeakTextures, 2);
        }

        //Rendering/Drawing Code
        public void Draw(int layer = 0)
        {
            if (_reRenderMap)
            {
                MapRendered = false;
                ReleaseRenderTextures();
                _preRenderLayer = 0;
                _preRenderStage = 0;
                _reRenderMap = false;
            }
            if (!MapRendered)
            {
                if (Globals.Database.RenderCaching) return;
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
                    GameGraphics.DrawGameTexture(LowerTextures[Globals.AnimFrame], GetX(), GetY());
                }
                else if (layer == 1)
                {
                    GameGraphics.DrawGameTexture(UpperTextures[Globals.AnimFrame], GetX(), GetY());
                }
                else
                {
                    GameGraphics.DrawGameTexture(PeakTextures[Globals.AnimFrame], GetX(), GetY());
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
            if (layer == 2)
            {
                DrawFog();
                DrawOverlayGraphic();
            }
        }

        private void HandleMapLoaded(MapInstance map)
        {
            //See if this new map is on the same grid as us
            if (map != this && Globals.GridMaps.Contains(map.Index) && Globals.GridMaps.Contains(this.Index))
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
                        CheckAutotile(0, Options.MapHeight -1, surroundingMaps);
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
                            CheckAutotile(x, Options.MapHeight -1, surroundingMaps);
                        }
                    }
                }
                else if (map.MapGridX == MapGridX + 1)
                {
                    if (map.MapGridY == MapGridY - 1)
                    {
                        //Check Northeast
                        CheckAutotile(Options.MapWidth -1, Options.MapHeight, surroundingMaps);
                    }
                    else if (map.MapGridY == MapGridY)
                    {
                        //Check East
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            CheckAutotile(Options.MapWidth -1, y, surroundingMaps);
                        }
                    }
                    else if (map.MapGridY == MapGridY + 1)
                    {
                        //Check Southeast
                        CheckAutotile(Options.MapWidth -1, Options.MapHeight - 1, surroundingMaps);
                    }
                }
            }
        }

        private void CheckAutotile(int x, int y, MapBase[,] surroundingMaps)
        {
            var addedLower = false;
            for (int layer = 0; layer < 5; layer++)
            {
                if (Autotiles.UpdateAutoTile(x, y, layer, surroundingMaps) && Globals.Database.RenderCaching)
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
                        GameGraphics.DrawGameTexture(GameGraphics.Renderer.GetWhiteTexture(), new FloatRect(0, 0, 1, 1), new FloatRect(pnt.X * Options.TileWidth, pnt.Y * Options.TileHeight, Options.TileWidth, Options.TileHeight), Intersect.Color.White, textures[animFrame], GameBlendModes.Cutout, null, 0f);
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
            GameRenderTexture tex)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (Layers[layerNum].Tiles[x, y].Autotile)
            {
                case MapAutotiles.AutotileWaterfall:
                    yOffset = (forceFrame - 1) * Options.TileHeight;
                    break;

                case MapAutotiles.AutotileAnim:
                    xOffset = forceFrame * Options.TileWidth * 2;
                    break;

                case MapAutotiles.AutotileCliff:
                    yOffset = -Options.TileHeight;
                    break;
            }
            if (TilesetBase.Lookup.Get<TilesetBase>(Layers[layerNum].Tiles[x, y].TilesetIndex) != null)
            {
                GameTexture tileset = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                    TilesetBase.Lookup.Get<TilesetBase>(Layers[layerNum].Tiles[x, y].TilesetIndex).Name);
                GameGraphics.DrawGameTexture(tileset, destX, destY,
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                    (int) Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                    Options.TileWidth / 2, Options.TileHeight / 2, tex);
            }
        }

        private void DrawMapLayer(GameRenderTexture tex, int layer, int animFrame, float xoffset = 0, float yoffset = 0)
        {
            int xmin = 0;
            int xmax = Options.MapWidth;
            int ymin = 0;
            int ymax = Options.MapHeight;
            if (!Globals.Database.RenderCaching)
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
                    DrawTile(x, y, layer, animFrame, xoffset, yoffset,tex);
                }
            }
        }

        private void DrawTile(int x, int y, int layer, int animFrame, float xoffset, float yoffset, GameRenderTexture tex)
        {
            var tileset = TilesetBase.Lookup.Get<TilesetBase>(Layers[layer].Tiles[x, y].TilesetIndex);
            if (tileset == null) return;
            GameTexture tilesetTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Tileset,
                tileset.Name);
            if (tilesetTex == null) return;
            switch (Autotiles.Autotile[x, y].Layer[layer].RenderState)
            {
                case MapAutotiles.RenderStateNormal:
                    GameGraphics.DrawGameTexture(tilesetTex, x * Options.TileWidth + xoffset,
                        y * Options.TileHeight + yoffset, Layers[layer].Tiles[x, y].X * Options.TileWidth,
                        Layers[layer].Tiles[x, y].Y * Options.TileHeight, Options.TileWidth, Options.TileHeight, tex);
                    break;
                case MapAutotiles.RenderStateAutotile:
                    DrawAutoTile(layer, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y,
                        animFrame, tex);
                    DrawAutoTile(layer, x * Options.TileWidth + (Options.TileWidth / 2) + xoffset,
                        y * Options.TileHeight + yoffset, 2, x, y, animFrame, tex);
                    DrawAutoTile(layer, x * Options.TileWidth + xoffset,
                        y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 3, x, y, animFrame, tex);
                    DrawAutoTile(layer, +x * Options.TileWidth + (Options.TileWidth / 2) + xoffset,
                        y * Options.TileHeight + (Options.TileHeight / 2) + yoffset, 4, x, y, animFrame, tex);
                    break;
            }
        }

        //Map Caching (Only used if Options.RenderCaching is true)
        public void PreRenderMap()
        {
            if (!Globals.HasGameData) return;
            GameGraphics.PreRenderedMapLayer = true;
            if (_preRenderStage < 3)
            {
                int i = _preRenderStage;
                if (LowerTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref LowerTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (UpperTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref UpperTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (PeakTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref PeakTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                LowerTextures[i].Clear(Color.Transparent);
                UpperTextures[i].Clear(Color.Transparent);
                PeakTextures[i].Clear(Color.Transparent);

                _preRenderStage++;
                return;
            }
            else if (_preRenderStage >= 3 && _preRenderStage < 6)
            {
                int i = _preRenderStage - 3;
                int l = _preRenderLayer;
                _preRenderLayer++;
                if (l < 3)
                {
                    LowerTextures[i].Begin();
                    DrawMapLayer(LowerTextures[i], l, i);
                    LowerTextures[i].End();
                    return;
                }
                else if (l == 3)
                {
                    UpperTextures[i].Begin();
                    DrawMapLayer(UpperTextures[i], l, i);
                    UpperTextures[i].End();
                    return;
                }
                else
                {
                    PeakTextures[i].Begin();
                    DrawMapLayer(PeakTextures[i], l, i);
                    PeakTextures[i].End();
                    _preRenderStage++;
                    _preRenderLayer = 0;
                    return;
                }
            }

            MapRendered = true;
        }

        //Fogs/Panorama/Overlay
        private void DrawFog()
        {
            if (Globals.Me == null || Lookup.Get(Globals.Me.CurrentMap) == null) return;
            float ecTime = Globals.System.GetTimeMS() - _fogUpdateTime;
            _fogUpdateTime = Globals.System.GetTimeMS();
            if (Index == Globals.Me.CurrentMap)
            {
                if (_curFogIntensity != 1)
                {
                    if (_curFogIntensity < 1)
                    {
                        _curFogIntensity += (ecTime / 2000f);
                        if (_curFogIntensity > 1)
                        {
                            _curFogIntensity = 1;
                        }
                    }
                    else
                    {
                        _curFogIntensity -= (ecTime / 2000f);
                        if (_curFogIntensity < 1)
                        {
                            _curFogIntensity = 1;
                        }
                    }
                }
            }
            else
            {
                if (_curFogIntensity != 0)
                {
                    _curFogIntensity -= (ecTime / 2000f);
                    if (_curFogIntensity < 0)
                    {
                        _curFogIntensity = 0;
                    }
                }
            }
            if (Fog.Length > 0)
            {
                GameTexture fogTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Fog, Fog);
                if (fogTex != null)
                {
                    int xCount = (int) (Options.MapWidth * Options.TileWidth * 3 / fogTex.GetWidth());
                    int yCount = (int) (Options.MapHeight * Options.TileHeight * 3 / fogTex.GetHeight());

                    _fogCurrentX -= (ecTime / 1000f) * FogXSpeed * -6;
                    _fogCurrentY += (ecTime / 1000f) * FogYSpeed * 2;
                    float deltaX = 0;
                    _fogCurrentX -= deltaX;
                    float deltaY = 0;
                    _fogCurrentY -= deltaY;

                    if (_fogCurrentX < fogTex.GetWidth())
                    {
                        _fogCurrentX += fogTex.GetWidth();
                    }
                    if (_fogCurrentX > fogTex.GetWidth())
                    {
                        _fogCurrentX -= fogTex.GetWidth();
                    }
                    if (_fogCurrentY < fogTex.GetHeight())
                    {
                        _fogCurrentY += fogTex.GetHeight();
                    }
                    if (_fogCurrentY > fogTex.GetHeight())
                    {
                        _fogCurrentY -= fogTex.GetHeight();
                    }

                    var drawX = (float) Math.Round(_fogCurrentX);
                    var drawY = (float) Math.Round(_fogCurrentY);

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            int fogW = fogTex.GetWidth();
                            int fogH = fogTex.GetHeight();
                            GameGraphics.DrawGameTexture(fogTex, new FloatRect(0, 0, fogW, fogH),
                                new FloatRect(GetX() - (Options.MapWidth * Options.TileWidth * 1f) + x * fogW + drawX,
                                    GetY() - (Options.MapHeight * Options.TileHeight * 1f) + y * fogH + drawY, fogW,
                                    fogH), new Intersect.Color((byte) (FogTransparency * _curFogIntensity), 255, 255, 255), null,
                                GameBlendModes.Alpha);
                        }
                    }
                }
            }
        }

        public void GridSwitched()
        {
            _panoramaIntensity = 1f;
            _curFogIntensity = 1f;
        }

        public void DrawPanorama()
        {
            float ecTime = Globals.System.GetTimeMS() - _panoramaUpdateTime;
            _panoramaUpdateTime = Globals.System.GetTimeMS();
            if (Index == Globals.Me.CurrentMap)
            {
                if (_panoramaIntensity != 1)
                {
                    _panoramaIntensity += (ecTime / 2000f);
                    if (_panoramaIntensity > 1)
                    {
                        _panoramaIntensity = 1;
                    }
                }
            }
            else
            {
                if (_panoramaIntensity != 0)
                {
                    _panoramaIntensity -= (ecTime / 2000f);
                    if (_panoramaIntensity < 0)
                    {
                        _panoramaIntensity = 0;
                    }
                }
            }
            GameTexture imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, Panorama);
            if (imageTex != null)
            {
                GameGraphics.DrawFullScreenTexture(imageTex, _panoramaIntensity);
            }
        }

        private void DrawOverlayGraphic()
        {
            float ecTime = Globals.System.GetTimeMS() - _overlayUpdateTime;
            _overlayUpdateTime = Globals.System.GetTimeMS();
            if (Index == Globals.Me.CurrentMap)
            {
                if (_overlayIntensity != 1)
                {
                    _overlayIntensity += (ecTime / 2000f);
                    if (_overlayIntensity > 1)
                    {
                        _overlayIntensity = 1;
                    }
                }
            }
            else
            {
                if (_overlayIntensity != 0)
                {
                    _overlayIntensity -= (ecTime / 2000f);
                    if (_overlayIntensity < 0)
                    {
                        _overlayIntensity = 0;
                    }
                }
            }
            GameTexture imageTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image,
                OverlayGraphic);
            if (imageTex != null)
            {
                GameGraphics.DrawFullScreenTexture(imageTex, _overlayIntensity);
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
                    _fogUpdateTime = oldMap._fogUpdateTime;
                    var ratio = ((float) oldMap.FogTransparency / FogTransparency);
                    _curFogIntensity = ratio * oldMap._curFogIntensity;
                    _fogCurrentX = oldMap._fogCurrentX;
                    _fogCurrentY = oldMap._fogCurrentY;
                    if (GetX() > oldMap.GetX())
                    {
                        _fogCurrentX -= (Options.TileWidth * Options.MapWidth) % fogTex.GetWidth();
                    }
                    else if (GetX() < oldMap.GetX())
                    {
                        _fogCurrentX += (Options.TileWidth * Options.MapWidth) % fogTex.GetWidth();
                    }
                    if (GetY() > oldMap.GetY())
                    {
                        _fogCurrentY -= (Options.TileHeight * Options.MapHeight) % fogTex.GetHeight();
                    }
                    else if (GetY() < oldMap.GetY())
                    {
                        _fogCurrentY += (Options.TileHeight * Options.MapHeight) % fogTex.GetHeight();
                    }
                    oldMap._curFogIntensity = 0;
                }
            }

            if (oldMap.Panorama == Panorama)
            {
                _panoramaIntensity = oldMap._panoramaIntensity;
                _panoramaUpdateTime = oldMap._panoramaUpdateTime;
                oldMap._panoramaIntensity = 0;
            }

            if (oldMap.OverlayGraphic == OverlayGraphic)
            {
                _overlayIntensity = oldMap._overlayIntensity;
                _overlayUpdateTime = oldMap._overlayUpdateTime;
                oldMap._overlayIntensity = 0;
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
                                  (1000 - (ActionMsgs[n].TransmittionTimer - Globals.System.GetTimeMS())) / 1000));
                var x = (int) Math.Ceiling(GetX() + ActionMsgs[n].X * Options.TileWidth + ActionMsgs[n].xOffset);
                float textWidth = GameGraphics.Renderer.MeasureText(ActionMsgs[n].msg, GameGraphics.GameFont, 1).X;
                GameGraphics.Renderer.DrawString(ActionMsgs[n].msg, GameGraphics.GameFont, (int) (x) - textWidth / 2f,
                    (int) (y), 1, ActionMsgs[n].clr);

                //Try to remove
                ActionMsgs[n].TryRemove();
            }
        }

        //Events
        public void AddEvent(Event evt)
        {
            if (!LocalEntities.ContainsKey(evt.MyIndex) || LocalEntities[evt.MyIndex].SpawnTime != evt.SpawnTime)
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
            if (Globals.Database.RenderCaching)
            {
                ReleaseRenderTextures();
            }

            foreach (var evt in mEvents)
            {
                evt.Dispose();
            }
            mEvents.Clear();

            if (killentities)
            {
                foreach (var en in Globals.Entities)
                {
                    if (en.Value.CurrentMap == Index && !Globals.Me.Party.Contains(en.Key))
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
            ClearMapAttributes();
            MapRendered = false;
            MapLoaded = false;
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
        public Color clr = new Color();
        public int MapNum = 0;
        public string msg = "";
        public long TransmittionTimer = 0;
        public int X = 0;
        public long xOffset = 0;
        public int Y = 0;

        public ActionMsgInstance(int mapNum, int x, int y, string message, Color color)
        {
            Random rnd = new Random();

            MapNum = mapNum;
            X = x;
            Y = y;
            msg = message;
            clr = color;
            xOffset = rnd.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
            TransmittionTimer = Globals.System.GetTimeMS() + 1000;
        }

        public void TryRemove()
        {
            if (TransmittionTimer <= Globals.System.GetTimeMS())
            {
                MapInstance.Lookup.Get<MapInstance>(MapNum).ActionMsgs.Remove(this);
            }
        }
    }
}
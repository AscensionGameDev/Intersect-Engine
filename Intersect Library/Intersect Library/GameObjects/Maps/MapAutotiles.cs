using System;
using Intersect.Logging;

namespace Intersect.GameObjects.Maps
{
    public class MapAutotiles
    {
        // Autotiles
        public const byte AutoTileInner = 1;
        public const byte AutoTileOuter = 2;
        public const byte AutoTileHorizontal = 3;
        public const byte AutoTileVertical = 4;
        public const byte AutoTileFill = 5;

        // XP Autotiles
        public const byte XPFill = 1;
        public const byte XPInner = 2;
        public const byte XPNw = 3;
        public const byte XPN = 4;
        public const byte XPNe = 5;
        public const byte XPE = 6;
        public const byte XPSe = 7;
        public const byte XPS = 8;
        public const byte XPSw = 9;
        public const byte XPW = 10;

        // Autotile types
        public const byte AutotileNone = 0;
        public const byte AutotileNormal = 1;
        public const byte AutotileFake = 2;
        public const byte AutotileAnim = 3;
        public const byte AutotileCliff = 4;
        public const byte AutotileWaterfall = 5;
        public const byte AutotileXP = 6;
        public const byte AutotileAnimXP = 7;

        // Rendering
        public const byte RenderStateNone = 0;
        public const byte RenderStateNormal = 1;
        public const byte RenderStateAutotile = 2;

        private readonly MapBase _myMap;

        // autotiling
        private static bool LoadedTemplates = false;
        public static PointStruct[] AutoInner = new PointStruct[6];
        public static PointStruct[] AutoNe = new PointStruct[6];
        public static PointStruct[] AutoNw = new PointStruct[6];
        public static PointStruct[] AutoSe = new PointStruct[6];
        public static PointStruct[] AutoSw = new PointStruct[6];

        // XP autotiling
        public static PointStruct[] AutoInnerXP = new PointStruct[6];
        public static PointStruct[] AutoNeXP = new PointStruct[6];
        public static PointStruct[] AutoNwXP = new PointStruct[6];
        public static PointStruct[] AutoSeXP = new PointStruct[6];
        public static PointStruct[] AutoSwXP = new PointStruct[6];
        public static PointStruct[] AutoNXP = new PointStruct[6];
        public static PointStruct[] AutoEXP = new PointStruct[6];
        public static PointStruct[] AutoSXP = new PointStruct[6];
        public static PointStruct[] AutoWXP = new PointStruct[6];
        public static PointStruct[] AutoCXP = new PointStruct[6];

        public AutoTileCls[,] Autotile;

        public MapAutotiles(MapBase map)
        {
            _myMap = map;
            if (!LoadedTemplates)
            {
                InitVXAutotileTemplate();
                InitXPAutotileTemplate();
            }
        }

        private void InitVXAutotileTemplate()
        {
            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInner[1].X = (Int16)Options.TileWidth;
            AutoInner[1].Y = 0;

            // NE - b
            AutoInner[2].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoInner[2].Y = 0;

            // SW - c
            AutoInner[3].X = (Int16)Options.TileWidth;
            AutoInner[3].Y = (Int16)(Options.TileHeight / 2);

            // SE - d
            AutoInner[4].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoInner[4].Y = (Int16)(Options.TileHeight / 2);

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNw[1].X = 0;
            AutoNw[1].Y = (Int16)Options.TileHeight;

            // NE - f
            AutoNw[2].X = (Int16)(Options.TileWidth / 2);
            AutoNw[2].Y = (Int16)Options.TileHeight;

            // SW - g
            AutoNw[3].X = 0;
            AutoNw[3].Y = (Int16)((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // SE - h
            AutoNw[4].X = (Int16)(Options.TileWidth / 2);
            AutoNw[4].Y = (Int16)((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNe[1].X = (Int16)Options.TileWidth;
            AutoNe[1].Y = (Int16)Options.TileHeight;

            // NE - g
            AutoNe[2].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoNe[2].Y = (Int16)Options.TileHeight;

            // SW - k
            AutoNe[3].X = (Int16)Options.TileWidth;
            AutoNe[3].Y = (Int16)((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // SE - l
            AutoNe[4].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoNe[4].Y = (Int16)((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSw[1].X = 0;
            AutoSw[1].Y = (Int16)(2 * Options.TileHeight);

            // NE - n
            AutoSw[2].X = (Int16)(Options.TileWidth / 2);
            AutoSw[2].Y = (Int16)(2 * Options.TileHeight);

            // SW - o
            AutoSw[3].X = 0;
            AutoSw[3].Y = (Int16)((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - p
            AutoSw[4].X = (Int16)(Options.TileWidth / 2);
            AutoSw[4].Y = (Int16)((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSe[1].X = (Int16)Options.TileWidth;
            AutoSe[1].Y = (Int16)(2 * Options.TileHeight);

            // NE - r
            AutoSe[2].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoSe[2].Y = (Int16)(2 * Options.TileHeight);

            // SW - s
            AutoSe[3].X = (Int16)Options.TileWidth;
            AutoSe[3].Y = (Int16)((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - t
            AutoSe[4].X = (Int16)((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoSe[4].Y = (Int16)((2 * Options.TileHeight) + (Options.TileHeight / 2));
        }

        private void InitXPAutotileTemplate()
        {
            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInnerXP[1].X = (Int16)(Options.TileWidth * 2);
            AutoInnerXP[1].Y = 0;

            // NE - b
            AutoInnerXP[2].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoInnerXP[2].Y = 0;

            // SW - c
            AutoInnerXP[3].X = (Int16)(Options.TileWidth * 2);
            AutoInnerXP[3].Y = (Int16)(Options.TileHeight / 2);

            // SE - d
            AutoInnerXP[4].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoInnerXP[4].Y = (Int16)(Options.TileHeight / 2);

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNwXP[1].X = 0;
            AutoNwXP[1].Y = (Int16)Options.TileHeight;

            // NE - f
            AutoNwXP[2].X = (Int16)(Options.TileWidth / 2);
            AutoNwXP[2].Y = (Int16)Options.TileHeight;

            // SW - g
            AutoNwXP[3].X = 0;
            AutoNwXP[3].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // SE - h
            AutoNwXP[4].X = (Int16)(Options.TileWidth / 2);
            AutoNwXP[4].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNeXP[1].X = (Int16)(Options.TileWidth * 2);
            AutoNeXP[1].Y = (Int16)Options.TileHeight;

            // NE - g
            AutoNeXP[2].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoNeXP[2].Y = (Int16)Options.TileHeight;

            // SW - k
            AutoNeXP[3].X = (Int16)(Options.TileWidth * 2);
            AutoNeXP[3].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // SE - l
            AutoNeXP[4].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoNeXP[4].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSwXP[1].X = 0;
            AutoSwXP[1].Y = (Int16)(3 * Options.TileHeight);

            // NE - n
            AutoSwXP[2].X = (Int16)(Options.TileWidth / 2);
            AutoSwXP[2].Y = (Int16)(3 * Options.TileHeight);

            // SW - o
            AutoSwXP[3].X = 0;
            AutoSwXP[3].Y = (Int16)((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - p
            AutoSwXP[4].X = (Int16)(Options.TileWidth / 2);
            AutoSwXP[4].Y = (Int16)((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSeXP[1].X = (Int16)(Options.TileWidth * 2);
            AutoSeXP[1].Y = (Int16)(3 * Options.TileHeight);

            // NE - r
            AutoSeXP[2].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoSeXP[2].Y = (Int16)(3 * Options.TileHeight);

            // SW - s
            AutoSeXP[3].X = (Int16)(Options.TileWidth * 2);
            AutoSeXP[3].Y = (Int16)((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - t
            AutoSeXP[4].X = (Int16)((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoSeXP[4].Y = (Int16)((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // Center Tiles - C
            // NW - A
            AutoCXP[1].X = (Int16)Options.TileWidth;
            AutoCXP[1].Y = (Int16)(Options.TileHeight * 2);

            // NE - B
            AutoCXP[2].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoCXP[2].Y = (Int16)(Options.TileHeight * 2);

            // SW - C
            AutoCXP[3].X = (Int16)Options.TileWidth;
            AutoCXP[3].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - D
            AutoCXP[4].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoCXP[4].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - N (North Horizontal region)
            // NW - E
            AutoNXP[1].X = (Int16)Options.TileWidth;
            AutoNXP[1].Y = (Int16)Options.TileHeight;

            // NE - F
            AutoNXP[2].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoNXP[2].Y = (Int16)Options.TileHeight;

            // SW - G
            AutoNXP[3].X = (Int16)Options.TileWidth;
            AutoNXP[3].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // SE - H
            AutoNXP[4].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoNXP[4].Y = (Int16)(Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - E (East Vertical region)
            // NW - I
            AutoEXP[1].X = (Int16)(Options.TileWidth * 2);
            AutoEXP[1].Y = (Int16)(Options.TileHeight * 2);

            // NE - J
            AutoEXP[2].X = (Int16)((Options.TileWidth * 2) + (Options.TileWidth / 2));
            AutoEXP[2].Y = (Int16)(Options.TileHeight * 2);

            // SW - K
            AutoEXP[3].X = (Int16)(Options.TileWidth * 2);
            AutoEXP[3].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - L
            AutoEXP[4].X = (Int16)((Options.TileWidth * 2) + (Options.TileWidth / 2));
            AutoEXP[4].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - W (West Vertical region)
            // NW - M
            AutoWXP[1].X = 0;
            AutoWXP[1].Y = (Int16)(Options.TileHeight * 2);

            // NE - N
            AutoWXP[2].X = (Int16)(Options.TileWidth / 2);
            AutoWXP[2].Y = (Int16)(Options.TileHeight * 2);

            // SW - O
            AutoWXP[3].X = 0;
            AutoWXP[3].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - P
            AutoWXP[4].X = (Int16)(Options.TileWidth / 2);
            AutoWXP[4].Y = (Int16)((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - S (South Horizontal region)
            // NW - Q
            AutoSXP[1].X = (Int16)(Options.TileWidth);
            AutoSXP[1].Y = (Int16)(Options.TileHeight * 3);

            // NE - R
            AutoSXP[2].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoSXP[2].Y = (Int16)(Options.TileHeight * 3);

            // SW - S
            AutoSXP[3].X = (Int16)Options.TileWidth;
            AutoSXP[3].Y = (Int16)((Options.TileHeight * 3) + (Options.TileHeight / 2));

            // SE - T
            AutoSXP[4].X = (Int16)(Options.TileWidth + (Options.TileWidth / 2));
            AutoSXP[4].Y = (Int16)((Options.TileHeight * 3) + (Options.TileHeight / 2));
        }

        private void CreateFields()
        {
            Autotile = new AutoTileCls[Options.MapWidth, Options.MapHeight];
            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    Autotile[x, y] = new AutoTileCls();
                    for (var i = 0; i < Options.LayerCount; i++)
                    {
                        Autotile[x, y].Layer[i] = new QuarterTileCls()
                        {
                            QuarterTile = new PointStruct[5]
                        };
                    }
                }
            }
        }

        public void InitAutotiles(MapBase[,] surroundingMaps)
        {
            if (Autotile == null)
            {
                CreateFields();
            }
            for (var i = 0; i < Options.LayerCount; i++)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        // calculate the subtile positions and place them
                        CalculateAutotile(x, y, i, surroundingMaps);
                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x, y, i);
                    }
                }
            }
        }

        public bool UpdateAutoTiles(int x, int y, MapBase[,] surroundingMaps)
        {
            var changed = false;
            for (var x1 = x - 1; x1 < x + 2; x1++)
            {
                if (x1 < 0 || x1 >= Options.MapWidth)
                {
                    continue;
                }
                for (var y1 = y - 1; y1 < y + 2; y1++)
                {
                    if (y1 < 0 || y1 >= Options.MapHeight)
                    {
                        continue;
                    }
                    var oldautotile = Autotile[x1, y1].Copy();
                    for (int i = 0; i < Options.LayerCount; i++)
                    {
                        // calculate the subtile positions and place them
                        CalculateAutotile(x1, y1, i, surroundingMaps);
                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x1, y1, i);
                    }
                    if (!Autotile[x1, y1].Equals(oldautotile)) changed = true;
                }
            }
            return changed;
        }

        public void UpdateAutoTiles(int x, int y, int layer, MapBase[,] surroundingMaps)
        {
            for (var x1 = x - 1; x1 < x + 2; x1++)
            {
                if (x1 < 0 || x1 >= Options.MapWidth)
                {
                    continue;
                }
                for (var y1 = y - 1; y1 < y + 2; y1++)
                {
                    if (y1 < 0 || y1 >= Options.MapHeight)
                    {
                        continue;
                    }
                    // calculate the subtile positions and place them
                    CalculateAutotile(x1, y1, layer, surroundingMaps);
                    // cache the rendering state of the tiles and set them
                    CacheRenderState(x1, y1, layer);
                }
            }
        }

        public bool UpdateAutoTile(int x, int y, int layer, MapBase[,] surroundingMaps)
        {
            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return false;
            }
            var oldautotile = Autotile[x, y].Copy();
            // calculate the subtile positions and place them
            CalculateAutotile(x, y, layer, surroundingMaps);
            // cache the rendering state of the tiles and set them
            CacheRenderState(x, y, layer);
            return !Autotile[x, y].Equals(oldautotile);
        }

        public void CacheRenderState(int x, int y, int layerNum)
        {
            // exit out early
            if (x < 0 || x > Options.MapWidth || y < 0 || y > Options.MapHeight)
            {
                return;
            }

            if (_myMap == null)
            {
                Log.Error($"{nameof(_myMap)}=null");
                return;
            }

            if (_myMap.Layers == null)
            {
                Log.Error($"{nameof(_myMap.Layers)}=null");
                return;
            }

            var layer = _myMap.Layers[layerNum];
            if (_myMap.Layers[layerNum].Tiles == null)
            {
                Log.Error($"{nameof(layer.Tiles)}=null");
                return;
            }

            var tile = layer.Tiles[x, y];

            // check if it needs to be rendered as an autotile
            if (tile.Autotile == AutotileNone || tile.Autotile == AutotileFake)
            {
                // default to... default
                Autotile[x, y].Layer[layerNum].RenderState = RenderStateNormal;
                //Autotile[x, y].Layer[layerNum].QuarterTile = null;
            }
            else
            {
                Autotile[x, y].Layer[layerNum].RenderState = RenderStateAutotile;
                // cache tileset positioning
                int quarterNum;
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X = (short)((tile.X * Options.TileWidth) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X);
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y = (short)((tile.Y * Options.TileHeight) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y);
                }
            }
        }

        public void CalculateAutotile(int x, int y, int layerNum, MapBase[,] surroundingMaps)
        {
            // Right, so we//ve split the tile block in to an easy to remember
            // collection of letters. We now need to do the calculations to find
            // out which little lettered block needs to be rendered. We do this
            // by reading the surrounding tiles to check for matches.

            // First we check to make sure an autotile situation is actually there.
            // Then we calculate exactly which situation has arisen.
            // The situations are "inner", "outer", "horizontal", "vertical" and "fill".

            // Exit out if we don//t have an auatotile
            if (_myMap == null)
            {
                Log.Error($"{nameof(_myMap)}=null");
                return;
            }

            if (_myMap.Layers == null)
            {
                Log.Error($"{nameof(_myMap.Layers)}=null");
                return;
            }

            var layer = _myMap.Layers[layerNum];
            if (_myMap.Layers[layerNum].Tiles == null)
            {
                Log.Error($"{nameof(layer.Tiles)}=null");
                return;
            }

            var tile = layer.Tiles[x, y];
            if (tile.Autotile == 0)
            {
                return;
            }

            // Okay, we have autotiling but which one?
            switch (tile.Autotile)
            {
                // Normal or animated - same difference
                case AutotileNormal:
                case AutotileAnim:
                    // North West Quarter
                    CalculateNW_Normal(layerNum, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_Normal(layerNum, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_Normal(layerNum, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_Normal(layerNum, x, y, surroundingMaps);
                    break;

                // Cliff
                case AutotileCliff:
                    // North West Quarter
                    CalculateNW_Cliff(layerNum, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_Cliff(layerNum, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_Cliff(layerNum, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_Cliff(layerNum, x, y, surroundingMaps);
                    break;

                // Waterfalls
                case AutotileWaterfall:
                    // North West Quarter
                    CalculateNW_Waterfall(layerNum, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_Waterfall(layerNum, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_Waterfall(layerNum, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_Waterfall(layerNum, x, y, surroundingMaps);
                    break;

                //Autotile XP
                case AutotileXP:
                case AutotileAnimXP:
                    // North West Quarter
                    CalculateNW_XP(layerNum, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_XP(layerNum, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_XP(layerNum, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_XP(layerNum, x, y, surroundingMaps);
                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_Normal(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[2] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }
            // Horizontal
            if (!tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[2] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Outer
            if (!tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileOuter;
            }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case AutoTileOuter:
                    PlaceAutotile(layerNum, x, y, 1, "a");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Normal(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileOuter;
            }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case AutoTileOuter:
                    PlaceAutotile(layerNum, x, y, 2, "b");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Normal(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }
            // Horizontal
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileOuter;
            }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case AutoTileOuter:
                    PlaceAutotile(layerNum, x, y, 3, "c");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Normal(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileOuter;
            }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case AutoTileOuter:
                    PlaceAutotile(layerNum, x, y, 4, "d");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        // Waterfall autotiling
        public void CalculateNW_Waterfall(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerNum, x, y, 1, "i");
            }
            else
            {
                // Edge
                PlaceAutotile(layerNum, x, y, 1, "e");
            }
        }

        public void CalculateNE_Waterfall(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerNum, x, y, 2, "f");
            }
            else
            {
                // Edge
                PlaceAutotile(layerNum, x, y, 2, "j");
            }
        }

        public void CalculateSW_Waterfall(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerNum, x, y, 3, "k");
            }
            else
            {
                // Edge
                PlaceAutotile(layerNum, x, y, 3, "g");
            }
        }

        public void CalculateSE_Waterfall(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerNum, x, y, 4, "h");
            }
            else
            {
                // Edge
                PlaceAutotile(layerNum, x, y, 4, "l");
            }
        }

        // Clif (f autotiling
        public void CalculateNW_Cliff(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[5];
            byte situation = 1;
            int tileLayer = 0;

            //Check side tile.
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                int i = y - 1;

                while (tileLayer == 0)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) && CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0)
                {
                    if (CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    i++;
                }
            }


            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            //Center
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps) && !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation - Horizontal
            if (!tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[2] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Fill
            if (tmpTile[2] && tmpTile[3])
            {
                situation = AutoTileFill;
            }
            // Inner
            if ((!tmpTile[2] && !tmpTile[3]))
            {
                situation = AutoTileInner;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AutoTileHorizontal;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
                {
                    situation = AutoTileVertical;
                }
                else
                {
                    if (!CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
                    {
                        situation = AutoTileHorizontal;
                    }
                    else
                    {
                        situation = AutoTileFill;
                    }
                }
            }

            if (!tmpTile[2] && tmpTile[3] && tmpTile[1])
            {
                situation = AutoTileInner;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Cliff(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[5];
            byte situation = 1;
            int tileLayer = 0;

            //Check side tile.
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                int i = y - 1;

                while (tileLayer == 0)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) && CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0)
                {
                    if (CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    i++;
                }
            }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            //Center
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps) && !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation - Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileFill;
            }
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AutoTileHorizontal;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
                {
                    situation = AutoTileVertical;
                }
                else
                {
                    if (!CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
                    {
                        situation = AutoTileHorizontal;
                    }
                    else
                    {
                        situation = AutoTileFill;
                    }
                }
            }

            if (!tmpTile[1] && tmpTile[3] && tmpTile[2])
            {
                situation = AutoTileInner;
            }


            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Cliff(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[5];
            byte situation = 1;
            int tileLayer = 0;

            //Check side tile.
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                int i = y - 1;

                while (tileLayer == 0)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) && CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0)
                {
                    if (CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    i++;
                }
            }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            //Center
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps) && !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileFill;
            }
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
                {
                    situation = AutoTileVertical;
                }
                else
                {
                    situation = AutoTileFill;
                }
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Cliff(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[5];
            byte situation = 1;
            int tileLayer = 0;

            //Check side tile.
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                int i = y - 1;

                while (tileLayer == 0)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) && CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0)
                {
                    if (CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    i++;
                }
            }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            //Center
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps) && !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation -  Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileHorizontal;
            }
            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileVertical;
            }
            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AutoTileFill;
            }
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AutoTileInner;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
                {
                    situation = AutoTileVertical;
                }
                else
                {
                    situation = AutoTileFill; situation = AutoTileFill;
                }
            }

            // Actually place the subtile
            switch (situation)
            {
                case AutoTileInner:
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case AutoTileHorizontal:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case AutoTileVertical:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case AutoTileFill:
                    PlaceAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_XP(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerNum, x, y, x + i, y + j, surroundingMaps))
                    {
                        tmpTile[i + 2, j + 2] = true;
                    }
                }
            }

            // Calculate Situations
            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XPSe;
                }
            }
            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XPNe;
                }
            }
            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XPSe;
                }
            }
            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XPSw;
                }
            }
            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPNw;
            }
            // Inner
            if (tmpTile[1, 2] && tmpTile[2, 1] && !tmpTile[1, 1])
            {
                situation = XPInner;
            }
            // Center
            if (tmpTile[1, 1] && tmpTile[2, 1] && tmpTile[1, 2])
            {
                situation = XPFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XPInner:
                    PlaceAutotileXP(layerNum, x, y, 1, "a");
                    break;
                case XPFill:
                    PlaceAutotileXP(layerNum, x, y, 1, "A");
                    break;
                case XPNw:
                    PlaceAutotileXP(layerNum, x, y, 1, "e");
                    break;
                case XPN:
                    PlaceAutotileXP(layerNum, x, y, 1, "E");
                    break;
                case XPNe:
                    PlaceAutotileXP(layerNum, x, y, 1, "i");
                    break;
                case XPE:
                    PlaceAutotileXP(layerNum, x, y, 1, "I");
                    break;
                case XPSe:
                    PlaceAutotileXP(layerNum, x, y, 1, "q");
                    break;
                case XPS:
                    PlaceAutotileXP(layerNum, x, y, 1, "Q");
                    break;
                case XPSw:
                    PlaceAutotileXP(layerNum, x, y, 1, "m");
                    break;
                case XPW:
                    PlaceAutotileXP(layerNum, x, y, 1, "M");
                    break;
            }
        }

        public void CalculateNE_XP(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerNum, x, y, x + i, y + j, surroundingMaps))
                    {
                        tmpTile[i + 2, j + 2] = true;
                    }
                }
            }

            // Calculate Situations
            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XPSe;
                }
            }
            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XPNe;
                }
            }
            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XPSw;
                }
            }
            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XPSe;
                }
            }
            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPNw;
            }
            // Corner Tile
            if (!tmpTile[2, 1] && !tmpTile[3, 2])
            {
                situation = XPNe;
            }
            // Inner
            if (tmpTile[3, 2] && tmpTile[2, 1] && !tmpTile[3, 1])
            {
                situation = XPInner;
            }
            // Center
            if (tmpTile[2, 1] && tmpTile[3, 1] && tmpTile[3, 2])
            {
                situation = XPFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XPInner:
                    PlaceAutotileXP(layerNum, x, y, 2, "b");
                    break;
                case XPFill:
                    PlaceAutotileXP(layerNum, x, y, 2, "B");
                    break;
                case XPNw:
                    PlaceAutotileXP(layerNum, x, y, 2, "f");
                    break;
                case XPN:
                    PlaceAutotileXP(layerNum, x, y, 2, "F");
                    break;
                case XPNe:
                    PlaceAutotileXP(layerNum, x, y, 2, "j");
                    break;
                case XPE:
                    PlaceAutotileXP(layerNum, x, y, 2, "J");
                    break;
                case XPSe:
                    PlaceAutotileXP(layerNum, x, y, 2, "r");
                    break;
                case XPS:
                    PlaceAutotileXP(layerNum, x, y, 2, "R");
                    break;
                case XPSw:
                    PlaceAutotileXP(layerNum, x, y, 2, "n");
                    break;
                case XPW:
                    PlaceAutotileXP(layerNum, x, y, 2, "N");
                    break;
            }
        }

        public void CalculateSW_XP(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerNum, x, y, x + i, y + j, surroundingMaps))
                    {
                        tmpTile[i + 2, j + 2] = true;
                    }
                }
            }

            // Calculate Situations
            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XPNe;
                }
            }
            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XPSe;
                }
            }
            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XPSe;
                }
            }
            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XPSw;
                }
            }
            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPNw;
            }
            // Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPSw;
            }
            // Inner
            if (tmpTile[1, 2] && tmpTile[2, 3] && !tmpTile[1, 3])
            {
                situation = XPInner;
            }
            // Center
            if (tmpTile[1, 2] && tmpTile[1, 3] && tmpTile[2, 3])
            {
                situation = XPFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XPInner:
                    PlaceAutotileXP(layerNum, x, y, 3, "c");
                    break;
                case XPFill:
                    PlaceAutotileXP(layerNum, x, y, 3, "C");
                    break;
                case XPNw:
                    PlaceAutotileXP(layerNum, x, y, 3, "g");
                    break;
                case XPN:
                    PlaceAutotileXP(layerNum, x, y, 3, "G");
                    break;
                case XPNe:
                    PlaceAutotileXP(layerNum, x, y, 3, "k");
                    break;
                case XPE:
                    PlaceAutotileXP(layerNum, x, y, 3, "K");
                    break;
                case XPSe:
                    PlaceAutotileXP(layerNum, x, y, 3, "s");
                    break;
                case XPS:
                    PlaceAutotileXP(layerNum, x, y, 3, "S");
                    break;
                case XPSw:
                    PlaceAutotileXP(layerNum, x, y, 3, "o");
                    break;
                case XPW:
                    PlaceAutotileXP(layerNum, x, y, 3, "O");
                    break;
            }
        }

        public void CalculateSE_XP(int layerNum, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerNum, x, y, x + i, y + j, surroundingMaps))
                    {
                        tmpTile[i + 2, j + 2] = true;
                    }
                }
            }

            // Calculate Situations
            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XPNe;
                }
            }
            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XPSe;
                }
            }
            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XPSw;
                }
            }
            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XPSe;
                }
            }
            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPNw;
            }
            // Sw Corner
            if (!tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPSw;
            }
            // Ne Corner
            if (!tmpTile[2, 1] && !tmpTile[3, 2])
            {
                situation = XPNe;
            }
            // Corner Tile
            if (!tmpTile[2, 3] && !tmpTile[3, 2])
            {
                situation = XPSe;
            }
            // Inner
            if (tmpTile[3, 2] && tmpTile[2, 3] && !tmpTile[3, 3])
            {
                situation = XPInner;
            }
            // Center
            if (tmpTile[3, 2] && tmpTile[3, 3] && tmpTile[2, 3])
            {
                situation = XPFill;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XPInner:
                    PlaceAutotileXP(layerNum, x, y, 4, "d");
                    break;
                case XPFill:
                    PlaceAutotileXP(layerNum, x, y, 4, "D");
                    break;
                case XPNw:
                    PlaceAutotileXP(layerNum, x, y, 4, "h");
                    break;
                case XPN:
                    PlaceAutotileXP(layerNum, x, y, 4, "H");
                    break;
                case XPNe:
                    PlaceAutotileXP(layerNum, x, y, 4, "l");
                    break;
                case XPE:
                    PlaceAutotileXP(layerNum, x, y, 4, "L");
                    break;
                case XPSe:
                    PlaceAutotileXP(layerNum, x, y, 4, "t");
                    break;
                case XPS:
                    PlaceAutotileXP(layerNum, x, y, 4, "T");
                    break;
                case XPSw:
                    PlaceAutotileXP(layerNum, x, y, 4, "p");
                    break;
                case XPW:
                    PlaceAutotileXP(layerNum, x, y, 4, "P");
                    break;
            }
        }

        public bool CheckTileMatch(int layerNum, int x1, int y1, int x2, int y2, MapBase[,] surroundingMaps)
        {
            Tile targetTile;
            targetTile.TilesetIndex = -1;
            targetTile.X = -1;
            targetTile.Y = -1;
            targetTile.Autotile = 0;

            int gridX = 0;
            int gridY = 0;
            if (x2 < 0)
            {
                gridX = -1;
                x2 += Options.MapWidth;
            }
            if (y2 < 0)
            {
                gridY = -1;
                y2 += Options.MapHeight;
            }
            if (x2 >= Options.MapWidth)
            {
                gridX = 1;
                x2 -= Options.MapWidth;
            }
            if (y2 >= Options.MapHeight)
            {
                gridY = 1;
                y2 -= Options.MapHeight;
            }

            if (surroundingMaps[gridX + 1, gridY + 1] != null)
            {
                var layers = surroundingMaps[gridX + 1, gridY + 1].Layers;
                if (layers == null)
                {
                    Log.Debug($"Layers are null [{gridX}/{gridX + 1},{gridY}/{gridY + 1}]");
                    return false;
                }

                var tiles = layers[layerNum].Tiles;
                if (tiles == null)
                {
                    Log.Debug($"Layer {layerNum}'s tiles are null.");
                    return false;
                }

                targetTile = tiles[x2, y2];
            }
            var sourceTile = _myMap.Layers[layerNum].Tiles[x1, y1];
            if (targetTile.X == -1) return true;
            // fakes ALWAYS return true
            if (targetTile.Autotile == AutotileFake)
            {
                return true;
            }

            // check neighbour is an autotile
            if (targetTile.Autotile == 0)
            {
                return false;
            }

            // check we//re a matching
            if (sourceTile.TilesetIndex != targetTile.TilesetIndex)
            {
                return false;
            }

            // check tiles match
            if (sourceTile.X != targetTile.X)
            {
                return false;
            }

            if (sourceTile.Y != targetTile.Y)
            {
                return false;
            }

            return true;
        }

        public void PlaceAutotile(int layerNum, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            switch (autoTileLetter)
            {
                case "a":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInner[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInner[1].Y;
                    break;
                case "b":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInner[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInner[2].Y;
                    break;
                case "c":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInner[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInner[3].Y;
                    break;
                case "d":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInner[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInner[4].Y;
                    break;
                case "e":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNw[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNw[1].Y;
                    break;
                case "f":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNw[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNw[2].Y;
                    break;
                case "g":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNw[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNw[3].Y;
                    break;
                case "h":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNw[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNw[4].Y;
                    break;
                case "i":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNe[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNe[1].Y;
                    break;
                case "j":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNe[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNe[2].Y;
                    break;
                case "k":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNe[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNe[3].Y;
                    break;
                case "l":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNe[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNe[4].Y;
                    break;
                case "m":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSw[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSw[1].Y;
                    break;
                case "n":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSw[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSw[2].Y;
                    break;
                case "o":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSw[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSw[3].Y;
                    break;
                case "p":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSw[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSw[4].Y;
                    break;
                case "q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSe[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSe[1].Y;
                    break;
                case "r":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSe[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSe[2].Y;
                    break;
                case "s":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSe[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSe[3].Y;
                    break;
                case "t":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSe[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSe[4].Y;
                    break;
            }
        }

        public void PlaceAutotileXP(int layerNum, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            switch (autoTileLetter)
            {
                case "a":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXP[1].Y;
                    break;
                case "b":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXP[2].Y;
                    break;
                case "c":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXP[3].Y;
                    break;
                case "d":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXP[4].Y;
                    break;
                case "e":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXP[1].Y;
                    break;
                case "f":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXP[2].Y;
                    break;
                case "g":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXP[3].Y;
                    break;
                case "h":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXP[4].Y;
                    break;
                case "i":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXP[1].Y;
                    break;
                case "j":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXP[2].Y;
                    break;
                case "k":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXP[3].Y;
                    break;
                case "l":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXP[4].Y;
                    break;
                case "m":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXP[1].Y;
                    break;
                case "n":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXP[2].Y;
                    break;
                case "o":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXP[3].Y;
                    break;
                case "p":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXP[4].Y;
                    break;
                case "q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXP[1].Y;
                    break;
                case "r":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXP[2].Y;
                    break;
                case "s":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXP[3].Y;
                    break;
                case "t":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXP[4].Y;
                    break;

                //XP Additional Templates
                case "A":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCXP[1].Y;
                    break;
                case "B":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCXP[2].Y;
                    break;
                case "C":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCXP[3].Y;
                    break;
                case "D":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCXP[4].Y;
                    break;
                case "E":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNXP[1].Y;
                    break;
                case "F":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNXP[2].Y;
                    break;
                case "G":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNXP[3].Y;
                    break;
                case "H":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNXP[4].Y;
                    break;
                case "I":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoEXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoEXP[1].Y;
                    break;
                case "J":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoEXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoEXP[2].Y;
                    break;
                case "K":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoEXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoEXP[3].Y;
                    break;
                case "L":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoEXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoEXP[4].Y;
                    break;
                case "M":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWXP[1].Y;
                    break;
                case "N":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWXP[2].Y;
                    break;
                case "O":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWXP[3].Y;
                    break;
                case "P":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWXP[4].Y;
                    break;
                case "Q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSXP[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSXP[1].Y;
                    break;
                case "R":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSXP[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSXP[2].Y;
                    break;
                case "S":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSXP[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSXP[3].Y;
                    break;
                case "T":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSXP[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSXP[4].Y;
                    break;
            }
        }
    }

    public struct PointStruct
    {
        public Int16 X;
        public Int16 Y;
    }

    public class QuarterTileCls
    {
        public PointStruct[] QuarterTile = new PointStruct[5];
        public byte RenderState;
    }

    public class AutoTileCls
    {
        public QuarterTileCls[] Layer = new QuarterTileCls[Options.LayerCount + 1];

        public AutoTileCls Copy()
        {
            var autotile = new AutoTileCls();
            for (int i = 0; i < Options.LayerCount; i++)
            {
                autotile.Layer[i] = new QuarterTileCls()
                {
                    RenderState = Layer[i].RenderState
                };
                for (int z = 0; z < 5; z++)
                {
                    autotile.Layer[i].QuarterTile[z] = new PointStruct()
                    {
                        X = Layer[i].QuarterTile[z].X,
                        Y = Layer[i].QuarterTile[z].Y
                    };
                }
            }
            return autotile;
        }

        public bool Equals(AutoTileCls autotile)
        {
            for (int i = 0; i < Options.LayerCount; i++)
            {
                if (autotile.Layer[i].RenderState != Layer[i].RenderState) return false;
                for (int z = 0; z < 5; z++)
                {
                    if (autotile.Layer[i].QuarterTile[z].X != Layer[i].QuarterTile[z].X) return false;
                    if (autotile.Layer[i].QuarterTile[z].Y != Layer[i].QuarterTile[z].Y) return false;
                }
            }
            return true;
        }
    }
}
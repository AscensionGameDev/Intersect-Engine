using System;
using System.Collections.Generic;
using Intersect.Logging;

namespace Intersect.GameObjects.Maps
{

    public class MapAutotiles
    {

        public const byte AUTO_TILE_FILL = 5;

        public const byte AUTO_TILE_HORIZONTAL = 3;

        // Autotiles
        public const byte AUTO_TILE_INNER = 1;

        public const byte AUTO_TILE_OUTER = 2;

        public const byte AUTO_TILE_VERTICAL = 4;

        public const byte AUTOTILE_ANIM = 3;

        public const byte AUTOTILE_ANIM_XP = 7;

        public const byte AUTOTILE_CLIFF = 4;

        public const byte AUTOTILE_FAKE = 2;

        // Autotile types
        public const byte AUTOTILE_NONE = 0;

        public const byte AUTOTILE_NORMAL = 1;

        public const byte AUTOTILE_WATERFALL = 5;

        public const byte AUTOTILE_XP = 6;

        public const byte RENDER_STATE_AUTOTILE = 2;

        // Rendering
        public const byte RENDER_STATE_NONE = 0;

        public const byte RENDER_STATE_NORMAL = 1;

        // XP Autotiles
        public const byte XP_FILL = 1;

        public const byte XP_INNER = 2;

        public const byte XP_NE = 5;

        public const byte XP_NW = 3;

        public const byte XP_SE = 7;

        public const byte XP_SW = 9;

        public const byte XPE = 6;

        public const byte XPN = 4;

        public const byte XPS = 8;

        public const byte XPW = 10;

        public static PointStruct[] AutoCxp = new PointStruct[6];

        public static PointStruct[] AutoExp = new PointStruct[6];

        public static PointStruct[] AutoInner = new PointStruct[6];

        // XP autotiling
        public static PointStruct[] AutoInnerXp = new PointStruct[6];

        public static PointStruct[] AutoNe = new PointStruct[6];

        public static PointStruct[] AutoNeXp = new PointStruct[6];

        public static PointStruct[] AutoNw = new PointStruct[6];

        public static PointStruct[] AutoNwXp = new PointStruct[6];

        public static PointStruct[] AutoNxp = new PointStruct[6];

        public static PointStruct[] AutoSe = new PointStruct[6];

        public static PointStruct[] AutoSeXp = new PointStruct[6];

        public static PointStruct[] AutoSw = new PointStruct[6];

        public static PointStruct[] AutoSwXp = new PointStruct[6];

        public static PointStruct[] AutoSxp = new PointStruct[6];

        public static PointStruct[] AutoWxp = new PointStruct[6];

        // autotiling
        private static bool sLoadedTemplates = false;

        private readonly MapBase mMyMap;

        public Dictionary<string, QuarterTileCls[,]> Layers;

        public MapAutotiles(MapBase map)
        {
            mMyMap = map;
            if (!sLoadedTemplates)
            {
                InitVxAutotileTemplate();
                InitXpAutotileTemplate();
            }
        }

        private void InitVxAutotileTemplate()
        {
            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInner[1].X = (Int16) Options.TileWidth;
            AutoInner[1].Y = 0;

            // NE - b
            AutoInner[2].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoInner[2].Y = 0;

            // SW - c
            AutoInner[3].X = (Int16) Options.TileWidth;
            AutoInner[3].Y = (Int16) (Options.TileHeight / 2);

            // SE - d
            AutoInner[4].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoInner[4].Y = (Int16) (Options.TileHeight / 2);

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNw[1].X = 0;
            AutoNw[1].Y = (Int16) Options.TileHeight;

            // NE - f
            AutoNw[2].X = (Int16) (Options.TileWidth / 2);
            AutoNw[2].Y = (Int16) Options.TileHeight;

            // SW - g
            AutoNw[3].X = 0;
            AutoNw[3].Y = (Int16) (2 * Options.TileHeight - Options.TileHeight / 2);

            // SE - h
            AutoNw[4].X = (Int16) (Options.TileWidth / 2);
            AutoNw[4].Y = (Int16) (2 * Options.TileHeight - Options.TileHeight / 2);

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNe[1].X = (Int16) Options.TileWidth;
            AutoNe[1].Y = (Int16) Options.TileHeight;

            // NE - g
            AutoNe[2].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoNe[2].Y = (Int16) Options.TileHeight;

            // SW - k
            AutoNe[3].X = (Int16) Options.TileWidth;
            AutoNe[3].Y = (Int16) (2 * Options.TileHeight - Options.TileHeight / 2);

            // SE - l
            AutoNe[4].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoNe[4].Y = (Int16) (2 * Options.TileHeight - Options.TileHeight / 2);

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSw[1].X = 0;
            AutoSw[1].Y = (Int16) (2 * Options.TileHeight);

            // NE - n
            AutoSw[2].X = (Int16) (Options.TileWidth / 2);
            AutoSw[2].Y = (Int16) (2 * Options.TileHeight);

            // SW - o
            AutoSw[3].X = 0;
            AutoSw[3].Y = (Int16) (2 * Options.TileHeight + Options.TileHeight / 2);

            // SE - p
            AutoSw[4].X = (Int16) (Options.TileWidth / 2);
            AutoSw[4].Y = (Int16) (2 * Options.TileHeight + Options.TileHeight / 2);

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSe[1].X = (Int16) Options.TileWidth;
            AutoSe[1].Y = (Int16) (2 * Options.TileHeight);

            // NE - r
            AutoSe[2].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoSe[2].Y = (Int16) (2 * Options.TileHeight);

            // SW - s
            AutoSe[3].X = (Int16) Options.TileWidth;
            AutoSe[3].Y = (Int16) (2 * Options.TileHeight + Options.TileHeight / 2);

            // SE - t
            AutoSe[4].X = (Int16) (2 * Options.TileWidth - Options.TileWidth / 2);
            AutoSe[4].Y = (Int16) (2 * Options.TileHeight + Options.TileHeight / 2);
        }

        private void InitXpAutotileTemplate()
        {
            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInnerXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoInnerXp[1].Y = 0;

            // NE - b
            AutoInnerXp[2].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoInnerXp[2].Y = 0;

            // SW - c
            AutoInnerXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoInnerXp[3].Y = (Int16) (Options.TileHeight / 2);

            // SE - d
            AutoInnerXp[4].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoInnerXp[4].Y = (Int16) (Options.TileHeight / 2);

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNwXp[1].X = 0;
            AutoNwXp[1].Y = (Int16) Options.TileHeight;

            // NE - f
            AutoNwXp[2].X = (Int16) (Options.TileWidth / 2);
            AutoNwXp[2].Y = (Int16) Options.TileHeight;

            // SW - g
            AutoNwXp[3].X = 0;
            AutoNwXp[3].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // SE - h
            AutoNwXp[4].X = (Int16) (Options.TileWidth / 2);
            AutoNwXp[4].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNeXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoNeXp[1].Y = (Int16) Options.TileHeight;

            // NE - g
            AutoNeXp[2].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoNeXp[2].Y = (Int16) Options.TileHeight;

            // SW - k
            AutoNeXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoNeXp[3].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // SE - l
            AutoNeXp[4].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoNeXp[4].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSwXp[1].X = 0;
            AutoSwXp[1].Y = (Int16) (3 * Options.TileHeight);

            // NE - n
            AutoSwXp[2].X = (Int16) (Options.TileWidth / 2);
            AutoSwXp[2].Y = (Int16) (3 * Options.TileHeight);

            // SW - o
            AutoSwXp[3].X = 0;
            AutoSwXp[3].Y = (Int16) (3 * Options.TileHeight + Options.TileHeight / 2);

            // SE - p
            AutoSwXp[4].X = (Int16) (Options.TileWidth / 2);
            AutoSwXp[4].Y = (Int16) (3 * Options.TileHeight + Options.TileHeight / 2);

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSeXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoSeXp[1].Y = (Int16) (3 * Options.TileHeight);

            // NE - r
            AutoSeXp[2].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoSeXp[2].Y = (Int16) (3 * Options.TileHeight);

            // SW - s
            AutoSeXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoSeXp[3].Y = (Int16) (3 * Options.TileHeight + Options.TileHeight / 2);

            // SE - t
            AutoSeXp[4].X = (Int16) (2 * Options.TileWidth + Options.TileWidth / 2);
            AutoSeXp[4].Y = (Int16) (3 * Options.TileHeight + Options.TileHeight / 2);

            // Center Tiles - C
            // NW - A
            AutoCxp[1].X = (Int16) Options.TileWidth;
            AutoCxp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - B
            AutoCxp[2].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoCxp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - C
            AutoCxp[3].X = (Int16) Options.TileWidth;
            AutoCxp[3].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // SE - D
            AutoCxp[4].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoCxp[4].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // Outer Tiles - N (North Horizontal region)
            // NW - E
            AutoNxp[1].X = (Int16) Options.TileWidth;
            AutoNxp[1].Y = (Int16) Options.TileHeight;

            // NE - F
            AutoNxp[2].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoNxp[2].Y = (Int16) Options.TileHeight;

            // SW - G
            AutoNxp[3].X = (Int16) Options.TileWidth;
            AutoNxp[3].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // SE - H
            AutoNxp[4].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoNxp[4].Y = (Int16) (Options.TileHeight + Options.TileHeight / 2);

            // Outer Tiles - E (East Vertical region)
            // NW - I
            AutoExp[1].X = (Int16) (Options.TileWidth * 2);
            AutoExp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - J
            AutoExp[2].X = (Int16) (Options.TileWidth * 2 + Options.TileWidth / 2);
            AutoExp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - K
            AutoExp[3].X = (Int16) (Options.TileWidth * 2);
            AutoExp[3].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // SE - L
            AutoExp[4].X = (Int16) (Options.TileWidth * 2 + Options.TileWidth / 2);
            AutoExp[4].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // Outer Tiles - W (West Vertical region)
            // NW - M
            AutoWxp[1].X = 0;
            AutoWxp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - N
            AutoWxp[2].X = (Int16) (Options.TileWidth / 2);
            AutoWxp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - O
            AutoWxp[3].X = 0;
            AutoWxp[3].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // SE - P
            AutoWxp[4].X = (Int16) (Options.TileWidth / 2);
            AutoWxp[4].Y = (Int16) (Options.TileHeight * 2 + Options.TileHeight / 2);

            // Outer Tiles - S (South Horizontal region)
            // NW - Q
            AutoSxp[1].X = (Int16) Options.TileWidth;
            AutoSxp[1].Y = (Int16) (Options.TileHeight * 3);

            // NE - R
            AutoSxp[2].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoSxp[2].Y = (Int16) (Options.TileHeight * 3);

            // SW - S
            AutoSxp[3].X = (Int16) Options.TileWidth;
            AutoSxp[3].Y = (Int16) (Options.TileHeight * 3 + Options.TileHeight / 2);

            // SE - T
            AutoSxp[4].X = (Int16) (Options.TileWidth + Options.TileWidth / 2);
            AutoSxp[4].Y = (Int16) (Options.TileHeight * 3 + Options.TileHeight / 2);
        }

        private void CreateFields()
        {
            Layers = new Dictionary<string, QuarterTileCls[,]>();
            foreach (var layerName in Options.Instance.MapOpts.Layers.All)
            {
                var layer = new QuarterTileCls[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        layer[x, y] = new QuarterTileCls()
                        {
                            QuarterTile = new PointStruct[5]
                        };
                    }
                }
                Layers.Add(layerName, layer);
            }
        }

        public void InitAutotiles(MapBase[,] surroundingMaps)
        {
            lock (mMyMap.MapLock)
            {
                if (Layers == null)
                {
                    CreateFields();
                }

                foreach (var layerName in Options.Instance.MapOpts.Layers.All)
                {
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            // calculate the subtile positions and place them
                            CalculateAutotile(x, y, layerName, surroundingMaps);

                            // cache the rendering state of the tiles and set them
                            CacheRenderState(x, y, layerName);
                        }
                    }
                }
            }
        }

        public bool UpdateAutoTiles(int x, int y, MapBase[,] surroundingMaps)
        {
            var changed = false;
            lock (mMyMap.MapLock)
            {
                foreach (var layer in Options.Instance.MapOpts.Layers.All)
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
                            
                            var oldautotile = Layers[layer][x1, y1].Copy();
                            // calculate the subtile positions and place them
                            CalculateAutotile(x1, y1, layer, surroundingMaps);

                            // cache the rendering state of the tiles and set them
                            CacheRenderState(x1, y1, layer);

                            if (!Layers[layer][x1, y1].Equals(oldautotile))
                            {
                                changed = true;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        public void UpdateAutoTiles(int x, int y, string layerName, MapBase[,] surroundingMaps)
        {
            lock (mMyMap.MapLock)
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
                        CalculateAutotile(x1, y1, layerName, surroundingMaps);

                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x1, y1, layerName);
                    }
                }
            }
        }

        public void UpdateCliffAutotiles(MapBase curMap, string layerName)
        {
            if (!curMap.Layers.ContainsKey(layerName))
            {
                return;
            }

            foreach (var map in curMap.GenerateAutotileGrid())
            {
                if (map != null)
                {
                    for (var x1 = 0; x1 < Options.MapWidth; x1++)
                    {
                        for (var y1 = 0; y1 < Options.MapHeight; y1++)
                        {
                            if (map.Layers[layerName][x1, y1].Autotile == AUTOTILE_CLIFF)
                            {
                                map.Autotiles.CalculateAutotile(x1, y1, layerName, map.GenerateAutotileGrid());
                                map.Autotiles.CacheRenderState(x1, y1, layerName);
                            }
                        }
                    }
                }
            }
        }

        public bool UpdateAutoTile(int x, int y, string layerName, MapBase[,] surroundingMaps)
        {
            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return false;
            }

            var oldautotile = Layers[layerName][x, y].Copy();
            lock (mMyMap.MapLock)
            {
                // calculate the subtile positions and place them
                CalculateAutotile(x, y, layerName, surroundingMaps);

                // cache the rendering state of the tiles and set them
                CacheRenderState(x, y, layerName);
            }

            return !Layers[layerName][x, y].Equals(oldautotile);
        }

        public void CacheRenderState(int x, int y, string layerName)
        {
            // exit out early
            if (x < 0 || x > Options.MapWidth || y < 0 || y > Options.MapHeight)
            {
                return;
            }

            if (mMyMap == null)
            {
                Log.Error($"{nameof(mMyMap)}=null");

                return;
            }

            if (mMyMap.Layers == null)
            {
                Log.Error($"{nameof(mMyMap.Layers)}=null");

                return;
            }

            if (!mMyMap.Layers.ContainsKey(layerName))
            {
                return;
            }

            var layer = mMyMap.Layers[layerName];
            if (mMyMap.Layers[layerName] == null)
            {
                Log.Error($"{nameof(layer)}=null");

                return;
            }

            var tile = layer[x, y];
            var autotile = Layers[layerName][x, y];

            // check if it needs to be rendered as an autotile
            if (tile.Autotile == AUTOTILE_NONE || tile.Autotile == AUTOTILE_FAKE)
            {
                // default to... default
                autotile.RenderState = RENDER_STATE_NORMAL;

                //Autotile[layerName][x, y].QuarterTile = null;
            }
            else
            {
                autotile.RenderState = RENDER_STATE_AUTOTILE;

                // cache tileset positioning
                int quarterNum;
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    autotile.QuarterTile[quarterNum].X = (short) (tile.X * Options.TileWidth + autotile.QuarterTile[quarterNum].X);

                    autotile.QuarterTile[quarterNum].Y = (short) (tile.Y * Options.TileHeight + autotile.QuarterTile[quarterNum].Y);
                }
            }
        }

        public void CalculateAutotile(int x, int y, string layerName, MapBase[,] surroundingMaps)
        {
            // Right, so we//ve split the tile block in to an easy to remember
            // collection of letters. We now need to do the calculations to find
            // out which little lettered block needs to be rendered. We do this
            // by reading the surrounding tiles to check for matches.

            // First we check to make sure an autotile situation is actually there.
            // Then we calculate exactly which situation has arisen.
            // The situations are "inner", "outer", "horizontal", "vertical" and "fill".

            // Exit out if we don//t have an auatotile
            if (mMyMap == null)
            {
                Log.Error($"{nameof(mMyMap)}=null");

                return;
            }

            if (mMyMap.Layers == null)
            {
                Log.Error($"{nameof(mMyMap.Layers)}=null");

                return;
            }

            if (!mMyMap.Layers.ContainsKey(layerName))
            {
                return;
            }

            var layer = mMyMap.Layers[layerName];
            if (mMyMap.Layers[layerName] == null)
            {
                Log.Error($"{nameof(layer)}=null");

                return;
            }

            var tile = layer[x, y];
            if (tile.Autotile == 0)
            {
                return;
            }


            // Okay, we have autotiling but which one?
            switch (tile.Autotile)
            {
                // Normal or animated - same difference
                case AUTOTILE_NORMAL:
                case AUTOTILE_ANIM:
                    // North West Quarter
                    CalculateNW_Normal(layerName, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_Normal(layerName, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_Normal(layerName, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_Normal(layerName, x, y, surroundingMaps);

                    break;

                // Cliff
                case AUTOTILE_CLIFF:

                    var cliffStart = 0;
                    var cliffHeight = CalculateCliffHeight(layerName, x, y, surroundingMaps, out cliffStart);

                    //Calculate cliffStart and cliffHeight of immediately adjacent cliffs
                    var leftCliffStart = 0;
                    var leftCliffHeight = 0;
                    if (CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps))
                    {
                        leftCliffHeight = CalculateCliffHeight(layerName, x - 1, y, surroundingMaps, out leftCliffStart);
                    }

                    var rightCliffStart = 0;
                    var rightCliffHeight = 0;
                    if (CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps))
                    {
                        rightCliffHeight = CalculateCliffHeight(
                            layerName, x + 1, y, surroundingMaps, out rightCliffStart
                        );
                    }

                    var assumeInteriorEast = CheckTileMatch(layerName, x, y, x + 1, cliffStart, surroundingMaps) &&
                                             !CheckTileMatch(layerName, x, y, x + 1, cliffStart - 1, surroundingMaps);

                    var assumeInteriorWest = CheckTileMatch(layerName, x, y, x - 1, cliffStart, surroundingMaps) &&
                                             !CheckTileMatch(layerName, x, y, x - 1, cliffStart - 1, surroundingMaps);

                    var rangeHeight = cliffHeight;

                    var x1 = x - 1;
                    while (x1 > -Options.MapWidth && CheckTileMatch(layerName, x, y, x1, cliffStart, surroundingMaps))
                    {
                        var adjStart = 0;
                        var height = CalculateCliffHeight(layerName, x1, cliffStart, surroundingMaps, out adjStart);
                        if (adjStart == cliffStart)
                        {
                            if (height > rangeHeight)
                            {
                                rangeHeight = height;
                            }
                        }
                        else
                        {
                            break;
                        }

                        x1--;
                    }

                    x1 = x + 1;
                    while (x1 < Options.MapWidth * 2 && CheckTileMatch(layerName, x, y, x1, cliffStart, surroundingMaps))
                    {
                        var adjStart = 0;
                        var height = CalculateCliffHeight(layerName, x1, cliffStart, surroundingMaps, out adjStart);
                        if (adjStart == cliffStart)
                        {
                            if (height > rangeHeight)
                            {
                                rangeHeight = height;
                            }
                        }
                        else
                        {
                            break;
                        }

                        x1++;
                    }

                    //var drawBottom = true;

                    //var lowestCliffBottom = cliffStart + cliffHeight;
                    //if (assumeInteriorEast || assumeInteriorWest)
                    //{
                    //    var x1 = x - 1;
                    //    while (x1 > -Options.MapWidth && CheckTileMatch(layerName, x, y, x1, cliffStart, surroundingMaps))
                    //    {
                    //        var adjStart = 0;
                    //        var height = CalculateCliffHeight(layerName, x1, cliffStart, surroundingMaps, out adjStart);
                    //        if (adjStart + height > lowestCliffBottom && adjStart == cliffStart) lowestCliffBottom = adjStart + height;
                    //        if (adjStart + height > cliffStart + cliffHeight) break;
                    //        x1--;
                    //    }

                    //    if (lowestCliffBottom <= cliffStart + cliffHeight)
                    //    {
                    //        x1 = x + 1;
                    //        while (x1 < Options.MapWidth * 2 && CheckTileMatch(layerName, x, y, x1, cliffStart, surroundingMaps))
                    //        {
                    //            var adjStart = 0;
                    //            var height = CalculateCliffHeight(layerName, x1, cliffStart, surroundingMaps, out adjStart);
                    //            if (adjStart + height > lowestCliffBottom && adjStart == cliffStart) lowestCliffBottom = adjStart + height;
                    //            if (adjStart + height > cliffStart + cliffHeight) break;
                    //            x1++;
                    //        }
                    //    }

                    //    if (lowestCliffBottom > cliffStart + cliffHeight)
                    //    {
                    //        drawBottom = false;
                    //    }
                    //}

                    var drawBottom = !(assumeInteriorEast && rangeHeight > cliffHeight ||
                                       assumeInteriorWest && rangeHeight > cliffHeight);

                    if ((assumeInteriorEast || assumeInteriorWest) && cliffHeight == 1 && cliffStart != y)
                    {
                        drawBottom = true;
                    }

                    // North West Quarter
                    CalculateNW_Cliff(
                        layerName, x, y, surroundingMaps, cliffStart, cliffHeight, leftCliffStart, leftCliffHeight,
                        assumeInteriorWest
                    );

                    // North East Quarter
                    CalculateNE_Cliff(
                        layerName, x, y, surroundingMaps, cliffStart, cliffHeight, rightCliffStart, rightCliffHeight,
                        assumeInteriorEast
                    );

                    // South West Quarter
                    CalculateSW_Cliff(
                        layerName, x, y, surroundingMaps, cliffStart, cliffHeight, leftCliffStart, leftCliffHeight,
                        assumeInteriorWest, drawBottom
                    );

                    // South East Quarter
                    CalculateSE_Cliff(
                        layerName, x, y, surroundingMaps, cliffStart, cliffHeight, rightCliffStart, rightCliffHeight,
                        assumeInteriorEast, drawBottom
                    );

                    break;

                // Waterfalls
                case AUTOTILE_WATERFALL:
                    // North West Quarter
                    CalculateNW_Waterfall(layerName, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_Waterfall(layerName, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_Waterfall(layerName, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_Waterfall(layerName, x, y, surroundingMaps);

                    break;

                //Autotile XP
                case AUTOTILE_XP:
                case AUTOTILE_ANIM_XP:
                    // North West Quarter
                    CalculateNW_XP(layerName, x, y, surroundingMaps);

                    // North East Quarter
                    CalculateNE_XP(layerName, x, y, surroundingMaps);

                    // South West Quarter
                    CalculateSW_XP(layerName, x, y, surroundingMaps);

                    // South East Quarter
                    CalculateSE_XP(layerName, x, y, surroundingMaps);

                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_Normal(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerName, x, y, x - 1, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North
            if (CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // West
            if (CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[2] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            // Horizontal
            if (!tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (tmpTile[2] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Outer
            if (!tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_OUTER;
            }

            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 1, "e");

                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerName, x, y, 1, "a");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 1, "i");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 1, "m");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 1, "q");

                    break;
            }
        }

        public void CalculateNE_Normal(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North East
            if (CheckTileMatch(layerName, x, y, x + 1, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            // Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_OUTER;
            }

            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 2, "j");

                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerName, x, y, 2, "b");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 2, "f");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 2, "r");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 2, "n");

                    break;
            }
        }

        public void CalculateSW_Normal(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South West
            if (CheckTileMatch(layerName, x, y, x - 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // South
            if (CheckTileMatch(layerName, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            // Horizontal
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_OUTER;
            }

            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 3, "o");

                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerName, x, y, 3, "c");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 3, "s");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 3, "g");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 3, "k");

                    break;
            }
        }

        public void CalculateSE_Normal(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerName, x, y, x, y + 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // South East
            if (CheckTileMatch(layerName, x, y, x + 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // East
            if (CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps))
            {
                tmpTile[3] = true;
            }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            // Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_OUTER;
            }

            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 4, "t");

                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerName, x, y, 4, "d");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 4, "p");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 4, "l");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 4, "h");

                    break;
            }
        }

        // Waterfall autotiling
        public void CalculateNW_Waterfall(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerName, x, y, 1, "i");
            }
            else
            {
                // Edge
                PlaceAutotile(layerName, x, y, 1, "e");
            }
        }

        public void CalculateNE_Waterfall(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerName, x, y, 2, "f");
            }
            else
            {
                // Edge
                PlaceAutotile(layerName, x, y, 2, "j");
            }
        }

        public void CalculateSW_Waterfall(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerName, x, y, 3, "k");
            }
            else
            {
                // Edge
                PlaceAutotile(layerName, x, y, 3, "g");
            }
        }

        public void CalculateSE_Waterfall(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps);

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                PlaceAutotile(layerName, x, y, 4, "h");
            }
            else
            {
                // Edge
                PlaceAutotile(layerName, x, y, 4, "l");
            }
        }

        // Cliff autotiling
        public void CalculateNW_Cliff(
            string layerName,
            int x,
            int y,
            MapBase[,] surroundingMaps,
            int cliffStart,
            int cliffHeight,
            int adjacentStart,
            int adjacentHeight,
            bool assumeInterior
        )
        {
            var tmpTile = new bool[5];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerName, x, y, x - 1, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North
            if (CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            //West
            //Check side tile.
            if (adjacentHeight > 0)
            {
                if (cliffStart == adjacentStart)
                {
                    tmpTile[3] = true;
                }
                else if (cliffStart > adjacentStart && cliffStart + cliffHeight <= adjacentStart + adjacentHeight)
                {
                    tmpTile[3] = true;
                }
            }

            //Center
            //if (CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps) &&
            //    !CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            //{
            //    //tmpTile[4] = true;
            //}

            // Calculate Situation - Horizontal
            if (!tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (tmpTile[2] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Fill
            if (tmpTile[2] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            if (situation == AUTO_TILE_VERTICAL && assumeInterior)
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 1, "e");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 1, "i");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 1, "m");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 1, "q");

                    break;
            }
        }

        public void CalculateNE_Cliff(
            string layerName,
            int x,
            int y,
            MapBase[,] surroundingMaps,
            int cliffStart,
            int cliffHeight,
            int adjacentStart,
            int adjacentHeight,
            bool assumeInterior
        )
        {
            var tmpTile = new bool[5];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[1] = true;
            }

            // North East
            if (CheckTileMatch(layerName, x, y, x + 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            //East
            //Check side tile.
            if (adjacentHeight > 0)
            {
                if (cliffStart == adjacentStart)
                {
                    tmpTile[3] = true;
                }
                else if (cliffStart > adjacentStart && cliffStart + cliffHeight <= adjacentStart + adjacentHeight)
                {
                    tmpTile[3] = true;
                }
            }

            //Center
            //if (CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps) && !CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            //{
            //    //tmpTile[4] = true;
            //}

            // Calculate Situation - Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            if (situation == AUTO_TILE_VERTICAL && assumeInterior)
            {
                situation = AUTO_TILE_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 2, "j");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 2, "f");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 2, "r");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 2, "n");

                    break;
            }
        }

        public void CalculateSW_Cliff(
            string layerName,
            int x,
            int y,
            MapBase[,] surroundingMaps,
            int cliffStart,
            int cliffHeight,
            int adjacentStart,
            int adjacentHeight,
            bool assumeInterior,
            bool drawBottom
        )
        {
            var tmpTile = new bool[5];
            byte situation = 1;

            //West
            //Check side tile.
            if (adjacentHeight > 0)
            {
                if (cliffStart == adjacentStart)
                {
                    tmpTile[1] = true;
                }
                else if (cliffStart > adjacentStart && cliffStart + cliffHeight <= adjacentStart + adjacentHeight)
                {
                    tmpTile[1] = true;
                }
            }

            // South West
            if (CheckTileMatch(layerName, x, y, x - 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            // South
            if (CheckTileMatch(layerName, x, y, x, y + 1, surroundingMaps) || !drawBottom)
            {
                tmpTile[3] = true;
            }

            //Center
            if (CheckTileMatch(layerName, x, y, x - 1, y, surroundingMaps) &&
                !CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            //Vertical check for edge of cliff for cliff layering.
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            if (situation == AUTO_TILE_VERTICAL && assumeInterior)
            {
                situation = AUTO_TILE_FILL;
            }

            if (situation == AUTO_TILE_INNER && assumeInterior)
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 3, "o");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 3, "s");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 3, "g");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 3, "k");

                    break;
            }
        }

        public void CalculateSE_Cliff(
            string layerName,
            int x,
            int y,
            MapBase[,] surroundingMaps,
            int cliffStart,
            int cliffHeight,
            int adjacentStart,
            int adjacentHeight,
            bool assumeInterior,
            bool drawBottom
        )
        {
            var tmpTile = new bool[5];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerName, x, y, x, y + 1, surroundingMaps) || !drawBottom)
            {
                tmpTile[1] = true;
            }

            // South East
            if (CheckTileMatch(layerName, x, y, x + 1, y + 1, surroundingMaps))
            {
                tmpTile[2] = true;
            }

            //East
            //Check side tile.
            if (adjacentHeight > 0)
            {
                if (cliffStart == adjacentStart)
                {
                    tmpTile[3] = true;
                }
                else if (cliffStart > adjacentStart && cliffStart + cliffHeight <= adjacentStart + adjacentHeight)
                {
                    tmpTile[3] = true;
                }
            }

            //Center
            if (CheckTileMatch(layerName, x, y, x + 1, y, surroundingMaps) &&
                !CheckTileMatch(layerName, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Vertical
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }

            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }

            // Calculate Situation -  Horizontal
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            if (situation == AUTO_TILE_VERTICAL && assumeInterior)
            {
                situation = AUTO_TILE_FILL;
            }

            if (situation == AUTO_TILE_INNER && assumeInterior)
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerName, x, y, 4, "t");

                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerName, x, y, 4, "p");

                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerName, x, y, 4, "l");

                    break;
                case AUTO_TILE_FILL:
                    PlaceAutotile(layerName, x, y, 4, "h");

                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_XP(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (var j = -1; j < 2; j++)
            {
                for (var i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerName, x, y, x + i, y + j, surroundingMaps))
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
                    situation = XP_SE;
                }
            }

            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XP_NE;
                }
            }

            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SE;
                }
            }

            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SW;
                }
            }

            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XP_NW;
            }

            // Inner
            if (tmpTile[1, 2] && tmpTile[2, 1] && !tmpTile[1, 1])
            {
                situation = XP_INNER;
            }

            // Center
            if (tmpTile[1, 1] && tmpTile[2, 1] && tmpTile[1, 2])
            {
                situation = XP_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XP_INNER:
                    PlaceAutotileXp(layerName, x, y, 1, "a");

                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerName, x, y, 1, "A");

                    break;
                case XP_NW:
                    PlaceAutotileXp(layerName, x, y, 1, "e");

                    break;
                case XPN:
                    PlaceAutotileXp(layerName, x, y, 1, "E");

                    break;
                case XP_NE:
                    PlaceAutotileXp(layerName, x, y, 1, "i");

                    break;
                case XPE:
                    PlaceAutotileXp(layerName, x, y, 1, "I");

                    break;
                case XP_SE:
                    PlaceAutotileXp(layerName, x, y, 1, "q");

                    break;
                case XPS:
                    PlaceAutotileXp(layerName, x, y, 1, "Q");

                    break;
                case XP_SW:
                    PlaceAutotileXp(layerName, x, y, 1, "m");

                    break;
                case XPW:
                    PlaceAutotileXp(layerName, x, y, 1, "M");

                    break;
            }
        }

        public void CalculateNE_XP(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (var j = -1; j < 2; j++)
            {
                for (var i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerName, x, y, x + i, y + j, surroundingMaps))
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
                    situation = XP_SE;
                }
            }

            // Horizontal North
            if (tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XPN;
                if (!tmpTile[3, 2])
                {
                    situation = XP_NE;
                }
            }

            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SW;
                }
            }

            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SE;
                }
            }

            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XP_NW;
            }

            // Corner Tile
            if (!tmpTile[2, 1] && !tmpTile[3, 2])
            {
                situation = XP_NE;
            }

            // Inner
            if (tmpTile[3, 2] && tmpTile[2, 1] && !tmpTile[3, 1])
            {
                situation = XP_INNER;
            }

            // Center
            if (tmpTile[2, 1] && tmpTile[3, 1] && tmpTile[3, 2])
            {
                situation = XP_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XP_INNER:
                    PlaceAutotileXp(layerName, x, y, 2, "b");

                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerName, x, y, 2, "B");

                    break;
                case XP_NW:
                    PlaceAutotileXp(layerName, x, y, 2, "f");

                    break;
                case XPN:
                    PlaceAutotileXp(layerName, x, y, 2, "F");

                    break;
                case XP_NE:
                    PlaceAutotileXp(layerName, x, y, 2, "j");

                    break;
                case XPE:
                    PlaceAutotileXp(layerName, x, y, 2, "J");

                    break;
                case XP_SE:
                    PlaceAutotileXp(layerName, x, y, 2, "r");

                    break;
                case XPS:
                    PlaceAutotileXp(layerName, x, y, 2, "R");

                    break;
                case XP_SW:
                    PlaceAutotileXp(layerName, x, y, 2, "n");

                    break;
                case XPW:
                    PlaceAutotileXp(layerName, x, y, 2, "N");

                    break;
            }
        }

        public void CalculateSW_XP(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (var j = -1; j < 2; j++)
            {
                for (var i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerName, x, y, x + i, y + j, surroundingMaps))
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
                    situation = XP_NE;
                }
            }

            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XP_SE;
                }
            }

            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SE;
                }
            }

            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SW;
                }
            }

            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XP_NW;
            }

            // Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XP_SW;
            }

            // Inner
            if (tmpTile[1, 2] && tmpTile[2, 3] && !tmpTile[1, 3])
            {
                situation = XP_INNER;
            }

            // Center
            if (tmpTile[1, 2] && tmpTile[1, 3] && tmpTile[2, 3])
            {
                situation = XP_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XP_INNER:
                    PlaceAutotileXp(layerName, x, y, 3, "c");

                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerName, x, y, 3, "C");

                    break;
                case XP_NW:
                    PlaceAutotileXp(layerName, x, y, 3, "g");

                    break;
                case XPN:
                    PlaceAutotileXp(layerName, x, y, 3, "G");

                    break;
                case XP_NE:
                    PlaceAutotileXp(layerName, x, y, 3, "k");

                    break;
                case XPE:
                    PlaceAutotileXp(layerName, x, y, 3, "K");

                    break;
                case XP_SE:
                    PlaceAutotileXp(layerName, x, y, 3, "s");

                    break;
                case XPS:
                    PlaceAutotileXp(layerName, x, y, 3, "S");

                    break;
                case XP_SW:
                    PlaceAutotileXp(layerName, x, y, 3, "o");

                    break;
                case XPW:
                    PlaceAutotileXp(layerName, x, y, 3, "O");

                    break;
            }
        }

        public void CalculateSE_XP(string layerName, int x, int y, MapBase[,] surroundingMaps)
        {
            var tmpTile = new bool[4, 4];
            byte situation = 1;

            // Find the tile matches of neighboring 8 tiles
            for (var j = -1; j < 2; j++)
            {
                for (var i = -1; i < 2; i++)
                {
                    if (CheckTileMatch(layerName, x, y, x + i, y + j, surroundingMaps))
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
                    situation = XP_NE;
                }
            }

            // Horizontal South
            if (tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XPS;
                if (!tmpTile[3, 2])
                {
                    situation = XP_SE;
                }
            }

            // Vertical West
            if (!tmpTile[1, 2] && tmpTile[2, 1])
            {
                situation = XPW;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SW;
                }
            }

            // Vertical East
            if (!tmpTile[3, 2] && tmpTile[2, 1])
            {
                situation = XPE;
                if (!tmpTile[2, 3])
                {
                    situation = XP_SE;
                }
            }

            // Top Left Corner Tile
            if (!tmpTile[1, 2] && !tmpTile[2, 1])
            {
                situation = XP_NW;
            }

            // Sw Corner
            if (!tmpTile[1, 2] && !tmpTile[2, 3])
            {
                situation = XP_SW;
            }

            // Ne Corner
            if (!tmpTile[2, 1] && !tmpTile[3, 2])
            {
                situation = XP_NE;
            }

            // Corner Tile
            if (!tmpTile[2, 3] && !tmpTile[3, 2])
            {
                situation = XP_SE;
            }

            // Inner
            if (tmpTile[3, 2] && tmpTile[2, 3] && !tmpTile[3, 3])
            {
                situation = XP_INNER;
            }

            // Center
            if (tmpTile[3, 2] && tmpTile[3, 3] && tmpTile[2, 3])
            {
                situation = XP_FILL;
            }

            // Actually place the subtile
            switch (situation)
            {
                case XP_INNER:
                    PlaceAutotileXp(layerName, x, y, 4, "d");

                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerName, x, y, 4, "D");

                    break;
                case XP_NW:
                    PlaceAutotileXp(layerName, x, y, 4, "h");

                    break;
                case XPN:
                    PlaceAutotileXp(layerName, x, y, 4, "H");

                    break;
                case XP_NE:
                    PlaceAutotileXp(layerName, x, y, 4, "l");

                    break;
                case XPE:
                    PlaceAutotileXp(layerName, x, y, 4, "L");

                    break;
                case XP_SE:
                    PlaceAutotileXp(layerName, x, y, 4, "t");

                    break;
                case XPS:
                    PlaceAutotileXp(layerName, x, y, 4, "T");

                    break;
                case XP_SW:
                    PlaceAutotileXp(layerName, x, y, 4, "p");

                    break;
                case XPW:
                    PlaceAutotileXp(layerName, x, y, 4, "P");

                    break;
            }
        }

        public bool CheckTileMatch(string layerName, int x1, int y1, int x2, int y2, MapBase[,] surroundingMaps)
        {
            Tile targetTile;
            targetTile.TilesetId = Guid.Empty;
            targetTile.X = -1;
            targetTile.Y = -1;
            targetTile.Autotile = 0;

            var gridX = 0;
            var gridY = 0;
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
                if (!layers.ContainsKey(layerName))
                {
                    return true;
                }
                var tiles = layers[layerName];
                targetTile = tiles[x2, y2];
            }

            if (!mMyMap.Layers.ContainsKey(layerName))
            {
                return true;
            }

            var sourceTile = mMyMap.Layers[layerName][x1, y1];
            if (targetTile.X == -1)
            {
                return true;
            }

            // fakes ALWAYS return true
            if (targetTile.Autotile == AUTOTILE_FAKE)
            {
                return true;
            }

            // check neighbour is an autotile
            if (targetTile.Autotile == 0)
            {
                return false;
            }

            // check we//re a matching
            if (sourceTile.TilesetId != targetTile.TilesetId)
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

        private int CalculateCliffHeight(string layerName, int x, int y, MapBase[,] surroundingMaps, out int cliffStart)
        {
            Tile sourceTile;
            sourceTile.TilesetId = Guid.Empty;
            sourceTile.X = -1;
            sourceTile.Y = -1;
            sourceTile.Autotile = 0;

            cliffStart = y;

            var gridX = 0;
            var gridY = 0;
            if (x < 0)
            {
                gridX = -1;
                x += Options.MapWidth;
            }

            if (y < 0)
            {
                gridY = -1;
                y += Options.MapHeight;
            }

            if (x >= Options.MapWidth)
            {
                gridX = 1;
                x -= Options.MapWidth;
            }

            if (y >= Options.MapHeight)
            {
                gridY = 1;
                y -= Options.MapHeight;
            }

            if (surroundingMaps[gridX + 1, gridY + 1] != null)
            {
                var layers = surroundingMaps[gridX + 1, gridY + 1].Layers;
                var tiles = layers[layerName];
                sourceTile = tiles[x, y];
            }

            if (sourceTile.Autotile == (int) AUTOTILE_CLIFF)
            {
                var height = 1;
                var i = y - 1;
                while (i > -Options.MapHeight)
                {
                    if (CheckTileMatch(layerName, x, y, x, i, surroundingMaps))
                    {
                        height++;
                        cliffStart--;
                    }
                    else
                    {
                        break;
                    }

                    i--;
                }

                i = y + 1;
                while (i < Options.MapHeight * 2)
                {
                    if (CheckTileMatch(layerName, x, y, x, i, surroundingMaps))
                    {
                        height++;
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }

                return height;
            }

            return 0;
        }

        public void PlaceAutotile(string layerName, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            var quarterTile = new PointStruct();
            switch (autoTileLetter)
            {
                case "a":
                    quarterTile.X = AutoInner[1].X;
                    quarterTile.Y = AutoInner[1].Y;

                    break;
                case "b":
                    quarterTile.X = AutoInner[2].X;
                    quarterTile.Y = AutoInner[2].Y;

                    break;
                case "c":
                    quarterTile.X = AutoInner[3].X;
                    quarterTile.Y = AutoInner[3].Y;

                    break;
                case "d":
                    quarterTile.X = AutoInner[4].X;
                    quarterTile.Y = AutoInner[4].Y;

                    break;
                case "e":
                    quarterTile.X = AutoNw[1].X;
                    quarterTile.Y = AutoNw[1].Y;

                    break;
                case "f":
                    quarterTile.X = AutoNw[2].X;
                    quarterTile.Y = AutoNw[2].Y;

                    break;
                case "g":
                    quarterTile.X = AutoNw[3].X;
                    quarterTile.Y = AutoNw[3].Y;

                    break;
                case "h":
                    quarterTile.X = AutoNw[4].X;
                    quarterTile.Y = AutoNw[4].Y;

                    break;
                case "i":
                    quarterTile.X = AutoNe[1].X;
                    quarterTile.Y = AutoNe[1].Y;

                    break;
                case "j":
                    quarterTile.X = AutoNe[2].X;
                    quarterTile.Y = AutoNe[2].Y;

                    break;
                case "k":
                    quarterTile.X = AutoNe[3].X;
                    quarterTile.Y = AutoNe[3].Y;

                    break;
                case "l":
                    quarterTile.X = AutoNe[4].X;
                    quarterTile.Y = AutoNe[4].Y;

                    break;
                case "m":
                    quarterTile.X = AutoSw[1].X;
                    quarterTile.Y = AutoSw[1].Y;

                    break;
                case "n":
                    quarterTile.X = AutoSw[2].X;
                    quarterTile.Y = AutoSw[2].Y;

                    break;
                case "o":
                    quarterTile.X = AutoSw[3].X;
                    quarterTile.Y = AutoSw[3].Y;

                    break;
                case "p":
                    quarterTile.X = AutoSw[4].X;
                    quarterTile.Y = AutoSw[4].Y;

                    break;
                case "q":
                    quarterTile.X = AutoSe[1].X;
                    quarterTile.Y = AutoSe[1].Y;

                    break;
                case "r":
                    quarterTile.X = AutoSe[2].X;
                    quarterTile.Y = AutoSe[2].Y;

                    break;
                case "s":
                    quarterTile.X = AutoSe[3].X;
                    quarterTile.Y = AutoSe[3].Y;

                    break;
                case "t":
                    quarterTile.X = AutoSe[4].X;
                    quarterTile.Y = AutoSe[4].Y;

                    break;
            }
            Layers[layerName][x, y].QuarterTile[tileQuarter] = quarterTile;
        }

        public void PlaceAutotileXp(string layerName, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            var quarterTile = new PointStruct();
            switch (autoTileLetter)
            {
                case "a":
                    quarterTile.X = AutoInnerXp[1].X;
                    quarterTile.Y = AutoInnerXp[1].Y;

                    break;
                case "b":
                    quarterTile.X = AutoInnerXp[2].X;
                    quarterTile.Y = AutoInnerXp[2].Y;

                    break;
                case "c":
                    quarterTile.X = AutoInnerXp[3].X;
                    quarterTile.Y = AutoInnerXp[3].Y;

                    break;
                case "d":
                    quarterTile.X = AutoInnerXp[4].X;
                    quarterTile.Y = AutoInnerXp[4].Y;

                    break;
                case "e":
                    quarterTile.X = AutoNwXp[1].X;
                    quarterTile.Y = AutoNwXp[1].Y;

                    break;
                case "f":
                    quarterTile.X = AutoNwXp[2].X;
                    quarterTile.Y = AutoNwXp[2].Y;

                    break;
                case "g":
                    quarterTile.X = AutoNwXp[3].X;
                    quarterTile.Y = AutoNwXp[3].Y;

                    break;
                case "h":
                    quarterTile.X = AutoNwXp[4].X;
                    quarterTile.Y = AutoNwXp[4].Y;

                    break;
                case "i":
                    quarterTile.X = AutoNeXp[1].X;
                    quarterTile.Y = AutoNeXp[1].Y;

                    break;
                case "j":
                    quarterTile.X = AutoNeXp[2].X;
                    quarterTile.Y = AutoNeXp[2].Y;

                    break;
                case "k":
                    quarterTile.X = AutoNeXp[3].X;
                    quarterTile.Y = AutoNeXp[3].Y;

                    break;
                case "l":
                    quarterTile.X = AutoNeXp[4].X;
                    quarterTile.Y = AutoNeXp[4].Y;

                    break;
                case "m":
                    quarterTile.X = AutoSwXp[1].X;
                    quarterTile.Y = AutoSwXp[1].Y;

                    break;
                case "n":
                    quarterTile.X = AutoSwXp[2].X;
                    quarterTile.Y = AutoSwXp[2].Y;

                    break;
                case "o":
                    quarterTile.X = AutoSwXp[3].X;
                    quarterTile.Y = AutoSwXp[3].Y;

                    break;
                case "p":
                    quarterTile.X = AutoSwXp[4].X;
                    quarterTile.Y = AutoSwXp[4].Y;

                    break;
                case "q":
                    quarterTile.X = AutoSeXp[1].X;
                    quarterTile.Y = AutoSeXp[1].Y;

                    break;
                case "r":
                    quarterTile.X = AutoSeXp[2].X;
                    quarterTile.Y = AutoSeXp[2].Y;

                    break;
                case "s":
                    quarterTile.X = AutoSeXp[3].X;
                    quarterTile.Y = AutoSeXp[3].Y;

                    break;
                case "t":
                    quarterTile.X = AutoSeXp[4].X;
                    quarterTile.Y = AutoSeXp[4].Y;

                    break;

                //XP Additional Templates
                case "A":
                    quarterTile.X = AutoCxp[1].X;
                    quarterTile.Y = AutoCxp[1].Y;

                    break;
                case "B":
                    quarterTile.X = AutoCxp[2].X;
                    quarterTile.Y = AutoCxp[2].Y;

                    break;
                case "C":
                    quarterTile.X = AutoCxp[3].X;
                    quarterTile.Y = AutoCxp[3].Y;

                    break;
                case "D":
                    quarterTile.X = AutoCxp[4].X;
                    quarterTile.Y = AutoCxp[4].Y;

                    break;
                case "E":
                    quarterTile.X = AutoNxp[1].X;
                    quarterTile.Y = AutoNxp[1].Y;

                    break;
                case "F":
                    quarterTile.X = AutoNxp[2].X;
                    quarterTile.Y = AutoNxp[2].Y;

                    break;
                case "G":
                    quarterTile.X = AutoNxp[3].X;
                    quarterTile.Y = AutoNxp[3].Y;

                    break;
                case "H":
                    quarterTile.X = AutoNxp[4].X;
                    quarterTile.Y = AutoNxp[4].Y;

                    break;
                case "I":
                    quarterTile.X = AutoExp[1].X;
                    quarterTile.Y = AutoExp[1].Y;

                    break;
                case "J":
                    quarterTile.X = AutoExp[2].X;
                    quarterTile.Y = AutoExp[2].Y;

                    break;
                case "K":
                    quarterTile.X = AutoExp[3].X;
                    quarterTile.Y = AutoExp[3].Y;

                    break;
                case "L":
                    quarterTile.X = AutoExp[4].X;
                    quarterTile.Y = AutoExp[4].Y;

                    break;
                case "M":
                    quarterTile.X = AutoWxp[1].X;
                    quarterTile.Y = AutoWxp[1].Y;

                    break;
                case "N":
                    quarterTile.X = AutoWxp[2].X;
                    quarterTile.Y = AutoWxp[2].Y;

                    break;
                case "O":
                    quarterTile.X = AutoWxp[3].X;
                    quarterTile.Y = AutoWxp[3].Y;

                    break;
                case "P":
                    quarterTile.X = AutoWxp[4].X;
                    quarterTile.Y = AutoWxp[4].Y;

                    break;
                case "Q":
                    quarterTile.X = AutoSxp[1].X;
                    quarterTile.Y = AutoSxp[1].Y;

                    break;
                case "R":
                    quarterTile.X = AutoSxp[2].X;
                    quarterTile.Y = AutoSxp[2].Y;

                    break;
                case "S":
                    quarterTile.X = AutoSxp[3].X;
                    quarterTile.Y = AutoSxp[3].Y;

                    break;
                case "T":
                    quarterTile.X = AutoSxp[4].X;
                    quarterTile.Y = AutoSxp[4].Y;

                    break;
            }
            Layers[layerName][x, y].QuarterTile[tileQuarter] = quarterTile;
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

        public QuarterTileCls Copy()
        {
            var autotile = new QuarterTileCls();

            autotile.RenderState = RenderState;

            for (var z = 0; z < 5; z++)
            {
                autotile.QuarterTile[z] = new PointStruct()
                {
                    X = QuarterTile[z].X,
                    Y = QuarterTile[z].Y
                };
            }

            return autotile;
        }

        public bool Equals(QuarterTileCls quarterTile)
        {
            if (quarterTile.RenderState != RenderState)
            {
                return false;
            }

            for (var z = 0; z < 5; z++)
            {
                if (quarterTile.QuarterTile[z].X != QuarterTile[z].X)
                {
                    return false;
                }

                if (quarterTile.QuarterTile[z].Y != QuarterTile[z].Y)
                {
                    return false;
                }
            }

            return true;
        }

    }


}

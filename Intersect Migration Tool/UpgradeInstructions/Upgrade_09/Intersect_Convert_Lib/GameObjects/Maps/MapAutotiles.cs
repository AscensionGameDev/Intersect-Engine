using System;
using Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Logging;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.GameObjects.Maps
{
    public class MapAutotiles
    {
        // Autotiles
        public const byte AUTO_TILE_INNER = 1;

        public const byte AUTO_TILE_OUTER = 2;
        public const byte AUTO_TILE_HORIZONTAL = 3;
        public const byte AUTO_TILE_VERTICAL = 4;
        public const byte AUTO_TILE_FILL = 5;

        // XP Autotiles
        public const byte XP_FILL = 1;

        public const byte XP_INNER = 2;
        public const byte XP_NW = 3;
        public const byte XPN = 4;
        public const byte XP_NE = 5;
        public const byte XPE = 6;
        public const byte XP_SE = 7;
        public const byte XPS = 8;
        public const byte XP_SW = 9;
        public const byte XPW = 10;

        // Autotile types
        public const byte AUTOTILE_NONE = 0;

        public const byte AUTOTILE_NORMAL = 1;
        public const byte AUTOTILE_FAKE = 2;
        public const byte AUTOTILE_ANIM = 3;
        public const byte AUTOTILE_CLIFF = 4;
        public const byte AUTOTILE_WATERFALL = 5;
        public const byte AUTOTILE_XP = 6;
        public const byte AUTOTILE_ANIM_XP = 7;

        // Rendering
        public const byte RENDER_STATE_NONE = 0;

        public const byte RENDER_STATE_NORMAL = 1;
        public const byte RENDER_STATE_AUTOTILE = 2;

        // autotiling
        private static bool sLoadedTemplates = false;

        public static PointStruct[] AutoInner = new PointStruct[6];
        public static PointStruct[] AutoNe = new PointStruct[6];
        public static PointStruct[] AutoNw = new PointStruct[6];
        public static PointStruct[] AutoSe = new PointStruct[6];
        public static PointStruct[] AutoSw = new PointStruct[6];

        // XP autotiling
        public static PointStruct[] AutoInnerXp = new PointStruct[6];

        public static PointStruct[] AutoNeXp = new PointStruct[6];
        public static PointStruct[] AutoNwXp = new PointStruct[6];
        public static PointStruct[] AutoSeXp = new PointStruct[6];
        public static PointStruct[] AutoSwXp = new PointStruct[6];
        public static PointStruct[] AutoNxp = new PointStruct[6];
        public static PointStruct[] AutoExp = new PointStruct[6];
        public static PointStruct[] AutoSxp = new PointStruct[6];
        public static PointStruct[] AutoWxp = new PointStruct[6];
        public static PointStruct[] AutoCxp = new PointStruct[6];

        private readonly MapBase mMyMap;

        public AutoTileCls[,] Autotile;

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
            AutoInner[2].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoInner[2].Y = 0;

            // SW - c
            AutoInner[3].X = (Int16) Options.TileWidth;
            AutoInner[3].Y = (Int16) (Options.TileHeight / 2);

            // SE - d
            AutoInner[4].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
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
            AutoNw[3].Y = (Int16) ((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // SE - h
            AutoNw[4].X = (Int16) (Options.TileWidth / 2);
            AutoNw[4].Y = (Int16) ((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNe[1].X = (Int16) Options.TileWidth;
            AutoNe[1].Y = (Int16) Options.TileHeight;

            // NE - g
            AutoNe[2].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoNe[2].Y = (Int16) Options.TileHeight;

            // SW - k
            AutoNe[3].X = (Int16) Options.TileWidth;
            AutoNe[3].Y = (Int16) ((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // SE - l
            AutoNe[4].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoNe[4].Y = (Int16) ((2 * Options.TileHeight) - (Options.TileHeight / 2));

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSw[1].X = 0;
            AutoSw[1].Y = (Int16) (2 * Options.TileHeight);

            // NE - n
            AutoSw[2].X = (Int16) (Options.TileWidth / 2);
            AutoSw[2].Y = (Int16) (2 * Options.TileHeight);

            // SW - o
            AutoSw[3].X = 0;
            AutoSw[3].Y = (Int16) ((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - p
            AutoSw[4].X = (Int16) (Options.TileWidth / 2);
            AutoSw[4].Y = (Int16) ((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSe[1].X = (Int16) Options.TileWidth;
            AutoSe[1].Y = (Int16) (2 * Options.TileHeight);

            // NE - r
            AutoSe[2].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoSe[2].Y = (Int16) (2 * Options.TileHeight);

            // SW - s
            AutoSe[3].X = (Int16) Options.TileWidth;
            AutoSe[3].Y = (Int16) ((2 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - t
            AutoSe[4].X = (Int16) ((2 * Options.TileWidth) - (Options.TileWidth / 2));
            AutoSe[4].Y = (Int16) ((2 * Options.TileHeight) + (Options.TileHeight / 2));
        }

        private void InitXpAutotileTemplate()
        {
            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInnerXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoInnerXp[1].Y = 0;

            // NE - b
            AutoInnerXp[2].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoInnerXp[2].Y = 0;

            // SW - c
            AutoInnerXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoInnerXp[3].Y = (Int16) (Options.TileHeight / 2);

            // SE - d
            AutoInnerXp[4].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
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
            AutoNwXp[3].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // SE - h
            AutoNwXp[4].X = (Int16) (Options.TileWidth / 2);
            AutoNwXp[4].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNeXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoNeXp[1].Y = (Int16) Options.TileHeight;

            // NE - g
            AutoNeXp[2].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoNeXp[2].Y = (Int16) Options.TileHeight;

            // SW - k
            AutoNeXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoNeXp[3].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // SE - l
            AutoNeXp[4].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoNeXp[4].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSwXp[1].X = 0;
            AutoSwXp[1].Y = (Int16) (3 * Options.TileHeight);

            // NE - n
            AutoSwXp[2].X = (Int16) (Options.TileWidth / 2);
            AutoSwXp[2].Y = (Int16) (3 * Options.TileHeight);

            // SW - o
            AutoSwXp[3].X = 0;
            AutoSwXp[3].Y = (Int16) ((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - p
            AutoSwXp[4].X = (Int16) (Options.TileWidth / 2);
            AutoSwXp[4].Y = (Int16) ((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSeXp[1].X = (Int16) (Options.TileWidth * 2);
            AutoSeXp[1].Y = (Int16) (3 * Options.TileHeight);

            // NE - r
            AutoSeXp[2].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoSeXp[2].Y = (Int16) (3 * Options.TileHeight);

            // SW - s
            AutoSeXp[3].X = (Int16) (Options.TileWidth * 2);
            AutoSeXp[3].Y = (Int16) ((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // SE - t
            AutoSeXp[4].X = (Int16) ((2 * Options.TileWidth) + (Options.TileWidth / 2));
            AutoSeXp[4].Y = (Int16) ((3 * Options.TileHeight) + (Options.TileHeight / 2));

            // Center Tiles - C
            // NW - A
            AutoCxp[1].X = (Int16) Options.TileWidth;
            AutoCxp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - B
            AutoCxp[2].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoCxp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - C
            AutoCxp[3].X = (Int16) Options.TileWidth;
            AutoCxp[3].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - D
            AutoCxp[4].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoCxp[4].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - N (North Horizontal region)
            // NW - E
            AutoNxp[1].X = (Int16) Options.TileWidth;
            AutoNxp[1].Y = (Int16) Options.TileHeight;

            // NE - F
            AutoNxp[2].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoNxp[2].Y = (Int16) Options.TileHeight;

            // SW - G
            AutoNxp[3].X = (Int16) Options.TileWidth;
            AutoNxp[3].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // SE - H
            AutoNxp[4].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoNxp[4].Y = (Int16) (Options.TileHeight + (Options.TileHeight / 2));

            // Outer Tiles - E (East Vertical region)
            // NW - I
            AutoExp[1].X = (Int16) (Options.TileWidth * 2);
            AutoExp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - J
            AutoExp[2].X = (Int16) ((Options.TileWidth * 2) + (Options.TileWidth / 2));
            AutoExp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - K
            AutoExp[3].X = (Int16) (Options.TileWidth * 2);
            AutoExp[3].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - L
            AutoExp[4].X = (Int16) ((Options.TileWidth * 2) + (Options.TileWidth / 2));
            AutoExp[4].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - W (West Vertical region)
            // NW - M
            AutoWxp[1].X = 0;
            AutoWxp[1].Y = (Int16) (Options.TileHeight * 2);

            // NE - N
            AutoWxp[2].X = (Int16) (Options.TileWidth / 2);
            AutoWxp[2].Y = (Int16) (Options.TileHeight * 2);

            // SW - O
            AutoWxp[3].X = 0;
            AutoWxp[3].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // SE - P
            AutoWxp[4].X = (Int16) (Options.TileWidth / 2);
            AutoWxp[4].Y = (Int16) ((Options.TileHeight * 2) + (Options.TileHeight / 2));

            // Outer Tiles - S (South Horizontal region)
            // NW - Q
            AutoSxp[1].X = (Int16) (Options.TileWidth);
            AutoSxp[1].Y = (Int16) (Options.TileHeight * 3);

            // NE - R
            AutoSxp[2].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoSxp[2].Y = (Int16) (Options.TileHeight * 3);

            // SW - S
            AutoSxp[3].X = (Int16) Options.TileWidth;
            AutoSxp[3].Y = (Int16) ((Options.TileHeight * 3) + (Options.TileHeight / 2));

            // SE - T
            AutoSxp[4].X = (Int16) (Options.TileWidth + (Options.TileWidth / 2));
            AutoSxp[4].Y = (Int16) ((Options.TileHeight * 3) + (Options.TileHeight / 2));
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

            var layer = mMyMap.Layers[layerNum];
            if (mMyMap.Layers[layerNum].Tiles == null)
            {
                Log.Error($"{nameof(layer.Tiles)}=null");
                return;
            }

            var tile = layer.Tiles[x, y];

            // check if it needs to be rendered as an autotile
            if (tile.Autotile == AUTOTILE_NONE || tile.Autotile == AUTOTILE_FAKE)
            {
                // default to... default
                Autotile[x, y].Layer[layerNum].RenderState = RENDER_STATE_NORMAL;
                //Autotile[x, y].Layer[layerNum].QuarterTile = null;
            }
            else
            {
                Autotile[x, y].Layer[layerNum].RenderState = RENDER_STATE_AUTOTILE;
                // cache tileset positioning
                int quarterNum;
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X =
                        (short) ((tile.X * Options.TileWidth) +
                                 Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X);
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y =
                        (short) ((tile.Y * Options.TileHeight) +
                                 Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y);
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

            var layer = mMyMap.Layers[layerNum];
            if (mMyMap.Layers[layerNum].Tiles == null)
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
                case AUTOTILE_NORMAL:
                case AUTOTILE_ANIM:
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
                case AUTOTILE_CLIFF:
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
                case AUTOTILE_WATERFALL:
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
                case AUTOTILE_XP:
                case AUTOTILE_ANIM_XP:
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
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerNum, x, y, 1, "a");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case AUTO_TILE_FILL:
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
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerNum, x, y, 2, "b");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case AUTO_TILE_FILL:
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
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerNum, x, y, 3, "c");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case AUTO_TILE_FILL:
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
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case AUTO_TILE_OUTER:
                    PlaceAutotile(layerNum, x, y, 4, "d");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case AUTO_TILE_FILL:
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

                while (tileLayer == 0 && i > -Options.MapHeight)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) &&
                        CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0 && i < Options.MapHeight * 2)
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
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps) &&
                !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

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
            // Inner
            if ((!tmpTile[2] && !tmpTile[3]))
            {
                situation = AUTO_TILE_INNER;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
                {
                    situation = AUTO_TILE_VERTICAL;
                }
                else
                {
                    if (!CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
                    {
                        situation = AUTO_TILE_HORIZONTAL;
                    }
                    else
                    {
                        situation = AUTO_TILE_FILL;
                    }
                }
            }

            if (!tmpTile[2] && tmpTile[3] && tmpTile[1])
            {
                situation = AUTO_TILE_INNER;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case AUTO_TILE_FILL:
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
                while (tileLayer == 0 && i > -Options.MapHeight)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) &&
                        CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0 && i < Options.MapHeight * 2)
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
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps) &&
                !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

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
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            //Horizontal
            if (tmpTile[4])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
                {
                    situation = AUTO_TILE_VERTICAL;
                }
                else
                {
                    if (!CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
                    {
                        situation = AUTO_TILE_HORIZONTAL;
                    }
                    else
                    {
                        situation = AUTO_TILE_FILL;
                    }
                }
            }

            if (!tmpTile[1] && tmpTile[3] && tmpTile[2])
            {
                situation = AUTO_TILE_INNER;
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case AUTO_TILE_FILL:
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

                while (tileLayer == 0 && i > -Options.MapHeight)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) &&
                        CheckTileMatch(layerNum, x, y, x - 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0 && i < Options.MapHeight * 2)
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
            if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps) &&
                !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_HORIZONTAL;
            }
            // Vertical
            if (!tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_VERTICAL;
            }
            // Fill
            if (tmpTile[1] && tmpTile[3])
            {
                situation = AUTO_TILE_FILL;
            }
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x - 1, y, surroundingMaps))
                {
                    situation = AUTO_TILE_VERTICAL;
                }
                else
                {
                    situation = AUTO_TILE_FILL;
                }
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case AUTO_TILE_FILL:
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

                while (tileLayer == 0 && i > -Options.MapHeight)
                {
                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps))
                    {
                        tileLayer = 1;
                    }

                    if (!CheckTileMatch(layerNum, x, y, x, i, surroundingMaps) &&
                        CheckTileMatch(layerNum, x, y, x + 1, i, surroundingMaps))
                    {
                        tileLayer = 2;
                    }

                    i--;
                }
            }
            else
            {
                int i = y + 1;

                while (tileLayer == 0 && i < Options.MapHeight * 2)
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
            if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps) &&
                !CheckTileMatch(layerNum, x, y, x, y - 1, surroundingMaps))
            {
                tmpTile[4] = true;
            }

            // Calculate Situation -  Horizontal
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
            // Inner
            if (!tmpTile[1] && !tmpTile[3])
            {
                situation = AUTO_TILE_INNER;
            }

            //check for edge of cliff for cliff layering.
            if (tileLayer == 2)
            {
                if (CheckTileMatch(layerNum, x, y, x + 1, y, surroundingMaps))
                {
                    situation = AUTO_TILE_VERTICAL;
                }
                else
                {
                    situation = AUTO_TILE_FILL;
                    situation = AUTO_TILE_FILL;
                }
            }

            // Actually place the subtile
            switch (situation)
            {
                case AUTO_TILE_INNER:
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case AUTO_TILE_HORIZONTAL:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case AUTO_TILE_VERTICAL:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case AUTO_TILE_FILL:
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
                    PlaceAutotileXp(layerNum, x, y, 1, "a");
                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerNum, x, y, 1, "A");
                    break;
                case XP_NW:
                    PlaceAutotileXp(layerNum, x, y, 1, "e");
                    break;
                case XPN:
                    PlaceAutotileXp(layerNum, x, y, 1, "E");
                    break;
                case XP_NE:
                    PlaceAutotileXp(layerNum, x, y, 1, "i");
                    break;
                case XPE:
                    PlaceAutotileXp(layerNum, x, y, 1, "I");
                    break;
                case XP_SE:
                    PlaceAutotileXp(layerNum, x, y, 1, "q");
                    break;
                case XPS:
                    PlaceAutotileXp(layerNum, x, y, 1, "Q");
                    break;
                case XP_SW:
                    PlaceAutotileXp(layerNum, x, y, 1, "m");
                    break;
                case XPW:
                    PlaceAutotileXp(layerNum, x, y, 1, "M");
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
                    PlaceAutotileXp(layerNum, x, y, 2, "b");
                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerNum, x, y, 2, "B");
                    break;
                case XP_NW:
                    PlaceAutotileXp(layerNum, x, y, 2, "f");
                    break;
                case XPN:
                    PlaceAutotileXp(layerNum, x, y, 2, "F");
                    break;
                case XP_NE:
                    PlaceAutotileXp(layerNum, x, y, 2, "j");
                    break;
                case XPE:
                    PlaceAutotileXp(layerNum, x, y, 2, "J");
                    break;
                case XP_SE:
                    PlaceAutotileXp(layerNum, x, y, 2, "r");
                    break;
                case XPS:
                    PlaceAutotileXp(layerNum, x, y, 2, "R");
                    break;
                case XP_SW:
                    PlaceAutotileXp(layerNum, x, y, 2, "n");
                    break;
                case XPW:
                    PlaceAutotileXp(layerNum, x, y, 2, "N");
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
                    PlaceAutotileXp(layerNum, x, y, 3, "c");
                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerNum, x, y, 3, "C");
                    break;
                case XP_NW:
                    PlaceAutotileXp(layerNum, x, y, 3, "g");
                    break;
                case XPN:
                    PlaceAutotileXp(layerNum, x, y, 3, "G");
                    break;
                case XP_NE:
                    PlaceAutotileXp(layerNum, x, y, 3, "k");
                    break;
                case XPE:
                    PlaceAutotileXp(layerNum, x, y, 3, "K");
                    break;
                case XP_SE:
                    PlaceAutotileXp(layerNum, x, y, 3, "s");
                    break;
                case XPS:
                    PlaceAutotileXp(layerNum, x, y, 3, "S");
                    break;
                case XP_SW:
                    PlaceAutotileXp(layerNum, x, y, 3, "o");
                    break;
                case XPW:
                    PlaceAutotileXp(layerNum, x, y, 3, "O");
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
                    PlaceAutotileXp(layerNum, x, y, 4, "d");
                    break;
                case XP_FILL:
                    PlaceAutotileXp(layerNum, x, y, 4, "D");
                    break;
                case XP_NW:
                    PlaceAutotileXp(layerNum, x, y, 4, "h");
                    break;
                case XPN:
                    PlaceAutotileXp(layerNum, x, y, 4, "H");
                    break;
                case XP_NE:
                    PlaceAutotileXp(layerNum, x, y, 4, "l");
                    break;
                case XPE:
                    PlaceAutotileXp(layerNum, x, y, 4, "L");
                    break;
                case XP_SE:
                    PlaceAutotileXp(layerNum, x, y, 4, "t");
                    break;
                case XPS:
                    PlaceAutotileXp(layerNum, x, y, 4, "T");
                    break;
                case XP_SW:
                    PlaceAutotileXp(layerNum, x, y, 4, "p");
                    break;
                case XPW:
                    PlaceAutotileXp(layerNum, x, y, 4, "P");
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
                var tiles = layers[layerNum].Tiles;
                targetTile = tiles[x2, y2];
            }
            var sourceTile = mMyMap.Layers[layerNum].Tiles[x1, y1];
            if (targetTile.X == -1) return true;
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

        public void PlaceAutotileXp(int layerNum, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            switch (autoTileLetter)
            {
                case "a":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXp[1].Y;
                    break;
                case "b":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXp[2].Y;
                    break;
                case "c":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXp[3].Y;
                    break;
                case "d":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoInnerXp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoInnerXp[4].Y;
                    break;
                case "e":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXp[1].Y;
                    break;
                case "f":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXp[2].Y;
                    break;
                case "g":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXp[3].Y;
                    break;
                case "h":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNwXp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNwXp[4].Y;
                    break;
                case "i":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXp[1].Y;
                    break;
                case "j":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXp[2].Y;
                    break;
                case "k":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXp[3].Y;
                    break;
                case "l":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNeXp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNeXp[4].Y;
                    break;
                case "m":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXp[1].Y;
                    break;
                case "n":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXp[2].Y;
                    break;
                case "o":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXp[3].Y;
                    break;
                case "p":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSwXp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSwXp[4].Y;
                    break;
                case "q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXp[1].Y;
                    break;
                case "r":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXp[2].Y;
                    break;
                case "s":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXp[3].Y;
                    break;
                case "t":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSeXp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSeXp[4].Y;
                    break;

                //XP Additional Templates
                case "A":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCxp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCxp[1].Y;
                    break;
                case "B":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCxp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCxp[2].Y;
                    break;
                case "C":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCxp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCxp[3].Y;
                    break;
                case "D":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoCxp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoCxp[4].Y;
                    break;
                case "E":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNxp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNxp[1].Y;
                    break;
                case "F":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNxp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNxp[2].Y;
                    break;
                case "G":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNxp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNxp[3].Y;
                    break;
                case "H":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoNxp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoNxp[4].Y;
                    break;
                case "I":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoExp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoExp[1].Y;
                    break;
                case "J":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoExp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoExp[2].Y;
                    break;
                case "K":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoExp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoExp[3].Y;
                    break;
                case "L":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoExp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoExp[4].Y;
                    break;
                case "M":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWxp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWxp[1].Y;
                    break;
                case "N":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWxp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWxp[2].Y;
                    break;
                case "O":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWxp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWxp[3].Y;
                    break;
                case "P":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoWxp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoWxp[4].Y;
                    break;
                case "Q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSxp[1].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSxp[1].Y;
                    break;
                case "R":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSxp[2].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSxp[2].Y;
                    break;
                case "S":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSxp[3].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSxp[3].Y;
                    break;
                case "T":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].X = AutoSxp[4].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].Y = AutoSxp[4].Y;
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
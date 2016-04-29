using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Library.GameObjects.Maps
{
    public class MapAutotiles
    {
        // Autotiles
        public const byte AutoTileInner = 1;
        public const byte AutoTileOuter = 2;
        public const byte AutoTileHorizontal = 3;
        public const byte AutoTileVertical = 4;
        public const byte AutoTileFill = 5;

        // Autotile types
        public const byte AutotileNone = 0;
        public const byte AutotileNormal = 1;
        public const byte AutotileFake = 2;
        public const byte AutotileAnim = 3;
        public const byte AutotileCliff = 4;
        public const byte AutotileWaterfall = 5;

        // Rendering
        public const byte RenderStateNone = 0;
        public const byte RenderStateNormal = 1;
        public const byte RenderStateAutotile = 2;

        // autotiling
        public PointStruct[] AutoInner = new PointStruct[6];
        public PointStruct[] AutoNw = new PointStruct[6];
        public PointStruct[] AutoNe = new PointStruct[6];
        public PointStruct[] AutoSw = new PointStruct[6];
        public PointStruct[] AutoSe = new PointStruct[6];

        private readonly MapStruct _myMap;
        public AutoTileCls[,] Autotile;

        public MapAutotiles(MapStruct map)
        {
            _myMap = map;
        }

        public void InitAutotiles(Dictionary<int, MapStruct> gameMaps)
        {
            Autotile = new AutoTileCls[Options.MapWidth, Options.MapHeight];


            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    Autotile[x, y] = new AutoTileCls();
                    for (var i = 0; i < Options.LayerCount; i++)
                    {
                        Autotile[x, y].Layer[i] = new QuarterTileCls();
                    }
                }
            }

            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInner[1].X = Options.TileWidth;
            AutoInner[1].Y = 0;

            // NE - b
            AutoInner[2].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoInner[2].Y = 0;

            // SW - c
            AutoInner[3].X = Options.TileWidth;
            AutoInner[3].Y = Options.TileHeight / 2;

            // SE - d
            AutoInner[4].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoInner[4].Y = Options.TileHeight / 2;

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNw[1].X = 0;
            AutoNw[1].Y = Options.TileHeight;

            // NE - f
            AutoNw[2].X = Options.TileWidth / 2;
            AutoNw[2].Y = Options.TileHeight;

            // SW - g
            AutoNw[3].X = 0;
            AutoNw[3].Y = (2 * Options.TileHeight) - (Options.TileHeight / 2);

            // SE - h
            AutoNw[4].X = Options.TileWidth / 2;
            AutoNw[4].Y = (2 * Options.TileHeight) - (Options.TileHeight / 2);

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNe[1].X = Options.TileWidth;
            AutoNe[1].Y = Options.TileHeight;

            // NE - g
            AutoNe[2].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoNe[2].Y = Options.TileHeight;

            // SW - k
            AutoNe[3].X = Options.TileWidth;
            AutoNe[3].Y = (2 * Options.TileHeight) - (Options.TileHeight / 2);

            // SE - l
            AutoNe[4].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoNe[4].Y = (2 * Options.TileHeight) - (Options.TileHeight / 2);

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSw[1].X = 0;
            AutoSw[1].Y = 2 * Options.TileHeight;

            // NE - n
            AutoSw[2].X = Options.TileWidth / 2;
            AutoSw[2].Y = 2 * Options.TileHeight;

            // SW - o
            AutoSw[3].X = 0;
            AutoSw[3].Y = (2 * Options.TileHeight) + (Options.TileHeight / 2);

            // SE - p
            AutoSw[4].X = Options.TileWidth / 2;
            AutoSw[4].Y = (2 * Options.TileHeight) + (Options.TileHeight / 2);

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSe[1].X = Options.TileWidth;
            AutoSe[1].Y = 2 * Options.TileHeight;

            // NE - r
            AutoSe[2].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoSe[2].Y = 2 * Options.TileHeight;

            // SW - s
            AutoSe[3].X = Options.TileWidth;
            AutoSe[3].Y = (2 * Options.TileHeight) + (Options.TileHeight / 2);

            // SE - t
            AutoSe[4].X = (2 * Options.TileWidth) - (Options.TileWidth / 2);
            AutoSe[4].Y = (2 * Options.TileHeight) + (Options.TileHeight / 2);

            for (var i = 0; i < Options.LayerCount; i++)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        // calculate the subtile positions and place them
                        CalculateAutotile(x, y, i, gameMaps);
                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x, y, i);
                    }
                }
            }
        }

        public void UpdateAutoTiles(int x, int y, Dictionary<int, MapStruct> gameMaps)
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
                    for (int i = 0; i < Options.LayerCount; i++)
                    {
                        // calculate the subtile positions and place them
                        CalculateAutotile(x1, y1, i, gameMaps);
                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x1, y1, i);
                    }
                }
            }
        }

        public void UpdateAutoTiles(int x, int y, int layer,  Dictionary<int, MapStruct> gameMaps)
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
                    CalculateAutotile(x1, y1, layer, gameMaps);
                    // cache the rendering state of the tiles and set them
                    CacheRenderState(x1, y1, layer);
                }
            }
        }

        public void CacheRenderState(int x, int y, int layerNum)
        {
            // exit out early
            if (x < 0 || x > Options.MapWidth || y < 0 || y > Options.MapHeight) { return; }

            // check if it needs to be rendered as an autotile
            if (_myMap.Layers[layerNum].Tiles[x, y].Autotile == AutotileNone || _myMap.Layers[layerNum].Tiles[x, y].Autotile == AutotileFake)
            {
                // default to... default
                Autotile[x, y].Layer[layerNum].RenderState = RenderStateNormal;
                Autotile[x, y].Layer[layerNum].QuarterTile = null;
            }
            else
            {
                Autotile[x, y].Layer[layerNum].RenderState = RenderStateAutotile;
                // cache tileset positioning
                int quarterNum;
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X = (_myMap.Layers[layerNum].Tiles[x, y].X * Options.TileWidth) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X;
                    Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y = (_myMap.Layers[layerNum].Tiles[x, y].Y * Options.TileHeight) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y;
                }
            }
        }

        public void CalculateAutotile(int x, int y, int layerNum, Dictionary<int, MapStruct> gameMaps)
        {
            // Right, so we//ve split the tile block in to an easy to remember
            // collection of letters. We now need to do the calculations to find
            // out which little lettered block needs to be rendered. We do this
            // by reading the surrounding tiles to check for matches.

            // First we check to make sure an autotile situation is actually there.
            // Then we calculate exactly which situation has arisen.
            // The situations are "inner", "outer", "horizontal", "vertical" and "fill".

            // Exit out if we don//t have an auatotile
            if (_myMap.Layers[layerNum].Tiles[x, y].Autotile == 0) { return; }

            // Okay, we have autotiling but which one?
            switch (_myMap.Layers[layerNum].Tiles[x, y].Autotile)
            {

                // Normal or animated - same difference
                case AutotileNormal:
                case AutotileAnim:
                    // North West Quarter
                    CalculateNW_Normal(layerNum, x, y, gameMaps);

                    // North East Quarter
                    CalculateNE_Normal(layerNum, x, y, gameMaps);

                    // South West Quarter
                    CalculateSW_Normal(layerNum, x, y, gameMaps);

                    // South East Quarter
                    CalculateSE_Normal(layerNum, x, y, gameMaps);
                    break;

                // Cliff
                case AutotileCliff:
                    // North West Quarter
                    CalculateNW_Cliff(layerNum, x, y, gameMaps);

                    // North East Quarter
                    CalculateNE_Cliff(layerNum, x, y, gameMaps);

                    // South West Quarter
                    CalculateSW_Cliff(layerNum, x, y, gameMaps);

                    // South East Quarter
                    CalculateSE_Cliff(layerNum, x, y, gameMaps);
                    break;

                // Waterfalls
                case AutotileWaterfall:
                    // North West Quarter
                    CalculateNW_Waterfall(layerNum, x, y, gameMaps);

                    // North East Quarter
                    CalculateNE_Waterfall(layerNum, x, y, gameMaps);

                    // South West Quarter
                    CalculateSW_Waterfall(layerNum, x, y, gameMaps);

                    // South East Quarter
                    CalculateSE_Waterfall(layerNum, x, y, gameMaps);
                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_Normal(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1, gameMaps)) { tmpTile[1] = true; }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, gameMaps)) { tmpTile[2] = true; }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = AutoTileInner; }
            // Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Outer
            if (!tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }

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

        public void CalculateNE_Normal(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, gameMaps)) { tmpTile[1] = true; }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1, gameMaps)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = AutoTileOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }

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

        public void CalculateSW_Normal(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps)) { tmpTile[1] = true; }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1, gameMaps)) { tmpTile[2] = true; }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }
            // Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = AutoTileOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }

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

        public void CalculateSE_Normal(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, gameMaps)) { tmpTile[1] = true; }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1, gameMaps)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = AutoTileOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }

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
        public void CalculateNW_Waterfall(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps);

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

        public void CalculateNE_Waterfall(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps);

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

        public void CalculateSW_Waterfall(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps);

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

        public void CalculateSE_Waterfall(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps);

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
        public void CalculateNW_Cliff(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1, gameMaps)) { tmpTile[1] = true; }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, gameMaps)) { tmpTile[2] = true; }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }
            // Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = AutoTileInner; }

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

        public void CalculateNE_Cliff(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1, gameMaps)) { tmpTile[1] = true; }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1, gameMaps)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }

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

        public void CalculateSW_Cliff(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y, gameMaps)) { tmpTile[1] = true; }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1, gameMaps)) { tmpTile[2] = true; }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }

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

        public void CalculateSE_Cliff(int layerNum, int x, int y, Dictionary<int, MapStruct> gameMaps)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1, gameMaps)) { tmpTile[1] = true; }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1, gameMaps)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y, gameMaps)) { tmpTile[3] = true; }

            // Calculate Situation -  Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = AutoTileHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = AutoTileVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = AutoTileFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = AutoTileInner; }

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

        public bool CheckTileMatch(int layerNum, int x1, int y1, int x2, int y2, Dictionary<int, MapStruct> gameMaps)
        {
            Tile targetTile;
            targetTile.TilesetIndex = -1;
            targetTile.X = -1;
            targetTile.Y = -1;
            targetTile.Autotile = 0;
            // if ( it//s off the map ) { set it as autotile and exit out early
            if (x2 < 0 || x2 >= Options.MapWidth || y2 < 0 || y2 >= Options.MapHeight)
            {
                if (((x2 < 0 && y2 < 0)) || (x2 >= Options.MapWidth && y2 >= Options.MapHeight) || (x2 < 0 && y2 >= Options.MapHeight) || (x2 >= Options.MapWidth && y2 < 0))
                {
                    return true;
                }
                MapStruct otherMap;
                if (x2 < 0)
                {
                    if (_myMap.Left > -1 && gameMaps.ContainsKey(_myMap.Left))
                    {
                        otherMap = gameMaps[_myMap.Left];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[Options.MapWidth + x2, y2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (x2 >= Options.MapWidth)
                {
                    if (_myMap.Right > -1 && gameMaps.ContainsKey(_myMap.Right))
                    {
                        otherMap = gameMaps[_myMap.Right];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2 - Options.MapWidth, y2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (y2 < 0)
                {
                    if (_myMap.Up > -1 && gameMaps.ContainsKey(_myMap.Up))
                    {
                        otherMap = gameMaps[_myMap.Up];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2, Options.MapHeight + y2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (y2 >= Options.MapHeight)
                {
                    if (_myMap.Down > -1 && gameMaps.ContainsKey(_myMap.Down))
                    {
                        otherMap = gameMaps[_myMap.Down];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2, y2 - Options.MapHeight];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                targetTile = _myMap.Layers[layerNum].Tiles[x2, y2];
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
            //With Autotile(x, y).Layer(layerNum).QuarterTile(tileQuarter)
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
    }

    public struct PointStruct
    {
        public int X;
        public int Y;
    }

    public class QuarterTileCls
    {
        public PointStruct[] QuarterTile = new PointStruct[5];
        public byte RenderState;
    }

    public class AutoTileCls
    {
        public QuarterTileCls[] Layer = new QuarterTileCls[Options.LayerCount + 1];
    }
}

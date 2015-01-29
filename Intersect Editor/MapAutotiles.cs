using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor
{
    public class MapAutotiles
    {

        // autotiling
        public Point[] autoInner = new Point[6];
        public Point[] autoNW = new Point[6];
        public Point[] autoNE = new Point[6];
        public Point[] autoSW = new Point[6];
        public Point[] autoSE = new Point[6];

        private Map myMap;
        public AutoTileCls[,] Autotile;

        public MapAutotiles(Map map)
        {
            myMap = map;
        }

        public void initAutotiles()
        {
            Autotile = new AutoTileCls[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];


            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    Autotile[x, y] = new AutoTileCls();
                    for (int i = 0; i < Constants.LAYER_COUNT; i++)
                    {
                        Autotile[x, y].Layer[i] = new QuarterTileCls();
                    }
                }
            }

            for (int i = 0; i < 6; i++)
            {
                autoInner[i] = new Point();
                autoNW[i] = new Point();
                autoNE[i] = new Point();
                autoSW[i] = new Point();
                autoSE[i] = new Point();
            }

            // Inner tiles (Top right subtile region)
            // NW - a
            autoInner[1].x = 32;
            autoInner[1].y = 0;

            // NE - b
            autoInner[2].x = 48;
            autoInner[2].y = 0;

            // SW - c
            autoInner[3].x = 32;
            autoInner[3].y = 16;

            // SE - d
            autoInner[4].x = 48;
            autoInner[4].y = 16;

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            autoNW[1].x = 0;
            autoNW[1].y = 32;

            // NE - f
            autoNW[2].x = 16;
            autoNW[2].y = 32;

            // SW - g
            autoNW[3].x = 0;
            autoNW[3].y = 48;

            // SE - h
            autoNW[4].x = 16;
            autoNW[4].y = 48;

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            autoNE[1].x = 32;
            autoNE[1].y = 32;

            // NE - g
            autoNE[2].x = 48;
            autoNE[2].y = 32;

            // SW - k
            autoNE[3].x = 32;
            autoNE[3].y = 48;

            // SE - l
            autoNE[4].x = 48;
            autoNE[4].y = 48;

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            autoSW[1].x = 0;
            autoSW[1].y = 64;

            // NE - n
            autoSW[2].x = 16;
            autoSW[2].y = 64;

            // SW - o
            autoSW[3].x = 0;
            autoSW[3].y = 80;

            // SE - p
            autoSW[4].x = 16;
            autoSW[4].y = 80;

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            autoSE[1].x = 32;
            autoSE[1].y = 64;

            // NE - r
            autoSE[2].x = 48;
            autoSE[2].y = 64;

            // SW - s
            autoSE[3].x = 32;
            autoSE[3].y = 80;

            // SE - t
            autoSE[4].x = 48;
            autoSE[4].y = 80;

            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        // calculate the subtile positions and place them
                        calculateAutotile(x, y, i);
                        // cache the rendering state of the tiles and set them
                        cacheRenderState(x, y, i);
                    }
                }
            }
        }

        public void cacheRenderState(int x, int y, int layerNum)
        {
            int quarterNum;

            // exit out early
            if (x < 0 || x > Constants.MAP_HEIGHT || y < 0 || y > Constants.MAP_HEIGHT) { return; }

            // check if it needs to be rendered as an autotile
            if (myMap.Layers[layerNum].Tiles[x, y].Autotile == Constants.AUTOTILE_NONE || myMap.Layers[layerNum].Tiles[x, y].Autotile == Constants.AUTOTILE_FAKE)
            {
                // default to... default
                Autotile[x, y].Layer[layerNum].renderState = Constants.RENDER_STATE_NORMAL;
            }
            else
            {
                Autotile[x, y].Layer[layerNum].renderState = Constants.RENDER_STATE_AUTOTILE;
                // cache tileset positioning
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    Autotile[x, y].Layer[layerNum].srcX[quarterNum] = (myMap.Layers[layerNum].Tiles[x, y].x * 32) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].x;
                    Autotile[x, y].Layer[layerNum].srcY[quarterNum] = (myMap.Layers[layerNum].Tiles[x, y].y * 32) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].y;
                }
            }
        }

        public void calculateAutotile(int x, int y, int layerNum)
        {
            // Right, so we//ve split the tile block in to an easy to remember
            // collection of letters. We now need to do the calculations to find
            // out which little lettered block needs to be rendered. We do this
            // by reading the surrounding tiles to check for matches.

            // First we check to make sure an autotile situation is actually there.
            // Then we calculate exactly which situation has arisen.
            // The situations are "inner", "outer", "horizontal", "vertical" and "fill".

            // Exit out if we don//t have an auatotile
            if (myMap.Layers[layerNum].Tiles[x, y].Autotile == 0) { return; }

            // Okay, we have autotiling but which one?
            switch (myMap.Layers[layerNum].Tiles[x, y].Autotile)
            {

                // Normal or animated - same difference
                case Constants.AUTOTILE_NORMAL:
                case Constants.AUTOTILE_ANIM:
                    // North West Quarter
                    CalculateNW_Normal(layerNum, x, y);

                    // North East Quarter
                    CalculateNE_Normal(layerNum, x, y);

                    // South West Quarter
                    CalculateSW_Normal(layerNum, x, y);

                    // South East Quarter
                    CalculateSE_Normal(layerNum, x, y);
                    break;

                // Cliff
                case Constants.AUTOTILE_CLIFF:
                    // North West Quarter
                    CalculateNW_Cliff(layerNum, x, y);

                    // North East Quarter
                    CalculateNE_Cliff(layerNum, x, y);

                    // South West Quarter
                    CalculateSW_Cliff(layerNum, x, y);

                    // South East Quarter
                    CalculateSE_Cliff(layerNum, x, y);
                    break;

                // Waterfalls
                case Constants.AUTOTILE_WATERFALL:
                    // North West Quarter
                    CalculateNW_Waterfall(layerNum, x, y);

                    // North East Quarter
                    CalculateNE_Waterfall(layerNum, x, y);

                    // South West Quarter
                    CalculateSW_Waterfall(layerNum, x, y);

                    // South East Quarter
                    CalculateSE_Waterfall(layerNum, x, y);
                    break;
            }
        }

        // Normal autotiling
        public void CalculateNW_Normal(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (checkTileMatch(layerNum, x, y, x - 1, y - 1)) { tmpTile[1] = true; }

            // North
            if (checkTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[2] = true; }

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }
            // Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Outer
            if (!tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_OUTER; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 1, "e");
                    break;
                case Constants.AUTO_OUTER:
                    placeAutotile(layerNum, x, y, 1, "a");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 1, "i");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 1, "m");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Normal(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (checkTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[1] = true; }

            // North East
            if (checkTileMatch(layerNum, x, y, x + 1, y - 1)) { tmpTile[2] = true; }

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_OUTER; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 2, "j");
                    break;
                case Constants.AUTO_OUTER:
                    placeAutotile(layerNum, x, y, 2, "b");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 2, "f");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 2, "r");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Normal(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[1] = true; }

            // South West
            if (checkTileMatch(layerNum, x, y, x - 1, y + 1)) { tmpTile[2] = true; }

            // South
            if (checkTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }
            // Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_OUTER; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 3, "o");
                    break;
                case Constants.AUTO_OUTER:
                    placeAutotile(layerNum, x, y, 3, "c");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 3, "s");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 3, "g");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Normal(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (checkTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[1] = true; }

            // South East
            if (checkTileMatch(layerNum, x, y, x + 1, y + 1)) { tmpTile[2] = true; }

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_OUTER; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 4, "t");
                    break;
                case Constants.AUTO_OUTER:
                    placeAutotile(layerNum, x, y, 4, "d");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 4, "p");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 4, "l");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        // Waterfall autotiling
        public void CalculateNW_Waterfall(int layerNum, int x, int y)
        {
            bool tmpTile = false;

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile = true; }

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                placeAutotile(layerNum, x, y, 1, "i");
            }
            else
            {
                // Edge
                placeAutotile(layerNum, x, y, 1, "e");
            }
        }

        public void CalculateNE_Waterfall(int layerNum, int x, int y)
        {
            bool tmpTile = false;

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile = true; }

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                placeAutotile(layerNum, x, y, 2, "f");
            }
            else
            {
                // Edge
                placeAutotile(layerNum, x, y, 2, "j");
            }
        }

        public void CalculateSW_Waterfall(int layerNum, int x, int y)
        {
            bool tmpTile = false;

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile = true; }

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                placeAutotile(layerNum, x, y, 3, "k");
            }
            else
            {
                // Edge
                placeAutotile(layerNum, x, y, 3, "g");
            }
        }

        public void CalculateSE_Waterfall(int layerNum, int x, int y)
        {
            bool tmpTile = false;

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile = true; }

            // Actually place the subtile
            if (tmpTile)
            {
                // Extended
                placeAutotile(layerNum, x, y, 4, "h");
            }
            else
            {
                // Edge
                placeAutotile(layerNum, x, y, 4, "l");
            }
        }

        // Clif (f autotiling
        public void CalculateNW_Cliff(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (checkTileMatch(layerNum, x, y, x - 1, y - 1)) { tmpTile[1] = true; }

            // North
            if (checkTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[2] = true; }

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }
            // Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 1, "e");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 1, "i");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 1, "m");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Cliff(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (checkTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[1] = true; }

            // North East
            if (checkTileMatch(layerNum, x, y, x + 1, y - 1)) { tmpTile[2] = true; }

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 2, "j");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 2, "f");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 2, "r");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Cliff(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (checkTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[1] = true; }

            // South West
            if (checkTileMatch(layerNum, x, y, x - 1, y + 1)) { tmpTile[2] = true; }

            // South
            if (checkTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 3, "o");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 3, "s");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 3, "g");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Cliff(int layerNum, int x, int y)
        {
            bool[] tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (checkTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[1] = true; }

            // South East
            if (checkTileMatch(layerNum, x, y, x + 1, y + 1)) { tmpTile[2] = true; }

            // East
            if (checkTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation -  Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AUTO_HORIZONTAL; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_VERTICAL; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AUTO_FILL; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AUTO_INNER; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AUTO_INNER:
                    placeAutotile(layerNum, x, y, 4, "t");
                    break;
                case Constants.AUTO_HORIZONTAL:
                    placeAutotile(layerNum, x, y, 4, "p");
                    break;
                case Constants.AUTO_VERTICAL:
                    placeAutotile(layerNum, x, y, 4, "l");
                    break;
                case Constants.AUTO_FILL:
                    placeAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        public bool checkTileMatch(int layerNum, int x1, int y1, int x2, int y2)
        {
            Map otherMap;
            Tile targetTile = null;
            // if ( it//s off the map ) { set it as autotile and exit out early
            if (x2 < 0 || x2 >= Constants.MAP_WIDTH || y2 < 0 || y2 >= Constants.MAP_HEIGHT)
            {
                if (((x2 < 0 && y2 < 0)) || (x2 >= Constants.MAP_WIDTH && y2 >= Constants.MAP_HEIGHT) || (x2 < 0 && y2 >= Constants.MAP_HEIGHT) || (x2 >= Constants.MAP_WIDTH && y2 < 0))
                {
                    return true;
                }
                else
                {
                    if (x2 < 0)
                    {
                        if (myMap.left > -1)
                        {
                            otherMap = Globals.GameMaps[myMap.left];
                            if (otherMap != null)
                            {
                                 targetTile = otherMap.Layers[layerNum].Tiles[Constants.MAP_WIDTH + x2, y2];
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
                    else if (x2 >= Constants.MAP_WIDTH)
                    {
                        if (myMap.right > -1)
                        {
                            otherMap = Globals.GameMaps[myMap.right];
                            if (otherMap != null)
                            {
                                 targetTile = otherMap.Layers[layerNum].Tiles[x2 - Constants.MAP_WIDTH, y2];
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
                        if (myMap.up > -1)
                        {
                            otherMap = Globals.GameMaps[myMap.up];
                            if (otherMap != null)
                            {
                                 targetTile = otherMap.Layers[layerNum].Tiles[x2, Constants.MAP_HEIGHT + y2];
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
                    else if (y2 >= Constants.MAP_HEIGHT)
                    {
                        if (myMap.down > -1)
                        {
                            otherMap = Globals.GameMaps[myMap.down];
                            if (otherMap != null)
                            {
                                 targetTile = otherMap.Layers[layerNum].Tiles[x2, y2 - Constants.MAP_HEIGHT];
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
            }
            else
            {
                 targetTile = myMap.Layers[layerNum].Tiles[x2, y2];
            }
            Tile sourceTile = myMap.Layers[layerNum].Tiles[x1, y1];
            if (targetTile == null) { return true; }
            // fakes ALWAYS return true
            if (targetTile.Autotile == Constants.AUTOTILE_FAKE)
            {
                return true;
            }

            // check neighbour is an autotile
            if (targetTile.Autotile == 0)
            {
                return false;
            }

            // check we//re a matching
            if (sourceTile.tilesetIndex != targetTile.tilesetIndex)
            {
                return false;
            }

            // check tiles match
            if (sourceTile.x != targetTile.x)
            {
                return false;
            }

            if (sourceTile.y != targetTile.y)
            {
                return false;
            }

            return true;
        }


        public void placeAutotile(int layerNum, int x, int y, byte tileQuarter, string autoTileLetter)
        {
            //With Autotile(x, y).Layer(layerNum).QuarterTile(tileQuarter)
            switch (autoTileLetter)
            {
                case "a":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoInner[1].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoInner[1].y;
                    break;
                case "b":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoInner[2].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoInner[2].y;
                    break;
                case "c":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoInner[3].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoInner[3].y;
                    break;
                case "d":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoInner[4].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoInner[4].y;
                    break;
                case "e":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNW[1].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNW[1].y;
                    break;
                case "f":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNW[2].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNW[2].y;
                    break;
                case "g":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNW[3].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNW[3].y;
                    break;
                case "h":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNW[4].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNW[4].y;
                    break;
                case "i":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNE[1].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNE[1].y;
                    break;
                case "j":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNE[2].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNE[2].y;
                    break;
                case "k":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNE[3].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNE[3].y;
                    break;
                case "l":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoNE[4].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoNE[4].y;
                    break;
                case "m":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSW[1].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSW[1].y;
                    break;
                case "n":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSW[2].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSW[2].y;
                    break;
                case "o":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSW[3].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSW[3].y;
                    break;
                case "p":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSW[4].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSW[4].y;
                    break;
                case "q":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSE[1].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSE[1].y;
                    break;
                case "r":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSE[2].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSE[2].y;
                    break;
                case "s":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSE[3].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSE[3].y;
                    break;
                case "t":
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].x = autoSE[4].x;
                    Autotile[x, y].Layer[layerNum].QuarterTile[tileQuarter].y = autoSE[4].y;
                    break;
            }
        }









    }

    public class Point
    {
        public int x;
        public int y;
    }

    public class QuarterTileCls
    {
        public Point[] QuarterTile = new Point[5];
        public byte renderState;
        public long[] srcX = new long[5];
        public long[] srcY = new long[5];
        public QuarterTileCls()
        {
            for (int i = 0; i < 5; i++) {
                QuarterTile[i] = new Point();
            }
        }
    }

    public class AutoTileCls
    {
        public QuarterTileCls[] Layer = new QuarterTileCls[Constants.LAYER_COUNT + 1];
    }
}

namespace Intersect_Editor.Classes
{
    public class MapAutotiles
    {

        // autotiling
        public Point[] AutoInner = new Point[6];
        public Point[] AutoNw = new Point[6];
        public Point[] AutoNe = new Point[6];
        public Point[] AutoSw = new Point[6];
        public Point[] AutoSe = new Point[6];

        private readonly MapStruct _myMap;
        public AutoTileCls[,] Autotile;

        public MapAutotiles(MapStruct map)
        {
            _myMap = map;
        }

        public void InitAutotiles()
        {
            Autotile = new AutoTileCls[Constants.MapWidth, Constants.MapHeight];


            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    Autotile[x, y] = new AutoTileCls();
                    for (var i = 0; i < Constants.LayerCount; i++)
                    {
                        Autotile[x, y].Layer[i] = new QuarterTileCls();
                    }
                }
            }

            for (var i = 0; i < 6; i++)
            {
                AutoInner[i] = new Point();
                AutoNw[i] = new Point();
                AutoNe[i] = new Point();
                AutoSw[i] = new Point();
                AutoSe[i] = new Point();
            }

            // Inner tiles (Top right subtile region)
            // NW - a
            AutoInner[1].X = 32;
            AutoInner[1].Y = 0;

            // NE - b
            AutoInner[2].X = 48;
            AutoInner[2].Y = 0;

            // SW - c
            AutoInner[3].X = 32;
            AutoInner[3].Y = 16;

            // SE - d
            AutoInner[4].X = 48;
            AutoInner[4].Y = 16;

            // Outer Tiles - NW (bottom subtile region)
            // NW - e
            AutoNw[1].X = 0;
            AutoNw[1].Y = 32;

            // NE - f
            AutoNw[2].X = 16;
            AutoNw[2].Y = 32;

            // SW - g
            AutoNw[3].X = 0;
            AutoNw[3].Y = 48;

            // SE - h
            AutoNw[4].X = 16;
            AutoNw[4].Y = 48;

            // Outer Tiles - NE (bottom subtile region)
            // NW - i
            AutoNe[1].X = 32;
            AutoNe[1].Y = 32;

            // NE - g
            AutoNe[2].X = 48;
            AutoNe[2].Y = 32;

            // SW - k
            AutoNe[3].X = 32;
            AutoNe[3].Y = 48;

            // SE - l
            AutoNe[4].X = 48;
            AutoNe[4].Y = 48;

            // Outer Tiles - SW (bottom subtile region)
            // NW - m
            AutoSw[1].X = 0;
            AutoSw[1].Y = 64;

            // NE - n
            AutoSw[2].X = 16;
            AutoSw[2].Y = 64;

            // SW - o
            AutoSw[3].X = 0;
            AutoSw[3].Y = 80;

            // SE - p
            AutoSw[4].X = 16;
            AutoSw[4].Y = 80;

            // Outer Tiles - SE (bottom subtile region)
            // NW - q
            AutoSe[1].X = 32;
            AutoSe[1].Y = 64;

            // NE - r
            AutoSe[2].X = 48;
            AutoSe[2].Y = 64;

            // SW - s
            AutoSe[3].X = 32;
            AutoSe[3].Y = 80;

            // SE - t
            AutoSe[4].X = 48;
            AutoSe[4].Y = 80;

            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        // calculate the subtile positions and place them
                        CalculateAutotile(x, y, i);
                        // cache the rendering state of the tiles and set them
                        CacheRenderState(x, y, i);
                    }
                }
            }
        }

        public void CacheRenderState(int x, int y, int layerNum)
        {
            // exit out early
            if (x < 0 || x > Constants.MapHeight || y < 0 || y > Constants.MapHeight) { return; }

            // check if it needs to be rendered as an autotile
            if (_myMap.Layers[layerNum].Tiles[x, y].Autotile == Constants.AutotileNone || _myMap.Layers[layerNum].Tiles[x, y].Autotile == Constants.AutotileFake)
            {
                // default to... default
                Autotile[x, y].Layer[layerNum].RenderState = Constants.RenderStateNormal;
            }
            else
            {
                Autotile[x, y].Layer[layerNum].RenderState = Constants.RenderStateAutotile;
                // cache tileset positioning
                int quarterNum;
                for (quarterNum = 1; quarterNum < 5; quarterNum++)
                {
                    Autotile[x, y].Layer[layerNum].SrcX[quarterNum] = (_myMap.Layers[layerNum].Tiles[x, y].X * 32) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X;
                    Autotile[x, y].Layer[layerNum].SrcY[quarterNum] = (_myMap.Layers[layerNum].Tiles[x, y].Y * 32) + Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y;
                }
            }
        }

        public void CalculateAutotile(int x, int y, int layerNum)
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
                case Constants.AutotileNormal:
                case Constants.AutotileAnim:
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
                case Constants.AutotileCliff:
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
                case Constants.AutotileWaterfall:
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
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1)) { tmpTile[1] = true; }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[2] = true; }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = Constants.AutoInner; }
            // Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Outer
            if (!tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case Constants.AutoOuter:
                    PlaceAutotile(layerNum, x, y, 1, "a");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Normal(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[1] = true; }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AutoOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case Constants.AutoOuter:
                    PlaceAutotile(layerNum, x, y, 2, "b");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Normal(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[1] = true; }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1)) { tmpTile[2] = true; }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }
            // Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AutoOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case Constants.AutoOuter:
                    PlaceAutotile(layerNum, x, y, 3, "c");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Normal(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[1] = true; }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }
            // Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Outer
            if (tmpTile[1] && !tmpTile[2] && tmpTile[3]) { situation = Constants.AutoOuter; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case Constants.AutoOuter:
                    PlaceAutotile(layerNum, x, y, 4, "d");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        // Waterfall autotiling
        public void CalculateNW_Waterfall(int layerNum, int x, int y)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y);

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

        public void CalculateNE_Waterfall(int layerNum, int x, int y)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y);

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

        public void CalculateSW_Waterfall(int layerNum, int x, int y)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x - 1, y);

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

        public void CalculateSE_Waterfall(int layerNum, int x, int y)
        {
            var tmpTile = CheckTileMatch(layerNum, x, y, x + 1, y);

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
        public void CalculateNW_Cliff(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North West
            if (CheckTileMatch(layerNum, x, y, x - 1, y - 1)) { tmpTile[1] = true; }

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[2] = true; }

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[2] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[2] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }
            // Inner
            if (!tmpTile[2] && !tmpTile[3]) { situation = Constants.AutoInner; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 1, "e");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 1, "i");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 1, "m");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 1, "q");
                    break;
            }
        }

        public void CalculateNE_Cliff(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // North
            if (CheckTileMatch(layerNum, x, y, x, y - 1)) { tmpTile[1] = true; }

            // North East
            if (CheckTileMatch(layerNum, x, y, x + 1, y - 1)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 2, "j");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 2, "f");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 2, "r");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 2, "n");
                    break;
            }
        }

        public void CalculateSW_Cliff(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // West
            if (CheckTileMatch(layerNum, x, y, x - 1, y)) { tmpTile[1] = true; }

            // South West
            if (CheckTileMatch(layerNum, x, y, x - 1, y + 1)) { tmpTile[2] = true; }

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[3] = true; }

            // Calculate Situation - Horizontal
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 3, "o");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 3, "s");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 3, "g");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 3, "k");
                    break;
            }
        }

        public void CalculateSE_Cliff(int layerNum, int x, int y)
        {
            var tmpTile = new bool[4];
            byte situation = 1;

            // South
            if (CheckTileMatch(layerNum, x, y, x, y + 1)) { tmpTile[1] = true; }

            // South East
            if (CheckTileMatch(layerNum, x, y, x + 1, y + 1)) { tmpTile[2] = true; }

            // East
            if (CheckTileMatch(layerNum, x, y, x + 1, y)) { tmpTile[3] = true; }

            // Calculate Situation -  Horizontal
            if (!tmpTile[1] && tmpTile[3]) { situation = Constants.AutoHorizontal; }
            // Vertical
            if (tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoVertical; }
            // Fill
            if (tmpTile[1] && tmpTile[2] && tmpTile[3]) { situation = Constants.AutoFill; }
            // Inner
            if (!tmpTile[1] && !tmpTile[3]) { situation = Constants.AutoInner; }

            // Actually place the subtile
            switch (situation)
            {
                case Constants.AutoInner:
                    PlaceAutotile(layerNum, x, y, 4, "t");
                    break;
                case Constants.AutoHorizontal:
                    PlaceAutotile(layerNum, x, y, 4, "p");
                    break;
                case Constants.AutoVertical:
                    PlaceAutotile(layerNum, x, y, 4, "l");
                    break;
                case Constants.AutoFill:
                    PlaceAutotile(layerNum, x, y, 4, "h");
                    break;
            }
        }

        public bool CheckTileMatch(int layerNum, int x1, int y1, int x2, int y2)
        {
            Tile targetTile = null;
            // if ( it//s off the map ) { set it as autotile and exit out early
            if (x2 < 0 || x2 >= Constants.MapWidth || y2 < 0 || y2 >= Constants.MapHeight)
            {
                if (((x2 < 0 && y2 < 0)) || (x2 >= Constants.MapWidth && y2 >= Constants.MapHeight) || (x2 < 0 && y2 >= Constants.MapHeight) || (x2 >= Constants.MapWidth && y2 < 0))
                {
                    return true;
                }
                MapStruct otherMap;
                if (x2 < 0)
                {
                    if (_myMap.Left > -1)
                    {
                        otherMap = Globals.GameMaps[_myMap.Left];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[Constants.MapWidth + x2, y2];
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
                else if (x2 >= Constants.MapWidth)
                {
                    if (_myMap.Right > -1)
                    {
                        otherMap = Globals.GameMaps[_myMap.Right];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2 - Constants.MapWidth, y2];
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
                    if (_myMap.Up > -1)
                    {
                        otherMap = Globals.GameMaps[_myMap.Up];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2, Constants.MapHeight + y2];
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
                else if (y2 >= Constants.MapHeight)
                {
                    if (_myMap.Down > -1)
                    {
                        otherMap = Globals.GameMaps[_myMap.Down];
                        if (otherMap != null)
                        {
                            targetTile = otherMap.Layers[layerNum].Tiles[x2, y2 - Constants.MapHeight];
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
            if (targetTile == null) { return true; }
            // fakes ALWAYS return true
            if (targetTile.Autotile == Constants.AutotileFake)
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

    public class Point
    {
        public int X;
        public int Y;
    }

    public class QuarterTileCls
    {
        public Point[] QuarterTile = new Point[5];
        public byte RenderState;
        public long[] SrcX = new long[5];
        public long[] SrcY = new long[5];
        public QuarterTileCls()
        {
            for (var i = 0; i < 5; i++) {
                QuarterTile[i] = new Point();
            }
        }
    }

    public class AutoTileCls
    {
        public QuarterTileCls[] Layer = new QuarterTileCls[Constants.LayerCount + 1];
    }
}

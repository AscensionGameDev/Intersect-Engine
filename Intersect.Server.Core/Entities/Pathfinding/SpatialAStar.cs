using System;
using System.Collections.Generic;

namespace Intersect.Server.Entities.Pathfinding
{

    public interface IIndexedObject
    {

        int Index { get; set; }

    }

    public partial class PathNode : IComparer<PathNode>, IIndexedObject
    {

        public static readonly PathNode Comparer = new PathNode(0, 0, false);

        public PathNode(int inX, int inY, bool isWall)
        {
            X = inX;
            Y = inY;
            IsWall = isWall;
        }

        public double G { get; internal set; }

        public double H { get; internal set; }

        public double F { get; internal set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool IsWall { get; set; }

        public int Compare(PathNode x, PathNode y)
        {
            if (x.F < y.F)
            {
                return -1;
            }
            else if (x.F > y.F)
            {
                return 1;
            }

            return 0;
        }

        public int Index { get; set; }

        public void Reset()
        {
            G = 0.0;
            H = 0.0;
            F = 0.0;
            IsWall = false;
        }

    }

    /// <summary>
    ///     Uses about 50 MB for a 1024x1024 grid.
    /// </summary>
    public partial class SpatialAStar
    {

        private static readonly double Sqrt2 = Math.Sqrt(2);

        private PathNode[,] mCameFrom;

        private OpenCloseMap mClosedSet;

        private OpenCloseMap mOpenSet;

        private PriorityQueue<PathNode> mOrderedOpenSet;

        private OpenCloseMap mRuntimeGrid;

        private PathNode[,] mSearchSpace;

        public SpatialAStar(PathNode[,] inGrid)
        {
            SearchSpace = inGrid;
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);
            mSearchSpace = inGrid;
            mClosedSet = new OpenCloseMap(Width, Height);
            mOpenSet = new OpenCloseMap(Width, Height);
            mCameFrom = new PathNode[Width, Height];
            mRuntimeGrid = new OpenCloseMap(Width, Height);
            mOrderedOpenSet = new PriorityQueue<PathNode>(PathNode.Comparer);
        }

        public PathNode[,] SearchSpace { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        protected virtual double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt(
                (inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y)
            );
        }

        protected virtual double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            var diffX = Math.Abs(inStart.X - inEnd.X);
            var diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1:
                    return 1;
                case 2:
                    return Sqrt2;
                case 0:
                    return 0;
                default:
                    throw new ApplicationException();
            }
        }

        //private List<Int64> elapsed = new List<long>();

        /// <summary>
        ///     Returns null, if no path is found. Start- and End-Node are included in returned path. The user context
        ///     is passed to IsWalkable().
        /// </summary>
        public LinkedList<PathNode> Search(Point inStartNode, Point inEndNode, PathNode inUserContext)
        {
            var startNode = mSearchSpace[inStartNode.X, inStartNode.Y];
            var endNode = mSearchSpace[inEndNode.X, inEndNode.Y];

            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            if (startNode == endNode)
            {
                return new LinkedList<PathNode>(new PathNode[] {startNode});
            }

            var neighborNodes = new PathNode[8];

            mClosedSet.Clear();
            mOpenSet.Clear();
            mRuntimeGrid.Clear();
            mOrderedOpenSet.Clear();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    mCameFrom[x, y] = null;
                }
            }

            startNode.G = 0;
            startNode.H = Heuristic(startNode, endNode);
            startNode.F = startNode.H;

            mOpenSet.Add(startNode);
            mOrderedOpenSet.Push(startNode);

            mRuntimeGrid.Add(startNode);

            var nodes = 0;
            PathNode closestNode = null;

            while (!mOpenSet.IsEmpty)
            {
                var x = mOrderedOpenSet.Pop();

                if (x == endNode)
                {
                    // watch.Stop();

                    //elapsed.Add(watch.ElapsedMilliseconds);

                    var result = ReconstructPath(mCameFrom, mCameFrom[endNode.X, endNode.Y]);

                    result.AddLast(endNode);

                    return result;
                }

                mOpenSet.Remove(x);
                mClosedSet.Add(x);

                StoreNeighborNodes(x, neighborNodes);

                for (var i = 0; i < neighborNodes.Length; i++)
                {
                    var y = neighborNodes[i];
                    bool tentativeIsBetter;

                    if (y == null)
                    {
                        continue;
                    }

                    if (y.IsWall)
                    {
                        continue;
                    }

                    if (mClosedSet.Contains(y))
                    {
                        continue;
                    }

                    nodes++;

                    var tentativeGScore = mRuntimeGrid[x].G + NeighborDistance(x, y);
                    var wasAdded = false;

                    if (!mOpenSet.Contains(y))
                    {
                        mOpenSet.Add(y);
                        tentativeIsBetter = true;
                        wasAdded = true;
                    }
                    else if (tentativeGScore < mRuntimeGrid[y].G)
                    {
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = false;
                    }

                    if (tentativeIsBetter)
                    {
                        mCameFrom[y.X, y.Y] = x;

                        if (!mRuntimeGrid.Contains(y))
                        {
                            mRuntimeGrid.Add(y);
                        }

                        mRuntimeGrid[y].G = tentativeGScore;
                        mRuntimeGrid[y].H = Heuristic(y, endNode);
                        mRuntimeGrid[y].F = mRuntimeGrid[y].G + mRuntimeGrid[y].H;

                        if (wasAdded)
                        {
                            mOrderedOpenSet.Push(y);
                        }
                        else
                        {
                            mOrderedOpenSet.Update(y);
                        }

                        if (closestNode == null || closestNode.H > y.H)
                        {
                            closestNode = y;
                        }
                    }
                }
            }

            if (closestNode != null && closestNode.H < startNode.H)
            {
                var result = ReconstructPath(mCameFrom, mCameFrom[closestNode.X, closestNode.Y]);
                result.AddLast(closestNode);
                return result;
            }

            return null;
        }

        private LinkedList<PathNode> ReconstructPath(PathNode[,] cameFrom, PathNode currentNode)
        {
            var result = new LinkedList<PathNode>();

            ReconstructPathRecursive(cameFrom, currentNode, result);

            return result;
        }

        private void ReconstructPathRecursive(PathNode[,] cameFrom, PathNode currentNode, LinkedList<PathNode> result)
        {
            var item = cameFrom[currentNode.X, currentNode.Y];

            if (item != null)
            {
                ReconstructPathRecursive(cameFrom, item, result);

                result.AddLast(currentNode);
            }
            else
            {
                result.AddLast(currentNode);
            }
        }

        private void StoreNeighborNodes(PathNode inAround, IList<PathNode> inNeighbors)
        {
            var x = inAround.X;
            var y = inAround.Y;

            inNeighbors[1] = y > 0 ? mSearchSpace[x, y - 1] : null; // Up
            inNeighbors[3] = x > 0 ? mSearchSpace[x - 1, y] : null; // Left
            inNeighbors[4] = x < Width - 1 ? mSearchSpace[x + 1, y] : null; // Right
            inNeighbors[6] = y < Height - 1 ? mSearchSpace[x, y + 1] : null; // Down

            if (Options.Instance.MapOpts.EnableDiagonalMovement)
            {
                inNeighbors[0] = y > 0 && x > 0 ? mSearchSpace[x - 1, y - 1] : null; // UpLeft
                inNeighbors[2] = y > 0 && x < Width - 1 ? mSearchSpace[x + 1, y - 1] : null; // UpRight
                inNeighbors[5] = y < Height - 1 && x > 0 ? mSearchSpace[x - 1, y + 1] : null; // DownLeft
                inNeighbors[7] = y < Height - 1 && x < Width - 1 ? mSearchSpace[x + 1, y + 1] : null; // DownRight
            }
            else
            {
                inNeighbors[0] = null;
                inNeighbors[2] = null;
                inNeighbors[5] = null;
                inNeighbors[7] = null;
            }
        }

        private partial class OpenCloseMap
        {

            private PathNode[,] mMap;

            public OpenCloseMap(int inWidth, int inHeight)
            {
                mMap = new PathNode[inWidth, inHeight];
                Width = inWidth;
                Height = inHeight;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public int Count { get; private set; }

            public PathNode this[int x, int y] => mMap[x, y];

            public PathNode this[PathNode node] => mMap[node.X, node.Y];

            public bool IsEmpty => Count == 0;

            public void Add(PathNode inValue)
            {
                var item = mMap[inValue.X, inValue.Y];

#if DEBUG
                if (item != null)
                {
                    throw new ApplicationException();
                }
#endif

                Count++;
                mMap[inValue.X, inValue.Y] = inValue;
            }

            public bool Contains(PathNode inValue)
            {
                var item = mMap[inValue.X, inValue.Y];

                if (item == null)
                {
                    return false;
                }

#if DEBUG
                if (!inValue.Equals(item))
                {
                    throw new ApplicationException();
                }
#endif

                return true;
            }

            public void Remove(PathNode inValue)
            {
                var item = mMap[inValue.X, inValue.Y];

#if DEBUG
                if (!inValue.Equals(item))
                {
                    throw new ApplicationException();
                }
#endif

                Count--;
                mMap[inValue.X, inValue.Y] = null;
            }

            public void Clear()
            {
                Count = 0;

                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        mMap[x, y] = null;
                    }
                }
            }

        }

    }

}

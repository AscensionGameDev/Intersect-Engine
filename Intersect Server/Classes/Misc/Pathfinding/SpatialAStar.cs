using System;
using System.Collections.Generic;
using Intersect.Server.Classes.Misc.Pathfinding;

namespace Intersect.Server.Classes.Misc
{
    public interface IIndexedObject
    {
        int Index { get; set; }
    }

    public class PathNode : IComparer<PathNode>, IIndexedObject
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
                return -1;
            else if (x.F > y.F)
                return 1;

            return 0;
        }

        public int Index { get; set; }
    }

    /// <summary>
    ///     Uses about 50 MB for a 1024x1024 grid.
    /// </summary>
    public class SpatialAStar
    {
        private static readonly double SQRT_2 = Math.Sqrt(2);
        private PathNode[,] m_CameFrom;
        private OpenCloseMap m_ClosedSet;
        private OpenCloseMap m_OpenSet;
        private PriorityQueue<PathNode> m_OrderedOpenSet;
        private OpenCloseMap m_RuntimeGrid;
        private PathNode[,] m_SearchSpace;

        public SpatialAStar(PathNode[,] inGrid)
        {
            SearchSpace = inGrid;
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);
            m_SearchSpace = inGrid;
            m_ClosedSet = new OpenCloseMap(Width, Height);
            m_OpenSet = new OpenCloseMap(Width, Height);
            m_CameFrom = new PathNode[Width, Height];
            m_RuntimeGrid = new OpenCloseMap(Width, Height);
            m_OrderedOpenSet = new PriorityQueue<PathNode>(PathNode.Comparer);
        }

        public PathNode[,] SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        protected virtual double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) +
                             (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));
        }

        protected virtual double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            int diffX = Math.Abs(inStart.X - inEnd.X);
            int diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return SQRT_2;
                case 0: return 0;
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
            PathNode startNode = m_SearchSpace[inStartNode.X, inStartNode.Y];
            PathNode endNode = m_SearchSpace[inEndNode.X, inEndNode.Y];

            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            if (startNode == endNode)
                return new LinkedList<PathNode>(new PathNode[] {startNode});

            PathNode[] neighborNodes = new PathNode[8];

            m_ClosedSet.Clear();
            m_OpenSet.Clear();
            m_RuntimeGrid.Clear();
            m_OrderedOpenSet.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    m_CameFrom[x, y] = null;
                }
            }

            startNode.G = 0;
            startNode.H = Heuristic(startNode, endNode);
            startNode.F = startNode.H;

            m_OpenSet.Add(startNode);
            m_OrderedOpenSet.Push(startNode);

            m_RuntimeGrid.Add(startNode);

            int nodes = 0;

            while (!m_OpenSet.IsEmpty)
            {
                PathNode x = m_OrderedOpenSet.Pop();

                if (x == endNode)
                {
                    // watch.Stop();

                    //elapsed.Add(watch.ElapsedMilliseconds);

                    LinkedList<PathNode> result = ReconstructPath(m_CameFrom, m_CameFrom[endNode.X, endNode.Y]);

                    result.AddLast(endNode);

                    return result;
                }

                m_OpenSet.Remove(x);
                m_ClosedSet.Add(x);

                StoreNeighborNodes(x, neighborNodes);

                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    PathNode y = neighborNodes[i];
                    bool tentative_is_better;

                    if (y == null)
                        continue;

                    if (y.IsWall)
                        continue;

                    if (m_ClosedSet.Contains(y))
                        continue;

                    nodes++;

                    double tentative_g_score = m_RuntimeGrid[x].G + NeighborDistance(x, y);
                    bool wasAdded = false;

                    if (!m_OpenSet.Contains(y))
                    {
                        m_OpenSet.Add(y);
                        tentative_is_better = true;
                        wasAdded = true;
                    }
                    else if (tentative_g_score < m_RuntimeGrid[y].G)
                    {
                        tentative_is_better = true;
                    }
                    else
                    {
                        tentative_is_better = false;
                    }

                    if (tentative_is_better)
                    {
                        m_CameFrom[y.X, y.Y] = x;

                        if (!m_RuntimeGrid.Contains(y))
                            m_RuntimeGrid.Add(y);

                        m_RuntimeGrid[y].G = tentative_g_score;
                        m_RuntimeGrid[y].H = Heuristic(y, endNode);
                        m_RuntimeGrid[y].F = m_RuntimeGrid[y].G + m_RuntimeGrid[y].H;

                        if (wasAdded)
                            m_OrderedOpenSet.Push(y);
                        else
                            m_OrderedOpenSet.Update(y);
                    }
                }
            }

            return null;
        }

        private LinkedList<PathNode> ReconstructPath(PathNode[,] came_from, PathNode current_node)
        {
            LinkedList<PathNode> result = new LinkedList<PathNode>();

            ReconstructPathRecursive(came_from, current_node, result);

            return result;
        }

        private void ReconstructPathRecursive(PathNode[,] came_from, PathNode current_node, LinkedList<PathNode> result)
        {
            PathNode item = came_from[current_node.X, current_node.Y];

            if (item != null)
            {
                ReconstructPathRecursive(came_from, item, result);

                result.AddLast(current_node);
            }
            else
                result.AddLast(current_node);
        }

        private void StoreNeighborNodes(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            inNeighbors[0] = null;

            if (y > 0)
                inNeighbors[1] = m_SearchSpace[x, y - 1];
            else
                inNeighbors[1] = null;

            inNeighbors[2] = null;

            if (x > 0)
                inNeighbors[3] = m_SearchSpace[x - 1, y];
            else
                inNeighbors[3] = null;

            if (x < Width - 1)
                inNeighbors[4] = m_SearchSpace[x + 1, y];
            else
                inNeighbors[4] = null;

            inNeighbors[5] = null;

            if (y < Height - 1)
                inNeighbors[6] = m_SearchSpace[x, y + 1];
            else
                inNeighbors[6] = null;

            inNeighbors[7] = null;
        }

        private class OpenCloseMap
        {
            private PathNode[,] m_Map;

            public OpenCloseMap(int inWidth, int inHeight)
            {
                m_Map = new PathNode[inWidth, inHeight];
                Width = inWidth;
                Height = inHeight;
            }

            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Count { get; private set; }

            public PathNode this[int x, int y]
            {
                get { return m_Map[x, y]; }
            }

            public PathNode this[PathNode Node]
            {
                get { return m_Map[Node.X, Node.Y]; }
            }

            public bool IsEmpty
            {
                get { return Count == 0; }
            }

            public void Add(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

#if DEBUG
                if (item != null)
                    throw new ApplicationException();
#endif

                Count++;
                m_Map[inValue.X, inValue.Y] = inValue;
            }

            public bool Contains(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

                if (item == null)
                    return false;

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                return true;
            }

            public void Remove(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                Count--;
                m_Map[inValue.X, inValue.Y] = null;
            }

            public void Clear()
            {
                Count = 0;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        m_Map[x, y] = null;
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Helper to compute a map to calculate movement costs and building paths avoiding other units, given a certain limit of movements
    /// </summary>
    public class PathFinder
    {

        public class Cell
        {
            public Vector2Int Position;
            public Vector2Int ComeFrom;
            public int Cost = int.MaxValue;
        }

        #region Properties

        public Vector2Int Size
        {
            get
            {
                return costMap.Size;
            }
        }

        /// <summary>
        /// The origin where the computation is done
        /// </summary>
        public Vector2Int Origin
        {
            get; private set;
        }

        /// <summary>
        /// Map to represent the cost and procedence of going to each cell. Values can be different if the cell is not reachable.
        /// </summary>
        private Matrix2D<Cell> costMap { get; set; }

        /// <summary>
        /// Map to quickly check obstacles. Built using units present in the map.
        /// </summary>
        private Matrix2D<int> obstacleMap { get; set; }

        /// <summary>
        /// Map to quickly check what cells are reachable
        /// </summary>
        public Matrix2D<int> ReachabilityMap { get; private set; }

        /// <summary>
        /// Tells if the PathFinder was set to an invalid state
        /// </summary>
        public bool InvalidState { get; private set; }
        #endregion

        /// <summary>
        /// Fetches the path to a given point. Giving an unreachable point, this will end up in infinite loop.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<Vector2Int> GetPathTo(Vector2Int v)
        {
            var path = new List<Vector2Int>();
            var maxIterations = Size.x * Size.y;
            while(v != Origin && maxIterations >= 0)
            {
                maxIterations--;
                path.Add(v);
                v = GetCell(v).ComeFrom;
                if(maxIterations < 0)
                {
                    InvalidState = true;
                    path = null;
                }
            }
            return path;
        }

        /// <summary>
        /// Reachable nodes iterator
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cell> ReachableNodes()
        {
            for (int i = 0; i < ReachabilityMap.Size.x; i++)
            {
                for (int j = 0; j < ReachabilityMap.Size.y; j++)
                {
                    if (ReachabilityMap.Get(i, j) == 1)
                    {
                        yield return GetCell(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="costMap"/>
        /// </summary>
        /// <param name="origin">The starting point</param>
        /// <param name="limit">The limit of movements</param>
        /// <param name="obstacleMap">A map with the present obstacles</param>
        public PathFinder(Vector2Int origin, int limit, Matrix2D<int> obstacleMap)
        {
            costMap = new Matrix2D<Cell>(obstacleMap.Size.x, obstacleMap.Size.y, null);
            ReachabilityMap = new Matrix2D<int>(obstacleMap.Size.x, obstacleMap.Size.y, 0);
            this.obstacleMap = obstacleMap;
            Origin = origin;
            var open = new Queue<Vector2Int>();
            open.Enqueue(origin);
            GetCell(origin).Cost = 0;
            var iterations = 0;
            while(open.Count > 0 && !InvalidState)
            {
                iterations++;
                var current = open.Dequeue();
                var neighboors = calculateNeighboors(current);
                var cost = GetCell(current).Cost + 1;
                if( cost <= limit )
                {
                    foreach (var n in neighboors)
                    {
                        if (GetCell(n).Cost > cost)
                        {
                            ReachabilityMap.Set(n, 1);
                            GetCell(n).ComeFrom = current;
                            GetCell(n).Cost = cost;
                            open.Enqueue(n);
                        }
                    }
                }
                InvalidState = iterations > Size.x * Size.y + 1;
            }
        }

        /// <summary>
        /// Checks if a cell is an obstacle or not
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsObstacle(Vector2Int c)
        {
            return obstacleMap.Get(c) != 0;
        }

        /// <summary>
        /// Helper to check if the cell is inside bounds and is not an obstacle
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool canWalkTo(Vector2Int c)
        {
            return 
                c.x >= 0 && c.x < obstacleMap.Size.x &&
                c.y >= 0 && c.y < obstacleMap.Size.y &&
                !IsObstacle(c);
        }

        /// <summary>
        /// Fetches the neighboors if it's possible to move
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private List<Vector2Int> calculateNeighboors(Vector2Int c)
        {
            var neighboors = new List<Vector2Int>();
            void addIfCanWalkTo(Vector2Int n) { if (canWalkTo(n)) neighboors.Add(n); }
            addIfCanWalkTo(c + new Vector2Int( 1,  0));
            addIfCanWalkTo(c + new Vector2Int(-1,  0));
            addIfCanWalkTo(c + new Vector2Int( 0,  1));
            addIfCanWalkTo(c + new Vector2Int( 0, -1));
            return neighboors;
        }

        /// <summary>
        /// Gets or creates a <see cref="Cell"/> in the <see cref="costMap"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell GetCell(int x, int y)
        {
            if (costMap.Get(x, y) == null)
            {
                costMap.Set(x, y, new Cell() { Position = new Vector2Int(x, y) });
            }
            return costMap.Get(x, y);
        }

        public Cell GetCell(Vector2Int cell)
        {
            return GetCell(cell.x, cell.y);
        }


    }

}
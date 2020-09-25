using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CubeWars
{

    namespace Tests
    {
        public class PathFinderTest
        {

            private Matrix2D<int> RandomEmptyObstacleMap()
            {
                var width = Random.Range(10, 20);
                var height = Random.Range(10, 20);
                return new Matrix2D<int>(width, height, 0);
            }

            [Test]
            public void ReachableTest()
            {
                int numIterations = 1000;
                while (--numIterations > 0)
                {
                    var obstacleMap = RandomEmptyObstacleMap();
                    var origin = new Vector2Int(Random.Range(0, obstacleMap.Size.x), Random.Range(0, obstacleMap.Size.y));
                    var maxValue = Mathf.Max(obstacleMap.Size.x, obstacleMap.Size.y);
                    var limit = (int)Random.Range(0.25f * maxValue, 0.75f * maxValue);

                    var pathfinder = new PathFinder(origin, limit, obstacleMap);
                    Assert.IsFalse(pathfinder.InvalidState);

                    var reachableIteratorTest = 0;
                    foreach (var reachableNode in pathfinder.ReachableNodes())
                    {
                        reachableIteratorTest++;
                        Debug.Assert(reachableNode.Cost == Utils.ManhattanDistance(origin, reachableNode.Position));
                    }
                }
            }



            [Test]
            public void OccupiedNodesTest()
            {
                int numIterations = 1000;
                while (--numIterations > 0)
                {
                    var obstacleMap = RandomEmptyObstacleMap();
                    var maxLimit = obstacleMap.Size.x * obstacleMap.Size.y;

                    var occupiedNodes = 0;
                    obstacleMap.Fill(0);
                    for (int i = 0; i < obstacleMap.Size.x; i++)
                    {
                        for (int j = 0; j < obstacleMap.Size.y; j++)
                        {
                            if (i % 2 == 0 && j % 2 == 0)
                            {
                                obstacleMap.Set(i, j, 1);
                                occupiedNodes++;
                            }
                        }
                    }

                    var pathfinder = new PathFinder(new Vector2Int(1, 1), maxLimit, obstacleMap);
                    Assert.IsFalse(pathfinder.InvalidState);

                    for (int i = 0; i < pathfinder.ReachabilityMap.Size.x; i++)
                    {
                        for (int j = 0; j < pathfinder.ReachabilityMap.Size.y; j++)
                        {
                            Assert.IsTrue(pathfinder.GetCell(i, j).Position == new Vector2Int(i, j));
                            if (pathfinder.ReachabilityMap.Get(i, j) == 0)
                            {
                                occupiedNodes--;
                            }
                        }
                    }
                    Assert.IsTrue(occupiedNodes == -1);
                }
            }

        }
    }


}

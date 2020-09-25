using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CubeWars
{

    namespace Tests
    {
        public class UtilsTest
        {

            [Test]
            public void ManhattanDistanceOrderTest()
            {
                int numIterations = 1000;
                while (--numIterations > 0)
                {
                    var v1 = new Vector2Int(Random.Range(10, 1000), Random.Range(10, 1000));
                    var v2 = new Vector2Int(Random.Range(10, 1000), Random.Range(10, 1000));
                    Assert.IsTrue(Utils.ManhattanDistance(v1, v2) == Utils.ManhattanDistance(v2, v1));

                    var val1 = Random.Range(500, 10000);
                    var val2 = Random.Range(500, 10000);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(val1, val2), new Vector2Int(val1, val2)) == 0);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(val2, val1), new Vector2Int(val2, val1)) == 0);
                }
            }

            [Test]
            public void ManhattanDistanceDirectCalculationTest()
            {
                int numIterations = 1000;
                while (--numIterations > 0)
                {
                    var v1 = Random.Range(500, 10000);
                    var v2 = Random.Range(500, 10000);
                    var r = Random.Range(500, 10000);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(v1, v2), new Vector2Int(v1 + r, v2)) == r);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(v1, v2), new Vector2Int(v1, v2 + r)) == r);
                }
            }

            [Test]
            public void ManhattanDistanceDiagonalTest()
            {
                int numIterations = 1000;
                while (--numIterations > 0)
                {
                    var v = Random.Range(500, 10000);
                    var r = Random.Range(500, 10000);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(v, v), new Vector2Int(v + r, v + r)) == r * 2);
                    Assert.IsTrue(Utils.ManhattanDistance(new Vector2Int(v, v), new Vector2Int(v - r, v - r)) == r * 2);
                }
            }

        }
    }


}

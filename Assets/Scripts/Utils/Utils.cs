using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    public class Utils
    {
        public static int ManhattanDistance(Vector2Int v1, Vector2Int v2)
        {
            return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
        }

        public static int MahnattanDistance(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

    }

}
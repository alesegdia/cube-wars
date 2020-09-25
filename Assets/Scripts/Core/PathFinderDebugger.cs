using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    public class PathFinderDebugger : MonoBehaviour
    {

        public PathFinder PathFinder
        {
            get; set;
        }

        private void Start()
        {
            Matrix2D<int> obstacleMap = new Matrix2D<int>(10, 10, 0);
            obstacleMap.Set(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        0, 0, 1, 0, 1, 1, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0, 1, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0, 1, 0, 0, 0, 0,
                                        0, 0, 1, 0, 0, 1, 0, 0, 0, 0,
                                        0, 0, 1, 0, 1, 1, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        });
            PathFinder = new PathFinder(new Vector2Int(3, 5), 5, obstacleMap);
        }

        private void OnGUI()
        {
            if(PathFinder != null)
            {
                var rect = new Rect(0, 0, 60, 50);
                for(int i = 0; i < PathFinder.Size.x; i++)
                {
                    for (int j = 0; j < PathFinder.Size.y; j++)
                    {
                        var c = new Vector2Int(i, j);
                        rect.x = 100 + i * rect.width;
                        rect.y = 100 + j * rect.height;
                        var cell = PathFinder.GetCell(c);
                        var obstacle = PathFinder.IsObstacle(c);
                        var unreachable = cell.Cost == int.MaxValue && !obstacle;
                        if(unreachable)
                        {
                            GUI.backgroundColor = Color.magenta;
                        }
                        else if(obstacle)
                        {
                            GUI.backgroundColor = Color.red;
                        }
                        else
                        {
                            GUI.backgroundColor = Color.green;
                        }

                        if (PathFinder.Origin == c)
                        {
                            GUI.Button(rect, "ORIGIN");
                        }
                        else
                        {
                            var cost = unreachable || obstacle ? "##" : cell.Cost.ToString();
                            GUI.Button(rect, "O: " + cell.ComeFrom + "\nC: " + cost);
                        }
                    }
                }
            }
        }

    }

}
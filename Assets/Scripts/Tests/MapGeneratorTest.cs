using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    public class MapGeneratorTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var map = new int[] { 0, 0, 0, 0, 0, 0,
                                  1, 0, 1, 0, 1, 1,
                                  0, 0, 0, 0, 1, 0,
                                  1, 1, 1, 1, 0, 1 };
            GetComponent<MapGenerator>().CreateMap(1, 1, 6, 4);
        }

    }

}
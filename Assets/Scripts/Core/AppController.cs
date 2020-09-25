using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    public enum GameMode
    {
        SingleMatch,
        Infinite
    }


    public class AppController : MonoBehaviour
    {

        public GameMode GameMode { get; set; }
        public int CurrentInfiniteLevel { get; set; }


    }

}
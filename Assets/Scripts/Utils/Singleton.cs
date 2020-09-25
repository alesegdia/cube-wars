using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public class Singleton<T> where T : MonoBehaviour
    {
        static private T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var go = new GameObject() { name = typeof(T).Name };
                    s_instance = go.AddComponent<T>();
                    Object.DontDestroyOnLoad(go);
                }
                return s_instance;
            }
        }

    }

}
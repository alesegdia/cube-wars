using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public static class TestUtils
    {

        public static IEnumerator WaitUntilSceneIsLoaded(string sceneName)
        {
            var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            while (!async.isDone)
            {
                yield return null;
            }
        }

        public static IEnumerator WaitUntilSceneIsUnloaded(string sceneName)
        {
            var async = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!async.isDone)
            {
                yield return null;
            }
        }

    }

}
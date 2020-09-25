using UnityEngine.SceneManagement;
using UnityEngine;


namespace CubeWars
{

    public class MainMenuSceneController : MonoBehaviour
    {
        public void OnSingleMatchClick()
        {
            Singleton<AppController>.Instance.GameMode = GameMode.SingleMatch;
            goToGameScene();
        }

        public void OnInfiniteModeClick()
        {
            Singleton<AppController>.Instance.GameMode = GameMode.Infinite;
            goToGameScene();
        }

        private void goToGameScene()
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }

}
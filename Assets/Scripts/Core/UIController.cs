using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{
    /// <summary>
    /// Handles all the UI functionality like enabling/disabling elements, camera movement,
    /// show match results or log messages to console.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        /// <summary>
        /// Message type for the logging to console
        /// </summary>
        public enum MessageType
        {
            Info, Warning
        }

        #region Serialized Fields
        [Header("General")]
        [SerializeField] private MatchManager m_matchManager = null;
        [SerializeField] private Camera m_camera = null;

        [Header("UI Elements")]
        [SerializeField] private GameObject m_unitInfo = null;
        [SerializeField] private GameObject m_debugButtons = null;
        [SerializeField] private LogConsole m_logConsole = null;

        [SerializeField] private TMPro.TextMeshProUGUI m_turnText = null;

        [Header("Mouse Movement")]
        [SerializeField] private float m_timeToConsiderDrag = 0.3f;
        [SerializeField] private Vector2 m_mouseMovementFactor = new Vector2(1, 1);

        [Header("End Game")]
        [SerializeField] private GameObject m_endGameUI = null;
        [SerializeField] private TMPro.TextMeshProUGUI m_endGameText = null;
        [SerializeField] private TMPro.TextMeshProUGUI m_continueText = null;
        #endregion

        #region Properties
        private float timeMouseDown { get; set; } = 0.0f;
        private Vector3 lastMousePos { get; set; }
        private HumanPlayerController humanPlayer { get; set; }
        #endregion

        public void Initialize(HumanPlayerController human)
        {
            humanPlayer = human;
        }

        #region UI messages
        /// <summary>
        /// Rotates the camera 90 degrees to the left
        /// </summary>
        public void RotateLeft()
        {
            StartCoroutine(rotateAround(-1));
        }

        /// <summary>
        /// Rotates the camera 90 degrees to the right
        /// </summary>
        public void RotateRight()
        {
            StartCoroutine(rotateAround(1));
        }

        /// <summary>
        /// Toggles the unit info panel
        /// </summary>
        public void ToggleUnitInfo()
        {
            m_unitInfo.SetActive(!m_unitInfo.activeSelf);
        }

        /// <summary>
        /// Player ends the turn clicking the End Turn button
        /// </summary>
        public void EndTurn()
        {
            humanPlayer.EndTurn();
        }

        /// <summary>
        /// Toggles debug menu
        /// </summary>
        public void ToggleDebug()
        {
            m_debugButtons.SetActive(!m_debugButtons.activeSelf);
        }

        /// <summary>
        /// Forces the win of the human player for debugging purposes.
        /// </summary>
        public void ForceWin()
        {
            m_matchManager.ForceHumanWin();
            ShowMatchResults();
        }

        /// <summary>
        /// Forces the lose of the human player for debugging purposes.
        /// </summary>
        public void ForceLose()
        {
            m_matchManager.ForceHumanLose();
            ShowMatchResults();
        }

        /// <summary>
        /// Called when the player clicks the end game button. If the <see cref="GameMode"/>
        /// is <see cref="GameMode.Infinite"/>, it will relaunch the GameScene, and if it's
        /// <see cref="GameMode.SingleMatch"/>, it will relaunch the MenuScene.
        /// </summary>
        public void ContinueGame()
        {
            Debug.Assert(m_matchManager.GameFinished());
            switch (Singleton<AppController>.Instance.GameMode)
            {
                case GameMode.SingleMatch:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
                    break;
                case GameMode.Infinite:
                    if (m_matchManager.HumanWon)
                    {
                        Singleton<AppController>.Instance.CurrentInfiniteLevel++;
                        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                    }
                    else
                    {
                        Singleton<AppController>.Instance.CurrentInfiniteLevel = 0;
                        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Notifies the UI about the new turn.
        /// </summary>
        /// <param name="playerIndex">The index of the faction which turn is starting now</param>
        public void NewTurn(int playerIndex)
        {
            var gameMode = Singleton<AppController>.Instance.GameMode;
            var baseTurnText = gameMode == GameMode.Infinite ? "LEVEL" + Singleton<AppController>.Instance.CurrentInfiniteLevel : "" + " ";
            m_turnText.text = baseTurnText + "Player " + playerIndex + "'s Turn";
        }

        /// <summary>
        /// Logs a message with a timestamp and a given color
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void Log(string message, Color color)
        {
            m_logConsole.Log(
                "[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " +
                "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" +
                message +
                "</color>");
        }

        /// <summary>
        /// Called when a match finishes to set up the labels and continue button
        /// </summary>
        public void ShowMatchResults()
        {
            var gameMode = Singleton<AppController>.Instance.GameMode;
            m_endGameUI.SetActive(true);
            if (m_matchManager.HumanWon)
            {
                m_endGameText.text = "You WIN";
                switch (gameMode)
                {
                    case GameMode.SingleMatch:
                        m_continueText.text = "Main Menu";
                        break;
                    case GameMode.Infinite:
                        m_continueText.text = "Next Match";
                        break;
                }
            }
            else
            {
                m_endGameText.text = "You LOSE";
                m_continueText.text = "Main Menu";
            }
        }

        /// <summary>
        /// Used for camera scroll
        /// </summary>
        /// <param name="currentMousePosition"></param>
        /// <param name="lastMousePosition"></param>
        private void onDrag(Vector3 currentMousePosition, Vector3 lastMousePosition)
        {
            var deltaVector = currentMousePosition - lastMousePosition;
            var prevY = m_camera.transform.position.y;
            var pos = m_camera.transform.position +
                deltaVector.x * m_camera.transform.right * m_mouseMovementFactor.x +
                deltaVector.y * m_camera.transform.up * m_mouseMovementFactor.y;
            m_camera.transform.position = pos;
        }
        
        /// <summary>
        /// Coroutine to rotate the camera with a given factor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        private IEnumerator rotateAround(int factor)
        {
            var angle = 0.0f;
            while (angle <= 90.0f)
            {
                var delta = Time.deltaTime * 100.0f;
                angle += delta;
                rotateCam(delta * factor);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Actually rotates the camera for a given amount around the center of the map
        /// </summary>
        /// <param name="amount"></param>
        private void rotateCam(float amount)
        {
            m_camera.transform.RotateAround(new Vector3(m_matchManager.MapManager.Size.x / 2.0f, 0, m_matchManager.MapManager.Size.y / 2.0f), Vector3.up, amount);
        }

        /// <summary>
        /// Handles mouse logic and sets the selected unit info string.
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                timeMouseDown = 0.0f;
            }

            if (Input.GetMouseButton(0))
            {
                timeMouseDown += Time.deltaTime;
                if (timeMouseDown > m_timeToConsiderDrag)
                {
                    onDrag(Input.mousePosition, lastMousePos);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (timeMouseDown <= m_timeToConsiderDrag)
                {
                    humanPlayer.OnClick();
                }
            }

            if (humanPlayer.SelectedUnit != null)
            {
                m_unitInfo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = humanPlayer.SelectedUnit.StatsInfoString;
            }
            else
            {
                m_unitInfo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "NO UNIT SELECTED";
            }

            lastMousePos = Input.mousePosition;
        }

    }

}
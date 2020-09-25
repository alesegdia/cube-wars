using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public class GameSceneController : MonoBehaviour
    {
        [SerializeField] private GameController m_gameController = null;

        void Start()
        {
            m_gameController.StartMatch();
        }

    }

}
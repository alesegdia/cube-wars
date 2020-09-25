using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public class LogConsole : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI m_text;

        private void Start()
        {
            m_text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        public void Log(string text)
        {
            m_text.text = text + "\n" + m_text.text;
        }
    }

}
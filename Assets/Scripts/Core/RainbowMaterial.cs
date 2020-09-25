using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_renderer = null;
    private float m_timer = 0.0f;

    public Color Color
    {
        set
        {
            m_renderer.material.color = value;
        }
    }

    void Start()
    {
        Color = Color.red;
    }


    void Update()
    {
        if (m_timer < 1.0f)
        {
            Color = Color.Lerp(Color.red, Color.blue, m_timer);
        }
        else if (m_timer < 2.0f)
        {
            Color = Color.Lerp(Color.blue, Color.green, m_timer - 1.0f);
        }
        else if (m_timer < 3.0f)
        {
            Color = Color.Lerp(Color.green, Color.red, m_timer - 2.0f);
        }
        m_timer += Time.deltaTime;
        if (m_timer >= 3.0f) m_timer = 0.0f;
    }
}

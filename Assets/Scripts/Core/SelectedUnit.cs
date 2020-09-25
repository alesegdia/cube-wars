using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    private static readonly float TopPoint = 0.3f;
    private static readonly float FloatingSpeed = 1.5f;

    private float m_initialY;
    private float m_timer;

    private void Start()
    {
        m_initialY = transform.position.y;
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        var y = Mathf.SmoothStep(m_initialY, m_initialY + TopPoint, Mathf.PingPong(m_timer * FloatingSpeed, 1.0f));
        var prevPos = transform.position;
        prevPos.y = y;
        transform.position = prevPos;
    }

    public void Deselect()
    {
        var prevPos = transform.position;
        prevPos.y = m_initialY;
        transform.position = prevPos;
    }

}

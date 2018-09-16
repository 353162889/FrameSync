using UnityEngine;
using System.Collections;

public class LaserEffect : MonoBehaviour
{
    private LineRenderer m_cLineRender;
    private bool m_bDelay = false;
    private float m_fSpeed = 160f;
    private float m_fTargetLength;
    private void Awake()
    {
        m_cLineRender = GetComponentInChildren<LineRenderer>();
        UpdateLengthImmediately(0);
    }
    
    public void UpdateLength(float length)
    {
        if (m_cLineRender == null) return;
        Vector3 pos = m_cLineRender.GetPosition(m_cLineRender.positionCount - 1);
        if (pos.z < length)
        {
            m_bDelay = true;
            m_fTargetLength = length;
        }
        else
        {
            UpdateLengthImmediately(length);
        }
    }

    private void UpdateLengthImmediately(float length)
    {
        if (m_cLineRender == null) return;
        m_bDelay = false;
        Vector3 pos = m_cLineRender.GetPosition(m_cLineRender.positionCount - 1);
        pos.z = length;
        m_cLineRender.SetPosition(m_cLineRender.positionCount - 1, pos);
    }

    private void Update()
    {
        if(m_bDelay)
        {
            var pos = m_cLineRender.GetPosition(m_cLineRender.positionCount - 1);
            float t = Mathf.MoveTowards(pos.z, m_fTargetLength, m_fSpeed * Time.deltaTime);
            pos.z = t;
            m_cLineRender.SetPosition(m_cLineRender.positionCount - 1, pos);
        }
    }
}

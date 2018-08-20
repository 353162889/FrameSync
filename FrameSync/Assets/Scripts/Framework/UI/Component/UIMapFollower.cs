using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class UIMapFollower : MonoBehaviour
    {
        public Transform m_cTarget;
        public Rect m_sTargetRect;
        private RectTransform m_cParent;
        private RectTransform m_cRectTransform;
        private Rect m_sParentRect;

        void Awake()
        {
            m_cParent = transform.parent as RectTransform;
            m_cRectTransform = transform as RectTransform;
            m_sParentRect = m_cParent.rect;
        }

        public void SetTarget(Transform target, Rect targetRect)
        {
            m_cTarget = target;
            m_sTargetRect = targetRect;
        }

        void LateUpdate()
        {
            if (m_cTarget != null)
            {
                var position = m_cTarget.position;
                float percentX = (position.x - m_sTargetRect.xMin) / m_sTargetRect.width;
                float percentZ = (position.z - m_sTargetRect.yMin) / m_sTargetRect.height;
                percentX = Mathf.Clamp01(percentX);
                percentZ = Mathf.Clamp01(percentZ);
                float xPos = (m_sParentRect.width * percentX) + m_sParentRect.xMin;
                float yPos = (m_sParentRect.height * percentZ) + m_sParentRect.yMin;
                m_cRectTransform.anchoredPosition = new Vector2(xPos, yPos);
            }
        }

        public void Dispose()
        {
            m_cTarget = null;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}
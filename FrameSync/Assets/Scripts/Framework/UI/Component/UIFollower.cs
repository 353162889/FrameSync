using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class UIFollower : MonoBehaviour
    {
        public GameObject m_cTarget;
        private Canvas m_cCanvas;
        public Camera m_cCamera;
        private RectTransform m_cParent;
        private RectTransform m_cRectTransform;
        private bool m_bNotOutUI;
        private bool m_bInUINotShow;

        void Awake()
        {
            m_cCanvas = (transform as RectTransform).GetComponentInParent<Canvas>();
            if (m_cCanvas != null) m_cCanvas = m_cCanvas.rootCanvas;
            m_cParent = transform.parent as RectTransform;
            m_cRectTransform = transform as RectTransform;
            m_bNotOutUI = false;
            m_bInUINotShow = false;
        }

        public void SetTarget(Camera camera, Transform target, bool notOutUI = false, bool inUINotShow = false)
        {
            m_cCanvas = (transform as RectTransform).GetComponentInParent<Canvas>();
            if (m_cCanvas != null) m_cCanvas = m_cCanvas.rootCanvas;
            m_cParent = transform.parent as RectTransform;
            m_cRectTransform = transform as RectTransform;
            m_cCamera = camera;
            m_cTarget = target.gameObject;
            m_bNotOutUI = notOutUI;
            m_bInUINotShow = inUINotShow;
        }

        void LateUpdate()
        {
            if (m_cCanvas != null && m_cCamera != null && m_cTarget != null)
            {
                Vector3 screenPos = m_cCamera.WorldToScreenPoint(m_cTarget.transform.position);
                screenPos.z = 0;
                Vector2 pos = Vector2.zero;
                if (m_cCanvas.renderMode == RenderMode.ScreenSpaceCamera && m_cCanvas.worldCamera != null /*&& !m_cCanvas.worldCamera.orthographic*/)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(m_cParent, screenPos, m_cCanvas.worldCamera, out pos);
                }
                else
                {
                    pos = m_cParent.InverseTransformPoint(screenPos);
                }
                Vector2 min = m_cParent.rect.min;
                Vector2 max = m_cParent.rect.max;
                bool showUI = true;
                if (m_bInUINotShow)
                {
                    bool inUI = pos.x > min.x && pos.x < max.x && pos.y > min.y && pos.y < max.y;
                    if (inUI)
                    {
                        showUI = false;
                        pos.x = 10000;
                        pos.y = 10000;
                    }
                }

                if (showUI && m_bNotOutUI)
                {
                    if (pos.x < min.x)
                    {
                        pos.x = min.x;
                    }
                    if (pos.x > max.x)
                    {
                        pos.x = max.x;
                    }
                    if (pos.y < min.y)
                    {
                        pos.y = min.y;
                    }
                    if (pos.y > max.y)
                    {
                        pos.y = max.y;
                    }
                }
                transform.localScale = m_cCamera.transform.localScale;
                m_cRectTransform.anchoredPosition = pos;
            }
        }

        public void Dispose()
        {
            m_cTarget = null;
            m_cCanvas = null;
            m_cCamera = null;
            m_cParent = null;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class UIDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public delegate bool UIDragCheckHandler(UIDrag drag);
        public delegate void UIDragHandler(UIDrag drag, PointerEventData data);
        public delegate void UIDragExitHandler(UIDrag drag);
        public event UIDragCheckHandler OnCanDragListener;
        public event UIDragHandler OnBeginDragListener;
        public event UIDragHandler OnDragListener;
        public event UIDragHandler OnEndDragListener;
        public event UIDragExitHandler OnDragExitListener;
        protected RectTransform m_cDragTrans;
        public RectTransform dragTrans
        {
            get { return m_cDragTrans; }
        }
        protected RectTransform m_cCanvasTrans;
        private Canvas canvas;
        protected virtual void Awake()
        {
            canvas = gameObject.FindInParents<Canvas>();
            if (canvas != null)
            {
                m_cCanvasTrans = canvas.transform as RectTransform;
            }
        }

        protected void SetDraggedPosition(PointerEventData data)
        {
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_cCanvasTrans, data.position, data.pressEventCamera, out globalMousePos))
            {
                m_cDragTrans.position = globalMousePos;
                m_cDragTrans.rotation = m_cCanvasTrans.rotation;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (OnCanDragListener != null)
            {
                if (!OnCanDragListener(this)) return;
            }
            if (m_cCanvasTrans == null) return;
            m_cDragTrans = GameObject.Instantiate(gameObject).transform as RectTransform;
            m_cDragTrans.SetParent(m_cCanvasTrans.transform, false);
            m_cDragTrans.SetAsLastSibling();
            RectTransform trans = transform as RectTransform;
            m_cDragTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, trans.rect.width);
            m_cDragTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, trans.rect.height);
            var group = m_cDragTrans.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            SetDraggedPosition(eventData);
            if (OnBeginDragListener != null)
            {
                OnBeginDragListener(this, eventData);
            }
        }

        public bool IsOnArea(RectTransform obj, PointerEventData eventData)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(obj, eventData.position, canvas.worldCamera, out pos))
            {
                bool result = obj.rect.Contains(pos);
                return result;
            }
            return false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_cDragTrans == null) return;
            SetDraggedPosition(eventData);
            if (OnDragListener != null)
            {
                OnDragListener(this, eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_cDragTrans == null) return;
            Destroy(m_cDragTrans.gameObject);
            m_cDragTrans = null;
            if (OnEndDragListener != null)
            {
                OnEndDragListener(this, eventData);
            }
        }

        public virtual void StopDrag()
        {
            if (m_cDragTrans != null)
            {
                Destroy(m_cDragTrans.gameObject);
                m_cDragTrans = null;
                if (OnDragExitListener != null)
                {
                    OnDragExitListener(this);
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (m_cDragTrans != null)
                {
                    Destroy(m_cDragTrans.gameObject);
                    m_cDragTrans = null;
                    if (OnDragExitListener != null)
                    {
                        OnDragExitListener(this);
                    }
                }
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (m_cDragTrans != null)
                {
                    Destroy(m_cDragTrans.gameObject);
                    m_cDragTrans = null;
                    if (OnDragExitListener != null)
                    {
                        OnDragExitListener(this);
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            if (m_cDragTrans != null)
            {
                Destroy(m_cDragTrans.gameObject);
                m_cDragTrans = null;
            }
            OnCanDragListener = null;
            OnBeginDragListener = null;
            OnDragListener = null;
            OnEndDragListener = null;
            OnDragExitListener = null;
        }
    }
}
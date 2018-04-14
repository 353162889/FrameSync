using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class UIJoystick : MonoBehaviour
    {
        public delegate void JoystickHandler(Vector2 vector2);
        public event JoystickHandler OnBegin;
        public event JoystickHandler OnMove;
        public event JoystickHandler OnEnd;
        private static readonly int MouseLeftKey = 0;
        public RectTransform mEffectTransfrom;
        //base与move在相同父节点下
        public RectTransform mBase;
        public RectTransform mMove;

        private Canvas m_cCanvas;
        private Camera m_cCanvasCamera;

        private int m_nCurTouchId;
        private Vector2 m_defaultBasePosition;
        private Vector2 m_startBasePosition;
        public float moveRadius { get { return m_fMoveRadius; } }
        private float m_fMoveRadius;
        private bool m_bFixedBase;

        private Dictionary<KeyCode, Vector2> m_dicKeyCode = new Dictionary<KeyCode, Vector2>();

        public float radius = 100;
        public bool fixedBase = true;
        void Awake()
        {
            m_nCurTouchId = -1;
            m_cCanvas = gameObject.GetComponentInParent<Canvas>();
            m_cCanvasCamera = m_cCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? m_cCanvas.worldCamera : null;
           
            //Init(radius,fixedBase);
            //AddKeyCode(KeyCode.A, new Vector2(-1f, 0));
            //AddKeyCode(KeyCode.D, new Vector2(1f, 0));
            //AddKeyCode(KeyCode.W, new Vector2(0, 1f));
            //AddKeyCode(KeyCode.S, new Vector2(0, -1f));
        }

        public void AddKeyCode(KeyCode keyCode,Vector2 direction)
        {
            direction.Normalize();
            m_dicKeyCode.Add(keyCode, direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveRadius">anchorPosition的宽度为准</param>
        /// <param name="fixedBase"></param>
        public void Init(float moveRadius,bool fixedBase)
        {
            m_defaultBasePosition = mBase.anchoredPosition;
            m_fMoveRadius = moveRadius;
            m_bFixedBase = fixedBase;
            //关闭遥感的Raycast
            ForbidRaycastTarget(transform as RectTransform);
            //关闭mEffectTransfrom与mBase与mMove下的碰撞检测
            //ForbidRaycastTarget(mEffectTransfrom);
            //ForbidRaycastTarget(mBase);
            //ForbidRaycastTarget(mMove);
            if (mBase.parent != mMove.parent)
            {
                CLog.LogError("mBase与mMove必须在相同父节点下");
            }
        }

        public void Init(RectTransform effectRTrans,RectTransform baseRTrans,RectTransform moveRTrans,float moveRadius,bool fixBase)
        {
            mEffectTransfrom = effectRTrans;
            mBase = baseRTrans;
            mMove = moveRTrans;
            this.Init(moveRadius, fixBase);
        }

        private void ForbidRaycastTarget(RectTransform transform)
        {
            var graphic = transform.GetComponent<Graphic>();
            if (graphic != null) graphic.raycastTarget = false;
            var childs = transform.GetComponentsInChildren<Graphic>();
            if (childs != null && childs.Length > 0)
            {
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].raycastTarget = false;
                }
            }
        }

        void Update()
        {
            if(m_nCurTouchId < 0)
            {
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR)
                //鼠标操作
                bool isOnUI = (EventSystem.current != null) ? EventSystem.current.IsPointerOverGameObject() : false;
                if(!isOnUI)
                {
                    if(Input.GetMouseButtonDown(MouseLeftKey))
                    {
                        Vector3 screenPosition = Input.mousePosition;
                        bool inEffectRect = RectTransformUtility.RectangleContainsScreenPoint(mEffectTransfrom, screenPosition, m_cCanvasCamera);
                        if(inEffectRect)
                        {
                            m_nCurTouchId = 0;
                            //设置位置
                            Vector2 localPosition;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(mBase.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                            BeginMovePosition(localPosition);
                        }
                    }
                }
                //固定轮盘才允许键盘操作,优先鼠标操作
                if(m_bFixedBase && m_nCurTouchId < 0)
                {
                    Vector2 direct = Vector2.zero;
                    foreach (var item in m_dicKeyCode)
                    {
                        if (Input.GetKeyDown(item.Key))
                        {
                            direct += item.Value;
                        }
                    }
                    if(direct != Vector2.zero)
                    {
                        direct.Normalize();
                        m_nCurTouchId = 1;
                        Vector2 localPosition = m_defaultBasePosition + direct * m_fMoveRadius;
                        BeginMovePosition(localPosition);
                    }
                }
#else
                if(m_nCurTouchId < 0)
                {
                    int touchCount = Input.touchCount;
                    if(touchCount > 0)
                    {
                        for (int i = 0; i < touchCount; i++)
                        {
                            Touch touch = Input.GetTouch(i);
                            if (touch.phase == TouchPhase.Began)
                            {
                                bool isTouchOnUI = (EventSystem.current != null) ? EventSystem.current.IsPointerOverGameObject(touch.fingerId) : false;
                                if (!isTouchOnUI)
                                {
                                    bool inEffectRect = RectTransformUtility.RectangleContainsScreenPoint(mEffectTransfrom, touch.position, m_cCanvasCamera);
                                    if (inEffectRect)
                                    {
                                        m_nCurTouchId = touch.fingerId;
                                        //设置位置
                                        Vector2 localPosition;
                                        RectTransformUtility.ScreenPointToLocalPointInRectangle(mBase.parent as RectTransform, touch.position, m_cCanvasCamera, out localPosition);
                                        BeginMovePosition(localPosition);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
#endif
            }


            if (m_nCurTouchId > -1)
            {
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR)
                if (m_nCurTouchId == 0)
                {
                    if (Input.GetMouseButtonUp(MouseLeftKey))
                    {
                        Vector3 screenPosition = Input.mousePosition;
                        Vector2 localPosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                        EndMovePosition(localPosition);
                    }
                    else if (Input.GetMouseButton(MouseLeftKey))
                    {
                        Vector3 screenPosition = Input.mousePosition;
                        Vector2 localPosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                        UpdateMovePosition(localPosition);
                    }
                }
                else if(m_nCurTouchId == 1)
                {
                    Vector2 direct = Vector2.zero;
                    foreach (var item in m_dicKeyCode)
                    {
                        if (Input.GetKey(item.Key))
                        {
                            direct += item.Value;
                        }
                    }
                    if (direct != Vector2.zero)
                    {
                        direct.Normalize();
                        Vector2 localPosition = m_defaultBasePosition + direct * m_fMoveRadius;
                        UpdateMovePosition(localPosition);
                    }
                    else
                    {
                        EndMovePosition(m_startBasePosition);
                    }
                }
#else
                if(m_nCurTouchId > -1)
                {
                    if(m_nCurTouchId < Input.touchCount)
                    {
                        Touch touch = Input.GetTouch(m_nCurTouchId);
                        if(touch.phase == TouchPhase.Moved)
                        {
                            Vector3 screenPosition = touch.position;
                            Vector2 localPosition;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                            UpdateMovePosition(localPosition);
                        }
                        else
                        {
                            Vector3 screenPosition = touch.position;
                            Vector2 localPosition;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                            EndMovePosition(localPosition);
                        }
                    }
                    else
                    {
                        EndMovePosition(m_startBasePosition);
                    }
                }
#endif
            }
        }

        private void SetBaseAnchoredPosition(Vector2 position)
        {
            if (!m_bFixedBase)
            {
                mBase.anchoredPosition = position;
            }
            m_startBasePosition = mBase.anchoredPosition;
        }

        private void SetMoveAnchoredPosition(Vector2 position)
        {
            Vector2 offset = position - m_startBasePosition;
            float len = offset.magnitude;
            if (len <= m_fMoveRadius)
            {
                mMove.anchoredPosition = position;
            }
            else
            {
                offset.Normalize();
                mMove.anchoredPosition = m_startBasePosition + offset * m_fMoveRadius;
            }
        }

        private void EndMovePosition(Vector2 position)
        {
            m_nCurTouchId = -1;
            Vector2 offsetPosition = position - m_startBasePosition;
            SetBaseAnchoredPosition(m_defaultBasePosition);
            SetMoveAnchoredPosition(m_defaultBasePosition);
            //派发取消事件
            if(null != OnEnd)
            {
                OnEnd(offsetPosition);
            }
           
        }

        private void BeginMovePosition(Vector2 position)
        {
            SetBaseAnchoredPosition(position);
            SetMoveAnchoredPosition(position);
            //派发开始遥感事件
            if (null != OnBegin)
            {
                OnBegin(position - m_startBasePosition);
            }
        }

        private void UpdateMovePosition(Vector2 position)
        {
            SetMoveAnchoredPosition(position);
            //派发移动事件
            if(null != OnMove)
            {
                OnMove(position - m_startBasePosition);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                EndMovePosition(m_startBasePosition);
            }
        }

        private void OnApplicationQuit()
        {
            EndMovePosition(m_startBasePosition);
        }

        public void Clear()
        {
            OnBegin = null;
            OnMove = null;
            OnEnd = null;
        }
    }
}

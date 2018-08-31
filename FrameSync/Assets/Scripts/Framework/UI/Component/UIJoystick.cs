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
        private static float Epsilon6 = 1e-6f;
        public delegate void JoystickHandler(Vector2 screenPos, Vector2 offset,Vector2 delta);
        public event JoystickHandler OnBegin;
        public event JoystickHandler OnMove;//点击不动也算move
        public event JoystickHandler OnEnd;
        private static int KeyTouchId = 10000;
        private static readonly int MouseLeftKey = 0;
        public bool IsKeyTouch { get { return m_nCurTouchId == KeyTouchId; } }
        public RectTransform mEffectTransfrom;
        //base与move在相同父节点下
        public RectTransform mBase;
        public RectTransform mMove;

        private Canvas m_cCanvas;
        private Camera m_cCanvasCamera;

        private int m_nCurTouchId;
        //初始化锚点坐标
        private Vector2 m_defaultAnchorPosition;
        //初始化屏幕坐标
        private Vector2 m_defaultScreenPosition;
        //开始锚点坐标
        private Vector2 m_startAnchorPosition;
        //开始屏幕坐标
        private Vector2 m_startScreenPosition;
        //最后一次移动的屏幕坐标
        private Vector2 m_preScreenPosition;
        public float moveRadius { get { return m_fMoveRadius; } }
        private float m_fMoveRadius;
        private bool m_bFixedBase;

        private Dictionary<KeyCode, Vector2> m_dicKeyCode = new Dictionary<KeyCode, Vector2>();

        public float radius = 100;
        public bool fixedBase = true;
        void Awake()
        {
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
            m_nCurTouchId = -1;
            m_cCanvas = gameObject.GetComponentInParent<Canvas>();
            m_cCanvasCamera = m_cCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? m_cCanvas.worldCamera : null;
            m_defaultAnchorPosition = mBase.anchoredPosition;
            m_defaultScreenPosition = RectTransformUtility.WorldToScreenPoint(m_cCanvasCamera,mBase.position);
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
                            BeginMovePosition(m_bFixedBase,screenPosition, localPosition);
                            return;
                        }
                    }
                }
                //固定轮盘才允许键盘操作,优先鼠标操作
                //if(m_bFixedBase && m_nCurTouchId < 0)
                if (m_nCurTouchId < 0)
                {
                    Vector2 direct = Vector2.zero;
                    foreach (var item in m_dicKeyCode)
                    {
                        if (Input.GetKey(item.Key))
                        {
                            direct += item.Value;
                        }
                    }
                    if(direct != Vector2.zero)
                    {
                        direct.Normalize();
                        m_nCurTouchId = KeyTouchId;
                        Vector2 localPosition = m_defaultAnchorPosition + direct * m_fMoveRadius;
                        BeginMovePosition(true,localPosition);
                        return;
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
                                        BeginMovePosition(m_bFixedBase,touch.position,localPosition);
                                        return;
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
                        EndMovePosition(m_bFixedBase,screenPosition,localPosition);
                    }
                    else if (Input.GetMouseButton(MouseLeftKey))
                    {
                        Vector3 screenPosition = Input.mousePosition;
                        if (!Mathf.Approximately(m_preScreenPosition.x - screenPosition.x, 0) || !Mathf.Approximately(m_preScreenPosition.y - screenPosition.y, 0))
                        {
                            Vector2 localPosition;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                            UpdateMovePosition(screenPosition, localPosition);
                        }
                    }
                }
                else if(m_nCurTouchId == KeyTouchId)
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
                        Vector2 preDirect = (m_preScreenPosition - m_startScreenPosition);
                        preDirect.Normalize();
                        if (Mathf.Abs(preDirect.x - direct.x) > Epsilon6 || Mathf.Abs(preDirect.y - direct.y) > Epsilon6)
                        {
                            Vector2 localPosition = m_defaultAnchorPosition + direct * m_fMoveRadius;
                            UpdateMovePosition(localPosition);
                        }
                    }
                    else
                    {
                        EndMovePosition(true, m_startScreenPosition,m_startAnchorPosition);
                    }
                }
#else
                if(m_nCurTouchId > -1)
                {
                    if(m_nCurTouchId < Input.touchCount)
                    {
                        Touch touch = Input.GetTouch(m_nCurTouchId);
                        if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            Debug.Log("touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary");
                            Vector3 screenPosition = touch.position;
                            if (!Mathf.Approximately(m_preScreenPosition.x - screenPosition.x, 0) || !Mathf.Approximately(m_preScreenPosition.y - screenPosition.y, 0))
                            {
                                Vector2 localPosition;
                                RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                                UpdateMovePosition(screenPosition,localPosition);
                            }
                        }
                        else
                        {
                            Vector3 screenPosition = touch.position;
                            Vector2 localPosition;
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(mMove.parent as RectTransform, screenPosition, m_cCanvasCamera, out localPosition);
                            EndMovePosition(m_bFixedBase,screenPosition,localPosition);
                        }
                    }
                    else
                    {
                        EndMovePosition(m_bFixedBase,m_startScreenPosition,m_startAnchorPosition);
                    }
                }
#endif
            }
        }
        //在有屏幕坐标的情况下更新base的位置
        private void SetBaseAnchoredPosition(bool fixBase,Vector2 screenPosition, Vector2 anchoredPosition)
        {
            if (!fixBase)
            {
                mBase.anchoredPosition = anchoredPosition;
                m_startScreenPosition = screenPosition;
            }
            m_startAnchorPosition = mBase.anchoredPosition;
        }
        //在没有屏幕坐标的情况下更新base的位置
        private void SetBaseAnchoredPosition(bool fixBase,Vector2 anchoredPosition)
        {
            if (!fixBase)
            {
                mBase.anchoredPosition = anchoredPosition;
            }
            m_startAnchorPosition = mBase.anchoredPosition;
            m_startScreenPosition = RectTransformUtility.WorldToScreenPoint(m_cCanvasCamera, mBase.position);
        }

        private void SetMoveAnchoredPosition(Vector2 position)
        {
            Vector2 offset = position - m_startAnchorPosition;
            float len = offset.magnitude;
            if (len <= m_fMoveRadius)
            {
                mMove.anchoredPosition = position;
            }
            else
            {
                offset.Normalize();
                mMove.anchoredPosition = m_startAnchorPosition + offset * m_fMoveRadius;
            }
        }

        private void EndMovePosition(bool fixBase,Vector2 screenPos, Vector2 anchorPosition)
        {
            m_nCurTouchId = -1;
            SetBaseAnchoredPosition(fixBase,m_defaultScreenPosition, m_defaultAnchorPosition);
            SetMoveAnchoredPosition(m_defaultAnchorPosition);
            //派发取消事件
            if(null != OnEnd)
            {
                OnEnd(screenPos,Vector2.zero,Vector2.zero);
            }
           
        }

        //在有屏幕坐标的情况下开始移动位置
        private void BeginMovePosition(bool fixBase, Vector2 screenPos, Vector2 anchorPosition)
        {
            SetBaseAnchoredPosition(fixBase,screenPos, anchorPosition);
            SetMoveAnchoredPosition(anchorPosition);
            //派发开始遥感事件
            if (null != OnBegin)
            {
                OnBegin(screenPos,screenPos - m_startScreenPosition, Vector2.zero);
            }
            m_preScreenPosition = screenPos;
        }

        //没有屏幕坐标的情况下开始移动位置
        private void BeginMovePosition(bool fixBase, Vector2 anchorPosition)
        {
            SetBaseAnchoredPosition(fixBase,anchorPosition);
            SetMoveAnchoredPosition(anchorPosition);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_cCanvasCamera, mMove.position);
            //派发开始遥感事件
            if (null != OnBegin)
            {
                OnBegin(screenPos, screenPos - m_startScreenPosition, Vector2.zero);
            }
            m_preScreenPosition = screenPos;
        }

        private void UpdateMovePosition(Vector2 screenPos, Vector2 anchorPosition)
        {
            SetMoveAnchoredPosition(anchorPosition);
            //派发移动事件
            if(null != OnMove)
            {
                OnMove(screenPos, screenPos - m_startScreenPosition, screenPos - m_preScreenPosition);
            }
            m_preScreenPosition = screenPos;
        }

        private void UpdateMovePosition(Vector2 anchorPosition)
        {
            SetMoveAnchoredPosition(anchorPosition);
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(m_cCanvasCamera, mMove.position);
            //派发移动事件
            if (null != OnMove)
            {
                OnMove(screenPos, screenPos - m_startScreenPosition, screenPos - m_preScreenPosition);
            }
            m_preScreenPosition = screenPos;
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                EndMovePosition(m_bFixedBase,m_startScreenPosition, m_startAnchorPosition);
            }
        }

        private void OnApplicationQuit()
        {
            EndMovePosition(m_bFixedBase, m_startScreenPosition,m_startAnchorPosition);
        }

        public void Clear()
        {
            OnBegin = null;
            OnMove = null;
            OnEnd = null;
        }
    }
}

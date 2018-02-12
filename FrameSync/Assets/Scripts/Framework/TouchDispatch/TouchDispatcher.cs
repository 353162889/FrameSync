using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class TouchDispatcher : SingletonMonoBehaviour<TouchDispatcher>
    {
        public event Action<TouchEventParam> touchBeganListeners;
        public event Action<TouchEventParam> touchMovedListeners;
        public event Action<TouchEventParam> touchEndedListeners;
        public event Action<TouchEventParam> multiTouchMovedListeners;
        public event Action<TouchEventParam> scrollWheelListeners;
        public bool isMobile;

        private ITouchDetector _detector;
        private TouchEventParam _eventParam;

        private static TouchDispatcher m_cInstance;
        public static TouchDispatcher instance
        {
            get { return m_cInstance; }
        }

        protected override void Init()
        {
            m_cInstance = this;
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR)
            _detector = new PCTouchDetector();
            isMobile = false;
#else
		_detector = new MobileTouchDetector();
		isMobile = true;
#endif

            _eventParam = new TouchEventParam();
            ObjectPool<TouchInfo>.Instance.Init(2);
        }

        void LateUpdate()
        {
            _detector.Update();
        }

        public void OnTouchBegan(Vector2 pos)
        {
            if (touchBeganListeners != null)
            {
                _eventParam.Reset();
                _eventParam.eventType = TouchEventType.TouchBegan;

                TouchInfo touch = ObjectPool<TouchInfo>.Instance.GetObject();
                touch.position = pos;

                _eventParam.AddTouch(touch);


                touchBeganListeners.Invoke(_eventParam);
            }
        }

        public void OnTouchMoved(Vector2 pos, Vector2 deltaPos)
        {
            if (touchMovedListeners != null)
            {
                _eventParam.Reset();
                _eventParam.eventType = TouchEventType.TouchMoved;

                TouchInfo touch = ObjectPool<TouchInfo>.Instance.GetObject();
                touch.position = pos;
                touch.deltaPosition = deltaPos;

                _eventParam.AddTouch(touch);

                touchMovedListeners.Invoke(_eventParam);
            }
        }

        public void OnTouchEnded(Vector2 pos)
        {
            if (touchEndedListeners != null)
            {
                _eventParam.Reset();
                _eventParam.eventType = TouchEventType.TouchEnded;

                TouchInfo touch = ObjectPool<TouchInfo>.Instance.GetObject();
                touch.position = pos;

                _eventParam.AddTouch(touch);

                touchEndedListeners.Invoke(_eventParam);
            }
        }

        public void OnMultiTouchMoved(Touch[] touches)
        {
            if (multiTouchMovedListeners != null)
            {
                _eventParam.Reset();
                _eventParam.eventType = TouchEventType.MultiTouchMoved;

                int count = touches.Length;
                for (int i = 0; i < count; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    TouchInfo info = ObjectPool<TouchInfo>.Instance.GetObject();
                    info.position = touch.position;
                    info.deltaPosition = touch.deltaPosition;
                    info.fingerId = touch.fingerId;
                    _eventParam.AddTouch(info);
                }
                multiTouchMovedListeners.Invoke(_eventParam);
            }
        }

        //dir > 0 鼠标中间滚轮向后滚动
        //dir <0  鼠标中间滚轮向前滚动
        public void OnScrollWheel(float dir)
        {
            if (scrollWheelListeners != null)
            {
                _eventParam.Reset();
                _eventParam.eventType = TouchEventType.ScrollWheel;
                TouchInfo touch = ObjectPool<TouchInfo>.Instance.GetObject();
                touch.position.x = dir;

                _eventParam.AddTouch(touch);

                scrollWheelListeners.Invoke(_eventParam);
            }
        }

    }
}
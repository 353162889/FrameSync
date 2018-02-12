using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
    public class MobileTouchDetector : ITouchDetector
    {
        private int _mainFingerId = -1;

        public void Update()
        {
            int count = Input.touchCount;
            //touch down
            if (count == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    bool isOnUI = (EventSystem.current != null) ? EventSystem.current.IsPointerOverGameObject(touch.fingerId) : false;
                    if (!isOnUI)
                    {
                        _mainFingerId = touch.fingerId;
                        TouchDispatcher.instance.OnTouchBegan(touch.position);
                    }
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.fingerId == _mainFingerId)
                    {
                        TouchDispatcher.instance.OnTouchMoved(touch.position, touch.deltaPosition);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (touch.fingerId == _mainFingerId)
                    {
                        _mainFingerId = -1;
                        TouchDispatcher.instance.OnTouchEnded(touch.position);
                    }
                }
            }
            else if (count > 0)
            {
                //if(_mainFingerId > -1)
                //{
                bool isMainRelease = false;
                int mainIndex = -1;
                bool isAnyMoved = false;
                //touch up
                for (int i = 0; i < count; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.fingerId == _mainFingerId)
                    {
                        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            isMainRelease = true;
                            mainIndex = i;
                            break;
                        }
                    }

                    if ((i < 2) && (touch.phase == TouchPhase.Moved))
                    {
                        isAnyMoved = true;
                    }
                }

                if (isMainRelease)
                {
                    //_mainFingerId = -1;
                    TouchDispatcher.instance.OnTouchEnded(Input.GetTouch(mainIndex).position);
                }
                else if (isAnyMoved)
                {
                    TouchDispatcher.instance.OnMultiTouchMoved(Input.touches);
                }

                //}
            }
        }
    }

}
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
    public class PCTouchDetector : ITouchDetector
    {
        public static readonly int MouseLeftKey = 0;
        public static readonly string MouseXKey = "Mouse X";
        public static readonly string MouseYKey = "Mouse Y";
        public static readonly string MouseScrollWheelKey = "Mouse ScrollWheel";

        private bool _isTouchDown;
        private Vector2 _preMousePos;

        public void Update()
        {
            bool isOnUI = (EventSystem.current != null) ? EventSystem.current.IsPointerOverGameObject() : false;
            if (!isOnUI && Input.GetMouseButtonDown(MouseLeftKey))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                TouchDispatcher.instance.OnTouchBegan(pos);
                _preMousePos = pos;
                _isTouchDown = true;
            }

            if (_isTouchDown && Input.GetMouseButtonUp(MouseLeftKey))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                TouchDispatcher.instance.OnTouchEnded(pos);
                _preMousePos = Vector2.zero;
                _isTouchDown = false;
            }

            if (_isTouchDown && Input.GetMouseButton(MouseLeftKey))
            {
                Vector2 mousePositon = Input.mousePosition;
                Vector2 delta = mousePositon - _preMousePos;
                _preMousePos = mousePositon;
                if (delta.x != 0f || delta.y != 0f)
                {
                    Vector2 pos = new Vector2(mousePositon.x, mousePositon.y);
                    Vector2 deltaPos = new Vector2(delta.x, delta.y);
                    TouchDispatcher.instance.OnTouchMoved(pos, deltaPos);
                }
            }

            if (!isOnUI && Input.GetAxis(MouseScrollWheelKey) != 0)
            {
                TouchDispatcher.instance.OnScrollWheel(Input.GetAxis(MouseScrollWheelKey));
            }

        }

    }

}
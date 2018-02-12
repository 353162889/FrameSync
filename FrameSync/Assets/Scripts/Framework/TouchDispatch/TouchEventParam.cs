using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public enum TouchEventType
    {
        TouchBegan,
        TouchMoved,
        TouchEnded,
        MultiTouchMoved,
        ScrollWheel
    }

    public class TouchInfo : IPoolable
    {
        public Vector2 position;
        public Vector2 deltaPosition;
        public int fingerId;
        public void Reset()
        {
            position.x = 0;
            position.y = 0;
            deltaPosition.x = 0;
            deltaPosition.x = 0;
        }
    }

    public class TouchEventParam
    {
        public TouchEventType eventType;
        private List<TouchInfo> _touchs;
        private List<TouchInfo> _cachedInfos;
        public TouchEventParam()
        {
            _touchs = new List<TouchInfo>();
        }

        public void AddTouch(TouchInfo info)
        {
            _touchs.Add(info);
        }

        public int TouchCount
        {
            get
            {
                return _touchs.Count;
            }
        }

        public TouchInfo GetTouch(int index)
        {
            return _touchs[index];
        }

        public void Reset()
        {
            int count = _touchs.Count;
            for (int i = count - 1; i > -1; --i)
            {
                ObjectPool<TouchInfo>.Instance.SaveObject(_touchs[i]);
            }
            _touchs.Clear();
        }


    }
}
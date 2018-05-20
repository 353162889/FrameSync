using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class BehaviourPool<T> : Singleton<BehaviourPool<T>> where T : MonoBehaviour,IPoolable
    {
        private Queue<T> _pool;
        private int _capicity;
        private bool _inited;

        public void Init(int capicity)
        {
            if (!_inited)
            {
                this._capicity = capicity;
                _pool = new Queue<T>(_capicity);
                _inited = true;
            }
        }

        public void SetCapicaty(int capicity)
        {
            this._capicity = capicity;
        }

        public T GetObject(Transform parent,params object[] param)
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                GameObject go = new GameObject();
                obj = go.AddComponentOnce<T>();
            }
            if(parent != null)
            {
                GameObjectUtil.AddChildToParent(parent.gameObject, obj.gameObject);
            }
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void SaveObject(T obj, Transform parent = null)
        {
            obj.Reset();
            obj.gameObject.SetActive(false);
            if(parent != null)
            {
                GameObjectUtil.AddChildToParent(parent.gameObject, obj.gameObject);
            }
            if (_pool.Count < _capicity)
            {
                _pool.Enqueue(obj);
            }
            else
            {
                CLog.Log("<color='yellow'>" + typeof(T) + " over capicity:" + _capicity + "</color>");
            }
        }

        public override void Dispose()
        {
            _inited = false;
            base.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework
{
	public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : IPoolable
	{
		private Queue<T> _pool;
		private int _capicity;
        private bool _autoIncrease;
		private bool _inited;
        public bool inited { get { return _inited; } }

        public void Init(int capicity, bool autoIncrease = false)
		{
			if (!_inited)
			{
				this._capicity = capicity;
                this._autoIncrease = autoIncrease;
				_pool = new Queue<T> (_capicity);
                CacheObject(_capicity / 2);
                _inited = true;
			}
		}

		public void SetCapicaty(int capicity)
		{
			this._capicity = capicity;
		}

		public T GetObject(params object[] param)
		{
			T obj;
			if (_pool.Count > 0)
			{
				obj = _pool.Dequeue ();
			}
			else
			{
				obj = (T)Activator.CreateInstance (typeof(T), param);
			}
			return obj;
		}

		public void SaveObject(T obj)
		{
			obj.Reset ();
			if (_pool.Count < _capicity)
			{
				_pool.Enqueue (obj);
			}
			else
			{
                if (_autoIncrease)
                {
                    _capicity++;
                    _pool.Enqueue(obj);
                }
                else
                {
                    CLog.Log("<color='yellow'>" + typeof(T) + " over capicity:" + _capicity + "</color>");
                }
			}
		}

        public void CacheObject(int count, params object[] param)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), param);
            SaveObject(obj);
        }

        public override void Dispose ()
		{
			_inited = false;
			base.Dispose ();
		}
	}
}


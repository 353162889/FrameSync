using UnityEngine;
using System;

namespace Framework
{
	/**
	 * 该单例需要绑定某个对象，不确定该脚本是否存在的情况下，可用Exists 判断
	 * */
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		private static T uniqueInstance;
		
		public static T Instance
		{
			get
			{
				return uniqueInstance;
			}
		}
		
		protected virtual void Awake()
		{
			if (uniqueInstance == null)
			{
				uniqueInstance = (T)this;
				Exists = true;
				GameObject.DontDestroyOnLoad(this);
                uniqueInstance.Init();
            }
			else if (uniqueInstance != this)
			{
				throw new InvalidOperationException("Cannot have two instances of a SingletonMonoBehaviour : " + typeof(T).ToString() + ".");
			}
		}

        protected virtual void Init()
        {
        }

        protected virtual void OnDestroy()
		{
			if (uniqueInstance == this)
			{
				Exists = false;
				uniqueInstance = null;
			}
		}
		
		public static bool Exists
		{
			get;
			private set;
		}
	}
}
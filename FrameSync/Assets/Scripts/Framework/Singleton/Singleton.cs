using System.Diagnostics;

namespace Framework
{
	public class Singleton<T> where T: new()
	{
		private static readonly object _lock = new object();
		private static T _instance;
		
		protected Singleton()
		{
			Debug.Assert(_instance == null);
		}
		
		public static bool Exists
		{
			get
			{
				return _instance != null;
			}
		}
		
		public static T Instance
		{
			get {
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new T();
						}
					}
				}
				return _instance;
			}
		}

        public virtual void Dispose()
        {
            _instance = default(T);
        }
    }
}
using System;
using UnityEngine;

namespace Framework
{
	public static class GameObjectUtil
	{
		public static T AddComponentOnce<T>(this GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (t == null)
			{
				t = go.AddComponent<T> ();
			}
			return t;
		}

		public static void AddChildToParent(this GameObject go,GameObject child,string name = null, bool worldPositionStays = false)
		{
			child.transform.SetParent (go.transform,worldPositionStays);
            if(!name.IsEmpty())
            {
                child.name = name;
            }
		}

        public static void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }

        public static void DestroyChildren(this GameObject go)
        {
            if (go != null)
            {
                int count = go.transform.childCount;
                for (int i = count - 1; i >= 0; i--)
                {
                    Destroy(go.transform.GetChild(i).gameObject);
                }
            }
        }

		public static void SetLayerRecursive(this GameObject go,int layer)
		{
			if (go != null)
			{
				go.layer = layer;
				int count = go.transform.childCount;
				for (int i = 0; i < count; i++)
				{
					GameObject child = go.transform.GetChild (i).gameObject;
					child.SetLayerRecursive (layer);
				}
			}
		}

		public static GameObject FindChildRecursive(this GameObject go,string name)
		{
			if (go != null)
			{
				Transform child = go.transform.Find (name);
				if (child != null)
				{
					return child.gameObject;
				}
				int count = go.transform.childCount;
				for (int i = 0; i < count; i++)
				{
					GameObject childGO = go.transform.GetChild (i).gameObject;
					GameObject result = childGO.FindChildRecursive (name);
					if (result != null)
						return result;
				}
			}
			return null;
		}

		public static T FindChildComponentRecursive<T>(this GameObject go,string name) where T:Component
		{
			GameObject child = go.FindChildRecursive (name);
			if (child != null)
			{
				return child.GetComponent<T> ();
			}
			return null;
		}
	}

}


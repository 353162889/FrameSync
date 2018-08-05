using System;
using System.Collections.Generic;
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

        public static Component AddComponentOnce(this GameObject go, Type type)
        {
            if (go == null) return null;
            Component comp = go.GetComponent(type);
            if (comp == null) return go.AddComponent(type);
            return comp;
        }

        public static Component AddComponentOnce(this GameObject go, string name)
        {
            Type type = Type.GetType(name);
            if (type == null)
            {
                throw new Exception("找不到类型名为" + name + "类型");
            }
            return AddComponentOnce(go, type);
        }

        public static void AddChildToParent(this GameObject go,GameObject child,string name = null, bool worldPositionStays = false)
		{
			child.transform.SetParent (go.transform,worldPositionStays);
            if (!worldPositionStays)
            {
                child.transform.localPosition = Vector3.zero;
                child.transform.localEulerAngles = Vector3.zero;
                child.transform.localScale = Vector3.one;
            }
            if (!name.IsEmpty())
            {
                child.name = name;
            }
		}

        public static void Reset(this GameObject go)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
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

        public static GameObject[] GetChildren(this GameObject root)
        {
            int count = root.transform.childCount;
            GameObject[] objs = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                Transform tf = root.transform.GetChild(i);
                objs[i] = tf.gameObject;
            }
            return objs;
        }

        private static List<GameObject> m_lstGOS = new List<GameObject>();
        public static GameObject[] GetChildren(this GameObject root, bool visiable)
        {
            if (!visiable) return root.GetChildren();
            m_lstGOS.Clear();
            int count = root.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject go = root.transform.GetChild(i).gameObject;
                if (go.activeSelf)
                {
                    m_lstGOS.Add(go);
                }
            }
            GameObject[] result = m_lstGOS.ToArray();
            m_lstGOS.Clear();
            return result;
        }

        public static T FindInParents<T>(this GameObject go) where T : Component
        {
            if (go == null) return null;
            var comp = go.GetComponent<T>();

            if (comp != null)
                return comp;

            Transform t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
            return comp;
        }
    }

}


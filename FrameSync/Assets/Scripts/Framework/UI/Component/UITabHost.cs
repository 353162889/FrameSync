using System;
using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 不允许直接挂在GameObject上
    /// </summary>
    public class UITabHost : MonoBehaviour
    {
        public delegate void UITabHostHandler(int index);
        public delegate bool UITabHostResultHandler(int index);
        public UITab[] tabs { get; private set; }
        private List<UITabHostHandler> listOnTabClick = new List<UITabHostHandler>();
        private List<UITabHostResultHandler> listCanTabSelect = new List<UITabHostResultHandler>();
        private List<UITabHostHandler> listOnTabSelect = new List<UITabHostHandler>();
        private int curSelect = -1;
        public int CurSelect
        {
            get
            {
                return curSelect;
            }
        }

        /// <summary>
        /// 手动掉下初始化方法
        /// </summary>
        public void Init<T>(int selected = 0) where T : UITab
        {
            GameObject[] gos = gameObject.GetChildren(true);
            int count = gos == null ? 0 : gos.Length;
            tabs = new T[count];
            for (int i = 0; i < count; i++)
            {
                tabs[i] = gos[i].AddComponentOnce<T>();
                tabs[i].Init(i);
                tabs[i].IsSelected = (selected == i);
                tabs[i].onTabChange += OnTabClick;
            }
            curSelect = selected;
        }

        public void Init(Type type, int selected = 0)
        {
            GameObject[] gos = gameObject.GetChildren(true);
            int count = gos == null ? 0 : gos.Length;
            tabs = new UITab[count];
            for (int i = 0; i < count; i++)
            {
                tabs[i] = (UITab)gos[i].AddComponentOnce(type);
                tabs[i].Init(i);
                tabs[i].IsSelected = (selected == i);
                tabs[i].onTabChange += OnTabClick;
            }
            curSelect = selected;
        }

        public void Init(int selected = 0)
        {
            GameObject[] gos = gameObject.GetChildren(true);
            int count = gos == null ? 0 : gos.Length;
            tabs = new UITab[count];
            for (int i = 0; i < count; i++)
            {
                tabs[i] = gos[i].AddComponentOnce<UITab>();
                tabs[i].Init(i);
                tabs[i].IsSelected = (selected == i);
                tabs[i].onTabChange += OnTabClick;
            }
            curSelect = selected;
        }

        public void Reset(int selected = 0)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].IsSelected = (selected == i);
            }
            curSelect = selected;
        }

        public void OnUpdate()
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].OnUpdate();
            }
        }

        private void OnTabClick(UITab tab)
        {
            int index = -1;
            for (int i = tabs.Length - 1; i >= 0; --i)
            {
                if (tabs[i] == tab)
                {
                    index = i;
                    break;
                }
            }
            int oldSelect = curSelect;
            for (int i = listOnTabClick.Count - 1; i >= 0; --i)
            {
                listOnTabClick[i].Invoke(index);
            }
            if (oldSelect != index)
            {
                if (LocalCanTabSelect(index))
                {
                    SetSelect(index);
                }
            }
        }

        private bool LocalCanTabSelect(int index)
        {
            for (int i = listCanTabSelect.Count - 1; i >= 0; --i)
            {
                if (!listCanTabSelect[i].Invoke(index))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetSelect(int index)
        {
            if (curSelect != -1)
            {
                tabs[curSelect].IsSelected = false;
            }
            curSelect = index;
            if (curSelect != -1)
            {
                tabs[curSelect].IsSelected = true;
            }
            for (int i = listOnTabSelect.Count - 1; i >= 0; --i)
            {
                listOnTabSelect[i].Invoke(curSelect);
            }
        }

        public void OnDestroy()
        {
            if (listCanTabSelect != null)
            {
                listCanTabSelect.Clear();
                listCanTabSelect = null;
            }
            if (listOnTabSelect != null)
            {
                listOnTabSelect.Clear();
                listOnTabSelect = null;
            }
            if (listOnTabClick != null)
            {
                listOnTabClick.Clear();
                listOnTabClick = null;
            }
            if (tabs != null)
            {
                for (int i = tabs.Length - 1; i >= 0; --i)
                {
                    tabs[i].OnDestroy();
                }
            }
            tabs = null;
        }

        public void AddCanTabSelectListener(UITabHostResultHandler listener)
        {
            if (listener == null) return;
            listCanTabSelect.Add(listener);
        }

        public void RemoveCanTabSelectListener(UITabHostResultHandler listener)
        {
            if (listener == null) return;
            listCanTabSelect.Remove(listener);
        }

        public void AddOnTabSelectListener(UITabHostHandler listener)
        {
            if (listener == null) return;
            listOnTabSelect.Add(listener);
        }

        public void RemoveOnTabSelectListener(UITabHostHandler listener)
        {
            if (listener == null) return;
            listOnTabSelect.Remove(listener);
        }

        public void AddOnTabClickListener(UITabHostHandler listener)
        {
            if (listener == null) return;
            listOnTabClick.Add(listener);
        }

        public void RemoveOnTabClickListener(UITabHostHandler listener)
        {
            if (listener == null) return;
            listOnTabClick.Remove(listener);
        }

    }

}
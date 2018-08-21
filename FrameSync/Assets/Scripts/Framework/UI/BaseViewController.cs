using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Framework
{
    public enum ViewOptype
    {
        None,
        Open,
        Close,
    }

    public enum ViewStatus
    {
        None,
        Initing,
        Openning,
        Closing,
        Open,
        Close
    }

    public class UIViewMono : MonoBehaviour
    {
        private List<UIGraphicRaycaster> m_lstRaycaster;
        public List<UIGraphicRaycaster> lstRaycaster { get { return m_lstRaycaster; } }

        void Awake()
        {
            m_lstRaycaster = new List<UIGraphicRaycaster>();
            FindOrAddUIGraphicRaycaster(this.transform, m_lstRaycaster);
        }

        void OnDestroy()
        {
            m_lstRaycaster.Clear();
        }

        public void FindOrAddUIGraphicRaycaster(Transform trans, List<UIGraphicRaycaster> lst)
        {
            var canvas = trans.GetComponent<Canvas>();
            if (canvas != null)
            {
                var uiRaycaster = trans.gameObject.AddComponentOnce<UIGraphicRaycaster>();
                lst.Add(uiRaycaster);
            }
            int count = trans.childCount;
            for (int i = 0; i < count; i++)
            {
                var child = trans.GetChild(i);
                FindOrAddUIGraphicRaycaster(child, lst);
            }
        }
    }


    public class BaseViewController : IDynamicObj
    {
        protected ViewOptype m_eOpStatus = ViewOptype.None;

        protected ViewStatus m_eStatus = ViewStatus.None;
        public ViewStatus status { get { return m_eStatus; } }

        protected ViewParam m_cOpenParam = new ViewParam();
        public ViewParam openParam { get { return m_cOpenParam; } }

        private string m_sViewName;
        private int m_iHashCode;
        public string viewName { get { return m_sViewName; } set { m_sViewName = value; m_iHashCode = m_sViewName.GetHashCode(); } }
        public string viewPath { get; set; }

        public List<BaseSubView> _subViews;

        public GameObject MainGO { get; private set; }

        protected Resource m_cResource;

        public bool isOpen
        {
            get
            {
                if (m_eStatus == ViewStatus.Open || m_eStatus == ViewStatus.Openning)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool isActive
        {
            get
            {
                if (m_eStatus == ViewStatus.Open || m_eStatus == ViewStatus.Openning || m_eStatus == ViewStatus.Closing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int key
        {
            get
            {
                return m_iHashCode;
            }
        }

        private void OnVisualEvent(int name)
        {
            if (_subViews == null) return;

            int count = _subViews.Count;
            for (int i = 0; i < count; ++i)
            {
                switch (name)
                {
                    case 0:
                        _subViews[i].OnEnter(m_cOpenParam);
                        break;
                    case 1:
                        _subViews[i].OnEnterFinished();
                        break;
                    case 2:
                        _subViews[i].OnExit();
                        break;
                    case 3:
                        _subViews[i].OnExitFinished();
                        break;
                }
            }
        }

        private void OnResLoad()
        {
            if (m_eOpStatus == ViewOptype.Open)
            {
                m_eOpStatus = ViewOptype.None;
                this.DoOpen();
            }
            else if (m_eOpStatus == ViewOptype.Close)
            {
                m_eOpStatus = ViewOptype.None;
                this.m_eStatus = ViewStatus.Closing;
            }
        }

        private void DoInit()
        {
            this.m_eStatus = ViewStatus.Initing;
            //加载资源
            ResourceSys.Instance.GetResource(viewPath, OnResourceLoaded);
        }

        private void OnResourceLoaded(Resource res,string path)
        {
            if (m_cResource != null) m_cResource.Release();
            m_cResource = res;
            m_cResource.Retain();
            UnityEngine.Object objPrefab = m_cResource.GetAsset(path);
            this.MainGO = GameObject.Instantiate(objPrefab) as GameObject;
           
            if (this.MainGO != null)
            {
                this.MainGO.transform.SetParent(ViewSys.Instance.Root2D.transform, false);
                this.MainGO.name = this.viewName;
            }

            this._subViews = this.BuildSubViews();
            if (this._subViews != null)
            {
                int count = this._subViews.Count;
                for (int i = 0; i < count; ++i)
                {
                    this._subViews[i].viewController = this;
                }
            }

            if (this.MainGO != null)
            {
                Canvas canvas = this.MainGO.AddComponentOnce<Canvas>();
                canvas.overrideSorting = true;
                this.MainGO.AddComponentOnce<UIViewMono>();
                canvas.sortingOrder = 0;
            }

            if (this.MainGO != null)
            {
                this.MainGO.SetActive(false);
            }
            OnResLoad();
        }

        private void SortViewPosition()
        {
            Transform parent = this.MainGO.transform.parent;
            int childCount = parent.childCount;
            int index = 0;
            for (int i = childCount - 1; i > -1; i--)
            {
                Transform child = parent.GetChild(i);
                if (!child.gameObject.activeSelf) continue;
                float z = index * 1000f;
                Vector3 pos = child.localPosition;
                child.localPosition = new Vector3(pos.x, pos.y, z);
                var viewMono = child.GetComponent<UIViewMono>();
                var lstRaycaster = viewMono.lstRaycaster;
                for (int j = 0; j < lstRaycaster.Count; j++)
                {
                    var raycaster = lstRaycaster[j];
                    var position = child.InverseTransformPoint(raycaster.transform.position);
                    raycaster.priority = (int)-parent.localPosition.z + i * 1000 - (int)position.z;
                }
                index++;
            }

        }

        private void DoOpen()
        {
            this.m_eStatus = ViewStatus.Openning;
            this.MainGO.SetActive(true);
            this.MainGO.transform.SetAsLastSibling();
            SortViewPosition();

            this.OnVisualEvent(0);
            this.PlayEnterAnimation();
        }

        private void DoClose()
        {
            this.m_eStatus = ViewStatus.Closing;
            this.OnVisualEvent(2);
            this.PlayCloseAnimation();
        }

        public void Open(ViewParam param)
        {
            switch (m_eStatus)
            {
                case ViewStatus.None:
                    m_cOpenParam.Copy(param);
                    m_eOpStatus = ViewOptype.Open;
                    this.DoInit();
                    break;
                case ViewStatus.Initing:
                    m_cOpenParam.Copy(param);
                    m_eOpStatus = ViewOptype.Open;
                    break;
                case ViewStatus.Open:
                    //m_eOpStatus = ViewOptype.None;
                    //DoOpen();
                    break;
                case ViewStatus.Openning:
                    m_eOpStatus = ViewOptype.None;
                    break;
                case ViewStatus.Close:
                    m_eOpStatus = ViewOptype.None;
                    m_cOpenParam.Copy(param);
                    this.DoOpen();
                    break;
                case ViewStatus.Closing:
                    m_eOpStatus = ViewOptype.Open;
                    break;
            }
        }

        public void Close()
        {
            switch (m_eStatus)
            {
                case ViewStatus.None:
                    m_eOpStatus = ViewOptype.None;
                    break;
                case ViewStatus.Initing:
                    m_eOpStatus = ViewOptype.Close;
                    break;
                case ViewStatus.Open:
                    m_eOpStatus = ViewOptype.None;
                    this.DoClose();
                    break;
                case ViewStatus.Openning:
                    m_eOpStatus = ViewOptype.Close;
                    break;
                case ViewStatus.Close:
                    m_eOpStatus = ViewOptype.None;
                    break;
                case ViewStatus.Closing:
                    m_eOpStatus = ViewOptype.None;
                    break;
            }
        }

        public virtual void Destroy()
        {
            //如果资源没有加载玩，移除加载毁掉监听

            //如果动画没有播放玩，结束动画异步   
            OnDestroyAnimation();
            if (_subViews != null)
            {
                int len = _subViews.Count;
                for (int i = 0; i < len; ++i)
                {
                    _subViews[i].UnbindEvents();
                    _subViews[i].DestroyUI();
                }
            }
            if (this.MainGO != null)
            {
                GameObject.Destroy(this.MainGO);
            }
            if(m_cResource != null)
            {
                m_cResource.Release();
                m_cResource = null;
            }
        }

        protected virtual void OnDestroyAnimation()
        {

        }

        protected void OnEnterAnimationDone()
        {
            this.OnVisualEvent(1);
            this.m_eStatus = ViewStatus.Open;
            if (m_eOpStatus == ViewOptype.Close)
            {
                this.DoClose();
            }
        }

        protected void OnCloseAnimationDone()
        {
            this.OnVisualEvent(3);
            this.m_eStatus = ViewStatus.Close;
            this.MainGO.SetActive(false);
            if (m_eOpStatus == ViewOptype.Open)
            {
                this.DoOpen();
            }
        }

        protected virtual void PlayEnterAnimation()
        {
            OnEnterAnimationDone();
        }

        protected virtual void PlayCloseAnimation()
        {
            OnCloseAnimationDone();
        }

        protected virtual List<BaseSubView> BuildSubViews()
        {
            //默认只有一个view，支持重写BuildSubViews
            Type type = Type.GetType("Game."+viewName);
            if (type == null)
            {
                return null;
            }
            BaseSubView subView = Activator.CreateInstance(type, new object[] { this.MainGO }) as BaseSubView;

            List<BaseSubView> list = new List<BaseSubView>();
            list.Add(subView);
            return list;
        }

        public void CloseThis()
        {
            ViewSys.Instance.Close(viewName);
        }

        public virtual void OnUpdate()
        {
            if (_subViews != null)
            {
                int len = _subViews.Count;
                for (int i = 0; i < len; ++i)
                {
                    _subViews[i].OnUpdate();
                }
            }
        }
    }
}
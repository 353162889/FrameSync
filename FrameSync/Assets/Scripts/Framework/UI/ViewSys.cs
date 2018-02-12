using UnityEngine;
using System.Collections.Generic;
using System.Security;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;
using System;
using System.Linq;

namespace Framework
{
    public class ViewSys : SingletonMonoBehaviour<ViewSys>
    {

        private GameObject m_root2D;//canvas

        public GameObject Root2D { get { return m_root2D; } }

        private RectTransform m_transRoot2D;

        public float uiScreenWidth
        {
            get { return m_transRoot2D.rect.width; }
        }

        public float uiScreenHeight
        {
            get { return m_transRoot2D.rect.height; }
        }

        private Canvas m_cCanvas;
        public Canvas canvas
        {
            get { return m_cCanvas; }
        }

        private Dictionary<string, BaseViewController> _views = new Dictionary<string, BaseViewController>();
        private DynamicContainer m_cContainer;

        protected override void Init()
        {
            m_root2D = this.gameObject;
            m_transRoot2D = m_root2D.transform as RectTransform;
            m_cCanvas = m_root2D.GetComponent<Canvas>();

            m_cContainer = new DynamicContainer();
            m_cContainer.OnAdd += OnAdd;
            m_cContainer.OnRemove += OnRemove;
            m_cContainer.OnUpdate += OnUpdate;
        }

        private void OnUpdate(IDynamicObj obj, object param)
        {
            BaseViewController controller = obj as BaseViewController;
            if (controller.isActive)
            {
                controller.OnUpdate();
            }
        }

        private void OnRemove(IDynamicObj obj, object param)
        {
            BaseViewController controller = obj as BaseViewController;
            if (controller != null)
            {
                controller.Destroy();
            }
            _views.Remove(name);
        }

        private void OnAdd(IDynamicObj obj, object param)
        {
            BaseViewController controller = obj as BaseViewController;
            ViewParam p = param as ViewParam;
            _views.Add(controller.viewName, controller);
            controller.Open(p);
        }

        void Update()
        {
            if (m_cContainer != null)
            {
                m_cContainer.Update();
            }
        }

        public void DestoryAll()
        {
            m_cContainer.Clear();
            _views.Clear();
        }

        /**
            * 主动销毁一个界面
            * */
        public void Destroy(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            m_cContainer.RemoveKey(name.GetHashCode());
        }

        public void Open(string name, ViewParam param = null)
        {
            if (!_views.ContainsKey(name))
            {
                string clsName = name + "Controller";
                Type type = Type.GetType(clsName);
                if (type == null)
                {
                    type = typeof(BaseViewController);
                }
                object obj = Activator.CreateInstance(type);
                if (obj == null)
                {
                    CLog.LogError("Cannot find class with type [" + type + "].");
                    return;
                }
                BaseViewController controller = obj as BaseViewController;
                controller.viewName = name;
                //_views.Add(name, controller);
                m_cContainer.Add(controller, param);
            }
            else
            {
                _views[name].Open(param);
            }
        }

        public void Close(string name)
        {
            if (!_views.ContainsKey(name))
            {
                return;
            }

            _views[name].Close();
        }

        public bool IsOpen(string name)
        {
            if (!_views.ContainsKey(name))
            {
                return false;
            }
            return _views[name].isOpen;
        }
    }
}
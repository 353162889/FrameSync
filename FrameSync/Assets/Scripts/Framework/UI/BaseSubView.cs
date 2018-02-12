using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class BaseSubView
    {
        protected List<BaseSubView> listSubView;

        public GameObject MainGO { get; private set; }

        private BaseViewController controller;
        public virtual BaseViewController viewController
        {
            get
            {
                return controller;
            }
            set
            {
                controller = value;
                if (listSubView != null)
                {
                    int subViewCount = listSubView.Count;
                    for (int i = 0; i < subViewCount; ++i)
                    {
                        listSubView[i].viewController = controller;
                    }
                }
            }
        }

        public BaseSubView(GameObject go)
        {
            MainGO = go;

            BuidUI();
            BindEvents();

            InitSubViews();
        }

        //初始化子面板时需要调用的函数(初始化这个listSubView)
        protected virtual void InitSubViews()
        {

        }

        protected virtual void BuidUI()
        {
        }

        protected virtual void BindEvents()
        {

        }

        public virtual void UnbindEvents()
        {

        }

        public virtual void DestroyUI()
        {

        }

        public virtual void OnUpdate()
        {
            int subViewCount = listSubView == null ? 0 : listSubView.Count;
            for (int i = 0; i < subViewCount; ++i)
            {
                listSubView[i].OnUpdate();
            }
        }

        public virtual void OnEnter(ViewParam openParam)
        {
            int subViewCount = listSubView == null ? 0 : listSubView.Count;
            for (int i = 0; i < subViewCount; ++i)
            {
                listSubView[i].OnEnter(openParam);
            }
        }

        public virtual void OnEnterFinished()
        {
            int subViewCount = listSubView == null ? 0 : listSubView.Count;
            for (int i = 0; i < subViewCount; ++i)
            {
                listSubView[i].OnEnterFinished();
            }
        }

        public virtual void OnExit()
        {
            int subViewCount = listSubView == null ? 0 : listSubView.Count;
            for (int i = 0; i < subViewCount; ++i)
            {
                listSubView[i].OnExit();
            }
        }

        public virtual void OnExitFinished()
        {
            int subViewCount = listSubView == null ? 0 : listSubView.Count;
            for (int i = 0; i < subViewCount; ++i)
            {
                listSubView[i].OnExitFinished();
            }
        }

        public virtual void CloseThis()
        {
            viewController.CloseThis();
        }
    }
}
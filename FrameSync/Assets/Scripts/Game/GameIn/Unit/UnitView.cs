using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public partial class Unit
    {
        private GameObject m_cView;
        protected void InitView()
        {
            GameObjectPool.Instance.GetObject(m_sPrefab, OnResLoad);
        }

        private void OnResLoad(GameObject go)
        {
            m_cView = go;
            this.gameObject.AddChildToParent(m_cView);
        }

        protected void ResetView()
        {
            if(m_cView != null)
            {
                GameObjectPool.Instance.SaveObject(m_sPrefab, m_cView);
                m_cView = null;
            }
            GameObjectPool.Instance.RemoveCallback(m_sPrefab, OnResLoad);
        }

        protected void DisposeView()
        {
            if (m_cView != null)
            {
                GameObjectPool.Instance.SaveObject(m_sPrefab, m_cView);
                m_cView = null;
            }
            GameObjectPool.Instance.RemoveCallback(m_sPrefab, OnResLoad);
        }

        protected void SetViewPosition(TSVector position)
        {
            transform.position = position.ToUnityVector3();
        }

        protected void SetViewForward(TSVector forward)
        {
            transform.forward = forward.ToUnityVector3();
        }

        protected void UpdateView(FP deltaTime)
        {

        }

    }
}

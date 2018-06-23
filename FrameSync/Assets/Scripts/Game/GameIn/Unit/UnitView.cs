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
        private HangPoint m_cHangPoint;

        public Transform GetHangPoint(string name, out TSVector position, out TSVector forward)
        {
            return m_cHangPoint.GetHangPoint(name, curPosition, curForward, out position, out forward);
        }

        protected void InitView()
        {
            m_cHangPoint = gameObject.AddComponentOnce<HangPoint>();
            m_cHangPoint.Init(m_sPrefab);
            GameObjectPool.Instance.GetObject(m_sPrefab, OnResLoad);
        }

        private void OnResLoad(GameObject go)
        {
            m_cView = go;
            this.gameObject.AddChildToParent(m_cView);
            HangPointView hangPointView = m_cView.GetComponent<HangPointView>();
            m_cHangPoint.InitHangView(hangPointView);
        }

        protected void ResetView()
        {
            m_cHangPoint.Clear();
            if(m_cView != null)
            {
                GameObjectPool.Instance.SaveObject(m_sPrefab, m_cView);
                m_cView = null;
            }
            GameObjectPool.Instance.RemoveCallback(m_sPrefab, OnResLoad);
        }

        protected void DisposeView()
        {
            m_cHangPoint.Clear();
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

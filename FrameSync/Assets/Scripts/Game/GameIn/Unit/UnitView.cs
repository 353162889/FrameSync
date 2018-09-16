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
        public GameCollider gameCollider { get { return m_cCollider; } }
        private GameCollider m_cCollider;
        private AgentAnimation m_cAnimation;
        private SimpleLerpView m_cSimpleLerpView;
        private LerpRotateView m_cLerpRotateView;

        public Transform GetHangPoint(string name, out TSVector position, out TSVector forward)
        {
            if(m_cHangPoint == null)
            {
                position = curPosition;
                forward = curForward;
                return null;
            }
            return m_cHangPoint.GetHangPoint(name, curPosition, curForward, out position, out forward);
        }

        public Transform GetHangPoint(string name,TSVector cPosition,TSVector cForward,out TSVector position,out TSVector forward)
        {
            if (m_cHangPoint == null)
            {
                position = cPosition;
                forward = cForward;
                return null;
            }
            return m_cHangPoint.GetHangPoint(name, cPosition, cForward, out position, out forward);
        }

        public void SetColliderEnable(bool enable)
        {
            if(m_cCollider != null)
            {
                m_cCollider.SetEnable(enable);
            }
        }

        protected void InitView()
        {
            m_cHangPoint = gameObject.AddComponentOnce<HangPoint>();
            m_cHangPoint.Init(PathTool.GetBasePrefabPath(m_sPrefab));
            m_cCollider = ObjectPool<GameCollider>.Instance.GetObject();
            m_cCollider.Init(curPosition,PathTool.GetBasePrefabPath(m_sPrefab));
            m_cCollider.Update(curPosition, curForward);

            if(m_cAnimation == null)
            {
                m_cAnimation = new AgentAnimation();
            }
            m_cSimpleLerpView = gameObject.AddComponentOnce<SimpleLerpView>();
            m_cSimpleLerpView.Stop();

            m_cLerpRotateView = gameObject.AddComponentOnce<LerpRotateView>();
            m_cLerpRotateView.Stop();

            SceneGOPool.Instance.GetObject(m_sPrefab, false,OnResLoad);
        }

        private void OnResLoad(string path, UnityEngine.Object go)
        {
            m_cView = (GameObject)go;
            this.gameObject.AddChildToParent(m_cView);
            HangPointView hangPointView = m_cView.GetComponent<HangPointView>();
            m_cHangPoint.InitHangView(hangPointView);
            var animator = m_cView.GetComponentInChildren<Animator>();
            if(animator != null)
            {
                m_cAnimation.Init(animator);
            }
            m_cAnimation.ResetParam();
        }

        protected void ResetView()
        {
            if (m_cCollider != null)
            {
                ObjectPool<GameCollider>.Instance.SaveObject(m_cCollider);
                m_cCollider = null;
            }
            if (m_cHangPoint != null)
            {
                m_cHangPoint.Clear();
            }
            if (m_cAnimation != null)
            {
                m_cAnimation.Clear();
            }
            if (m_cView != null)
            {
                SceneGOPool.Instance.SaveObject(m_sPrefab, m_cView);
                m_cView = null;
            }
            if (m_cSimpleLerpView != null)
            {
                m_cSimpleLerpView.Stop();
                m_cSimpleLerpView = null;
            }
            SceneGOPool.Instance.RemoveCallback(m_sPrefab, OnResLoad);
        }

        protected void SetViewPosition(TSVector position,bool immediately)
        {
            if (immediately || m_cSimpleLerpView == null)
            {
                transform.position = position.ToUnityVector3();
            }
            else
            {
                m_cSimpleLerpView.LerpTo(position.ToUnityVector3(), ViewConst.OnFrameTime);
            }
        }

        protected void SetViewForward(TSVector forward, bool immediately = true)
        {
            if (immediately || m_cLerpRotateView == null)
            {
                transform.forward = forward.ToUnityVector3();
            }
            else
            {
                m_cLerpRotateView.LerpTo(forward.ToUnityVector3(),ViewConst.OnFrameTime * 2);
            }
        }

        protected void UpdateView(FP deltaTime)
        {
            if (m_cCollider != null)
            {
                m_cCollider.Update(curPosition, curForward);
            }
        }

        protected virtual void DieView(DamageInfo damageInfo)
        {
            if (m_cView != null)
            {
                m_cView.SetActive(false);
            }
        }

        public void SetAnimFloat(string strName, float fValue)
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.SetFloat(strName, fValue);
            }
        }

        public void SetAnimBool(string strName, bool bState)
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.SetBool(strName, bState);
            }
        }

        public void SetAnimInteger(string strName, int nValue)
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.SetInteger(strName, nValue);
            }
        }

        public void SetAnimTrigger(string strName)
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.SetTrigger(strName);
            }
        }

        public void ResetAnimTrigger(string strName)
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.ResetTrigger(strName);
            }
        }

        public void ResetAnimParam()
        {
            if (m_cAnimation != null)
            {
                m_cAnimation.ResetParam();
            }
        }
    }
}

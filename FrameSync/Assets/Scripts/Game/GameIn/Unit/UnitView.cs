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
        private Resource m_cResource;
        protected void InitView()
        {
            ResourceSys.Instance.GetResource(m_sPrefab, OnResLoad);
        }

        private void OnResLoad(Resource res)
        {
            if(res.isSucc)
            {
                m_cResource = res;
                m_cResource.Retain();
                UnityEngine.Object prefab = m_cResource.GetAsset(res.path);
                GameObject go = (GameObject)GameObject.Instantiate(prefab);
                this.gameObject.AddChildToParent(go);
            }
            else
            {
                CLog.LogError("加载unit资源"+res.path+"失败");
            }
        }

        protected void ResetView()
        {
            ResourceSys.Instance.RemoveListener(m_sPrefab, OnResLoad);
            if(m_cResource != null)
            {
                m_cResource.Release();
                m_cResource = null;
            }
        }

        protected void DisposeView()
        {
            ResourceSys.Instance.RemoveListener(m_sPrefab, OnResLoad);
            if (m_cResource != null)
            {
                m_cResource.Release();
                m_cResource = null;
            }
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

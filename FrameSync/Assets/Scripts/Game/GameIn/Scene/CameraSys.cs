using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class CameraSys : Singleton<CameraSys>
    {
        private Camera m_cMainCamera;
        public Camera mainCamera { get { return m_cMainCamera; } }
        private Transform m_cCameraParentTrans;
        public Rect viewPort { get { return m_sViewPort; } }
        private Rect m_sViewPort;
        private float m_fSceneScreenRate;
        private string m_sPath;
        private MoveBound m_cBound;

        public void Init()
        {
            //进入场景时保证预加载相机资源
            //初始化相机
            m_sPath = "Prefab/Scene/Camera";
            var go = SceneGOPool.Instance.GetObject(m_sPath, null);
            go.Reset();
            m_cCameraParentTrans = go.transform;
            m_cMainCamera = m_cCameraParentTrans.GetComponentInChildren<Camera>();
            m_cBound = m_cCameraParentTrans.GetComponentInChildren<MoveBound>();
            InitViewPort();
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.ResolutionUpdate, OnReolutionUpdate);
        }

        public Vector2 GetSceneDeltaByScreenDelta(Vector2 screenDelta)
        {
            return screenDelta * m_fSceneScreenRate;
        }

        private void OnReolutionUpdate(object args)
        {
            InitViewPort();
        }

        private void InitViewPort()
        {
            float radio = (float)Screen.width / Screen.height;
            float height;
            float width;
            if (m_cMainCamera.orthographic)
            {
                height = m_cMainCamera.orthographicSize * 2;
                width = height * radio;
            }
            else
            {
                var pos = m_cMainCamera.transform.position;
                pos = m_cCameraParentTrans.InverseTransformPoint(pos);
                float len = pos.y;
                height = len * Mathf.Tan(m_cMainCamera.fieldOfView / 2f * Mathf.Deg2Rad) * 2;
                width = height * radio;
            }
            m_sViewPort = new Rect(m_cCameraParentTrans.position.x - width / 2, m_cCameraParentTrans.position.z - height / 2, width, height);
            m_fSceneScreenRate = m_sViewPort.width / Screen.width;
            if(m_cBound != null)
            {
                m_cBound.SetRect(m_sViewPort);
            }
        }

        public void Clear()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.ResolutionUpdate, OnReolutionUpdate);
            if (m_cCameraParentTrans != null)
            {
                SceneGOPool.Instance.SaveObject(m_sPath, m_cCameraParentTrans.gameObject);
                m_cCameraParentTrans = null;
                m_cMainCamera = null;
            }
        }
    }
}

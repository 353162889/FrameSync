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

        private float m_fSceneScreenRate;
        private string m_sPath;

        public void Init(Rect viewRect)
        {
            //进入场景时保证预加载相机资源
            //初始化相机
            m_sPath = "Prefab/Scene/Camera";
            var go = (GameObject)SceneGOPool.Instance.GetObject(m_sPath,false, null);
            go.Reset();
            m_cCameraParentTrans = go.transform;
            m_cCameraParentTrans.position = Vector3.zero;
            m_cCameraParentTrans.eulerAngles = Vector3.zero;
            m_cMainCamera = m_cCameraParentTrans.GetComponentInChildren<Camera>();
            InitViewPort(viewRect);
            UpdateViewPort();
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.ResolutionUpdate, OnReolutionUpdate);
        }

        public Vector2 GetSceneDeltaByScreenDelta(Vector2 screenDelta)
        {
           // CLog.LogArgs(screenDelta, m_fSceneScreenRate, screenDelta * m_fSceneScreenRate);
            return screenDelta * m_fSceneScreenRate;
        }

        private void OnReolutionUpdate(object args)
        {
            UpdateViewPort();
        }

        private void InitViewPort(Rect rect)
        {
            if (m_cMainCamera.orthographic)
            {
                m_cMainCamera.orthographicSize = rect.height / 2;
            }
            else
            {
                float len = (m_cMainCamera.transform.position - Vector3.zero).magnitude;
                float angle = Mathf.Rad2Deg * Mathf.Atan2(rect.height / 2, len);
                m_cMainCamera.fieldOfView = angle * 2;
            }
        }

        private void UpdateViewPort()
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
            m_fSceneScreenRate = width / Screen.width;
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

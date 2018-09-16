using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{

    public class GizmosUtility : MonoBehaviour
    {
        public class GizmosAction : IPoolable, IDynamicObj
        {
            public int id;
            public float time;
            public Action action;

            public int key
            {
                get
                {
                    return id;
                }
            }

            public void Reset()
            {
                id = 0;
                time = 0;
                action = null;
            }
        }

        public static GizmosUtility instance
        {
            get { return m_cInstance; }
        }
        private static GizmosUtility m_cInstance;

        private ObjectPool<GizmosAction> m_cPool;

        private Dictionary<int, GizmosAction> m_Dic;
        private List<int> m_lstRemoveIds;
        private List<GizmosAction> m_lstAddAction;

        private static int ID = 0;
        void Awake()
        {
            m_cInstance = this;
            m_cPool = ObjectPool<GizmosAction>.Instance;
            m_cPool.Init(10);
            m_Dic = new Dictionary<int, GizmosAction>();
            m_lstRemoveIds = new List<int>();
            m_lstAddAction = new List<GizmosAction>();
        }

        public void DrawCircle(Vector3 center, float radius, float theta = 0.3f)
        {
            if (radius <= 0 || theta <= 0) return;
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            for (float i = 0; i < 2 * Mathf.PI; i += theta)
            {
                float x = radius * Mathf.Cos(i);
                float z = radius * Mathf.Sin(i);
                Vector3 endPoint = new Vector3(center.x + x, center.y, center.z + z);
                if (i == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }
                beginPoint = endPoint;
            }
            // 绘制最后一条线段  
            Gizmos.DrawLine(firstPoint, beginPoint);
        }

        public void DrawSector(Vector3 center, Vector3 forward, float radius, float angle, float theta = 0.3f)
        {
            if (radius <= 0 || angle <= 0 || theta <= 0) return;
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            angle = angle / 2f;
            Vector2 forward2D = new Vector2(forward.x,forward.z);
            float a = Vector2.Angle(Vector2.right, forward2D);
            float start = (a - angle) * Mathf.Deg2Rad;
            float end = (a + angle) * Mathf.Deg2Rad;
            for (float i = start; i < end; i += theta)
            {
                float x = radius * Mathf.Cos(i);
                float z = radius * Mathf.Sin(i);
                Vector3 endPoint = new Vector3(center.x + x, center.y, center.z + z);
                if (i == start)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }
                beginPoint = endPoint;
            }
            Vector3 lastPoint = new Vector3(center.x + radius * Mathf.Cos(end), center.y, center.z + radius * Mathf.Sin(end));
            Gizmos.DrawLine(beginPoint, lastPoint);
            // 绘制最后一条线段  
            Gizmos.DrawLine(firstPoint, center);
            Gizmos.DrawLine(lastPoint, center);
        }

        public void DrawRect(Vector3 center, Vector3 forward, float nHalfWidth, float nHalfHeight)
        {
            Vector3 forward3D = forward;
            Vector3 dir = Vector3.Cross(forward3D, Vector3.up);
            dir = dir.normalized * nHalfWidth;
            forward3D = forward3D.normalized * nHalfHeight;
            Vector3 pos1 = center + dir + forward3D;
            Vector3 pos2 = center - dir + forward3D;
            Vector3 pos3 = center - dir - forward3D;
            Vector3 pos4 = center + dir - forward3D;
            Gizmos.DrawLine(pos1, pos2);
            Gizmos.DrawLine(pos2, pos3);
            Gizmos.DrawLine(pos3, pos4);
            Gizmos.DrawLine(pos4, pos1);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">秒,渲染多长的时间，如果小于等于0，只渲染一帧</param>
        /// <param name="action">一般使用闭包，并且引用到的都是局部变量</param>
        public int BeginDrawGizmos(float time, Action action, bool isInUpdate = true)
        {
#if UNITY_EDITOR
            if (action == null) return -1;
            if (isInUpdate)
            {
                if (UnityEditor.SceneView.focusedWindow != null && UnityEditor.SceneView.focusedWindow.titleContent.text != "Scene")
                {
                    return -1;
                }
                if (m_Dic.Count > 100) return -1;
            }
            var cameras = UnityEditor.SceneView.GetAllSceneCameras();
            if (cameras != null && cameras.Length < 1) return -1;
            var entity = m_cPool.GetObject();
            int id = ++ID;
            entity.id = id;
            entity.time = time;
            entity.action = action;
            m_lstAddAction.Add(entity);
            return id;
#else
        return -1;
#endif
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            foreach (var item in m_Dic)
            {
                GizmosAction ga = item.Value as GizmosAction;
                try
                {
                    ga.action();
                    ga.time -= Time.deltaTime;
                    if (ga.time <= 0)
                    {
                        m_lstRemoveIds.Add(ga.id);
                        m_cPool.SaveObject(ga);
                    }
                }
                catch (Exception e)
                {
                    CLog.LogError(e.Message + "\n" + e.StackTrace);
                    m_lstRemoveIds.Add(ga.id);
                    m_cPool.SaveObject(ga);
                }
            }
            for (int i = 0; i < m_lstRemoveIds.Count; i++)
            {
                int id = m_lstRemoveIds[i];
                m_Dic.Remove(id);
            }
            m_lstRemoveIds.Clear();
            for (int i = 0; i < m_lstAddAction.Count; i++)
            {
                var ga = m_lstAddAction[i];
                m_Dic.Add(ga.id, ga);
            }
            m_lstAddAction.Clear();
#endif
        }

        void Update()
        {
#if UNITY_EDITOR
            var cameras = UnityEditor.SceneView.GetAllSceneCameras();
            if (cameras != null && cameras.Length < 1)
            {
                m_Dic.Clear();
            }
#endif
        }
    }
}
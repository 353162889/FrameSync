using BTCore;
using Framework;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class Remote : MonoBehaviour, IPoolable,IDynamicObj
    {

        public static NETreeComposeType RemoteComposeType = new NETreeComposeType(typeof(RemoteTree), new List<Type> { typeof(RemoteNodeAttribute), typeof(NENodeAttribute) }, "", "bytes", "远程");
        private static bool m_bInit = false;
        private static List<Type> m_lstRemoteNodeType;
        public static List<Type> lstRemoteNodeType { get { return m_lstRemoteNodeType; } }
        private static List<Type> m_lstRemoteNodeDataType;
        public static List<Type> lstRemoteNodeDataType { get { return m_lstRemoteNodeDataType; } }
        private static Type[] m_arrRemoteNodeDataType;
        public static Type[] arrRemoteNodeDataType { get { return m_arrRemoteNodeDataType; } }

        public static void Init()
        {
            if (m_bInit) return;
            m_bInit = true;
            NEUtil.LoadTreeComposeTypes(RemoteComposeType, out m_lstRemoteNodeType, out m_lstRemoteNodeDataType);
            m_arrRemoteNodeDataType = m_lstRemoteNodeDataType.ToArray();
        }

        public int key { get { return (int)m_nId; } }
        public uint id { get { return m_nId; } }
        protected uint m_nId;
        public int configId { get { return m_nConfigId; } }
        protected int m_nConfigId;
        public RemoteData remoteData { get { return m_cRemoteData; } }
        protected RemoteData m_cRemoteData;
        protected RemoteTree m_cRemoteTree;
        public TSVector curPosition { get { return m_sCurPosition; } }
        protected TSVector m_sCurPosition;
        public TSVector curForward { get { return m_sCurForward; } }
        protected TSVector m_sCurForward;

        public TSVector lastPosition { get { return m_sLastPosition; } }
        protected TSVector m_sLastPosition;
        public TSVector lastForward { get { return m_sLastForward; } }
        protected TSVector m_sLastForward;

        protected RemoteBlackBoard m_cBlackBoard;

        public AgentObject target { get { return m_cTarget; } }
        private AgentObject m_cTarget;
        public TSVector targetPosition { get { return m_sTargetPosition; } }
        private TSVector m_sTargetPosition;
        public TSVector targetForward { get { return m_sTargetForward; } }
        private TSVector m_sTargetForward;

        private LerpMoveView m_cLerpView;

        private GameObject m_cView;

        public void Init(uint id, int configId, TSVector position, TSVector forward, uint targetAgentId, AgentObjectType targetAgentType, TSVector targetPosition, TSVector targetForward)
        {
            m_nId = id;
            m_nConfigId = configId;
            this.gameObject.name = "remote_" + m_nId + "_" + m_nConfigId;
            m_cRemoteTree = RemoteTreePool.Instance.GetRemoteTree(m_nConfigId);
            m_cRemoteData = m_cRemoteTree.data as RemoteData;
            SetPosition(position);
            m_sLastPosition = position;
            SetForward(forward);
            m_sLastForward = forward;
            if(m_cBlackBoard == null)
            {
                m_cBlackBoard = new RemoteBlackBoard(this);
            }

            m_cTarget = AgentObject.GetAgentObject(targetAgentId, targetAgentType);
            m_sTargetPosition = targetPosition;
            m_sTargetForward = targetForward;
            GameObjectPool.Instance.GetObject(m_cRemoteData.remotePath, OnResLoad);
            m_cLerpView = gameObject.AddComponentOnce<LerpMoveView>();
            m_cLerpView.Init();
            m_cLerpView.StopMove();
        }

        public void StartMove(TSVector startPosition, List<TSVector> lstPosition)
        {
            TSVector forward = m_sCurForward;
            if (lstPosition.Count > 0) forward = lstPosition[0] - startPosition;
            if (!forward.IsZero()) forward.Normalize();
            SetForward(forward);
            SetPosition(startPosition);
            List<Vector3> lst = GameInTool.TSVectorToLstUnityVector3(lstPosition);
            m_cLerpView.StartMove(transform.position, lst);
        }

        public void Move(TSVector position)
        {
            SetPosition(position);
            SetForward(m_sCurPosition - m_sLastPosition);
        }

        public void WillMove(TSVector position, int logicPointCount)
        {
            m_cLerpView.Move(position.ToUnityVector3(), logicPointCount);
        }

        public void StopMove()
        {
            m_cLerpView.StopMove();
        }

        private void OnResLoad(GameObject go)
        {
            m_cView = go;
            this.gameObject.AddChildToParent(m_cView);
        }

        public void SetPosition(TSVector position)
        {
            m_sLastPosition = m_sCurPosition;
            m_sCurPosition = position;
            SetViewPosition(position);
        }

        public void SetForward(TSVector forward)
        {
            if (forward == TSVector.zero) return;
            forward.Normalize();
            m_sLastForward = m_sCurForward;
            m_sCurForward = forward;
            SetViewForward(forward);
        }

        public void OnUpdate(FP deltaTime)
        {
            m_cBlackBoard.deltaTime = deltaTime;
            BTResult result = m_cRemoteTree.OnTick(m_cBlackBoard);
            if (result != BTResult.Running)
            {
                StopMove();
                End();
            }
        }

        protected void End()
        {
            m_cRemoteTree.Clear();
            StopMove();
            //回收远程
            BattleScene.Instance.DestroyRemote(this);
        }

        public void Reset()
        {
            if(m_cView != null)
            {
                GameObjectPool.Instance.SaveObject(m_cRemoteData.remotePath, m_cView);
                m_cView = null;
            }
            m_cLerpView.StopMove();
            GameObjectPool.Instance.RemoveCallback(m_cRemoteData.remotePath, OnResLoad);
            m_cBlackBoard.Clear();
            m_cTarget = null;
            RemoteTreePool.Instance.SaveRemoteTree(m_nConfigId, m_cRemoteTree);
            m_cRemoteData = null;
            m_cRemoteTree = null;
        }


        protected void SetViewPosition(TSVector position)
        {
            transform.position = position.ToUnityVector3();
        }

        protected void SetViewForward(TSVector forward)
        {
            transform.forward = forward.ToUnityVector3();
        }
    }
}

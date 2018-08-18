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

        public static NETreeComposeType RemoteComposeType = new NETreeComposeType(typeof(RemoteTree), new List<Type> { typeof(RemoteNodeAttribute), typeof(BTNodeAttribute),typeof(BTGameNodeAttribute) }, "Assets/ResourceEx/Config/Remote", "remote", "bytes", "远程");
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
        public int campId { get { return m_nCampId; } }
        protected int m_nCampId;
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
        public uint targetAgentId { get { return m_cTarget == null ? 0 : m_cTarget.id; } }
        public AgentObjectType targetAgentType { get { return m_cTarget == null ? AgentObjectType.Unit : m_cTarget.agentType; } }

        public TSVector targetPosition { get { return m_sTargetPosition; } }
        private TSVector m_sTargetPosition;
        public TSVector targetForward { get { return m_sTargetForward; } }
        private TSVector m_sTargetForward;

        private LerpMoveView m_cLerpView;

        private GameObject m_cView;

        private HangPoint m_cHangPoint;
        public GameCollider gameCollider { get { return m_cCollider; } }
        private GameCollider m_cCollider;

        private AgentObject m_cAgentObj;
        public AgentObject agentObj { get { return m_cAgentObj; } }

        private ValueContainer m_cValueContainer;

        public void Init(uint id, int configId,int campId, TSVector position, TSVector forward, uint targetAgentId, AgentObjectType targetAgentType, TSVector targetPosition, TSVector targetForward)
        {
            m_nId = id;
            m_nConfigId = configId;
            m_nCampId = campId;
            this.gameObject.name = "remote_" + m_nId + "_" + m_nConfigId;
            m_cRemoteTree = RemoteTreePool.Instance.GetRemoteTree(m_nConfigId);
            m_cRemoteData = m_cRemoteTree.data as RemoteData;
            m_cAgentObj = new AgentRemote(this);
            SetPosition(position);
            SetViewPosition(position);
            m_sLastPosition = position;
            SetForward(forward);
            SetViewForward(forward);
            m_sLastForward = forward;
            if(m_cBlackBoard == null)
            {
                m_cBlackBoard = new RemoteBlackBoard();
            }
            m_cBlackBoard.Init(this);

            m_cTarget = AgentObject.GetAgentObject(targetAgentId, targetAgentType);
            m_sTargetPosition = targetPosition;
            m_sTargetForward = targetForward;
            m_cView = SceneEffectPool.Instance.CreateEffect(m_cRemoteData.remotePath, false, this.transform);
            m_cLerpView = gameObject.AddComponentOnce<LerpMoveView>();
            m_cLerpView.Init();
            m_cLerpView.StopMove();

            m_cHangPoint = gameObject.AddComponentOnce<HangPoint>();
            string remoteFullPath = PathTool.GetSceneEffectPath(m_cRemoteData.remotePath);
            m_cHangPoint.Init(remoteFullPath);
            //暂时不支持表现挂点(特效上挂特效)
            m_cHangPoint.InitHangView(null);

            m_cCollider = ObjectPool<GameCollider>.Instance.GetObject();
            m_cCollider.Init(remoteFullPath);
            m_cCollider.Update(curPosition, curForward);

            if (m_cValueContainer == null)
            {
                m_cValueContainer = new ValueContainer();
                m_cValueContainer.Add((int)AttrType.Attack);
            }
            m_cValueContainer.Reset();
        }

        public FP GetAttrValue(int key)
        {
            return m_cValueContainer.GetValue(key);
        }
        public void SetAttrValue(int key, FP value)
        {
            m_cValueContainer.SetValue(key, value);
        }

        public Transform GetHangPoint(string name, out TSVector position, out TSVector forward)
        {
            return m_cHangPoint.GetHangPoint(name, curPosition, curForward, out position, out forward);
        }

        public Transform GetHangPoint(string name, TSVector cPosition, TSVector cForward, out TSVector position, out TSVector forward)
        {
            return m_cHangPoint.GetHangPoint(name, cPosition, cForward, out position, out forward);
        }

        public void StartMove(TSVector startPosition, List<TSVector> lstPosition,bool stopToMove)
        {
            TSVector forward = m_sCurForward;
            if (lstPosition.Count > 0) forward = lstPosition[0] - startPosition;
            if (!forward.IsZero()) forward.Normalize();
            SetForward(forward);
            SetPosition(startPosition);
            List<Vector3> lst = GameInTool.TSVectorToLstUnityVector3(lstPosition);
            m_cLerpView.StartMove(transform.position, lst,stopToMove);
        }

        public void Move(TSVector position,int moveTimes)
        {
            SetPosition(position);
            SetForward(m_sCurPosition - m_sLastPosition);
            m_cLerpView.Move(position.ToUnityVector3(), moveTimes);
        }

        public void WillMove(TSVector willPosition, TSVector willforward, PM_CenterPoints willCenterPoints)
        {
            m_cLerpView.WillMove(willPosition.ToUnityVector3(), LPM_CenterPoints.FromPM_CenterPoints(willCenterPoints));
        }

        public void StopMove()
        {
            m_cLerpView.StopMove();
        }

        public void SetPosition(TSVector position)
        {
            m_sLastPosition = m_sCurPosition;
            m_sCurPosition = position;
        }

        public void SetForward(TSVector forward)
        {
            if (forward == TSVector.zero) return;
            forward.Normalize();
            m_sLastForward = m_sCurForward;
            m_sCurForward = forward;
           
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
            if(m_cCollider != null)
            {
                m_cCollider.Update(curPosition, curForward);
            }
        }

        protected void End()
        {
            m_cRemoteTree.Clear();
            m_cBlackBoard.Clear();
            StopMove();
            //回收远程
            BattleScene.Instance.DestroyRemote(this);
        }

        public void Reset()
        {
            if(m_cView != null)
            {
                SceneEffectPool.Instance.DestroyEffectGO(m_cView);
                m_cView = null;
            }
            if (m_cCollider != null)
            {
                ObjectPool<GameCollider>.Instance.SaveObject(m_cCollider);
                m_cCollider = null;
            }
            if(m_cValueContainer != null)
            {
                m_cValueContainer.Reset();
            }
            m_cLerpView.StopMove();
            m_cBlackBoard.Clear();
            m_cHangPoint.Clear();
            m_cTarget = null;
            RemoteTreePool.Instance.SaveRemoteTree(m_nConfigId, m_cRemoteTree);
            m_cRemoteData = null;
            m_cRemoteTree = null;
            ClearAgent();
        }


        protected void SetViewPosition(TSVector position)
        {
            transform.position = position.ToUnityVector3();
        }

        protected void SetViewForward(TSVector forward)
        {
            transform.forward = forward.ToUnityVector3();
        }

        private void ClearAgent()
        {
            if (m_cAgentObj != null)
            {
                m_cAgentObj.Clear();
                m_cAgentObj = null;
            }
        }
    }
}

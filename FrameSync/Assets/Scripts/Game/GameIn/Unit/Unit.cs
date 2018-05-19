using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using Proto;

namespace Game
{
    public partial class Unit : MonoBehaviour,IDynamicObj,IPoolable
    {
        public int key{ get { return (int)m_nId; } }

        private uint m_nId;
        public uint id { get { return m_nId; } }

        protected int m_nConfigId;
        public int configId { get { return m_nConfigId; } }

        protected UnitType m_eUnitType;
        public UnitType unitType { get { return m_eUnitType; } }
        

        protected TSVector m_sCurPosition;
        public TSVector curPosition { get { return m_sCurPosition; } }

        protected TSVector m_sCurForward;
        public TSVector curForward { get { return m_sCurForward; } }

        protected string m_sPrefab;

        protected AgentObject m_cAgentObj;
        public AgentObject agentObj { get { return m_cAgentObj; } }

        public void Init(uint id,int configId,UnitType type, TSVector position, TSVector forward)
        {
            m_nId = id;
            m_nConfigId = configId;
            m_eUnitType = type;
            m_cAgentObj = new AgentUnit(this);
            SetPosition(position);
            SetForward(forward);
            SubInit();
        }

        public void ReqSetPosition(TSVector position)
        {
            if (position == m_sCurPosition) return;
            Frame_ReqSetPosition_Data data = new Frame_ReqSetPosition_Data();
            data.unitId = id;
            data.position = GameInTool.ToProtoVector2(position);
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqSetPosition, data);
        }

        public void SetPosition(TSVector position)
        {
            m_sCurPosition = position;
            SetViewPosition(position);
        }

        public void ReqSetForward(TSVector forward)
        {
            if (TSMath.Abs(TSVector.Angle(m_sCurForward, forward)) < FP.EN1) return;
            Frame_ReqSetForward_Data data = new Frame_ReqSetForward_Data();
            data.unitId = id;
            data.forward = GameInTool.ToProtoVector2(forward);
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqSetForward, data);

        }

        public void SetForward(TSVector forward)
        {
            if (forward == TSVector.zero) return;
            m_sCurForward = forward;
            SetViewForward(forward);
        }

        protected virtual void SubInit()
        {
            InitMove();
            InitView();
            InitSkill();
        }

        public virtual void OnUpdate(FP deltaTime)
        {
            UpdateMove(deltaTime);
            UpdateView(deltaTime);
            UpdateSkill(deltaTime);
        }

        public void Dispose()
        {
            DisposeMove();
            DisposeView();
            ClearAgent();
            DisposeSkill();
        }

        public void Reset()
        {
            ResetMove();
            ResetView();
            ClearAgent();
            ResetSkill();
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

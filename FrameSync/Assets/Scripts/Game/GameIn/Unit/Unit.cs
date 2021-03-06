﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using Proto;

namespace Game
{
    public enum ForwardFromType
    {
        Init,
        Player,
        UnitMove,
        Skill,
        AI,
    }

    public enum PositionFromType
    {
        Init,
        Player,
        Skill,
    }

    public partial class Unit : MonoBehaviour,IDynamicObj,IPoolable
    {
        public delegate void UnitDamageHandler(Unit unit, DamageInfo damageInfo);
        public event UnitDamageHandler OnUnitHurt;
        public event UnitDamageHandler OnUnitRecovery;
        public event UnitDamageHandler OnUnitDie;
        public int key{ get { return (int)m_nId; } }

        private uint m_nId;
        public uint id { get { return m_nId; } }

        protected int m_nConfigId;
        public int configId { get { return m_nConfigId; } }

        protected int m_nCampId;
        public int campId { get { return m_nCampId; } }

        protected UnitType m_eUnitType;
        public UnitType unitType { get { return m_eUnitType; } }
        

        protected TSVector m_sCurPosition;
        public TSVector curPosition { get { return m_sCurPosition; } protected set { m_sLastPosition = m_sCurPosition; m_sCurPosition = value; } }

        protected TSVector m_sCurForward;
        public TSVector curForward { get { return m_sCurForward; } protected set { m_sLastForward = m_sCurForward;m_sCurForward = value; } }

        public TSVector lastPosition { get { return m_sLastPosition; } }
        protected TSVector m_sLastPosition;
        public TSVector lastForward { get { return m_sLastForward; } }
        protected TSVector m_sLastForward;

        protected string m_sPrefab;

        protected AgentObject m_cAgentObj;
        public AgentObject agentObj { get { return m_cAgentObj; } }

        protected bool m_bIsDie;
        public bool isDie { get { return m_bIsDie; } }
        protected bool m_bIsUpdate;
        protected DamageInfo m_cDelayDamageInfo;
        protected FP m_sDieTime;
        public FP dieTime { get { return m_sDieTime; } }
        protected FP m_sStartDieTime;
        public FP startDieTime { get { return m_sStartDieTime; } }

        public void Init(uint id,int configId,int campId, UnitType type, TSVector position, TSVector forward)
        {
            m_nId = id;
            m_nConfigId = configId;
            m_nCampId = campId;
            m_eUnitType = type;
            m_bIsDie = false;
            m_cAgentObj = new AgentUnit(this);
            m_sCurPosition = m_sLastPosition = position;
            m_sCurForward = m_sLastForward = forward;
            SetPosition(position,PositionFromType.Init,true);
            SetForward(forward,ForwardFromType.Init);
            SubInit();
        }

        public void ReqSetPosition(TSVector position, bool immediately)
        {
            if (position == m_sCurPosition) return;
            Frame_ReqSetPosition_Data data = new Frame_ReqSetPosition_Data();
            data.unitId = id;
            data.position = GameInTool.ToProtoVector2(position);
            data.immediately = immediately;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqSetPosition, data);
        }

        public void SetPosition(TSVector position,PositionFromType fromType, bool immediately)
        {
            curPosition = position;
            SetViewPosition(position,immediately);
        }

        public void ReqSetForward(TSVector forward,bool immediately = true)
        {
            if (!CanSetForward(ForwardFromType.Player)) return;
            if (TSMath.Abs(TSVector.Angle(m_sCurForward, forward)) < FP.EN1) return;
            Frame_ReqSetForward_Data data = new Frame_ReqSetForward_Data();
            data.unitId = id;
            data.forward = GameInTool.ToProtoVector2(forward);
            data.immediately = immediately;
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.Frame_ReqSetForward, data);

        }

        public void SetForward(TSVector forward, ForwardFromType fromType, bool immediately = true,bool viewImmediately = true)
        {
            if (forward == TSVector.zero) return;
            if (!CanSetForward(fromType)) return;
            if (immediately)
            {
                curForward = forward;
                SetViewForward(forward, viewImmediately);
                if(fromType != ForwardFromType.UnitMove)
                {
                    StopRotate();
                }
            }
            else
            {
                RotateToTarget(forward);
            }
        }

        public bool CanSetForward(ForwardFromType fromType)
        {
            //初始化设置方向，不判断是否禁止
            if (fromType == ForwardFromType.Init) return true;
            if (fromType == ForwardFromType.Player && IsForbid(UnitForbidType.ForbidPlayerForward)) return false;
            return !IsForbid(UnitForbidType.ForbidForward);
        }

        public void OnHurt(DamageInfo damageInfo)
        {
            this.hp -= damageInfo.damage;
            if (damageInfo.damage >= 0)
            {
                if (null != OnUnitHurt)
                {
                    OnUnitHurt(this, damageInfo);
                }
                GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitHurt, damageInfo);
                if (this.hp <= 0)
                {
                    this.Die(damageInfo);
                }
            }
            else
            {
                if (null != OnUnitRecovery)
                {
                    OnUnitRecovery(this, damageInfo);
                }
                GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitRecovery, damageInfo);
            }
        }

        protected virtual void Die(DamageInfo damageInfo)
        {
            m_bIsDie = true;
            m_sStartDieTime = FrameSyncSys.time;
            if (null != OnUnitDie)
            {
                OnUnitDie(this, damageInfo);
            }
            GlobalEventDispatcher.Instance.Dispatch(GameEvent.UnitDie, damageInfo);
            if (m_bIsUpdate)
            {
                m_cDelayDamageInfo = ObjectPool<DamageInfo>.Instance.GetObject();
                m_cDelayDamageInfo.CloneFrom(damageInfo);
            }
            else
            {
                ClearDamageInfo(damageInfo);
            }
        }

        protected virtual void ClearDamageInfo(DamageInfo damageInfo)
        {
            DieAI(damageInfo);
            DieAttr(damageInfo);
            DieMove(damageInfo);
            DieSkill(damageInfo);
            DieView(damageInfo);
            DieForbid(damageInfo);
        }

        protected virtual void SubInit()
        {
            m_cDelayDamageInfo = null;
            InitForbid();
            InitAttr();
            InitMove();
            InitView();
            InitSkill();
            InitAI();
        }

        public virtual void OnUpdate(FP deltaTime)
        {
            if (m_bIsDie) return;
            m_bIsUpdate = true;
            UpdateAI(deltaTime);
            UpdateAttr(deltaTime);
            UpdateMove(deltaTime);
            UpdateView(deltaTime);
            UpdateSkill(deltaTime);
            UpdateForbid(deltaTime);
            m_bIsUpdate = false;
            if(m_cDelayDamageInfo != null)
            {
                ClearDamageInfo(m_cDelayDamageInfo);
                ObjectPool<DamageInfo>.Instance.SaveObject(m_cDelayDamageInfo);
                m_cDelayDamageInfo = null;
            }
        }

        public void Reset()
        {
            m_bIsUpdate = false;
            if (m_cDelayDamageInfo != null)
            {
                ObjectPool<DamageInfo>.Instance.SaveObject(m_cDelayDamageInfo);
                m_cDelayDamageInfo = null;
            }
            ResetAI();
            ResetAttr();
            ResetMove();
            ResetView();
            ClearAgent();
            ResetSkill();
            ResetForbid();
            OnUnitHurt = null;
            OnUnitDie = null;
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

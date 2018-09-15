using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class PvpPlayer
    {

        private static int MaxPlayerSkillCount = 2;
        private long m_lId;
        public long id { get { return m_lId; } }

        private Unit m_cUnit;
        public Unit unit { get { return m_cUnit; } }

        private PvpPlayerData m_cPlayerData;

        private TSVector m_sBornPos;
        private bool m_bAIEnable;
        public bool initUnit { get { return m_bInitUnit; } }
        private bool m_bInitUnit;
        private FP m_sColliderTime;

        //技能栏
        private int[] m_arrSkillId = new int[MaxPlayerSkillCount];
        private FP m_sLastHp = 0;

        public void Init(long id,PvpPlayerData playerData = null)
        {
            m_lId = id;
            m_cPlayerData = playerData;
            m_bInitUnit = false;
            m_bAIEnable = false;
            m_sColliderTime = 3;
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.UnitRemove, OnUnitRemove);
        }

        private void OnUnitRemove(object args)
        {
            Unit unit = (Unit)args;
            if(unit.configId == m_cPlayerData.configId)
            {
                //清除子弹技能列表
                for (int i = 0; i < m_arrSkillId.Length; i++)
                {
                    m_arrSkillId[i] = 0;
                }
                m_sLastHp = 0;
            }
            if(unit == m_cUnit)
            {
                m_cUnit = null;
                GlobalEventDispatcher.Instance.Dispatch(GameEvent.PvpPlayerUnitDie, this);
                CreatePlayerUnit(m_sBornPos);
                m_cUnit.SetColliderEnable(false);
                m_sColliderTime = 3;
                m_cUnit.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
                if (m_bAIEnable)
                {
                    m_cUnit.StartAI();
                }
            }
        }

        public void SetBornPos(TSVector pos)
        {
            m_sBornPos = pos;
        }

        public void SetAIEnable(bool enable)
        {
            m_bAIEnable = enable;
            if(m_cUnit != null && !m_cUnit.isDie)
            {
                if (m_bAIEnable)
                {
                    m_cUnit.StartAI();
                }
                else
                {
                    m_cUnit.StopAI();
                }
            }
        }

        //创建玩家的主unit
        public Unit CreatePlayerUnit(TSVector pos)
        {
            Unit unit = CreateUnit(m_cPlayerData.configId,pos);
            UpdateHp();
            UpdateSkills();
            return unit;
        }

        public void HitSkill(int index,int skillId)
        {
            if (index < 0 || index >= m_arrSkillId.Length) return;
            //记录当前主玩家的技能
            if (m_arrSkillId[index] == skillId) return;
            m_arrSkillId[index] = skillId;
            if(m_cUnit != null && m_cUnit.configId == m_cPlayerData.configId)
            {
                UpdateSkills();
            }
        }

        public void HitUnit(int configId)
        {
            TSVector curPosition = m_sBornPos;
            if(m_cUnit != null)
            {
                if (configId == m_cUnit.configId) return;
                curPosition = m_cUnit.curPosition;
                var tempUnit = m_cUnit;
                //记录主玩家的血量
                if (tempUnit.configId == m_cPlayerData.configId)
                {
                    m_sLastHp = tempUnit.hp;
                }
                m_cUnit = null;
                BattleScene.Instance.DestroyUnit(tempUnit);
            }
            var unit = CreateUnit(configId, curPosition);
            //变更ai
            var resInfo = ResCfgSys.Instance.GetCfg<ResAirShip>(m_cPlayerData.configId);
            unit.SetAI(resInfo.ai_path);
            unit.Forbid(UnitForbidType.ForbidForward, UnitForbidFromType.Game);
            if (m_bAIEnable)
            {
                unit.StartAI();
            }
        }

        //创建玩家的unit
        private Unit CreateUnit(int configId,TSVector pos)
        {
            m_bInitUnit = true;
            m_cUnit = BattleScene.Instance.CreateUnit(configId, m_cPlayerData.campId, UnitType.AirShip, pos, TSVector.forward);
            GlobalEventDispatcher.Instance.DispatchByParam(GameEvent.AddUnitDestory, UnitDestoryType.DieDestory, m_cUnit);
            return m_cUnit;
        }

        private void UpdateSkills()
        {
            //移除所有主动技能
            var lst = m_cUnit.lstActiveSkill;
            for (int i = lst.Count - 1; i > -1; i--)
            {
                m_cUnit.RemoveSkill(lst[i].skillId);
            }
            //如果技能栏中没有技能，设置默认技能，如果有技能，设置技能
            if (m_arrSkillId[0] <= 0)
            {
                var resSkills = ((UnitAirShip)m_cUnit).resInfo.active_skills;
                if (resSkills.Count > 0)
                {
                    m_arrSkillId[0] = resSkills[0];
                }
            }
            for (int i = 0; i < m_arrSkillId.Length; i++)
            {
                if (m_arrSkillId[i] > 0)
                {
                    m_cUnit.AddSkill(m_arrSkillId[i]);
                }
            }
            lst = m_cUnit.lstActiveSkill;
            var lstSkillId = ResetObjectPool<List<int>>.Instance.GetObject();
            for (int i = 0; i < lst.Count; i++)
            {
                lstSkillId.Add(lst[i].skillId);
            }
            //设置ai的技能
            m_cUnit.SetAISkill(lstSkillId);
            ResetObjectPool<List<int>>.Instance.SaveObject(lstSkillId);
        }

        private void UpdateHp()
        {
            if(m_sLastHp > 0)
            {
                m_cUnit.hp = m_sLastHp;
            }
        }

        public void FrameUpdate(FP deltaTime)
        {
            if(m_sColliderTime > 0)
            {
                m_sColliderTime -= deltaTime;
                if(m_sColliderTime <= 0 && m_cUnit != null)
                {
                    m_cUnit.SetColliderEnable(true);
                }
            }
        }

        public void Clear()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.UnitRemove, OnUnitRemove);
            m_cUnit = null;
        }
    }
}

using Framework;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class UnitAirShip : Unit
    {
        private ResAirShip m_resInfo;
        public ResAirShip resInfo { get { return m_resInfo; } }
        protected override void SubInit()
        {
            m_resInfo = ResCfgSys.Instance.GetCfg<ResAirShip>(configId);
            if(m_resInfo == null)
            {
                CLog.LogError("找不到配置id="+configId + "的ResAirShip配置");
                return;
            }
            m_sPrefab = m_resInfo.prefab;
            base.SubInit();
            this.hpLmt = m_resInfo.hp;
            this.hp = this.hpLmt;
            this.attack = m_resInfo.attack;
            this.moveSpeed = m_resInfo.move_speed;
            for (int i = 0; i < m_resInfo.active_skills.Count; i++)
            {
                AddSkill(m_resInfo.active_skills[i]);
            }
            
            if(m_resInfo.active_skills.Count > 0)
            {
                var lst = ResetObjectPool<List<int>>.Instance.GetObject();
                lst.Add(m_resInfo.active_skills[0]);
                SetAISkill(lst);
                ResetObjectPool<List<int>>.Instance.SaveObject(lst);
            }
            
            for (int i = 0; i < m_resInfo.passive_skills.Count; i++)
            {
                AddSkill(m_resInfo.passive_skills[i]);
            }
            if (!string.IsNullOrEmpty(m_resInfo.ai_path))
            {
                this.SetAI(m_resInfo.ai_path);
            }
        }

        protected override void OnStartMove(TSVector position, TSVector forward,bool stopToMove)
        {
            base.OnStartMove(position, forward,stopToMove);
            SetAnimFloat("xSpeed", forward.x.AsFloat());
        }

        protected override void OnMove(TSVector position, TSVector forward)
        {
            base.OnMove(position, forward);
            SetAnimFloat("xSpeed", forward.x.AsFloat());
        }

        protected override void OnStopMove(TSVector position, TSVector forward)
        {
            base.OnStopMove(position, forward);
            SetAnimFloat("xSpeed", 0);
        }
    }
}

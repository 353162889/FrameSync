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
            this.hpLmt = 100;
            this.hp = this.hpLmt;
            this.attack = 10;
            this.moveSpeed = 30;
            for (int i = 0; i < m_resInfo.skills.Count; i++)
            {
                AddSkill(m_resInfo.skills[i]);
            }
           if(m_resInfo.ai > 0)
            {
                this.SetAI(m_resInfo.ai);
            }
        }

        protected override void OnStartMove(TSVector position, TSVector forward)
        {
            base.OnStartMove(position, forward);
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

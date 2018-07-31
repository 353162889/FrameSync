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
            this.attack = 1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using GameData;

namespace Game
{
    public class UnitItem : Unit
    {
        public ResItem resInfo { get { return m_resInfo; } }
        private ResItem m_resInfo;
        protected override void SubInit()
        {
            m_resInfo = ResCfgSys.Instance.GetCfg<ResItem>(configId);
            if (m_resInfo == null)
            {
                CLog.LogError("找不到配置id=" + configId + "的ResItem配置");
                return;
            }
            m_sPrefab = m_resInfo.prefab;
            InitAttr();
            InitMove();
            InitView();
            InitAI();
            this.moveSpeed = m_resInfo.move_speed;
        }

        public void HitUnit(Unit unit)
        {
            if(m_resInfo.hp != 0)
            {
                var damageInfo = ObjectPool<DamageInfo>.Instance.GetObject();
                damageInfo.attack = this.agentObj;
                damageInfo.defence = unit.agentObj;
                damageInfo.damage = -m_resInfo.hp;
                unit.OnHurt(damageInfo);
                ObjectPool<DamageInfo>.Instance.SaveObject(damageInfo);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public enum UnitAttrType
    {
        HP,//血量
        HPLmt,//血量上限
        Attack,//攻击力
    }
    public partial class Unit
    {
        public event ValueContainer.ValueContainerHandler OnValueChanged
        {
            add { m_cValueContainer.OnValueChanged += value; }
            remove { m_cValueContainer.OnValueChanged -= value; }
        }

        private ValueContainer m_cValueContainer;

        public int hp { get { return GetValue((int)UnitAttrType.HP); } set { SetValue((int)UnitAttrType.HP,value); } }
        public int hpLmt { get { return GetValue((int)UnitAttrType.HPLmt); } set { SetValue((int)UnitAttrType.HPLmt, value); } }
        public int attack { get { return GetValue((int)UnitAttrType.Attack); } set { SetValue((int)UnitAttrType.Attack, value); } }

        protected void InitAttr()
        {
            if (m_cValueContainer == null)
            {
                m_cValueContainer = new ValueContainer();
            }
            m_cValueContainer.Add((int)UnitAttrType.HP);
            m_cValueContainer.Add((int)UnitAttrType.HPLmt);
            m_cValueContainer.Add((int)UnitAttrType.Attack);
        }

        protected int GetValue(int key)
        {
            return m_cValueContainer.GetValue(key);
        }
        protected void SetValue(int key,int value)
        {
            switch(key)
            {
                case (int)UnitAttrType.HP:
                    if (value > hpLmt) value = hpLmt;
                    break;
            }
            m_cValueContainer.SetValue(key, value);
        }

        protected void UpdateAttr(FP deltaTime) { }

        protected void ResetAttr()
        {
            m_cValueContainer.Clear();
        }

        protected void DieAttr(DamageInfo damageInfo)
        {
            m_cValueContainer.Reset();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public partial class Unit
    {
        public event ValueContainer.ValueContainerHandler OnValueChanged
        {
            add { m_cValueContainer.OnValueChanged += value; }
            remove { m_cValueContainer.OnValueChanged -= value; }
        }

        private ValueContainer m_cValueContainer;

        public FP hp { get { return GetAttrValue((int)AttrType.HP); } set { SetAttrValue((int)AttrType.HP,value); } }
        public FP hpLmt { get { return GetAttrValue((int)AttrType.HPLmt); } set { SetAttrValue((int)AttrType.HPLmt, value); } }
        public FP attack { get { return GetAttrValue((int)AttrType.Attack); } set { SetAttrValue((int)AttrType.Attack, value); } }
        public FP moveSpeed { get { return GetAttrValue((int)AttrType.MoveSpeed); } set { SetAttrValue((int)AttrType.MoveSpeed, value); } }

        protected void InitAttr()
        {
            if (m_cValueContainer == null)
            {
                m_cValueContainer = new ValueContainer();
            }
            m_cValueContainer.Add((int)AttrType.HP);
            m_cValueContainer.Add((int)AttrType.HPLmt);
            m_cValueContainer.Add((int)AttrType.Attack);
            m_cValueContainer.Add((int)AttrType.MoveSpeed);
        }

        public FP GetAttrValue(int key)
        {
            return m_cValueContainer.GetValue(key);
        }
        public void SetAttrValue(int key, FP value)
        {
            switch(key)
            {
                case (int)AttrType.HP:
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

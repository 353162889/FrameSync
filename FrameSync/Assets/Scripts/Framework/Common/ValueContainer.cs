using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class ValueContainer
    {
        public delegate void ValueContainerHandler(int key, FP oldValue, FP newValue);
        public event ValueContainerHandler OnValueChanged;
        protected List<int> m_lstKey;
        protected List<FP> m_lstValue;

        public ValueContainer()
        {
            m_lstKey = new List<int>();
            m_lstValue = new List<FP>();
        }

        public List<int> GetAllKey()
        {
            return m_lstKey;
        }

        public void Add(int key)
        {
            int index = m_lstKey.IndexOf(key);
            if (index == -1)
            {
                m_lstKey.Add(key);
                m_lstValue.Add(0);
            }
            else
            {
                CLog.LogError("[" + this.GetType().ToString() + "]key=" + key + "已存在，不能重复添加");
            }
        }

        public virtual void SetValue(int key, FP value)
        {
            int index = m_lstKey.IndexOf(key);
            if (index > -1)
            {
                FP oldValue = m_lstValue[index];
                if (oldValue != value)
                {
                    m_lstValue[index] = value;
                    if (null != OnValueChanged)
                    {
                        OnValueChanged(key, oldValue, value);
                    }
                }
            }
            else
            {
                CLog.LogError("[" + this.GetType().ToString() + "]key=" + key + "不存在");
            }
        }

        public virtual FP GetValue(int key)
        {
            int index = m_lstKey.IndexOf(key);
            if (index > -1) return m_lstValue[index];
            CLog.LogError("[" + this.GetType().ToString() + "]key=" + key + "不存在");
            return 0;
        }

        public virtual void Reset()
        {
            for (int i = 0; i < m_lstValue.Count; i++)
            {
                m_lstValue[i] = 0;
            }
        }

        public virtual void Clear()
        {
            m_lstKey.Clear();
            m_lstValue.Clear();
            OnValueChanged = null;
        }
    }
}

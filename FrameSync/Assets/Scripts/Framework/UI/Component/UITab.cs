using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class UITab : MonoBehaviour
    {
        protected GameObject m_cSelectedGO;
        protected GameObject m_cUnSelectGO;
        public event Action<UITab> onTabChange;

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                bool oldSelected = isSelected;
                isSelected = value;
                m_cSelectedGO.SetActive(isSelected);
                m_cUnSelectGO.SetActive(!isSelected);
                OnSelectChange(oldSelected, isSelected);
            }
        }

        protected int index;

        public virtual void Init(int index)
        {
            this.index = index;
            m_cSelectedGO = GetSelectedGO();
            m_cUnSelectGO = GetUnSelectGO();
            m_cSelectedGO.SetActive(false);
            m_cUnSelectGO.SetActive(false);
            UIEventTrigger.Get(GetClickGO()).AddListener(EventTriggerType.PointerClick, OnTabClick);
        }

        public virtual void OnUpdate()
        {

        }

        protected virtual void OnSelectChange(bool old, bool cur)
        {

        }

        public virtual void OnDestroy()
        {
            UIEventTrigger.Get(GetClickGO()).RemoveListener(EventTriggerType.PointerClick, OnTabClick);
            onTabChange = null;
        }

        protected virtual void OnTabClick(BaseEventData data)
        {
            if (onTabChange != null)
            {
                onTabChange.Invoke(this);
            }
        }

        protected virtual GameObject GetClickGO()
        {
            return gameObject;
        }

        protected virtual GameObject GetSelectedGO()
        {
            return gameObject.FindChildRecursive("SelectedGO");
        }

        protected virtual GameObject GetUnSelectGO()
        {
            return gameObject.FindChildRecursive("UnSelectedGO");
        }
    }

}
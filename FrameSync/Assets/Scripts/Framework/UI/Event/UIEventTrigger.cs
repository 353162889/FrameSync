using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework
{
    public class UIEventTrigger : EventTrigger
    {

        public static UIEventTrigger Get(GameObject go)
        {
            UIEventTrigger trigger = go.GetComponent<UIEventTrigger>();
            if (null == trigger)
            {
                trigger = go.AddComponentOnce<UIEventTrigger>();
            }
            return trigger;
        }

        public static UIEventTrigger Get(Transform transform)
        {
            return Get(transform.gameObject);
        }

        /// <summary>
        /// 添加UGUI事件监听，同一组eventType+action只能监听一次
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public void AddListener(EventTriggerType eventType, UnityAction<BaseEventData> action)
        {
            triggers = triggers != null ? triggers : new List<Entry>();
            List<Entry> listEntry = triggers;

            EventTrigger.Entry entry = null;
            for (int i = listEntry.Count - 1; i >= 0; --i)
            {
                if (listEntry[i].eventID == eventType)
                {
                    entry = listEntry[i];
                    entry.callback.RemoveListener(action);//移除重复的监听
                    break;
                }
            }
            if (entry == null)
            {
                entry = new EventTrigger.Entry();
                entry.eventID = eventType;
                listEntry.Add(entry);
            }

            entry.callback.AddListener(action);
        }

        /// <summary>
        /// 移除UGUI事件监听。如果action为null，则移除所有eventType类型的监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public void RemoveListener(EventTriggerType eventType, UnityAction<BaseEventData> action = null)
        {
            List<Entry> listEntry = triggers;
            if (null == listEntry)
            {
                return;
            }

            for (int i = listEntry.Count - 1; i >= 0; --i)
            {
                if (listEntry[i].eventID == eventType)
                {
                    if (action == null)
                    {
                        listEntry.RemoveAt(i);
                    }
                    else
                    {
                        listEntry[i].callback.RemoveListener(action);
                        break;
                    }
                }
            }
        }
    }
}

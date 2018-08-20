using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    
    public enum EventPriority
    {
        LOW = 0,
        NORMAL = 1,
        HIGH = 2,
        COUNT = 3
    }
    public class EventDispatcher
    {
        public delegate void EventHandler(object args);
        public struct DispatcherCallback : IDynamicDispatcherObj
        {
            public bool once;
            public EventHandler callback;

            public bool OnDispatcher(object args)
            {
                if (callback != null)
                {
                    callback(args);
                    return !once;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                DispatcherCallback other = (DispatcherCallback)obj;
                return callback == other.callback;
            }

            public override int GetHashCode()
            {
                return this.callback.GetHashCode();
            }

            public bool EqualOther(IDynamicDispatcherObj dispatcherObj)
            {
                return this.Equals(dispatcherObj);
            }

            public static bool operator ==(DispatcherCallback lhs, DispatcherCallback rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(DispatcherCallback lhs, DispatcherCallback rhs)
            {
                return !(lhs == rhs);
            }
        }


        private EDynamicDispatcher m_cDispatcher;
        public EventDispatcher()
        {
            m_cDispatcher = new EDynamicDispatcher();
        }
        //lua这边如果要使用，不允许同名函数
        public void AddEvent(int key, EventHandler callback,bool once = false, EventPriority priority = EventPriority.NORMAL)
        {
            DispatcherCallback dc = new DispatcherCallback();
            dc.once = once;
            dc.callback = callback;
            m_cDispatcher.AddDispatcherObj(key, dc, (DynamicDispatcherObjEntry.EntryPriority)(int)priority);
        }

        public void RemoveEvent(int key, EventHandler callback)
        {
            DispatcherCallback dc = new DispatcherCallback();
            dc.callback = callback;
            m_cDispatcher.RemoveDispatcherObj(key, dc);
        }

        public void DispatchByParam(int key, params object[] arg)
        {
            m_cDispatcher.DispatchByParam(key, arg);
        }

        public void Dispatch(int key, object args = null)
        {
            m_cDispatcher.Dispatch(key, args);
        }

        public bool HasEvent(int key, EventHandler callback)
        {
            DispatcherCallback dc = new DispatcherCallback();
            dc.callback = callback;
            return m_cDispatcher.HasDispatcher(key, dc);
        }

        public bool HasKey(int key)
        {
            return m_cDispatcher.HasKey(key);
        }

        public void Clear()
        {
            m_cDispatcher.Clear();
        }
    }
}

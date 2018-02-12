using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    public interface IDynamicDispatcherObj
    {
        /// <summary>
        /// 执行派发对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回false移除该对象</returns>
        bool OnDispatcher(object obj);
        bool EqualOther(IDynamicDispatcherObj dispatcherObj);
    }

    public class DynamicDispatcherParam : IPoolable
    {
        public object[] Arg { get; private set; }
        public DynamicDispatcherParam()
        {
        }

        public void Init(params object[] arg)
        {
            this.Arg = arg;
        }

        public void Reset()
        {
            this.Arg = null;
        }
    }

    public class DynamicDispatcherObjEntry : IPoolable
    {
        public enum EntryStatus
        {
            IDLE,
            ACTIVE,
            TOINSERT,
            TODELETE
        }
        public enum EntryPriority
        {
            LOW = 0,
            NORMAL = 1,
            HIGH = 2,
            COUNT = 3
        }

        protected int _key = 0;
        protected IDynamicDispatcherObj _dispatcherObj;
        protected EntryStatus _status = EntryStatus.IDLE;
        protected EntryPriority _priority = EntryPriority.NORMAL;


        public DynamicDispatcherObjEntry()
        {

        }

        public void Reset()
        {
            _key = 0;
            _dispatcherObj = null;
            _status = EntryStatus.IDLE;
            _priority = EntryPriority.NORMAL;
        }

        public int Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public IDynamicDispatcherObj DispatcherObj
        {
            get { return _dispatcherObj; }
            set { _dispatcherObj = value; }
        }

        public EntryStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public EntryPriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
    }

    public class DynamicDispatcherEntry : IPoolable
    {
        public int Key
        { get; set; }

        public object Args
        { get; set; }

        public DynamicDispatcherEntry()
        {

        }

        public void Reset()
        {
            Args = null;
        }
    }
    public class EDynamicDispatcher
    {
        protected static ObjectPool<DynamicDispatcherEntry> _poolDispatch;
        protected static ObjectPool<DynamicDispatcherObjEntry> _pool;
        protected static ObjectPool<DynamicDispatcherParam> _poolParam;

        protected Dictionary<int, List<List<DynamicDispatcherObjEntry>>> _listeners;
        protected bool _isDispatching = false;
        protected List<DynamicDispatcherObjEntry> _pendings;
        protected Queue<DynamicDispatcherEntry> _dispatchPendings;
        protected List<int> _lstRemoveId;
        protected bool _isClear = false;

        public EDynamicDispatcher()
        {
            if (_pool == null)
            {
                _pool = ObjectPool<DynamicDispatcherObjEntry>.Instance;
                _pool.Init(10);
            }
            if (_pendings == null)
            {
                _pendings = new List<DynamicDispatcherObjEntry>();
            }
            if (_poolDispatch == null)
            {
                _poolDispatch = ObjectPool<DynamicDispatcherEntry>.Instance;
                _poolDispatch.Init(10);
            }
            if (_poolParam == null)
            {
                _poolParam = ObjectPool<DynamicDispatcherParam>.Instance;
                _poolParam.Init(10);
            }
            if (_dispatchPendings == null)
            {
                _dispatchPendings = new Queue<DynamicDispatcherEntry>();
            }
            if (_lstRemoveId == null)
            {
                _lstRemoveId = new List<int>();
            }
            _isClear = false;
        }

        public void Clear()
        {
            if (_isDispatching)
            {
                _isClear = true;
            }
            else
            {
                if (_listeners != null)
                {
                    _listeners.Clear();
                }
                if (_pendings != null)
                {
                    _pendings.Clear();
                }
                if (_dispatchPendings != null)
                {
                    _dispatchPendings.Clear();
                }
                if (_lstRemoveId != null)
                {
                    _lstRemoveId.Clear();
                }
                _isDispatching = false;
                _isClear = false;
            }

        }

        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="handler"></param>
        /// <param name="priorty"></param>
        public void AddDispatcherObj(int key, IDynamicDispatcherObj dispatcherObj, DynamicDispatcherObjEntry.EntryPriority priorty = DynamicDispatcherObjEntry.EntryPriority.NORMAL)
        {
            if (HasDispatcher(key, dispatcherObj))
            {
                return;
            }

            if (_listeners == null)
            {
                _listeners = new Dictionary<int, List<List<DynamicDispatcherObjEntry>>>();
            }

            if (!_listeners.ContainsKey(key))
            {
                _listeners[key] = new List<List<DynamicDispatcherObjEntry>>();
                List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];
                int priorityCount = (int)DynamicDispatcherObjEntry.EntryPriority.COUNT;
                for (int i = 0; i < priorityCount; ++i)
                {
                    List<DynamicDispatcherObjEntry> entries = new List<DynamicDispatcherObjEntry>();
                    allEntries.Add(entries);
                }
            }

            DynamicDispatcherObjEntry entry = _pool.GetObject();
            entry.DispatcherObj = dispatcherObj;
            entry.Priority = priorty;
            entry.Key = key;

            if (_isDispatching)
            {
                entry.Status = DynamicDispatcherObjEntry.EntryStatus.TOINSERT;
                _pendings.Add(entry);
            }
            else
            {
                entry.Status = DynamicDispatcherObjEntry.EntryStatus.ACTIVE;
                List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];

                List<DynamicDispatcherObjEntry> entries = allEntries[(int)priorty];
                entries.Add(entry);
            }
        }

        public void AddDispatcherObj(int key, IDynamicDispatcherObj dispatcherObj, int priorty)
        {
            AddDispatcherObj(key, dispatcherObj, (DynamicDispatcherObjEntry.EntryPriority)priorty);
        }

        public void RemoveDispatcherObj(int key, IDynamicDispatcherObj dispatcherObj)
        {
            if (_listeners != null && _listeners.ContainsKey(key))
            {
                if (_isDispatching)
                {
                    DynamicDispatcherObjEntry entry = _pool.GetObject();
                    entry.DispatcherObj = dispatcherObj;
                    entry.Status = DynamicDispatcherObjEntry.EntryStatus.TODELETE;
                    entry.Key = key;
                    _pendings.Add(entry);
                }
                else
                {
                    SafeRemoveDispatcherObj(key, dispatcherObj);
                }
            }
        }

        private void SafeRemoveDispatcherObj(int key, IDynamicDispatcherObj dispatcherObj)
        {
            if (_listeners == null || !_listeners.ContainsKey(key))
            {
                return;
            }

            List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];
            int priorityCount = allEntries.Count;
            int count;
            List<DynamicDispatcherObjEntry> entries;
            for (int i = 0; i < priorityCount; ++i)
            {
                entries = allEntries[i];
                count = entries.Count;
                for (int j = count - 1; j > -1; --j)
                {
                    if (entries[j].DispatcherObj.EqualOther(dispatcherObj))
                    {
                        _pool.SaveObject(entries[j]);
                        entries.RemoveAt(j);
                    }
                }
            }
        }


        public void DispatchByParam(int key, params object[] arg)
        {
            DynamicDispatcherParam param = _poolParam.GetObject();
            param.Init(arg);
            Dispatch(key, param);
        }

        /// <summary>
        /// 派发事件，当参数为一个或没有时调用
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="args"></param>
        public void Dispatch(int key, object args = null)
        {
            if (_listeners == null || !_listeners.ContainsKey(key))
            {
                return;
            }
            if (_isDispatching)
            {
                DynamicDispatcherEntry dispatchEntry = _poolDispatch.GetObject();
                dispatchEntry.Key = key;
                dispatchEntry.Args = args;
                _dispatchPendings.Enqueue(dispatchEntry);
                return;
            }

            _isDispatching = true;

            List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];
            int priorityCount = allEntries.Count;
            int count;
            List<DynamicDispatcherObjEntry> entries;

            for (int i = priorityCount - 1; i > -1; --i)
            {
                entries = allEntries[i];
                count = entries.Count;
                //先添加的事件需要先执行
                _lstRemoveId.Clear();
                for (int j = 0; j < count; ++j)
                {
                    IDynamicDispatcherObj dispatcherObj = entries[j].DispatcherObj;
                    try
                    {
                        object p = args;
                        if (args != null && args is DynamicDispatcherParam)
                        {
                            p = ((DynamicDispatcherParam)args).Arg;
                        }
                        if (!dispatcherObj.OnDispatcher(p))
                        {
                            //移除
                            _lstRemoveId.Add(j);
                        }
                    }
                    catch (Exception e)
                    {
                        CLog.LogError(e.ToString());
                    }
                }
                //移除
                for (int j = _lstRemoveId.Count - 1; j > -1; j--)
                {
                    entries.RemoveAt(_lstRemoveId[j]);
                }
            }
            _isDispatching = false;
            if (_isClear)
            {
                Clear();
            }

            //deal with pendings
            count = _pendings.Count;
            for (int i = 0; i < count; ++i)
            {
                if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TOINSERT)
                {
                    _pendings[i].Status = DynamicDispatcherObjEntry.EntryStatus.ACTIVE;
                    allEntries = _listeners[_pendings[i].Key];
                    entries = allEntries[(int)(_pendings[i].Priority)];
                    entries.Add(_pendings[i]);
                }
                else if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TODELETE)
                {
                    SafeRemoveDispatcherObj(_pendings[i].Key, _pendings[i].DispatcherObj);
                    _pool.SaveObject(_pendings[i]);
                }
            }
            _pendings.Clear();

            if (_dispatchPendings.Count > 0)
            {
                DynamicDispatcherEntry dispatchEntry = _dispatchPendings.Dequeue();
                object p = dispatchEntry.Args;
                if (dispatchEntry.Args != null && dispatchEntry.Args is DynamicDispatcherParam)
                {
                    p = ((DynamicDispatcherParam)dispatchEntry.Args).Arg;
                }
                Dispatch(dispatchEntry.Key, p);
                _poolDispatch.SaveObject(dispatchEntry);
            }
        }

        public bool HasDispatcher(int key, IDynamicDispatcherObj dispatcherObj)
        {
            bool bRet = false;
            if ((_listeners == null) || !_listeners.ContainsKey(key))
            {
                return bRet;
            }

            List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];
            int priorityCount = allEntries.Count;
            int count;
            bool flag = false;
            List<DynamicDispatcherObjEntry> entries;
            for (int i = 0; i < priorityCount; ++i)
            {
                entries = allEntries[i];
                count = entries.Count;
                for (int j = 0; j < count; ++j)
                {
                    if (entries[j].DispatcherObj.EqualOther(dispatcherObj))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    break;
                }
            }

            // when dispatching,check the toInsert,toDelete
            if (_isDispatching)
            {
                count = _pendings.Count;
                int insertTimes = 0;
                for (int i = 0; i < count; ++i)
                {
                    if (_pendings[i].DispatcherObj.EqualOther(dispatcherObj) && _pendings[i].Key == key)
                    {
                        if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TOINSERT)
                        {
                            insertTimes++;
                            if (insertTimes > 1)
                            {
                                insertTimes = 1;
                            }
                        }
                        else if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TODELETE)
                        {
                            insertTimes--;
                            if (insertTimes < -1)
                            {
                                insertTimes = -1;
                            }
                        }
                    }
                }
                if (bRet && (insertTimes < 0))
                {
                    bRet = false;
                }
                if (!bRet && (insertTimes > 0))
                {
                    bRet = true;
                }
            }
            else
            {
                return flag;
            }

            return bRet;
        }

        public bool HasKey(int key)
        {
            bool bRet = false;
            if ((_listeners == null) || !_listeners.ContainsKey(key))
            {
                return bRet;
            }

            List<List<DynamicDispatcherObjEntry>> allEntries = _listeners[key];
            int priorityCount = allEntries.Count;
            int count;
            bool flag = false;
            List<DynamicDispatcherObjEntry> entries;
            for (int i = 0; i < priorityCount; ++i)
            {
                entries = allEntries[i];
                count = entries.Count;
                if (count > 0)
                {
                    flag = true;
                    break;
                }
            }

            // when dispatching,check the toInsert,toDelete
            if (_isDispatching && _pendings.Count > 0)
            {
                count = _pendings.Count;
                int insertTimes = 0;
                for (int i = 0; i < count; ++i)
                {
                    if (_pendings[i].Key == key)
                    {
                        if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TOINSERT)
                        {
                            insertTimes++;
                            if (insertTimes > 1)
                            {
                                insertTimes = 1;
                            }
                        }
                        else if (_pendings[i].Status == DynamicDispatcherObjEntry.EntryStatus.TODELETE)
                        {
                            insertTimes--;
                            if (insertTimes < -1)
                            {
                                insertTimes = -1;
                            }
                        }
                    }
                }
                if (bRet && (insertTimes < 0))
                {
                    bRet = false;
                }
                if (!bRet && (insertTimes > 0))
                {
                    bRet = true;
                }
            }
            else
            {
                return flag;
            }

            return bRet;
        }
    }

}
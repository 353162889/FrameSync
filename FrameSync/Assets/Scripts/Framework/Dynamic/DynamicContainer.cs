using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public interface IDynamicObj
    {
        int key { get; }
    }

    public class DynamicContainer
    {
        public enum DCState
        {
            Init,
            ToAdd,
            ToRemove,
            ToDoAction,
            Error
        }

        public class DCEntity : IPoolable
        {
            public DCState state { get; set; }
            public IDynamicObj obj { get; private set; }

            private int key;
            public int Key
            {
                get
                {
                    if (obj != null) return obj.key;
                    return key;
                }
            }
            public object param { get; private set; }

            public void Init(IDynamicObj obj, object param = null)
            {
                this.obj = obj;
                state = DCState.Init;
                key = -1;
                this.param = param;
            }

            public void Init(int key, object param = null)
            {
                this.obj = null;
                state = DCState.Init;
                this.key = key;
                this.param = param;
            }

            public void Reset()
            {
                state = DCState.Init;
                obj = null;
                key = -1;
                param = null;
            }
        }

        public delegate void DCOperateHandler(IDynamicObj obj, object param);
        public event DCOperateHandler OnAdd;
        public event DCOperateHandler OnRemove;
        public event DCOperateHandler OnUpdate;

        private List<DCEntity> m_lstEntity = new List<DCEntity>();
        private List<DCEntity> m_lstOperateEntity = new List<DCEntity>();
        private ObjectPool<DCEntity> m_cEntityPool;
        private bool m_bIsUpdating = false;

        static DynamicContainer()
        {
            ObjectPool<DCEntity>.Instance.Init(300);
        }

        public DynamicContainer()
        {
            m_lstEntity = new List<DCEntity>();
            m_lstOperateEntity = new List<DCEntity>();
            m_cEntityPool = ObjectPool<DCEntity>.Instance;
        }

        public bool Add(IDynamicObj obj, object param = null)
        {
            if (AddObj(obj, param))
            {
                if (OnAdd != null)
                {
                    OnAdd(obj, param);
                }
                return true;
            }
            return false;
        }

        protected bool AddObj(IDynamicObj obj, object param = null)
        {
            if (Has(obj.key)) return false;
            DCEntity entity = m_cEntityPool.GetObject();
            entity.Init(obj, param);
            if (m_bIsUpdating)
            {
                entity.state = DCState.ToAdd;
                m_lstOperateEntity.Add(entity);
            }
            else
            {
                entity.state = DCState.ToDoAction;
                m_lstEntity.Add(entity);
            }
            return true;
        }

        public bool Remove(IDynamicObj obj)
        {
            if (RemoveObj(obj))
            {
                if (OnRemove != null)
                {
                    OnRemove(obj, null);
                }
                return true;
            }
            else
            {
                CLog.LogError("移除" + obj.key + "失败!");
            }
            return false;
        }

        public bool RemoveKey(int key)
        {
            var obj = RemoveObjKey(key);
            if (obj != null)
            {
                if (OnRemove != null)
                {
                    OnRemove(obj, null);
                }
                return true;
            }
            return false;
        }

        protected bool RemoveObj(IDynamicObj obj)
        {
            if (!Has(obj.key)) return false;
            if (m_bIsUpdating)
            {
                DCEntity entity = m_cEntityPool.GetObject();
                entity.Init(obj);
                entity.state = DCState.ToRemove;
                m_lstOperateEntity.Add(entity);
                for (int i = 0; i < m_lstEntity.Count; i++)
                {
                    if (m_lstEntity[i].Key == obj.key)
                    {
                        m_lstEntity[i].state = DCState.ToRemove;
                    }
                }
            }
            else
            {
                for (int i = m_lstEntity.Count - 1; i > -1; i--)
                {
                    if (m_lstEntity[i].Key == obj.key)
                    {
                        var entity = m_lstEntity[i];
                        m_lstEntity.RemoveAt(i);
                        var entityObj = entity.obj;
                        m_cEntityPool.SaveObject(entity);
                        break;
                    }
                }
            }
            return true;
        }

        protected IDynamicObj RemoveObjKey(int key)
        {
            if (!Has(key)) return null;
            if (m_bIsUpdating)
            {
                DCEntity entity = m_cEntityPool.GetObject();
                entity.Init(key);
                entity.state = DCState.ToRemove;
                m_lstOperateEntity.Add(entity);
                for (int i = 0; i < m_lstEntity.Count; i++)
                {
                    if (m_lstEntity[i].Key == key)
                    {
                        m_lstEntity[i].state = DCState.ToRemove;
                        return m_lstEntity[i].obj;
                    }
                }
                for (int i = 0; i < m_lstOperateEntity.Count; i++)
                {
                    if (m_lstOperateEntity[i].Key == key && m_lstOperateEntity[i].obj != null)
                    {
                        return m_lstOperateEntity[i].obj;
                    }
                }
            }
            else
            {
                for (int i = m_lstEntity.Count - 1; i > -1; i--)
                {
                    if (m_lstEntity[i].Key == key)
                    {
                        var entity = m_lstEntity[i];
                        m_lstEntity.RemoveAt(i);
                        var entityObj = entity.obj;
                        m_cEntityPool.SaveObject(entity);
                        return entityObj;
                    }
                }
            }
            return null;
        }

        public bool Has(int key)
        {
            bool result = false;
            for (int i = 0; i < m_lstEntity.Count; i++)
            {
                if (m_lstEntity[i].Key == key)
                {
                    result = true;
                    break;
                }
            }
            if (m_bIsUpdating)
            {
                int count = result ? 1 : 0;
                for (int i = 0; i < m_lstOperateEntity.Count; i++)
                {
                    var entity = m_lstOperateEntity[i];
                    if (entity.Key == key)
                    {
                        if (entity.state == DCState.ToAdd)
                        {
                            count++;
                        }
                        else if (entity.state == DCState.ToRemove)
                        {
                            count--;
                        }
                    }
                }
                result = count > 0;
            }
            return result;
        }
        public void Update(object param = null)
        {
            m_bIsUpdating = true;
            int count = m_lstEntity.Count;
            for (int i = 0; i < count; i++)
            {
                var entity = m_lstEntity[i];
                if (entity.state == DCState.Error)
                    continue;
                if (entity.state == DCState.ToDoAction)
                {
                    try
                    {
                        if (OnUpdate != null)
                        {
                            OnUpdate(entity.obj, param);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        entity.state = DCState.Error;
                        Remove(entity.obj);
                        CLog.LogError(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            m_bIsUpdating = false;
            int count1 = m_lstOperateEntity.Count;
            for (int i = 0; i < count1; i++)
            {
                var entity = m_lstOperateEntity[i];
                if (entity.state == DCState.ToAdd)
                {
                    AddObj(entity.obj, entity.param);
                }
                else if (entity.state == DCState.ToRemove)
                {
                    RemoveObj(entity.obj);
                }
                m_cEntityPool.SaveObject(entity);
            }
            m_lstOperateEntity.Clear();
        }

        public string GetCurLstKey()
        {
            string result = "";
            for (int i = 0; i < m_lstEntity.Count; i++)
            {
                result += m_lstEntity[i].Key + ",";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        public string GetOperateLstKey()
        {
            string result = "";
            for (int i = 0; i < m_lstOperateEntity.Count; i++)
            {
                result += m_lstOperateEntity[i].Key + ",";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        public void Clear()
        {
            for (int i = m_lstEntity.Count - 1; i > -1; i--)
            {
                Remove(m_lstEntity[i].obj);
            }
        }

        public void Dispose()
        {
            OnAdd = null;
            OnRemove = null;
            OnUpdate = null;
            Clear();
        }
    }
}
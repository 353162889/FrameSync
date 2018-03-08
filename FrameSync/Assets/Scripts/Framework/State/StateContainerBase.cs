using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class StateContainerBase : StateBase
    {
        protected Dictionary<int, StateBase> m_dicState = new Dictionary<int, StateBase>();
        protected StateBase m_cCurState = null;

        public virtual bool SwitchState(int stateKey,IStateContext context = null)
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnExit();
            }
            m_cCurState = GetState(stateKey);
            if(m_cCurState != null)
            {
                m_cCurState._OnEnter(context);
            }
            return false;
        }

        public virtual bool AddState(int key,StateBase state,bool defaultState = false)
        {
            if (m_dicState.ContainsKey(key)) return false;
            m_dicState.Add(key, state);
            if(defaultState)
            {
                m_cCurState = state;
            }
            return true;
        }

        public virtual bool RemoveState(int key)
        {
            StateBase state = GetState(key);
            if (state == null) return false;
            if(state == m_cCurState)
            {
                m_cCurState._OnExit();
                m_cCurState = null;
            }
            state._OnDispose();
            m_dicState.Remove(key);
            return true;
        }

        protected override void OnEnter()
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnEnter(m_cContext);
            }
            else
            {
                CLog.Log("状态容器:"+this.GetType().ToString()+"没有设置默认状态",CLogColor.Red);
            }
        }

        protected override void OnExit()
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnExit();
                m_cCurState = null;
            }
        }

        protected override void OnUpdate()
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnUpdate();
            }
        }

        protected override void OnLogicUpdate()
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnLogicUpdate();
            }
        }

        protected override void OnLateUpdate()
        {
            if(m_cCurState != null)
            {
                m_cCurState._OnLateUpdate();
            }
        }

        protected override void OnDispose()
        {
            foreach (var item in m_dicState)
            {
                item.Value._OnDispose();
            }
            m_dicState.Clear();
            m_cCurState = null;
        }


        public virtual StateBase GetState(int stateKey)
        {
            StateBase result = null;
            m_dicState.TryGetValue(stateKey, out result);
            return result;
        }
    }
}

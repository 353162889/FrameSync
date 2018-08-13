using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class StateContainerBase : StateBase
    {
        public delegate void StateContainerHandler(int oldState, int newState);
        public event StateContainerHandler OnBeforeSwitchState;
        public event StateContainerHandler OnAfterSwitchState;
        protected Dictionary<int, StateBase> m_dicState = new Dictionary<int, StateBase>();
        protected StateBase m_cCurState = null;

        public virtual bool SwitchState(int stateKey,IStateContext context = null)
        {
            int oldStateKey = m_cCurState == null ? -1 : m_cCurState.key;
            if(null != OnBeforeSwitchState)
            {
                OnBeforeSwitchState(oldStateKey, stateKey);
            }
            if(m_cCurState != null)
            {
                m_cCurState._OnExit();
            }
            m_cCurState = GetState(stateKey);
            bool succ = false;
            if(m_cCurState != null)
            {
                m_cCurState._OnEnter(context);
                succ = true;
            }
            if(null != OnAfterSwitchState)
            {
                OnAfterSwitchState(oldStateKey, stateKey);
            }
            return succ;
        }

        public virtual bool AddState(int key,StateBase state,bool defaultState = false)
        {
            if (m_dicState.ContainsKey(key)) return false;
            m_dicState.Add(key, state);
            state.Parent = this;
            state.UpdateKey(key);
            if (defaultState)
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
            state.UpdateKey(-1);
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

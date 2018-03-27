using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class StateBase
    {
        public delegate void StateHandler(StateBase state);
        public event StateHandler OnBeforeEnter;
        public event StateHandler OnAfterEnter;
        public event StateHandler OnBeforeExit;
        public event StateHandler OnAfterExit;
        public StateContainerBase Parent { get; set; }
        protected int m_nKey;
        public int key { get { return m_nKey; } }
        public bool StateEnabled { get { return mStateEnabled; } }
        protected bool mStateEnabled = false;
        protected IStateContext m_cContext;

        public void _OnEnter(IStateContext context = null)
        {
            if(null != OnBeforeEnter)
            {
                OnBeforeEnter(this);
            }
            m_cContext = context;
            OnEnter();
            mStateEnabled = true;
            if (null != OnAfterEnter)
            {
                OnAfterEnter(this);
            }
        }

        public void UpdateKey(int key)
        {
            m_nKey = key;
        }

        protected virtual void OnEnter()
        {

        }


        public void _OnLogicUpdate()
        {
            if(mStateEnabled)
            {
                OnLogicUpdate();
            }
        }
        protected virtual void OnLogicUpdate()
        {

        }

        public void _OnUpdate()
        {
            if (mStateEnabled)
            {
                OnUpdate();
            }
        }
        protected virtual void OnUpdate()
        {

        }

        public void _OnLateUpdate()
        {
            if(mStateEnabled)
            {
                OnLateUpdate();
            }
        }

        protected virtual void OnLateUpdate()
        {

        }

        public void _OnExit()
        {
            if(null != OnBeforeExit)
            {
                OnBeforeExit(this);
            }
            OnExit();
            m_cContext = null;
            mStateEnabled = false;
            if(null != OnAfterExit)
            {
                OnAfterExit(this);
            }
        }


        protected virtual void OnExit()
        {

        }

        public void _OnDispose()
        {
            OnBeforeEnter = null;
            OnAfterEnter = null;
            OnBeforeExit = null;
            OnAfterExit = null;
            OnDispose();
            m_nKey = -1;
            m_cContext = null;
            mStateEnabled = false;
        }

        protected virtual void OnDispose()
        {

        }

        public bool ParentSwitchState(int state)
        {
            if(Parent != null)
            {
                return Parent.SwitchState(state);
            }
            return false;
        }
    }
}

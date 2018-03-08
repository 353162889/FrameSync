using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class StateBase
    {
        public StateContainerBase Parent { get; set; }
        public bool StateEnabled { get { return mStateEnabled; } }
        protected bool mStateEnabled = false;
        protected IStateContext m_cContext;

        public void _OnEnter(IStateContext context = null)
        {
            m_cContext = context;
            OnEnter();
            mStateEnabled = true;
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
            OnExit();
            m_cContext = null;
            mStateEnabled = false;
        }


        protected virtual void OnExit()
        {

        }

        public void _OnDispose()
        {
            OnDispose();
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

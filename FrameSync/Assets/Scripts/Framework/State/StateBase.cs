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

        public virtual void OnEnter()
        {

        }

        public virtual void OnLogicUpdate()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnExit()
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

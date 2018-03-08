using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class StateContainerBase : StateBase
    {
        public virtual bool SwitchState(int state)
        {
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class SharedBool : BTSharedVariable<bool>
    {
        public static implicit operator SharedBool(bool value) { return new SharedBool { mValue = value }; }
    }
}

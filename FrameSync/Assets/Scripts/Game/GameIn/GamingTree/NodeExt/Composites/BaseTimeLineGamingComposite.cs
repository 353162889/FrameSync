using BTCore;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public abstract class BaseTimeLineGamingComposite : BaseGamingComposite, IBTTimeLineNode
    {
        public abstract FP time { get; }
    }
}

using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    abstract public class BaseTimeLineBTGameDecorator : BaseBTGameDecorator, IBTTimeLineNode
    {
        public abstract FP time { get; }
    }
}

using BTCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace Game
{
    public abstract class BaseTimeLineSkillAction :BaseSkillAction, IBTTimeLineNode
    {
        public abstract FP time { get; }
    }
}

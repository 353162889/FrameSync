using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    [NENodeCategory("Condition")]
    public abstract class BTCondition : BTNode
    {
        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if(Evaluate(blackBoard))
            {
                return BTResult.Success;
            } 
            else
            {
                return BTResult.Failure;
            }
        }
        public virtual bool Evaluate(BTBlackBoard blackBoard) { return true; }
    }
}

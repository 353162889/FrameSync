using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("BTGame/Composite")]
    public class BaseBTGameComposite : BTComposite
    {
        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            return this.OnTick((AgentObjectBlackBoard)blackBoard);
        }

        public virtual BTResult OnTick(AgentObjectBlackBoard blackBoard)
        {
            return base.OnTick(blackBoard);
        }
    }
}

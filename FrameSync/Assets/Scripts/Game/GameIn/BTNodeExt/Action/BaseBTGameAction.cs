using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("BTGame/Action")]
    public class BaseBTGameAction : BTAction
    {
        sealed public override void OnEnter(BTBlackBoard blackBoard)
        {
            this.OnEnter((AgentObjectBlackBoard)blackBoard);
        }

        protected virtual void OnEnter(AgentObjectBlackBoard blackBoard) { }

        sealed public override void OnExit(BTBlackBoard blackBoard)
        {
            this.OnExit((AgentObjectBlackBoard)blackBoard);
        }
        public virtual void OnExit(AgentObjectBlackBoard blackBoard) { }

        sealed public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            return this.OnRun((AgentObjectBlackBoard)blackBoard);
        }
        public virtual BTActionResult OnRun(AgentObjectBlackBoard blackBoard) { return BTActionResult.Running; }
    }
}

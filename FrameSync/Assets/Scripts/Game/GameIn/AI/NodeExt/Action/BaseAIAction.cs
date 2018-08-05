using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("AI/Action")]
    public class BaseAIAction : BTAction
    {
        sealed public override void OnEnter(BTBlackBoard blackBoard)
        {
            this.OnEnter((AIBlackBoard)blackBoard);
        }

        protected virtual void OnEnter(AIBlackBoard blackBoard) { }

        sealed public override void OnExit(BTBlackBoard blackBoard)
        {
            this.OnExit((AIBlackBoard)blackBoard);
        }
        public virtual void OnExit(AIBlackBoard blackBoard) { }

        sealed public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            return this.OnRun((AIBlackBoard)blackBoard);
        }
        public virtual BTActionResult OnRun(AIBlackBoard blackBoard) { return BTActionResult.Running; }
    }
}

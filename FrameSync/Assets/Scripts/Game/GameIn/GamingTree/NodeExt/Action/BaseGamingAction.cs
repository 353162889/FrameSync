using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{

    [NENodeCategory("Gaming/Action")]
    public class BaseGamingAction : BTAction
    {
        sealed public override void OnEnter(BTBlackBoard blackBoard)
        {
            this.OnEnter((GamingBlackBoard)blackBoard);
        }

        protected virtual void OnEnter(GamingBlackBoard blackBoard) { }

        sealed public override void OnExit(BTBlackBoard blackBoard)
        {
            this.OnExit((GamingBlackBoard)blackBoard);
        }
        public virtual void OnExit(GamingBlackBoard blackBoard) { }

        sealed public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            return this.OnRun((GamingBlackBoard)blackBoard);
        }
        public virtual BTActionResult OnRun(GamingBlackBoard blackBoard) { return BTActionResult.Running; }
    }
}

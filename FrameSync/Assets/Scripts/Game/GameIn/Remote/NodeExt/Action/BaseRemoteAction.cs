using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("Remote/Action")]
    public class BaseRemoteAction : BTAction
    {
        sealed public override void OnEnter(BTBlackBoard blackBoard)
        {
            this.OnEnter((RemoteBlackBoard)blackBoard);
        }

        protected virtual void OnEnter(RemoteBlackBoard blackBoard) { }

        sealed public override void OnExit(BTBlackBoard blackBoard)
        {
            this.OnExit((RemoteBlackBoard)blackBoard);
        }
        public virtual void OnExit(RemoteBlackBoard blackBoard) { }

        sealed public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            return this.OnRun((RemoteBlackBoard)blackBoard);
        }
        public virtual BTActionResult OnRun(RemoteBlackBoard blackBoard) { return BTActionResult.Running; }
    }
}

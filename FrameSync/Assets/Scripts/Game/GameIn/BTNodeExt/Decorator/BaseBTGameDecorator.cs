using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using BTCore;
using NodeEditor;

namespace Game
{
    [NENodeCategory("BTGame/Decorator")]
    public class BaseBTGameDecorator : BTDecorator
    {
        sealed public override BTResult OnEnter(BTBlackBoard blackBoard)
        {
            return this.OnEnter((AgentObjectBlackBoard)blackBoard);
        }

        public virtual BTResult OnEnter(AgentObjectBlackBoard blackBoard) { return base.OnEnter(blackBoard); }

        sealed public override BTResult Decorate(BTBlackBoard blackBoard, BTResult result)
        {
            return this.Decorate((AgentObjectBlackBoard)blackBoard, result);
        }

        public virtual BTResult Decorate(AgentObjectBlackBoard blackBoard, BTResult result) { return result; }
    }
}

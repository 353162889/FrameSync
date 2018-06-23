using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("Skill/Action")]
    public class BaseSkillAction : BTAction
    {
        sealed public override void OnEnter(BTBlackBoard blackBoard)
        {
            this.OnEnter((SkillBlackBoard)blackBoard);
        }

        protected virtual void OnEnter(SkillBlackBoard blackBoard) { }

        sealed public override void OnExit(BTBlackBoard blackBoard)
        {
            this.OnExit((SkillBlackBoard)blackBoard);
        }
        public virtual void OnExit(SkillBlackBoard blackBoard) { }

        sealed public override BTActionResult OnRun(BTBlackBoard blackBoard)
        {
            return this.OnRun((SkillBlackBoard)blackBoard);
        }
        public virtual BTActionResult OnRun(SkillBlackBoard blackBoard) { return BTActionResult.Running; }
    }
}

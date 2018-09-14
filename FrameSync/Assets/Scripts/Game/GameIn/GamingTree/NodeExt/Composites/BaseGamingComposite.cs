using BTCore;
using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    [NENodeCategory("Gaming/Composite")]
    public class BaseGamingComposite : BTComposite
    {
        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            return this.OnTick((GamingBlackBoard)blackBoard);
        }

        public virtual BTResult OnTick(GamingBlackBoard blackBoard)
        {
            return base.OnTick(blackBoard);
        }
    }
}

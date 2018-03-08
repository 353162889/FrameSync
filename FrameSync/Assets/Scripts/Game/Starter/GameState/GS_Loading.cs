using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class GS_Loading : StateBase
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            CLog.Log("GS_Loading:OnEnter");
        }

        protected override void OnExit()
        {
            base.OnExit();
            CLog.Log("GS_Loading:OnExit");
        }
    }
}

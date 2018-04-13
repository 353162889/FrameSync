using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game
{
    public class LoadingView : BaseSubView
    {
        public LoadingView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            base.BuidUI();
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
            CLog.Log("LoadingView[OnEnter]");
        }

        public override void OnExit()
        {
            base.OnExit();
            CLog.Log("LoadingView[OnExit]");
        }
    }
}

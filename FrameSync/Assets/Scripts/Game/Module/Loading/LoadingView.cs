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
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartMatchOther, OnStartMatchOther);
        }

        private void OnStartMatchOther(object args)
        {

        }

        public override void OnExit()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.StartMatchOther, OnStartMatchOther);
            base.OnExit();
            CLog.Log("LoadingView[OnExit]");
        }
    }
}

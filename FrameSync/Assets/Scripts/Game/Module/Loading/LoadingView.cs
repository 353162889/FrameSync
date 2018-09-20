using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LoadingView : BaseSubView
    {
        private string[] m_arrPoints = new string[] { "...","..","."};
        private Text m_cTxtDesc;
        private string m_sDesc;
        private int m_nPointIdx;
        private float m_fSpaceTime = 0.5f;
        private float m_fCurTime;
        public LoadingView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            m_cTxtDesc = this.MainGO.FindChildComponentRecursive<Text>("TxtDesc");
            base.BuidUI();
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
            CLog.Log("LoadingView[OnEnter]");
            m_cTxtDesc.text = "正在进入游戏...";
            m_nPointIdx = 0;
            m_sDesc = "正在进入游戏";
            m_fCurTime = 0;
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartMatchOther, OnStartMatchOther);
        }

        private void OnStartMatchOther(object args)
        {
            m_sDesc = "正在匹配其他玩家";
        }

        public override void OnUpdate()
        {
            m_fCurTime += Time.deltaTime;
            if (m_fCurTime > m_fSpaceTime)
            {
                m_fCurTime -= m_fSpaceTime;
                m_cTxtDesc.text = m_sDesc + m_arrPoints[m_nPointIdx];
                m_nPointIdx = (m_nPointIdx + 1) % m_arrPoints.Length;
            }

        }

        public override void OnExit()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.StartMatchOther, OnStartMatchOther);
            base.OnExit();
            CLog.Log("LoadingView[OnExit]");
        }
    }
}

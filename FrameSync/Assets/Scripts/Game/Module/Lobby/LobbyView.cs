using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using GameData;

namespace Game
{
    public class LobbyView : BaseSubView
    {
        private Button m_cBtnSingle;
        private Button m_cBtnMulti;
        public LobbyView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            base.BuidUI();
            m_cBtnSingle = this.MainGO.FindChildComponentRecursive<Button>("BtnSingle");
            m_cBtnMulti = this.MainGO.FindChildComponentRecursive<Button>("BtnMulti");
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            UIEventTrigger.Get(m_cBtnSingle.gameObject).AddListener(EventTriggerType.PointerClick, OnClickSingle);
            UIEventTrigger.Get(m_cBtnMulti.gameObject).AddListener(EventTriggerType.PointerClick, OnClickMulti);
        }

        private void OnClickSingle(BaseEventData arg0)
        {
            LobbyController.Instance.JoinSingle();
        }

        private void OnClickMulti(BaseEventData arg0)
        {
            LobbyController.Instance.JoinMulti();
        }

        public override void UnbindEvents()
        {
            base.UnbindEvents();
        }

        public override void DestroyUI()
        {
            base.DestroyUI();
        }
    }
}
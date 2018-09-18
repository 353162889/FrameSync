using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Game
{
    public class PopupTipsView : BaseSubView
    {
        private GameObject m_cMask;
        private Button m_cBtnConfirm;
        private Button m_cBtnCancel;
        private Text m_cTxtTitle;
        private Text m_cTxtContent;
        private PopupViewOpenObject m_cOpenObject;
        private float m_fContentHeight;
        public PopupTipsView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            base.BuidUI();
            m_cMask = this.MainGO.FindChildRecursive("BgMask");
            m_cBtnConfirm = this.MainGO.FindChildComponentRecursive<Button>("BtnConfirm");
            m_cBtnCancel = this.MainGO.FindChildComponentRecursive<Button>("BtnCancel");
            m_cTxtTitle = this.MainGO.FindChildComponentRecursive<Text>("TxtTitle");
            m_cTxtContent = this.MainGO.FindChildComponentRecursive<Text>("TxtContent");
            m_fContentHeight = m_cTxtContent.preferredHeight;
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            UIEventTrigger.Get(m_cMask).AddListener(EventTriggerType.PointerClick, OnCancel);
            UIEventTrigger.Get(m_cBtnCancel.gameObject).AddListener(EventTriggerType.PointerClick, OnCancel);
            UIEventTrigger.Get(m_cBtnConfirm.gameObject).AddListener(EventTriggerType.PointerClick, OnConfirm);
        }

        private void OnConfirm(BaseEventData arg0)
        {
            this.CloseThis();
            if(m_cOpenObject != null && m_cOpenObject.confirmCallback != null)
            {
                Action callback = m_cOpenObject.confirmCallback;
                callback.Invoke();
            }
        }

        private void OnCancel(BaseEventData arg0)
        {
            this.CloseThis();
            if (m_cOpenObject != null && m_cOpenObject.cancelCallback != null)
            {
                Action callback = m_cOpenObject.cancelCallback;
                callback.Invoke();
            }
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
            m_cOpenObject = (PopupViewOpenObject)openParam.objParam;
            if(m_cOpenObject == null)
            {
                this.CloseThis();
                return;
            }

            m_cTxtTitle.text = m_cOpenObject.title;
            m_cTxtContent.text = m_cOpenObject.content;
            if(!string.IsNullOrEmpty(m_cTxtContent.text))
            {
                float lineCount = m_cTxtContent.preferredHeight / this.m_fContentHeight;
                if(lineCount < 2)
                {
                    m_cTxtContent.alignment = TextAnchor.MiddleCenter;
                }
                else
                {
                    m_cTxtContent.alignment = TextAnchor.MiddleLeft;
                }
            }
        }

        public override void OnExit()
        {
            m_cOpenObject = null;
            base.OnExit();
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
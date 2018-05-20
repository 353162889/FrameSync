using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class FightView : BaseSubView
    {
        private UIJoystick m_cJoystick;
        private RectTransform m_cBaseTrans;
        private RectTransform m_cMoveTrans;
        private GameObject m_cBtnSkillTest;
        private float minDirLen;
        public FightView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            RectTransform rectTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickRect");
            m_cBaseTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickBase");
            m_cMoveTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickMove");
            m_cBtnSkillTest = this.MainGO.FindChildRecursive("btnSkillTest");
            GameObject joystickGO = this.MainGO.FindChildRecursive("Joystick");
            m_cJoystick = joystickGO.AddComponentOnce<UIJoystick>();
            m_cJoystick.Init(rectTrans, m_cBaseTrans, m_cMoveTrans, m_cBaseTrans.rect.width / 2f, false);
            minDirLen = m_cBaseTrans.rect.width * 0.1f;
            m_cJoystick.OnBegin += OnJoystickBegin;
            m_cJoystick.OnMove += OnJoystickMove;
            m_cJoystick.OnEnd += OnJoystickEnd;
            //m_cJoystick.enabled = false;
            SetJoystickActive(false);

            UIEventTrigger.Get(m_cBtnSkillTest).AddListener(EventTriggerType.PointerClick, OnClick);
        }

        private void OnClick(BaseEventData data)
        {
            if(PvpPlayerMgr.Instance.mainPlayer != null)
            {
                Unit unit = PvpPlayerMgr.Instance.mainPlayer.unit;
                if(unit != null)
                {
                    if (unit.GetSkill(1) == null) unit.AddSkill(1);
                    unit.ReqDoSkill(1, 0, AgentObjectType.Unit, unit.curPosition, unit.curForward);
                }
            }
            
        }

        private void SetJoystickActive(bool active)
        {
            if (m_cBaseTrans.gameObject.activeSelf != active)
            {
                m_cBaseTrans.gameObject.SetActive(active);
                m_cMoveTrans.gameObject.SetActive(active);
            }
        }

        private void OnJoystickBegin(Vector2 vector2)
        {
            SetJoystickActive(true);
        }

        private void OnJoystickMove(Vector2 vector2)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                if (vector2.magnitude > minDirLen)
                {
                    var dir = new TSVector(FP.FromFloat(vector2.x), 0, FP.FromFloat(vector2.y));
                    dir.Normalize();
                    PvpPlayerMgr.Instance.mainPlayer.unit.ReqMoveForward(dir);
                }
            }
        }

        private void OnJoystickEnd(Vector2 vector2)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                PvpPlayerMgr.Instance.mainPlayer.unit.ReqStopMove();
            }
            SetJoystickActive(false);
        }

        public override void DestroyUI()
        {
            m_cJoystick.OnMove -= OnJoystickMove;
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
        }
    }
}

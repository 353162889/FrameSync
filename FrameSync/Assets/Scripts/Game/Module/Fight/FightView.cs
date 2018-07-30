using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class FightView : BaseSubView
    {
        private UIJoystick m_cJoystick;
        private RectTransform m_cBaseTrans;
        private RectTransform m_cMoveTrans;
        private GameObject m_cBtnSkillTest;
        private Text m_txtFps;
        private float minDirLen;
        public FightView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            m_txtFps = this.MainGO.FindChildComponentRecursive<Text>("txtFps");
            RectTransform rectTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickRect");
            m_cBaseTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickBase");
            m_cMoveTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickMove");
            m_cBtnSkillTest = this.MainGO.FindChildRecursive("btnSkillTest");
            GameObject joystickGO = this.MainGO.FindChildRecursive("Joystick");
            m_cJoystick = joystickGO.AddComponentOnce<UIJoystick>();
            m_cJoystick.Init(rectTrans, m_cBaseTrans, m_cMoveTrans, m_cBaseTrans.rect.width / 2f, true);
            minDirLen = m_cBaseTrans.rect.width * 0.1f;
            minDirLen = 5f;
            m_cJoystick.OnBegin += OnJoystickBegin;
            m_cJoystick.OnMove += OnJoystickMove;
            m_cJoystick.OnEnd += OnJoystickEnd;
            m_cJoystick.AddKeyCode(KeyCode.A, Vector2.left);
            m_cJoystick.AddKeyCode(KeyCode.D, Vector2.right);
            m_cJoystick.AddKeyCode(KeyCode.W, Vector2.up);
            m_cJoystick.AddKeyCode(KeyCode.S, Vector2.down);
            //m_cJoystick.enabled = false;
            SetJoystickActive(false);

            UIEventTrigger.Get(m_cBtnSkillTest).AddListener(EventTriggerType.PointerClick, OnClick);

            //TouchDispatcher.instance.touchBeganListeners += OnTouch;
        }

        private void OnTouch(TouchEventParam obj)
        {
            Ray ray = Camera.main.ScreenPointToRay(obj.GetTouch(0).position);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo))
            {
                if (PvpPlayerMgr.Instance.mainPlayer != null)
                {
                    Unit unit = PvpPlayerMgr.Instance.mainPlayer.unit;
                    if (unit != null)
                    {
                        unit.ReqMove(TSVector.FromUnitVector3(hitInfo.point));
                    }
                }
            }
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
            //if (m_cBaseTrans.gameObject.activeSelf != active)
            //{
            //    m_cBaseTrans.gameObject.SetActive(active);
            //    m_cMoveTrans.gameObject.SetActive(active);
            //}
        }

        private void OnJoystickBegin(Vector2 screenPos, Vector2 offset,Vector2 delta)
        {
            SetJoystickActive(true);
        }

        private void OnJoystickMove(Vector2 screenPos, Vector2 offset, Vector2 delta)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                if ((!Mathf.Approximately(offset.x,0) || !Mathf.Approximately(offset.y,0)) && offset.magnitude > minDirLen)
                {
                    var dir = new TSVector(FP.FromFloat(offset.x), 0, FP.FromFloat(offset.y));
                    dir.Normalize();
                    PvpPlayerMgr.Instance.mainPlayer.unit.ReqMoveForward(dir);
                }
                if(!Mathf.Approximately(delta.x, 0) || !Mathf.Approximately(delta.y, 0))
                {

                }
            }
        }

        private void OnJoystickEnd(Vector2 screenPos, Vector2 offset,Vector2 delta)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                PvpPlayerMgr.Instance.mainPlayer.unit.ReqStopMove();
            }
            //SetJoystickActive(false);
        }

        public override void DestroyUI()
        {
            m_cJoystick.OnBegin -= OnJoystickBegin;
            m_cJoystick.OnMove -= OnJoystickMove;
            m_cJoystick.OnEnd -= OnJoystickEnd;
        }

        public override void OnEnter(ViewParam openParam)
        {
            base.OnEnter(openParam);
        }

        public override void OnUpdate()
        {
            m_txtFps.text = FPSMono.Instance.realFPS.ToString();
        }
    }
}

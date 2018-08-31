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
        private Button m_btnTest;
        private float minDirLen;
        public FightView(GameObject go) : base(go)
        {
        }

        protected override void BuidUI()
        {
            m_txtFps = this.MainGO.FindChildComponentRecursive<Text>("txtFps");
            m_btnTest = this.MainGO.FindChildComponentRecursive<Button>("btnTest");
            RectTransform rectTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickRect");
            m_cBaseTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickBase");
            m_cMoveTrans = this.MainGO.FindChildComponentRecursive<RectTransform>("JoystickMove");
            GameObject joystickGO = this.MainGO.FindChildRecursive("Joystick");
            
            m_cJoystick = joystickGO.AddComponentOnce<UIJoystick>();
            m_cJoystick.Init(rectTrans, m_cBaseTrans, m_cMoveTrans, 100000 /*m_cBaseTrans.rect.width / 2f*/, false);
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

            UIEventTrigger.Get(m_btnTest.gameObject).AddListener(EventTriggerType.PointerClick, OnClickTest);
        }

        private void OnClickTest(BaseEventData arg0)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null)
            {
                Unit unit = PvpPlayerMgr.Instance.mainPlayer.unit;
                if (unit != null)
                {
                    unit.ReqDoSkill(1, 0, AgentObjectType.Unit, TSVector.zero, TSVector.forward);
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

        private Vector3 startPosition = Vector3.positiveInfinity;
        private void OnJoystickBegin(Vector2 screenPos, Vector2 offset,Vector2 delta)
        {
            //SetJoystickActive(true);
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                startPosition = PvpPlayerMgr.Instance.mainPlayer.unit.curPosition.ToUnityVector3();
            }
            OnJoystickMove(screenPos, offset, delta);
        }

        private float m_fLastReqSetPositionTime = float.MinValue;
        private float m_fLastReqStopTime = float.MinValue;
        private float m_fLastReqMoveTime = float.MinValue;
        private List<TSVector> m_lstMovePath = new List<TSVector>();
        private void OnJoystickMove(Vector2 screenPos, Vector2 offset, Vector2 delta)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                var unit = PvpPlayerMgr.Instance.mainPlayer.unit;
                if (m_cJoystick.IsKeyTouch)
                {
                    if ((!Mathf.Approximately(offset.x, 0) || !Mathf.Approximately(offset.y, 0)) && offset.magnitude > minDirLen)
                    {
                        var curOffset = offset.normalized;
                        TSVector2 start = new TSVector2(unit.curPosition.x,unit.curPosition.z);
                        TSVector2 dir = new TSVector2(FP.FromFloat(curOffset.x), FP.FromFloat(curOffset.y));
                        dir.Normalize();
                        //FP dis =TSMath.Min(CameraSys.Instance.cameraViewPort.mRect.halfWidth, CameraSys.Instance.cameraViewPort.mRect.halfHeight);
                        FP dis = TSMath.Max(CameraSys.Instance.cameraViewPort.mRect.width, CameraSys.Instance.cameraViewPort.mRect.height) * 10;

                        TSVector2 end = start + dir * dis;
                        TSVector2 hitPoint;
                        TSVector2 resultPoint;
                        bool hit = GetViewPortPositionByLine(start, end, out hitPoint, out resultPoint);
                        if(resultPoint == start)
                        {
                            if (Time.time - m_fLastReqStopTime > ViewConst.OnFrameTime)
                            {
                                m_fLastReqStopTime = Time.time;
                                unit.ReqStopMove();
                            }
                        }
                        else
                        {
                            //这里加了会导致一些需要改变的帧发布出去，例如，先按左键，然后立即按上键（0.001s）会导致最终左上方向被忽略掉
                            //if (Time.time - m_fLastReqMoveTime > ViewConst.OnFrameTime)
                            //{
                            //m_fLastReqMoveTime = Time.time;
                                if (hit)
                                {
                                    m_lstMovePath.Clear();
                                    m_lstMovePath.Add(new TSVector(hitPoint.x, unit.curPosition.y, hitPoint.y));
                                    m_lstMovePath.Add(new TSVector(resultPoint.x, unit.curPosition.y, resultPoint.y));
                                    unit.ReqMove(m_lstMovePath);
                                }
                                else
                                {
                                    unit.ReqMove(new TSVector(resultPoint.x, unit.curPosition.y, resultPoint.y));
                                }
                            //}
                        }
                       
                    }
                }
                else
                {
                    if (startPosition != Vector3.positiveInfinity && Time.time - m_fLastReqSetPositionTime > ViewConst.OnFrameTime && !Mathf.Approximately(delta.x, 0) || !Mathf.Approximately(delta.y, 0))
                    {
                        var curPos = unit.curPosition.ToUnityVector3();
                        var scenePosDelta = CameraSys.Instance.GetSceneDeltaByScreenDelta(offset);

                        var nextPos = startPosition + new Vector3(scenePosDelta.x,0,scenePosDelta.y);
                        TSVector2 start = new TSVector2(unit.curPosition.x, unit.curPosition.z);
                        TSVector2 end = new TSVector2(FP.FromFloat(nextPos.x),FP.FromFloat(nextPos.z));
                        TSVector2 hitPoint;
                        TSVector2 resultPoint;
                        bool hit = GetViewPortPositionByLine(start, end,out hitPoint,out resultPoint);
                        unit.ReqSetPosition(new TSVector(resultPoint.x, unit.curPosition.y, resultPoint.y), false);
                        m_fLastReqSetPositionTime = Time.time;
                    }
                }
            }
        }

        public bool GetViewPortPositionByLine(TSVector2 start, TSVector2 end,out TSVector2 hitPoint,out TSVector2 resultPoint)
        {
            hitPoint = start;
            resultPoint = end;
            TSRect rect = CameraSys.Instance.cameraViewPort.mRect;
            if (rect.Contains(end)) return false;
            TSVector2 result;
            var dir = (end - start);
            var startLen = dir.magnitude;
            bool hit = false;
            if (dir == TSVector2.zero)
            {
                result = start;
            }   
            else
            {
                dir.Normalize();
                FP nDis = TSCheck2D.CheckAabbAndLine(new TSVector2(rect.xCenter, rect.yCenter), rect.width / 2, rect.height / 2, start, dir, startLen);
                if (nDis < 0)
                {
                    result = start;
                }
                else
                {
                    result = start + dir * nDis;
                    hit = true;
                }
            }
            hitPoint = result;
            FP endLen = (result - start).magnitude;
            if(endLen < startLen && endLen < FP.EN8)
            {
                TSVector2 nextPos = result;
                if (dir.x < 0 && dir.y < 0)
                {
                    nextPos = new TSVector2(rect.xMin, rect.yMin);
                }
                else if(dir.x > 0 && dir.y < 0)
                {
                    nextPos = new TSVector2(rect.xMax,rect.yMin);
                }
                else if(dir.x < 0 && dir.y > 0)
                {
                    nextPos = new TSVector2(rect.xMin, rect.yMax);
                }
                else if(dir.x > 0 && dir.y > 0)
                {
                    nextPos = new TSVector2(rect.xMax, rect.yMax);
                }
                var nextDir = nextPos - result;
                if (nextDir != TSVector2.zero)
                {
                    var nextLen = nextDir.magnitude;
                    nextDir.Normalize();
                    FP subLen = 0;
                    if(nextDir.x != 0)
                    {
                        subLen = TSMath.Abs(end.x - start.x) - TSMath.Abs(result.x - start.x);
                    }
                    else if(nextDir.y != 0)
                    {
                        subLen = TSMath.Abs(end.y - start.y) - TSMath.Abs(result.y - start.y);
                    }
                    if (subLen > 0)
                    {
                        if (subLen <= nextLen)
                        {
                            result = result + nextDir * subLen;
                        }
                        else
                        {
                            result = nextPos;
                        }
                    }
                }
            }
            resultPoint = result;  
            return hit;           
        }

        private void OnJoystickEnd(Vector2 screenPos, Vector2 offset,Vector2 delta)
        {
            if (PvpPlayerMgr.Instance.mainPlayer != null && PvpPlayerMgr.Instance.mainPlayer.unit != null)
            {
                PvpPlayerMgr.Instance.mainPlayer.unit.ReqStopMove();
            }
            startPosition = Vector3.positiveInfinity;
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
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.PvpPlayerUnitDie, OnPvpPlayerUnitDie);
        }

        public override void OnExit()
        {
            GlobalEventDispatcher.Instance.RemoveEvent(GameEvent.PvpPlayerUnitDie, OnPvpPlayerUnitDie);
            base.OnExit();
        }

        private void OnPvpPlayerUnitDie(object args)
        {
            startPosition = Vector3.positiveInfinity;
        }

        public override void OnUpdate()
        {
            m_txtFps.text = FPSMono.Instance.realFPS.ToString();
        }
    }
}

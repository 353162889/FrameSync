using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTCore;
using Framework;
using NodeEditor;

namespace Game
{
    public class SK_PointMoveActionData
    {
        [NEProperty("触发时间", true)]
        public FP time;
        [NEProperty("移动路径点")]
        [NEPropertyBtn("打开路径编辑窗口", "NENodeFuncExt", "ShowPathEditorWindow")]
        public TSVector[] points;
    }
    [SkillNode(typeof(SK_PointMoveActionData))]
    public class SK_PointMoveAction : BaseTimeLineSkillAction
    {
        private SK_PointMoveActionData m_cPointMoveData;
        private bool m_moveEnd;
        public override FP time
        {
            get
            {
                return m_cPointMoveData.time;
            }
        }

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cPointMoveData = data as SK_PointMoveActionData;
        }

        protected override void OnEnter(SkillBlackBoard blackBoard)
        {
            m_moveEnd = false;
            if(m_cPointMoveData.points == null || m_cPointMoveData.points.Length == 0)
            {
                m_moveEnd = true;
            }
            if(!m_moveEnd)
            {
                
                if(blackBoard.host.agentType == AgentObjectType.Unit)
                {
                    Unit unit = (Unit)blackBoard.host.agent;
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = m_cPointMoveData.points[0];
                    for (int i = 0; i < m_cPointMoveData.points.Length; i++)
                    {
                        lst.Add(unit.curPosition + m_cPointMoveData.points[i] - firstPoint);
                    }
                    unit.Move(lst,MoveFromType.Skill);
                    unit.OnUnitMoveStop += OnOneMoveStop;
                    unit.OnUnitMoveStart += OnOneMoveStop;
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                }
                else
                {
                    m_moveEnd = true;
                }
               
            }
            base.OnEnter(blackBoard);
        }

        private void OnOneMoveStop(TSVector position, TSVector forward)
        {
            m_moveEnd = true;
        }

        public override BTActionResult OnRun(SkillBlackBoard blackBoard)
        {
            if(m_moveEnd)
            {
                return BTActionResult.Ready;
            }
            else
            {
                if(blackBoard.host.agentType == AgentObjectType.Unit)
                {
                    Unit unit = (Unit)blackBoard.host.agent;
                    if (!unit.isMoving) return BTActionResult.Ready;
                }
                return BTActionResult.Running;
            }
        }

        public override void OnExit(SkillBlackBoard blackBoard)
        {
            if (blackBoard.host.agentType == AgentObjectType.Unit)
            {
                Unit unit = (Unit)blackBoard.host.agent;
                unit.OnUnitMoveStop -= OnOneMoveStop;
                unit.OnUnitMoveStart -= OnOneMoveStop;
            }
            base.OnExit(blackBoard);
        }
    }
}

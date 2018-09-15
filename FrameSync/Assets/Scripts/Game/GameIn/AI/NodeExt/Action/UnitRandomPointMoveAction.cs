using UnityEngine;
using System.Collections;
using Framework;
using NodeEditor;
using BTCore;
using System.Collections.Generic;

namespace Game
{
    public class UnitRandomPointMoveActionData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEPropertyBtn("打开刷新点编辑器", "NENodeFuncExt", "ShowPointEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] points;
    }

    [AINode(typeof(UnitRandomPointMoveActionData))]
    [NENodeDesc("随机点移动")]
    public class UnitRandomPointMoveAction : BaseTimeLineAIAction
    {
        private UnitRandomPointMoveActionData m_cPointsData;
        public override FP time
        {
            get
            {
                return m_cPointsData.time;
            }
        }

        private Unit m_cUnit;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cPointsData = data as UnitRandomPointMoveActionData;
        }

        protected override void OnEnter(AIBlackBoard blackBoard)
        {
            m_cUnit = (Unit)blackBoard.host.agent;
            if (m_cUnit == null) return;
            if (m_cPointsData.points == null || m_cPointsData.points.Length <= 0) return;
            RandomMove();
            base.OnEnter(blackBoard);
        }

        private void RandomMove()
        {
            int idx = GameInTool.Random(m_cPointsData.points.Length);
            m_cUnit.Move(m_cPointsData.points[idx], MoveFromType.Game);
        }

        public override void OnExit(AIBlackBoard blackBoard)
        {
            m_cUnit = null;
            base.OnExit(blackBoard);
        }

        public override BTActionResult OnRun(AIBlackBoard blackBoard)
        {
            if (m_cUnit == null)
            {
                return BTActionResult.Ready;
            }
            if (!m_cUnit.isMoving)
            {
                RandomMove();
            }
            return BTActionResult.Running;
        }

    }

}
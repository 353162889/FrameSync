using UnityEngine;
using System.Collections;
using Framework;
using NodeEditor;
using BTCore;
using System.Collections.Generic;

namespace Game
{
    public class UnitMovePathActionData
    {
        [NEProperty("触发时间")]
        public FP time;
        [NEPropertyBtn("打开路径编辑器", "NENodeFuncExt", "ShowPathEditorWindow")]
        [NEProperty("移动路径")]
        public TSVector[] path;
        [NEProperty("移动完成后是否移动到最开始的位置，再继续移动")]
        public bool bLoop;
    }

    [AINode(typeof(UnitMovePathActionData))]
    [NENodeDesc("单位按照路径移动")]
    public class UnitMovePathAction : BaseTimeLineAIAction
    {
        private UnitMovePathActionData m_cMovePathData;
        public override FP time
        {
            get
            {
                return m_cMovePathData.time;
            }
        }

        private Unit m_cUnit;
        private TSVector m_sFirstPosition;

        protected override void OnInitData(object data)
        {
            base.OnInitData(data);
            m_cMovePathData = data as UnitMovePathActionData;
        }

        protected override void OnEnter(AIBlackBoard blackBoard)
        {
            var path = m_cMovePathData.path;
            if (path != null)
            {
                m_cUnit = (Unit)blackBoard.host.agent;
                if (m_cUnit == null) return;
                if (path.Length > 1)
                {
                    var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                    var firstPoint = path[0];
                    for (int i = 1; i < path.Length; i++)
                    {
                        lst.Add(m_cUnit.curPosition + path[i] - firstPoint);
                    }
                    m_sFirstPosition = firstPoint;
                    m_cUnit.Move(lst, MoveFromType.Game);
                    ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                }
            }
            base.OnEnter(blackBoard);
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
            if(!m_cUnit.isMoving)
            {
                if (!m_cMovePathData.bLoop)
                {
                    return BTActionResult.Ready;
                }
                else
                {
                    var path = m_cMovePathData.path;
                    if (path.Length > 1)
                    {
                        var lst = ResetObjectPool<List<TSVector>>.Instance.GetObject();
                        lst.Add(m_sFirstPosition);
                        var firstPoint = m_sFirstPosition;
                        for (int i = 1; i < path.Length; i++)
                        {
                            lst.Add(m_cUnit.curPosition + path[i] - firstPoint);
                        }
                        m_cUnit.Move(lst, MoveFromType.Game);
                        ResetObjectPool<List<TSVector>>.Instance.SaveObject(lst);
                    }
                    else
                    {
                        return BTActionResult.Ready;
                    }
                }
            }
            return BTActionResult.Running;
        }

    }

}